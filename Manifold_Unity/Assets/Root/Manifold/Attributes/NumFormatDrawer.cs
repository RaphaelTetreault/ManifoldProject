using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(NumFormat))]
public class NumFormatDrawer : PropertyDrawer
{
    public const float kColorMult = 0.92f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var attr = (attribute as NumFormat);

        // Set up global position data
        var pos = position;
        var viewWidthScale = position.width / EditorGUIUtility.currentViewWidth;
        var labelW = EditorGUIUtility.labelWidth;
        var viewW = EditorGUIUtility.currentViewWidth - labelW;
        var halfWidth = viewW * viewWidthScale / 2f;
        var indent = (pos.x * EditorGUI.indentLevel);

        var newLabel = new GUIContent(label.text);
        //if (!string.IsNullOrEmpty(attr.Format0))
        //    newLabel.text = string.Format(attr.Format0, newLabel.text);

        var labelWidth = (pos.width - halfWidth);
        pos.width = labelWidth;
        EditorGUI.PropertyField(pos, property, newLabel);

        pos.x += pos.width - indent;
        pos.width = halfWidth + indent;

        var value = property.intValue;
        var text = System.Convert.ToString(value, attr.NumBase).PadLeft(attr.NumDigits, '0');
        var color = GUI.color;

        GUI.color *= kColorMult;
        var textField = EditorGUI.DelayedTextField(pos, text);
        GUI.color = color;

        if (text == textField)
            return;

        var hexPattern = @"^[a-fA-F0-9]+";
        var regex = new System.Text.RegularExpressions.Regex(hexPattern);
        var regexMatch = regex.Match(textField).ToString();
        var isHex = regexMatch == textField;
        if (isHex)
        {
            property.intValue = System.Convert.ToInt32(textField, 16);
        }
    }
}