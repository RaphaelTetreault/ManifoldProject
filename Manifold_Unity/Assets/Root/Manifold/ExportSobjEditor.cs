using UnityEngine;

namespace UnityEditor
{
    [CustomEditor(typeof(ExportSobj), true)]
    public class ExportSobjEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var editorTarget = target as ExportSobj;

            if (GUILayout.Button(editorTarget.ButtonText))
            {
                editorTarget.Export();

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
