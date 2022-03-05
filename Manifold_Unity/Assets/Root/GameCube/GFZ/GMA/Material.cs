using GameCube.GX;
using Manifold;
using Manifold.IO;
using System.IO;

namespace GameCube.GFZ.GMA
{
    /// <summary>
    /// The material properties applied to a GCMF's display lists.
    /// </summary>
    public class Material :
        IBinaryAddressable,
        IBinarySerializable
    {
        // FIELDS
        private ushort zero0x00; //2
        private MatFlags0x02 unk0x02; // <- COLOR blending stuff?
        private MatFlags0x03 unk0x03; // <- cont
        private GXColor color0 = new GXColor(ComponentType.GX_RGBA8);
        private GXColor color1 = new GXColor(ComponentType.GX_RGBA8);
        private GXColor color2 = new GXColor(ComponentType.GX_RGBA8);
        private MatFlags0x10 unk0x10;
        private MatFlags0x11 unk0x11;
        private byte textureCount;
        private DisplayListRenderFlags displayListRenderFlags;
        private byte unk0x14;
        private MatFlags0x15 unk0x15;
        private short textureIndex0 = -1; // 0xFFFF
        private short textureIndex1 = -1; // 0xFFFF
        private short textureIndex2 = -1; // 0xFFFF
        private GXAttributes vertexAttributes;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public MatFlags0x02 Unk0x02 { get => unk0x02; set => unk0x02 = value; }
        public MatFlags0x03 Unk0x03 { get => unk0x03; set => unk0x03 = value; }
        public GXColor Color0 { get => color0; set => color0 = value; }
        public GXColor Color1 { get => color1; set => color1 = value; }
        public GXColor Color2 { get => color2; set => color2 = value; }
        public MatFlags0x10 Unk0x10 { get => unk0x10; set => unk0x10 = value; }
        public MatFlags0x11 Unk0x11 { get => unk0x11; set => unk0x11 = value; }
        public byte TextureCount { get => textureCount; set => textureCount = value; }
        public DisplayListRenderFlags DisplayListRenderFlags { get => displayListRenderFlags; set => displayListRenderFlags = value; }
        public byte Unk0x14 { get => unk0x14; set => unk0x14 = value; }
        public MatFlags0x15 Unk0x15 { get => unk0x15; set => unk0x15 = value; }
        public short TextureIndex0 { get => textureIndex0; set => textureIndex0 = value; }
        public short TextureIndex1 { get => textureIndex1; set => textureIndex1 = value; }
        public short TextureIndex2 { get => textureIndex2; set => textureIndex2 = value; }
        public GXAttributes VertexAttributes { get => vertexAttributes; set => vertexAttributes = value; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref zero0x00);
                reader.ReadX(ref unk0x02);
                reader.ReadX(ref unk0x03);
                color0.Deserialize(reader);
                color1.Deserialize(reader);
                color2.Deserialize(reader);
                reader.ReadX(ref unk0x10);
                reader.ReadX(ref unk0x11);
                reader.ReadX(ref textureCount);
                reader.ReadX(ref displayListRenderFlags);
                reader.ReadX(ref unk0x14);
                reader.ReadX(ref unk0x15);
                reader.ReadX(ref textureIndex0);
                reader.ReadX(ref textureIndex1);
                reader.ReadX(ref textureIndex2);
                reader.ReadX(ref vertexAttributes);
            }
            this.RecordEndAddress(reader);
            {
                Assert.IsTrue(zero0x00 == 0);
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            {
                Assert.IsTrue(zero0x00 == 0);
                Assert.IsTrue(color0.ComponentType == ComponentType.GX_RGBA8);
                Assert.IsTrue(color1.ComponentType == ComponentType.GX_RGBA8);
                Assert.IsTrue(color2.ComponentType == ComponentType.GX_RGBA8);
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(zero0x00);
                writer.WriteX(unk0x02);
                writer.WriteX(unk0x03);
                writer.WriteX(color0);
                writer.WriteX(color1);
                writer.WriteX(color2);
                writer.WriteX(unk0x10);
                writer.WriteX(unk0x11);
                writer.WriteX(textureCount);
                writer.WriteX(displayListRenderFlags);
                writer.WriteX(unk0x14);
                writer.WriteX(unk0x15);
                writer.WriteX(textureIndex0);
                writer.WriteX(textureIndex1);
                writer.WriteX(textureIndex2);
                writer.WriteX(vertexAttributes);
            }
            this.RecordEndAddress(writer);
        }

    }
}