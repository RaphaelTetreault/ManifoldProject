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
    internal class GfzSpiralPathSimpleEditor : GfzRootNodeEditor
    {
        [SerializeField] private static bool nodeInfoFoldout = false;

        SerializedProperty axes;
        SerializedProperty rotationZ;
        SerializedProperty scaleX;
        SerializedProperty scaleY;
        SerializedProperty radius0;
        SerializedProperty radius1;
        SerializedProperty axisOffset;
        SerializedProperty rotateDegrees;
        SerializedProperty keysPer360Degrees;
        SerializedProperty animationCurveTRS;
        SerializedProperty autoGenerateTRS;

        void OnEnable()
        {
            axes = serializedObject.FindProperty(nameof(axes));
            rotationZ = serializedObject.FindProperty(nameof(rotationZ));
            scaleX = serializedObject.FindProperty(nameof(scaleX));
            scaleY = serializedObject.FindProperty(nameof(scaleY));
            radius0 = serializedObject.FindProperty(nameof(radius0));
            radius1 = serializedObject.FindProperty(nameof(radius1));
            axisOffset = serializedObject.FindProperty(nameof(axisOffset));
            rotateDegrees = serializedObject.FindProperty(nameof(rotateDegrees));
            keysPer360Degrees = serializedObject.FindProperty(nameof(keysPer360Degrees));
            animationCurveTRS = serializedObject.FindProperty(nameof(animationCurveTRS));
            autoGenerateTRS = serializedObject.FindProperty(nameof(autoGenerateTRS));
        }

        public override void OnInspectorGUI()
        {
            var spiralPath = target as GfzSpiralPathSimple;

            serializedObject.Update();
            {
                DrawDefaults(spiralPath);
                EditorGUILayout.Separator();
                DrawSourceFields(spiralPath);
                EditorGUILayout.Separator();
                DrawTrsGeneration(spiralPath);
            }
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            // Uncomment if you need to debug something
            //base.OnInspectorGUI();
        }

        public void DrawSourceFields(GfzSpiralPathSimple spiralPath)
        {
            GuiSimple.Label("Source Values", EditorStyles.boldLabel);
            {
                EditorGUI.indentLevel++;
                GuiSimple.Label("Curve", EditorStyles.boldLabel);
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(axes);
                    EditorGUILayout.PropertyField(rotateDegrees);
                    EditorGUILayout.PropertyField(radius0);
                    EditorGUILayout.PropertyField(radius1);
                    EditorGUILayout.PropertyField(axisOffset);
                    EditorGUI.indentLevel--;
                }
                GuiSimple.Label("Rotation", EditorStyles.boldLabel);
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(rotationZ);
                    EditorGUI.indentLevel--;
                }
                GuiSimple.Label("Scale", EditorStyles.boldLabel);
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(scaleX);
                    EditorGUILayout.PropertyField(scaleY);
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
            }
        }

        public void DrawTrsGeneration(GfzSpiralPathSimple spiralPath)
        {
            //var buttonWidths = GUILayout.Width(GuiSimple.GetElementsWidth(3));

            GuiSimple.Label("Generate TRS", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(autoGenerateTRS);

            if (GUILayout.Button("Generate All Components"))
            {
                spiralPath.UpdateAllRelatedToTRS();
            }

            EditorGUILayout.PropertyField(animationCurveTRS);
        }

    }
}
