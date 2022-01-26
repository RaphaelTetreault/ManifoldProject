using GameCube.GFZ.CourseCollision;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public sealed class GfzTrackCheckpoints : MonoBehaviour
    {
        [SerializeField] private GfzTrackSegment segment;
        [SerializeField] private float metersPerCheckpoint = 10f;

        public GfzTrackSegment Segment => segment;

        public TrackCheckpoint[] GetCheckpoints()
        {
            // Make 100% sure you feed checkpoints in order!

            //
            var segmentLength = segment.GetSegmentLength();
            var numCheckpoints = Mathf.CeilToInt(segmentLength / metersPerCheckpoint);

            // 
            var checkpoints = new TrackCheckpoint[numCheckpoints];
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


                // CHECKPOINT
                checkpoints[ic] = new TrackCheckpoint();
                var checkpoint = checkpoints[ic];
                //
                checkpoint.curveTimeStart = (float)currCheckpointTime;
                checkpoint.start.position = pos;
                checkpoint.start.distanceFromStart = 0f;
                checkpoint.start.forward = Quaternion.Euler(rot) * Vector3.forward;
                checkpoint.transformDistanceStart = distance;
                //
                checkpoint.trackWidth = scl.x;
                checkpoint.hasTrackIn = true;
                checkpoint.hasTrackOut = true;
            }

            // Copy values from one checkpoints to the previous ones
            // NOTE: start at first index
            for (int i = 1; i < checkpoints.Length; i++)
            {
                var prevCheckpoint = checkpoints[i - 1];
                var currCheckpoint = checkpoints[i];
                // Copy over values
                prevCheckpoint.curveTimeEnd = currCheckpoint.curveTimeStart;
                prevCheckpoint.end = currCheckpoint.start;
            }

            // trim off last checkpoint, it was generated to gather some data
            var usedCheckpoints = new TrackCheckpoint[checkpoints.Length - 1];
            for (int i = 0; i < usedCheckpoints.Length; i++)
                usedCheckpoints[i] = checkpoints[i];

            // TODO outside of this function
            // Set checkpoint[0].hasTrackIn based on previous segment
            // Set checkpoint[length-1].hasTrackOut based on next segment

            return usedCheckpoints;
        }

        private void OnDrawGizmos()
        {
             var checkpoints = GetCheckpoints();

            //
            Gizmos.color = Color.green;
            var mesh = UnityEditor.AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Root/Resources/normal-cylinder-16-hollowed.fbx");
            
            for (int i = 0; i < checkpoints.Length; i++)
            {
                var checkpoint = checkpoints[i];
                var from = checkpoint.start.position;
                var to = checkpoint.end.position;
                var halfWidth = checkpoint.trackWidth / 2f;
                var scaleFrom = new Vector3(halfWidth, halfWidth, 1f);
                var scaleTo = 2f * (5f) * Vector3.one;

                Gizmos.DrawLine(from, to);
                Gizmos.DrawMesh(mesh, 0, from, Quaternion.LookRotation(checkpoint.start.forward), scaleFrom);
                Gizmos.DrawWireMesh(mesh, 0, to, Quaternion.LookRotation(checkpoint.end.forward), scaleTo);
            }
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
        }

    }
}
