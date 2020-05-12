using StarkTools.IO;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Manifold.IO
{
    public static class ExportUtility
    {
        public enum ExportOptions
        {
            ExportFiles,
            ExportAllOfType,
            ExportAllOfTypeInFolder,
        }

        public static string[] ExportFiles<TSobj>(TSobj[] exportSobjs, string exportDest, string extension, bool overwriteFiles, bool preserveFolderStructure = true, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.ReadWrite, FileShare share = FileShare.Read)
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

        //public static TSobj[] LoadAllOfTypeFromAssetDatabase<TSobj>(string[] searchInFolders = null)
        //    where TSobj : ScriptableObject
        //{
        //    var searchQuery = $"t:{typeof(GMASobj).Name}";
        //    string[] allAssetsOfType = AssetDatabase.FindAssets(searchQuery, searchInFolders);
        //    var list = new List<TSobj>();
        //    foreach (var assetGuid in allAssetsOfType)
        //    {
        //        var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
        //        var asset = AssetDatabase.LoadAssetAtPath<TSobj>(assetPath);
        //        list.Add(asset);
        //    }
        //    list.ToArray();
        //    return list.ToArray();
        //}
    }
}