using GameCube.GX;
using StarkTools.IO;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;

// BobJrSr
// https://github.com/bobjrsenior/GxUtils/blob/master/GxUtils/LibGxFormat/Gma/GMAFormat.xml
// https://github.com/bobjrsenior/GxUtils/blob/master/GxUtils/LibGxFormat/Gma/GcmfMesh.cs

// My old docs
// http://www.gc-forever.com/forums/viewtopic.php?f=35&t=1897&p=26717&hilit=TPL#p26717
// http://www.gc-forever.com/forums/viewtopic.php?t=2311
// https://docs.google.com/spreadsheets/d/1WS9H2GcPGnjbOo7KS37tiySXDHRIHE3kcTbHLG0ICgc/edit#gid=0

// LZ resources
// https://www.google.com/search?q=lempel+ziv&rlz=1C1CHBF_enCA839CA839&oq=lempel+&aqs=chrome.0.0j69i57j0l4.2046j0j4&sourceid=chrome&ie=UTF-8

// noclip.website
// https://github.com/magcius/noclip.website/blob/master/src/metroid_prime/mrea.ts

// Dolphin (for patching files)
// https://forums.dolphin-emu.org/Thread-how-to-decompile-patch-a-gamecube-game

namespace GameCube.Games
{
    public static class FZeroGX
    {
        // TODO
        // pass GXAttrType at render time (where is index array?)

        private static readonly GxVertexAttributeFormat vaf0 = new GxVertexAttributeFormat()
        {
            pos = new GxVertexAttribute(GXAttrType.GX_DIRECT, GXAttr.GX_VA_POS, GXCompCnt_Rev2.GX_POS_XYZ, GXCompType.GX_F32, 0),
            nrm = new GxVertexAttribute(GXAttrType.GX_DIRECT, GXAttr.GX_VA_NRM, GXCompCnt_Rev2.GX_NRM_XYZ, GXCompType.GX_F32, 0),
            clr0 = new GxVertexAttribute(GXAttrType.GX_DIRECT, GXAttr.GX_VA_CLR0, GXCompCnt_Rev2.GX_CLR_RGBA, GXCompType.GX_F32, 0),
            tex0 = new GxVertexAttribute(GXAttrType.GX_DIRECT, GXAttr.GX_VA_TEX0, GXCompCnt_Rev2.GX_TEX_ST, GXCompType.GX_F32, 0),
            tex1 = new GxVertexAttribute(GXAttrType.GX_DIRECT, GXAttr.GX_VA_TEX1, GXCompCnt_Rev2.GX_TEX_ST, GXCompType.GX_F32, 0),
            tex2 = new GxVertexAttribute(GXAttrType.GX_DIRECT, GXAttr.GX_VA_TEX2, GXCompCnt_Rev2.GX_TEX_ST, GXCompType.GX_F32, 0),
            nbt = new GxVertexAttribute(GXAttrType.GX_DIRECT, GXAttr.GX_VA_NBT, GXCompCnt_Rev2.GX_NRM_NBT, GXCompType.GX_F32, 0),
        };

        private static readonly GxVertexAttributeFormat vaf1 = new GxVertexAttributeFormat()
        {
            pos = new GxVertexAttribute(GXAttrType.GX_INDEX16, GXAttr.GX_VA_POS, GXCompCnt_Rev2.GX_POS_XYZ, GXCompType.GX_S16, 0),
            nrm = new GxVertexAttribute(GXAttrType.GX_INDEX16, GXAttr.GX_VA_NRM, GXCompCnt_Rev2.GX_POS_XYZ, GXCompType.GX_S16, 0),
            tex0 = new GxVertexAttribute(GXAttrType.GX_INDEX16, GXAttr.GX_VA_TEX0, GXCompCnt_Rev2.GX_TEX_ST, GXCompType.GX_S16, 0),
        };

        public static readonly GxVertexAttributeTable VAT = new GxVertexAttributeTable(vaf0, vaf1);
    }
}

#region DEFINITIONS

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
public enum MatFlags0x14_U16 : ushort
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
#endregion

#region TEXTURE ENUMS

[Flags]
public enum TexFlags0x00_U16 : ushort
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

[Flags]
public enum TexFlags0x02_U8 : byte
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
public enum TexFlags0x0D_U8 : byte
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

/// <summary>
/// Code from https://github.com/bobjrsenior/GxUtils/blob/master/GxUtils/LibGxFormat/Gma/Gcmf.cs
/// </summary>
[Flags]
public enum GcmfAttributes : UInt32
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
    /// Called "Skin Model" in the debug menu. Has associated transform matrices and indexed vartices.
    /// </summary>
    SkinModel = 1 << 3, //0x08,
    /// <summary>
    /// Called "Effective Model" in the debug menu. Has indexed vertices.
    /// </summary>
    EffectiveModel = 1 << 4 // 0x10,
}


/// <summary>
/// Flag form of Gx.GXAttr
/// 
/// Is it off-by-one-shift or does GX used a shifted set?
/// </summary>
[Flags]
public enum GMA_GXAttrFlagU32 : UInt32
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

/// <summary>
/// MEANT AS TEMP until you solve this enum compression thing
/// </summary>
public enum GMA_GXAnisotropyU8 : byte
{
    GX_ANISO_1,
    GX_ANISO_2,
    GX_ANISO_4,
}


[Serializable]
public class GMA : IBinarySerializable, IFile
{
    #region MEMBERS

    private const int kGmaHeaderSize = 8;

    [SerializeField] private string fileName;
    [SerializeField] private int gcmfCount;
    [SerializeField] private uint headerEndPosition;

    public const uint kNullPtr = 0xFFFFFFFF;
    [SerializeField] private GCMF[] gcmf;

    #endregion

    #region PROPERTIES

    public uint HeaderEndPosition => headerEndPosition;
    public int GcmfCount => gcmfCount;
    public int GcmfNamePtr => (0x08) + gcmfCount * 0x08;
    public GCMF[] GCMF => gcmf;

    public string FileName
    {
        get => fileName;
        set => fileName = value;
    }

    #endregion

    #region METHODS 

    public void Deserialize(BinaryReader reader)
    {
        reader.ReadX(ref gcmfCount);
        reader.ReadX(ref headerEndPosition);

        gcmf = new GCMF[GcmfCount];
        for (int i = 0; i < gcmf.Length; i++)
        {
            uint gcmfRelativePtr = 0;
            uint nameRelativePtr = 0;
            reader.ReadX(ref gcmfRelativePtr);
            reader.ReadX(ref nameRelativePtr);

            // If 1st of 2 pointers is null, skip
            if (gcmfRelativePtr == kNullPtr)
            {
                gcmf[i] = null;
                continue;
            }

            var endPosition = reader.BaseStream.Position;

            var modelName = string.Empty;
            var modelNamePtr = nameRelativePtr + GcmfNamePtr;
            reader.BaseStream.Seek(modelNamePtr, SeekOrigin.Begin);
            reader.ReadXCString(ref modelName, Encoding.ASCII);

            gcmf[i] = new GCMF();
            gcmf[i].ModelName = modelName;
            gcmf[i].FileName = FileName;

            var gcmfPtr = gcmfRelativePtr + HeaderEndPosition;
            reader.BaseStream.Seek(gcmfPtr, SeekOrigin.Begin);
            reader.ReadX(ref gcmf[i], false);

            reader.BaseStream.Seek(endPosition, SeekOrigin.Begin);
        }
    }

    public void Serialize(BinaryWriter writer)
    {
        throw new NotImplementedException();
    }

    #endregion
}

[Serializable]
public class GCMF : IBinarySerializable, IBinaryAddressable, IFile
{
    public const uint kGCMF = 0x47434D46; // 47 43 4D 46 - GCMF in ASCII
    public const int kTransformMatrixDefaultLength = 8;
    public const int kFifoPaddingSize = 16;

    [Header("Debugging")]
    [SerializeField] long startAddress;
    [SerializeField] long endAddress;
    [SerializeField] string modelName;
    [SerializeField] string fileName;

    [Header("Content")]
    /// <summary>
    /// 2019/03/31 VERIFIED: constant GCMF in ASCII
    /// </summary>
    private uint gcmfMagic;
    /// <summary>
    /// 2019/03/31 VERIFIED VALUES: 0, 1, 4, 5, 8, 16
    /// </summary>
    [SerializeField] private GcmfAttributes attributes;
    /// <summary>
    /// 2019/03/31 : origin point
    /// </summary>
    [SerializeField] private Vector3 origin;
    /// <summary>
    /// 2019/03/31 : bounding sphere radius
    /// </summary>
    [SerializeField] private float radius;
    /// <summary>
    /// 2019/03/31 : number of texture references
    /// </summary>
    [SerializeField] private ushort textureCount;
    /// <summary>
    /// 2019/03/31 : number (nb) of materials
    /// </summary>
    [SerializeField] private ushort materialCount;
    /// <summary>
    /// 2019/03/31 : number (nb) of translucid (tl) materials
    /// </summary>
    [SerializeField] private ushort translucidMaterialCount;
    /// <summary>
    /// 2019/03/31 : number of matrix entries
    /// </summary>
    [SerializeField] private byte transformMatrixCount;
    /// <summary>
    /// 2019/03/31 VERIFIED VALUES: only value is 0
    /// </summary>
    [SerializeField] private byte zero_0x1F;
    /// <summary>
    /// 
    /// </summary>
    [SerializeField] private uint indicesRelPtr; // GxUtils: Header Size
    /// <summary>
    /// 2019/03/31 VERIFIED VALUES: only 0 
    /// </summary>
    [SerializeField] private uint zero_0x24;
    /// <summary>
    /// 
    /// </summary>
    [SerializeField] private byte[] transformMatrixDefaultIndices;
    /// <summary>
    /// 2019/03/31 VERIFIED VALUES: GameCube GX FIFO Padding to 32 bytes
    /// </summary>
    private byte[] fifoPadding;

    [SerializeField] private Texture[] texture;
    [SerializeField] private UnknownMaterialStructure? unkMatStruct;
    [SerializeField] private TransformMatrix3x4[] transformMatrices;
    [SerializeField] private Material material;
    //[SerializeField] private Mesh mesh;

    public GcmfAttributes Attributes => attributes;
    public Vector3 Origin => origin;
    public float Radius => radius;
    public ushort TextureCount => textureCount;
    public ushort Materialcount => materialCount;
    public ushort Translucidmaterialcount => translucidMaterialCount;
    public byte Transformmatrixcount => transformMatrixCount;
    public byte Zero_0x1F => zero_0x1F;
    public uint Indicesrelptr => indicesRelPtr;
    public uint Zero_0x24 => zero_0x24;
    public byte[] Transformmatrixdefaultindices => transformMatrixDefaultIndices;

    public Texture[] Texture => texture;
    public UnknownMaterialStructure? UnknownMaterialStructure => unkMatStruct;
    public TransformMatrix3x4[] TransformMatrix => transformMatrices;
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
        get => modelName;
        set => modelName = value;
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

        // TODO: see if there are multiple materials per "mt_count"
        // If so, we need to create a Material[] instead of a simple check
        var matCount = materialCount + translucidMaterialCount;
        if (matCount > 0)
        {
            material = new Material(); // temp
            material.ModelName = modelName;
            material.FileName = fileName;
            reader.ReadX(ref material, false);
        }

        // TODO DisplayLists
    }

    public void Serialize(BinaryWriter writer)
    {
        throw new NotImplementedException();
    }
}


[Serializable]
public class Texture : IBinarySerializable, IBinaryAddressable
{
    public const int kFifoPaddingSize = 12;

    [Header("Debugging")]
    [SerializeField] long startAddress;
    [SerializeField] long endAddress;

    [Header("Content")]
    [SerializeField, EnumFlags] TexFlags0x00_U16 unk_0x00;
    [SerializeField, EnumFlags] TexFlags0x02_U8 unk_0x02;
    [SerializeField, EnumFlags] TexWrapFlags_U8 wrapFlags;
    [SerializeField] ushort tplTextureID;
    [SerializeField, EnumFlags] TexFlags0x06_U8 unk_0x06;
    [SerializeField] GMA_GXAnisotropyU8 anisotropicLevel;
    /// <summary>
    /// 2019/04/01 VERIFIED: only 0
    /// </summary>
    [SerializeField] uint zero_0x08;
    /// <summary>
    /// 019/04/01 VERIFIED: All values from 0-255. May NOT be flags...
    /// </summary>
    [SerializeField, EnumFlags] TexFlags0x0C_U8 unk_0x0C;
    /// <summary>
    /// 2019/04/01 VERIFIED: only values are 0, 1.
    /// </summary>
    [SerializeField, EnumFlags] TexFlags0x0D_U8 unk_0x0D;
    /// <summary>
    /// Texture index
    /// </summary>
    [SerializeField] ushort index;
    /// <summary>
    /// 2019/04/01 VERIFIED: all unique values: 0, 1, 48, 49, 256, 304, 305, 512, 560, 561, 816, 818
    /// </summary>
    [SerializeField, EnumFlags] TexFlags0x10_U32 unk_0x10;
    [SerializeField] byte[] fifoPadding;

    public TexFlags0x00_U16 Unk_0x00 => unk_0x00;
    public TexFlags0x02_U8 Unk_0x02 => unk_0x02;
    public TexWrapFlags_U8 Wrapflags => wrapFlags;
    public ushort Tpltextureid => tplTextureID;
    public TexFlags0x06_U8 Unk_0x06 => unk_0x06;
    public GMA_GXAnisotropyU8 Anisotropiclevel => anisotropicLevel;
    public uint Zero_0x08 => zero_0x08;
    public TexFlags0x0C_U8 Unk_0x0C => unk_0x0C;
    public TexFlags0x0D_U8 Unk_0x0D => unk_0x0D;
    public ushort Index => index;
    public TexFlags0x10_U32 Unk_0x10 => unk_0x10;
    public byte[] Fifopadding => fifoPadding;

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
        reader.ReadX(ref unk_0x02);
        reader.ReadX(ref wrapFlags);
        reader.ReadX(ref tplTextureID);
        reader.ReadX(ref unk_0x06);
        reader.ReadX(ref anisotropicLevel);
        reader.ReadX(ref zero_0x08);
        reader.ReadX(ref unk_0x0C);
        reader.ReadX(ref unk_0x0D);
        reader.ReadX(ref index);
        reader.ReadX(ref unk_0x10);
        reader.ReadX(ref fifoPadding, kFifoPaddingSize);
        foreach (var @byte in fifoPadding)
            Assert.IsTrue(@byte == 0x00);

        EndAddress = reader.BaseStream.Position;
    }

    public void Serialize(BinaryWriter writer)
    {
        throw new NotImplementedException();
    }
}

[Serializable]
public class Material : IBinarySerializable, IBinaryAddressable, IFile
{
    public const int kTransformArrayLength = 8;
    public const int kFifoPaddingSize = 4;

    [Header("Debugging")]
    [SerializeField] string modelName;
    [SerializeField] string fileName;
    [SerializeField] long startAddress;
    [SerializeField] long endAddress;

    [Header("Contents")]
    // 0x00
    [SerializeField, EnumFlags] MatFlags0x00_U32 unk_flags_0x00;
    [SerializeField] /********/ uint unk_0x04;
    [SerializeField] /********/ uint unk_0x08;
    [SerializeField] /********/ uint unk_0x0C;
    // 0x10
    [SerializeField, EnumFlags] MatFlags0x10_U8 unk_0x10;
    [SerializeField, EnumFlags] MatFlags0x11_U8 unk_0x11; // mostly FF, but can be other things // GxUtils: Used Material Count
    [SerializeField, EnumFlags] MatFlags0x12_U8 unk_0x12; // 0, 1, or 2
    [SerializeField, EnumFlags] MatVertexRenderFlag_U8 vertexRenderFlags;
    [SerializeField, EnumFlags] MatFlags0x14_U16 unk_0x14; // is 0xFF00 if (unk_0x12) is 1 or 2, and not when 3. // 2 bytes one is null?
    [SerializeField] /********/ ushort tex0Index; //////////////////////////
    [SerializeField] /********/ ushort tex1Index; // Can be nulled 0xFFFF //
    [SerializeField] /********/ ushort tex2Index; //////////////////////////
    [SerializeField, EnumFlags] GMA_GXAttrFlagU32 vertexDescriptorFlags; // GC Manual: Vertex and primitive data
    // 0x20
    [SerializeField] /********/ byte[] transformMatrixSpecidicIndices; // GxUtils: Transformation Matrix Specific Indexes Object 1 (8 separate bytes)
    [SerializeField] /********/ uint matDisplayListSize; // Section 1 length, 32 byte aligned
    [SerializeField] /********/ uint tlMatDisplayListSize; // Section 2 length, 32 byte aligned
    // 0x30
    [SerializeField] /********/ Vector3 uvwCoordinates;
    [SerializeField] /********/ uint unk_0x3C;
    // 0x40
    [SerializeField] /********/ uint unk_0x40;
    [SerializeField] /********/ uint unk_0x44;
    [SerializeField] /********/ uint unk_0x48;
    [SerializeField] /********/ uint unk_0x4C;
    // 0x50
    [SerializeField] /********/ uint unk_0x50;
    [SerializeField] /********/ uint unk_0x54;
    [SerializeField] /********/ uint unk_0x58;
    [SerializeField] /********/ byte[] fifoPadding;
    // 0x60
    [SerializeField] GxDisplayListGroup matDisplayList;

    // 0x00
    public MatFlags0x00_U32 Unk_Flags_0x00 => unk_flags_0x00;
    public uint Unk_0x04 => unk_0x04; // color RGBA 32? B2B2B2FF x2094, 
    public uint Unk_0x08 => unk_0x08; // color RGBA 32? 7F7F7FFF x2544,
    public uint Unk_0x0C => unk_0x0C; // color RGBA 32? FFFFFFFF x1120,
    // 0x10
    public MatFlags0x10_U8 Unk_0x10 => unk_0x10; // 0x2A x1497, 
    public MatFlags0x11_U8 Unk_Count => unk_0x11;
    public MatFlags0x12_U8 Unk_0x12 => unk_0x12;
    public MatVertexRenderFlag_U8 Vertexrenderflags => vertexRenderFlags;
    public MatFlags0x14_U16 Unk_0X14 => unk_0x14; // 0xFF00 x3485
    public ushort Tex0Index => tex0Index;
    public ushort Tex1Index => tex1Index;
    public ushort Tex2Index => tex2Index;
    public GMA_GXAttrFlagU32 Vertexdescriptorflags => vertexDescriptorFlags;
    // 0x20
    public byte[] TransformMatrixSpecificIndices => transformMatrixSpecidicIndices;
    public uint Matdisplaylistsize => matDisplayListSize;
    public uint Tlmatdisplaylistsize => tlMatDisplayListSize;
    // 0x30
    public Vector3 Uvwcoordinates => uvwCoordinates;
    public uint Unk_0x3C => unk_0x3C;
    // 0x40
    public uint Unk_0x40 => unk_0x40;
    public uint Unk_0x44 => unk_0x44;
    public uint Unk_0x48 => unk_0x48;
    public uint Unk_0x4C => unk_0x4C;
    // 0x50
    public uint Unk_0x50 => unk_0x50;
    public uint Unk_0x54 => unk_0x54;
    public uint Unk_0x58 => unk_0x58;
    public byte[] Fifopadding => fifoPadding;
    // 0x60


    // Metadata
    public string FileName
    {
        get => fileName;
        set => fileName = value;
    }
    public string ModelName
    {
        get => modelName;
        set => modelName = value;
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

    // temp, bad debugging
    public override string ToString()
    {
        return
            $"GMA Material Log:\n" +
            $"File : {FileName}\n" +
            $"Model: {ModelName}\n" +
            $"Start: {StartAddress.ToString("X8")}\n" +
            $"End  : {EndAddress.ToString("X8")}\n" +
            $"===0x00===\n" +
            $"unk_flags_0x00: {unk_flags_0x00}\n" +
            $"unk_0x04: {unk_0x04}\n" +
            $"unk_0x08: {unk_0x08}\n" +
            $"unk_0x0C: {unk_0x0C}\n" +
            $"===0x10===\n" +
            $"unk_0x10: {unk_0x10}\n" +
            $"unk_Count: {unk_0x11}\n" +
            $"unk_0x12: {unk_0x12}\n" +
            $"vertexRenderFlags: {vertexRenderFlags}\n" +
            $"unk_0x14: {unk_0x14}\n" +
            $"tex0Index: {tex0Index} {tex0Index.ToString("X4")}\n" +
            $"tex1Index: {tex1Index} {tex1Index.ToString("X4")}\n" +
            $"tex2Index: {tex2Index} {tex2Index.ToString("X4")}\n" +
            $"GXAttrFlag: {vertexDescriptorFlags}\n" +
            $"===0x20===\n" +
            $"matDisplayListSize: {matDisplayListSize}\n" +
            $"tlMatDisplayListSize: {tlMatDisplayListSize}\n" +
            $"===0x30===\n" +
            $"uvwCoordinates: {uvwCoordinates}\n" +
            $"unk_0x3C: {unk_0x3C}\n" +
            $"===0x40===\n" +
            $"unk_0x40: {unk_0x40}";
    }

    public void Deserialize(BinaryReader reader)
    {
        StartAddress = reader.BaseStream.Position;

        // 0x00
        reader.ReadX(ref unk_flags_0x00);
        reader.ReadX(ref unk_0x04);
        reader.ReadX(ref unk_0x08);
        reader.ReadX(ref unk_0x0C);
        // 0x10
        reader.ReadX(ref unk_0x10);
        reader.ReadX(ref unk_0x11);
        reader.ReadX(ref unk_0x12);
        reader.ReadX(ref vertexRenderFlags);
        reader.ReadX(ref unk_0x14);
        reader.ReadX(ref tex0Index);
        reader.ReadX(ref tex1Index);
        reader.ReadX(ref tex2Index);
        reader.ReadX(ref vertexDescriptorFlags);
        // 0x20
        reader.ReadX(ref transformMatrixSpecidicIndices, kTransformArrayLength);
        reader.ReadX(ref matDisplayListSize);
        reader.ReadX(ref tlMatDisplayListSize);
        // 0x30
        reader.ReadX(ref uvwCoordinates);
        reader.ReadX(ref unk_0x3C);
        // 0x40+
        reader.ReadX(ref unk_0x40);
        reader.ReadX(ref unk_0x44);
        reader.ReadX(ref unk_0x48);

        // THE FOLLOWING IS USED EXCLUSIVELY BY [*_vtx] AND [*_SKL] FILES
        // Figure out what their type actually is
        reader.ReadX(ref unk_0x4C);
        reader.ReadX(ref unk_0x50);
        reader.ReadX(ref unk_0x54);
        reader.ReadX(ref unk_0x58);
        reader.ReadX(ref fifoPadding, kFifoPaddingSize);

        EndAddress = reader.BaseStream.Position;

        for (int i = 0; i < fifoPadding.Length; i++)
            Assert.IsTrue(fifoPadding[i] == 0x00, $"{FileName} - {ModelName} - {reader.BaseStream.Position.ToString("X8")} - {fifoPadding[i]} - {i}");
    }

    public void Serialize(BinaryWriter writer)
    {
        throw new NotImplementedException();
    }

}

[Serializable]
public struct TransformMatrix3x4 : IBinarySerializable
{
    private Vector3 row0;
    private Vector3 row1;
    private Vector3 row2;
    private Vector3 row3;

    public Vector3 Row0
    {
        get => row0;
        set => row0 = value;
    }

    public Vector3 Row1
    {
        get => row1;
        set => row1 = value;
    }

    public Vector3 Row2
    {
        get => row2;
        set => row2 = value;
    }

    public Vector3 Row3
    {
        get => row3;
        set => row3 = value;
    }

    public Vector4 Col0
    {
        get => new Vector4(row0.x, row1.x, row2.x, row3.x);
        set
        {
            row0.x = value.x;
            row1.x = value.y;
            row2.x = value.z;
            row3.x = value.w;
        }
    }

    public Vector4 Col1
    {
        get => new Vector4(row0.x, row1.x, row2.x, row3.x);
        set
        {
            row0.y = value.x;
            row1.y = value.y;
            row2.y = value.z;
            row3.y = value.w;
        }
    }

    public Vector4 Col2
    {
        get => new Vector4(row0.x, row1.x, row2.x, row3.x);
        set
        {
            row0.z = value.x;
            row1.z = value.y;
            row2.z = value.z;
            row3.z = value.w;
        }
    }

    public void Deserialize(BinaryReader reader)
    {
        reader.ReadX(ref row0);
        reader.ReadX(ref row1);
        reader.ReadX(ref row2);
        reader.ReadX(ref row3);
    }

    public void Serialize(BinaryWriter writer)
    {
        throw new NotImplementedException();
    }
}

[Serializable]
public struct UnknownMaterialStructure : IBinarySerializable
{
    public uint unk_0x00;
    public uint unk_0x04;
    public uint unk_0x08;
    public uint unk_0x0C;
    public uint unk_0x10;
    public uint unk_0x14;
    public ushort unk_0x18;
    public ushort unk_0x1A; // const 0
    public uint unk_0x1C;

    public void Deserialize(BinaryReader reader)
    {
        reader.ReadX(ref unk_0x00);
        reader.ReadX(ref unk_0x04);
        reader.ReadX(ref unk_0x08);
        reader.ReadX(ref unk_0x0C);
        reader.ReadX(ref unk_0x10);
        reader.ReadX(ref unk_0x14);
        reader.ReadX(ref unk_0x18);
        reader.ReadX(ref unk_0x1A);
        reader.ReadX(ref unk_0x1C);
    }

    public void Serialize(BinaryWriter writer)
    {
        throw new NotImplementedException();
    }
}

[Serializable]
public class Mesh : IBinarySerializable
{
    private int materialCount;
    private int translucidMaterialCount;

    private GxVertexAttributeFormat vertexFormat;
    private SubMesh[] matMeshes;
    private SubMesh[] tlMatMeshes;

    public Mesh() { }

    public Mesh(GxVertexAttributeFormat vertexFormat, int materialCount, int translucidMaterialCount)
    {
        this.materialCount = materialCount;
        this.translucidMaterialCount = translucidMaterialCount;
        this.vertexFormat = vertexFormat;

        matMeshes = new SubMesh[this.materialCount];
        for (int i = 0; i < matMeshes.Length; i++)
            matMeshes[i] = new SubMesh(vertexFormat);

        tlMatMeshes = new SubMesh[this.translucidMaterialCount];
        for (int i = 0; i < tlMatMeshes.Length; i++)
            tlMatMeshes[i] = new SubMesh(vertexFormat);
    }

    public void Deserialize(BinaryReader reader)
    {
        // REquires initialization before use
        reader.ReadX(ref matMeshes, matMeshes.Length, false);
        reader.ReadX(ref tlMatMeshes, tlMatMeshes.Length, false);
    }

    public void Serialize(BinaryWriter writer)
    {
        throw new NotImplementedException();
    }
}

[Serializable]
public class SubMesh : IBinarySerializable
{
    private GxVertexAttributeFormat vertexFormat;
    [SerializeField] private GXPrimitive gxPrimitive;
    [SerializeField] private ushort gxVertsCount;
    [SerializeField] private GxDisplayList[] gxDisplayList;

    public SubMesh() { }

    public SubMesh(GxVertexAttributeFormat vertexFormat)
    {
        this.vertexFormat = vertexFormat;
    }

    public void Deserialize(BinaryReader reader)
    {
        reader.ReadX(ref gxPrimitive);
        reader.ReadX(ref gxVertsCount);
    }

    public void Serialize(BinaryWriter writer)
    {
        throw new NotImplementedException();
    }
}

[Serializable]
public class GxVertexAttributeTable
{
    public GxVertexAttributeFormat[] gxVertexAttributeFormats
    { get; } = new GxVertexAttributeFormat[8];

    public GxVertexAttributeTable(params GxVertexAttributeFormat[] formats)
    {
        if (formats.Length > 8)
            throw new ArgumentOutOfRangeException();

        // Update formats
        for (int i = 0; i < formats.Length; i++)
            gxVertexAttributeFormats[i] = formats[i];
        // Clear old refs
        for (int i = formats.Length; i < gxVertexAttributeFormats.Length; i++)
            gxVertexAttributeFormats[i] = null;
    }

    public void SetVtxAttrFmt(GXVtxFmt index, GXAttrType vcd, GXAttr attribute, GXCompCnt_Rev2 nElements, GXCompType format, int nFracBits = 0)
    {
        var value = new GxVertexAttribute(vcd, attribute, nElements, format, nFracBits);
        gxVertexAttributeFormats[(int)index].SetAttr(value);
    }
}

[Serializable]
public struct GxVertexAttribute
{
    [SerializeField] public GXAttrType vcd;
    [SerializeField] public GXAttr attribute;
    [SerializeField] public GXCompCnt_Rev2 nElements;
    [SerializeField] public GXCompType componentFormat;
    [SerializeField] public int nFracBits;

    public GxVertexAttribute(GXAttrType vcd, GXAttr attribute, GXCompCnt_Rev2 nElements, GXCompType format, int nFracBits = 0)
    {
        // Assert that we aren't shifting more bits than we have
        Assert.IsTrue((nFracBits < 8) & (format == GXCompType.GX_S8 | format == GXCompType.GX_U8));
        Assert.IsTrue((nFracBits < 16) & (format == GXCompType.GX_S16 | format == GXCompType.GX_U16));

        this.vcd = vcd;
        this.attribute = attribute;
        this.nElements = nElements;
        this.componentFormat = format;
        this.nFracBits = nFracBits;
    }
}

//What would compromise a column in GX VAT - Vertex Attribute Table
[Serializable]
public class GxVertexAttributeFormat
{
    [SerializeField] public GXAttrType attributeType;

    [SerializeField] public GxVertexAttribute pos;
    [SerializeField] public GxVertexAttribute? nrm;
    [SerializeField] public GxVertexAttribute? nbt;
    [SerializeField] public GxVertexAttribute? clr0;
    [SerializeField] public GxVertexAttribute? clr1;
    [SerializeField] public GxVertexAttribute? tex0;
    [SerializeField] public GxVertexAttribute? tex1;
    [SerializeField] public GxVertexAttribute? tex2;
    [SerializeField] public GxVertexAttribute? tex3;
    [SerializeField] public GxVertexAttribute? tex4;
    [SerializeField] public GxVertexAttribute? tex5;
    [SerializeField] public GxVertexAttribute? tex6;
    [SerializeField] public GxVertexAttribute? tex7;

    public GxVertexAttribute? GetAttr(GXAttr attribute)
    {
        switch (attribute)
        {
            case GXAttr.GX_VA_POS: return pos;
            case GXAttr.GX_VA_NRM: return nrm;
            case GXAttr.GX_VA_NBT: return nbt;
            case GXAttr.GX_VA_CLR0: return clr0;
            case GXAttr.GX_VA_CLR1: return clr1;
            case GXAttr.GX_VA_TEX0: return tex0;
            case GXAttr.GX_VA_TEX1: return tex1;
            case GXAttr.GX_VA_TEX2: return tex2;
            case GXAttr.GX_VA_TEX3: return tex3;
            case GXAttr.GX_VA_TEX4: return tex4;
            case GXAttr.GX_VA_TEX5: return tex5;
            case GXAttr.GX_VA_TEX6: return tex6;
            case GXAttr.GX_VA_TEX7: return tex7;

            default:
                throw new NotImplementedException();
        }
    }

    public void SetAttr(GxVertexAttribute value)
    {
        switch (value.attribute)
        {
            case GXAttr.GX_VA_POS: pos = value; break;
            case GXAttr.GX_VA_NRM: nrm = value; break;
            case GXAttr.GX_VA_NBT: nbt = value; break;
            case GXAttr.GX_VA_CLR0: clr0 = value; break;
            case GXAttr.GX_VA_CLR1: clr1 = value; break;
            case GXAttr.GX_VA_TEX0: tex0 = value; break;
            case GXAttr.GX_VA_TEX1: tex1 = value; break;
            case GXAttr.GX_VA_TEX2: tex2 = value; break;
            case GXAttr.GX_VA_TEX3: tex3 = value; break;
            case GXAttr.GX_VA_TEX4: tex4 = value; break;
            case GXAttr.GX_VA_TEX5: tex5 = value; break;
            case GXAttr.GX_VA_TEX6: tex6 = value; break;
            case GXAttr.GX_VA_TEX7: tex7 = value; break;

            default:
                throw new NotImplementedException();
        }
    }
}

[Serializable]
public struct GxVertex : IBinarySerializable
{
    [SerializeField] public GxVertexAttributeFormat vertAttr;

    // (Raph:) Missing any other data
    [SerializeField] public Vector3 position;
    [SerializeField] public Vector3? normal;
    [SerializeField] public Vector3? binormal;
    [SerializeField] public Vector3? tangent;
    [SerializeField] public Color32? color0;
    [SerializeField] public Color32? color1;
    [SerializeField] public Vector2? tex0;
    [SerializeField] public Vector2? tex1;
    [SerializeField] public Vector2? tex2;
    [SerializeField] public Vector2? tex3;
    [SerializeField] public Vector2? tex4;
    [SerializeField] public Vector2? tex5;
    [SerializeField] public Vector2? tex6;
    [SerializeField] public Vector2? tex7;

    public void Deserialize(BinaryReader reader)
    {
        // POSITION
        position = GxUtility.ReadVectorXYZ(reader, vertAttr.pos);

        // NORMALS
        if (vertAttr.nrm != null)
        {
            normal = GxUtility.ReadVectorXYZ(reader, (GxVertexAttribute)vertAttr.nrm);
        }
        else if (vertAttr.nbt != null)
        {
            // This code is untested...
            // And it lacks another case for NBT3
            throw new NotImplementedException();

            normal = GxUtility.ReadVectorXYZ(reader, (GxVertexAttribute)vertAttr.nbt);
            binormal = GxUtility.ReadVectorXYZ(reader, (GxVertexAttribute)vertAttr.nbt);
            tangent = GxUtility.ReadVectorXYZ(reader, (GxVertexAttribute)vertAttr.nbt);
        }

        // COLOR
        if (vertAttr.clr0 != null)
            color0 = GxUtility.ReadGXColor(reader, (GxVertexAttribute)vertAttr.clr0);
        if (vertAttr.clr1 != null)
            color1 = GxUtility.ReadGXColor(reader, (GxVertexAttribute)vertAttr.clr1);

        // TEX
        if (vertAttr.tex0 != null)
            tex0 = GxUtility.ReadGxTextureST(reader, (GxVertexAttribute)vertAttr.tex0);
        if (vertAttr.tex1 != null)
            tex0 = GxUtility.ReadGxTextureST(reader, (GxVertexAttribute)vertAttr.tex1);
        if (vertAttr.tex2 != null)
            tex0 = GxUtility.ReadGxTextureST(reader, (GxVertexAttribute)vertAttr.tex2);
        if (vertAttr.tex3 != null)
            tex0 = GxUtility.ReadGxTextureST(reader, (GxVertexAttribute)vertAttr.tex3);
        if (vertAttr.tex4 != null)
            tex0 = GxUtility.ReadGxTextureST(reader, (GxVertexAttribute)vertAttr.tex4);
        if (vertAttr.tex5 != null)
            tex0 = GxUtility.ReadGxTextureST(reader, (GxVertexAttribute)vertAttr.tex5);
        if (vertAttr.tex6 != null)
            tex0 = GxUtility.ReadGxTextureST(reader, (GxVertexAttribute)vertAttr.tex6);
        if (vertAttr.tex7 != null)
            tex0 = GxUtility.ReadGxTextureST(reader, (GxVertexAttribute)vertAttr.tex7);
    }

    public void Serialize(BinaryWriter writer)
    {
        throw new NotImplementedException();
    }
}

[Serializable]
public struct GxDisplayCommand : IBinarySerializable
{
    [SerializeField] public GXPrimitive primitive;
    [SerializeField] public GXVtxFmt vertexFormat;
    public ushort command;

    public void Deserialize(BinaryReader reader)
    {
        reader.ReadX(ref command);
        primitive = (GXPrimitive)(command & 0b_1111_1000); // 5 highest bits
        vertexFormat = (GXVtxFmt)(command & 0b_0000_0111); // 3 lowest bits
    }

    public void Serialize(BinaryWriter writer)
    {
        throw new NotImplementedException();
    }
}

[Serializable]
public class GxDisplayList : IBinarySerializable
{
    // property or...?
    [SerializeField] public GxDisplayCommand gxCommand;
    [SerializeField] public ushort vertCount;
    [SerializeField] public GxVertex[] verts;
    [SerializeField] public GxVertexAttributeTable vat;

    public void Deserialize(BinaryReader reader)
    {
        reader.ReadX(ref gxCommand, true);
        reader.ReadX(ref vertCount);

        // Init vertex VAT references
        for (int i = 0; i < vertCount; i++)
        {
            var vertexFormat = gxCommand.vertexFormat;
            var vatIndex = (int)vertexFormat;
            var vertex = new GxVertex()
            {
                vertAttr = vat.gxVertexAttributeFormats[vatIndex],
            };
        }

        reader.ReadX(ref verts, vertCount, true);
    }

    public void Serialize(BinaryWriter writer)
    {
        throw new NotImplementedException();
    }
}

[Serializable]
public sealed class GxDisplayListGroup : IBinarySerializable
{
    [SerializeField] GxDisplayList[] command;
    [SerializeField] uint gxBufferSize;
    [SerializeField] GxVertexAttributeTable vat;

    public GxDisplayListGroup() { }

    public GxDisplayListGroup(GxVertexAttributeTable vat, uint gxBufferSize)
    {
        this.vat = vat;
        this.gxBufferSize = gxBufferSize;
    }

    public void Deserialize(BinaryReader reader)
    {
        // RAPH: this could be better and cleaner if I made
        // utility scripts like Jasper/noclip.website to
        // calculate the components' sizes.

        // Temp list to store commands for this list
        var command = new List<GxDisplayList>();

        while (true)
        {
            var absPtr = reader.BaseStream.Position;
            var gxCommand = new GxDisplayCommand();
            reader.ReadX(ref gxCommand, true);
            reader.BaseStream.Seek(absPtr, SeekOrigin.Begin);

            if (gxCommand.command != 0)
            {
                // Read command into list
                var displayList = new GxDisplayList();
                displayList.vat = vat;
                reader.ReadX(ref displayList, false);
                command.Add(displayList);
            }
            // Break when we are reading GX_NOP (0)
            else break;
        }

        this.command = command.ToArray();
    }

    public void Serialize(BinaryWriter writer)
    {
        throw new NotImplementedException();
    }
}
#endregion


public static class GxUtility
{
    public const long GX_FIFO_ALIGN = 32;

    public static Vector3 ReadVectorXYZ(BinaryReader reader, GxVertexAttribute vertexAttributes)
    {
        if (vertexAttributes.nElements == GXCompCnt_Rev2.GX_POS_XY)
        {
            return new Vector2(
                ReadVertexFloat(reader, vertexAttributes),
                ReadVertexFloat(reader, vertexAttributes));
        }
        else if (vertexAttributes.nElements == GXCompCnt_Rev2.GX_NRM_XYZ
              || vertexAttributes.nElements == GXCompCnt_Rev2.GX_POS_XYZ)
        {
            return new Vector3(
                ReadVertexFloat(reader, vertexAttributes),
                ReadVertexFloat(reader, vertexAttributes),
                ReadVertexFloat(reader, vertexAttributes));
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    public static Vector2 ReadGxTextureST(BinaryReader reader, GxVertexAttribute vertexAttributes)
    {
        if (vertexAttributes.nElements == GXCompCnt_Rev2.GX_TEX_S)
        {
            return new Vector2(ReadVertexFloat(reader, vertexAttributes), 0f);
        }
        else if (vertexAttributes.nElements == GXCompCnt_Rev2.GX_TEX_ST)
        {
            return new Vector2(
                ReadVertexFloat(reader, vertexAttributes),
                ReadVertexFloat(reader, vertexAttributes));
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    public static Color32 ReadGXColor(BinaryReader reader, GxVertexAttribute vertexAttributes)
    {
        switch (vertexAttributes.attribute)
        {
            case GXAttr.GX_VA_CLR0:
            case GXAttr.GX_VA_CLR1:
                return ReadColorComponent(reader, vertexAttributes.componentFormat);

            default:
                throw new NotImplementedException();
        }
    }

    public static float ReadVertexFloat(BinaryReader reader, GxVertexAttribute vertexAttributes)
    {
        switch (vertexAttributes.attribute)
        {
            case GXAttr.GX_VA_POS:
            case GXAttr.GX_VA_NRM:
            case GXAttr.GX_VA_NBT:
            case GXAttr.GX_VA_TEX0:
            case GXAttr.GX_VA_TEX1:
            case GXAttr.GX_VA_TEX2:
            case GXAttr.GX_VA_TEX3:
            case GXAttr.GX_VA_TEX4:
            case GXAttr.GX_VA_TEX5:
            case GXAttr.GX_VA_TEX6:
            case GXAttr.GX_VA_TEX7:
                return ReadNumericComponent(reader, vertexAttributes.componentFormat, vertexAttributes.nFracBits);

            default:
                throw new NotImplementedException();
        }
    }

    public static Color32 ReadColorComponent(BinaryReader reader, GXCompType type)
    {
        switch (type)
        {
            case GXCompType.GX_RGB565:
                {
                    var rgb565 = BinaryIoUtility.ReadUInt16(reader);
                    var r = (byte)(((rgb565 >> 11) & (0b_0001_1111)) * (1 << 3));
                    var g = (byte)(((rgb565 >> 05) & (0b_0011_1111)) * (1 << 2));
                    var b = (byte)(((rgb565 >> 00) & (0b_0001_1111)) * (1 << 3));
                    return new Color32(r, g, b, byte.MaxValue);
                }

            case GXCompType.GX_RGB8:
                {
                    var r = BinaryIoUtility.ReadUInt8(reader);
                    var g = BinaryIoUtility.ReadUInt8(reader);
                    var b = BinaryIoUtility.ReadUInt8(reader);
                    return new Color32(r, g, b, byte.MaxValue);
                }

            case GXCompType.GX_RGBA4:
                {
                    var rgba4 = BinaryIoUtility.ReadUInt16(reader);
                    var r = (byte)(((rgba4 >> 12) & (0b_0000_1111)) * (1 << 4));
                    var g = (byte)(((rgba4 >> 08) & (0b_0000_1111)) * (1 << 4));
                    var b = (byte)(((rgba4 >> 04) & (0b_0000_1111)) * (1 << 4));
                    var a = (byte)(((rgba4 >> 00) & (0b_0000_1111)) * (1 << 4));
                    return new Color32(r, g, b, a);
                }

            case GXCompType.GX_RGBA6:
                {
                    var upper16 = BinaryIoUtility.ReadUInt16(reader);
                    var lower8 = BinaryIoUtility.ReadUInt8(reader);
                    var rgba6 = (upper16 << 8) | (lower8);

                    var r = (byte)(((rgba6 >> 16) & (0b_0011_1111)) * (1 << 2));
                    var g = (byte)(((rgba6 >> 12) & (0b_0011_1111)) * (1 << 2));
                    var b = (byte)(((rgba6 >> 06) & (0b_0011_1111)) * (1 << 2));
                    var a = (byte)(((rgba6 >> 00) & (0b_0011_1111)) * (1 << 2));
                    return new Color32(r, g, b, a);
                }

            case GXCompType.GX_RGBA8:
                {
                    var r = BinaryIoUtility.ReadUInt8(reader);
                    var g = BinaryIoUtility.ReadUInt8(reader);
                    var b = BinaryIoUtility.ReadUInt8(reader);
                    var a = BinaryIoUtility.ReadUInt8(reader);
                    return new Color32(r, g, b, a);
                }

            case GXCompType.GX_RGBX8:
                {
                    // review this code. Is X discarded?
                    throw new NotImplementedException();

                    var r = BinaryIoUtility.ReadUInt8(reader);
                    var g = BinaryIoUtility.ReadUInt8(reader);
                    var b = BinaryIoUtility.ReadUInt8(reader);
                    var x = BinaryIoUtility.ReadUInt8(reader); // discarded
                    return new Color32(r, g, b, byte.MaxValue);
                }

            default:
                throw new NotImplementedException();
        }
    }

    public static float ReadNumericComponent(BinaryReader reader, GXCompType type, int nFracBits)
    {
        switch (type)
        {
            case GXCompType.GX_F32:
                return BinaryIoUtility.ReadFloat(reader);

            case GXCompType.GX_S8:
                return FixedToFloat(BinaryIoUtility.ReadInt8(reader), nFracBits);

            case GXCompType.GX_U8:
                return FixedToFloat(BinaryIoUtility.ReadUInt8(reader), nFracBits);

            case GXCompType.GX_S16:
                return FixedToFloat(BinaryIoUtility.ReadInt16(reader), nFracBits);

            case GXCompType.GX_U16:
                return FixedToFloat(BinaryIoUtility.ReadUInt16(reader), nFracBits);

            default:
                throw new NotImplementedException();
        }
    }

    public static float FixedToFloat(float value, int nFracBits)
    {
        value = (float)Math.Pow(2, value);
        var divisor = (float)Math.Pow(2, nFracBits);
        value = value / divisor;
        return value;
    }

    //public static sbyte Float32ToS8(GXCompType componentType, int nFractionBits, float value)
    //{
    //    throw new NotImplementedException();
    //}
}

// TODO: move to own file
public interface IBinaryAddressable
{
    long StartAddress { get; set; }
    long EndAddress { get; set; }
}