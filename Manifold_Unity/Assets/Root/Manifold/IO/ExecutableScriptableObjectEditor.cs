using System;
using UnityEngine;
using UnityEditor;

namespace Manifold.IO
{
    [CustomEditor(typeof(ExecutableScriptableObject), true)]
    class ExecutableScriptableObjectEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var editorTarget = target as ExecutableScriptableObject;

            if (GUILayout.Button(editorTarget.ExecuteText))
            {
                editorTarget.Execute();
            }
            EditorGUILayout.Space();

            base.OnInspectorGUI();
        }
    }
}
