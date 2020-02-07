using StarkTools.IO;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace GameCube.GX
{
    /// <summary>
    /// Primitive type.
    /// </summary>
    public enum GXPrimitive : byte
    {
        GX_QUADS                    = 0x80, // 0b10000000
        GX_TRIANGLES                = 0x90, // 0b10010000
        GX_TRIANGLESTRIP            = 0x98, // 0b10011000
        GX_TRIANGLEFAN              = 0xA0, // 0b10100000
        GX_LINES                    = 0xA8, // 0b10101000
        GX_LINESTRIP                = 0xB0, // 0b10110000
        GX_POINTS                   = 0xB8, // 0b10111000
    }

    /// <summary>
    /// Vertex format number.
    /// </summary>
    public enum GXVtxFmt
    {
        GX_VTXFMT0 = 0,
        GX_VTXFMT1,
        GX_VTXFMT2,
        GX_VTXFMT3,
        GX_VTXFMT4,
        GX_VTXFMT5,
        GX_VTXFMT6,
        GX_VTXFMT7,
        GX_MAX_VTXFMT,
    }

    /// <summary>
    /// GX Component Type
    /// Related to GXVtxFmt
    /// </summary>
    public enum GXCompType
    {
        /// <summary>
        /// Unsigned 8-bit integer
        /// </summary>
        GX_U8 = 0,
        /// <summary>
        /// Signed 8-bit integer
        /// </summary>
        GX_S8,
        /// <summary>
        /// Unsigned 16-bit integer
        /// </summary>
        GX_U16,
        /// <summary>
        /// Signed 16-bit integer
        /// </summary>
        GX_S16,
        /// <summary>
        /// 32-bit floating-point
        /// </summary>
        GX_F32,

        /// <summary>
        /// 16-bit RGB
        /// </summary>
        GX_RGB565 = 0,
        /// <summary>
        /// 24-bit RGB
        /// </summary>
        GX_RGB8,
        /// <summary>
        /// 32-bit RGBX
        /// </summary>
        GX_RGBX8,
        /// <summary>
        /// 16-bit RGBA
        /// </summary>
        GX_RGBA4,
        /// <summary>
        /// 24-bit RGBA
        /// </summary>
        GX_RGBA6,
        /// <summary>
        /// 32-bit RGBA
        /// </summary>
        GX_RGBA8,
    }

    /// <summary>
    /// GX Component Count
    /// </summary>
    public enum GXCompCnt_Rev2
    {
        /// <summary>
        /// X,Y position
        /// </summary>
        GX_POS_XY = 0,
        /// <summary>
        /// X,Y,Z position
        /// </summary>
        GX_POS_XYZ = 1,


        /// <summary>
        /// X,Y,Z normal
        /// </summary>
        GX_NRM_XYZ = 0,
        /// <summary>
        /// Normal, Binormal, Tangent
        /// one index per NBT
        /// </summary>
        GX_NRM_NBT = 1,
        /// <summary>
        /// Normal, Binormal, Tangent x3 (HW2 only)
        /// one index per each of N/B/T
        /// </summary>
        GX_NRM_NBT3 = 2,

        /// <summary>
        /// RGB color
        /// </summary>
        GX_CLR_RGB = 0,
        /// <summary>
        /// RGBA color
        /// </summary>
        GX_CLR_RGBA = 1,

        /// <summary>
        /// One texture dimension
        /// </summary>
        GX_TEX_S = 0,
        /// <summary>
        /// Two texture dimensions
        /// </summary>
        GX_TEX_ST = 1,
    }

    /// <summary>
    /// Name of vertex attribute or array. Attributes are listed in the ascending order vertex data is required to be sent to the GP.
    /// 
    /// Notes:
    /// Tells GX what to expect from oncoming vertex information.
    /// The data provided should be 32-byte aligned. Refer to GX FIFO.
    /// 
    /// There appears to be conflict between this and some information in the 
    /// "Vertex and primitive data" Nintendo SDK manual. The manual says
    /// GX_VA_NRM and GX_VA_NBT both share a value of 10, but that's not what
    /// the enum here had, and I recall copying from the SDK enum script.
    /// </summary>
    public enum GXAttr
    {
        /// <summary>
        /// position/normal matrix index
        /// </summary>
        GX_VA_PNMTXIDX = 0,
        /// <summary>
        /// texture 0 matrix index
        /// </summary>
        GX_VA_TEX0MTXIDX,
        /// <summary>
        /// texture 1 matrix index
        /// </summary>
        GX_VA_TEX1MTXIDX,
        /// <summary>
        /// texture 2 matrix index
        /// </summary>
        GX_VA_TEX2MTXIDX,
        /// <summary>
        /// texture 3 matrix index
        /// </summary>
        GX_VA_TEX3MTXIDX,
        /// <summary>
        /// texture 4 matrix index
        /// </summary>
        GX_VA_TEX4MTXIDX,
        /// <summary>
        /// texture 5 matrix index
        /// </summary>
        GX_VA_TEX5MTXIDX,
        /// <summary>
        /// texture 6 matrix index
        /// </summary>
        GX_VA_TEX6MTXIDX,
        /// <summary>
        /// texture 7 matrix index
        /// </summary>
        GX_VA_TEX7MTXIDX,

        /// <summary>
        /// position
        /// </summary>
        GX_VA_POS,
        /// <summary>
        /// normal
        /// </summary>
        GX_VA_NRM,

        /// <summary>
        /// color 0
        /// </summary>
        GX_VA_CLR0,
        /// <summary>
        /// color 1
        /// </summary>
        GX_VA_CLR1,

        /// <summary>
        /// input texture coordinate 0
        /// </summary>
        GX_VA_TEX0,
        /// <summary>
        /// input texture coordinate 1
        /// </summary>
        GX_VA_TEX1,
        /// <summary>
        /// input texture coordinate 2
        /// </summary>
        GX_VA_TEX2,
        /// <summary>
        /// input texture coordinate 3
        /// </summary>
        GX_VA_TEX3,
        /// <summary>
        /// input texture coordinate 4
        /// </summary>
        GX_VA_TEX4,
        /// <summary>
        /// input texture coordinate 5
        /// </summary>
        GX_VA_TEX5,
        /// <summary>
        /// input texture coordinate 6
        /// </summary>
        GX_VA_TEX6,
        /// <summary>
        /// input texture coordinate 7
        /// </summary>
        GX_VA_TEX7,

        /// <summary>
        /// position matrix array pointer
        /// </summary>
        GX_VA_POS_MTX_ARRAY,
        /// <summary>
        /// normal matrix array pointer
        /// </summary>
        GX_VA_NRM_MTX_ARRAY,
        /// <summary>
        /// texture matrix array pointer
        /// </summary>
        GX_VA_TEX_MTX_ARRAY,
        /// <summary>
        /// light matrix array pointer
        /// </summary>
        GX_VA_LIGHT_ARRAY,
        /// <summary>
        /// normal, bi-normal, tangent 
        /// </summary>
        GX_VA_NBT,
        /// <summary>
        /// maximum number of vertex attributes
        /// </summary>
        GX_VA_MAX_ATTR,

        /// <summary>
        /// NULL attribute (to mark end of lists)
        /// </summary>
        GX_VA_NULL = 0xff,
    }

    /// <summary>
    /// What would compromise a column in GX VAT - Vertex Attribute Table
    /// </summary>
    [Serializable]
    public class GxVertexAttributeFormat
    {
        [SerializeField] public GxVertexAttribute pos;
        [SerializeField] public GxVertexAttribute nrm;
        [SerializeField] public GxVertexAttribute nbt;
        [SerializeField] public GxVertexAttribute clr0;
        [SerializeField] public GxVertexAttribute clr1;
        [SerializeField] public GxVertexAttribute tex0;
        [SerializeField] public GxVertexAttribute tex1;
        [SerializeField] public GxVertexAttribute tex2;
        [SerializeField] public GxVertexAttribute tex3;
        [SerializeField] public GxVertexAttribute tex4;
        [SerializeField] public GxVertexAttribute tex5;
        [SerializeField] public GxVertexAttribute tex6;
        [SerializeField] public GxVertexAttribute tex7;

        public GxVertexAttribute GetAttr(GXAttr attribute)
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

        public void SetAttr(GXAttr attribute, GxVertexAttribute value)
        {
            switch (attribute)
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
    public class GxVertexAttributeTable
    {
        [SerializeField] GxVertexAttributeFormat[] gxVertexAttributeFormats = new GxVertexAttributeFormat[8];

        public GxVertexAttributeFormat[] GxVertexAttributeFormats => gxVertexAttributeFormats;

        public GxVertexAttributeTable(params GxVertexAttributeFormat[] formats)
        {
            if (formats.Length > 8)
                throw new ArgumentOutOfRangeException();

            // Update formats
            for (int i = 0; i < formats.Length; i++)
                GxVertexAttributeFormats[i] = formats[i];
            // Clear old refs
            for (int i = formats.Length; i < GxVertexAttributeFormats.Length; i++)
                GxVertexAttributeFormats[i] = null;
        }

        public void SetVtxAttrFmt(GXVtxFmt index, /*/GXAttrType vcd, GXAttr attribute,/*/ GXCompCnt_Rev2 nElements, GXCompType format, int nFracBits = 0)
        {
            //var value = new GxVertexAttribute(vcd, attribute, nElements, format, nFracBits);
            //GxVertexAttributeFormats[(int)index].SetAttr(value);
        }
    }

    [Serializable]
    public struct GxVertexAttribute
    {
        [SerializeField] public bool enabled;
        //[SerializeField] public GXAttrType vcd;
        //[SerializeField] public GXAttr attribute;
        [SerializeField] public GXCompCnt_Rev2 nElements;
        [SerializeField] public GXCompType componentFormat;
        [SerializeField] public int nFracBits;

        public GxVertexAttribute(/*/GXAttrType vcd, GXAttr attribute,/*/ GXCompCnt_Rev2 nElements, GXCompType format, int nFracBits = 0)
        {
            // Assert that we aren't shifting more bits than we have
            if (format == GXCompType.GX_S8 | format == GXCompType.GX_U8)
                Assert.IsTrue(nFracBits < 8);
            if (format == GXCompType.GX_S16 | format == GXCompType.GX_U16)
                Assert.IsTrue(nFracBits < 16);

            this.enabled = true;
            //this.vcd = vcd;
            //this.attribute = attribute;
            this.nElements = nElements;
            this.componentFormat = format;
            this.nFracBits = nFracBits;
        }
    }

    [Serializable]
    public struct GxVertex : IBinarySerializable
    {
        [SerializeField] public GxVertexAttributeFormat vertAttr;

        // (Raph:) Missing other data
        [SerializeField] public Vector3 position;
        [SerializeField] public Vector3 normal;
        [SerializeField] public Vector3 binormal;
        [SerializeField] public Vector3 tangent;
        [SerializeField] public Color32 color0;
        [SerializeField] public Color32 color1;
        [SerializeField] public Vector2 tex0;
        [SerializeField] public Vector2 tex1;
        [SerializeField] public Vector2 tex2;
        [SerializeField] public Vector2 tex3;
        [SerializeField] public Vector2 tex4;
        [SerializeField] public Vector2 tex5;
        [SerializeField] public Vector2 tex6;
        [SerializeField] public Vector2 tex7;

        public void Deserialize(BinaryReader reader)
        {
            //// POSITION
            //position = GxUtility.ReadGxVectorXYZ(reader, vertAttr.pos);

            //// NORMALS
            //if (vertAttr.nrm.enabled)
            //{
            //    normal = GxUtility.ReadGxVectorXYZ(reader, vertAttr.nrm);
            //}
            //else if (vertAttr.nbt.enabled)
            //{
            //    // This code is untested...
            //    // And it lacks another case for NBT3
            //    throw new NotImplementedException();

            //    normal = GxUtility.ReadGxVectorXYZ(reader, vertAttr.nbt);
            //    binormal = GxUtility.ReadGxVectorXYZ(reader, vertAttr.nbt);
            //    tangent = GxUtility.ReadGxVectorXYZ(reader, vertAttr.nbt);
            //}

            //// COLOR
            //if (vertAttr.clr0.enabled)
            //    color0 = GxUtility.ReadGXColor(reader, vertAttr.clr0);
            //if (vertAttr.clr1.enabled)
            //    color1 = GxUtility.ReadGXColor(reader, vertAttr.clr1);

            //// TEX
            //if (vertAttr.tex0.enabled)
            //    tex0 = GxUtility.ReadGxTextureST(reader, vertAttr.tex0.nElements, );
            //if (vertAttr.tex1.enabled)
            //    tex1 = GxUtility.ReadGxTextureST(reader, vertAttr.tex1);
            //if (vertAttr.tex2.enabled)
            //    tex2 = GxUtility.ReadGxTextureST(reader, vertAttr.tex2);
            //if (vertAttr.tex3.enabled)
            //    tex3 = GxUtility.ReadGxTextureST(reader, vertAttr.tex3);
            //if (vertAttr.tex4.enabled)
            //    tex4 = GxUtility.ReadGxTextureST(reader, vertAttr.tex4);
            //if (vertAttr.tex5.enabled)
            //    tex5 = GxUtility.ReadGxTextureST(reader, vertAttr.tex5);
            //if (vertAttr.tex6.enabled)
            //    tex6 = GxUtility.ReadGxTextureST(reader, vertAttr.tex6);
            //if (vertAttr.tex7.enabled)
            //    tex7 = GxUtility.ReadGxTextureST(reader, vertAttr.tex7);
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
            primitive = (GXPrimitive)(command & 0b_00000000_11111000); // 5 highest bits
            vertexFormat = (GXVtxFmt)(command & 0b_00000000_00000111); // 3 lowest bits
        }

        public void Serialize(BinaryWriter writer)
        {
            //command = 
        }
    }

    [Serializable]
    public class GxDisplayObject : IBinarySerializable, IBinaryAddressable
    {
        [Header("GxDisplayList")]
        [SerializeField, Hex(8)] long startAddress;
        [SerializeField, Hex(8)] long endAddress;

        [Space]
        [SerializeField, LabelPrefix("00 -")] public GxDisplayCommand gxDisplayCommand;
        [SerializeField, Hex("01 -", 2)] public byte vertCount;
        [SerializeField, LabelPrefix("02 -")] public GxVertex[] verts;
        [SerializeField, HideInInspector] public GxVertexAttributeTable vat;

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

        public GxDisplayObject() { }

        public GxDisplayObject(GxVertexAttributeTable vat)
        {
            this.vat = vat;
        }

        public void Deserialize(BinaryReader reader)
        {
            startAddress = reader.BaseStream.Position;

            reader.ReadX(ref gxDisplayCommand, true);
            reader.ReadX(ref vertCount);

            // Init vertex VAT references
            verts = new GxVertex[vertCount];
            for (int i = 0; i < verts.Length; i++)
            {
                var vertexFormat = gxDisplayCommand.vertexFormat;
                var vatIndex = (int)vertexFormat;
                verts[i] = new GxVertex()
                {
                    vertAttr = vat.GxVertexAttributeFormats[vatIndex],
                };
                reader.ReadX(ref verts[i], false);
            }

            endAddress = reader.BaseStream.Position;
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }

    [Serializable]
    public sealed class GxDisplayList : IBinarySerializable
    {
        [Header("GX Display List Group")]
        [SerializeField, Hex(8)] long startAddress;
        [SerializeField, Hex(8)] long endAddress;
        [SerializeField] GxVertexAttributeTable vat;

        [Space]
        [SerializeField, Hex(8)] uint gxBufferSize;
        [SerializeField] GxDisplayObject[] gxDisplayList;


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

        public GxDisplayList() { }

        public GxDisplayList(GxVertexAttributeTable vat, uint gxBufferSize)
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
            var newList = new List<GxDisplayObject>();

            startAddress = reader.BaseStream.Position;

            // this code doesn't work because you're doing it wrong

            //var endPos = startAddress + gxBufferSize;
            //while (reader.BaseStream.Position < endPos)
            //{
            //    var absPtr = reader.BaseStream.Position;
            //    var gxCommand = new GxDisplayCommand();
            //    reader.ReadX(ref gxCommand, true);
            //    reader.BaseStream.Seek(absPtr, SeekOrigin.Begin);

            //    if (gxCommand.command != 0)
            //    {
            //        // Read command into list
            //        var displayList = new GxDisplayList();
            //        displayList.vat = vat;
            //        reader.ReadX(ref displayList, false);
            //        newList.Add(displayList);
            //    }
            //    // Break when we are reading GX_NOP (0)
            //    else break;
            //}
            endAddress = reader.BaseStream.Position;

            this.gxDisplayList = newList.ToArray();
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }

    public static class GxUtility
    {
        public const int GX_FIFO_ALIGN = 32;

        // Split into POS ans NRM?
        public static Vector3 ReadVectorComponent(BinaryReader reader, GXCompCnt_Rev2 nElements, GXCompType componentType, int nFracs)
        {
            if (nElements == GXCompCnt_Rev2.GX_POS_XY)
            {
                return new Vector2(
                    ReadNumericComponent(reader, componentType, nFracs),
                    ReadNumericComponent(reader, componentType, nFracs));
            }
            else if (nElements == GXCompCnt_Rev2.GX_NRM_XYZ
                  || nElements == GXCompCnt_Rev2.GX_POS_XYZ)
            {
                return new Vector3(
                    ReadNumericComponent(reader, componentType, nFracs),
                    ReadNumericComponent(reader, componentType, nFracs),
                    ReadNumericComponent(reader, componentType, nFracs));
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static Vector2 ReadGxTextureST(BinaryReader reader, GXCompCnt_Rev2 nElements, GXCompType componentType, int nFracs)
        {
            if (nElements == GXCompCnt_Rev2.GX_TEX_S)
            {
                return new Vector2(
                    ReadNumericComponent(reader, componentType, nFracs),
                    0f);
            }
            else if (nElements == GXCompCnt_Rev2.GX_TEX_ST)
            {
                return new Vector2(
                    ReadNumericComponent(reader, componentType, nFracs),
                    ReadNumericComponent(reader, componentType, nFracs));
            }
            else
            {
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

        // TODO: replace with lookup table
        public static float FixedToFloat(float value, int nFracBits)
        {
            value = (float)Math.Pow(2, value);
            var divisor = (float)Math.Pow(2, nFracBits);
            value = value / divisor;
            return value;
        }
    }
}