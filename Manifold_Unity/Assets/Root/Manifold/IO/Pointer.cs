using System;
using System.IO;

namespace Manifold.IO
{
    [Serializable]
    public struct Pointer : IBinarySerializable
    {
        public int address;

        public string HexAddress => $"0x{address:X8}";


        public void Deserialize(BinaryReader reader)
        {
            reader.ReadX(ref address);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(address);
        }
    }
}
