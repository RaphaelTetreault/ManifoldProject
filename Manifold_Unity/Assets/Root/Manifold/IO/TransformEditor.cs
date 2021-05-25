using UnityEngine;
using UnityEditor;

namespace Manifold.IO
{
    [CustomEditor(typeof(Transform))]
    public class TransformEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var transform = (Transform)serializedObject.targetObject;
            serializedObject.Update();

            // DRAW LOCAL
            transform.localPosition = EditorGUILayout.Vector3Field("Local Position", transform.localPosition);
            transform.localRotation = Quaternion.Euler(EditorGUILayout.Vector3Field("Local Rotation", transform.localRotation.eulerAngles));
            transform.localScale = EditorGUILayout.Vector3Field("Local Scale", transform.localScale);
            
            // DRAW GLOBAL
            GUI.enabled = false;
            EditorGUILayout.Vector3Field("Global Position", transform.position);
            EditorGUILayout.Vector3Field("Global Rotation", transform.rotation.eulerAngles);
            EditorGUILayout.Vector3Field("Global Scale", transform.lossyScale);
            GUI.enabled = true;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
