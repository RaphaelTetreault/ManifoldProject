using System;
using System.IO;

namespace Manifold.IO
{
    [Serializable]
    public struct Pointer : IBinarySerializable, IPointer
    {
        public int address;

        //public Pointer()
        //{
        //    address = 0;
        //}

        public Pointer(int address)
        {
            this.address = address;
        }
        public int Address => address;
        public string HexAddress => $"0x{address:X8}";
        public bool IsNotNullPointer => address != 0;


        public void Deserialize(BinaryReader reader)
        {
            reader.ReadX(ref address);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(address);
        }

        public override string ToString()
        {
            return HexAddress;
        }
    }
}
