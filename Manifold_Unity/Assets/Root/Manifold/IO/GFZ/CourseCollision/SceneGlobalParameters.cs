using GameCube.GFZ;
using GameCube.GFZ.CourseCollision;
using UnityEngine;

namespace Manifold.IO.GFZ.CourseCollision
{
    public class SceneGlobalParameters : MonoBehaviour
    {
        public Venue venue;
        public string courseName;

        [Header("Fog")]
        public bool exportCustomFog = false;
        public FogType fogInterpolation;
        public float fogNear;
        public float fogFar;
        public Color32 color;

        public UnityEngine.AnimationCurve fogCurveR;
        public UnityEngine.AnimationCurve fogCurveG;
        public UnityEngine.AnimationCurve fogCurveB;
    }
}
