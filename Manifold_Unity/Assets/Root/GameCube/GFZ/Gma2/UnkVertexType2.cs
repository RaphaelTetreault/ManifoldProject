using Manifold;
using Manifold.IO;
using System.IO;
using Unity.Mathematics;
using LibGxFormat;

namespace GameCube.GFZ.Gma2
{
    public class UnkVertexType2 :
        IBinaryAddressable,
        IBinarySerializable
    {
        // FIELDS
        private float3 position;
        private float3 normal;
        private float2 textureUV0;
        private float2 textureUV1;
        private float2 textureUV2;
        private uint color;
        private uint unk0x34; // TODO: confirm this and next 2 values are not zero / FIFO padding.
        private uint unk0x38;
        private uint unk0x3C;

        // PROPERTIES
        public AddressRange AddressRange { get; set; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                //reader.ReadX(ref );
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                //writer.WriteX();
            }
            this.RecordEndAddress(writer);
        }

    }

}