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
    /// What would compromise a column in GX VAT - Vertex Attribute Table
    /// </summary>
    [Serializable]
    public class GxVertexAttributeFormat
    {
        [SerializeField] public GXAttrType attributeType;

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

        public void SetVtxAttrFmt(GXVtxFmt index, GXAttrType vcd, GXAttr attribute, GXCompCnt_Rev2 nElements, GXCompType format, int nFracBits = 0)
        {
            var value = new GxVertexAttribute(vcd, attribute, nElements, format, nFracBits);
            GxVertexAttributeFormats[(int)index].SetAttr(value);
        }
    }

    [Serializable]
    public struct GxVertexAttribute
    {
        [SerializeField] public bool enabled;
        [SerializeField] public GXAttrType vcd;
        [SerializeField] public GXAttr attribute;
        [SerializeField] public GXCompCnt_Rev2 nElements;
        [SerializeField] public GXCompType componentFormat;
        [SerializeField] public int nFracBits;

        public GxVertexAttribute(GXAttrType vcd, GXAttr attribute, GXCompCnt_Rev2 nElements, GXCompType format, int nFracBits = 0)
        {
            // Assert that we aren't shifting more bits than we have
            if (format == GXCompType.GX_S8 | format == GXCompType.GX_U8)
                Assert.IsTrue(nFracBits < 8);
            if (format == GXCompType.GX_S16 | format == GXCompType.GX_U16)
                Assert.IsTrue(nFracBits < 16);

            this.enabled = true;
            this.vcd = vcd;
            this.attribute = attribute;
            this.nElements = nElements;
            this.componentFormat = format;
            this.nFracBits = nFracBits;
        }
    }

    [Serializable]
    public struct GxVertex : IBinarySerializable
    {
        [SerializeField] public GxVertexAttributeFormat vertAttr;

        // (Raph:) Missing any other data?
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
            // POSITION
            position = GxUtility.ReadGxVectorXYZ(reader, vertAttr.pos);

            // NORMALS
            if (vertAttr.nrm.enabled)
            {
                normal = GxUtility.ReadGxVectorXYZ(reader, vertAttr.nrm);
            }
            else if (vertAttr.nbt.enabled)
            {
                // This code is untested...
                // And it lacks another case for NBT3
                throw new NotImplementedException();

                normal = GxUtility.ReadGxVectorXYZ(reader, vertAttr.nbt);
                binormal = GxUtility.ReadGxVectorXYZ(reader, vertAttr.nbt);
                tangent = GxUtility.ReadGxVectorXYZ(reader, vertAttr.nbt);
            }

            // COLOR
            if (vertAttr.clr0.enabled)
                color0 = GxUtility.ReadGXColor(reader, vertAttr.clr0);
            if (vertAttr.clr1.enabled)
                color1 = GxUtility.ReadGXColor(reader, vertAttr.clr1);

            // TEX
            if (vertAttr.tex0.enabled)
                tex0 = GxUtility.ReadGxTextureST(reader, vertAttr.tex0);
            if (vertAttr.tex1.enabled)
                tex1 = GxUtility.ReadGxTextureST(reader, vertAttr.tex1);
            if (vertAttr.tex2.enabled)
                tex2 = GxUtility.ReadGxTextureST(reader, vertAttr.tex2);
            if (vertAttr.tex3.enabled)
                tex3 = GxUtility.ReadGxTextureST(reader, vertAttr.tex3);
            if (vertAttr.tex4.enabled)
                tex4 = GxUtility.ReadGxTextureST(reader, vertAttr.tex4);
            if (vertAttr.tex5.enabled)
                tex5 = GxUtility.ReadGxTextureST(reader, vertAttr.tex5);
            if (vertAttr.tex6.enabled)
                tex6 = GxUtility.ReadGxTextureST(reader, vertAttr.tex6);
            if (vertAttr.tex7.enabled)
                tex7 = GxUtility.ReadGxTextureST(reader, vertAttr.tex7);
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
            throw new NotImplementedException();
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

        public static Vector3 ReadGxVectorXYZ(BinaryReader reader, GxVertexAttribute vertexAttributes)
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
}