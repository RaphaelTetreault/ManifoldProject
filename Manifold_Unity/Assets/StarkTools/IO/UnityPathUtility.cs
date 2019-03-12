using System;
using System.IO;

namespace UnityEngine
{
    /// <summary>
    /// Utility to manage Unity-level directories and System-level directories within Unity
    /// </summary>
    /// <remarks>
    /// Unity Path/Folder refers to paths using Unity-specific separator characters (/) relative to some Unity Folder (mostly 'Assets/')
    /// System Path/Directory refers to system-level directories with system-specific separators (Windows: \, Unix: /)
    /// </remarks>
    public static class UnityPathUtility
    {
        #region DEFINITIONS

        /// <summary>
        /// Special Unity folders enum
        /// </summary>
        public enum UnityFolder
        {
            Assets,
            Editor,
            EditorDefaultResources,
            Gizmos,
            PersistentData,
            Plugins,
            ProjectRoot,
            Resources,
            StandardAssets,
            StreamingAssets,
        }

        #endregion 

        #region CONSTS

        /// <summary>
        /// System-specific directory separator character
        /// </summary>
        public static char DirectorySeparator { get { return Path.DirectorySeparatorChar; } }
        public const char UnixDirectorySeparator = '/';
        public const char NonUnixDirectorySeparator = '\\';

        // Warnings/Errors
        /// <summary>
        /// Warning for 'ToUnityFolderPath()' warning when path does contain relative directory
        /// </summary>
        private const string ToUnityFolderPathWarningFormat0 = "Path Regexed not a path in Unity Project \"{0}\"";
        
        /// <summary>
        /// Error for 'GetUnityFolder' when invalid arguments are used (folders that live outside Unity Project)
        /// </summary>
        private const string GetUnityFolderErrorFormat0 = "{0} is an invalid argument for GetUnityFolder() as the special folder is not within the Unity Project.";
        
        #endregion

        #region SPECIAL FOLDER STRINGS

        // Unity Special Folders
        /// <summary>
        /// Path from ProjectRoot/Assets/ to 'Assets' folder
        /// </summary>
        public const string UnityAssetsFolder = "Assets/";

        /// <summary>
        /// Path from ProjectRoot/Assets/ to 'Editor' folder
        /// </summary>
        public const string UnityEditorFolder = "Editor/";

        /// <summary>
        /// Path from ProjectRoot/Assets/ to 'Editor Default Resources' folder
        /// </summary>
        public const string UnityEditorDefaultResourcesFolder = "Editor Default Resources/";

        /// <summary>
        /// Path from ProjectRoot/Assets/ to 'Gizmos' folder
        /// </summary>
        public const string UnityGizmosFolder = "Gizmos/";

        /// <summary>
        /// Path from ProjectRoot/Assets/ to 'Plugins' folder
        /// </summary>
        public const string UnityPluginsFolder = "Plugins/";

        /// <summary>
        /// Path from ProjectRoot/Assets/ to 'Resources' folder
        /// </summary>
        public const string UnityResourcesFolder = "Resources/";

        /// <summary>
        /// Path from ProjectRoot/Assets/ to 'StandardAssets' folder
        /// </summary>
        public const string UnityStandardAssetsFolder = "StandardAssets/";

        /// <summary>
        /// Path from ProjectRoot/Assets/ to 'StreamingAssets' folder
        /// </summary>
        public const string UnityStreamingAssetsFolder = "StreamingAssets/";

        // Unity Special Directories
        /// <summary>
        /// Path from drive root to 'Assets' folder
        /// </summary>
        public static readonly string UnityAssetsDirectory = EnforceSystemSeparators(Application.dataPath + DirectorySeparator);

        /// <summary>
        /// Path from drive root to 'Editor' folder
        /// </summary>
        public static readonly string UnityEditorDirectory = EnforceSystemSeparators(UnityAssetsDirectory + UnityEditorFolder);

        /// <summary>
        /// Path from drive root to 'Editor Default Resources' folder
        /// </summary>
        public static readonly string UnityEditorDefaultResourcesDirectory = EnforceSystemSeparators(UnityAssetsDirectory + UnityEditorDefaultResourcesFolder);

        /// <summary>
        /// Path from drive root to 'Gizmos' folder
        /// </summary>
        public static readonly string UnityGizmosDirectory = EnforceSystemSeparators(UnityAssetsDirectory + UnityGizmosFolder);

        /// <summary>
        /// Path from drive root to 'Plugins' folder
        /// </summary>
        public static readonly string UnityPluginsDirectory = EnforceSystemSeparators(UnityAssetsDirectory + UnityPluginsFolder);

        /// <summary>
        /// Path from drive root to 'Resources' folder
        /// </summary>
        public static readonly string UnityResourcesDirectory = EnforceSystemSeparators(UnityAssetsDirectory + UnityResourcesFolder);

        /// <summary>
        /// Path from drive root to 'StandardAssets' folder
        /// </summary>
        public static readonly string UnityStandardAssetsDirectory = EnforceSystemSeparators(UnityAssetsDirectory + UnityStandardAssetsFolder);

        /// <summary>
        /// Path from drive root to 'StreamingAssets' folder
        /// </summary>
        public static readonly string UnityStreamingAssetsDirectory = EnforceSystemSeparators(UnityAssetsDirectory + UnityStreamingAssetsFolder);

        /// <summary>
        /// Path to current Unity project's root folder
        /// </summary>
        public static readonly string ProjectRootDirectory = EnforceSystemSeparators(Application.dataPath.Replace("Assets", string.Empty));

        /// <summary>
        /// Path to Unity's platform-specific 'Persistent Data Path' directory
        /// </summary>
        public static readonly string UnityPersistantDataDirectory = EnforceSystemSeparators(Application.persistentDataPath + DirectorySeparator);

        // Common System Directories
        /// <summary>
        /// System-level Personal directory (Environment.SpecialFolder.Personal)
        /// </summary>
        public static string PersonalDirectory
        {
            get
            {
                return GetSystemSpecialDirectory(Environment.SpecialFolder.Personal);
            }
        }

        /// <summary>
        /// System-level Desktop directory (Environment.SpecialFolder.Desktop)
        /// </summary>
        public static string DesktopDirectory
        {
            get
            {
                return GetSystemSpecialDirectory(Environment.SpecialFolder.Desktop);
            }
        }

        #endregion

        #region METHODS

        // Get path relative to X
        /// <summary>
        /// Returns the remainder of <paramref name="directory"/> after <paramref name="relativeDirectory"/>. Use <paramref name="containsPath"/>
        /// to see if <paramref name="directory"/> contains <paramref name="relativeDirectory"/>. Return value (<paramref name="directory"/>) will
        /// not be changed if it does not contain <paramref name="relativeDirectory"/>.
        /// </summary>
        /// <param name="directory">The value to trim</param>
        /// <param name="relativeDirectory">The string to trim from <paramref name="directory"/></param>
        /// <param name="containsPath">Does <paramref name="directory"/> contain <paramref name="relativeDirectory"/>?</param>
        /// <returns></returns>
        private static string GetPathRelative(string directory, string relativeDirectory, out bool containsPath)
        {
            containsPath = directory.Contains(relativeDirectory);

            if (containsPath)
                directory = directory.Remove(0, relativeDirectory.Length);

            return directory;
        }

        // Get path relative System/Unity
        /// <summary>
        /// Converts full folder directory path <paramref name="directory"/> from drive root path into Unity folder path relative to <paramref name="relativeToDirectory"/>
        /// </summary>
        /// <param name="directory">The value to converts</param>
        /// <param name="relativeToDirectory">The folder to trim relative to</param>
        /// <param name="isUnityProjectPath">Indicates if folder path is in Unity Project</param>
        /// <returns></returns>
        private static string ToUnityFolderPath(string directory, UnityFolder relativeToDirectory, out bool isUnityProjectPath)
        {
            directory = GetPathRelative(directory, GetUnityDirectory(relativeToDirectory), out isUnityProjectPath);
            directory = EnforceUnitySeparators(directory);

            return directory;
        }

        /// <summary>
        /// Converts full folder directory path <paramref name="directory"/> from drive root path into Unity folder path relative to <paramref name="relativeDirectory"/>
        /// </summary>
        /// <param name="directory">The value to converts</param>
        /// <param name="relativeDirectory">The folder to trim relative to</param>
        /// <returns></returns>
        public static string ToUnityFolderPath(string directory, UnityFolder relativeToDirectory)
        {
            bool isUnityProjectPath;
            directory = ToUnityFolderPath(directory, relativeToDirectory, out isUnityProjectPath);

            if (!isUnityProjectPath)
                Debug.LogWarningFormat(ToUnityFolderPathWarningFormat0, directory);

            return directory;
        }

        /// <summary>
        /// Converts folder path <paramref name="folder"/> relative to <paramref name="relativeToDirectory"/> into a full directory path from drive root
        /// </summary>
        /// <param name="folder">The value to converts</param>
        /// <param name="relativeToDirectory">The directory to prepend to <paramref name="folder"/></param>
        /// <returns></returns>
        public static string ToSystemPath(string folder, UnityFolder relativeToDirectory)
        {
            string directory = GetUnityDirectory(relativeToDirectory);
            folder = CombineSystemPath(true, directory, folder);
            folder = EnforceSystemSeparators(folder);

            return folder;
        }

        // Enforce directory separator character
        /// <summary>
        /// Returns the path replacing incompatible directory separators with proper Unity directory separators
        /// </summary>
        /// <param name="value">The value to enforce directory separators</param>
        /// <returns></returns>
        public static string EnforceSystemSeparators(string value)
        {
            return value.Replace(UnixDirectorySeparator, DirectorySeparator);
        }

        /// <summary>
        /// Returns the path replacing incompatible directory separators with proper System directory separators
        /// </summary>
        /// <param name="value">The value to enforce directory separators</param>
        /// <returns></returns>
        public static string EnforceUnitySeparators(string value)
        {
            return value.Replace(NonUnixDirectorySeparator, UnixDirectorySeparator);
        }

        // Get special folder
        /// <summary>
        /// Returns the <paramref name="folder"/> path starting after 'Assets/'
        /// </summary>
        /// <param name="folder">The folder to get relative to Unity 'Assets/'</param>
        /// <returns></returns>
        public static string GetUnityFolder(UnityFolder folder)
        {
            switch (folder)
            {
                default:
                    throw new NotImplementedException();

                case UnityFolder.PersistentData:
                case UnityFolder.ProjectRoot:
                    throw new ArgumentException(string.Format(GetUnityFolderErrorFormat0, folder));

                case UnityFolder.Assets:
                    // When regexing we need to keep the assets folder in the path string
                    return string.Empty;

                case UnityFolder.Editor:
                    return UnityEditorFolder;

                case UnityFolder.EditorDefaultResources:
                    return UnityEditorDefaultResourcesFolder;

                case UnityFolder.Gizmos:
                    return UnityGizmosFolder;

                case UnityFolder.Plugins:
                    return UnityPluginsFolder;

                case UnityFolder.Resources:
                    return UnityResourcesFolder;

                case UnityFolder.StandardAssets:
                    return UnityStandardAssetsFolder;

                case UnityFolder.StreamingAssets:
                    return UnityStreamingAssetsFolder;
            }
        }

        /// <summary>
        /// Returns the full directory path of <paramref name="directory"/>
        /// </summary>
        /// <param name="directory">The full directory to get</param>
        /// <returns></returns>
        public static string GetUnityDirectory(UnityFolder directory)
        {
            switch (directory)
            {
                default:
                    throw new NotImplementedException();

                case UnityFolder.Assets:
                    // When regexing we need to keep the assets folder in the directory string
                    return UnityAssetsDirectory;

                case UnityFolder.Editor:
                    return UnityEditorDirectory;

                case UnityFolder.EditorDefaultResources:
                    return UnityEditorDefaultResourcesDirectory;

                case UnityFolder.Gizmos:
                    return UnityGizmosDirectory;

                case UnityFolder.PersistentData:
                    return UnityPersistantDataDirectory;

                case UnityFolder.Plugins:
                    return UnityPluginsDirectory;

                case UnityFolder.ProjectRoot:
                    return ProjectRootDirectory;

                case UnityFolder.Resources:
                    return UnityResourcesDirectory;

                case UnityFolder.StandardAssets:
                    return UnityStandardAssetsDirectory;

                case UnityFolder.StreamingAssets:
                    return UnityStreamingAssetsDirectory;
            }
        }

        /// <summary>
        /// Returns the full directory path of environment <paramref name="folder"/>
        /// </summary>
        /// <param name="folder">Special folder to get directory of</param>
        /// <param name="option">Special folder option (optional)</param>
        /// <returns></returns>
        public static string GetSystemSpecialDirectory(Environment.SpecialFolder folder, Environment.SpecialFolderOption option = Environment.SpecialFolderOption.None)
        {
            return Directory.GetParent(Environment.GetFolderPath(folder, option)).FullName + DirectorySeparator;
        }


        // File Exists
        /// <summary>
        /// Returns true if file exists in Unity Project
        /// </summary>
        /// <param name="path">Path relative to <paramref name="relativeToFolder"/></param>
        /// <param name="relativeToFolder">The folder to check <paramref name="path"/> relative to</param>
        /// <returns></returns>
        public static bool UnityFileExists(string path, UnityFolder relativeToFolder = UnityFolder.Assets)
        {
            path = EnforceUnitySeparators(path);
            string relativeDirectory = GetUnityDirectory(relativeToFolder);
            string directory = CombineSystemPath(relativeDirectory, path);

            return File.Exists(directory);
        }

        /// <summary>
        /// Returns true if files exist in system
        /// </summary>
        /// <param name="path">Path starting at system root</param>
        /// <returns></returns>
        public static bool SystemFileExists(string path)
        {
            path = EnforceUnitySeparators(path);

            return File.Exists(path);
        }

        // Directory Exists
        /// <summary>
        /// Returns true if <paramref name="folder"/> directory exists
        /// </summary>
        /// <param name="folder">The Unity special folder directory to check</param>
        /// <returns></returns>
        public static bool DirectoryExists(UnityFolder folder)
        {
            string directory = GetUnityDirectory(folder);
            return Directory.Exists(directory);
        }

        /// <summary>
        /// Returns true if directory exists (path is ensured to use proper system directory separator)
        /// </summary>
        /// <param name="path">The path to check</param>
        /// <returns></returns>
        public static bool DirectoryExists(string path)
        {
            return Directory.Exists(EnforceUnitySeparators(path));
        }

        // Combine
        /// <summary>
        /// Combines path using Unity separator characters
        /// </summary>
        /// <param name="appendDirectorySeparator">Append proper directory character</param>
        /// <param name="values">Values to combine</param>
        /// <returns></returns>
        public static string CombineUnityPath(bool appendDirectorySeparator, params string[] values)
        {
            string value = EnforceUnitySeparators(Path.Combine(values));

            if (appendDirectorySeparator)
                value += UnixDirectorySeparator;

            return value;
        }
        /// <summary>
        /// Combines path using Unity separator characters
        /// </summary>
        /// <param name="values">Values to combine</param>
        /// <returns></returns>
        public static string CombineUnityPath(params string[] values)
        {
            return CombineUnityPath(false, values);
        }

        /// <summary>
        /// Combines path using system separator characters
        /// </summary>
        /// <param name="appendDirectorySeparator">Append proper directory separator character</param>
        /// <param name="values">Values to combine</param>
        /// <returns></returns>
        public static string CombineSystemPath(bool appendDirectorySeparator, params string[] values)
        {
            string value = EnforceSystemSeparators(Path.Combine(values));

            if (appendDirectorySeparator)
                value += DirectorySeparator;

            return value;
        }
        /// <summary>
        /// Combines path using system separator characters
        /// </summary>
        /// <param name="values">Values to combine</param>
        /// <returns></returns>
        public static string CombineSystemPath(params string[] values)
        {
            return CombineSystemPath(false, values);
        }

        #endregion

    }
}