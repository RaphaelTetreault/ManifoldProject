using System.IO;

namespace Manifold.IO
{
    /// <summary>
    /// Interface which enables types to define how it is serialized and 
    /// deserialized from a binary stream.
    /// </summary>
    public interface IBinarySerializable
    {
        /// <summary>
        /// Deserializes this type from a binary stream.
        /// </summary>
        /// <param name="reader">The binary reader to read from</param>
        void Deserialize(BinaryReader reader);

        /// <summary>
        /// Serializes this type to a binary stream.
        /// </summary>
        /// <param name="writer">The binary writer to write to</param>
        void Serialize(BinaryWriter writer);
    }
}