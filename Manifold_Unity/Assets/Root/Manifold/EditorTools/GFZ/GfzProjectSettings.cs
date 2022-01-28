using GameCube.GFZ.CourseCollision;
using System;
using UnityEngine;
using UnityEditor;

namespace Manifold.EditorTools.GC.GFZ
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
        [SerializeField] string sceneExportPath = string.Empty;

        public ColiScene.SerializeFormat SerializeFormat => serializeFormat;
        public string RootFolder => rootFolder;
        public string LogOutput => logOutput;
        public string AnalysisOutput => analysisOutput;
        public string FileOutput => fileOutput;
        public string UnityImportDir => unityImportDir;
        public int SceneOfInterestID => sceneOfInterestID;
        public string SceneExportPath => sceneExportPath;

        // Easy accessors for common places
        public string StageDir => $"{rootFolder}stage/";


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
            sceneExportPath = GuiSimple.BrowseFolder(sceneExportPath, "Scene Export Path", "Open Scene Export Directory", sceneExportPath, "");
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

}