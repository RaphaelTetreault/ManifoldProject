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
        /// <param name="convertCoordinateSpace">Coordinate space is for game.</param>
        /// <returns></returns>
        public static Checkpoint[] CreateCheckpoints(GfzTrackSegment trackSegment, bool convertCoordinateSpace)
        {
            // Set up info about checkpoints
            var segmentLength = trackSegment.GetSegmentLength();
            var numCheckpoints = (int)math.ceil(segmentLength / trackSegment.MetersPerCheckpoint);
            var checkpoints = new Checkpoint[numCheckpoints];

            // Get direction vectors
            float3 forward = convertCoordinateSpace ? new float3(0, 0, -1) : new float3(0, 0, +1);
            float3 backward = -forward;

            // Get the AnimationCurveTransform appropriate for requester.
            // Use GFZ space (game) if 'true'
            // Use Unity space if 'false'
            var animationTRS = convertCoordinateSpace
                ? trackSegment.AnimationCurveTRS.GetInGfzCoordinateSpace()
                : trackSegment.AnimationCurveTRS;

            // Get matrices, time
            var baseMtx = trackSegment.transform.localToWorldMatrix;
            var curveMaxTime = animationTRS.GetMaxTime(); // is also length

            var checkpointDistanceOffset = trackSegment.GetDistanceOffset();
            for (int i = 0; i < numCheckpoints; i++)
            {
                // Curve-sampling start and end times.
                double checkpointTimeStart = (double)(i + 0) / numCheckpoints;
                double checkpointTimeEnd = (double)(i + 1) / numCheckpoints;
                // Scale normalized time by max
                checkpointTimeStart *= curveMaxTime;
                checkpointTimeEnd *= curveMaxTime;

                // Evaluate matrices
                var animMtx = animationTRS.EvaluateMatrix(checkpointTimeStart);
                var mtx = baseMtx * animMtx;
                var position = mtx.GetPosition();
                var rotation = mtx.rotation;
                var scale = animationTRS.Scale.Evaluate(checkpointTimeStart);

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
                checkpoint.CurveTimeStart = (float)checkpointTimeStart;
                checkpoint.StartDistance = checkpointDistanceOffset + (float)checkpointTimeStart;
                checkpoint.EndDistance = checkpointDistanceOffset + (float)checkpointTimeEnd;
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

            // Complete missing information in last checkpoint of segment
            {
                var lastCheckpoint = checkpoints[lastIndex];
                lastCheckpoint.CurveTimeEnd = curveMaxTime;

                var animMtx = animationTRS.EvaluateMatrix(curveMaxTime);
                var mtx = baseMtx * animMtx;

                var origin = mtx.GetPosition();
                var rotation = mtx.rotation;
                var normal = rotation * backward;

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
