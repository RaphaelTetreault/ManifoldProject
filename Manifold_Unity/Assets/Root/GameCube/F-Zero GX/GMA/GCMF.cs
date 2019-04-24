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
    public class GCMF : IBinarySerializable, IBinaryAddressable, IFile
    {
        public const uint kGCMF = 0x47434D46; // 47 43 4D 46 - GCMF in ASCII
        public const int kTransformMatrixDefaultLength = 8;
        public const int kFifoPaddingSize = 16;
        public const string kNullEntryName = "null";

        [Header("GCMF")]
        [SerializeField] string name = kNullEntryName;
        [SerializeField, Hex(8)] long startAddress;
        [SerializeField, Hex(8)] long endAddress;
        [SerializeField, HideInInspector] string fileName;

        /// <summary>
        /// 2019/03/31 VERIFIED: constant GCMF in ASCII
        /// </summary>
        [SerializeField, Hex("00", 8), Space]
        uint gcmfMagic;

        /// <summary>
        /// 2019/03/31 VERIFIED VALUES: 0, 1, 4, 5, 8, 16
        /// </summary>
        [SerializeField, HexFlags("04 -", 2)]
        GcmfAttributes_U32 attributes;

        /// <summary>
        /// 2019/03/31 : origin point
        /// </summary>
        [SerializeField, LabelPrefix("08 -")]
        Vector3 origin;

        /// <summary>
        /// 2019/03/31 : bounding sphere radius
        /// </summary>
        [SerializeField, LabelPrefix("14 -")]
        float radius;

        /// <summary>
        /// 2019/03/31 : number of texture references
        /// </summary>
        [SerializeField, Hex("18 -", 4)]
        short textureCount;

        /// <summary>
        /// 2019/03/31 : number (nb) of materials
        /// </summary>
        [SerializeField, Hex("1A -", 4)]
        short materialCount;

        /// <summary>
        /// 2019/03/31 : number (nb) of translucid (tl) materials
        /// </summary>
        [SerializeField, Hex("1C -", 2)]
        short translucidMaterialCount;

        /// <summary>
        /// 2019/03/31 : number of matrix entries
        /// </summary>
        [SerializeField, Hex("1E -", 2)]
        byte transformMatrixCount;

        /// <summary>
        /// 2019/03/31 VERIFIED VALUES: only value is 0
        /// </summary>
        [SerializeField, Hex("1F -", 2)]
        byte zero_0x1F;

        /// <summary>
        /// Size of GCMF including Texture[] and TransformMatrix[], excluding
        /// VertexControlData (and associated data) and Material[]
        /// </summary>
        [SerializeField, Hex("20 -", 8)]
        int gcmfSize;

        /// <summary>
                            /// 2019/03/31 VERIFIED VALUES: only 0 
                            /// </summary>
        [SerializeField, Hex("24 -", 8)]
        uint zero_0x24;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField, Hex("28 -", 2)]
        byte[] transformMatrixDefaultIndices;

        /// <summary>
        /// 2019/03/31 VERIFIED VALUES: GameCube GX FIFO Padding to 32 bytes
        /// </summary>
        byte[] fifoPadding;

        [SerializeField] Texture[] texture;
        [SerializeField] TransformMatrix3x4[] transformMatrices;
        [SerializeField] VertexControlData vcd;
        [SerializeField] Material[] materials;
        // Mesh data - is it part of Material or not?

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

        public byte Transformmatrixcount
            => transformMatrixCount;

        public byte Zero_0x1F
            => zero_0x1F;

        public int GcmfSize
            => gcmfSize;

        public uint Zero_0x24
            => zero_0x24;

        public byte[] TransformMatrixDefaultIndices
            => transformMatrixDefaultIndices;

        #endregion

        public Texture[] Texture => texture;
        public TransformMatrix3x4[] TransformMatrix => transformMatrices;
        public VertexControlData VCD => vcd;
        public Material[] Materials => materials;

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
            reader.ReadX(ref fifoPadding, kFifoPaddingSize); foreach (var fifoPad in fifoPadding) Assert.IsTrue(fifoPad == 0);
            reader.ReadX(ref texture, textureCount, true);
            reader.ReadX(ref transformMatrices, transformMatrixCount, true); reader.Align(GxUtility.GX_FIFO_ALIGN);


            // Only present in certain models
            var isSkinModel = (attributes & GcmfAttributes_U32.IS_SKIN_MODEL) != 0;
            var isEffectiveModel = (attributes & GcmfAttributes_U32.IS_EFFECTIVE_MODEL) != 0;
            if (isSkinModel || isEffectiveModel)
            {
                vcd = new VertexControlData
                {
                    ModelName = name,
                    FileName = fileName
                };
                reader.ReadX(ref vcd, false);
            }

            // TODO: see if there are multiple materials per "mt_count"
            // If so, we need to create a Material[] instead of a simple check
            var matCount = materialCount + translucidMaterialCount;
            materials = new Material[matCount];
            //Debug.Log($"ADDR:{reader.BaseStream.Position.ToString("X8")} - {matCount}");
            for (int i = 0; i < matCount; i++)
            {
                var startAddr = reader.BaseStream.Position;
                var material = new Material
                {
                    ModelName = name,
                    FileName = fileName
                };
                reader.ReadX(ref material, false);

                // looks like this can have a value even if no verts for effective model?
                // Count for something else?
                var matDLSize = (isEffectiveModel || isSkinModel) ? 0 : material.MatDisplayListSize;
                var tlMatDLSize = (isEffectiveModel || isSkinModel) ? 0 : material.TlMatDisplayListSize;

                var seekAddr = startAddr + matDLSize + tlMatDLSize + 0x60;
                reader.BaseStream.Seek(seekAddr, SeekOrigin.Begin);

                materials[i] = material;

                var isRenderFlag2 = (material.VertexRenderFlags & MatVertexRenderFlag_U8.RENDER_SKIN_OR_EFFECTIVE_A) != 0;
                var isRenderFlag3 = (material.VertexRenderFlags & MatVertexRenderFlag_U8.RENDER_SKIN_OR_EFFECTIVE_B) != 0;
                if (isRenderFlag2 && isRenderFlag3)
                {
                    // Only temp ATM
                    var temp = new SkinRenderHeader();
                    reader.ReadX(ref temp, true);

                    var offset = temp.VertexSize0 + temp.VertexSize1;
                    reader.BaseStream.Seek(offset, SeekOrigin.Current);
                }
            }

            // TODO DisplayLists
            //var vat = GameCube.Games.FZeroGX.VAT;
            //mesh = new Mesh(vat, this);
            //reader.ReadX(ref mesh, false);

            EndAddress = reader.BaseStream.Position;
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
