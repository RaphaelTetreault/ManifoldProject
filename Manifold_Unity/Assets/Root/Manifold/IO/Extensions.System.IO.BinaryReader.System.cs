using System;
using System.IO;

namespace Manifold.IO
{
    public static partial class BinaryReaderExtensions
    {
        public static void ReadX(this BinaryReader reader, ref DateTime value)
        {
            long dateTimeBinary = 0;
            reader.ReadX(ref dateTimeBinary);
            value = DateTime.FromBinary(dateTimeBinary);
        }
    }
}
