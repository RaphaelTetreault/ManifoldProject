using System;
using System.IO;

namespace Manifold.IO
{
    [Serializable]
    public struct AddressRange
    {
        // FIELDS
        public long startAddress;
        public long endAddress;


        // PROPERTIES
        /// <summary>
        /// Creates a pointer to this address range.
        /// </summary>
        public Pointer Pointer => new Pointer(startAddress);
        public int Size => (int)(endAddress - startAddress);


        // METHODS
        public void RecordStartAddress(Stream stream)
        {
            startAddress = stream.Position;
        }

        public void RecordStartAddress(BinaryReader reader)
            => RecordStartAddress(reader.BaseStream);

        public void RecordStartAddress(BinaryWriter writer)
            => RecordStartAddress(writer.BaseStream);


        public void RecordEndAddress(Stream stream)
        {
            endAddress = stream.Position;
        }

        public void RecordEndAddress(BinaryReader reader)
            => RecordEndAddress(reader.BaseStream);

        public void RecordEndAddress(BinaryWriter writer)
            => RecordEndAddress(writer.BaseStream);


        public string PrintStartAddress(string prefix = "0x", string format = "x8")
        {
            return $"{prefix}{startAddress.ToString(format)}";
        }

        public string PrintEndAddress(string prefix = "0x", string format = "x8")
        {
            return $"{prefix}{endAddress.ToString(format)}";
        }

        public override string ToString()
        {
            return $"{nameof(AddressRange)}(Start: {startAddress:x8}, End: {endAddress:x8}, Size: {Size} 0x{Size:x})";
        }
    }
}
