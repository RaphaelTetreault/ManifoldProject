using StarkTools.IO;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//public abstract class AnalyzerSobjs<T> : AnalyzerSobj
//    where T : IBinarySerializable, IFile, new()
//{
//    [Header("Analyzer Settings")]
//    [SerializeField]
//    protected ImportMode analysis = ImportMode.ImportFilesFromFolderTree;

//    [SerializeField, BrowseFolderField]
//    protected string analysisFolder;

//    [SerializeField]
//    protected string queryFormat;

//    [SerializeField]
//    protected bool ignoreErrors;

//    [SerializeField, BrowseFileField(false)]
//    protected string[] importFiles;

//    public override void Analyze()
//    {
//        if (analysis != ImportMode.ImportFilesList)
//            importFiles = ImportSobj.GetFilesFromDirectory(analysis, analysisFolder, queryFormat);

//        var count = 0;
//        var total = importFiles.Length;
//        var title = $"Analyzing {total} {TypeName}";

//        foreach (var importFile in importFiles)
//        {
//            try
//            {
//                using (var fileStream = File.Open(importFile, read.mode, read.access, read.share))
//                {
//                    using (var reader = new BinaryReader(fileStream))
//                    {
//                        // Progress bar update
//                        var progress = count / (float)total;
//                        var currentIndexStr = (count + 1).ToString().PadLeft(total.ToString().Length);
//                        var info = $"({currentIndexStr}/{total}) {importFile}";
//                        EditorUtility.DisplayProgressBar(title, info, progress);

//                        T value = new T();
//                        reader.ReadX<T>(ref value, true);
//                        value.FileName = Path.GetFileName(importFile);
//                        CreateAnalysis(value);
//                    }
//                }
//            }
//            catch (Exception e)
//            {
//                var msg = $"Failed to read Index[{count}] {importFile})";
//                Debug.LogError(msg);

//                if (!ignoreErrors)
//                {
//                    EditorUtility.ClearProgressBar();
//                    bool proceed = EditorUtility.DisplayDialog("Import Error", msg, "Proceed", "Cancel");

//                    if (!proceed)
//                        throw e;
//                }
//            }
//            count++;
//        }
//        EditorUtility.ClearProgressBar();
//    }

//    public abstract void CreateAnalysis(T[] value);
//}


public abstract class AnalyzerSobj<T> : AnalyzerSobj
    where T : ScriptableObject, IBinarySerializable, IFile, new()
{
    [Header("Analyzer Settings")]
    [SerializeField]
    [BrowseFolderField]
    protected string destinationDirectory;
    [SerializeField]
    protected bool preserveFolderStructure = true;
    [SerializeField]
    protected T[] analysisSobjs;

    public void AnalyzeIndividual(Action<T> AnalyzeSobj)
    {
        var title = $"Analyzing {analysisSobjs.Length} {TypeName}";

        var count = 0;
        foreach (var sobj in analysisSobjs)
        {
            if (sobj is null)
                continue;

            CheckFolderHierarchy(sobj);
            DisplayProgressBar(title, count, analysisSobjs);
            AnalyzeSobj(sobj);

            count++;
        }
        EditorUtility.ClearProgressBar();
    }

    //public void WriteAnalysis(T sobj, string fileName, Action<T, StreamWriter> writeAnalysis)
    //{
    //    using (var fileStream = File.Open(fileName, write.mode, write.access, write.share))
    //    {
    //        using (var writer = new StreamWriter(fileStream))
    //        {
    //            writeAnalysis(sobj, writer);
    //        }
    //    }
    //}

    //public void WriteAnalysis(T[] sobjs, string fileName, Action<T[], StreamWriter> writeAnalysis)
    //{
    //    using (var fileStream = File.Open(fileName, write.mode, write.access, write.share))
    //    {
    //        using (var writer = new StreamWriter(fileStream))
    //        {
    //            writeAnalysis(sobjs, writer);
    //        }
    //    }
    //}

    public void DisplayProgressBar(string title, int index, T[] sobjs)
    {
        var sobj = sobjs[index];
        var total = sobjs.Length;

        // Get file without .asset
        var fileName = Path.GetFileNameWithoutExtension(sobj.FileName);

        // Progress bar update
        var progress = index / (float)total;
        var currentIndexStr = (index + 1).ToString().PadLeft(total.ToString().Length);
        var info = $"({currentIndexStr}/{total}) {fileName}";
        EditorUtility.DisplayProgressBar(title, info, progress);
    }

    public void CheckFolderHierarchy(T sobj)
    {
        if (preserveFolderStructure)
        {
            var folderPath = AssetDatabase.GetAssetPath(sobj);
            folderPath = UnityPathUtility.EnforceSystemSeparators(folderPath);
            folderPath = Path.GetDirectoryName(folderPath);
            var length = "Assets/".Length;
            folderPath = folderPath.Remove(0, length);
            var dest = UnityPathUtility.CombineSystemPath(destinationDirectory, folderPath);

            if (!Directory.Exists(dest))
            {
                Directory.CreateDirectory(dest);
                Debug.Log($"Created path <i>{dest}</i>");
            }
        }
    }

}