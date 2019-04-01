using StarkTools.IO;
using System;
using System.IO;
using UnityEngine;
using UnityEditor;

public abstract class ImportSobj : ScriptableObject
{
    [Header("File Read")]
    [SerializeField]
    protected FileStreamSettings read = FileStreamSettings.Read;

    #region PROPERTIES

    public virtual string ButtonText
    {
        get
        {
            return "Import";
        }
    }

    public abstract string ProcessMessage
    {
        get;
    }

    public abstract string HelpBoxMessage
    {
        get;
    }

    public abstract string TypeName
    {
        get;
    }

    #endregion

    #region METHODS 

    public abstract void Import();

    public static TSobj Create<TSobj>(string destinationDir, string fileName)
        where TSobj : ScriptableObject
    {
        var sobj = CreateInstance<TSobj>();
        var filePath = $"{destinationDir}/{fileName}.asset";
        AssetDatabase.CreateAsset(sobj, filePath);
        return sobj;
    }

    public static TSobj CreateFromBinary<TSobj>(string destinationDir, string fileName, BinaryReader reader)
        where TSobj : ScriptableObject, IBinarySerializable
    {
        var sobj = CreateInstance<TSobj>();
        var filePath = $"Assets/{destinationDir}/{fileName}.asset";
        AssetDatabase.CreateAsset(sobj, filePath);
        sobj.Deserialize(reader);
        return sobj;
    }

    public static TSobj CreateFromBinaryFile<TSobj>(string destinationDir, string fileName, BinaryReader reader)
    where TSobj : ScriptableObject, IBinarySerializable, IFile
    {
        var sobj = CreateInstance<TSobj>();
        var filePath = $"Assets/{destinationDir}/{fileName}.asset";
        AssetDatabase.CreateAsset(sobj, filePath);
        sobj.FileName = fileName;
        sobj.Deserialize(reader);
        return sobj;
    }


    /// <summary>
    /// TODO: move this somewhere else
    /// </summary>
    /// <param name="importMode"></param>
    /// <param name="importFolder"></param>
    /// <param name="queryFormat"></param>
    /// <returns></returns>
    public static string[] GetFilesFromDirectory(ImportMode importMode, string importFolder, string queryFormat)
    {
        switch (importMode)
        {
            case ImportMode.ImportFilesFromFolder:
                return Directory.GetFiles(importFolder, queryFormat, SearchOption.TopDirectoryOnly);

            case ImportMode.ImportFilesFromFolderTree:
                return Directory.GetFiles(importFolder, queryFormat, SearchOption.AllDirectories);

            case ImportMode.ImportFilesList:
                throw new ArgumentException();

            default:
                throw new NotImplementedException();
        }
    }

    #endregion
}
