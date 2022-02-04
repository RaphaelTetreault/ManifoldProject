using UnityEngine;
using Manifold.Spline;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    [System.Serializable]
    public struct BezierPoint
    {
        [ReadOnlyGUI] public BezierControlPointMode mode;
        [ReadOnlyGUI] public Vector3 point;
        [ReadOnlyGUI] public Vector3 inTangent;
        [ReadOnlyGUI] public Vector3 outTangent;
        [ReadOnlyGUI] public float width;
        [ReadOnlyGUI] public float roll;
    }

}
