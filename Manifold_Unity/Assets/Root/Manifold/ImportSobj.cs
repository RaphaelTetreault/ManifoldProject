using StarkTools.IO;
using System.IO;
using UnityEngine;
using UnityEditor;

public abstract class ImportSobj : ScriptableObject
{
    [Header("File Read")]
    [SerializeField]
    protected FileStreamSettings Read = FileStreamSettings.Read;

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
        AssetDatabase.ImportAsset(filePath, ImportAssetOptions.ForceUpdate);
        sobj.Deserialize(reader);
        return sobj;
    }

    #endregion
}
