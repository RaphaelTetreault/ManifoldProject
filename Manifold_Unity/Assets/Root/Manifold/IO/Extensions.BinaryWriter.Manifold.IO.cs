using System;
using System.IO;

namespace Manifold.IO
{
    public static partial class BinaryWriterExtensions
    {
        public static void JumpToAddress(this BinaryReader reader, Pointer pointer, SeekOrigin seekOrigin = SeekOrigin.Begin)
        {
            reader.BaseStream.Seek(pointer.address, seekOrigin);
        }

        public static void JumpToAddress(this BinaryReader reader, ArrayPointer arrayPointer, SeekOrigin seekOrigin = SeekOrigin.Begin)
        {
            reader.BaseStream.Seek(arrayPointer.Address, seekOrigin);
        }
    }
}
