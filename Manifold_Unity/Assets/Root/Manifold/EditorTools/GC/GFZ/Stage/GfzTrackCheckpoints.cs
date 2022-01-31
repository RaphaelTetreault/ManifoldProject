using GameCube.GFZ.CourseCollision;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public sealed class GfzTrackCheckpoints : MonoBehaviour
    {
        [SerializeField] private GfzTrackSegment segment;
        [Delayed]
        [SerializeField] private float metersPerCheckpoint = 10f;
        [SerializeField] private bool genCheckpoints = true;

        public GfzTrackSegment Segment => segment;

        public Checkpoint[] GetCheckpoints(bool convertCoordinateSpace)
        {
            //
            var segmentLength = segment.GetSegmentLength();
            var numCheckpoints = Mathf.CeilToInt(segmentLength / metersPerCheckpoint);
            var checkpoints = new Checkpoint[numCheckpoints];

            var distanceOffset = 0f;

            // Get the AnimationCurveTransform appropriate for requester.
            // Use GFZ space (game) if 'true'
            // Use Unity space if 'false'
            var animTransform = convertCoordinateSpace
                ? segment.AnimTransform.GetGfzCoordSpaceAnimTransform()
                : segment.AnimTransform;

            var curveMaxTime = animTransform.GetMaxTime();
            var pos = animTransform.Position;
            var rot = animTransform.Rotation;
            var scl = animTransform.Scale;

            for (int i = 0; i < numCheckpoints; i++)
            {
                // Curve-sampling start and end times.
                double checkpointTimeStart = (double)(i + 0) / numCheckpoints;
                double checkpointTimeEnd   = (double)(i + 1) / numCheckpoints;

                // Get origin of start plane, track width at start sampling point
                var origin = pos.Evaluate(checkpointTimeStart);
                var normal = Quaternion.Euler(rot.Evaluate(checkpointTimeStart)) * Vector3.back;
                var trackWidth = scl.x.Evaluate((float)checkpointTimeStart);

                // DISTANCE
                // Compute the distance between these 2 points, keep track of total distance travelled along segment
                var distanceBetween = animTransform.GetDistanceBetweenRepeated(checkpointTimeStart, checkpointTimeEnd);
                var distanceStart = distanceOffset;
                var distanceEnd = distanceOffset + distanceBetween;
                distanceOffset = distanceEnd;

                // CHECKPOINT
                checkpoints[i] = new Checkpoint();
                var checkpoint = checkpoints[i];
                checkpoint.curveTimeStart = (float)checkpointTimeStart;
                checkpoint.startDistance = distanceStart;
                checkpoint.endDistance = distanceEnd;
                checkpoint.trackWidth = trackWidth;
                checkpoint.connectToTrackIn = true;
                checkpoint.connectToTrackOut = true;
                checkpoint.planeStart.origin = origin;
                checkpoint.planeStart.normal = normal;
                checkpoint.planeStart.ComputeDotProduct();
                // We construct (copy) the checkpoint.planeEnd later
            }

            // Copy values from one checkpoint to the previous one
            // NOTE: start at second index '1' since we refer to the previous checkpoint (i-1)
            for (int i = 1; i < checkpoints.Length; i++)
            {
                var prevCheckpoint = checkpoints[i - 1];
                var currCheckpoint = checkpoints[i];
                // Copy over values
                prevCheckpoint.curveTimeEnd = currCheckpoint.curveTimeStart;
                prevCheckpoint.planeEnd = currCheckpoint.planeStart.GetMirror();
            }

            // Complete missing information in last checkpoint of segment
            {
                var lastCheckpoint = checkpoints[checkpoints.Length - 1];
                lastCheckpoint.curveTimeEnd = curveMaxTime;
                var origin = pos.Evaluate(curveMaxTime);
                var normal = Quaternion.Euler(rot.Evaluate(curveMaxTime)) * Vector3.forward;
                lastCheckpoint.planeEnd.origin = origin;
                lastCheckpoint.planeEnd.normal = normal;
                lastCheckpoint.planeEnd.ComputeDotProduct();
            }

            return checkpoints;
        }



        private void Reset()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            if (segment == null)
            {
                segment = GetComponent<GfzTrackSegment>();
            }

            if (genCheckpoints)
            {
                var checkpoints = GetCheckpoints(false);

                int index = 0;
                foreach (var checkpoint in checkpoints)
                {
                    var gobj = new GameObject($"Checkpoint[{index++}]");
                    gobj.transform.parent = this.transform;
                    var script = gobj.AddComponent<GfzCheckpoint>();
                    script.Init(checkpoint);
                }
                genCheckpoints = false;
            }
        }

    }
}
