using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;

namespace Manifold.EditorTools.Attributes
{
    /// <summary>
    /// Default drawer for both BrowseFileField and BrowseFolderField
    /// </summary>
    [CustomPropertyDrawer(typeof(BrowseFileField))]
    [CustomPropertyDrawer(typeof(BrowseFolderField))]
    public class BrowseFieldDrawer : PropertyDrawer
    {
        const int browseFieldWidth = 60;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (property.type == "string")
                ? base.GetPropertyHeight(property, label)       // [Browse] String
                : base.GetPropertyHeight(property, label) * 2f; // Helpbox Error
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                base.OnGUI(position, property, label);
                return;
            }

            BrowsePathAttribute attributeTarget = attribute as BrowsePathAttribute;
            string value = property.stringValue;

            Rect contentPosition = EditorGUI.PrefixLabel(position, new GUIContent(ObjectNames.NicifyVariableName(property.name)));
            float textContentWidth = contentPosition.width - browseFieldWidth;
            contentPosition.width = browseFieldWidth;


            string browsePath = Path.Combine(Application.dataPath, attributeTarget.RelativeDirectory);

            if (GUI.Button(contentPosition, "Browse"))
            {
                string str = (attribute is BrowseFileField)
                    ? EditorUtility.OpenFilePanel(attributeTarget.PanelTitle, browsePath, (attributeTarget as BrowseFileField).FileTypeArgs)
                    : EditorUtility.OpenFolderPanel(attributeTarget.PanelTitle, browsePath, string.Empty);

                // Min requirements for C:/, etc.
                // Prevents returning null on cancel.
                if (str.Length > 2)
                    value = str;
            }

            contentPosition.x += contentPosition.width;
            contentPosition.width = textContentWidth;

            // Only Regex if string contains desired directory
            // This can fail if the regex folder is contained twice in the directoy
            if (value.Contains(attributeTarget.RelativeDirectory))
            {
                // Get remainder of string after BrowsePathAttribute.RelativeDirectory
                // By default it's value is "Assets/"
                value = Regex.Match(value, string.Format("(?<={0}).*$", attributeTarget.RelativeDirectory)).Value;

                if (attribute is BrowseFileField)
                    if ((attribute as BrowseFileField).RemoveExtension)
                        value = Path.GetFileNameWithoutExtension(value);

            }
            property.stringValue = GUI.TextField(contentPosition, value);
        }
    }
}