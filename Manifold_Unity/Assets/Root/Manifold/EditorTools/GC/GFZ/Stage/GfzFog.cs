using GameCube.GFZ.Stage;
using System;
using Unity.Mathematics;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public class GfzFog : MonoBehaviour
    {
        public enum FogExportMode
        {
            None,
            CustomFog,
            CustomFogCurves,
            SceneVenue,
            SelectVenue,
        }

        // Custom export things
        [SerializeField] private FogExportMode mode;
        [SerializeField] private GfzSceneParameters sceneParameters;
        [SerializeField] private Venue venue;
        // Fog general
        [SerializeField] private FogType interpolation;
        [SerializeField] private float near;
        [SerializeField] private float far;
        [SerializeField] private Color color;
        // Fog curves
        [SerializeField] private UnityEngine.AnimationCurve curveNear;
        [SerializeField] private UnityEngine.AnimationCurve curveFar;
        [SerializeField] private UnityEngine.AnimationCurve curveR;
        [SerializeField] private UnityEngine.AnimationCurve curveG;
        [SerializeField] private UnityEngine.AnimationCurve curveB;
        [SerializeField] private UnityEngine.AnimationCurve curveA;


        public FogExportMode Mode { get => mode; set => mode = value; }
        public GfzSceneParameters SceneParameters { get => sceneParameters; set => sceneParameters = value; }
        public Venue Venue { get => venue; set => venue = value; }
        public FogType Interpolation { get => interpolation; set => interpolation = value; }
        public float Near { get => near; set => near = value; }
        public float Far { get => far; set => far = value; }
        public Color Color { get => color; set => color = value; }
        public UnityEngine.AnimationCurve CurveNear { get => curveNear; set => curveNear = value; }
        public UnityEngine.AnimationCurve CurveFar { get => curveFar; set => curveFar = value; }
        public UnityEngine.AnimationCurve CurveR { get => curveR; set => curveR = value; }
        public UnityEngine.AnimationCurve CurveG { get => curveG; set => curveG = value; }
        public UnityEngine.AnimationCurve CurveB { get => curveB; set => curveB = value; }
        public UnityEngine.AnimationCurve CurveA { get => curveA; set => curveA = value; }


        private Fog CreateFogFromParameters()
        {
            // Create fog from parameters
            var fog = new Fog();
            fog.FogRange = new ViewRange(near, far);
            fog.ColorRGB = new float3(color.r, color.g, color.b);
            return fog;
        }
        private FogCurves CreateFogCurvesFromParameters()
        {
            var fogCurves = new FogCurves();

            // Assign values
            fogCurves.FogCurveNear = curveNear.ToGfz();
            fogCurves.FogCurveFar = curveFar.ToGfz();
            fogCurves.FogCurveR = curveR.ToGfz();
            fogCurves.FogCurveG = curveG.ToGfz();
            fogCurves.FogCurveB = curveB.ToGfz();
            fogCurves.FogCurveUnk = curveA.ToGfz();
            // Create empty curve with 1 keyable
            //var keyables5 = new KeyableAttribute[] { new KeyableAttribute() };
            //fogCurves.FogCurveUnk = new GameCube.GFZ.Stage.AnimationCurve(keyables5);

            return fogCurves;
        }

        public Fog ToGfzFog()
        {
            switch (mode)
            {
                case FogExportMode.None:
                    {
                        // Get fog for Mute City which does not actually have anything
                        var fog = Fog.GetFogDefault(Venue.MuteCity);
                        return fog;
                    }
                case FogExportMode.CustomFog:
                    {
                        // Construct fog from variables
                        var fog = CreateFogFromParameters();
                        return fog;
                    }
                case FogExportMode.CustomFogCurves:
                    {
                        // Construct fog from initial curve values
                        float r = curveR.Evaluate(0);
                        float g = curveR.Evaluate(0);
                        float b = curveR.Evaluate(0);
                        var fog = new Fog
                        {
                            Interpolation = interpolation,
                            FogRange = new ViewRange(near, far),
                            ColorRGB = new float3(r, g, b),
                        };
                        return fog;
                    }
                case FogExportMode.SceneVenue:
                    {
                        // Fallback value is Mute City which has no fog
                        var venue = Venue.MuteCity;
                        // If not null, get venue from SceneParameters
                        if (sceneParameters != null)
                            venue = sceneParameters.venue;

                        var fog = Fog.GetFogDefault(venue);
                        return fog;
                    }
                case FogExportMode.SelectVenue:
                    {
                        // Get fog for selected
                        var fog = Fog.GetFogDefault(venue);
                        return fog;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        public FogCurves ToGfzFogCurves()
        {
            switch (mode)
            {
                case FogExportMode.None:
                    {
                        return null;
                    }

                case FogExportMode.CustomFog:
                case FogExportMode.SceneVenue:
                case FogExportMode.SelectVenue:
                    {
                        // Use default fog parameters for venue
                        var defaultFog = Fog.GetFogDefault(venue);
                        var fogCurves = defaultFog.ToFogCurves(defaultFog);
                        return fogCurves;
                    }

                case FogExportMode.CustomFogCurves:
                    {
                        // Create fog from animation curve parameters
                        var fogCurves = CreateFogCurvesFromParameters();
                        return fogCurves;
                    }


                default:
                    throw new NotImplementedException();
            }
        }

        private void Reset()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            if (sceneParameters == null)
                sceneParameters = FindObjectOfType<GfzSceneParameters>();
        }

    }
}
