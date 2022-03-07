namespace Manifold.IO
{
    public interface IBinaryAddressable
    {
        /// <summary>
        /// The binary value's address within a stream.
        /// </summary>
        AddressRange AddressRange { get; set; }
    }
}