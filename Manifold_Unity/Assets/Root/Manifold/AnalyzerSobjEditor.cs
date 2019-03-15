using UnityEngine;

namespace UnityEditor
{
    [CustomEditor(typeof(AnalyzerSobj), true)]
    public class AnalyzerSobjEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var editorTarget = target as AnalyzerSobj;

            if (GUILayout.Button(editorTarget.ButtonText))
            {
                editorTarget.Analyze();

                if (!string.IsNullOrEmpty(editorTarget.ProcessMessage))
                    Debug.Log(editorTarget.ProcessMessage);
            }
            if (!string.IsNullOrEmpty(editorTarget.HelpBoxMessage))
                EditorGUILayout.HelpBox(editorTarget.HelpBoxMessage, MessageType.Info);
            EditorGUILayout.Space();

            base.OnInspectorGUI();
        }
    }
}
