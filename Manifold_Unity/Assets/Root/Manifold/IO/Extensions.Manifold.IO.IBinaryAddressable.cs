using System.IO;

namespace Manifold.IO
{
    public static class IBinaryAddressableExtensions
    {
        public static void RecordStartAddress<T>(this T binaryAddressable, Stream stream)
            where T : IBinaryAddressable
        {
            var addressRange = binaryAddressable.AddressRange;
            addressRange.startAddress = stream.Position;
            binaryAddressable.AddressRange = addressRange;
        }

        public static void RecordStartAddress<T>(this T binaryAddressable, BinaryReader reader)
            where T : IBinaryAddressable
        {
            RecordStartAddress(binaryAddressable, reader.BaseStream);
        }

        public static void RecordStartAddress<T>(this T binaryAddressable, BinaryWriter writer)
            where T : IBinaryAddressable
        {
            RecordStartAddress(binaryAddressable, writer.BaseStream);
        }


        public static void RecordEndAddress<T>(this T binaryAddressable, Stream stream)
            where T : IBinaryAddressable
        {
            var addressRange = binaryAddressable.AddressRange;
            addressRange.endAddress = stream.Position;
            binaryAddressable.AddressRange = addressRange;
        }
        public static void RecordEndAddress<T>(this T binaryAddressable, BinaryReader reader)
            where T : IBinaryAddressable
        {
            RecordEndAddress(binaryAddressable, reader.BaseStream);
        }

        public static void RecordEndAddress<T>(this T binaryAddressable, BinaryWriter writer)
            where T : IBinaryAddressable
        {
            RecordEndAddress(binaryAddressable, writer.BaseStream);
        }

        public static void SetReaderToStartAddress<T>(this T binaryAddressable, BinaryReader reader)
            where T : IBinaryAddressable
        {
            var address = binaryAddressable.AddressRange.startAddress;
            reader.BaseStream.Seek(address, SeekOrigin.Begin);
        }

        public static void SetReaderToEndAddress<T>(this T binaryAddressable, BinaryReader reader)
            where T : IBinaryAddressable
        {
            var address = binaryAddressable.AddressRange.endAddress;
            reader.BaseStream.Seek(address, SeekOrigin.Begin);
        }

        public static long GetBinarySize<T>(this T binaryAddressable)
            where T : IBinaryAddressable
        {
            var startAddress = binaryAddressable.AddressRange.startAddress;
            var endAddress = binaryAddressable.AddressRange.endAddress;
            var size = endAddress - startAddress;
            return size;
        }


        public static string StartAddressHex<T>(this T binaryAddressable, string prefix = "0x", string format = "X8")
            where T : IBinaryAddressable
        {
            var startAddress = binaryAddressable.AddressRange.startAddress;
            return $"{prefix}{startAddress.ToString(format)}";
        }

        public static string EndAddressHex<T>(this T binaryAddressable, string prefix = "0x", string format = "X8")
            where T : IBinaryAddressable
        {
            var endAddress = binaryAddressable.AddressRange.endAddress;
            return $"{prefix}{endAddress.ToString(format)}";
        }

        public static string PrintAddressRange<T>(this T binaryAddressable, string format = "x8")
            where T : IBinaryAddressable
        {
            var addressRange = binaryAddressable.AddressRange;
            var size = addressRange.Size;

            return $"Address: 0x{addressRange.startAddress:x8} to 0x{addressRange.endAddress:x8} (hex:{size:x}, dec:{size})";
        }

        /// <summary>
        /// Get the address of the value. Address is relative to last (de)serialization stream.
        /// Will return null pointer if object is null.
        /// </summary>
        /// <param name="value">The value to get the pointer from.</param>
        /// <returns></returns>
        public static Pointer GetPointer<T>(this T value)
            where T : IBinaryAddressable
        {
            if (value is null)
                return 0;

            var pointer = value.AddressRange.GetPointer();
            return pointer;
        }

        /// <summary>
        /// Get the address of the first element in the array. Address is relative to last (de)serialization stream.
        /// </summary>
        /// <param name="values">The values to get the pointer base from.</param>
        /// <returns></returns>
        public static Pointer GetBasePointer<T>(this T[] values)
            where T : IBinaryAddressable
        {
            // Return null pointer if null OR if array empty. Perfect!
            if (values == null || values.Length == 0)
                return new Pointer();

            // Get pointer of first item in array
            // Return null pointer if null.
            var firstValue = values[0];
            if (firstValue == null)
                return new Pointer();

            // Get pointer of first item in array which exists and is not null.
            Pointer pointer = values[0].GetPointer();
            return pointer;
        }

        /// <summary>
        /// Get the address of all values. Address is relative to last (de)serialization stream.
        /// </summary>
        /// <param name="values">The values to get pointers from.</param>
        /// <returns></returns>
        public static Pointer[] GetPointers<T>(this T[] values)
            where T : IBinaryAddressable
        {
            // Since we want a pointer for each thing, throw an error since
            // we would be returning 0 pointers. AFAIK this is not wanted behaviour.
            if (values == null || values.Length == 0)
                throw new System.ArgumentException("Array cannot be null!");

            // Get a pointer to each item in array
            var pointers = new Pointer[values.Length];
            for (int i = 0; i < pointers.Length; i++)
            {
                var value = values[i];
                var isNull = value == null;

                // Assign pointer based on value
                pointers[i] = isNull
                    ? new Pointer()
                    : values[i].GetPointer();
            }

            return pointers;
        }

        /// <summary>
        /// Get the length address of the value. Address is relative to last (de)serialization stream.
        /// </summary>
        /// <param name="values">The array to get the array pointer from.</param>
        /// <returns></returns>
        public static ArrayPointer GetArrayPointer<T>(this T[] values)
            where T : IBinaryAddressable
        {
            if (values == null || values.Length == 0)
                return new ArrayPointer();

            int length = values.Length;
            var arrayPointer = new ArrayPointer()
            {
                Length = length,
                Address = values[0].GetPointer(),
            };

            return arrayPointer;
        }

        ///// <summary>
        ///// Get the length address of the value. Address is relative to last (de)serialization stream.
        ///// </summary>
        ///// <param name="values">The array to get the array pointer from.</param>
        ///// <returns></returns>
        //public static ArrayPointer2D GetArrayPointer2D(this IBinaryAddressable[] values)
        //{
        //    // TODO: consider null checks.

        //    var arrayPointer = new ArrayPointer()
        //    {
        //        Length = values.Length,
        //        // Address is 0 if no length, is address of first item otherwise
        //        Address = values.Length == 0 ? 0 : values[0].AddressRange.GetPointer().address,
        //    };
        //    return arrayPointer;
        //}

    }
}
