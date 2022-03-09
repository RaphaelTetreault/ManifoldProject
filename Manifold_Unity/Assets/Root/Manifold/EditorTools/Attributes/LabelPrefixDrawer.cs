using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Manifold.EditorTools.Attributes
{
    [CustomPropertyDrawer(typeof(LabelPrefix))]
    public class LabelPrefixDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = attribute as LabelPrefix;
            var str = (!string.IsNullOrEmpty(attr.prefix))
                ? $"{attr.prefix} - {label.text}"
                : label.text;
            var text = new GUIContent(str);
            EditorGUI.PropertyField(position, property, text);
        }
    }
}