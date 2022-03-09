using UnityEngine;
using UnityEditor;

namespace Manifold.EditorTools.Attributes
{
    [CustomPropertyDrawer(typeof(HexFlags))]
    public class HexFlagsDrawer : PropertyDrawer
    {
        public const float kColorMult = HexDrawer.kColorMult;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = (attribute as HexFlags);

            // Set up global position data
            var pos = position;
            var viewWidthScale = position.width / EditorGUIUtility.currentViewWidth;
            var labelW = EditorGUIUtility.labelWidth;
            var viewW = EditorGUIUtility.currentViewWidth - labelW;
            var halfWidth = viewW * viewWidthScale / 2f;
            var indent = (pos.x * EditorGUI.indentLevel);

            var newLabel = new GUIContent(label.text);
            if (!string.IsNullOrEmpty(attr.prefix))
                newLabel.text = $"{attr.prefix} - {newLabel.text}";

            pos.width -= halfWidth;
            property.intValue = EditorGUI.MaskField(pos, newLabel, property.intValue, property.enumNames);

            pos.x += pos.width - indent;
            pos.width = halfWidth + indent;
            var text = property.intValue.ToString(attr.Format);
            if (attr.NumDigits > 0)
                text = text.Substring(text.Length - attr.NumDigits, attr.NumDigits);

            var color = GUI.color;
            GUI.color *= kColorMult;
            EditorGUI.TextArea(pos, text);
            GUI.color = color;
        }
    }
}