using UnityEngine;
using UnityEditor;

namespace Manifold.IO
{
    // TODO: cleanup this crap from 3 iterations...
    [CustomEditor(typeof(Transform))]
    public class TransformEditor : Editor
    {
        SerializedProperty m_Position;
        SerializedProperty m_Rotation;
        SerializedProperty m_Scale;

        const string positionLabel = "Position";
        const string rotationLabel = "Rotation";
        const string scaleLabel = "Scale";

        public void OnEnable()
        {
            m_Position = serializedObject.FindProperty("m_LocalPosition");
            m_Rotation = serializedObject.FindProperty("m_LocalRotation");
            m_Scale = serializedObject.FindProperty("m_LocalScale");
        }
        public override void OnInspectorGUI()
        {
            var transform = serializedObject.targetObject as Transform;
            serializedObject.Update();

            // DRAW LOCAL
            EditorGUILayout.LabelField("Local", EditorStyles.boldLabel);
            var pos = EditorGUILayout.Vector3Field(positionLabel, m_Position.vector3Value);
            var rot = EditorGUILayout.Vector3Field(rotationLabel, m_Rotation.quaternionValue.eulerAngles);
            var sca = EditorGUILayout.Vector3Field(scaleLabel, m_Scale.vector3Value);

            // DRAW GLOBAL
            EditorGUILayout.LabelField("Global", EditorStyles.boldLabel);
            GUI.enabled = false;
            EditorGUILayout.Vector3Field(positionLabel, transform.position);
            EditorGUILayout.Vector3Field(rotationLabel, transform.rotation.eulerAngles);
            EditorGUILayout.Vector3Field(scaleLabel, transform.lossyScale);
            GUI.enabled = true;

            if (GUI.changed)
            {
                Undo.RecordObject(transform, "Transform Changed");
                transform.localPosition = pos;
                transform.localEulerAngles = rot;
                transform.localScale = sca;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
