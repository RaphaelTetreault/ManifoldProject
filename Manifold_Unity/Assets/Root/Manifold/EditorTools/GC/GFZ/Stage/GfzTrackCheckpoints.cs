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
            // Make 100% sure you feed checkpoints in order!

            //
            var segmentLength = segment.GetSegmentLength();
            var numCheckpoints = Mathf.CeilToInt(segmentLength / metersPerCheckpoint);

            // 
            var checkpoints = new Checkpoint[numCheckpoints];
            //var checkpointIncrement = 1f / numCheckpoints;
            //
            //var distanceIncrement = checkpointIncrement / 1000f; // 1000 iterations
            const int numDistanceIterations = 1000;
            var distanceOffset = 0f;
            //
            var curveMaxTime = segment.AnimTransform.GetMaxTime();
            var position = segment.AnimTransform.Position;
            var rotation = segment.AnimTransform.Rotation;
            var scale = segment.AnimTransform.Scale;

            for (int ic = 0; ic < numCheckpoints; ic++)
            {
                double currCheckpointTime = (double)ic / numCheckpoints;
                double nextCheckpointDistance = (double)(ic + 1) / numCheckpoints;

                // TRANSFORM
                // Compute transform values
                var pos = position.EvaluateNormalized((float)currCheckpointTime);
                var rot = rotation.EvaluateNormalized((float)currCheckpointTime);
                var scl = scale.EvaluateNormalized((float)currCheckpointTime);

                // DISTANCE
                // Compute distance between last checkpoint and next one.
                // pd = percentage distance
                var distanceStart = distanceOffset;
                var distance = 0f;
                double checkpointTimeDelta = nextCheckpointDistance - currCheckpointTime;
                for (int id = 0; id < numDistanceIterations; id++)
                {
                    var currDistance = currCheckpointTime + checkpointTimeDelta / numDistanceIterations * id;
                    var nextDistance = currCheckpointTime + checkpointTimeDelta / numDistanceIterations * (id + 1);

                    var currPosition = position.EvaluateNormalized((float)currDistance);
                    var nextPosition = position.EvaluateNormalized((float)nextDistance);
                    var delta = Vector3.Distance(currPosition, nextPosition);
                    distance += delta;
                }
                // Append how far we have already traveled to distance
                distance += distanceOffset;
                // Set offset for next iteration
                distanceOffset = distance;
                //
                var distanceEnd = distanceOffset;


                // CHECKPOINT
                checkpoints[ic] = new Checkpoint();
                var checkpoint = checkpoints[ic];
                //
                checkpoint.curveTimeStart = (float)currCheckpointTime;
                checkpoint.startDistance = distanceStart;
                checkpoint.endDistance = distanceEnd;
                // Set plane
                checkpoint.planeStart.origin = pos;
                var from = position.EvaluateNormalized((float)currCheckpointTime);
                var to = position.EvaluateNormalized((float)(currCheckpointTime + 0.0001));
                var direction = to - from;
                checkpoint.planeStart.normal = direction.normalized;
                checkpoint.planeStart.ComputeDotProduct();
                //
                checkpoint.trackWidth = scl.x;
                checkpoint.connectToTrackIn = true;
                checkpoint.connectToTrackOut = true;
            }

            // Copy values from one checkpoints to the previous ones
            // NOTE: start at second index '1'
            for (int i = 1; i < checkpoints.Length; i++)
            {
                var prev = checkpoints[i - 1];
                var curr = checkpoints[i];
                // Copy over values
                prev.curveTimeEnd = curr.curveTimeStart;
                // Plane
                // Tangent of end point inwards towards the first
                prev.planeEnd.origin = curr.planeStart.origin;
                prev.planeEnd.normal = -curr.planeStart.normal;
                prev.planeEnd.dotProduct = -curr.planeStart.dotProduct;
            }
            //
            var firstCheckpoint = checkpoints[0];
            var lastCheckpoint = checkpoints[checkpoints.Length - 1];
            lastCheckpoint.planeEnd = firstCheckpoint.planeStart;

            // trim off last checkpoint, it was generated to gather some data
            //var usedCheckpoints = new TrackCheckpoint[checkpoints.Length - 1];
            //for (int i = 0; i < usedCheckpoints.Length; i++)
            //    usedCheckpoints[i] = checkpoints[i];

            // TODO outside of this function
            // Set checkpoint[0].hasTrackIn based on previous segment
            // Set checkpoint[length-1].hasTrackOut based on next segment

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

                foreach (var child in transform.GetChildren())
                {
                    GameObject.Destroy(child.gameObject);
                }

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
