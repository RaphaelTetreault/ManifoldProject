using GameCube.GFZ.Stage;
using Unity.Mathematics;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public static class CheckpointUtility
    {
        /// <summary>
        /// The difference: if key time is embedded distance, sample time = sample distance
        /// </summary>
        /// <param name="trackSegment">The track segment to generate checkpoints for.</param>
        /// <param name="isGfzCoordinateSpace">Coordinate space is for game.</param>
        /// <returns></returns>
        public static Checkpoint[] CreateCheckpoints(GfzCheckpointGenerator gfzCheckpointGenerator, bool isGfzCoordinateSpace)
        {
            // Simplify access
            GfzTrackSegmentNode trackSegmentNode = gfzCheckpointGenerator.TrackSegmentNode;
            float metersPerCheckpoint = gfzCheckpointGenerator.MetersPerCheckpoint;

            // Get the hierarchy to sample matrices from
            var hacTRS = trackSegmentNode.CreateHierarchichalAnimationCurveTRS(isGfzCoordinateSpace);
            // Length is max key time from root segment
            double segmentLength = hacTRS.GetRoot().GetMaxTime();
            // Get the distance where this segment begins
            var rootNode = trackSegmentNode.GetRoot();
            float checkpointDistanceOffset = rootNode.GetDistanceOffset();
            // Figure out how many checkpoints we need to make
            int numCheckpoints = (int)math.ceil(segmentLength / metersPerCheckpoint);
            var checkpoints = new Checkpoint[numCheckpoints];

            // Get direction vectors
            float3 forward = isGfzCoordinateSpace ? new float3(0, 0, -1) : new float3(0, 0, +1);
            float3 backward = -forward;

            for (int i = 0; i < numCheckpoints; i++)
            {
                // Curve-sampling start and end times. Normalized time: 0 through 1 (inclusive, exclusive).
                double checkpointTimeStart = (double)(i + 0) / numCheckpoints;
                double checkpointTimeEnd = (double)(i + 1) / numCheckpoints;
                // Get distanced for checkpoint
                float distanceStart = (float)(checkpointTimeStart * segmentLength);
                float distanceEnd = (float)(checkpointTimeEnd * segmentLength);

                // Evaluate matrix hierarchy using normalized time
                var matrix = hacTRS.EvaluateHierarchyMatrix(distanceStart);
                var position = matrix.Position();
                var rotation = matrix.Rotation();
                var scale = matrix.Scale();

                // Get origin of start plane, track width at start sampling point
                var origin = position;// + transform.position;
                var trackWidth = scale.x;
                var normal = rotation * forward;
                // Create plane
                var planeStart = new Plane() { origin = origin, normal = normal };
                planeStart.ComputeDotProduct();

                // CHECKPOINT
                checkpoints[i] = new Checkpoint();
                var checkpoint = checkpoints[i];
                checkpoint.CurveTimeStart = distanceStart;
                checkpoint.StartDistance = checkpointDistanceOffset + distanceStart;
                checkpoint.EndDistance = checkpointDistanceOffset + distanceEnd;
                checkpoint.TrackWidth = trackWidth;
                checkpoint.ConnectToTrackIn = true;
                checkpoint.ConnectToTrackOut = true;
                checkpoint.PlaneStart = planeStart;
                // We construct (copy) the checkpoint.planeEnd later
            }

            // Copy values from one checkpoint to the previous one
            // NOTE: start at second index '1' since we refer to the previous checkpoint (i-1)
            for (int i = 1; i < checkpoints.Length; i++)
            {
                var prevCheckpoint = checkpoints[i - 1];
                var currCheckpoint = checkpoints[i];
                // Copy over values
                prevCheckpoint.CurveTimeEnd = currCheckpoint.CurveTimeStart;
                prevCheckpoint.PlaneEnd = currCheckpoint.PlaneStart.GetMirror();
            }

            // Index for last checkpoint
            var lastIndex = checkpoints.Length - 1;

            // Missing information in last checkpoint about end plane
            {
                var lastCheckpoint = checkpoints[lastIndex];
                lastCheckpoint.CurveTimeEnd = (float)segmentLength;

                var matrix = hacTRS.EvaluateHierarchyMatrix(segmentLength);
                var origin = matrix.GetPosition();
                var normal = matrix.rotation * backward;

                var endPlane = new Plane() { origin = origin, normal = normal };
                endPlane.ComputeDotProduct();

                lastCheckpoint.PlaneEnd = endPlane;
            }

            // Make sure the metadata used after this point exists. Provide robust reason if it fails.
            bool prevNodeIsNull = rootNode.Prev == null;
            bool nextNodeIsNull = rootNode.Next == null;
            if (prevNodeIsNull || nextNodeIsNull)
            {
                var msg =
                    $"Track segment's auto-assigned prev or next segments are null! " +
                    $"Prev is null == {prevNodeIsNull}; Next is null == {nextNodeIsNull}. " +
                    $"Try force-reloading references on {nameof(GfzTrack)} object.";
                throw new System.NullReferenceException();
            }

            // Get hacTRS of previous and next nodes
            var fromHacTRS = rootNode.Prev.CreateHierarchichalAnimationCurveTRS(isGfzCoordinateSpace);
            var toHacTRS = rootNode.Next.CreateHierarchichalAnimationCurveTRS(isGfzCoordinateSpace);
            // Set segment in/out connections
            var connectToTrackIn = IsContinuousBetweenFromTo(fromHacTRS, hacTRS);
            var connectToTrackOut = IsContinuousBetweenFromTo(hacTRS, toHacTRS);
            // Assign to appropriate checkpoints
            checkpoints[0].ConnectToTrackIn = connectToTrackIn;
            checkpoints[lastIndex].ConnectToTrackOut = connectToTrackOut;

            // That's all!
            return checkpoints;
        }

        public static bool IsContinuousBetweenFromTo(HierarchichalAnimationCurveTRS from, HierarchichalAnimationCurveTRS to)
        {
            var endMaxTime = from.GetMaxTime();
            var endMatrix = from.EvaluateHierarchyMatrix(endMaxTime);
            var startMatrix = to.EvaluateHierarchyMatrix(0);

            var startPosition = startMatrix.Position();
            var endPosition = endMatrix.Position();

            bool isContinuousBetween = math.distance(endPosition, startPosition) < 0.01f; // 1cm
            return isContinuousBetween;
        }

    }
}
