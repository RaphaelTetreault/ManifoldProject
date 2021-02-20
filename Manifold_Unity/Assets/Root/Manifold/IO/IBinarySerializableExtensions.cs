using System.IO;

namespace Manifold.IO
{
    public static class IBinarySerializableExtensions
    {
        /// <summary>
        /// Returns size of this IBinarySerializable serialized.
        /// *Note: Opens new buffer to guarantee size. Value should be stored after use.
        /// </summary>
        public static int Size<T>(this T value)
            where T : IBinarySerializable, new()
        {
            using (var buffer = new BinaryWriter(new MemoryStream()))
            {
                buffer.WriteX(value);
                return (int)buffer.BaseStream.Length;
            }
        }
    }
}