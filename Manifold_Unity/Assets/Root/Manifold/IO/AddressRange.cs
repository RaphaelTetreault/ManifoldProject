using System;
using System.IO;

namespace Manifold.IO
{
    [Serializable]
    public struct AddressRange
    {
        [Hex(8)]
        public long startAddress;

        [Hex(8)]
        public long endAddress;

        public void RecordStartAddress(Stream stream)
        {
            startAddress = stream.Position;
        }

        public void RecordEndAddress(Stream stream)
        {
            endAddress = stream.Position;
        }

        public Pointer GetPointer()
        {
            return new Pointer()
            {
                address = (int)startAddress
            };
        }

        public int Size => (int)(endAddress - startAddress);

        public override string ToString()
        {
            return $"Start: {startAddress:x8}, End: {endAddress:x8}, Size: {Size} ({Size:x})";
        }
    }
}
