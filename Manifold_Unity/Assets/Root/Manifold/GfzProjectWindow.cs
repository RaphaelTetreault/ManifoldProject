using GameCube.GFZ.CourseCollision;
using System;
using UnityEngine;
using UnityEditor;

namespace Manifold.IO.GFZ
{
    [Serializable]
    public class GfzProjectSettings
    {
        [SerializeField] ColiScene.SerializeFormat serializeFormat = ColiScene.SerializeFormat.GX;
        [SerializeField] string rootFolder = string.Empty;
        [SerializeField] string testGfzj01 = string.Empty;
        [SerializeField] string testGfze01 = string.Empty;
        [SerializeField] string testGfzp01 = string.Empty;
        [SerializeField] string testGfzj8p = string.Empty;
        //
        [SerializeField] string logOutput = string.Empty;
        [SerializeField] string analysisOutput = string.Empty;
        [SerializeField] string fileOutput = string.Empty;
        [SerializeField] string unityImportDir = "GFZ/";
        [SerializeField] int sceneOfInterestID = 1;

        public ColiScene.SerializeFormat SerializeFormat => serializeFormat;
        public string RootFolder => rootFolder;
        public string LogOutput => logOutput;
        public string AnalysisOutput => analysisOutput;
        public string FileOutput => fileOutput;
        public string UnityImportDir => unityImportDir;
        public int SceneOfInterestID => sceneOfInterestID;

        // Easy accessors for common places
        public string StageDir => $"{rootFolder}/stage/";


        public string[] GetTestRootDirectories()
        {
            return new string[]
            {
                testGfzj01,
                testGfze01,
                testGfzp01,
                testGfzj8p,
            };
        }

        public string SeekRootStart()
        {
            // TODO: change per editor platform
            return "C:/";
        }

        public void DrawSettingsTab()
        {
            EditorGUILayout.Space();
            serializeFormat = GuiSimple.EnumPopup(nameof(serializeFormat), serializeFormat);
            rootFolder = GuiSimple.BrowseFolder(rootFolder, "Root Folder", "Open Root GFZ Folder", SeekRootStart(), "");
        }

        public void DrawTestTab()
        {
            EditorGUILayout.Space();
            GuiSimple.Label("Folders for all version for mass tests", EditorStyles.boldLabel);
            testGfzj01 = GuiSimple.BrowseFolder(testGfzj01, "Root Folder (gfzj01)", "Open Root GFZ J Folder", testGfzj01, "");
            testGfze01 = GuiSimple.BrowseFolder(testGfze01, "Root Folder (gfze01)", "Open Root GFZ E Folder", testGfze01, "");
            testGfzp01 = GuiSimple.BrowseFolder(testGfzp01, "Root Folder (gfzp01)", "Open Root GFZ P Folder", testGfzp01, "");
            testGfzj8p = GuiSimple.BrowseFolder(testGfzj8p, "Root Folder (gfzj8p)", "Open Root AX Folder", testGfzj8p, "");
            EditorGUILayout.Space();
            GuiSimple.Label("Output Logs Go Here", EditorStyles.boldLabel);
            logOutput = GuiSimple.BrowseFolder(logOutput, "Log Output Directory", "Open Log Directory", logOutput, "");
            EditorGUILayout.Space();
            GuiSimple.Label("Analysis", EditorStyles.boldLabel);
            analysisOutput = GuiSimple.BrowseFolder(analysisOutput, "Analysis Output Directory", "Open Analysis Directory", analysisOutput, "");
            EditorGUILayout.Space();
            GuiSimple.Label("File (Binaries)", EditorStyles.boldLabel);
            fileOutput = GuiSimple.BrowseFolder(fileOutput, "File/Binary Output Directory", "Open File Output Directory", fileOutput, "");
            EditorGUILayout.Space();
            GuiSimple.Label("GFZ->Unity Output", EditorStyles.boldLabel);
            unityImportDir = GuiSimple.String("Unity Import Dest", unityImportDir);
            EditorGUILayout.Space();
            GuiSimple.Label("Scene Single", EditorStyles.boldLabel);
            sceneOfInterestID = GuiSimple.Int("Scene Of Interest", sceneOfInterestID);
        }


        public static GfzProjectSettings Load(string fileName)
        {
            // Load in data
            var settings = new GfzProjectSettings();
            var data = EditorPrefs.GetString(fileName, JsonUtility.ToJson(settings, false));
            JsonUtility.FromJsonOverwrite(data, settings);
            return settings;
        }

        public static void Save(string fileName, GfzProjectSettings projectSettings)
        {
            var data = JsonUtility.ToJson(projectSettings, false);
            EditorPrefs.SetString(fileName, data);
        }
    }



    public class GfzProjectWindow : EditorWindow
    {
        private static GfzProjectWindow self;

        // Handy window serialization
        // https://answers.unity.com/questions/119978/how-do-i-have-an-editorwindow-save-its-data-inbetw.html
        public const string saveName = "gfz-project-window";

        // Serialize values so they persist
        [SerializeField] int tabIndex;
        [SerializeField] GfzProjectSettings settings = new GfzProjectSettings();


        private void OnEnable()
        {
            // Load in data
            settings = GfzProjectSettings.Load(saveName);
            //
            self = this;
        }

        private void OnDisable()
        {
            // Save out data
            GfzProjectSettings.Save(saveName, settings);
            //
            self = null;
        }

        // Add menu named "My Window" to the Window menu
        [MenuItem(Const.Menu.Manifold + "Open Settings Window")]
        static void OpenNewWindow()
        {
            // Get existing open window or if none, make a new one:
            GfzProjectWindow window = (GfzProjectWindow)GetWindow(typeof(GfzProjectWindow));
            window.titleContent = new GUIContent("GFZ Project Settings");
            window.Show();
        }

        void OnGUI()
        {
            // The current window instance in serializable format
            var window = new SerializedObject(this);
            // The window tab we want to look at
            tabIndex = GUILayout.Toolbar(tabIndex, new string[] { "Settings", "Tests", "TBD" });

            switch (tabIndex)
            {
                case 0:
                    {
                        settings.DrawSettingsTab();
                    }
                    break;

                case 1:
                    {
                        settings.DrawTestTab();
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


        /// <summary>
        /// Returns setting data stored in the GFZ Project Window panel.
        /// </summary>
        /// <returns></returns>
        public static GfzProjectSettings GetSettings()
        {
            if (self != null)
            {
                //Debug.Log("Got instance data");
                return self.settings;
            }
            else
            {
                //Debug.Log("Got data from disk");
                return GfzProjectSettings.Load(saveName);
            }
        }
    }


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