using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(EnumFlags))]
public class EnumFlagsDrawer : PropertyDrawer
{
    public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
    {
        _property.intValue = EditorGUI.MaskField(_position, _label, _property.intValue, _property.enumNames);
    }
}

[CustomPropertyDrawer(typeof(HexEnumFlags))]
public class HexEnumFlagsDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var attr = (attribute as HexEnumFlags);
        var pos = position;
        var width = pos.width - EditorGUIUtility.labelWidth;
        var halfWidth = width / 2f;

        var newLabel = new GUIContent(label.text);
        if (!string.IsNullOrEmpty(attr.prefix))
            newLabel.text = $"{attr.prefix} {newLabel.text}";

        pos.width -= halfWidth;
        property.intValue = EditorGUI.MaskField(pos, newLabel, property.intValue, property.enumNames);

        pos.x = pos.x + pos.width;
        pos.width = halfWidth;
        var text = property.intValue.ToString(attr.format);
        if (attr.prepend0x)
            text = $"0x{text}";

        EditorGUI.TextArea(pos, text);
    }
}
