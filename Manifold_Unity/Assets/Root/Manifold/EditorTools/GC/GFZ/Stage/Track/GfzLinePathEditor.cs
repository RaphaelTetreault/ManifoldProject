using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    [CustomEditor(typeof(GfzLinePath))]
    internal class GfzLinePathEditor : Editor
    {
        // position
        SerializedProperty endPositionX;
        SerializedProperty endPositionY;
        SerializedProperty endPositionZ;
        // rotation
        SerializedProperty rotationY;
        SerializedProperty rotationZ;
        // scale
        SerializedProperty scaleX;
        SerializedProperty scaleY;
        // all
        SerializedProperty animationCurveTRS;
        // other
        SerializedProperty step;
        SerializedProperty autoGenerateTRS;
        SerializedProperty showGizmos;

        void OnEnable()
        {
            endPositionX = serializedObject.FindProperty(nameof(endPositionX));
            endPositionY = serializedObject.FindProperty(nameof(endPositionY));
            endPositionZ = serializedObject.FindProperty(nameof(endPositionZ));
            rotationY = serializedObject.FindProperty(nameof(rotationY));
            rotationZ = serializedObject.FindProperty(nameof(rotationZ));
            scaleX = serializedObject.FindProperty(nameof(scaleX));
            scaleY = serializedObject.FindProperty(nameof(scaleY));
            animationCurveTRS = serializedObject.FindProperty(nameof(animationCurveTRS));

            step = serializedObject.FindProperty(nameof(step));
            autoGenerateTRS = serializedObject.FindProperty(nameof(autoGenerateTRS));
            showGizmos = serializedObject.FindProperty(nameof(showGizmos));
        }

        public override void OnInspectorGUI()
        {
            var linePath = target as GfzLinePath;

            serializedObject.Update();
            {
                DrawUnityEditorDefaults(linePath);
                EditorGUILayout.Separator();
                DrawGizmosFields(linePath);
                EditorGUILayout.Separator();
                DrawSourceFields(linePath);
                EditorGUILayout.Separator();
                DrawTrsGeneration(linePath);
            }
            serializedObject.ApplyModifiedProperties();

            // Uncomment if you need to debug something
            //base.OnInspectorGUI();
        }

        private void DrawUnityEditorDefaults(GfzLinePath linePath)
        {
            GuiSimple.DefaultScript("Script", linePath);
            GUI.enabled = false;
            EditorGUILayout.ObjectField(nameof(linePath.Prev), linePath.Prev, typeof(GfzTrackSegmentRootNode), false);
            EditorGUILayout.ObjectField(nameof(linePath.Next), linePath.Next, typeof(GfzTrackSegmentRootNode), false);
            GUI.enabled = true;
        }

        private void DrawGizmosFields(GfzLinePath linePath)
        {
            GuiSimple.Label("Gizmos", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            {
                EditorGUILayout.PropertyField(showGizmos);
                EditorGUILayout.PropertyField(step);
            }
            EditorGUI.indentLevel--;
        }


        private void DrawSourceFields(GfzLinePath linePath)
        {
            GuiSimple.Label("Source Values", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.HelpBox("Transform values are used as the default values. The parameters below are offsets from that.", MessageType.None);
            {
                GuiSimple.Label("Position", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(endPositionX);
                EditorGUILayout.PropertyField(endPositionY);
                EditorGUILayout.PropertyField(endPositionZ);
                EditorGUI.indentLevel--;
                GuiSimple.Label("Rotation", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(rotationY);
                EditorGUILayout.PropertyField(rotationZ);
                EditorGUI.indentLevel--;
                GuiSimple.Label("Scale", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(scaleX);
                EditorGUILayout.PropertyField(scaleY);
                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;
        }

        public void DrawTrsGeneration(GfzLinePath linePath)
        {
            var buttonWidths = GUILayout.Width(GuiSimple.GetElementsWidth(3));

            GuiSimple.Label("Generate TRS", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(autoGenerateTRS);
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Generate Position", buttonWidths))
                    GeneratePosition(linePath);
                if (GUILayout.Button("Generate Rotation", buttonWidths))
                    GenerateRotation(linePath);
                if (GUILayout.Button("Generate Scale", buttonWidths))
                    GenerateScale(linePath);
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Generate All Components"))
                linePath.UpdateTRS();

            EditorGUILayout.PropertyField(animationCurveTRS);
        }

        private void OnSceneGUI()
        {
            if (!showGizmos.boolValue)
                return;

            var line = target as GfzLinePath;

            var hacTRS = line.CreateHierarchichalAnimationCurveTRS(false);
            float length = line.GetLineLength();
            int nSteps = (int)Mathf.Ceil(length / step.floatValue);
            var matrices = new Matrix4x4[nSteps + 1];
            for (int i = 0; i <= nSteps; i++)
            {
                var time = i / (float)nSteps * length;
                matrices[i] = hacTRS.EvaluateAnimationMatrices(time);
            }

            const float thickness = 5f;
            Vector3 left = Vector3.left * 0.5f;
            Vector3 right = Vector3.right * 0.5f;

            // Start and en lines
            {
                Handles.color = Color.white;
                var mtx0 = matrices[0];
                var mtxN = matrices[matrices.Length - 1];
                var sl = mtx0.MultiplyPoint(left);
                var sr = mtx0.MultiplyPoint(right);
                var el = mtxN.MultiplyPoint(left);
                var er = mtxN.MultiplyPoint(right);
                Handles.DrawLine(sl, sr, thickness);
                Handles.DrawLine(el, er, thickness);
            }

            for (int i = 0; i < matrices.Length - 1; i++)
            {
                var mtx0 = matrices[i + 0];
                var mtx1 = matrices[i + 1];
                var l0 = mtx0.MultiplyPoint(left);
                var l1 = mtx1.MultiplyPoint(left);
                var r0 = mtx0.MultiplyPoint(right);
                var r1 = mtx1.MultiplyPoint(right);
                Handles.color = Color.white;
                Handles.DrawLine(l0, r0, 1.25f);
                Handles.color = Color.green;
                Handles.DrawLine(l0, l1, thickness);
                Handles.color = Color.red;
                Handles.DrawLine(r0, r1, thickness);

            }
        }

        public void GeneratePosition(GfzLinePath linePath)
        {
            Undo.RecordObject(linePath, $"Set line path position");
            linePath.UpdateTrsPosition();
            EditorUtility.SetDirty(linePath);
        }

        public void GenerateRotation(GfzLinePath linePath)
        {
            Undo.RecordObject(linePath, $"Set line path rotation");
            linePath.UpdateTrsRotation();
            EditorUtility.SetDirty(linePath);
        }

        public void GenerateScale(GfzLinePath linePath)
        {
            Undo.RecordObject(linePath, $"Set line path scale");
            linePath.UpdateTrsScale();
            EditorUtility.SetDirty(linePath);
        }
    }
}
