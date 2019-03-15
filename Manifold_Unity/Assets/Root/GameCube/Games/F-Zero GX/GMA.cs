using GameCube.GX;
using StarkTools.IO;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;


#region DEFINITIONS

public class GxDisplayList : IBinarySerializable
{
    public GXPrimitive primitive;
    public GXVtxFmt vertexFormat;
    private ushort gxCommand; 
    public ushort vertCount;
    public GxVert[] verts;

    public void Deserialize(BinaryReader reader)
    {
        reader.ReadX(ref gxCommand);
        reader.ReadX(ref vertCount);
        primitive = (GXPrimitive)(gxCommand & 0b_1111_1000); // 5 highest bits
        vertexFormat = (GXVtxFmt)(gxCommand & 0b_0000_0111); // 3 lowest bits

        reader.ReadX(ref verts, vertCount, true);
    }

    public void Serialize(BinaryWriter writer)
    {
        throw new NotImplementedException();
    }
}

[Flags]
public enum GMA_WrapFlagsU8 : byte
{
    unk0 = 1 << 0, // Unused AFAIK
    unk1 = 1 << 1, // Unused AFAIK
    repeatX = 1 << 2,
    mirrorX = 1 << 3,
    repeatY = 1 << 4,
    mirrorY = 1 << 5,
    unk6 = 1 << 6, // Double check, was named 'isRegular1'
    unk7 = 1 << 7, // Double check, was named 'isRegular2'
}

[Flags]
public enum GMA_UvFlagsU8 : byte
{
    unk1 = 1 << 0,
    unk2 = 1 << 1,
    unk3 = 1 << 2,
    unk4 = 1 << 3,
    unk5 = 1 << 4,
    unk6 = 1 << 5,
    unk7 = 1 << 6,
    unk8 = 1 << 7,
}

[Flags]
public enum GMA_VertexRenderFlagU8 : byte
{
    renderMaterials = 1 << 0,
    renderTranslucidMaterials = 1 << 1,
}

/// <summary>
/// Code from https://github.com/bobjrsenior/GxUtils/blob/master/GxUtils/LibGxFormat/Gma/Gcmf.cs
/// </summary>
[Flags]
enum GcmfSectionFlagsU32 : UInt32
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
public class GMA : IBinarySerializable, INamedFile
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

            // If 1st pointer is null, skip
            if (gcmfRelativePtr == kNullPtr)
                continue;

            var endPosition = reader.BaseStream.Position;

            gcmf[i] = new GCMF();
            var gcmfPtr = gcmfRelativePtr + HeaderEndPosition;
            reader.BaseStream.Seek(gcmfPtr, SeekOrigin.Begin);
            reader.ReadX(ref gcmf[i], true);

            var fileName = string.Empty;
            var modelNamePtr = nameRelativePtr + GcmfNamePtr;
            reader.BaseStream.Seek(modelNamePtr, SeekOrigin.Begin);
            reader.ReadXCString(ref fileName, Encoding.ASCII);
            gcmf[i].FileName = fileName;

            reader.BaseStream.Seek(endPosition, SeekOrigin.Begin);
        }
    }

    public void Serialize(BinaryWriter writer)
    {
        throw new NotImplementedException();
    }

    #endregion
}

// Model and GMA should be combined
//[Serializable]
//public class Model : IBinarySerializable
//{
//    public const uint kNullPtr = 0xFFFFFFFF;

//    private GMA gma;

//    private uint gcmfRelativePtr;
//    private uint nameRelativePtr;
//    [SerializeField] private string name;
//    [SerializeField] private GCMF gcmf;

//    public void Deserialize(BinaryReader reader)
//    {
//        //Debug.Log(debugIterationIndex++);

//        reader.ReadX(ref gcmfRelativePtr);
//        reader.ReadX(ref nameRelativePtr);

//        if (gcmfRelativePtr == kNullPtr)
//            return;

//        var endPosition = reader.BaseStream.Position;

//        var gcmfPtr = gcmfRelativePtr + gma.HeaderEndPosition;
//        reader.BaseStream.Seek(gcmfPtr, SeekOrigin.Begin);
//        reader.ReadX(ref gcmf, true);

//        var modelNamePtr = nameRelativePtr + gma.ModelNamePtr;
//        reader.BaseStream.Seek(modelNamePtr, SeekOrigin.Begin);
//        reader.ReadXCString(ref name, Encoding.ASCII);

//        reader.BaseStream.Seek(endPosition, SeekOrigin.Begin);
//    }

//    public void Serialize(BinaryWriter writer)
//    {
//        throw new NotImplementedException();
//    }

//    public void SetGMA(GMA gma)
//    {
//        this.gma = gma;
//    }
//}

[Serializable]
public class GCMF : IBinarySerializable, INamedFile
{
    public const uint kGCMF = 0x47434D46; // 47 43 4D 46 - GCMF in ASCII
    public const int kTransformMatrixDefaultLength = 8;

    public string fileName;

    private uint gcmfMagic;
    [SerializeField] private uint unk0x04; // GxUtils: Section flags (16Bit: 0x01; Stitching Model: 0x04; Skin Model: 0x08; Effective Model: 0x10)
    [SerializeField] private Vector3 origin;
    [SerializeField] private float radius;
    [SerializeField] private ushort textureCount;
    [SerializeField] private ushort materialCount; // nb mat
    [SerializeField] private ushort translucidMaterialCount; // nb tl mat
    [SerializeField] private byte transformMatrixCount; // Matrix Num
    [SerializeField] private byte const0x00;
    [SerializeField] private uint indicesRelPtr; // GxUtils: Header Size
    [SerializeField] private uint unk0x24; // GxUtils: const 0x0000_0000
    [SerializeField] private byte[] transformMatrixDefaultIndices;
    [SerializeField] private uint unk0x30; // padding FIFO 32
    [SerializeField] private uint unk0x34; // padding FIFO 32
    [SerializeField] private uint unk0x38; // padding FIFO 32
    [SerializeField] private uint unk0x3C; // padding FIFO 32
    [SerializeField] private Texture[] texture;
    [SerializeField] private Material material;
    // GxUtils: Some transform thing here
    [SerializeField] private Mesh mesh;

    public uint Unk0x04 => unk0x04;

    public string FileName
    { 
        get => fileName;
        set => fileName = value;
    }

    public void Deserialize(BinaryReader reader)
    {
        reader.ReadX(ref gcmfMagic); Assert.IsTrue(gcmfMagic == kGCMF);
        reader.ReadX(ref unk0x04);
        reader.ReadX(ref origin);
        reader.ReadX(ref radius);
        reader.ReadX(ref textureCount);
        reader.ReadX(ref materialCount);
        reader.ReadX(ref translucidMaterialCount);
        reader.ReadX(ref transformMatrixCount);
        reader.ReadX(ref const0x00);
        reader.ReadX(ref indicesRelPtr);
        reader.ReadX(ref unk0x24);
        reader.ReadX(ref transformMatrixDefaultIndices, kTransformMatrixDefaultLength);
        reader.ReadX(ref unk0x30);
        reader.ReadX(ref unk0x34);
        reader.ReadX(ref unk0x38);
        reader.ReadX(ref unk0x3C);
        reader.ReadX(ref texture, textureCount, true);
        reader.ReadX(ref material, true);

        // TODO DisplayLists

        if (unk0x04 != 0) Debug.Log($"unk0x04 is not 0. ({unk0x04.ToString("X")})"); // 1
        if (unk0x24 != 0) Debug.Log($"unk0x24 is not 0. ({unk0x24.ToString("X")})");
        if (unk0x30 != 0) Debug.Log($"unk0x30 is not 0. ({unk0x30.ToString("X")})");
        if (unk0x34 != 0) Debug.Log($"unk0x34 is not 0. ({unk0x34.ToString("X")})");
        if (unk0x38 != 0) Debug.Log($"unk0x38 is not 0. ({unk0x38.ToString("X")})");
        if (unk0x3C != 0) Debug.Log($"unk0x3C is not 0. ({unk0x3C.ToString("X")})");
    }

    public void Serialize(BinaryWriter writer)
    {
        throw new NotImplementedException();
    }
}

[Serializable]
public class Texture : IBinarySerializable
{
    public const int kFifoPaddingSize = 12;

    [SerializeField] private ushort unkFlag;
    [SerializeField, EnumFlags] private GMA_UvFlagsU8 uvFlags;
    [SerializeField, EnumFlags] private GMA_WrapFlagsU8 wrapFlags;
    [SerializeField] private ushort tplTextureID;
    [SerializeField] private byte unk_0x06;
    [SerializeField] private GMA_GXAnisotropyU8 anisotropicLevel;
    [SerializeField] private uint unk_0x08;
    [SerializeField] private byte unk_flags_0x0C;
    [SerializeField] private byte unk_0x0D;
    [SerializeField] private ushort index;
    [SerializeField] private uint unk_0x10; // Could be flags, 0x30 is 0b_0110_0000
    [SerializeField] private byte[] fifoPadding;


    public void Deserialize(BinaryReader reader)
    {
        reader.ReadX(ref unkFlag);
        reader.ReadX(ref uvFlags);
        reader.ReadX(ref wrapFlags);
        reader.ReadX(ref tplTextureID);
        reader.ReadX(ref unk_0x06);
        reader.ReadX(ref anisotropicLevel);
        reader.ReadX(ref unk_0x08);
        reader.ReadX(ref unk_flags_0x0C);
        reader.ReadX(ref unk_0x0D);
        reader.ReadX(ref index);
        reader.ReadX(ref unk_0x10);
        reader.ReadX(ref fifoPadding, kFifoPaddingSize);
        foreach (var @byte in fifoPadding)
            Assert.IsTrue(@byte == 0x00);

        if (unk_0x08 != 0) Debug.Log($"unk_0x08 is not 0. ({unk_0x08.ToString("X")}) ({Convert.ToString(unk_0x08, 2)})");
        if (unk_flags_0x0C != 0) Debug.Log($"unk_flags_0x0C is not 0. ({unk_flags_0x0C.ToString("X")}) ({Convert.ToString(unk_flags_0x0C, 2)})"); // 1, 5, 11.x6, 14, 2B, 2F, B2.x2, 54, 55, 30.x35, 3A, 3C, F6, FF.x2
        if (unk_0x0D != 0) Debug.Log($"unk_0x0D is not 0. ({unk_0x0D.ToString("X")}) ({Convert.ToString(unk_0x0D, 2)})");
        if (unk_0x10 != 0) Debug.Log($"unk_0x10 is not 0. ({unk_0x10.ToString("X")}) ({Convert.ToString(unk_0x10, 2)})");
    }

    public void Serialize(BinaryWriter writer)
    {
        throw new NotImplementedException();
    }
}

[Serializable]
public class Material : IBinarySerializable
{
    public const int kTransformArrayLength = 8;
    public const int kFifoPaddingSize = 28;

    // 0x00
    [SerializeField] private uint unk_flags_0x00;
    [SerializeField] private uint unk_0x04;
    [SerializeField] private uint unk_0x08;
    [SerializeField] private uint unk_0x0C;
    // 0x10
    [SerializeField] private byte unk_0x10;
    [SerializeField] private byte unk_Count; // mostly FF, but can be other things // GxUtils: Used Material Count
    [SerializeField] private byte unk_0x12; // 0, 1, or 2
    [SerializeField, EnumFlags] private GMA_VertexRenderFlagU8 vertexRenderFlags;
    [SerializeField] private ushort unk_0x14; // is 0xFF00 if (unk_0x12) is 1 or 2, and not when 3. // 2 bytes one is null?
    [SerializeField] private ushort tex0Index; //////////////////////////
    [SerializeField] private ushort tex1Index; // Can be nulled 0xFFFF //
    [SerializeField] private ushort tex2Index; //////////////////////////
    [SerializeField, EnumFlags] private GMA_GXAttrFlagU32 vertexDescriptorFlags; // GC Manual: Vertex and primitive data
    // 0x20
    [SerializeField] private byte[] transformMatrixSpecidicIndices; // GxUtils: Transformation Matrix Specific Indexes Object 1 (8 separate bytes)
    [SerializeField] private uint matDisplayListSize; // Section 1 length, 32 byte aligned
    [SerializeField] private uint tlMatDisplayListSize; // Section 2 length, 32 byte aligned
    // 0x30
    [SerializeField] private Vector3 uvwCoordinates;
    [SerializeField] private uint unk_0x3C;
    // 0x40
    [SerializeField] private uint unk_0x40;
    [SerializeField] private byte[] fifoPadding;

    public GMA_GXAttrFlagU32 VertexDescriptorFlags => vertexDescriptorFlags;

    // temp, bad debugging
    public override string ToString()
    {
        return
            $"unk_flags_0x00: {unk_flags_0x00}\n" +
            $"unk_0x04: {unk_0x04}\n" +
            $"unk_0x08: {unk_0x08}\n" +
            $"unk_0x0C: {unk_0x0C}\n" +
            $"unk_0x10: {unk_0x10}\n" +
            $"unk_Count: {unk_Count}\n" +
            $"unk_0x12: {unk_0x12}\n" +
            $"vertexRenderFlags: {vertexRenderFlags}\n" +
            $"unk_0x14: {unk_0x14}\n" +
            $"unk_0x16: {tex0Index}\n" +
            $"unk_0x18: {tex1Index}\n" +
            $"unk_0x1A: {tex2Index}\n" +
            $"GXAttrFlag: {vertexDescriptorFlags}\n" +
            $"vertexCount1: {matDisplayListSize}\n" +
            $"vertexCount2: {tlMatDisplayListSize}\n" +
            $"uvwCoordinates: {uvwCoordinates}\n" +
            $"unk_0x3C: {unk_0x3C}\n" +
            $"unk_0x40: {unk_0x40}";
    }

    public void Deserialize(BinaryReader reader)
    {
        // 0x00
        reader.ReadX(ref unk_flags_0x00);
        reader.ReadX(ref unk_0x04);
        reader.ReadX(ref unk_0x08);
        reader.ReadX(ref unk_0x0C);
        // 0x10
        reader.ReadX(ref unk_0x10);
        reader.ReadX(ref unk_Count);
        reader.ReadX(ref unk_0x12);
        reader.ReadX(ref vertexRenderFlags);
        reader.ReadX(ref unk_0x14);
        //
        //if (unk_0x14 == 0xFF00)
        //    Assert.IsTrue(unk_0x12 == 1 || unk_0x12 == 2);
        //
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
        reader.ReadX(ref fifoPadding, kFifoPaddingSize);
        //foreach (var @byte in fifoPadding)
        //    Assert.IsTrue(@byte == 0x00);


        Debug.LogWarning(this.ToString());

        if (unk_0x04 != 0) Debug.Log($"unk_0x04 is not 0. ({unk_0x04.ToString("X")})");
        if (unk_0x08 != 0) Debug.Log($"unk_0x08 is not 0. ({unk_0x08.ToString("X")})");
        if (unk_0x0C != 0) Debug.Log($"unk_0x0C is not 0. ({unk_0x0C.ToString("X")})");
        if (unk_0x10 != 0) Debug.Log($"unk_0x10 is not 0. ({unk_0x10.ToString("X")})");
        if (unk_0x14 != 0) Debug.Log($"unk_0x14 is not 0. ({unk_0x14.ToString("X")})");
        if (unk_0x3C != 0) Debug.Log($"unk_0x3C is not 0. ({unk_0x3C.ToString("X")})");
        if (unk_0x40 != 0) Debug.Log($"unk_0x40 is not 0. ({unk_0x40.ToString("X")})");
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
        reader.ReadX(ref tlMatMeshes,tlMatMeshes.Length, false);
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
    { get; set; } = new GxVertexAttributeFormat[8];

    public void SetVtxAttrFmt(GXVtxFmt index, GXAttrType vcd, GXAttr attribute, GXCompCnt_Rev2 nElements, GXCompType format, int nFracBits = 0)
    {
        var value = new GxVertexAttribute(vcd, attribute, nElements, format, nFracBits);
        gxVertexAttributeFormats[(int)index].SetAttr(value);
    }
}

[Serializable]
public struct GxVertexAttribute
{
    public GXAttrType vcd;
    public GXAttr attribute;
    public GXCompCnt_Rev2 nElements;
    public GXCompType componentFormat;
    public int nFracBits;

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
    public GXAttrType attributeType;

    public GxVertexAttribute pos;
    public GxVertexAttribute? nrm;
    public GxVertexAttribute? nbt;
    public GxVertexAttribute? clr0;
    public GxVertexAttribute? clr1;
    public GxVertexAttribute? tex0;
    public GxVertexAttribute? tex1;
    public GxVertexAttribute? tex2;
    public GxVertexAttribute? tex3;
    public GxVertexAttribute? tex4;
    public GxVertexAttribute? tex5;
    public GxVertexAttribute? tex6;
    public GxVertexAttribute? tex7;

    public GxVertexAttribute? GetAttr(GXAttr attribute)
    {
        switch (attribute)
        {
            case GXAttr.GX_VA_POS:  return pos;
            case GXAttr.GX_VA_NRM:  return nrm;
            case GXAttr.GX_VA_NBT:  return nbt;
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
public struct GxVert : IBinarySerializable
{
    public GxVertexAttributeFormat vertAttr;

    // (Raph:) Missing any other data
    public Vector3 position;
    public Vector3? normal;
    public Vector3? binormal;
    public Vector3? tangent;
    public Color32? color0;
    public Color32? color1;
    public Vector2? tex0;
    public Vector2? tex1;
    public Vector2? tex2;
    public Vector2? tex3;
    public Vector2? tex4;
    public Vector2? tex5;
    public Vector2? tex6;
    public Vector2? tex7;

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

#endregion


public static class GxUtility
{
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