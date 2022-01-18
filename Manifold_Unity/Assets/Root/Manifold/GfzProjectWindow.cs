using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEditor;
using GameCube.GFZ.CourseCollision;


namespace Manifold
{
    public abstract class EditorData
    {
        public void Draw(string heading)
        {
            if (!string.IsNullOrEmpty(heading))
            {
                var label = ObjectNames.NicifyVariableName(heading);
                EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            }

            OnGUI();
        }

        public abstract void OnGUI();

    }


    [Serializable]
    public class GfzProjectSettings : EditorData
    {
        public ColiScene.SerializeFormat serializeFormat = ColiScene.SerializeFormat.GX;

        public sealed override void OnGUI()
        {
            serializeFormat = GuiSimple.EnumPopup(nameof(serializeFormat), serializeFormat);
        }
    }



    public class GfzProjectWindow : EditorWindow
    {
        // Handy window serialization
        // https://answers.unity.com/questions/119978/how-do-i-have-an-editorwindow-save-its-data-inbetw.html
        private const string saveName = "gfz-window";

        // Serialize values so they persist
        [SerializeField] int tabIndex;
        [SerializeField] GfzProjectSettings settings = new GfzProjectSettings();
        [SerializeField] string rootFolder = string.Empty;

        private void OnEnable()
        {
            // Load in data
            var data = EditorPrefs.GetString(saveName, JsonUtility.ToJson(this, false));
            JsonUtility.FromJsonOverwrite(data, this);
        }

        private void OnDisable()
        {
            // Save out data
            var data = JsonUtility.ToJson(this, false);
            EditorPrefs.SetString(saveName, data);
        }

        // Add menu named "My Window" to the Window menu
        [MenuItem("Manifold/Open Settings Window")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            GfzProjectWindow window = (GfzProjectWindow)GetWindow(typeof(GfzProjectWindow));
            window.Show();
        }

        void OnGUI()
        {
            // The current window instance in serializable format
            var window = new SerializedObject(this);
            // The window tab we want to look at
            tabIndex = GUILayout.Toolbar(tabIndex, new string[] { "Tab 0", "Tab 1", "Tab 2" });

            switch (tabIndex)
            {
                case 0:
                    {
                        settings.Draw("Settings");
                        rootFolder = GuiSimple.BrowseFolder(rootFolder, "Root Folder", "Open Root GFZ Folder", "C:/", "");
                    }
                    break;

                case 1:
                    {

                    }
                    break;

                case 2:
                    {

                    }
                    break;

                default:
                    throw new NotImplementedException();
            }

            // Save changes
            window.ApplyModifiedProperties();
        }
    }


    public static class GuiSimple
    {
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

        public static TEnum EnumPopup<TEnum>(string name, TEnum @enum) where TEnum : Enum
        {
            var label = ObjectNames.NicifyVariableName(name);
            return (TEnum)EditorGUILayout.EnumPopup(label, @enum);
        }

        public static string Labelize(string name)
        {
            return ObjectNames.NicifyVariableName(name);
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
                    value = result;
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
                    value = result;
                }
            }
            value = EditorGUILayout.TextField(value);
            EditorGUILayout.EndHorizontal();
            return value;
        }

    }
}