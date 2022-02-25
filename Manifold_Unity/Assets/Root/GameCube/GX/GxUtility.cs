using GameCube.GFZ.GMA;
using Manifold.IO;
using System;
using System.IO;
using UnityEngine;
using Unity.Mathematics;

namespace GameCube.GX
{
    public static class GXUtility
    {
        public const int GX_FIFO_ALIGN = 32;

        public static float3 ReadPos(BinaryReader reader, ComponentCount nElements, ComponentType componentType, int nFracs)
        {
            if (nElements == ComponentCount.GX_POS_XYZ)
            {
                return new float3(
                    ReadNumber(reader, componentType, nFracs),
                    ReadNumber(reader, componentType, nFracs),
                    ReadNumber(reader, componentType, nFracs));
            }
            else if (nElements == ComponentCount.GX_POS_XY)
            {
                return new float3(
                    ReadNumber(reader, componentType, nFracs),
                    ReadNumber(reader, componentType, nFracs),
                    0f);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static float3 ReadNormal(BinaryReader reader, ComponentCount nElements, ComponentType componentType, int nFracs)
        {
            if (nElements == ComponentCount.GX_NRM_XYZ)
            {
                return new float3(
                    ReadNumber(reader, componentType, nFracs),
                    ReadNumber(reader, componentType, nFracs),
                    ReadNumber(reader, componentType, nFracs));
            }
            else if (nElements == ComponentCount.GX_NRM_NBT)
            {
                throw new NotImplementedException();
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static float2 ReadUV(BinaryReader reader, ComponentCount nElements, ComponentType componentType, int nFracs)
        {
            if (nElements == ComponentCount.GX_TEX_ST)
            {
                return new float2(
                    ReadNumber(reader, componentType, nFracs),
                    ReadNumber(reader, componentType, nFracs));
            }
            else if (nElements == ComponentCount.GX_TEX_S)
            {
                return new float2(
                    ReadNumber(reader, componentType, nFracs),
                    0f);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static Color32 ReadColor(BinaryReader reader, ComponentType type)
        {
            switch (type)
            {
                case ComponentType.GX_RGB565:
                    {
                        var rgb565 = BinaryIoUtility.ReadUInt16(reader);
                        var r = (byte)(((rgb565 >> 11) & (0b_0001_1111)) * (1 << 3));
                        var g = (byte)(((rgb565 >> 05) & (0b_0011_1111)) * (1 << 2));
                        var b = (byte)(((rgb565 >> 00) & (0b_0001_1111)) * (1 << 3));
                        return new Color32(r, g, b, byte.MaxValue);
                    }

                case ComponentType.GX_RGB8:
                    {
                        var r = BinaryIoUtility.ReadUInt8(reader);
                        var g = BinaryIoUtility.ReadUInt8(reader);
                        var b = BinaryIoUtility.ReadUInt8(reader);
                        return new Color32(r, g, b, byte.MaxValue);
                    }

                case ComponentType.GX_RGBA4:
                    {
                        var rgba4 = BinaryIoUtility.ReadUInt16(reader);
                        var r = (byte)(((rgba4 >> 12) & (0b_0000_1111)) * (1 << 4));
                        var g = (byte)(((rgba4 >> 08) & (0b_0000_1111)) * (1 << 4));
                        var b = (byte)(((rgba4 >> 04) & (0b_0000_1111)) * (1 << 4));
                        var a = (byte)(((rgba4 >> 00) & (0b_0000_1111)) * (1 << 4));
                        return new Color32(r, g, b, a);
                    }

                case ComponentType.GX_RGBA6:
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

                case ComponentType.GX_RGBA8:
                    {
                        var r = BinaryIoUtility.ReadUInt8(reader);
                        var g = BinaryIoUtility.ReadUInt8(reader);
                        var b = BinaryIoUtility.ReadUInt8(reader);
                        var a = BinaryIoUtility.ReadUInt8(reader);
                        return new Color32(r, g, b, a);
                    }

                case ComponentType.GX_RGBX8:
                    {
                        var r = BinaryIoUtility.ReadUInt8(reader);
                        var g = BinaryIoUtility.ReadUInt8(reader);
                        var b = BinaryIoUtility.ReadUInt8(reader);
                        var _ = BinaryIoUtility.ReadUInt8(reader); // discarded
                        return new Color32(r, g, b, byte.MaxValue);
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        public static float ReadNumber(BinaryReader reader, ComponentType type, int nFracBits)
        {
            switch (type)
            {
                case ComponentType.GX_F32:
                    return BinaryIoUtility.ReadFloat(reader);

                case ComponentType.GX_S8:
                    return FixedS8ToFloat(BinaryIoUtility.ReadInt8(reader), nFracBits);

                case ComponentType.GX_U8:
                    return FixedU8ToFloat(BinaryIoUtility.ReadUInt8(reader), nFracBits);

                case ComponentType.GX_S16:
                    return FixedS16ToFloat(BinaryIoUtility.ReadInt16(reader), nFracBits);

                case ComponentType.GX_U16:
                    return FixedU16ToFloat(BinaryIoUtility.ReadUInt16(reader), nFracBits);

                default:
                    throw new NotImplementedException();
            }
        }

        public static float FixedU8ToFloat(byte value, int nFracBits)
        {
            return (float)value / (1 << nFracBits);
        }
        public static float FixedS8ToFloat(sbyte value, int nFracBits)
        {
            return (float)value / (1 << nFracBits);
        }
        public static float FixedU16ToFloat(ushort value, int nFracBits)
        {
            return (float)value / (1 << nFracBits);
        }
        public static float FixedS16ToFloat(short value, int nFracBits)
        {
            return (float)value / (1 << nFracBits);
        }

        public static int CalcGxVtxStride(GXAttributes attrFlag, VertexAttributeFormat fmt)
        {
            int size = 0;
            const int mtxIdxSize = 1;

            if ((attrFlag & GXAttributes.GX_VA_PNMTXIDX) != 0)
                size += mtxIdxSize;

            if ((attrFlag & GXAttributes.GX_VA_TEX0MTXIDX) != 0)
                throw new NotImplementedException();
            if ((attrFlag & GXAttributes.GX_VA_TEX1MTXIDX) != 0)
                throw new NotImplementedException();
            if ((attrFlag & GXAttributes.GX_VA_TEX2MTXIDX) != 0)
                throw new NotImplementedException();
            if ((attrFlag & GXAttributes.GX_VA_TEX3MTXIDX) != 0)
                throw new NotImplementedException();
            if ((attrFlag & GXAttributes.GX_VA_TEX4MTXIDX) != 0)
                throw new NotImplementedException();
            if ((attrFlag & GXAttributes.GX_VA_TEX5MTXIDX) != 0)
                throw new NotImplementedException();
            if ((attrFlag & GXAttributes.GX_VA_TEX6MTXIDX) != 0)
                throw new NotImplementedException();
            if ((attrFlag & GXAttributes.GX_VA_TEX7MTXIDX) != 0)
                throw new NotImplementedException();

            if ((attrFlag & GXAttributes.GX_VA_POS_MTX_ARRAY) != 0)
                throw new NotImplementedException();
            if ((attrFlag & GXAttributes.GX_VA_NRM_MTX_ARRAY) != 0)
                throw new NotImplementedException();
            if ((attrFlag & GXAttributes.GX_VA_TEX_MTX_ARRAY) != 0)
                throw new NotImplementedException();
            if ((attrFlag & GXAttributes.GX_VA_LIGHT_ARRAY) != 0)
                throw new NotImplementedException();


            if (fmt.pos != null && (attrFlag & GXAttributes.GX_VA_POS) != 0)
                size += CompSizeColor(fmt.pos.ComponentFormat);

            if (fmt.nrm != null && (attrFlag & GXAttributes.GX_VA_NRM) != 0)
                size += CompSizeNumber(fmt.nrm.ComponentFormat);
            if (fmt.nbt != null && (attrFlag & GXAttributes.GX_VA_NBT) != 0)
                size += CompSizeNumber(fmt.nbt.ComponentFormat);

            // Get size of colors
            if (fmt.clr0 != null && (attrFlag & GXAttributes.GX_VA_CLR0) != 0)
                size += CompSizeColor(fmt.clr0.ComponentFormat);
            if (fmt.clr1 != null && (attrFlag & GXAttributes.GX_VA_CLR1) != 0)
                size += CompSizeColor(fmt.clr1.ComponentFormat);

            if (fmt.tex0 != null && (attrFlag & GXAttributes.GX_VA_TEX0) != 0)
                size += CompSizeNumber(fmt.tex0.ComponentFormat);
            if (fmt.tex1 != null && (attrFlag & GXAttributes.GX_VA_TEX1) != 0)
                size += CompSizeNumber(fmt.tex1.ComponentFormat);
            if (fmt.tex2 != null && (attrFlag & GXAttributes.GX_VA_TEX2) != 0)
                size += CompSizeNumber(fmt.tex2.ComponentFormat);
            if (fmt.tex3 != null && (attrFlag & GXAttributes.GX_VA_TEX3) != 0)
                size += CompSizeNumber(fmt.tex3.ComponentFormat);
            if (fmt.tex4 != null && (attrFlag & GXAttributes.GX_VA_TEX4) != 0)
                size += CompSizeNumber(fmt.tex4.ComponentFormat);
            if (fmt.tex5 != null && (attrFlag & GXAttributes.GX_VA_TEX5) != 0)
                size += CompSizeNumber(fmt.tex5.ComponentFormat);
            if (fmt.tex6 != null && (attrFlag & GXAttributes.GX_VA_TEX6) != 0)
                size += CompSizeNumber(fmt.tex6.ComponentFormat);
            if (fmt.tex7 != null && (attrFlag & GXAttributes.GX_VA_TEX7) != 0)
                size += CompSizeNumber(fmt.tex7.ComponentFormat);

            return size;
        }

        private static int CompSizeNumber(ComponentType compType)
        {
            switch (compType)
            {
                case ComponentType.GX_U8: return 1;
                case ComponentType.GX_S8: return 1;
                case ComponentType.GX_U16: return 2;
                case ComponentType.GX_S16: return 2;
                case ComponentType.GX_F32: return 4;

                default:
                    throw new NotImplementedException();
            }
        }

        private static int CompSizeColor(ComponentType compType)
        {
            switch (compType)
            {
                case ComponentType.GX_RGB565: return 2;
                case ComponentType.GX_RGB8: return 1;
                case ComponentType.GX_RGBX8: return 4;
                case ComponentType.GX_RGBA4: return 2;
                case ComponentType.GX_RGBA6: return 3;
                case ComponentType.GX_RGBA8: return 4;

                default:
                    throw new NotImplementedException();
            }
        }

    }
}