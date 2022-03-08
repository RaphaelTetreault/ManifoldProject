using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Manifold
{
    public class ReadOnlyGUIAttribute : PropertyAttribute
    {

    }

    [CustomPropertyDrawer(typeof(ReadOnlyGUIAttribute))]
    public class ReadOnlyGUIDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property,
                                                GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position,
                                   SerializedProperty property,
                                   GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
}
