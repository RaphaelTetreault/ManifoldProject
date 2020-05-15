using UnityEditor;
using UnityEngine;

namespace Manifold.IO
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

    }
}