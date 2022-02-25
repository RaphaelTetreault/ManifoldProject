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
        private uint vertexCount0; // skinned display list 0 CW count
        private uint vertexCount1; // skinned display list 1 CCW count

        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public uint Unk0x00 { get => unk0x00; set => unk0x00 = value; }
        public uint Unk0x04 { get => unk0x04; set => unk0x04 = value; }
        public uint VertexCount0 { get => vertexCount0; set => vertexCount0 = value; }
        public uint VertexCount1 { get => vertexCount1; set => vertexCount1 = value; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref unk0x00);
                reader.ReadX(ref unk0x04);
                reader.ReadX(ref vertexCount0);
                reader.ReadX(ref vertexCount1);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                writer.WriteX(unk0x00);
                writer.WriteX(unk0x04);
                writer.WriteX(vertexCount0);
                writer.WriteX(vertexCount1);
            }
            this.RecordEndAddress(writer);
        }

    }

}