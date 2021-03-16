using System;
using System.IO;

namespace Manifold.IO
{
    [Serializable]
    public struct ArrayPointer : IBinarySerializable, IPointer
    {
        public int length;
        public int address;


        public int Address => address;

        public string HexAddress => $"0x{address:X8}";

        public bool IsNotNullPointer => address != 0;


        public void Deserialize(BinaryReader reader)
        {
            reader.ReadX(ref length);
            reader.ReadX(ref address);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(length);
            writer.WriteX(address);
        }

        public override string ToString()
        {
            return $"Length: {length}, Address: {HexAddress}";
        }
    }
}
