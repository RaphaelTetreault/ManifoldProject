using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Hex))]
public class HexValueDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var attr = (attribute as Hex);
        var pos = position;//EditorGUI.PrefixLabel(position, label);

        var newLabel = new GUIContent(label.text);
        if (!string.IsNullOrEmpty(attr.prefix))
            newLabel.text = $"{attr.prefix} {newLabel.text}";

        var width = pos.width - EditorGUIUtility.labelWidth;
        var halfWidth = width / 2f;
        pos.width -= halfWidth;
        EditorGUI.PropertyField(pos, property, newLabel);

        pos.x = pos.x + pos.width;
        pos.width = halfWidth;

        //var attributeWidth = (attr.Width > 0)
        //    ? attr.Width
        //    : (pos.width - EditorGUIUtility.labelWidth) / 2f;
        //var propertyWidth = pos.width - attributeWidth;
        //pos.width = propertyWidth;
        //EditorGUI.PropertyField(pos, property, newLabel);

        //pos.x = pos.width;
        //pos.width = attributeWidth;
        var value = property.intValue;
        var valueFormat = attr.Format;
        var text = $"0x{value.ToString(valueFormat)}";
        EditorGUI.TextField(pos, text);
    }
}
