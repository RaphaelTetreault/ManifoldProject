using GameCube.GX;
using Manifold;
using Manifold.IO;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ.GMA
{
    /// <summary>
    /// Appears to be a GX vertex meant for skinning. It is not part of a formal
    /// display list. The GXAttributes properly describe the intial position,
    /// normal, textureUV0, and textureUV1. The data afterwards is somewhat strange.
    /// The color values may not be true colors, but some kind of weight painting.
    /// </summary>
    public class SkinnedVertexB :
        IBinaryAddressable,
        IBinarySerializable
    {
        // FIELDS
        private float3 position;
        private float3 normal;
        private float2 textureUV0;
        private float2 textureUV1;
        private uint zero0x28;
        private uint zero0x2C;
        private GXColor color0 = new GXColor(ComponentType.GX_RGBA8); // RGBA. Appears to truly be a color.
        private GXColor color1 = new GXColor(ComponentType.GX_RGBA8); // Magic bits. Variations: 00000000, 00010100, 02000002, 01000001, 03000003
        private GXColor color2 = new GXColor(ComponentType.GX_RGBA8); // RGBA. Color-looking, but does use alpha channel.
        private GXColor color3 = new GXColor(ComponentType.GX_RGBA8); // RGBA. Color-looking, but does use alpha channel. 00000004 is the only magic-bit looking value.

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
                reader.ReadX(ref zero0x28);
                reader.ReadX(ref zero0x2C);
                color0.Deserialize(reader);
                color1.Deserialize(reader);
                color2.Deserialize(reader);
                color3.Deserialize(reader);
            }
            this.RecordEndAddress(reader);
            {
                Assert.IsTrue(zero0x28 == 0);
                Assert.IsTrue(zero0x2C == 0);
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            {
                Assert.IsTrue(zero0x28 == 0);
                Assert.IsTrue(zero0x2C == 0);
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(position);
                writer.WriteX(normal);
                writer.WriteX(textureUV0);
                writer.WriteX(textureUV1);
                writer.WriteX(zero0x28);
                writer.WriteX(zero0x2C);
                writer.WriteX(color0);
                writer.WriteX(color1);
                writer.WriteX(color2);
                writer.WriteX(color3);
            }
            this.RecordEndAddress(writer);
        }

    }
}