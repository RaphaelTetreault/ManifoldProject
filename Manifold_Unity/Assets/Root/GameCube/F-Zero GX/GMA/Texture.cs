using StarkTools.IO;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;

namespace GameCube.GFZX01.GMA
{
    #region TEXTURE ENUMS

    /// <summary>
    /// Notes:
    /// Combinations: 4&5 (x1986), 1&3 (x7), 1&4 (x1)
    /// Flags used: 1, 3, 4, 5, 6
    /// </summary>

    [Flags]
    public enum TexFlags0x00_U16 : ushort
    {
        /// <summary>
        /// Unused
        /// </summary>
        UNUSED_FLAG_0 = 1 << 0,
        /// <summary>
        /// Based on st24 models, uv scroll. Perhaps x/y depends on another flag?
        /// </summary>
        ENABLE_UV_SCROLL = 1 << 1,
        /// <summary>
        /// Unused
        /// </summary>
        UNUSED_FLAG_2 = 1 << 2,
        /// <summary>
        /// 7 occurences total. (st21,lz.gma, [75,76,77/130] guide_light*, [1/6])
        /// </summary>
        UNK_FLAG_3 = 1 << 3,
        /// <summary>
        /// Appears to be used whenever tex is for bg reflections
        /// </summary>
        UNK_FLAG_4 = 1 << 4,
        /// <summary>
        /// ..?
        /// </summary>
        UNK_FLAG_5 = 1 << 5,
        /// <summary>
        /// Appears to be used whenever tex is for bg reflections
        /// </summary>
        UNK_FLAG_6 = 1 << 6,
        /// <summary>
        /// Unused
        /// </summary>
        UNUSED_FLAG_7 = 1 << 7,
    }

    [Flags]
    public enum TexWrapFlags_U8 : byte
    {
        UNK_FLAG_0 = 1 << 0,
        UNK_FLAG_1 = 1 << 1,
        REPEAT_X = 1 << 2,
        MIRROR_X = 1 << 3,
        REPEAT_Y = 1 << 4,
        MIRROR_Y = 1 << 5,
        UNK_FLAG_6 = 1 << 6,
        UNK_FLAG_7 = 1 << 7,
    }

    // if 0&1, enable 2
    /// <summary>
    /// MIPMAP Settings
    /// Look at: GXTexFilter
    /// </summary>
    [Flags]
    public enum TexMipmapSettings_U8 : byte
    {
        /// <summary>
        /// 2019/04/03 VERIFIED: Enable (large preview) mipmaps
        /// </summary>
        ENABLE_MIPMAP = 1 << 0,
        /// <summary>
        /// 2019/04/03 THEORY: when only flag, "use custom mip-map"
        /// See: bg_san_s [39/41] tex [3/8] - RIVER01
        /// See: bg_big [118/120] tex [1/1] - OCE_OCEAN_C14_B_ltmp2
        /// See: any recovery pad
        /// </summary>
        UNK_FLAG_1 = 1 << 1, // Working together?
        UNK_FLAG_2 = 1 << 2, // Working together?
        /// <summary>
        /// 2019/04/03 VERIFIED: Enable Mipmap NEAR
        /// </summary>
        ENABLE_NEAR = 1 << 3,
        /// <summary>
        /// Height map? Blend? (they are greyscale)
        /// Low occurences: 188 for tracks and st2 boulders
        /// </summary>
        UNK_FLAG_4 = 1 << 4,
        /// <summary>
        /// Used as alpha mask? (likely?) Perhaps some mip preservation stuff.
        /// </summary>
        UNK_FLAG_5 = 1 << 5,
        /// <summary>
        /// Total occurences = 3. Only MCSO, on a single geometry set. Perhaps error from devs?
        /// </summary>
        UNK_FLAG_6 = 1 << 6, // only on 3?
        /// <summary>
        /// On many vehicles
        /// </summary>
        UNK_FLAG_7 = 1 << 7,
    }

    [Flags]
    public enum TexFlags0x06_U8 : byte
    {
        UNK_FLAG_0 = 1 << 0,
        UNK_FLAG_1 = 1 << 1,
        UNK_FLAG_2 = 1 << 2,
        UNK_FLAG_3 = 1 << 3,
        UNK_FLAG_4 = 1 << 4,
        UNK_FLAG_5 = 1 << 5,
        UNK_FLAG_6 = 1 << 6,
        UNK_FLAG_7 = 1 << 7,
    }

    [Flags]
    public enum TexFlags0x0C_U8 : byte
    {
        UNK_FLAG_0 = 1 << 0,
        UNK_FLAG_1 = 1 << 1,
        UNK_FLAG_2 = 1 << 2,
        UNK_FLAG_3 = 1 << 3,
        UNK_FLAG_4 = 1 << 4,
        UNK_FLAG_5 = 1 << 5,
        UNK_FLAG_6 = 1 << 6,
        UNK_FLAG_7 = 1 << 7,
    }

    [Flags]
    public enum TexFlags0x10_U32 : uint
    {
        UNK_FLAG_0 = 1 << 0,
        UNK_FLAG_1 = 1 << 1,
        UNK_FLAG_2 = 1 << 2,
        UNK_FLAG_3 = 1 << 3,
        UNK_FLAG_4 = 1 << 4,
        UNK_FLAG_5 = 1 << 5,
        UNK_FLAG_6 = 1 << 6,
        UNK_FLAG_7 = 1 << 7,
    }

    #endregion


    public enum GXAnisotropy_U8 : byte
    {
        GX_ANISO_1,
        GX_ANISO_2,
        GX_ANISO_4,
    }


    [Serializable]
    public struct TextureDescriptor : IBinarySerializable, IBinaryAddressable
    {
        public const int kFifoPaddingSize = 12;

        [Header("Texture")]
        [SerializeField, Hex] long startAddress;
        [SerializeField, Hex] long endAddress;
        [Space]

        #region MEMBERS

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
        TexMipmapSettings_U8 mipmapSettings;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField, HexFlags("03 -", 2)]
        TexWrapFlags_U8 wrapFlags;

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
        GXAnisotropy_U8 anisotropicLevel;

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

        #region PROEPRTIES

        public TexFlags0x00_U16 Unk_0x00
            => unk_0x00;

        public TexMipmapSettings_U8 MipmapSettings
            => mipmapSettings;

        public TexWrapFlags_U8 Wrapflags
            => wrapFlags;

        public ushort Tpltextureid
            => tplTextureIndex;

        public TexFlags0x06_U8 Unk_0x06
            => unk_0x06;

        public GXAnisotropy_U8 Anisotropiclevel
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

        public long StartAddress
        {
            get => startAddress;
            set => startAddress = value;
        }
        public long EndAddress
        {
            get => endAddress;
            set => endAddress = value;
        }

        public void Deserialize(BinaryReader reader)
        {
            StartAddress = reader.BaseStream.Position;

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
            foreach (var @byte in fifoPadding)
                Assert.IsTrue(@byte == 0x00);

            EndAddress = reader.BaseStream.Position;
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
