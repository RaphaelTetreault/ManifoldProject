using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.GMA
{
    [Serializable]
    public struct TextureDescriptor : IBinarySerializable, IBinaryAddressable
    {

        #region FIELDS


        public const int kFifoPaddingSize = 12;

        [Header("Texture")]
        [SerializeField]
        private AddressRange addressRange;

        [Space]

        /// <summary>
        /// 2019/04/03: Appears to be shader scrolling flags
        /// A good example is HOROGRAM
        /// </summary>
        [SerializeField, HexFlags("00 -", 2)]
        TexFlags0x00_U16 unk_0x00;

        /// <summary>
        /// Flags for mipmap generation(?)
        /// </summary>
        [SerializeField, HexFlags("02 -", 2)]
        TextureMipmapSetting mipmapSettings;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField, HexFlags("03 -", 2)]
        TextureWrapSetting wrapFlags;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField, Hex("04 -", 4)]
        ushort tplTextureIndex;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField, HexFlags("06 -", 2)]
        TexFlags0x06_U8 unk_0x06;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField, Hex("07 -", 2)]
        GXAnisotropy anisotropicLevel;

        /// <summary>
        /// 2019/04/01 VERIFIED: only 0
        /// </summary>
        [SerializeField, Hex("08 -", 8)]
        uint zero_0x08;

        /// <summary>
        /// 019/04/01 VERIFIED: All values from 0-255. May NOT be flags...
        /// </summary>
        [SerializeField, HexFlags("0C -", 2)]
        TexFlags0x0C_U8 unk_0x0C;

        /// <summary>
        /// 2019/04/03 VERIFIED: bool on models (13 total) that use swappable textures such as
        /// "Lap Gate" which indicates laps remaining and countdown display (3,2,1,GO!).
        /// </summary>
        [SerializeField, Hex("0D -", 2)]
        bool isSwappableTexture;

        /// <summary>
        /// Texture index
        /// </summary>
        [SerializeField, Hex("0E -", 4)]
        ushort index;

        /// <summary>
        /// 2019/04/01 VERIFIED: all unique values: 0, 1, 48, 49, 256, 304, 305, 512, 560, 561, 816, 818
        /// </summary>
        [SerializeField, HexFlags("10 -", 2)]
        TexFlags0x10_U32 unk_0x10;

        /// <summary>
        /// 
        /// </summary>
        byte[] fifoPadding;

        #endregion

        #region PROPERTIES


        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        public TexFlags0x00_U16 Unk_0x00
            => unk_0x00;

        public TextureMipmapSetting MipmapSettings
            => mipmapSettings;

        public TextureWrapSetting Wrapflags
            => wrapFlags;

        public ushort Tpltextureid
            => tplTextureIndex;

        public TexFlags0x06_U8 Unk_0x06
            => unk_0x06;

        public GXAnisotropy Anisotropiclevel
            => anisotropicLevel;

        public uint Zero_0x08
            => zero_0x08;

        public TexFlags0x0C_U8 Unk_0x0C
            => unk_0x0C;

        public bool IsSwappableTexture
            => isSwappableTexture;

        public ushort Index
            => index;

        public TexFlags0x10_U32 Unk_0x10
            => unk_0x10;

        public byte[] Fifopadding
            => fifoPadding;


        #endregion

        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref unk_0x00);
                reader.ReadX(ref mipmapSettings);
                reader.ReadX(ref wrapFlags);
                reader.ReadX(ref tplTextureIndex);
                reader.ReadX(ref unk_0x06);
                reader.ReadX(ref anisotropicLevel);
                reader.ReadX(ref zero_0x08);
                reader.ReadX(ref unk_0x0C);
                reader.ReadX(ref isSwappableTexture);
                reader.ReadX(ref index);
                reader.ReadX(ref unk_0x10);
                reader.ReadX(ref fifoPadding, kFifoPaddingSize);
            }
            this.RecordEndAddress(reader);

            foreach (var @byte in fifoPadding)
                Assert.IsTrue(@byte == 0x00);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(unk_0x00);
            writer.WriteX(mipmapSettings);
            writer.WriteX(wrapFlags);
            writer.WriteX(tplTextureIndex);
            writer.WriteX(unk_0x06);
            writer.WriteX(anisotropicLevel);
            writer.WriteX(zero_0x08);
            writer.WriteX(unk_0x0C);
            writer.WriteX(isSwappableTexture);
            writer.WriteX(index);
            writer.WriteX(unk_0x10);

            for (int i = 0; i < kFifoPaddingSize; i++)
            writer.WriteX((byte)0x00);
        }
    }
}
