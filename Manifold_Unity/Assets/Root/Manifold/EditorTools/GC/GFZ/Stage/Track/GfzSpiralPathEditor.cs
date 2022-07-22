using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    [CustomEditor(typeof(GfzSpiralPath))]
    internal class GfzSpiralPathEditor : GfzRootNodeEditor
    {
        SerializedProperty animationCurveTRS;
        SerializedProperty autoGenerateTRS;

        void OnEnable()
        {
            animationCurveTRS = serializedObject.FindProperty(nameof(animationCurveTRS));
            autoGenerateTRS = serializedObject.FindProperty(nameof(autoGenerateTRS));
        }

        public override void OnInspectorGUI()
        {
            var spiralPath = target as GfzSpiralPath;

            serializedObject.Update();
            {
                DrawDefaults(spiralPath);
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

        public void DrawTrsGeneration(GfzSpiralPath spiralPath)
        {
            //var buttonWidths = GUILayout.Width(GuiSimple.GetElementsWidth(3));

            GuiSimple.Label("Generate TRS", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(autoGenerateTRS);

            if (GUILayout.Button("Generate All Components"))
                spiralPath.InvokeUpdates();

            EditorGUILayout.PropertyField(animationCurveTRS);
        }

    }
}
