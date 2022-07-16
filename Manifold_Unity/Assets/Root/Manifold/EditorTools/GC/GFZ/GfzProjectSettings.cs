using GameCube.GFZ.Stage;
using System;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Manifold.EditorTools.GC.GFZ
{
    [Serializable]
    public class GfzProjectSettings
    {
        // General settings
        [field: SerializeField] public SerializeFormat SerializeFormat { get; private set; } = SerializeFormat.GX;
        [field: SerializeField] public string SourceDirectory { get; private set; } = string.Empty;
        [field: SerializeField] public string WorkingFilesDirectory { get; private set; } = string.Empty;
        [field: SerializeField] public string UnityWorkingDirectory { get; private set; } = "gfz/";

        // Output Directories
        [field: SerializeField] public string LogOutput { get; private set; } = string.Empty;
        [field: SerializeField] public string AnalysisOutput { get; private set; } = string.Empty;
        [field: SerializeField] public string FileOutput { get; private set; } = string.Empty;

        // Scene Gen
        [field: SerializeField] public string SceneExportPath { get; private set; } = string.Empty;
        [field: SerializeField] public bool ConvertCoordSpace { get; private set; } = true;
        [field: SerializeField] public int SingleSceneIndex { get; private set; } = 1;

        // Collision Gen
        [field: SerializeField] public int Collider256SceneIndex { get; private set; } = 1;
        [field: SerializeField] public bool CreateColliderBackfaces { get; private set; } = true;
        [field: SerializeField] public StaticColliderMeshProperty Collider256MeshType { get; private set; } = StaticColliderMeshProperty.death1;

        // Debug / testing
        [field: SerializeField] public string Gfzj01Dir { get; private set; } = string.Empty;
        [field: SerializeField] public string Gfze01Dir { get; private set; } = string.Empty;
        [field: SerializeField] public string Gfzp01Dir { get; private set; } = string.Empty;
        [field: SerializeField] public string Gfzj8pDir { get; private set; } = string.Empty;


        // Easy accessors for common places
        public string SourceStageDirectory => $"{SourceDirectory}stage/";
        /// <summary>
        /// Location to store working files in Unity project (begins with "Assets/").
        /// </summary>
        public string AssetsWorkingDirectory => $"Assets/{UnityWorkingDirectory}";
        /// <summary>
        /// Used to define a safe output directory when a path is undefined.
        /// </summary>
        public string UndefinedDestination => Path.Combine(DriveRootDirectory, "Manifold");


        public string[] GetTestRootDirectories()
        {
            return new string[]
            {
                Gfzj01Dir,
                Gfze01Dir,
                Gfzp01Dir,
                Gfzj8pDir,
            };
        }

        private static string DriveRootDirectory => Directory.GetDirectoryRoot(Directory.GetCurrentDirectory());

        public void DrawSettingsTab()
        {
            GuiSimple.Label("General", EditorStyles.boldLabel);
            SerializeFormat = GuiSimple.EnumPopup(nameof(SerializeFormat), SerializeFormat);
            SourceDirectory = GuiSimple.BrowseFolder(SourceDirectory, nameof(SourceDirectory), "Open GFZ source directory", DriveRootDirectory);
            WorkingFilesDirectory = GuiSimple.BrowseFolder(WorkingFilesDirectory, nameof(WorkingFilesDirectory), "Open GFZ working directory", DriveRootDirectory);
            UnityWorkingDirectory = GuiSimple.BrowseUnityAssets(UnityWorkingDirectory, nameof(UnityWorkingDirectory), "Open Unity working directory");

            EditorGUILayout.Space();
            GuiSimple.Label("Scene Generation", EditorStyles.boldLabel);
            SceneExportPath = GuiSimple.BrowseFolder(SceneExportPath, nameof(SceneExportPath), "Open Scene Export Directory", DriveRootDirectory);
            SingleSceneIndex = GuiSimple.Int(nameof(SingleSceneIndex), SingleSceneIndex);
            ConvertCoordSpace = GuiSimple.Bool(nameof(ConvertCoordSpace), ConvertCoordSpace);

            EditorGUILayout.Space();
            GuiSimple.Label("Collider Generation", EditorStyles.boldLabel);
            CreateColliderBackfaces = GuiSimple.Bool(nameof(CreateColliderBackfaces), CreateColliderBackfaces);
            Collider256SceneIndex = GuiSimple.Int(nameof(Collider256SceneIndex), Collider256SceneIndex);
            Collider256MeshType = GuiSimple.EnumPopup(nameof(Collider256MeshType), Collider256MeshType);

            EditorGUILayout.Space();
            GuiSimple.Label("File Output Directories", EditorStyles.boldLabel);
            LogOutput = GuiSimple.BrowseFolder(LogOutput, "Log Output Directory", "Open Log Directory", DriveRootDirectory);
            AnalysisOutput = GuiSimple.BrowseFolder(AnalysisOutput, "Analysis Output Directory", "Open Analysis Directory", DriveRootDirectory);
            FileOutput = GuiSimple.BrowseFolder(FileOutput, "File/Binary Output Directory", "Open File Output Directory", DriveRootDirectory);

        }

        public void DrawTestTab()
        {
            GuiSimple.Label("Mass IO Test Folders", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("These folders are used to test all files from all extracted versions of F-Zero AX and F-Zero GX.", MessageType.None, true);
            Gfzj01Dir = GuiSimple.BrowseFolder(Gfzj01Dir, "GFZJ01 Directory (GX-JP)", "Open F-Zero GX (JP) root folder with extracted files", DriveRootDirectory);
            Gfze01Dir = GuiSimple.BrowseFolder(Gfze01Dir, "GFZE01 Directory (GX-NA)", "Open F-Zero GX (NA) root folder with extracted files", DriveRootDirectory);
            Gfzp01Dir = GuiSimple.BrowseFolder(Gfzp01Dir, "GFZP01 Directory (GX-PAL)", "Open F-Zero GX (PAL) root folder with extracted files", DriveRootDirectory);
            Gfzj8pDir = GuiSimple.BrowseFolder(Gfzj8pDir, "GFZJ8P Directory (AX-JP)", "Open F-Zero AX (JP) root folder with extracted files", DriveRootDirectory);
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