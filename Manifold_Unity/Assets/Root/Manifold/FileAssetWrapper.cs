using StarkTools.IO;
using System;
using System.IO;
using UnityEngine;

[Serializable]
public class FileAssetWrapper<T> : ScriptableObject, IBinarySerializable, IFile
    where T : IBinarySerializable, IFile
{
    [SerializeField]
    public T value;

    public T Value
        => value;

    public string FileName
    {
        get => value.FileName;
        set => this.value.FileName = value;
    }

    public void Deserialize(BinaryReader reader)
    {
        value.Deserialize(reader);
    }

    public void Serialize(BinaryWriter writer)
    {
        value.Serialize(writer);
    }
}
