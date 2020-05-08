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

        public static void ExportFilesFrom<TSobj>(TSobj[] exportSobjs, string exportDest, string extension, bool preserveFolderStructure = true, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.ReadWrite, FileShare share = FileShare.Read)
            where TSobj : ScriptableObject, IBinarySerializable, IFile
        {
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

                // Get file without .asset
                var fileName = Path.GetFileNameWithoutExtension(exportSobj.FileName);
                var outputFilePath = $"{dest}/{fileName}.{extension}";
                outputFilePath = UnityPathUtility.EnforceSystemSeparators(outputFilePath);

                using (var fileStream = File.Open(outputFilePath, mode, access, share))
                {
                    using (var writer = new BinaryWriter(fileStream))
                    {
                        exportSobj.Serialize(writer);
                    }
                }
            }
        }

        public static TSobj[] LoadAllOfTypeFromAssetDatabase<TSobj>(string[] searchInFolders = null)
            where TSobj : ScriptableObject
        {
            var searchQuery = $"t:{typeof(GMASobj).Name}";
            string[] allAssetsOfType = AssetDatabase.FindAssets(searchQuery, searchInFolders);
            var list = new List<TSobj>();
            foreach (var assetGuid in allAssetsOfType)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
                var asset = AssetDatabase.LoadAssetAtPath<TSobj>(assetPath);
                list.Add(asset);
            }
            list.ToArray();
            return list.ToArray();
        }
    }
}