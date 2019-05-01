using GameCube.GX;
using StarkTools.IO;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace GameCube.FZeroGX.GMA
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
        IS_EFFECTIVE_MODEL = 1 << 4 // 0x10,
    }

    [Serializable]
    public class GcmfProperties : IBinarySerializable, IBinaryAddressable
    {
        public const uint kGCMF = 0x47434D46; // 47 43 4D 46 - GCMF in ASCII
        public const int kTransformMatrixDefaultLength = 8;
        public const int kFifoPaddingSize = 16;
        public const string kNullEntryName = "null";

        [Header("GCMF Properties")]
        [SerializeField, Hex(8)] long startAddress;
        [SerializeField, Hex(8)] long endAddress;

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
        /// Size of GCMF including Texture[] and TransformMatrix[], excluding
        /// VertexControlData (and associated data) and Material[]
        /// </summary>
        [SerializeField, Hex("20", 8)]
        int gcmfSize;

        /// <summary>
        /// 2019/03/31 VERIFIED VALUES: only 0 
        /// </summary>
        [SerializeField, Hex("24", 8)]
        uint zero_0x24;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField, Hex("28", 2)]
        byte[] transformMatrixDefaultIndices;

        /// <summary>
        /// 2019/03/31 VERIFIED VALUES: GameCube GX FIFO Padding to 32 bytes
        /// </summary>
        byte[] fifoPadding;


        #region PROPERTIES

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
            => gcmfSize;

        public uint Zero_0x24
            => zero_0x24;

        public byte[] TransformMatrixDefaultIndices
            => transformMatrixDefaultIndices;

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



        // Metadata
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

            reader.ReadX(ref gcmfMagic); Assert.IsTrue(gcmfMagic == kGCMF);
            reader.ReadX(ref attributes);
            reader.ReadX(ref origin);
            reader.ReadX(ref radius);
            reader.ReadX(ref textureCount);
            reader.ReadX(ref materialCount);
            reader.ReadX(ref translucidMaterialCount);
            reader.ReadX(ref transformMatrixCount);
            reader.ReadX(ref zero_0x1F); Assert.IsTrue(zero_0x1F == 0);
            reader.ReadX(ref gcmfSize);
            reader.ReadX(ref zero_0x24); Assert.IsTrue(zero_0x24 == 0);
            reader.ReadX(ref transformMatrixDefaultIndices, kTransformMatrixDefaultLength);
            reader.ReadX(ref fifoPadding, kFifoPaddingSize);
            foreach (var fifoPad in fifoPadding)
                Assert.IsTrue(fifoPad == 0);

            EndAddress = reader.BaseStream.Position;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(gcmfMagic); Assert.IsTrue(gcmfMagic == kGCMF);
            writer.WriteX(attributes);
            writer.WriteX(origin);
            writer.WriteX(radius);
            writer.WriteX(textureCount);
            writer.WriteX(materialCount);
            writer.WriteX(translucidMaterialCount);
            writer.WriteX(transformMatrixCount);
            writer.WriteX(zero_0x1F); Assert.IsTrue(zero_0x1F == 0);
            writer.WriteX(gcmfSize);
            writer.WriteX(zero_0x24); Assert.IsTrue(zero_0x24 == 0);
            writer.WriteX(transformMatrixDefaultIndices, false);

            for (int i = 0; i < kFifoPaddingSize; i++)
                writer.WriteX((byte)0x00);
        }
    }
}
