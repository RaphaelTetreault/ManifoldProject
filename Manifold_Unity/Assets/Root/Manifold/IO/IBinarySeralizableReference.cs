using System.IO;

namespace Manifold.IO
{
    public interface IBinarySeralizableReference :
        IBinaryAddressableRange,
        IBinarySerializable
    {
        /// <summary>
        /// Serializes this value to underlying stream.
        /// Returns address range of serialized value.
        /// </summary>
        /// <param name="writer">The writer stream to serialize to.</param>
        /// <returns>Returns address range of serialized value.</returns>
        AddressRange SerializeWithReference(BinaryWriter writer);

        // TODO when Unity C# runtime support default interface implementation,
        //      use this instead.

        //AddressRange SerializeReference(BinaryWriter writer)
        //{
        //    var addressRange = new AddressRange();
        //    addressRange.RecordStartAddress(writer.BaseStream);
        //    {
        //        Serialize(writer);
        //    }
        //    addressRange.RecordEndAddress(writer.BaseStream);
        //    return addressRange;
        //}

    }
}
