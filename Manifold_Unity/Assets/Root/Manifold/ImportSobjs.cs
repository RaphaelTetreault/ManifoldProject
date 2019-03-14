using StarkTools.IO;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public abstract class ImportSobjs<T> : ImportSobj
        where T : ScriptableObject, IBinarySerializable, INamedFile
{
    public enum ImportMode
    {
        ImportFilesList,
        ImportFilesFromFolder,
        ImportFilesFromFolderTree,
    }

    [Header("Export Settings")]
    [SerializeField, BrowseFolderField("Assets/")]
    protected string destinationDirectory;

    [SerializeField]
    protected ImportMode importMode = ImportMode.ImportFilesFromFolderTree;

    [SerializeField, BrowseFolderField]
    protected string importFolder;

    [SerializeField]
    protected string extension;

    [SerializeField, BrowseFileField(false)]
    protected string[] importFiles;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="extensions">Extension without period</param>
    /// <returns></returns>
    public bool FileExtensionMatch(string filePath, params string[] extensions)
    {
        var fileExtension = Path.GetExtension(filePath);

        foreach (var extension in extensions)
        {
            if (extension == fileExtension)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="files"></param>
    /// <param name="extension">Extension without period</param>
    /// <returns></returns>
    public string[] GetFilesOfExtension(string[] files, string extension)
    {
        var matchingFiles = new List<string>(files.Length);
        extension = $".{extension}";

        foreach (var file in files)
        {
            if (FileExtensionMatch(file, extension))
            {
                var systemFile = UnityPathUtility.EnforceSystemSeparators(file);
                matchingFiles.Add(systemFile);
            }
        }

        return matchingFiles.ToArray();
    }

    public override void Import()
    {
        switch (importMode)
        {
            case ImportMode.ImportFilesList:
                break;

            case ImportMode.ImportFilesFromFolder:
                var files = Directory.GetFiles(importFolder);
                importFiles = GetFilesOfExtension(files, extension);
                break;

            case ImportMode.ImportFilesFromFolderTree:
                var entries = Directory.GetFileSystemEntries(importFolder);
                importFiles = GetFilesOfExtension(entries, extension);
                break;

            default:
                throw new NotImplementedException();
        }

        foreach (var importFile in importFiles)
        {
            try
            {
                using (var fileStream = File.Open(importFile, Read.mode, Read.access, Read.share))
                {
                    using (var reader = new BinaryReader(fileStream))
                    {
                        // Get path to root import folder
                        var path = UnityPathUtility.GetUnityDirectory(UnityPathUtility.UnityFolder.Assets);
                        var dest = UnityPathUtility.CombineSystemPath(path, destinationDirectory);

                        // get path to file import folder
                        // TODO: Regex instead of this hack
                        var length = importFolder.Length;
                        var folder = importFile.Remove(0, length);
                        folder = Path.GetDirectoryName(folder);

                        // (A) prevent null/empty AND (B) prevent "/" or "\\"
                        if (!string.IsNullOrEmpty(folder) && folder.Length > 1) 
                            dest = UnityPathUtility.CombineSystemPath(dest, folder);

                        if (!Directory.Exists(dest))
                        {
                            Directory.CreateDirectory(dest);
                            Debug.Log($"Created path <i>{dest}</i>");
                        }

                        var fileName = Path.GetFileNameWithoutExtension(importFile);
                        var sobj = CreateFromBinary<T>(destinationDirectory, fileName, reader);
                        sobj.FileName = fileName;
                        var filePath = UnityEditor.AssetDatabase.GetAssetPath(sobj);
                        AssetDatabase.ImportAsset(filePath, ImportAssetOptions.ForceUpdate);
                        AssetDatabase.SaveAssets();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to read <b>{importFile}</b>)");
                throw e;
            }
        }
    }
}
