using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.GMA
{
    [Serializable]
    public struct IntArray : IBinarySerializable
    {
        public int count;
        public int[] addresses;

        public void Deserialize(BinaryReader reader)
        {
            reader.ReadX(ref count);
            reader.ReadX(ref addresses, count);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(count);
            writer.WriteX(addresses, false);
        }
    }
}