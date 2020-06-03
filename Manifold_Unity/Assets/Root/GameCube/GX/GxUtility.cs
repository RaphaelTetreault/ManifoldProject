using StarkTools.IO;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using GameCube.GFZX01.GMA;

namespace GameCube.GX
{

    public static class GxUtility
    {
        public const int GX_FIFO_ALIGN = 32;

        public static Vector3 ReadPos(BinaryReader reader, GXCompCnt_Rev2 nElements, GXCompType componentType, int nFracs)
        {
            if (nElements == GXCompCnt_Rev2.GX_POS_XY)
            {
                return new Vector2(
                    ReadNumericComponent(reader, componentType, nFracs),
                    ReadNumericComponent(reader, componentType, nFracs));
            }
            else if (nElements == GXCompCnt_Rev2.GX_POS_XYZ)
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

        public static Vector3 ReadNormal(BinaryReader reader, GXCompCnt_Rev2 nElements, GXCompType componentType, int nFracs)
        {
            if (nElements == GXCompCnt_Rev2.GX_NRM_XYZ || nElements == GXCompCnt_Rev2.GX_NRM_NBT)
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
                    return FixedS8ToFloat(BinaryIoUtility.ReadInt8(reader), nFracBits);

                case GXCompType.GX_U8:
                    return FixedU8ToFloat(BinaryIoUtility.ReadUInt8(reader), nFracBits);

                case GXCompType.GX_S16:
                    return FixedS16ToFloat(BinaryIoUtility.ReadInt16(reader), nFracBits);

                case GXCompType.GX_U16:
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

        public static int CalcGxVtxStride(GXAttrFlag_U32 attrFlag, GxVtxAttrFmt fmt)
        {
            int size = 0;
            const int mtxIdxSize = 1;

            if ((attrFlag & GXAttrFlag_U32.GX_VA_PNMTXIDX) != 0)
                size += mtxIdxSize;

            if ((attrFlag & GXAttrFlag_U32.GX_VA_TEX0MTXIDX) != 0)
                throw new NotImplementedException();
            if ((attrFlag & GXAttrFlag_U32.GX_VA_TEX1MTXIDX) != 0)
                throw new NotImplementedException();
            if ((attrFlag & GXAttrFlag_U32.GX_VA_TEX2MTXIDX) != 0)
                throw new NotImplementedException();
            if ((attrFlag & GXAttrFlag_U32.GX_VA_TEX3MTXIDX) != 0)
                throw new NotImplementedException();
            if ((attrFlag & GXAttrFlag_U32.GX_VA_TEX4MTXIDX) != 0)
                throw new NotImplementedException();
            if ((attrFlag & GXAttrFlag_U32.GX_VA_TEX5MTXIDX) != 0)
                throw new NotImplementedException();
            if ((attrFlag & GXAttrFlag_U32.GX_VA_TEX6MTXIDX) != 0)
                throw new NotImplementedException();
            if ((attrFlag & GXAttrFlag_U32.GX_VA_TEX7MTXIDX) != 0)
                throw new NotImplementedException();

            if ((attrFlag & GXAttrFlag_U32.GX_VA_POS_MTX_ARRAY) != 0)
                throw new NotImplementedException();
            if ((attrFlag & GXAttrFlag_U32.GX_VA_NRM_MTX_ARRAY) != 0)
                throw new NotImplementedException();
            if ((attrFlag & GXAttrFlag_U32.GX_VA_TEX_MTX_ARRAY) != 0)
                throw new NotImplementedException();
            if ((attrFlag & GXAttrFlag_U32.GX_VA_LIGHT_ARRAY) != 0)
                throw new NotImplementedException();


            if (fmt.pos != null && (attrFlag & GXAttrFlag_U32.GX_VA_POS) != 0)
                size += CompSizeColor(fmt.pos.componentFormat);

            if (fmt.nrm != null && (attrFlag & GXAttrFlag_U32.GX_VA_NRM) != 0)
                size += CompSizeNumber(fmt.nrm.componentFormat);
            if (fmt.nbt != null && (attrFlag & GXAttrFlag_U32.GX_VA_NBT) != 0)
                size += CompSizeNumber(fmt.nbt.componentFormat);

            // Get size of colors
            if (fmt.clr0 != null && (attrFlag & GXAttrFlag_U32.GX_VA_CLR0) != 0)
                size += CompSizeColor(fmt.clr0.componentFormat);
            if (fmt.clr1 != null && (attrFlag & GXAttrFlag_U32.GX_VA_CLR1) != 0)
                size += CompSizeColor(fmt.clr1.componentFormat);

            if (fmt.tex0 != null && (attrFlag & GXAttrFlag_U32.GX_VA_TEX0) != 0)
                size += CompSizeNumber(fmt.tex0.componentFormat);
            if (fmt.tex1 != null && (attrFlag & GXAttrFlag_U32.GX_VA_TEX1) != 0)
                size += CompSizeNumber(fmt.tex1.componentFormat);
            if (fmt.tex2 != null && (attrFlag & GXAttrFlag_U32.GX_VA_TEX2) != 0)
                size += CompSizeNumber(fmt.tex2.componentFormat);
            if (fmt.tex3 != null && (attrFlag & GXAttrFlag_U32.GX_VA_TEX3) != 0)
                size += CompSizeNumber(fmt.tex3.componentFormat);
            if (fmt.tex4 != null && (attrFlag & GXAttrFlag_U32.GX_VA_TEX4) != 0)
                size += CompSizeNumber(fmt.tex4.componentFormat);
            if (fmt.tex5 != null && (attrFlag & GXAttrFlag_U32.GX_VA_TEX5) != 0)
                size += CompSizeNumber(fmt.tex5.componentFormat);
            if (fmt.tex6 != null && (attrFlag & GXAttrFlag_U32.GX_VA_TEX6) != 0)
                size += CompSizeNumber(fmt.tex6.componentFormat);
            if (fmt.tex7 != null && (attrFlag & GXAttrFlag_U32.GX_VA_TEX7) != 0)
                size += CompSizeNumber(fmt.tex7.componentFormat);

            return size;
        }

        private static int CompSizeNumber(GXCompType compType)
        {
            switch (compType)
            {
                case GXCompType.GX_U8: return 1;
                case GXCompType.GX_S8: return 1;
                case GXCompType.GX_U16: return 2;
                case GXCompType.GX_S16: return 2;
                case GXCompType.GX_F32: return 4;

                default:
                    throw new NotImplementedException();
            }
        }

        private static int CompSizeColor(GXCompType compType)
        {
            switch (compType)
            {
                case GXCompType.GX_RGB565: return 2;
                case GXCompType.GX_RGB8: return 1;
                case GXCompType.GX_RGBX8: return 4;
                case GXCompType.GX_RGBA4: return 2;
                case GXCompType.GX_RGBA6: return 3;
                case GXCompType.GX_RGBA8: return 4;

                default:
                    throw new NotImplementedException();
            }
        }




    }
}