using StarkTools.IO;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GMASobj : ManifoldAsset<GMA>
{

}

public class ManifoldAsset<T>: ScriptableObject, IBinarySerializable
    where T : class, IBinarySerializable
{
    [SerializeField]
    protected T value;

    public T Value => value;

    public void Deserialize(BinaryReader reader)
    {
        value.Deserialize(reader);
    }

    public void Serialize(BinaryWriter writer)
    {
        value.Serialize(writer);
    }
}
