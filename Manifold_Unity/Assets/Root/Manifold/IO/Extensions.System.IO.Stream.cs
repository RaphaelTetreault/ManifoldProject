using System.IO;

namespace Manifold.IO
{
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Returns the length required to align <paramref name="stream"/> to <paramref name="alignment"/>.
        /// </summary>
        /// <param name="stream">The stream to base alignment length from.</param>
        /// <param name="alignment">The alignment stride.</param>
        /// <returns></returns>
        public static long GetLengthOfAlignment(this Stream stream, long alignment)
        {
            // Get number of bytes needed to aligment
            var lengthOfAlignment = alignment - (stream.Position % alignment);
            // If stream.Position is cleanly divisible by aligment, then the result is
            // 'alignment - 0 == alignment', whereas what we want is an alignment of 0.
            lengthOfAlignment = (lengthOfAlignment == alignment) ? 0 : lengthOfAlignment;

            return lengthOfAlignment;
        }

        /// <summary>
        /// Check to see if <paramref name="stream"/>'s position is it's length;
        /// </summary>
        /// <param name="stream">The stream to query.</param>
        /// <returns>
        /// Returns true if Stream.Position == Stream.Length, false otherwise.
        /// </returns>
        public static bool IsAtEndOfStream(this Stream stream)
        {
            return !(stream.Position < stream.Length);
        }
    }
}