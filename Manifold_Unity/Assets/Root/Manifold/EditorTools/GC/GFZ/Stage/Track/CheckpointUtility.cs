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
        public static Checkpoint[] CreateCheckpoints(GfzTrackSegmentNode trackSegment, float checkpointDistanceOffset, float metersPerCheckpoint, bool isGfzCoordinateSpace)
        {
            var hacTRS = trackSegment.CreateHierarchichalAnimationCurveTRS(isGfzCoordinateSpace);

            // Set up info about checkpoints
            var segmentLength = hacTRS.ComputeApproximateLength(0, 1);
            var numCheckpoints = (int)math.ceil(segmentLength / metersPerCheckpoint);
            var checkpoints = new Checkpoint[numCheckpoints];

            // Get direction vectors
            float3 forward = isGfzCoordinateSpace ? new float3(0, 0, -1) : new float3(0, 0, +1);
            float3 backward = -forward;

            for (int i = 0; i < numCheckpoints; i++)
            {
                // Curve-sampling start and end times. Normalized time: 0 through 1 (inclusive on both ends).
                double checkpointTimeStart = (double)(i + 0) / numCheckpoints;
                double checkpointTimeEnd = (double)(i + 1) / numCheckpoints;
                // Get distanced for checkpoint
                float distanceStart = (float)(checkpointTimeStart * segmentLength);
                float distanceEnd = (float)(checkpointTimeEnd * segmentLength);

                // Evaluate matrix hierarchy using normalized time
                var matrix = hacTRS.EvaluateHierarchyMatrixNormalized(checkpointTimeStart);
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
                checkpoint.CurveTimeStart = (float)(checkpointTimeStart * segmentLength);
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

                var matrix = hacTRS.EvaluateHierarchyMatrixNormalized(1);
                var origin = matrix.GetPosition();
                var normal = matrix.rotation * backward;

                var endPlane = new Plane() { origin = origin, normal = normal };
                endPlane.ComputeDotProduct();

                lastCheckpoint.PlaneEnd = endPlane;
            }

            // Set segment in/out connections
            var connectToTrackIn = trackSegment.IsContinuousFromPrevious();
            var connectToTrackOut = trackSegment.IsContinuousToNext();
            checkpoints[0].ConnectToTrackIn = connectToTrackIn;
            checkpoints[lastIndex].ConnectToTrackOut = connectToTrackOut;

            // That's all!
            return checkpoints;
        }


    }
}
