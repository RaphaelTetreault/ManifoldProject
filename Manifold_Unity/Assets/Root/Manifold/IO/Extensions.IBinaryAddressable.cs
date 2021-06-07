using System.IO;

namespace Manifold.IO
{
    public static class IBinaryAddressableExtensions
    {
        public static void RecordStartAddress(this IBinaryAddressable binaryAddressable, Stream stream)
        {
            var addressRange = binaryAddressable.AddressRange;
            addressRange.startAddress = stream.Position;
            binaryAddressable.AddressRange = addressRange;
        }
        public static void RecordStartAddress(this IBinaryAddressable binaryAddressable, BinaryReader reader)
            => RecordStartAddress(binaryAddressable, reader.BaseStream);
        public static void RecordStartAddress(this IBinaryAddressable binaryAddressable, BinaryWriter writer)
            => RecordStartAddress(binaryAddressable, writer.BaseStream);


        public static void RecordEndAddress(this IBinaryAddressable binaryAddressable, Stream stream)
        {
            var addressRange = binaryAddressable.AddressRange;
            addressRange.endAddress = stream.Position;
            binaryAddressable.AddressRange = addressRange;
        }
        public static void RecordEndAddress(this IBinaryAddressable binaryAddressable, BinaryReader reader)
            => RecordEndAddress(binaryAddressable, reader.BaseStream);
        public static void RecordEndAddress(this IBinaryAddressable binaryAddressable, BinaryWriter writer)
            => RecordEndAddress(binaryAddressable, writer.BaseStream);


        public static void SetReaderToStartAddress(this IBinaryAddressable binaryAddressable, BinaryReader reader)
        {
            var address = binaryAddressable.AddressRange.startAddress;
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
        }
        public static void SetReaderToEndAddress(this IBinaryAddressable binaryAddressable, BinaryReader reader)
        {
            var address = binaryAddressable.AddressRange.endAddress;
            reader.BaseStream.Seek(address, SeekOrigin.Begin);
        }

        public static long GetBinarySize(this IBinaryAddressable binaryAddressable)
        {
            var startAddress = binaryAddressable.AddressRange.startAddress;
            var endAddress = binaryAddressable.AddressRange.endAddress;
            var size = endAddress - startAddress;
            return size;
        }


        public static string StartAddressHex(this IBinaryAddressable binaryAddressable, string prefix = "0x", string format = "X8")
        {
            var startAddress = binaryAddressable.AddressRange.startAddress;
            return $"{prefix}{startAddress.ToString(format)}";
        }
        public static string EndAddressHex(this IBinaryAddressable binaryAddressable, string prefix = "0x", string format = "X8")
        {
            var endAddress = binaryAddressable.AddressRange.endAddress;
            return $"{prefix}{endAddress.ToString(format)}";
        }


        /// <summary>
        /// Get the address of the value. Address is relative to last (de)serialization stream.
        /// Will return null pointer if object is null.
        /// </summary>
        /// <param name="value">The value to get the pointer from.</param>
        /// <returns></returns>
        public static Pointer GetPointer(this IBinaryAddressable value)
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
        public static Pointer GetBasePointer(this IBinaryAddressable[] values)
        {
            // TODO: consider null checks.

            // Get pointer of first item in array
            // If array is empty, return null pointer
            Pointer pointer = values.Length == 0 ? new Pointer() : values[0].GetPointer();
            return pointer;
        }

        /// <summary>
        /// Get the address of all values. Address is relative to last (de)serialization stream.
        /// </summary>
        /// <param name="values">The values to get pointers from.</param>
        /// <returns></returns>
        public static Pointer[] GetPointers(this IBinaryAddressable[] values)
        {
            // TODO: consider null checks.

            var pointers = new Pointer[values.Length];
            for (int i = 0; i < pointers.Length; i++)
            {
                pointers[i] = values[i].GetPointer();
            }

            return pointers;
        }

        /// <summary>
        /// Get the length address of the value. Address is relative to last (de)serialization stream.
        /// </summary>
        /// <param name="values">The array to get the array pointer from.</param>
        /// <returns></returns>
        public static ArrayPointer GetArrayPointer(this IBinaryAddressable[] values)
        {
            // TODO: consider null checks.

            int length = values.Length;
            var arrayPointer = new ArrayPointer();

            // We only need to assign values if length > 0
            if (length > 0)
            {
                arrayPointer.Length = length;
                arrayPointer.Address = values[0].GetPointer();
            }

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
