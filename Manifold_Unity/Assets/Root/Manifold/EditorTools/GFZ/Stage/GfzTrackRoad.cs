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

        private readonly Vector3 gizmosScale = Vector3.one;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            var increment = 1f / 1000f;
            for (float p = 0; p < 1f; p += increment)
            {
                var pos = position.Evaluate(p);
                var rot = rotation.Evaluate(p);
                var scl = scale.Evaluate(p);

                Gizmos.DrawCube(pos, scl);
            }
        }
    }
}
