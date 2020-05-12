using StarkTools.IO;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public abstract class AnalyzerSobj<T> : AnalyzerSobj
    where T : ScriptableObject, IBinarySerializable, IFile, new()
{
    [Header("Analyzer Settings")]
    [SerializeField]
    protected bool loadAllOfType = false;
    [SerializeField, BrowseFolderField]
    protected string destinationDirectory;
    [SerializeField]
    protected bool preserveFolderStructure = true;
    [SerializeField]
    protected T[] analysisSobjs;


    public T[] LoadAllOfType()
    {
        var assetGUIDs = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
        var assetCount = assetGUIDs.Length;
        var assets = new T[assetCount];
        for (int i = 0; i < assetCount; i++)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(assetGUIDs[i]);
            var asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            assets[i] = asset;
        }

        return assets;
    }

    public StreamWriter OpenWriter(string fileName)
    {
        var writeFile = Path.Combine(destinationDirectory, fileName);
        var fileStream = File.Open(writeFile, write.mode, write.access, write.share);
        var writer = new StreamWriter(fileStream);
        return writer;
    }

    public string FileTimestamp()
    {
        return DateTime.Now.ToString("(yyyy-MM-dd)-(HH-mm-ss)");
    }
}