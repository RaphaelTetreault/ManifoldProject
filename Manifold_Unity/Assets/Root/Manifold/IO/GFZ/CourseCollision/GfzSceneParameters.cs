using GameCube.GFZ;
using GameCube.GFZ.CourseCollision;
using UnityEngine;

namespace Manifold.IO.GFZ.CourseCollision
{
    /// <summary>
    /// Component class which stores general information regarding the scene.
    /// </summary>
    public class GfzSceneParameters : MonoBehaviour
    {
        [Header("Course Details")]
        public Venue venue;
        public string courseName;
        public int courseIndex; // TODO: validate export venue to index

        [Header("Fog")]
        public bool exportCustomFog = false;
        public FogType fogInterpolation;
        public float fogNear;
        public float fogFar;
        public Color32 color;
        [Header("Fog Curve")]
        public UnityEngine.AnimationCurve fogCurveR;
        public UnityEngine.AnimationCurve fogCurveG;
        public UnityEngine.AnimationCurve fogCurveB;
    }
}
