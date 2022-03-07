using System;
using System.IO;

namespace Manifold.IO
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Given a 2D array size [n,m]; the underlying binary is represented as 'n' lengths followed after
    /// by 'n' pointers. Once paired, the length and pointer form an <cref>ArrayPointer</cref>.
    /// </remarks>
    [Serializable]
    public struct ArrayPointer2D :
        IBinaryAddressable,
        IBinarySerializable
    {
        // FIELDS
        private ArrayPointer[] arrayPointers;


        // CONSTRUCTORS
        public ArrayPointer2D(int length = 0)
        {
            AddressRange = new AddressRange();
            arrayPointers = new ArrayPointer[length];
        }
        public ArrayPointer2D(ArrayPointer[] arrayPointers)
        {
            AddressRange = new AddressRange();
            this.arrayPointers = arrayPointers;
        }


        // INDEXERS
        public ArrayPointer this[int index] { get => arrayPointers[index]; set => arrayPointers[index] = value; }

        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public ArrayPointer[] ArrayPointers { get => arrayPointers; set => arrayPointers = value; }
        public int Length => arrayPointers.Length;


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                // Read array lengths
                for (int i = 0; i < arrayPointers.Length; i++)
                {
                    int length = 0;
                    reader.ReadX(ref length);
                    arrayPointers[i].Length = length;
                }

                // Read array addresses
                for (int i = 0; i < arrayPointers.Length; i++)
                {
                    int address = 0;
                    reader.ReadX(ref address);
                    arrayPointers[i].Address = address;
                }
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                // Write array lengths
                for (int i = 0; i < arrayPointers.Length; i++)
                {
                    writer.WriteX(arrayPointers[i].Length);
                }

                // Write array addresses
                for (int i = 0; i < arrayPointers.Length; i++)
                {
                    writer.WriteX(arrayPointers[i].Address);
                }
            }
            this.RecordEndAddress(writer);
        }

    }
}
