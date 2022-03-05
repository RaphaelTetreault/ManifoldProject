using System;
using System.IO;

namespace Manifold.IO
{
    [Serializable]
    public struct AddressRange
    {
        // FIELDS
        private long startAddress;
        private long endAddress;


        // PROPERTIES
        public Pointer GetPointer()
        {
            return new Pointer()
            {
                Address = (int)StartAddress
            };
        }

        public long StartAddress { get => startAddress; set => startAddress = value; }
        public long EndAddress { get => endAddress; set => endAddress = value; }
        public int Size => (int)(EndAddress - StartAddress);


        // METHODS
        public void RecordStartAddress(Stream stream)
        {
            StartAddress = stream.Position;
        }

        public void RecordStartAddress(BinaryReader reader)
            => RecordStartAddress(reader.BaseStream);

        public void RecordStartAddress(BinaryWriter writer)
            => RecordStartAddress(writer.BaseStream);


        public void RecordEndAddress(Stream stream)
        {
            EndAddress = stream.Position;
        }

        public void RecordEndAddress(BinaryReader reader)
            => RecordEndAddress(reader.BaseStream);

        public void RecordEndAddress(BinaryWriter writer)
            => RecordEndAddress(writer.BaseStream);



        public override string ToString()
        {
            return $"Start: {StartAddress:x8}, End: {EndAddress:x8}, Size: {Size} ({Size:x})";
        }
    }
}
