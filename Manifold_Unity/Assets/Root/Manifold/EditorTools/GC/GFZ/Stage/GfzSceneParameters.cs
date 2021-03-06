using Manifold.IO;
using GameCube.GFZ;
using GameCube.GFZ.Stage;
using Unity.Mathematics;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    /// <summary>
    /// Component class which stores general information regarding the scene.
    /// </summary>
    public class GfzSceneParameters : MonoBehaviour
        //IGfzConvertable<Fog>,
        //IGfzConvertable<FogCurves>
    {
        [Header("About")]
        public string author;

        [Header("Course Details")]
        public Venue venue; // TODO: change to VenueName, make indexer helper
        public string courseName;
        public int courseIndex; // TODO: validate export venue to index
        public CircuitType circuitType = CircuitType.ClosedCircuit; // will become param from control points.
        public Bool32 staticColliderMeshesActive = Bool32.True;



        [Header("Unknown Range")]
        public float rangeNear;
        public float rangeFar;

        [Header("Fog")]
        public bool exportCustomFog = false;
        public FogType fogInterpolation;
        public float fogNear;
        public float fogFar;
        public Color32 color;
        [Header("Fog Curve")]
        public bool exportFogCurves = false;
        public bool exportCustomFogCurves = false;
        public UnityEngine.AnimationCurve fogCurveNear;
        public UnityEngine.AnimationCurve fogCurveFar;
        public UnityEngine.AnimationCurve fogCurveR;
        public UnityEngine.AnimationCurve fogCurveG;
        public UnityEngine.AnimationCurve fogCurveB;
        
        private Fog CreateCustomFog()
        {
            // Create fog from parameters
            var fog = new Fog();
            fog.FogRange = new ViewRange(fogNear, fogFar);
            fog.ColorRGB = new float3(color.r, color.g, color.b);
            //fog.zero0x18 = new float3();
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
            var keyables5 = new KeyableAttribute[] { new KeyableAttribute() };
            fogCurves.FogCurveUnk = new GameCube.GFZ.Stage.AnimationCurve(keyables5);

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
            if (!exportFogCurves)
                return null;

            if (exportCustomFogCurves)
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
