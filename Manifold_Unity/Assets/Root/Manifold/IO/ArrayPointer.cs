using System;
using System.IO;

namespace Manifold.IO
{
    [Serializable]
    public struct ArrayPointer :
        IBinarySerializable,
        IPointer
    {
        // FIELDS
        public int length;
        public int address;

        // CONSTRUCTORS
        public ArrayPointer(int length = 0, int address = 0)
        {
            this.length = length;
            this.address = address;
        }

        // PROPERTIES
        int IPointer.Address => address;
        public bool IsNotNull => address != 0;
        public bool IsNull => address == 0;
        public Pointer Pointer => new(address);


        public string PrintAddress => $"{address:x8}";



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
            return $"Length: {length}, Address: {PrintAddress}";
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
