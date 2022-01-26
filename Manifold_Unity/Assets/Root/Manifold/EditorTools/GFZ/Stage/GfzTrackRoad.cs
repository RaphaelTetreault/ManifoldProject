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

            var increment = 1f / 1000f;
            for (float p = 0; p < 1f; p += increment)
            {
                var pos = position.EvaluateNormalized(p);
                var rot = rotation.EvaluateNormalized(p);
                var scl = scale.EvaluateNormalized(p);

                Gizmos.DrawCube(pos, scl);
            }
        }

        public override Mesh[] GenerateMeshes()
        {
            throw new System.NotImplementedException();
        }

        public override void InitTrackSegment()
        {
            throw new System.NotImplementedException();
        }

        public override float GetSegmentLength()
        {
            throw new System.NotImplementedException();
        }

    }
}
