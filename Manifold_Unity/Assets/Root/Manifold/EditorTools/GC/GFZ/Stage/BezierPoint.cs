using UnityEngine;
using UnityEditor;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    [System.Serializable]
    public struct BezierPoint
    {
        public Vector3 position;
        public Vector3 inTangent;
        public Vector3 outTangent;
        public BezierControlPointMode tangentMode;
        public float width;
        public float height;
        public float roll;
        public AnimationUtility.TangentMode widthTangentMode;
        public AnimationUtility.TangentMode heightTangentMode;
        public AnimationUtility.TangentMode rollTangentMode;

        public Vector3 InTangentLocal => inTangent - position;
        public Vector3 OutTangentLocal => outTangent - position;
    }
}
