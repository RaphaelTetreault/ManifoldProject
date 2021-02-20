using System.IO;
using UnityEngine;

namespace Manifold.IO
{
    public static class IOUtility
    {
        /// <summary>
        /// Opens the <paramref name="filePath"/> folder in OS. Will not create duplicate windows.
        /// </summary>
        /// <param name="filePath">Open folder window at this path.</param>
        public static void OpenDirectoryOS(params string[] filePaths)
        {
            foreach (string filePath in filePaths)
            {
                var sysPath = UnityPathUtility.EnforceSystemSeparators(filePath);
                var dirPath = Path.GetDirectoryName(sysPath);
                var uriPath = UnityPathUtility.EnforceUnitySeparators(dirPath);
                Application.OpenURL(@"file:///" + uriPath);
            }
        }

        public static void OpenOSDirectory(bool doOpen, params string[] filePaths)
        {
            if (doOpen)
            {
                OpenDirectoryOS(filePaths);
            }
        }

    }
}