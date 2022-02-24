using Manifold;
using Manifold.IO;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ.Gma2
{
    public class UnkVertexType1 :
        IBinaryAddressable,
        IBinarySerializable
    {
        // FIELDS
        private float3 position;
        private float3 normal;
        private uint unk0x18;
        private float unk0x1C;

        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public float3 Position { get => position; set => position = value; }
        public float3 Normal { get => normal; set => normal = value; }
        public uint Unk0x18 { get => unk0x18; set => unk0x18 = value; }
        public float Unk0x1C { get => unk0x1C; set => unk0x1C = value; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref position);
                reader.ReadX(ref normal);
                reader.ReadX(ref unk0x18);
                reader.ReadX(ref unk0x1C);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                writer.WriteX(position);
                writer.WriteX(normal);
                writer.WriteX(unk0x18);
                writer.WriteX(unk0x1C);
            }
            this.RecordEndAddress(writer);
        }

    }

}