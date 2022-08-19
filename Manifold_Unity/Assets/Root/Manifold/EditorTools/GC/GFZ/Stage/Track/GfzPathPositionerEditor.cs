//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEditor;
//using UnityEngine;

//namespace Manifold.EditorTools.GC.GFZ.Stage.Track
//{
//    [CustomEditor(typeof(GfzPathPositioner))]
//    internal class GfzPathPositionerEditor : Editor
//    {
//        SerializedProperty segment;
//        SerializedProperty horizontalOffset;
//        SerializedProperty positionOffset;
//        SerializedProperty rotationOffset;
//        SerializedProperty scaleOffset;
//        SerializedProperty prefab;
//        SerializedProperty maxStep;
//        SerializedProperty totalInstances;
//        SerializedProperty from;
//        SerializedProperty to;
//        SerializedProperty removeFirst;
//        SerializedProperty removeLast;

//        private void OnEnable()
//        {
//            segment = serializedObject.FindProperty(nameof(segment));
//            horizontalOffset = serializedObject.FindProperty(nameof(horizontalOffset));
//            positionOffset = serializedObject.FindProperty(nameof(positionOffset));
//            rotationOffset = serializedObject.FindProperty(nameof(rotationOffset));
//            scaleOffset = serializedObject.FindProperty(nameof(scaleOffset));
//            prefab = serializedObject.FindProperty(nameof(prefab));
//            maxStep = serializedObject.FindProperty(nameof(maxStep));
//            totalInstances = serializedObject.FindProperty(nameof(totalInstances));
//            from = serializedObject.FindProperty(nameof(from));
//            to = serializedObject.FindProperty(nameof(to));
//            removeFirst = serializedObject.FindProperty(nameof(removeFirst));
//            removeLast = serializedObject.FindProperty(nameof(removeLast));
//        }


//        public override void OnInspectorGUI()
//        {
//            var editorTarget = target as GfzPathPositioner;

//            serializedObject.Update();
//            {
//                GuiSimple.DefaultScript("Script", editorTarget);
//                EditorGUILayout.Separator();
//                DrawFields();
//                DrawCreateDelete(editorTarget);
//            }
//            serializedObject.ApplyModifiedProperties();

//            //base.OnInspectorGUI();
//        }

//        private void DrawFields()
//        {
//        {
//            EditorGUILayout.PropertyField(segment);
//            GuiSimple.Label("Transform Offset", EditorStyles.boldLabel);
//            EditorGUI.indentLevel++;
//            EditorGUILayout.PropertyField(horizontalOffset);
//            EditorGUILayout.PropertyField(positionOffset);
//            EditorGUILayout.PropertyField(rotationOffset);
//            EditorGUILayout.PropertyField(scaleOffset);
//            EditorGUI.indentLevel--;
//            EditorGUILayout.Separator();
//            GuiSimple.Label("Object Positioning", EditorStyles.boldLabel);
//            EditorGUILayout.PropertyField(prefab);
//            EditorGUILayout.PropertyField(maxStep);
//            EditorGUILayout.PropertyField(totalInstances);
//            EditorGUILayout.PropertyField(from);
//            EditorGUILayout.PropertyField(to);
//            EditorGUILayout.PropertyField(removeFirst);
//            EditorGUILayout.PropertyField(removeLast);
//        }

//        private void DrawCreateDelete(GfzPathPositioner pathPositioner)
//        {
//            EditorGUILayout.BeginHorizontal();
//            {
//                EditorGUILayout.PrefixLabel("Generate");
//                if (GUILayout.Button("Create Instances"))
//                    pathPositioner.CreateObjects();
//                if (GUILayout.Button("Delete Instances"))
//                    pathPositioner.DeleteObjects();
//            }
//            EditorGUILayout.EndHorizontal();
//        }

//    }
//}
