using StarkTools.IO;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class AnalyzerSobjs<T> : AnalyzerSobj
    where T : IBinarySerializable, IFile, new()
{
    [Header("Analyzer Settings")]
    [SerializeField]
    protected ImportMode analysis = ImportMode.ImportFilesFromFolderTree;

    [SerializeField, BrowseFolderField]
    protected string analysisFolder;

    [SerializeField]
    protected string queryFormat;

    [SerializeField]
    protected bool ignoreErrors;

    [SerializeField, BrowseFileField(false)]
    protected string[] importFiles;

    public override void Analyze()
    {
        if (analysis != ImportMode.ImportFilesList)
            importFiles = ImportSobj.GetFilesFromDirectory(analysis, analysisFolder, queryFormat);

        var count = 0;
        var total = importFiles.Length;
        var title = $"Analyzing {total} {TypeName}";

        foreach (var importFile in importFiles)
        {
            try
            {
                using (var fileStream = File.Open(importFile, Read.mode, Read.access, Read.share))
                {
                    using (var reader = new BinaryReader(fileStream))
                    {
                        // Progress bar update
                        var progress = count / (float)total;
                        var currentIndexStr = (count + 1).ToString().PadLeft(total.ToString().Length);
                        var info = $"({currentIndexStr}/{total}) {importFile}";
                        EditorUtility.DisplayProgressBar(title, info, progress);

                        T value = new T();
                        reader.ReadX<T>(ref value, true);
                        value.FileName = Path.GetFileName(importFile);
                        CreateAnalysis(value);
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
        EditorUtility.ClearProgressBar();
    }

    public abstract void CreateAnalysis(T value);
}


public abstract class NewAnalyzerSobjs<T> : AnalyzerSobj
    where T : ScriptableObject, IBinarySerializable, IFile, new()
{
    [Header("Analyzer Settings")]
    [SerializeField]
    [BrowseFolderField]
    protected string destinationDirectory;
    [SerializeField]
    protected bool preserveFolderStructure = true;
    [SerializeField]
    protected T[] analyseSobjs;

    //[SerializeField]
    //protected ImportMode analysis = ImportMode.ImportFilesFromFolderTree;
    //[SerializeField]
    //protected string queryFormat;

    //// PROPERTIES
    //protected abstract string DefaultQueryFormat
    //{
    //    get;
    //}

    //public virtual void FindAllReferences()
    //{
    //    var files = ImportSobj.GetFilesFromDirectory(analysis, UnityPathUtility.ProjectRootDirectory, queryFormat);
    //}

    public override void Analyze()
    {
        var count = 0;
        var total = analyseSobjs.Length;
        var title = $"Analyzing {total} {TypeName}";

        foreach (var sobj in analyseSobjs)
        {
            if (sobj is null)
                continue;

            var dest = destinationDirectory;
            if (preserveFolderStructure)
            {
                var folderPath = AssetDatabase.GetAssetPath(sobj);
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
            var fileName = Path.GetFileNameWithoutExtension(sobj.FileName);

            // Progress bar update
            var progress = count / (float)total;
            var currentIndexStr = (count + 1).ToString().PadLeft(total.ToString().Length);
            var info = $"({currentIndexStr}/{total}) {fileName}";
            EditorUtility.DisplayProgressBar(title, info, progress);
            CreateAnalysis(sobj);

            count++;
        }
        EditorUtility.ClearProgressBar();
    }

    public abstract void CreateAnalysis(T value);
}