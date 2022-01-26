using GameCube.GFZ.CourseCollision;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public class GfzTrackRoad : GfzTrackSegment
    {
        [Header("Road Properties")]
        [Min(0f)]
        [SerializeField]
        private float railHeightLeft = 5f;

        [Min(0f)]
        [SerializeField]
        private float railHeightRight = 5f;

        //private readonly Vector3 gizmosScale = Vector3.one;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            var increment = 1f / 500f;
            for (float p = 0; p < 1f; p += increment)
            {
                var pos = animTransform.Position.EvaluateNormalized(p);
                var rot = animTransform.Rotation.EvaluateNormalized(p);
                var scl = animTransform.Scale.EvaluateNormalized(p);

                Gizmos.DrawCube(pos, scl);
            }
        }

        public override Mesh[] GenerateMeshes()
        {
            throw new System.NotImplementedException();
        }

        public override TrackSegment GenerateTrackSegment()
        {
            // TODO: get components, build any children

            var trackSegment = new TrackSegment();

            trackSegment.localPosition = Vector3.zero;
            trackSegment.localRotation = Vector3.zero;
            trackSegment.localScale = Vector3.one;

            // Rail height
            trackSegment.railHeightLeft = railHeightLeft;
            trackSegment.railHeightRight = railHeightRight;
            // Falgs for rail height
            if (railHeightLeft > 0f)
                trackSegment.perimeterFlags |= TrackPerimeterFlags.hasRailHeightLeft;
            if (railHeightRight > 0f)
                trackSegment.perimeterFlags |= TrackPerimeterFlags.hasRailHeightRight;

            // TODO: currently hardcoded
            trackSegment.segmentType = TrackSegmentType.IsTransformLeaf;

            trackSegment.trackCurves = animTransform.ToTrackCurves();

            return trackSegment;
        }

    }
}
