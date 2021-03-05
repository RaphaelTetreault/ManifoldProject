using Manifold.IO;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;

namespace GameCube.GFZ.GMA
{

    /// <summary>
    /// Code from https://github.com/bobjrsenior/GxUtils/blob/master/GxUtils/LibGxFormat/Gma/Gcmf.cs
    /// </summary>
    [Flags]
    public enum GcmfAttributes_U32 : UInt32
    {
        /// <summary>
        /// Vertices are stored in 16-bit compressed floating point number format using GameCube GX conventions.
        /// </summary>
        IS_16_BIT = 1 << 0, //0x01,
        UNUSED_1 = 1 << 1,
        /// <summary>
        /// Called "Stitching Model" in the debug menu. Has associated transform matrices.
        /// </summary>
        IS_STITCHING_MODEL = 1 << 2, //0x04,
        /// <summary>
        /// Called "Skin Model" in the debug menu. Has associated transform matrices and indexed vertices.
        /// </summary>
        IS_SKIN_MODEL = 1 << 3, //0x08,
        /// <summary>
        /// Called "Effective Model" in the debug menu. Has physics-driven indexed vertices.
        /// </summary>
        IS_EFFECTIVE_MODEL = 1 << 4, // 0x10,
        UNUSED_5 = 1 << 5,
        UNUSED_6 = 1 << 6,
        UNUSED_7 = 1 << 7,
    }

    [Serializable]
    public class GcmfProperties : IBinarySerializable, IBinaryAddressableRange
    {

        #region FIELDS


        public const uint kGCMF = 0x47434D46; // 47 43 4D 46 - GCMF in ASCII
        public const int kFifoPaddingSize = 16;

        [Header("GCMF Properties")]
        [SerializeField]
        private AddressRange addressRange;

        /// <summary>
        /// 2019/03/31 VERIFIED: constant GCMF in ASCII
        /// </summary>
        [SerializeField, Hex("00", 8), Space]
        uint gcmfMagic;

        /// <summary>
        /// 2019/03/31 VERIFIED VALUES: 0, 1, 4, 5, 8, 16
        /// </summary>
        [SerializeField, HexFlags("04", 2)]
        GcmfAttributes_U32 attributes;

        /// <summary>
        /// 2019/03/31 : origin point
        /// </summary>
        [SerializeField, LabelPrefix("08")]
        Vector3 origin;

        /// <summary>
        /// 2019/03/31 : bounding sphere radius
        /// </summary>
        [SerializeField, LabelPrefix("14")]
        float radius;

        /// <summary>
        /// 2019/03/31 : number of texture references
        /// </summary>
        [SerializeField, Hex("18", 4)]
        short textureCount;

        /// <summary>
        /// 2019/03/31 : number (nb) of materials
        /// </summary>
        [SerializeField, Hex("1A", 4)]
        short materialCount;

        /// <summary>
        /// 2019/03/31 : number (nb) of translucid (tl) materials
        /// </summary>
        [SerializeField, Hex("1C", 4)]
        short translucidMaterialCount;

        /// <summary>
        /// 2019/03/31 : number of matrix entries
        /// </summary>
        [SerializeField, Hex("1E", 2)]
        byte transformMatrixCount;

        /// <summary>
        /// 2019/03/31 VERIFIED VALUES: only value is 0
        /// </summary>
        [SerializeField, Hex("1F", 2)]
        byte zero_0x1F;

        /// <summary>
        /// Size of GcmfProperties, Texture[] and TransformMatrix[]
        /// </summary>
        [SerializeField, Hex("20", 8)]
        int gcmfTexMtxSize;

        /// <summary>
        /// 2019/03/31 VERIFIED VALUES: only 0 
        /// </summary>
        [SerializeField, Hex("24", 8)]
        uint zero_0x24;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]//, LabelPrefix("28")]
        TransformMatrixIndexes8 defaultIndexes;

        /// <summary>
        /// 2019/03/31 VERIFIED VALUES: GameCube GX FIFO Padding to 32 bytes
        /// </summary>
        byte[] fifoPadding;


        #endregion

        #region PROPERTIES


        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        public GcmfAttributes_U32 Attributes
            => attributes;

        public Vector3 Origin
            => origin;

        public float Radius
            => radius;

        public short TextureCount
            => textureCount;

        public short MaterialCount
            => materialCount;

        public short TranslucidMaterialCount
            => translucidMaterialCount;

        public int TotalMaterialCount
            => materialCount + translucidMaterialCount;

        public byte TransformMatrixCount
            => transformMatrixCount;

        public byte Zero_0x1F
            => zero_0x1F;

        public int GcmfSize
            => gcmfTexMtxSize;

        public uint Zero_0x24
            => zero_0x24;

        public TransformMatrixIndexes8 DefaultIndexes
            => defaultIndexes;

        public bool IsSkinOrEffective
        {
            get
            {
                var isSkinModel = (attributes & GcmfAttributes_U32.IS_SKIN_MODEL) != 0;
                var isEffectiveModel = (attributes & GcmfAttributes_U32.IS_EFFECTIVE_MODEL) != 0;
                var isSkinOrEffective = isSkinModel || isEffectiveModel;

                return isSkinOrEffective;
            }
        }


        #endregion

        #region METHODS

        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref gcmfMagic); Assert.IsTrue(gcmfMagic == kGCMF);
                reader.ReadX(ref attributes);
                reader.ReadX(ref origin);
                reader.ReadX(ref radius);
                reader.ReadX(ref textureCount);
                reader.ReadX(ref materialCount);
                reader.ReadX(ref translucidMaterialCount);
                reader.ReadX(ref transformMatrixCount);
                reader.ReadX(ref zero_0x1F); Assert.IsTrue(zero_0x1F == 0);
                reader.ReadX(ref gcmfTexMtxSize);
                reader.ReadX(ref zero_0x24); Assert.IsTrue(zero_0x24 == 0);
                reader.ReadX(ref defaultIndexes, false);
                reader.ReadX(ref fifoPadding, kFifoPaddingSize);
            }
            this.RecordEndAddress(reader);

            foreach (var fifoPad in fifoPadding)
                Assert.IsTrue(fifoPad == 0);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(kGCMF);
            writer.WriteX(attributes);
            writer.WriteX(origin);
            writer.WriteX(radius);
            writer.WriteX(textureCount);
            writer.WriteX(materialCount);
            writer.WriteX(translucidMaterialCount);
            writer.WriteX(transformMatrixCount);
            writer.WriteX((byte)0);
            writer.WriteX(gcmfTexMtxSize);
            writer.WriteX((uint)0);
            writer.WriteX(defaultIndexes);

            for (int i = 0; i < kFifoPaddingSize; i++)
                writer.WriteX((byte)0x00);
        }


        #endregion

    }
}
