using System;
using UnityEditor;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    [CustomEditor(typeof(GfzFog))]
    internal class GfzFogEditor : Editor
    {
        SerializedProperty sceneParameters;
        SerializedProperty mode;
        SerializedProperty venue;
        SerializedProperty interpolation;
        SerializedProperty near;
        SerializedProperty far;
        SerializedProperty color;
        SerializedProperty curveNear;
        SerializedProperty curveFar;
        SerializedProperty curveR;
        SerializedProperty curveG;
        SerializedProperty curveB;
        SerializedProperty curveA;


        private void OnEnable()
        {
            sceneParameters = serializedObject.FindProperty(nameof(sceneParameters));
            mode = serializedObject.FindProperty(nameof(mode));
            venue = serializedObject.FindProperty(nameof(venue));
            interpolation = serializedObject.FindProperty(nameof(interpolation));
            near = serializedObject.FindProperty(nameof(near));
            far = serializedObject.FindProperty(nameof(far));
            color = serializedObject.FindProperty(nameof(color));
            curveNear = serializedObject.FindProperty(nameof(curveNear));
            curveFar = serializedObject.FindProperty(nameof(curveFar));
            curveR = serializedObject.FindProperty(nameof(curveR));
            curveG = serializedObject.FindProperty(nameof(curveG));
            curveB = serializedObject.FindProperty(nameof(curveB));
            curveA = serializedObject.FindProperty(nameof(curveA));
        }

        public override void OnInspectorGUI()
        {
            var editorTarget = target as GfzFog;
            GuiSimple.DefaultScript("Script", editorTarget);

            serializedObject.Update();
            {
                // Always render mode
                EditorGUILayout.PropertyField(mode);

                // Draw custom inspector based on mode
                var fogExportMode = (GfzFog.FogExportMode)mode.intValue;
                switch (fogExportMode)
                {
                    case GfzFog.FogExportMode.None:
                        DrawFogNone(editorTarget); 
                        break;

                    case GfzFog.FogExportMode.CustomFog:
                        DrawFogCustom(editorTarget); 
                        break;

                    case GfzFog.FogExportMode.CustomFogCurves:
                        DrawFogCustomCurves(editorTarget); 
                        break;

                    case GfzFog.FogExportMode.SceneVenue: 
                        DrawFogSceneVenue(editorTarget); 
                        break;

                    case GfzFog.FogExportMode.SelectVenue: 
                        DrawFogSelectVenue(editorTarget); 
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
            serializedObject.ApplyModifiedProperties();

            //base.OnInspectorGUI();
        }

        private void DrawFogNone(GfzFog editorTarget)
        {
            EditorGUILayout.LabelField("No Fog", EditorStyles.boldLabel);
            const string msg = "No fog will be exported to scene.";
            EditorGUILayout.HelpBox(msg, MessageType.Info);
        }

        private void DrawFogCustom(GfzFog editorTarget)
        {
            EditorGUILayout.LabelField("Custom Fog", EditorStyles.boldLabel);
            const string msg = "Fog curves will be generated using these fog values.";
            EditorGUILayout.HelpBox(msg, MessageType.Info);
            EditorGUILayout.PropertyField(interpolation);
            EditorGUILayout.PropertyField(near);
            EditorGUILayout.PropertyField(far);
            EditorGUILayout.PropertyField(color);
        }
        private void DrawFogCustomCurves(GfzFog editorTarget)
        {
            EditorGUILayout.LabelField("Custom Fog Curves", EditorStyles.boldLabel);
            const string msg = "Construct the fog curves directly.";
            EditorGUILayout.HelpBox(msg, MessageType.Info);
            EditorGUILayout.PropertyField(interpolation);
            EditorGUILayout.PropertyField(curveNear);
            EditorGUILayout.PropertyField(curveFar);
            EditorGUILayout.PropertyField(curveR);
            EditorGUILayout.PropertyField(curveG);
            EditorGUILayout.PropertyField(curveB);
            EditorGUILayout.PropertyField(curveA);
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Custom Fog Fallback Values", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(near);
            EditorGUILayout.PropertyField(far);
            EditorGUILayout.PropertyField(color);
        }
        private void DrawFogSceneVenue(GfzFog editorTarget)
        {
            EditorGUILayout.LabelField("Scene Venue's Fog", EditorStyles.boldLabel);
            string msg =
                $"The venue specified in {nameof(sceneParameters)} will be used to " +
                $"create the appropriate fog.";
            EditorGUILayout.HelpBox(msg, MessageType.Info);
            EditorGUILayout.PropertyField(sceneParameters);
        }
        private void DrawFogSelectVenue(GfzFog editorTarget)
        {
            EditorGUILayout.LabelField("Selected Venue's Fog", EditorStyles.boldLabel);
            string msg =
                $"A template of the fog for the venue specified will be used " +
                $"regardless of the destination venue.";
            EditorGUILayout.HelpBox(msg, MessageType.Info);
            EditorGUILayout.PropertyField(venue);
        }

    }
}
