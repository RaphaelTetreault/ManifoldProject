using System;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Manifold.EditorTools
{
    /// <summary>
    /// A helper class to make drawing GUI Layout easier
    /// </summary>
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

        public static bool Bool(string name, bool value)
        {
            var label = ObjectNames.NicifyVariableName(name);
            return EditorGUILayout.Toggle(label, value);
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

        public static string String(string name, string value)
        {
            return EditorGUILayout.TextField(Labelize(name), value);
        }

        public static Vector3 Vector3(string name, Vector3 value)
        {
            var label = ObjectNames.NicifyVariableName(name);
            return EditorGUILayout.Vector3Field(label, value);
        }


        public static TEnum EnumPopup<TEnum>(string name, TEnum @enum) where TEnum : Enum
        {
            var label = ObjectNames.NicifyVariableName(name);
            return (TEnum)EditorGUILayout.EnumPopup(label, @enum);
        }


        public static string BrowseFile(string value, string label, string title, string directory, string extensions, string fallbackDirectory = "")
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(Labelize(label));
            if (GUILayout.Button("Browse"))
            {
                string directoryToOpen = Directory.Exists(directory) ? directory : fallbackDirectory;
                string result = EditorUtility.OpenFilePanel(title, directoryToOpen, extensions);
                if (!string.IsNullOrEmpty(result))
                {
                    value = result;
                }
            }
            value = EditorGUILayout.TextField(value);
            EditorGUILayout.EndHorizontal();

            return value;
        }
        public static string BrowseFolder(string directory, string label, string title, string fallbackDirectory = "")
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(Labelize(label));
            if (GUILayout.Button("Browse"))
            {
                string directoryToOpen = Directory.Exists(directory) ? directory : fallbackDirectory;
                string result = EditorUtility.OpenFolderPanel(title, directoryToOpen, "");
                if (!string.IsNullOrEmpty(result))
                {
                    directory = result + "/";
                }
            }
            directory = EditorGUILayout.TextField(directory);
            EditorGUILayout.EndHorizontal();

            return directory;
        }
        public static string BrowseUnityAssets(string assetsDirectory, string label, string title)
        {
            var unityRootPath = Directory.GetCurrentDirectory();
            var unityAssetsPath = Path.Combine(unityRootPath, "Assets/").Replace('\\', '/');
            var requestedAssetsPath = Path.Combine(unityAssetsPath, assetsDirectory).Replace('\\', '/');

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(Labelize(label));
            if (GUILayout.Button("Browse"))
            {
                string directoryToOpen = Directory.Exists(requestedAssetsPath) ? requestedAssetsPath : unityAssetsPath;
                string result = EditorUtility.OpenFolderPanel(title, directoryToOpen, "");
                if (!string.IsNullOrEmpty(result))
                {
                    // Remove path up to and including "Assets/"
                    assetsDirectory = result.Substring(unityAssetsPath.Length);
                    assetsDirectory += "/";
                }
            }
            assetsDirectory = EditorGUILayout.TextField(assetsDirectory);
            EditorGUILayout.EndHorizontal();

            return assetsDirectory;
        }

        public static string BrowseFolderWithFilters(string value, string label, string title, string directory, string fallbackDirectory = "", params string[] filters)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(Labelize(label));
            if (GUILayout.Button("Browse"))
            {
                string directoryToOpen = Directory.Exists(directory) ? directory : fallbackDirectory;
                var result = EditorUtility.OpenFilePanelWithFilters(title, directoryToOpen, filters);
                if (!string.IsNullOrEmpty(result))
                {
                    value = result + "/";
                }
            }
            value = EditorGUILayout.TextField(value);
            EditorGUILayout.EndHorizontal();
            return value;
        }


        public static void DefaultScript<T>(string label, T behaviour) where T: MonoBehaviour
        {
            MonoScript script = MonoScript.FromMonoBehaviour(behaviour);
            GUI.enabled = script == null;
            EditorGUILayout.ObjectField(Labelize(label), script, typeof(T), false);
            GUI.enabled = true;
        }

        public static void DefaultScript<T>(T behaviour) where T : MonoBehaviour
        {
            DefaultScript("Script", behaviour);
        }

        public static float GetInspectorWidth()
        {
            var width = EditorGUIUtility.currentViewWidth;
            return width;
        }

        public static float GetElementsWidth(int nElements)
        {
            var width = EditorGUIUtility.currentViewWidth;
            var widthDivided = width / (nElements * 1.1f); // 1.1 acounts for gap between controls
            return widthDivided;
        }
    }
}