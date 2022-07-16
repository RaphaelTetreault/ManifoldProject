using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Manifold.EditorTools
{
    public static class AssetDatabaseUtility
    {
        public static T[] GetAllOfType<T>(params string[] searchInFolders)
            where T : ScriptableObject
        {
            // Prepend "Assets/" to all folders
            for (int i = 0; i < searchInFolders.Length; i++)
            {
                searchInFolders[i] = $"Assets/{searchInFolders[i]}";
            }

            // Get asset GUIDs based on search parameters
            string[] guids;
            if (searchInFolders != null && searchInFolders.Length > 0)
            {
                var typeQuery = $"t:{typeof(T).Name}";
                guids = AssetDatabase.FindAssets(typeQuery, searchInFolders);
            }
            else
            {
                var typeQuery = $"t:{typeof(T).Name}";
                guids = AssetDatabase.FindAssets(typeQuery);
            }

            // Get asset references
            var assets = new T[guids.Length];
            for (int i = 0; i < assets.Length; i++)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                assets[i] = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            }

            return assets;
        }

        public static T[] GetAllOfType<T>()
            where T : ScriptableObject
        {
            return GetAllOfType<T>(new string[0]);
        }


        public static void CreateDirectoryForAsset(string assetPath)
        {
            // Ensure folder path is indeed a path
            if (string.IsNullOrEmpty(assetPath))
                throw new ArgumentException($"Asset path \"{assetPath}\" is null or empty.");

            var pathWithoutFileName = Path.GetDirectoryName(assetPath);

            CreateDirectory(pathWithoutFileName);
        }

        /// <summary>
        /// Ensures the path exists inside "Assets/"
        /// </summary>
        /// <param name="assetDatabasePath">The path to create inside "Assets/"</param>
        public static void CreateDirectory(string assetDatabasePath)
        {
            // Ensure folder path is indeed a path
            if (string.IsNullOrEmpty(assetDatabasePath))
                throw new ArgumentException($"Folder path \"{assetDatabasePath}\" is null or empty.");
            
            //
            string cleanPath = assetDatabasePath.Replace('\\', '/');
            string[] splitDirectories = cleanPath.Split('/');

            if (splitDirectories[0] != "Assets")
                throw new ArgumentException($"Path must begin with \"Assets/\"");

            // Append each directory to parent directory in succession
            var path = "Assets";
            for (int i = 1; i < splitDirectories.Length; i++)
            {
                // Get the name of the next directory in the path
                var directoryName = splitDirectories[i];

                // Make sure to skip empty entries such as "//" or "my/path/"
                if (string.IsNullOrEmpty(directoryName))
                    continue;

                // Add directory to base
                var currPath = $"{path}/{directoryName}";

                // Create folder if it does not exist
                var doCreateFolder = !AssetDatabase.IsValidFolder(currPath);
                if (doCreateFolder)
                    AssetDatabase.CreateFolder(path, directoryName);

                // Update base path for next iteration
                path = currPath;
            }
        }

        /// <summary>
        /// Creates asset at path. Directory structure is created if it does not exist.
        /// </summary>
        /// <param name="object"></param>
        /// <param name="path"></param>
        public static void CreateAsset(UnityEngine.Object @object, string path)
        {
            CreateDirectoryForAsset(path);
            AssetDatabase.CreateAsset(@object, path);
        }

    }
}