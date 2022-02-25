using Manifold;
using Manifold.IO;
using System.IO;

namespace GameCube.GFZ.Gma2
{
    internal class TextureConfig :
        IBinaryAddressable,
        IBinarySerializable
    {
        // FIELDS
        private TexFlags0x00 unk0x00;
        private MipmapSetting mipmapSetting;
        private TextureWrapMode wrapMode;
        private ushort tplTextureIndex;
        private TexFlags0x06 unk0x06;
        private GXAnisotropy anisotropicFilter;
        private uint zero0x08;
        private TexFlags0x0C unk0x0C;
        private bool isSwappableTexture; // perhaps a "cache texture" flag
        private ushort configIndex;
        private TexFlags0x10 unk0x10;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref unk0x00);
                reader.ReadX(ref mipmapSetting);
                reader.ReadX(ref wrapMode);
                reader.ReadX(ref tplTextureIndex);
                reader.ReadX(ref unk0x06);
                reader.ReadX(ref anisotropicFilter);
                reader.ReadX(ref zero0x08);
                reader.ReadX(ref unk0x0C);
                reader.ReadX(ref isSwappableTexture);
                reader.ReadX(ref configIndex);
                reader.ReadX(ref unk0x10);
            }
            this.RecordEndAddress(reader);
            {
                Assert.IsTrue(zero0x08 == 0);
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            {
                Assert.IsTrue(zero0x08 == 0);
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(unk0x00);
                writer.WriteX(mipmapSetting);
                writer.WriteX(wrapMode);
                writer.WriteX(tplTextureIndex);
                writer.WriteX(unk0x06);
                writer.WriteX(anisotropicFilter);
                writer.WriteX(zero0x08);
                writer.WriteX(unk0x0C);
                writer.WriteX(isSwappableTexture);
                writer.WriteX(configIndex);
                writer.WriteX(unk0x10);
            }
            this.RecordEndAddress(writer);
        }

    }

}