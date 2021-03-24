using System;
using System.IO;

namespace Manifold.IO
{
    public static partial class BinaryReaderExtensions
    {
        public static void ReadX(this BinaryReader reader, ref ArrayPointer2D arrayPointer2D)
        {
            arrayPointer2D = BinaryIoUtility.ReadIBinarySerializable(reader, arrayPointer2D);
        }

        public static void ReadX(this BinaryReader reader, ref ArrayPointer arrayPointer)
        {
            arrayPointer = BinaryIoUtility.ReadIBinarySerializable(reader, arrayPointer);
        }

        public static void ReadX(this BinaryReader reader, ref Pointer pointer)
        {
            pointer = BinaryIoUtility.ReadIBinarySerializable(reader, pointer);
        }

        public static void ReadX(this BinaryReader reader, ref Offset offset)
        {
            offset = BinaryIoUtility.ReadIBinarySerializable(reader, offset);
        }
    }
}
