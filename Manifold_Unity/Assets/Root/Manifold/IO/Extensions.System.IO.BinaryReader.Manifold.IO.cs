using System;
using System.IO;

namespace Manifold.IO
{
    public static partial class BinaryReaderExtensions
    {
        /// <summary>
        /// Moves the <paramref name="reader"/>'s stream position to the <paramref name="pointer"/>'s address.
        /// </summary>
        /// <param name="reader">The reader to jump in.</param>
        /// <param name="pointer">The pointer to jump to.</param>
        public static void JumpToAddress(this BinaryReader reader, Pointer pointer)
        {
            reader.BaseStream.Seek(pointer.address, SeekOrigin.Begin);
        }

        /// <summary>
        /// Moves the <paramref name="reader"/>'s stream position to the <paramref name="arrayPointer"/>'s address.
        /// </summary>
        /// <param name="reader">The reader to jump in.</param>
        /// <param name="arrayPointer">The pointer to jump to.</param>
        public static void JumpToAddress(this BinaryReader reader, ArrayPointer arrayPointer)
        {
            reader.BaseStream.Seek(arrayPointer.address, SeekOrigin.Begin);
        }

        /// <summary>
        /// Returns the <paramref name="reader"/>'s position as a Pointer.
        /// </summary>
        /// <param name="reader">The reader to convert position to pointer from.</param>
        /// <returns>
        /// A pointer pointing to the address of the <paramref name="reader"/>'s stream position.
        /// </returns>
        public static Pointer GetPositionAsPointer(this BinaryReader reader)
        {
            return new Pointer(reader.BaseStream.Position);
        }
    }
}
