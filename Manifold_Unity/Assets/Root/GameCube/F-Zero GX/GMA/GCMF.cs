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
        /// 16 bit flag (vertices are stored as uint16 format instead of float)
        /// </summary>
        _16Bit = 1 << 0, //0x01,
                         /// <summary>
                         /// Called "Stitching Model" in the debug menu. Has associated transform matrices.
                         /// </summary>
        StitchingModel = 1 << 2, //0x04,
                                 /// <summary>
                                 /// Called "Skin Model" in the debug menu. Has associated transform matrices and indexed vertices.
                                 /// </summary>
        SkinModel = 1 << 3, //0x08,
                            /// <summary>
                            /// Called "Effective Model" in the debug menu. Has indexed vertices.
                            /// </summary>
        EffectiveModel = 1 << 4 // 0x10,
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
        uint gcmfMagic;
        /// <summary>
        /// 2019/03/31 VERIFIED VALUES: 0, 1, 4, 5, 8, 16
        /// </summary>
        [Space]
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
        ushort textureCount;
        /// <summary>
        /// 2019/03/31 : number (nb) of materials
        /// </summary>
        [SerializeField, Hex("1A -", 4)]
        ushort materialCount;
        /// <summary>
        /// 2019/03/31 : number (nb) of translucid (tl) materials
        /// </summary>
        [SerializeField, Hex("1C -", 2)]
        ushort translucidMaterialCount;
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
        /// 
        /// </summary>
        [SerializeField, Hex("20 -", 8)]
        uint indicesRelPtr; // GxUtils: Header Size (size of header = offset)
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
        [SerializeField] Material material;
        // Mesh data - is it part of Material or not?

        #region PROPERTIES

        public GcmfAttributes_U32 Attributes
            => attributes;

        public Vector3 Origin
            => origin;

        public float Radius
            => radius;

        public ushort TextureCount
            => textureCount;

        public ushort MaterialCount
            => materialCount;

        public ushort TranslucidMaterialCount
            => translucidMaterialCount;

        public byte Transformmatrixcount
            => transformMatrixCount;

        public byte Zero_0x1F
            => zero_0x1F;

        public uint Indicesrelptr
            => indicesRelPtr;

        public uint Zero_0x24
            => zero_0x24;

        public byte[] Transformmatrixdefaultindices
            => transformMatrixDefaultIndices;

        #endregion

        public Texture[] Texture => texture;
        public TransformMatrix3x4[] TransformMatrix => transformMatrices;
        public VertexControlData VCD => vcd;
        public Material Material => material;

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
            reader.ReadX(ref indicesRelPtr);
            reader.ReadX(ref zero_0x24); Assert.IsTrue(zero_0x24 == 0);
            reader.ReadX(ref transformMatrixDefaultIndices, kTransformMatrixDefaultLength);
            reader.ReadX(ref fifoPadding, kFifoPaddingSize); foreach (var fifoPad in fifoPadding) Assert.IsTrue(fifoPad == 0);
            reader.ReadX(ref texture, textureCount, true);
            reader.ReadX(ref transformMatrices, transformMatrixCount, true); reader.Align(GxUtility.GX_FIFO_ALIGN);

            EndAddress = reader.BaseStream.Position;

            // Only present in certain models
            var isSkinModel = (attributes & GcmfAttributes_U32.SkinModel) != 0;
            var isEffectiveModel = (attributes & GcmfAttributes_U32.EffectiveModel) != 0;
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
            if (matCount > 0)
            {
                material = new Material
                {
                    ModelName = name,
                    FileName = fileName
                };
                reader.ReadX(ref material, false);
            }

            // TODO DisplayLists
            //var vat = GameCube.Games.FZeroGX.VAT;
            //mesh = new Mesh(vat, this);
            //reader.ReadX(ref mesh, false);
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
