using StarkTools.IO;
using System;
using System.IO;
using UnityEngine;
using UnityEditor;

public abstract class ExportSobjs<T> : ExportSobj
    where T : ScriptableObject, IBinarySerializable, IFile
{
    [Header("Export Settings")]
    [SerializeField]
    [BrowseFolderField]
    protected string destinationDirectory;
    [SerializeField]
    protected bool preserveFolderStructure = true;
    [SerializeField]
    protected T[] exportSobjs;


    public override string ProcessMessage => "Export complete";


    public override void Export()
    {
        foreach (var exportSobj in exportSobjs)
        {
            if (exportSobj is null)
            {
                Debug.LogWarning($"Did not export null entry in {name}");
                continue;
            }

            var dest = destinationDirectory;
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
            var outputFilePath = $"{dest}/{fileName}.{OutputFileExtension}";
            outputFilePath = UnityPathUtility.EnforceSystemSeparators(outputFilePath);

            //try
            //{
                using (var fileStream = File.Open(outputFilePath, write.mode, write.access, write.share))
                {
                    using (var writer = new BinaryWriter(fileStream))
                    {
                        exportSobj.Serialize(writer);
                    }
                }
            //}
            //catch (Exception e)
            //{
            //    Debug.LogError($"Failed to write <b>{outputFilePath}</b>.\n{e.Message}");
            //}
        }
    }
}