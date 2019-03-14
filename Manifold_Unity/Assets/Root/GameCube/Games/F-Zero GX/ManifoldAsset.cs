using StarkTools.IO;
using System.IO;
using UnityEngine;

[System.Serializable]
public class ManifoldAsset<T> : ScriptableObject, IBinarySerializable, INamedFile
    where T : IBinarySerializable, INamedFile
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
