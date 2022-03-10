using System;
using System.IO;

namespace Manifold.IO
{
    [Serializable]
    public struct Offset :
        IBinarySerializable,
        IEquatable<Offset>,
        IOffset
    {
        // FIELDS
        public int addressOffset;

        // CONSTRUCTORS
        public Offset(int offset)
        {
            this.addressOffset = offset;
        }

        public Offset(long offset)
        {
            this.addressOffset = (int)offset;
        }


        // PROPERTIES
        int IOffset.AddressOffset => addressOffset;
        public bool IsNotNull => addressOffset != 0;
        public bool IsNull => addressOffset == 0;
        public string PrintAddressOffset => $"{addressOffset:x8}";


        // OPERATORS
        public static implicit operator int(Offset offset)
        {
            return offset.addressOffset;
        }

        public static implicit operator Offset(int addressOffset)
        {
            return new Offset(addressOffset);
        }

        public static implicit operator Offset(long addressOffset)
        {
            return new Offset(addressOffset);
        }


        //METHODS
        public static Pointer CreatePointer(Offset lhs, Offset rhs)
        {
            return new Pointer(lhs.addressOffset + rhs.addressOffset);
        }
        public static Pointer CreatePointer(Pointer lhs, Offset rhs)
        {
            return new Pointer(lhs.address + rhs.addressOffset);
        }
        public static Pointer CreatePointer(Offset lhs, Pointer rhs)
        {
            return new Pointer(lhs.addressOffset + rhs.address);
        }


        public void Deserialize(BinaryReader reader)
        {
            reader.ReadX(ref addressOffset);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(addressOffset);
        }

        public override string ToString()
        {
            return PrintAddressOffset;
        }

        public override bool Equals(object obj)
        {
            return Equals((Offset)obj);
        }

        public bool Equals(Offset obj)
        {
            return obj.addressOffset == this.addressOffset;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}