using UnityEngine;

namespace UnityEditor
{
    [CustomEditor(typeof(ImportSobj), true)]
    public class ImportSobj_Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            var editorTarget = target as ImportSobj;

            if (GUILayout.Button(editorTarget.ButtonText))
            {
                editorTarget.Import();

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
