using StarkTools.IO;
using System;
using System.IO;
using UnityEngine;

[Serializable]
public class FileAssetWrapper<T> : ScriptableObject, IBinarySerializable, IFile
    where T : IBinarySerializable, IFile
{
    [SerializeField]
    private T value;

    public T Value
        => value;

    public string FileName
    {
        get => value.FileName;
        set => this.value.FileName = value;
    }

    public static implicit operator T(FileAssetWrapper<T> sobj)
    {
        return sobj.value;
    }

    public void Deserialize(BinaryReader reader)
    {
        value.Deserialize(reader);
    }

    public void Serialize(BinaryWriter writer)
    {
        value.Serialize(writer);
    }

    public void SetValue(T value)
    {
        this.value = value;
    }
}
