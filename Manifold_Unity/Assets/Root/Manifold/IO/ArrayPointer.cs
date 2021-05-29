using System;
using System.IO;

namespace Manifold.IO
{
    [Serializable]
    public struct ArrayPointer : IBinarySerializable, IPointer
    {
        [UnityEngine.SerializeField] private int length;
        [UnityEngine.SerializeField] private int address;

        public ArrayPointer(int length = 0, int address = 0)
        {
            this.length = length;
            this.address = address;
        }

        public int Length
        {
            get => length;
            set => length = value;
        }

        public int Address
        {
            get => address;
            set => address = value;
        }

        public Pointer Pointer
        {
            get => new Pointer() { address = address };
        }

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
            return $"Length: {Length}, Address: {HexAddress}";
        }
    }
}
