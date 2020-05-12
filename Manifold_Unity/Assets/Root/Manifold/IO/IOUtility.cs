using System;
using System.IO;
using UnityEngine;

namespace Manifold.IO
{
    public static class IOUtility
    {
        public static T[] GetSobjByOption<T>(T[] sobjs, IOOption option, string[] searchFolders)
            where T : ScriptableObject
        {
            switch (option)
            {
                case IOOption.selectedFiles:
                    return sobjs;

                case IOOption.allFromSourceFolder:
                    return AssetDatabaseUtility.GetAllOfType<T>(searchFolders);

                case IOOption.allFromAssetDatabase:
                    return AssetDatabaseUtility.GetAllOfType<T>();

                default:
                    throw new NotImplementedException();
            }
        }

        public static T[] GetSobjByOption<T>(T[] sobjs, IOOption option, string searchFolders)
            where T : ScriptableObject
        {
            return GetSobjByOption(sobjs, option, new string[] { searchFolders });
        }

        public static T[] GetSobjByOption<T>(T[] sobjs, IOOption option)
            where T : ScriptableObject
        {
            return GetSobjByOption(sobjs, option, new string[0]);
        }

        /// <summary>
        /// Opens the <paramref name="filePath"/> folder in OS. Will not create duplicate windows.
        /// </summary>
        /// <param name="filePath">Open folder window at this path.</param>
        public static void OpenFileFolder(string filePath)
        {
            var sysPath = UnityPathUtility.EnforceSystemSeparators(filePath);
            var dirPath = Path.GetDirectoryName(sysPath);
            var uriPath = UnityPathUtility.EnforceUnitySeparators(dirPath);
            Application.OpenURL(@"file:///" + uriPath);
        }

        /// <summary>
        /// Opens the <paramref name="filePaths"/> folders in OS. Will not create duplicate windows.
        /// </summary>
        /// <param name="filePath">Open folder window at this path.</param>
        public static void OpenFileFolder(string[] filePaths)
        {
            foreach (string filePath in filePaths)
            {
                OpenFileFolder(filePath);
            }
        }

    }
}