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
            {
                UnityEngine.Debug.Log($"{nameof(unk0x34)}: {unk0x34 }");
                Assert.IsTrue(unk0x38 == 0);
                Assert.IsTrue(unk0x3C == 0);
            }
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