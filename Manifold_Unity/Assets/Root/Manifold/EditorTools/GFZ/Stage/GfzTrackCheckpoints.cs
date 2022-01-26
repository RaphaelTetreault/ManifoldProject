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
            // TODO: make 100% sure you feed checkpoints in order!

            //
            var segmentLength = segment.GetSegmentLength();
            var numCheckpoints = Mathf.CeilToInt(segmentLength / metersPerCheckpoint);
            var checkpointStride = segmentLength / numCheckpoints;

            //
            var increment = 1f / numCheckpoints;
            for (float p = 0; p < 1f; p += increment)
            {
                var pos = segment.Position.EvaluateNormalized(p);
                var rot = segment.Rotation.EvaluateNormalized(p);
                var scl = segment.Scale.EvaluateNormalized(p);

                Gizmos.DrawCube(pos, scl);
            }

            throw new System.NotImplementedException();
        }

        private void OnDrawGizmos()
        {
            // Use exported data for vis!
            //var checkpoints = GetCheckpoints();
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
