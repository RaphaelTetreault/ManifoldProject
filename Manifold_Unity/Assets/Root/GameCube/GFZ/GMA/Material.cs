using GameCube.GX;
using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.GMA
{
    [Serializable]
    public class Material : IBinarySerializable, IBinaryAddressable
    {

        #region FIELDS


        public const int kTransformArrayLength = 8;
        public const int kFifoPaddingSize = 28;

        [Header("Material")]
        [SerializeField]
        private AddressRange addressRange;

        [Space]
        [SerializeField, Hex("00", 8)]
        ushort zero_0x00;

        /// <summary>
        /// Literally only used once for Mat [7/7] on st13 C13_ROAD01. See unk_0x3C
        /// </summary>
        [SerializeField, HexFlags("02", 8)]
        MatFlags0x02_U8 unk_0x02;

        [SerializeField, HexFlags("03", 8)]
        MatFlags0x03_U8 unk_0x03;

        [SerializeField, LabelPrefix("04")]
        Color32 color0;

        [SerializeField, LabelPrefix("08")]
        Color32 color1;

        [SerializeField, LabelPrefix("0C")]
        Color32 color2;

        [SerializeField, HexFlags("10", 2)]
        MatFlags0x10_U8 unk_0x10;

        [SerializeField, HexFlags("11", 2)]
        MatFlags0x11_U8 unk_0x11;

        [SerializeField, Hex("12", 2)]
        sbyte texturesUsedCount;

        [SerializeField, HexFlags("13", 2)]
        RenderDisplayListSetting vertexRenderFlags;

        [SerializeField, Hex("14", 2)]
        sbyte unk_0x14;

        [SerializeField, HexFlags("15", 2)]
        MatFlags0x15_U8 unk_0x15;

        [SerializeField, Hex("16", 4)]
        short tex0Index = -1;

        [SerializeField, Hex("18", 4)]
        short tex1Index = -1;

        [SerializeField, Hex("1A", 4)]
        short tex2Index = -1;

        [SerializeField, HexFlags("1C", 8)]
        GXAttributes vertexAttributeFlags;

        // TODO: label prefix on top node, not members
        [SerializeField, Hex("20")]
        TransformMatrixIndexes8 matrixIndexes;

        [SerializeField, Hex("28", 8)]
        int matDisplayListSize;

        [SerializeField, Hex("2C", 8)]
        int tlMatDisplayListSize;

        [SerializeField, LabelPrefix("30")]
        Vector3 boundingSphereOrigin;

        /// <summary>
        /// Literally only used once for Mat [7/7] on st13 C13_ROAD01. Value is 1f
        /// </summary>
        [SerializeField, LabelPrefix("3C")]
        float unk_0x3C;

        [SerializeField, HexFlags("40")]
        MatFlags0x40_U32 unk_0x40;

        byte[] fifoPadding;


        #endregion

        #region PROPERTIES

        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        public bool IsRenderExtraDisplayLists
        {
            get
            {
                var renderExtraDisplayList0 = (vertexRenderFlags & RenderDisplayListSetting.RENDER_EX_DISPLAY_LIST_0) != 0;
                var renderExtraDisplayList1 = (vertexRenderFlags & RenderDisplayListSetting.RENDER_EX_DISPLAY_LIST_1) != 0;
                var renderExtraDisplayList01 = renderExtraDisplayList0 && renderExtraDisplayList1;
                return renderExtraDisplayList01;
            }
        }

        // 0x00
        public ushort Zero_0x00 => zero_0x00;
        public MatFlags0x02_U8 Unk_0x02 => unk_0x02;
        public MatFlags0x03_U8 Unk_0x03 => unk_0x03;
        public Color32 Color0 => color0;
        public Color32 Color1 => color1;
        public Color32 Color2 => color2;
        // 0x10
        public MatFlags0x10_U8 Unk_0x10 => unk_0x10;
        public MatFlags0x11_U8 Unk_0x11 => unk_0x11;
        public sbyte TexturesUsedCount => texturesUsedCount;
        public RenderDisplayListSetting VertexRenderFlags => vertexRenderFlags;
        public sbyte Unk_0x14 => unk_0x14;
        public MatFlags0x15_U8 Unk_0x15 => unk_0x15;
        public short Tex0Index => tex0Index;
        public short Tex1Index => tex1Index;
        public short Tex2Index => tex2Index;
        public GXAttributes VertexDescriptorFlags => vertexAttributeFlags;
        // 0x20
        public TransformMatrixIndexes8 MatrixIndexes => matrixIndexes;
        public int MatDisplayListSize => matDisplayListSize;
        public int TlMatDisplayListSize => tlMatDisplayListSize;
        // 0x30
        public Vector3 BoudingSphereOrigin => boundingSphereOrigin;
        public float Unk_0x3C => unk_0x3C;
        public MatFlags0x40_U32 Unk_0x40 => unk_0x40;
        public byte[] Fifopadding => fifoPadding;
        // 0x60


        #endregion

        #region METHODS


        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                // 0x00
                reader.ReadX(ref zero_0x00);
                Assert.IsTrue(zero_0x00 == 0, $"{typeof(Material).Name}. Addr:{addressRange.startAddress:X8}");

                reader.ReadX(ref unk_0x02);
                reader.ReadX(ref unk_0x03);
                reader.ReadX(ref color0);
                reader.ReadX(ref color1);
                reader.ReadX(ref color2);
                // 0x10
                reader.ReadX(ref unk_0x10);
                reader.ReadX(ref unk_0x11);
                reader.ReadX(ref texturesUsedCount);
                reader.ReadX(ref vertexRenderFlags);
                reader.ReadX(ref unk_0x14);
                reader.ReadX(ref unk_0x15);
                reader.ReadX(ref tex0Index);
                reader.ReadX(ref tex1Index);
                reader.ReadX(ref tex2Index);
                reader.ReadX(ref vertexAttributeFlags);
                // 0x20
                reader.ReadX(ref matrixIndexes, false);
                reader.ReadX(ref matDisplayListSize);
                reader.ReadX(ref tlMatDisplayListSize);
                // 0x30
                reader.ReadX(ref boundingSphereOrigin);
                reader.ReadX(ref unk_0x3C);
                reader.ReadX(ref unk_0x40);
                reader.ReadX(ref fifoPadding, kFifoPaddingSize);
            }
            this.RecordEndAddress(reader);

            for (int i = 0; i < fifoPadding.Length; i++)
                Assert.IsTrue(fifoPadding[i] == 0x00);
        }

        public void Serialize(BinaryWriter writer)
        {
            // 0x00
            writer.WriteX(zero_0x00); Assert.IsTrue(zero_0x00 == 0);
            writer.WriteX(unk_0x02);
            writer.WriteX(unk_0x03);
            writer.WriteX(color0);
            writer.WriteX(color1);
            writer.WriteX(color2);
            // 0x10
            writer.WriteX(unk_0x10);
            writer.WriteX(unk_0x11);
            writer.WriteX(texturesUsedCount);
            writer.WriteX(vertexRenderFlags);
            writer.WriteX(unk_0x14);
            writer.WriteX(unk_0x15);
            writer.WriteX(tex0Index);
            writer.WriteX(tex1Index);
            writer.WriteX(tex2Index);
            writer.WriteX(vertexAttributeFlags);
            // 0x20
            writer.WriteX(matrixIndexes);
            writer.WriteX(matDisplayListSize);
            writer.WriteX(tlMatDisplayListSize);
            // 0x30
            writer.WriteX(boundingSphereOrigin);
            writer.WriteX(unk_0x3C);
            writer.WriteX(unk_0x40);

            for (int i = 0; i < kFifoPaddingSize; i++)
                writer.WriteX((byte)0x00);
        }


        #endregion

    }
}

