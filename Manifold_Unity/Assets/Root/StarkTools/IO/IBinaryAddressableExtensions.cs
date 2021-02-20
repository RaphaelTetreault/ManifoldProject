using System.IO;

namespace StarkTools.IO
{
    public static class IBinaryAddressableExtensions
    {
        public static void RecordStartAddress(this IBinaryAddressable binaryAddressable, Stream stream)
        {
            binaryAddressable.StartAddress = stream.Position;
        }

        public static void RecordStartAddress(this IBinaryAddressable binaryAddressable, BinaryReader reader)
            => RecordStartAddress(binaryAddressable, reader.BaseStream);

        public static void RecordStartAddress(this IBinaryAddressable binaryAddressable, BinaryWriter writer)
            => RecordStartAddress(binaryAddressable, writer.BaseStream);

        public static void RecordEndAddress(this IBinaryAddressable binaryAddressable, Stream stream)
        {
            binaryAddressable.EndAddress = stream.Position;
        }

        public static void RecordEndAddress(this IBinaryAddressable binaryAddressable, BinaryReader reader)
            => RecordEndAddress(binaryAddressable, reader.BaseStream);

        public static void RecordEndAddress(this IBinaryAddressable binaryAddressable, BinaryWriter writer)
            => RecordEndAddress(binaryAddressable, writer.BaseStream);
    }
}
