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
            get => new Pointer() { Address = address };
        }

        public string HexAddress => $"0x{address:X8}";

        public bool IsNotNull => address != 0;
        public bool IsNull => address == 0;


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

        public static bool operator ==(ArrayPointer lhs, ArrayPointer rhs)
        {
            return lhs.address == rhs.address && lhs.length == rhs.length;
        }

        public static bool operator !=(ArrayPointer lhs, ArrayPointer rhs)
        {
            return (lhs.address == rhs.address && lhs.length == rhs.length);
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(ArrayPointer))
                return false;

            return (ArrayPointer)obj == this;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
