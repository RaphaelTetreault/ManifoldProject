using System;
using System.IO;

namespace Manifold.IO
{
    public static partial class BinaryWriterExtensions
    {
        public static void WriteX(this BinaryWriter writer, DateTime value)
        {
            long dateTimeBinary = value.ToBinary();
            writer.WriteX(dateTimeBinary);
        }
    }
}
