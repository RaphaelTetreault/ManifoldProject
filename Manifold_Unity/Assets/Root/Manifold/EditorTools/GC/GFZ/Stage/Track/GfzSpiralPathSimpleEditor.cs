using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    [CustomEditor(typeof(GfzSpiralPathSimple))]
    internal class GfzSpiralPathSimpleEditor : Editor
    {
        SerializedProperty value;
        SerializedProperty animationCurveTRS;
        SerializedProperty autoGenerateTRS;

        void OnEnable()
        {
            animationCurveTRS = serializedObject.FindProperty(nameof(animationCurveTRS));
            autoGenerateTRS = serializedObject.FindProperty(nameof(autoGenerateTRS));
        }

        public override void OnInspectorGUI()
        {
            var spiralPath = target as GfzSpiralPathSimple;

            serializedObject.Update();
            {
                DrawUnityEditorDefaults(spiralPath);
                EditorGUILayout.Separator();
                DrawTrsGeneration(spiralPath);
            }
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            // Uncomment if you need to debug something
            base.OnInspectorGUI();
        }

        private void DrawUnityEditorDefaults(GfzSpiralPathSimple spiralPath)
        {
            GuiSimple.DefaultScript("Script", spiralPath);
            GUI.enabled = false;
            EditorGUILayout.ObjectField(nameof(spiralPath.Prev), spiralPath.Prev, typeof(GfzTrackSegmentRootNode), false);
            EditorGUILayout.ObjectField(nameof(spiralPath.Next), spiralPath.Next, typeof(GfzTrackSegmentRootNode), false);
            GUI.enabled = true;
        }


        public void DrawTrsGeneration(GfzSpiralPathSimple spiralPath)
        {
            //var buttonWidths = GUILayout.Width(GuiSimple.GetElementsWidth(3));

            GuiSimple.Label("Generate TRS", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(autoGenerateTRS);

            if (GUILayout.Button("Generate All Components"))
                spiralPath.UpdateTRS();

            EditorGUILayout.PropertyField(animationCurveTRS);
        }

    }
}
