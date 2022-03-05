using System.IO;

namespace Manifold.IO
{
    public static class IBinaryAddressableExtensions
    {

        public static void RecordStartAddress<TBinaryAddressable>(this TBinaryAddressable binaryAddressable, Stream stream)
            where TBinaryAddressable : IBinaryAddressable
        {
            var addressRange = binaryAddressable.AddressRange;
            addressRange.StartAddress = stream.Position;
            binaryAddressable.AddressRange = addressRange;
        }

        public static void RecordStartAddress<TBinaryAddressable>(this TBinaryAddressable binaryAddressable, BinaryReader reader)
            where TBinaryAddressable : IBinaryAddressable
            => RecordStartAddress(binaryAddressable, reader.BaseStream);

        public static void RecordStartAddress<TBinaryAddressable>(this TBinaryAddressable binaryAddressable, BinaryWriter writer)
            where TBinaryAddressable : IBinaryAddressable
            => RecordStartAddress(binaryAddressable, writer.BaseStream);


        public static void RecordEndAddress<TBinaryAddressable>(this TBinaryAddressable binaryAddressable, Stream stream)
            where TBinaryAddressable : IBinaryAddressable
        {
            var addressRange = binaryAddressable.AddressRange;
            addressRange.EndAddress = stream.Position;
            binaryAddressable.AddressRange = addressRange;
        }
        public static void RecordEndAddress<TBinaryAddressable>(this TBinaryAddressable binaryAddressable, BinaryReader reader)
            where TBinaryAddressable : IBinaryAddressable
            => RecordEndAddress(binaryAddressable, reader.BaseStream);

        public static void RecordEndAddress<TBinaryAddressable>(this TBinaryAddressable binaryAddressable, BinaryWriter writer)
            where TBinaryAddressable : IBinaryAddressable
            => RecordEndAddress(binaryAddressable, writer.BaseStream);


        public static void SetReaderToStartAddress<TBinaryAddressable>(this TBinaryAddressable binaryAddressable, BinaryReader reader)
            where TBinaryAddressable : IBinaryAddressable
        {
            var address = binaryAddressable.AddressRange.StartAddress;
            reader.BaseStream.Seek(address, SeekOrigin.Begin);
        }

        public static void SetWriterToStartAddress<TBinaryAddressable>(this TBinaryAddressable binaryAddressable, BinaryWriter writer)
            where TBinaryAddressable : IBinaryAddressable
        {
            var address = binaryAddressable.AddressRange.StartAddress;
            writer.BaseStream.Seek(address, SeekOrigin.Begin);
        }


        public static void SetReaderToEndAddress<TBinaryAddressable>(this TBinaryAddressable binaryAddressable, BinaryReader reader)
            where TBinaryAddressable : IBinaryAddressable
        {
            var address = binaryAddressable.AddressRange.EndAddress;
            reader.BaseStream.Seek(address, SeekOrigin.Begin);
        }

        public static void SetWriterToEndAddress<TBinaryAddressable>(this TBinaryAddressable binaryAddressable, BinaryWriter writer)
            where TBinaryAddressable : IBinaryAddressable
        {
            var address = binaryAddressable.AddressRange.EndAddress;
            writer.BaseStream.Seek(address, SeekOrigin.Begin);
        }


        public static string PrintStartAddress<TBinaryAddressable>(this TBinaryAddressable binaryAddressable, string prefix = "0x", string format = "X8")
            where TBinaryAddressable : IBinaryAddressable
        {
            var startAddress = binaryAddressable.AddressRange.StartAddress;
            return $"{prefix}{startAddress.ToString(format)}";
        }

        public static string PrintEndAddress<TBinaryAddressable>(this TBinaryAddressable binaryAddressable, string prefix = "0x", string format = "X8")
            where TBinaryAddressable : IBinaryAddressable
        {
            var endAddress = binaryAddressable.AddressRange.EndAddress;
            return $"{prefix}{endAddress.ToString(format)}";
        }

        public static string PrintAddressRange<TBinaryAddressable>(this TBinaryAddressable binaryAddressable, string format = "x8")
            where TBinaryAddressable : IBinaryAddressable
        {
            var addressRange = binaryAddressable.AddressRange;
            var size = addressRange.Size;

            return $"Address: 0x{addressRange.StartAddress.ToString(format)} to 0x{addressRange.EndAddress.ToString(format)} (hex:{size:x}, dec:{size})";
        }

        /// <summary>
        /// Get the address of the value. Address is relative to last (de)serialization stream.
        /// Will return null pointer if object is null.
        /// </summary>
        /// <param name="value">The value to get the pointer from.</param>
        /// <returns></returns>
        public static Pointer GetPointer<TBinaryAddressable>(this TBinaryAddressable value)
            where TBinaryAddressable : IBinaryAddressable
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
        public static Pointer GetBasePointer<TBinaryAddressable>(this TBinaryAddressable[] values)
            where TBinaryAddressable : IBinaryAddressable
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
        public static Pointer[] GetPointers<TBinaryAddressable>(this TBinaryAddressable[] values)
            where TBinaryAddressable : IBinaryAddressable
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
        public static ArrayPointer GetArrayPointer<TBinaryAddressable>(this TBinaryAddressable[] values)
            where TBinaryAddressable : IBinaryAddressable
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

    }
}
