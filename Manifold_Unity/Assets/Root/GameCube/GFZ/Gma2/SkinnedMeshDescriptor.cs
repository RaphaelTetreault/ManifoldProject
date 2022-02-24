using Manifold.IO;
using System.IO;

namespace GameCube.GFZ.Gma2
{
    internal class SkinnedMeshDescriptor :
        IBinaryAddressable,
        IBinarySerializable
    {
        // FIELDS
        private uint unk0x00;
        private uint unk0x04;
        private uint unk0x08; // skinned display list 0 CW count
        private uint unk0x0C; // skinned display list 1 CCW count

        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public uint Unk0x00 { get => unk0x00; set => unk0x00 = value; }
        public uint Unk0x04 { get => unk0x04; set => unk0x04 = value; }
        public uint Unk0x08 { get => unk0x08; set => unk0x08 = value; }
        public uint Unk0x0C { get => unk0x0C; set => unk0x0C = value; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref unk0x00);
                reader.ReadX(ref unk0x04);
                reader.ReadX(ref unk0x08);
                reader.ReadX(ref unk0x0C);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                writer.WriteX(unk0x00);
                writer.WriteX(unk0x04);
                writer.WriteX(unk0x08);
                writer.WriteX(unk0x0C);
            }
            this.RecordEndAddress(writer);
        }

    }

}