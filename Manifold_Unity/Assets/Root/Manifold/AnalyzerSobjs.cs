using StarkTools.IO;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class AnalyzerSobjs<T> : AnalyzerSobj
    where T : IBinarySerializable, INamedFile, new()
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
            //try
            //{
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
            //}
            //catch (Exception e)
            //{
            //    var msg = $"Failed to read Index[{count}] {importFile})";
            //    Debug.LogError(msg);

            //    if (!ignoreErrors)
            //    {
            //        EditorUtility.ClearProgressBar();
            //        bool proceed = EditorUtility.DisplayDialog("Import Error", msg, "Proceed", "Cancel");

            //        if (!proceed)
            //            throw e;
            //    }
            //}
            count++;
        }
        EditorUtility.ClearProgressBar();
    }

    public abstract void CreateAnalysis(T value);
}
