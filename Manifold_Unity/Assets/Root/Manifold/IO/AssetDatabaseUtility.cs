using UnityEditor;
using UnityEngine;

namespace Manifold.IO
{
    public static class AssetDatabaseUtility
    {
        public static T[] GetAllOfType<T>(string[] searchInFolders)
            where T : ScriptableObject
        {
            for (int i = 0; i < searchInFolders.Length; i++)
            {
                searchInFolders[i] = $"Assets/{searchInFolders[i]}";
            }

            var guids = searchInFolders != null || searchInFolders.Length > 0
                ? AssetDatabase.FindAssets($"t:{typeof(T).Name}", searchInFolders)
                : AssetDatabase.FindAssets($"t:{typeof(T).Name}");

            var assets = new T[guids.Length];
            for (int i = 0; i < assets.Length; i++)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                assets[i] = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            }
            return assets;
        }

        public static T[] GetAllOfType<T>(string searchInFolder)
            where T : ScriptableObject
        {
            return GetAllOfType<T>(new string[] { searchInFolder });
        }

        public static T[] GetAllOfType<T>()
            where T : ScriptableObject
        {
            return GetAllOfType<T>(new string[0]);
        }

    }
}