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

        public Checkpoint[] GetCheckpoints()
        {
            //
            var segmentLength = segment.GetSegmentLength();
            var numCheckpoints = Mathf.CeilToInt(segmentLength / metersPerCheckpoint);
            var checkpoints = new Checkpoint[numCheckpoints];

            var distanceOffset = 0f;

            //
            var animTransform = segment.AnimTransform;
            var curveMaxTime = animTransform.GetMaxTime();
            var position = animTransform.Position;
            var scale = animTransform.Scale;

            for (int i = 0; i < numCheckpoints; i++)
            {
                // Curve-sampling start and end times.
                double checkpointTimeStart = (double)(i + 0) / numCheckpoints;
                double checkpointTimeEnd   = (double)(i + 1) / numCheckpoints;

                // Get origin of start plane, track width at start sampling point
                var origin = position.Evaluate(checkpointTimeStart);
                var trackWidth = scale.x.EvaluateNormalized((float)checkpointTimeStart);

                // DISTANCE
                // Compute the distance between these 2 points, keep track of total distance travelled along segment
                var distanceBetween = animTransform.GetDistanceBetweenRepeated(checkpointTimeStart, checkpointTimeEnd);
                var distanceStart = distanceOffset;
                var distanceEnd = distanceOffset + distanceBetween;
                distanceOffset = distanceEnd;

                // CHECKPOINT
                // Construct start portion of checkpoint
                checkpoints[i] = new Checkpoint();
                var checkpoint = checkpoints[i];
                //
                checkpoint.curveTimeStart = (float)checkpointTimeStart;
                checkpoint.startDistance = distanceStart;
                checkpoint.endDistance = distanceEnd;
                checkpoint.trackWidth = trackWidth;
                checkpoint.connectToTrackIn = true;
                checkpoint.connectToTrackOut = true;
                checkpoint.planeStart.origin = origin;
                // Manually construct normal by sampling position + barely forward along segment
                var to = position.Evaluate(checkpointTimeStart + 0.000001);
                var direction = to - origin;
                checkpoint.planeStart.normal = direction.normalized;
                checkpoint.planeStart.ComputeDotProduct();
            }

            // Copy values from one checkpoints to the previous ones
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
                var origin = position.Evaluate(curveMaxTime);
                // Manually construct normal by sampling position + barely behind along segment (can't use forward here!)
                var from = position.Evaluate(curveMaxTime - 0.000001);
                var direction = from - origin;
                lastCheckpoint.planeEnd.normal = direction.normalized;
                lastCheckpoint.planeEnd.origin = position.Evaluate(curveMaxTime);
                lastCheckpoint.planeEnd.ComputeDotProduct();
            }

            //// REQUIRE TO FEED TO GAME COORD-SPACE
            //foreach (var checkpoint in checkpoints)
            //{
            //    checkpoint.planeStart.normal.z = -checkpoint.planeStart.normal.z;
            //    checkpoint.planeStart.origin.z = -checkpoint.planeStart.origin.z;
            //    //
            //    checkpoint.planeEnd.normal.z = -checkpoint.planeEnd.normal.z;
            //    checkpoint.planeEnd.origin.z = -checkpoint.planeEnd.origin.z;
            //}

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
                var checkpoints = GetCheckpoints();

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
