using System.IO;

namespace Manifold.IO
{
    public static class IBinarySerializableReferenceExtensions
    {
        public static AddressRange[] SerializeReferences(this IBinarySeralizableReference[] binarySerializableReferences, BinaryWriter writer)
        {
            var count = binarySerializableReferences.Length;
            var addressRanges = new AddressRange[count];
            for (int i = 0; i < count; i++)
            {
                var binarySerializableReference = binarySerializableReferences[i];
                addressRanges[i] = binarySerializableReference.SerializeReference(writer);
            }
            return addressRanges;
        }

        // shouldn't be here, but built to work off of above
        public static ArrayPointer GetArrayPointer(this AddressRange[] addressRanges)
        {
            var arrayPointer = new ArrayPointer()
            {
                Length = addressRanges.Length,
                Address = addressRanges.Length == 0 ? 0 : addressRanges[0].GetPointer().address,
            };
            return arrayPointer;
        }

    }
}
