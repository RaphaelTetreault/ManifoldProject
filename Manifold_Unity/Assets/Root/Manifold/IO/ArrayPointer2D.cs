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
    public sealed class ArrayPointer2D :
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
            AddressRange.RecordStartAddress(reader);
            {
                // Read array lengths
                for (int i = 0; i < arrayPointers.Length; i++)
                {
                    int length = 0;
                    reader.ReadX(ref length);
                    arrayPointers[i].length = length;
                }

                // Read array addresses
                for (int i = 0; i < arrayPointers.Length; i++)
                {
                    int address = 0;
                    reader.ReadX(ref address);
                    arrayPointers[i].address = address;
                }
            }
            AddressRange.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            AddressRange.RecordStartAddress(writer);
            {
                // Write array lengths
                for (int i = 0; i < arrayPointers.Length; i++)
                {
                    writer.WriteX(arrayPointers[i].length);
                }

                // Write array addresses
                for (int i = 0; i < arrayPointers.Length; i++)
                {
                    writer.WriteX(arrayPointers[i].address);
                }
            }
            AddressRange.RecordEndAddress(writer);
        }

    }
}
