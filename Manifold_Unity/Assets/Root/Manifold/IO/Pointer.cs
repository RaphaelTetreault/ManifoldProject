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
        public int address;

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
        int IPointer.Address => address;
        public string PrintAddress => $"{address:x8}";
        public bool IsNotNull => address != 0;
        public bool IsNull => address == 0;



        // OPERATORS
        public static implicit operator int(Pointer pointer)
        {
            return pointer.address;
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
            return obj.address == address;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
