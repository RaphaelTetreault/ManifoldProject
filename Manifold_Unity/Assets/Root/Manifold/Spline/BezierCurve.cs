using UnityEngine;
using Manifold.Spline;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    [System.Serializable]
    public struct BezierPoint
    {
        public BezierControlPointMode mode;
        public Vector3 point;
        public Vector3 inTangent;
        public Vector3 outTangent;
        public float width;
        public float roll;
    }

}
