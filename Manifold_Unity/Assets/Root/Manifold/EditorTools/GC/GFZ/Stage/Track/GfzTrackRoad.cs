using GameCube.GFZ.CourseCollision;
using Manifold.EditorTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{

    public class GfzTrackRoad : GfzSegmentShape
    {
        [Header("Road Properties")]
        [Min(0f)]
        [SerializeField]
        private float railHeightLeft = 5f;

        [Min(0f)]
        [SerializeField]
        private float railHeightRight = 5f;


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            var mesh = Resources.GetBuiltinResource<Mesh>(EditorTools.Const.Resources.Cube);

            var increment = 1f / 100f;
            for (float p = 0; p < 1f; p += increment)
            {
                var pos = AnimationCurveTransform.Position.Evaluate(p);
                var rot = AnimationCurveTransform.Rotation.Evaluate(p);
                var scl = AnimationCurveTransform.Scale.Evaluate(p);

                Gizmos.DrawMesh(mesh, 0, pos, Quaternion.Euler(rot), scl);
            }
        }

        public override Mesh[] GenerateMeshes()
        {
            throw new System.NotImplementedException();
        }

        public override TrackSegment GenerateTrackSegment()
        {
            var trackSegment = segment.GetSegment();

            // Override the rail properies
            Assert.IsTrue(trackSegment.segmentType == TrackSegmentType.IsTransformLeaf);

            // Rail height
            trackSegment.railHeightLeft = railHeightLeft;
            trackSegment.railHeightRight = railHeightRight;
            // Falgs for rail height
            if (railHeightLeft > 0f)
                trackSegment.perimeterFlags |= TrackPerimeterFlags.hasRailHeightLeft;
            if (railHeightRight > 0f)
                trackSegment.perimeterFlags |= TrackPerimeterFlags.hasRailHeightRight;

            //
            return trackSegment;
        }

    }
}
