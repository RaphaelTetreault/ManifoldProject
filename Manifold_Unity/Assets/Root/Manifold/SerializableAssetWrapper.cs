using StarkTools.IO;
using System;
using System.IO;
using UnityEngine;

[Serializable]
public class SerializableAssetWrapper<T> : ScriptableObject, IBinarySerializable
    where T : IBinarySerializable
{
    [SerializeField]
    public T value;

    public void Deserialize(BinaryReader reader)
    {
        value.Deserialize(reader);
    }

    public void Serialize(BinaryWriter writer)
    {
        value.Serialize(writer);
    }
}