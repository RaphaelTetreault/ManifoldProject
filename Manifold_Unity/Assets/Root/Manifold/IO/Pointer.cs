using System;
using System.IO;

namespace Manifold.IO
{
    [Serializable]
    public struct Pointer :
        IBinarySerializable,
        IEquatable<Pointer>,
        IPointer
    {
        // FIELDS
        private int address;

        // CONSTRUCTORS
        public Pointer(int address)
        {
            this.address = address;
        }
        public Pointer(long address)
        {
            this.address = (int)address;
        }

        // PROPERTIES
        public int Address { get => address; set => address = value; }
        public string PrintAddress => $"{Address:x8}";
        public bool IsNotNull => Address != 0;
        public bool IsNull => Address == 0;



        // OPERATORS
        public static implicit operator int(Pointer pointer)
        {
            return pointer.Address;
        }

        public static implicit operator Pointer(int address)
        {
            return new Pointer(address);
        }

        public static implicit operator Pointer(long address)
        {
            return new Pointer((int)address);
        }

        // METHODS
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
            return $"Pointer({PrintAddress})";
        }

        public override bool Equals(object obj)
        {
            return Equals((Pointer)obj);
        }

        public bool Equals(Pointer obj)
        {
            return obj.Address == Address;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
