using System.IO;

namespace Manifold.IO
{
    public static partial class BinaryWriterExtensions
    {
        /// <summary>
        /// Positions underlying stream at end of stream.
        /// </summary>
        /// <param name="writer"></param>
        public static void SeekEnd(this BinaryWriter writer)
        {
            long endOfStream = writer.BaseStream.Length;
            writer.BaseStream.Seek(endOfStream, SeekOrigin.Begin);
        }

        /// <summary>
        /// Positions underlying stream at beginning of stream.
        /// </summary>
        /// <param name="writer"></param>
        public static void SeekStart(this BinaryWriter writer)
        {
            writer.BaseStream.Seek(0, SeekOrigin.Begin);
        }
    }
}
