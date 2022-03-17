using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace Manifold.EditorTools
{
    [Serializable]
    public class SerializableAssetWrapper<T> : ScriptableObject, IBinarySerializable
        where T : IBinarySerializable
    {
        [SerializeField]
        public T value;

        public static implicit operator T(SerializableAssetWrapper<T> sobj)
        {
            return sobj.value;
        }

        public void Deserialize(EndianBinaryReader reader)
        {
            value.Deserialize(reader);
        }

        public void Serialize(EndianBinaryWriter writer)
        {
            value.Serialize(writer);
        }
    }
}
