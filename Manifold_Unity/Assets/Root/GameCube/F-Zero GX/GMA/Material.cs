using GameCube.GX;
using StarkTools.IO;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace GameCube.FZeroGX.GMA
{
    #region MATERIAL ENUMS
    [Flags]
    public enum MatFlags0x00_U32 : uint
    {
        UNK_FLAG_0 = 1 << 0,
        UNK_FLAG_1 = 1 << 1,
        UNK_FLAG_2 = 1 << 2,
        UNK_FLAG_3 = 1 << 3,
        UNK_FLAG_4 = 1 << 4,
        UNK_FLAG_5 = 1 << 5,
        UNK_FLAG_6 = 1 << 6,
        UNK_FLAG_7 = 1 << 7,
        UNK_FLAG_8 = 1 << 8,
        UNK_FLAG_9 = 1 << 9,
        UNK_FLAG_10 = 1 << 10,
        UNK_FLAG_11 = 1 << 11,
        UNK_FLAG_12 = 1 << 12,
        UNK_FLAG_13 = 1 << 13,
        UNK_FLAG_14 = 1 << 14,
        UNK_FLAG_15 = 1 << 15,
    }

    [Flags]
    public enum MatFlags0x10_U8 : byte
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
    public enum MatFlags0x11_U8 : byte
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
    public enum MatFlags0x12_U8 : byte
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

    /// <summary>
    /// 0x13
    /// </summary>
    [Flags]
    public enum MatVertexRenderFlag_U8 : byte
    {
        RENDER_MATERIALS = 1 << 0,
        RENDER_TRANSLUCID_MATERIALS = 1 << 1,
        UNK_FLAG_2 = 1 << 2,
        UNK_FLAG_3 = 1 << 3,
        UNK_FLAG_4 = 1 << 4,
        UNK_FLAG_5 = 1 << 5,
        UNK_FLAG_6 = 1 << 6,
        UNK_FLAG_7 = 1 << 7,
    }

    /// <summary>
    /// 0x14
    /// </summary>
    [Flags]
    public enum MatFlags0x14_U8 : byte
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
    public enum MatFlags0x15_U8 : byte
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

    /// <summary>
    /// Flag form of Gx.GXAttr
    /// </summary>
    [Flags]
    public enum GXAttrFlag_U32 : UInt32
    {
        /// <summary>
        /// position/normal matrix index
        /// Note: used for character skinning
        /// </summary>
        GX_VA_PNMTXIDX = 1 << GXAttr.GX_VA_PNMTXIDX,
        /// <summary>
        /// texture 0 matrix index
        /// Note: used for character skinning
        /// </summary>
        GX_VA_TEX0MTXIDX = 1 << GXAttr.GX_VA_TEX0MTXIDX,
        /// <summary>
        /// texture 1 matrix index
        /// Note: used for character skinning
        /// </summary>
        GX_VA_TEX1MTXIDX = 1 << GXAttr.GX_VA_TEX1MTXIDX,
        /// <summary>
        /// texture 2 matrix index
        /// Note: used for character skinning
        /// </summary>
        GX_VA_TEX2MTXIDX = 1 << GXAttr.GX_VA_TEX2MTXIDX,
        /// <summary>
        /// texture 3 matrix index
        /// Note: used for character skinning
        /// </summary>
        GX_VA_TEX3MTXIDX = 1 << GXAttr.GX_VA_TEX3MTXIDX,
        /// <summary>
        /// texture 4 matrix index
        /// Note: used for character skinning
        /// </summary>
        GX_VA_TEX4MTXIDX = 1 << GXAttr.GX_VA_TEX4MTXIDX,
        /// <summary>
        /// texture 5 matrix index
        /// Note: used for character skinning
        /// </summary>
        GX_VA_TEX5MTXIDX = 1 << GXAttr.GX_VA_TEX5MTXIDX,
        /// <summary>
        /// texture 6 matrix index
        /// Note: used for character skinning
        /// </summary>
        GX_VA_TEX6MTXIDX = 1 << GXAttr.GX_VA_TEX6MTXIDX,
        /// <summary>
        /// texture 7 matrix index
        /// Note: used for character skinning
        /// </summary>
        GX_VA_TEX7MTXIDX = 1 << GXAttr.GX_VA_TEX7MTXIDX,

        /// <summary>
        /// position
        /// </summary>
        GX_VA_POS = 1 << GXAttr.GX_VA_POS,
        /// <summary>
        /// normal
        /// </summary>
        GX_VA_NRM = 1 << GXAttr.GX_VA_NRM,

        /// <summary>
        /// color 0
        /// </summary>
        GX_VA_CLR0 = 1 << GXAttr.GX_VA_CLR0,
        /// <summary>
        /// color 1
        /// </summary>
        GX_VA_CLR1 = 1 << GXAttr.GX_VA_CLR1,

        /// <summary>
        /// input texture coordinate 0
        /// </summary>
        GX_VA_TEX0 = 1 << GXAttr.GX_VA_TEX0,
        /// <summary>
        /// input texture coordinate 1
        /// </summary>
        GX_VA_TEX1 = 1 << GXAttr.GX_VA_TEX1,
        /// <summary>
        /// input texture coordinate 2
        /// </summary>
        GX_VA_TEX2 = 1 << GXAttr.GX_VA_TEX2,
        /// <summary>
        /// input texture coordinate 3
        /// </summary>
        GX_VA_TEX3 = 1 << GXAttr.GX_VA_TEX3,
        /// <summary>
        /// input texture coordinate 4
        /// </summary>
        GX_VA_TEX4 = 1 << GXAttr.GX_VA_TEX4,
        /// <summary>
        /// input texture coordinate 5
        /// </summary>
        GX_VA_TEX5 = 1 << GXAttr.GX_VA_TEX5,
        /// <summary>
        /// input texture coordinate 6
        /// </summary>
        GX_VA_TEX6 = 1 << GXAttr.GX_VA_TEX6,
        /// <summary>
        /// input texture coordinate 7
        /// </summary>
        GX_VA_TEX7 = 1 << GXAttr.GX_VA_TEX7,

        /// <summary>
        /// position matrix array pointer
        /// </summary>
        GX_VA_POS_MTX_ARRAY = 1 << GXAttr.GX_VA_POS_MTX_ARRAY,
        /// <summary>
        /// normal matrix array pointer
        /// </summary>
        GX_VA_NRM_MTX_ARRAY = 1 << GXAttr.GX_VA_NRM_MTX_ARRAY,
        /// <summary>
        /// texture matrix array pointer
        /// </summary>
        GX_VA_TEX_MTX_ARRAY = 1 << GXAttr.GX_VA_TEX_MTX_ARRAY,
        /// <summary>
        /// light matrix array pointer
        /// </summary>
        GX_VA_LIGHT_ARRAY = 1 << GXAttr.GX_VA_LIGHT_ARRAY,
        /// <summary>
        /// normal, bi-normal, tangent 
        /// </summary>
        GX_VA_NBT = 1 << GXAttr.GX_VA_NBT,
    }

    [Serializable]
    public class Material : IBinarySerializable, IBinaryAddressable, IFile
    {
        public const int kTransformArrayLength = 8;
        public const int kFifoPaddingSize = 28;

        [Header("Material")]
        [SerializeField] string name;
        [SerializeField, HideInInspector] string fileName;
        [SerializeField, Hex] long startAddress;
        [SerializeField, Hex] long endAddress;
        [Space]

        #region MEMBERS

        [SerializeField, HexFlags("00 -", 8)]
        MatFlags0x00_U32 unk_0x00;

        [SerializeField, Hex("04 -", 8)]
        Color32 color0;

        [SerializeField, Hex("04 -", 8)]
        Color32 color1;

        [SerializeField, Hex("04 -", 8)]
        Color32 color2;

        [SerializeField, HexFlags("10 -", 2)]
        MatFlags0x10_U8 unk_0x10;

        [SerializeField, HexFlags("11 -", 2)]
        MatFlags0x11_U8 unk_0x11;

        [SerializeField, HexFlags("12 -", 2)]
        MatFlags0x12_U8 unk_0x12;

        [SerializeField, HexFlags("13 -", 2)]
        MatVertexRenderFlag_U8 vertexRenderFlags;

        // GxUtils: used textures count
        [SerializeField, HexFlags("14 -", 2)]
        MatFlags0x14_U8 unk_0x14;

        [SerializeField, HexFlags("15 -", 2)]
        MatFlags0x14_U8 unk_0x15;

        [SerializeField, Hex("16 -", 4)]
        short tex0Index = -1;

        [SerializeField, Hex("18 -", 4)]
        short tex1Index = -1;

        [SerializeField, Hex("1A -", 4)]
        short tex2Index = -1;

        [SerializeField, HexFlags("1C -", 8)]
        GXAttrFlag_U32 vertexDescriptorFlags;

        // TODO: label prefix on top node, not members
        [SerializeField, Hex("20 -")]
        byte[] transformMatrixSpecidicIndices;

        [SerializeField, Hex("28 -", 8)]
        uint matDisplayListSize;

        [SerializeField, Hex("2C -", 8)]
        uint tlMatDisplayListSize;

        // origin bounding box
        [SerializeField, LabelPrefix("30 -")]
        Vector3 boundingSphereOrigin;

        [SerializeField, LabelPrefix("3C -")]
        [FormerlySerializedAs("unk_0x3C")]
        uint zero_0x3C;

        [SerializeField, LabelPrefix("40 -")]
        uint unk_0x40;

        byte[] fifoPadding;

        #endregion

        #region PROPERTIES

        // 0x00
        public MatFlags0x00_U32 Unk_0x00 => unk_0x00;
        public Color32 Color0 => color0;
        public Color32 Color1 => color1;
        public Color32 Color2 => color2;
        // 0x10
        public MatFlags0x10_U8 Unk_0x10 => unk_0x10;
        public MatFlags0x11_U8 Unk_0x11 => unk_0x11;
        public MatFlags0x12_U8 Unk_0x12 => unk_0x12;
        public MatVertexRenderFlag_U8 VertexRenderFlags => vertexRenderFlags;
        public MatFlags0x14_U8 Unk_0x14 => unk_0x14;
        public MatFlags0x14_U8 Unk_0x15 => unk_0x15;
        public short Tex0Index => tex0Index;
        public short Tex1Index => tex1Index;
        public short Tex2Index => tex2Index;
        public GXAttrFlag_U32 VertexDescriptorFlags => vertexDescriptorFlags;
        // 0x20
        public byte[] TransformMatrixSpecificIndices => transformMatrixSpecidicIndices;
        public uint MatDisplayListSize => matDisplayListSize;
        public uint TlMatDisplayListSize => tlMatDisplayListSize;
        // 0x30
        public Vector3 BoudingSphereOrigin => boundingSphereOrigin;
        public uint Zero_0x3C => zero_0x3C;
        public uint Unk_0x40 => unk_0x40;
        public byte[] Fifopadding => fifoPadding;
        // 0x60

        #endregion

        // Metadata
        public string FileName
        {
            get => fileName;
            set => fileName = value;
        }
        public string ModelName
        {
            get => name;
            set => name = value;
        }
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

            // 0x00
            reader.ReadX(ref unk_0x00);
            reader.ReadX(ref color0);
            reader.ReadX(ref color1);
            reader.ReadX(ref color2);
            // 0x10
            reader.ReadX(ref unk_0x10);
            reader.ReadX(ref unk_0x11);
            reader.ReadX(ref unk_0x12);
            reader.ReadX(ref vertexRenderFlags);
            reader.ReadX(ref unk_0x14);
            reader.ReadX(ref unk_0x15);
            reader.ReadX(ref tex0Index);
            reader.ReadX(ref tex1Index);
            reader.ReadX(ref tex2Index);
            reader.ReadX(ref vertexDescriptorFlags);
            // 0x20
            reader.ReadX(ref transformMatrixSpecidicIndices, kTransformArrayLength);
            reader.ReadX(ref matDisplayListSize);
            reader.ReadX(ref tlMatDisplayListSize);
            // 0x30
            reader.ReadX(ref boundingSphereOrigin);
            reader.ReadX(ref zero_0x3C);
            reader.ReadX(ref unk_0x40);
            reader.ReadX(ref fifoPadding, kFifoPaddingSize);

            EndAddress = reader.BaseStream.Position;

            for (int i = 0; i < fifoPadding.Length; i++)
                Assert.IsTrue(fifoPadding[i] == 0x00, $"Index {i}");
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

    }
}

