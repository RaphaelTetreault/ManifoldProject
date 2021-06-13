using GameCube.GFZ;
using GameCube.GFZ.CourseCollision;
using Unity.Mathematics;
using UnityEngine;

namespace Manifold.IO.GFZ.CourseCollision
{
    /// <summary>
    /// Component class which stores general information regarding the scene.
    /// </summary>
    public class GfzSceneParameters : MonoBehaviour
        //IGfzConvertable<Fog>,
        //IGfzConvertable<FogCurves>
    {
        [Header("Course Details")]
        public Venue venue; // TODO: change to VenueName, make indexer helper
        public string courseName;
        public int courseIndex; // TODO: validate export venue to index

        [Header("Fog")]
        public bool exportCustomFog = false;
        public FogType fogInterpolation;
        public float fogNear;
        public float fogFar;
        public Color32 color;
        [Header("Fog Curve")]
        public UnityEngine.AnimationCurve fogCurveNear;
        public UnityEngine.AnimationCurve fogCurveFar;
        public UnityEngine.AnimationCurve fogCurveR;
        public UnityEngine.AnimationCurve fogCurveG;
        public UnityEngine.AnimationCurve fogCurveB;

        private Fog CreateCustomFog()
        {
            // Create fog from parameters
            var fog = new Fog();
            fog.fogRange = new Range(fogNear, fogFar);
            fog.colorRGB = new float3(color.r, color.g, color.b);
            fog.zero0x18 = new float3();
            return fog;
        }
        private FogCurves CreateCustomFogCurves()
        {
            var fogCurves = new FogCurves();

            // Assign values
            fogCurves.FogCurveNear = fogCurveNear.ToGfz();
            fogCurves.FogCurveFar = fogCurveFar.ToGfz();
            fogCurves.FogCurveR = fogCurveR.ToGfz();
            fogCurves.FogCurveG = fogCurveG.ToGfz();
            fogCurves.FogCurveB = fogCurveB.ToGfz();
            // Create empty curve with 1 keyable
            fogCurves.FogCurveUnk = new GameCube.GFZ.CourseCollision.AnimationCurve(1);

            return fogCurves;
        }


        public string GetGfzInternalName()
        {
            return $"COLI_COURSE{courseIndex:00}";
        }

        public string GetGfzDisplayName()
        {
            var venueName = EnumExtensions.GetDescription(venue);
            return $"{venueName} [{courseName}]";
        }

        public Fog ToGfzFog()
        {
            if (exportCustomFog)
            {
                // Create fog from parameters
                return CreateCustomFog();
            }
            else
            {
                // Use default fog parameters for venue
                return Fog.GetFogDefault(venue);
            }
        }

        public FogCurves ToGfzFogCurves()
        {
            if (exportCustomFog)
            {
                // Create fog from animation curve parameters
                return CreateCustomFogCurves();
            }
            else
            {
                // Use default fog parameters for venue
                var defaultFog = Fog.GetFogDefault(venue);
                var fogCurves = defaultFog.ToFogCurves(defaultFog);
                return fogCurves;
            }
        }

    }
}
