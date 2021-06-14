using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Manifold.IO
{
    public static class ExportUtility
    {
        public static string[] ExportFiles<TSobj>(TSobj[] exportSobjs, string exportDest, string extension, bool overwriteFiles, bool preserveFolderStructure = true, FileMode mode = FileMode.Create, FileAccess access = FileAccess.ReadWrite, FileShare share = FileShare.Read)
            where TSobj : ScriptableObject, IBinarySerializable, IFile
        {
            var exportedFiles = new List<string>();

            foreach (var exportSobj in exportSobjs)
            {
                if (exportSobj is null)
                    continue;

                var dest = exportDest;
                if (preserveFolderStructure)
                {
                    var folderPath = AssetDatabase.GetAssetPath(exportSobj);
                    folderPath = UnityPathUtility.EnforceSystemSeparators(folderPath);
                    folderPath = Path.GetDirectoryName(folderPath);
                    var length = "Assets/".Length;
                    folderPath = folderPath.Remove(0, length);
                    dest = UnityPathUtility.CombineSystemPath(dest, folderPath);

                    if (!Directory.Exists(dest))
                    {
                        Directory.CreateDirectory(dest);
                        Debug.Log($"Created path <i>{dest}</i>");
                    }
                }

                // Get file name without .asset
                var fileName = Path.GetFileNameWithoutExtension(exportSobj.FileName);
                var outputFilePath = string.IsNullOrEmpty(extension)
                    ? $"{dest}/{fileName}"
                    : $"{dest}/{fileName}.{extension}";
                outputFilePath = UnityPathUtility.EnforceSystemSeparators(outputFilePath);

                var fileExists = File.Exists(outputFilePath);
                // Fail if we DON'T want to overwrite files but file exists
                if (!overwriteFiles && fileExists)
                {
                    Debug.LogError($"Permission not set to overwrite \"{outputFilePath}\"!");
                    // set file as null so it can be removed from the list of exported files
                    outputFilePath = null;
                    continue;
                }
                else // Write / Overwrite files
                {
                    using (var fileStream = File.Open(outputFilePath, mode, access, share))
                    {
                        using (var writer = new BinaryWriter(fileStream))
                        {
                            exportSobj.Serialize(writer);
                        }
                    }
                    exportedFiles.Add(outputFilePath);
                }
            }

            return exportedFiles.ToArray();
        }

        public static string[] ExportSerializable<TBS>(TBS[] serializables, string exportDest, string extension, bool overwriteFiles, FileMode mode = FileMode.Create, FileAccess access = FileAccess.ReadWrite, FileShare share = FileShare.Read)
            where TBS : IBinarySerializable, IFile
        {
            var exportedFiles = new List<string>();

            foreach (var serializable in serializables)
            {
                if (serializable is null)
                    continue;                

                // Get file name without .asset
                var fileName = Path.GetFileNameWithoutExtension(serializable.FileName);
                var outputFilePath = string.IsNullOrEmpty(extension)
                    ? $"{exportDest}/{fileName}"
                    : $"{exportDest}/{fileName}.{extension}";
                outputFilePath = UnityPathUtility.EnforceSystemSeparators(outputFilePath);

                var fileExists = File.Exists(outputFilePath);
                // Fail if we DON'T want to overwrite files but file exists
                if (!overwriteFiles && fileExists)
                {
                    Debug.LogError($"Permission not set to overwrite \"{outputFilePath}\"!");
                    // set file as null so it can be removed from the list of exported files
                    outputFilePath = null;
                    continue;
                }
                else // Write / Overwrite files
                {
                    using (var fileStream = File.Open(outputFilePath, mode, access, share))
                    {
                        using (var writer = new BinaryWriter(fileStream))
                        {
                            serializable.Serialize(writer);
                        }
                    }
                    exportedFiles.Add(outputFilePath);
                }
            }

            return exportedFiles.ToArray();
        }

        public static string ExportSerializable<TBS>(TBS serializable, string exportDest, string extension, bool overwriteFiles, FileMode mode = FileMode.Create, FileAccess access = FileAccess.ReadWrite, FileShare share = FileShare.Read)
            where TBS : IBinarySerializable, IFile
        {
            var exportedFiles = ExportSerializable(new TBS[] { serializable }, exportDest, extension, overwriteFiles, mode, access, share);
            return exportedFiles[0];
        }


        public static void PrintExportsToConsole<T>(T sobj, string[] filePaths)
            where T : ExecutableScriptableObject, IExportable
        {
            foreach (var filePath in filePaths)
            {
                var sysPath = UnityPathUtility.EnforceSystemSeparators(filePath);
                Debug.Log($"{sobj.name} exported <color=blue>{sysPath}</color>");
            }
        }
    }
}