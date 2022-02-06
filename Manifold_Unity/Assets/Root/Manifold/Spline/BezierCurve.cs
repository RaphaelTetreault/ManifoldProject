using UnityEngine;
using Manifold.Spline;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    [System.Serializable]
    public struct BezierPoint
    {
        [ReadOnlyGUI] public BezierControlPointMode tangentMode;
        [ReadOnlyGUI] public Vector3 position;
        [ReadOnlyGUI] public Vector3 inTangent;
        [ReadOnlyGUI] public Vector3 outTangent;
        [ReadOnlyGUI] public float width;
        [ReadOnlyGUI] public float height;
        [ReadOnlyGUI] public float roll;
    }

}
