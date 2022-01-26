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
    }
}
