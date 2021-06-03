using System.IO;

namespace Manifold.IO
{
    public static class IBinarySerializableReferenceExtensions
    {

        //public static AddressRange[] SerializeWithReferences(this IBinarySeralizableReference[] binarySerializableReferences, BinaryWriter writer)
        //{
        //    var count = binarySerializableReferences.Length;
        //    var addressRanges = new AddressRange[count];
        //    for (int i = 0; i < count; i++)
        //    {
        //        var binarySerializableReference = binarySerializableReferences[i];
        //        addressRanges[i] = binarySerializableReference.SerializeWithReference(writer);
        //    }
        //    return addressRanges;
        //}

        //// shouldn't be here, but built to work off of above
        //public static ArrayPointer GetArrayPointer(this AddressRange[] addressRanges)
        //{
        //    var arrayPointer = new ArrayPointer()
        //    {
        //        Length = addressRanges.Length,
        //        Address = addressRanges.Length == 0 ? 0 : addressRanges[0].GetPointer().address,
        //    };
        //    return arrayPointer;
        //}

        //public static Pointer[] GetPointers(this AddressRange[] addressRanges)
        //{
        //    var pointers = new Pointer[addressRanges.Length];
        //    for (int i = 0; i < pointers.Length; i++)
        //    {
        //        pointers[i] = addressRanges[i].GetPointer();
        //    }

        //    return pointers;
        //}


        public static Pointer GetPointer(this IBinarySeralizableReference value)
        {
            var pointer = value.AddressRange.GetPointer();
            return pointer;
        }

        public static Pointer[] GetPointers(this IBinarySeralizableReference[] values)
        {
            var pointers = new Pointer[values.Length];
            for (int i = 0; i < pointers.Length; i++)
            {
                pointers[i] = values[i].GetPointer();
            }

            return pointers;
        }

        public static ArrayPointer GetArrayPointer(this IBinarySeralizableReference[] values)
        {
            var arrayPointer = new ArrayPointer()
            {
                Length = values.Length,
                Address = values.Length == 0 ? 0 : values[0].AddressRange.GetPointer().address,
            };
            return arrayPointer;
        }


    }
}
