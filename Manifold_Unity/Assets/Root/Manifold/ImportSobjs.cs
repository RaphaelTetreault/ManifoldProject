using StarkTools.IO;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public enum ImportMode
{
    ImportFilesList,
    ImportFilesFromFolder,
    ImportFilesFromFolderTree,
}

public abstract class ImportSobjs<T> : ImportSobj
        where T : ScriptableObject, IBinarySerializable, INamedFile
{
    [Header("Import Settings")]
    [SerializeField]
    protected ImportMode importMode = ImportMode.ImportFilesFromFolderTree;

    [SerializeField, BrowseFolderField]
    protected string importFolder;

    [SerializeField, BrowseFolderField("Assets/")]
    protected string destinationDirectory;


    [SerializeField]
    protected string queryFormat;

    [SerializeField]
    protected bool ignoreErrors;

    [SerializeField, BrowseFileField(false)]
    protected string[] importFiles;

    // PROPERTIES
    protected abstract string DefaultQueryFormat
    {
        get;
    }

    // METHODS
    public override void Import()
    {
        if (importMode != ImportMode.ImportFilesList)
            importFiles = GetFilesFromDirectory(importMode, importFolder, queryFormat);

        var count = 0;
        var total = importFiles.Length;
        var title = $"Importing {total} {TypeName}";

        foreach (var importFile in importFiles)
        {
            try
            {
                using (var fileStream = File.Open(importFile, read.mode, read.access, read.share))
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
                            dest = dest + folder;

                        if (!Directory.Exists(dest))
                        {
                            Directory.CreateDirectory(dest);
                            Debug.Log($"Created path <i>{dest}</i>");
                        }

                        var unityPath = UnityPathUtility.ToUnityFolderPath(dest, UnityPathUtility.UnityFolder.Assets);
                        var fileName = Path.GetFileNameWithoutExtension(importFile);
                        var sobj = CreateFromBinary<T>(unityPath, fileName, reader);
                        sobj.FileName = fileName;

                        // Progress bar update
                        var filePath = AssetDatabase.GetAssetPath(sobj);
                        var progress = count / (float)total;
                        var currentIndexStr = (count + 1).ToString().PadLeft(total.ToString().Length);
                        var info = $"({currentIndexStr}/{total}) {unityPath}/{fileName}";
                        EditorUtility.DisplayProgressBar(title, info, progress);
                        EditorUtility.SetDirty(sobj);
                    }
                }
            }
            catch (Exception e)
            {
                var msg = $"Failed to read Index[{count}] {importFile})";
                Debug.LogError(msg);

                if (!ignoreErrors)
                {
                    EditorUtility.ClearProgressBar();
                    bool proceed = EditorUtility.DisplayDialog("Import Error", msg, "Proceed", "Cancel");

                    if (!proceed)
                        throw e;
                }
            }
            count++;
        }
        AssetDatabase.SaveAssets();
        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }

    protected virtual void Reset()
    {
        queryFormat = DefaultQueryFormat;
    }
}
