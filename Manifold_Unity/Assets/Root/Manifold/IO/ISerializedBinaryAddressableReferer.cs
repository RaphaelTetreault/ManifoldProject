namespace Manifold.IO
{
    /// <summary>
    /// Interface "tag" to indicate this IBinarySerializable referes to some
    /// other IBinarySerializable + IBinaryAddressable and thus needs to be
    /// re-serialized after the refered type to ensure it has a valid pointer.
    /// </summary>
    public interface ISerializedBinaryAddressableReferer :
        IBinaryAddressable,
        IBinarySerializable
    {
        /// <summary>
        /// Runs a sanity check on this type as it relates to object references.
        /// </summary>
        void ValidateReferences();
    }
}
