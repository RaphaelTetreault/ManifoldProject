namespace Manifold.IO
{
    /// <summary>
    /// Interface "tag" to indicate this IBinarySerializable referes to some
    /// other IBinarySerializable + IBinaryAddressable and thus needs to be
    /// re-serialized after the refered type to ensure it has a valid pointer.
    /// </summary>
    public interface ISerializedBinaryAddressableReferer :
        IBinarySerializable
    {
        void ValidateReferences();
    }
}
