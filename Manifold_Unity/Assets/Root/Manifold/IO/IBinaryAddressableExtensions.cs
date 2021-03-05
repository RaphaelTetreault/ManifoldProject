using System.IO;

namespace Manifold.IO
{
    public static class IBinaryAddressableExtensions
    {
        public static void RecordStartAddress(this IBinaryAddressableRange binaryAddressable, Stream stream)
        {
            var addressRange = binaryAddressable.AddressRange;
            addressRange.startAddress = stream.Position;
            binaryAddressable.AddressRange = addressRange;
        }

        public static void RecordStartAddress(this IBinaryAddressableRange binaryAddressable, BinaryReader reader)
            => RecordStartAddress(binaryAddressable, reader.BaseStream);

        public static void RecordStartAddress(this IBinaryAddressableRange binaryAddressable, BinaryWriter writer)
            => RecordStartAddress(binaryAddressable, writer.BaseStream);

        public static void RecordEndAddress(this IBinaryAddressableRange binaryAddressable, Stream stream)
        {
            var addressRange = binaryAddressable.AddressRange;
            addressRange.endAddress = stream.Position;
            binaryAddressable.AddressRange = addressRange;
        }

        public static void RecordEndAddress(this IBinaryAddressableRange binaryAddressable, BinaryReader reader)
            => RecordEndAddress(binaryAddressable, reader.BaseStream);

        public static void RecordEndAddress(this IBinaryAddressableRange binaryAddressable, BinaryWriter writer)
            => RecordEndAddress(binaryAddressable, writer.BaseStream);



        public static void SetReaderToEndAddress(this IBinaryAddressableRange binaryAddressable, BinaryReader reader)
        {
            var address = binaryAddressable.AddressRange.endAddress;
            reader.BaseStream.Seek(address, SeekOrigin.Begin);
        }

        public static long GetBinarySize(this IBinaryAddressableRange binaryAddressable)
        {
            var startAddress = binaryAddressable.AddressRange.startAddress;
            var endAddress = binaryAddressable.AddressRange.endAddress;
            var size = endAddress - startAddress;
            return size;
        }



        public static string StartAddressHex(this IBinaryAddressableRange binaryAddressable, string prefix = "0x", string format = "X8")
        {
            var startAddress = binaryAddressable.AddressRange.startAddress;
            return $"{prefix}{startAddress.ToString(format)}";
        }

        public static string EndAddressHex(this IBinaryAddressableRange binaryAddressable, string prefix = "0x", string format = "X8")
        {
            var endAddress = binaryAddressable.AddressRange.endAddress;
            return $"{prefix}{endAddress.ToString(format)}";
        }

    }
}
