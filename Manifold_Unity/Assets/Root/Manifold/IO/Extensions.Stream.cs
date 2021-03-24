using System.IO;

namespace Manifold.IO
{
    public static partial class StreamExtensions
    {
        public static long GetLengthOffAlignment(this Stream stream, long alignment)
        {
            // Get number of bytes needed to aligment
            var lengthOfAlignment = alignment - (stream.Position % alignment);
            // If stream.Position is cleanly divisible by aligment, then the result is
            // 'alignment - 0 == alignment', whereas what we want is an alignment of 0.
            lengthOfAlignment = (lengthOfAlignment == alignment) ? 0 : lengthOfAlignment;

            return lengthOfAlignment;
        }

        public static bool IsAtEndOfStream(this Stream stream)
        {
            return !(stream.Position < stream.Length);
        }
    }
}