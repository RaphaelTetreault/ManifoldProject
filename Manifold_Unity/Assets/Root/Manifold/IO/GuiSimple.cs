using System;
using UnityEngine;
using UnityEditor;

namespace Manifold
{

    public static class GuiSimple
    {
        public static string Labelize(string name)
        {
            return ObjectNames.NicifyVariableName(name);
        }


        public static void Label(string label, params GUILayoutOption[] options)
        {
            EditorGUILayout.LabelField(Labelize(label), options);
        }
        public static void Label(string label, GUIStyle style)
        {
            EditorGUILayout.LabelField(Labelize(label), style);
        }


        public static int Int(string name, int value)
        {
            var label = ObjectNames.NicifyVariableName(name);
            return EditorGUILayout.IntField(label, value);
        }

        public static float Float(string name, float value)
        {
            var label = ObjectNames.NicifyVariableName(name);
            return EditorGUILayout.FloatField(label, value);
        }

        public static string String(string label, string value)
        {
            return EditorGUILayout.TextField(Labelize(label), value);
        }


        public static TEnum EnumPopup<TEnum>(string name, TEnum @enum) where TEnum : Enum
        {
            var label = ObjectNames.NicifyVariableName(name);
            return (TEnum)EditorGUILayout.EnumPopup(label, @enum);
        }


        public static string BrowseFile(string value, string label, string title, string directory, string extensions)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(Labelize(label));
            if (GUILayout.Button("Browse"))
            {
                var result = EditorUtility.OpenFilePanel(title, directory, extensions);
                if (!string.IsNullOrEmpty(result))
                {
                    value = result;
                }
            }
            value = EditorGUILayout.TextField(value);
            EditorGUILayout.EndHorizontal();

            return value;
        }
        public static string BrowseFolder(string value, string label, string title, string directory, string defaultName)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(Labelize(label));
            if (GUILayout.Button("Browse"))
            {
                var result = EditorUtility.OpenFolderPanel(title, directory, defaultName);
                if (!string.IsNullOrEmpty(result))
                {
                    value = result + "/";
                }
            }
            value = EditorGUILayout.TextField(value);
            EditorGUILayout.EndHorizontal();

            return value;
        }
        public static string BrowseFolderWithFilters(string value, string label, string title, string directory, params string[] filters)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(Labelize(label));
            if (GUILayout.Button("Browse"))
            {
                var result = EditorUtility.OpenFilePanelWithFilters(title, directory, filters);
                if (!string.IsNullOrEmpty(result))
                {
                    value = result + "/";
                }
            }
            value = EditorGUILayout.TextField(value);
            EditorGUILayout.EndHorizontal();
            return value;
        }

    }
}