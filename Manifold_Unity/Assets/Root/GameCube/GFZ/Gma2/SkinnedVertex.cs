using Manifold;
using Manifold.IO;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ.Gma2
{
    public class SkinnedVertex :
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
        private uint unk0x34; // Is some kind of flag
        private uint unk0x38; 
        private uint unk0x3C;

        // PROPERTIES
        public AddressRange AddressRange { get; set; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref position);
                reader.ReadX(ref normal);
                reader.ReadX(ref textureUV0);
                reader.ReadX(ref textureUV1);
                reader.ReadX(ref textureUV2);
                reader.ReadX(ref color);
                reader.ReadX(ref unk0x34);
                reader.ReadX(ref unk0x38);
                reader.ReadX(ref unk0x3C);
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

            throw new System.NotImplementedException();
        }

    }

}