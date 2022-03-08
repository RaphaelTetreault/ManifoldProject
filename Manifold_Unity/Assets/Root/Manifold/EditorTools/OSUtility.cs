using System.IO;
using UnityEngine;

namespace Manifold.EditorTools
{
    public static class OSUtility
    {
        /// <summary>
        /// Opens the <paramref name="filePath"/> folder in OS. Will not create duplicate windows.
        /// </summary>
        /// <param name="filePath">Open folder window at this path.</param>
        public static void OpenDirectory(params string[] filePaths)
        {
            // 2022/01-15
            // Check to see if UnityEditor.RevealInFinder(path) is better

            foreach (string filePath in filePaths)
            {
                var sysPath = UnityPathUtility.EnforceSystemSeparators(filePath);
                var dirPath = Path.GetDirectoryName(sysPath);
                var uriPath = UnityPathUtility.EnforceUnitySeparators(dirPath);
                Application.OpenURL(@"file:///" + uriPath);
            }
        }

        public static void OpenDirectory(bool doOpen, params string[] filePaths)
        {
            if (doOpen)
            {
                OpenDirectory(filePaths);
            }
        }

    }
}