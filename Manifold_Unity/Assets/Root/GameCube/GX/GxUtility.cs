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
            // For NBT, the caller of this function should call it 3 times, each for N, B, and T
            if (nElements == ComponentCount.GX_NRM_XYZ || nElements == ComponentCount.GX_NRM_NBT)
            {
                return new float3(
                    ReadNumber(reader, componentType, nFracs),
                    ReadNumber(reader, componentType, nFracs),
                    ReadNumber(reader, componentType, nFracs));
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


        public static byte FloatToFixedU8(float value, int nFracBits)
        {
            return (byte)(value * (1 << nFracBits));
        }
        public static sbyte FloatToFixedS8(float value, int nFracBits)
        {
            return (sbyte)(value * (1 << nFracBits));
        }
        public static ushort FloatToFixedU16(float value, int nFracBits)
        {
            return (ushort)(value * (1 << nFracBits));
        }
        public static short FloatToFixedS16(float value, int nFracBits)
        {
            return (short)(value * (1 << nFracBits));
        }




        public static void WritePosition(BinaryWriter writer, float3 position, ComponentCount nElements, ComponentType componentType, int nFracs)
        {
            if (nElements == ComponentCount.GX_POS_XYZ)
            {
                WriteNumber(writer, position.x, componentType, nFracs);
                WriteNumber(writer, position.y, componentType, nFracs);
                WriteNumber(writer, position.z, componentType, nFracs);
            }
            else if (nElements == ComponentCount.GX_POS_XY)
            {
                WriteNumber(writer, position.x, componentType, nFracs);
                WriteNumber(writer, position.y, componentType, nFracs);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public static void WriteNormal(BinaryWriter writer, float3 normal, ComponentCount nElements, ComponentType componentType, int nFracs)
        {
            // For NBT, the caller of this function should call it 3 times, each for N, B, and T
            if (nElements == ComponentCount.GX_NRM_XYZ || nElements == ComponentCount.GX_NRM_NBT)
            {
                WriteNumber(writer, normal.x, componentType, nFracs);
                WriteNumber(writer, normal.y, componentType, nFracs);
                WriteNumber(writer, normal.z, componentType, nFracs);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public static void WriteUV(BinaryWriter writer, float2 textureUV, ComponentCount nElements, ComponentType componentType, int nFracs)
        {
            if (nElements == ComponentCount.GX_TEX_ST)
            {
                WriteNumber(writer, textureUV.x, componentType, nFracs);
                WriteNumber(writer, textureUV.y, componentType, nFracs);
            }
            else if (nElements == ComponentCount.GX_TEX_S)
            {
                WriteNumber(writer, textureUV.x, componentType, nFracs);
            }
            else
            {
                throw new ArgumentException();
            }
        }
        public static void WriteColor(BinaryWriter writer, Color32 color, ComponentType type)
        {
            switch (type)
            {
                //case ComponentType.GX_RGB565:
                //    {

                //    }

                //case ComponentType.GX_RGB8:
                //    {

                //    }

                //case ComponentType.GX_RGBA4:
                //    {

                //    }

                //case ComponentType.GX_RGBA6:
                //    {

                //    }

                //case ComponentType.GX_RGBA8:
                //    {

                //    }

                //case ComponentType.GX_RGBX8:
                //    {

                //    }

                default:
                    throw new ArgumentException();
            }
        }
        public static void WriteNumber(BinaryWriter writer, float value, ComponentType componentType, int nFracs)
        {
            switch (componentType)
            {
                case ComponentType.GX_F32: writer.WriteX(value); return;
                case ComponentType.GX_S16: writer.WriteX(FloatToFixedS16(value, nFracs)); return;
                case ComponentType.GX_U16: writer.WriteX(FloatToFixedU16(value, nFracs)); return;
                case ComponentType.GX_S8: writer.WriteX(FloatToFixedS8(value, nFracs)); return;
                case ComponentType.GX_U8: writer.WriteX(FloatToFixedU8(value, nFracs)); return;

                default:
                    throw new ArgumentException();
            };
        }


        public static int GetGxVertexSize(GXAttributes attributes, VertexAttributeFormat fmt)
        {
            // Check each component type, see if it is used
            bool hasPNMTXIDX = attributes.HasFlag(GXAttributes.GX_VA_PNMTXIDX);
            bool hasTEX0MTXIDX = attributes.HasFlag(GXAttributes.GX_VA_TEX0MTXIDX);
            bool hasTEX1MTXIDX = attributes.HasFlag(GXAttributes.GX_VA_TEX1MTXIDX);
            bool hasTEX2MTXIDX = attributes.HasFlag(GXAttributes.GX_VA_TEX2MTXIDX);
            bool hasTEX3MTXIDX = attributes.HasFlag(GXAttributes.GX_VA_TEX3MTXIDX);
            bool hasTEX4MTXIDX = attributes.HasFlag(GXAttributes.GX_VA_TEX4MTXIDX);
            bool hasTEX5MTXIDX = attributes.HasFlag(GXAttributes.GX_VA_TEX5MTXIDX);
            bool hasTEX6MTXIDX = attributes.HasFlag(GXAttributes.GX_VA_TEX6MTXIDX);
            bool hasTEX7MTXIDX = attributes.HasFlag(GXAttributes.GX_VA_TEX7MTXIDX);
            bool hasPOS_MTX_ARRAY = attributes.HasFlag(GXAttributes.GX_VA_POS_MTX_ARRAY);
            bool hasNRM_MTX_ARRAY = attributes.HasFlag(GXAttributes.GX_VA_NRM_MTX_ARRAY);
            bool hasTEX_MTX_ARRAY = attributes.HasFlag(GXAttributes.GX_VA_TEX_MTX_ARRAY);
            bool hasLIGHT_ARRAY = attributes.HasFlag(GXAttributes.GX_VA_LIGHT_ARRAY);
            bool hasPOS = attributes.HasFlag(GXAttributes.GX_VA_POS);
            bool hasNRM = attributes.HasFlag(GXAttributes.GX_VA_NRM);
            bool hasNBT = attributes.HasFlag(GXAttributes.GX_VA_NBT);
            bool hasCLR0 = attributes.HasFlag(GXAttributes.GX_VA_CLR0);
            bool hasCLR1 = attributes.HasFlag(GXAttributes.GX_VA_CLR1);
            bool hasTEX0 = attributes.HasFlag(GXAttributes.GX_VA_TEX0);
            bool hasTEX1 = attributes.HasFlag(GXAttributes.GX_VA_TEX1);
            bool hasTEX2 = attributes.HasFlag(GXAttributes.GX_VA_TEX2);
            bool hasTEX3 = attributes.HasFlag(GXAttributes.GX_VA_TEX3);
            bool hasTEX4 = attributes.HasFlag(GXAttributes.GX_VA_TEX4);
            bool hasTEX5 = attributes.HasFlag(GXAttributes.GX_VA_TEX5);
            bool hasTEX6 = attributes.HasFlag(GXAttributes.GX_VA_TEX6);
            bool hasTEX7 = attributes.HasFlag(GXAttributes.GX_VA_TEX7);

            // Don't know what these look like
            if (hasPOS_MTX_ARRAY || hasNRM_MTX_ARRAY || hasTEX_MTX_ARRAY || hasLIGHT_ARRAY)
                throw new NotImplementedException("Unsupported GXAttributes flag");
            
            const int mtxIdxSize = 1;
            int size = 0;
            size += hasPNMTXIDX ? mtxIdxSize : 0;
            size += hasTEX0MTXIDX ? mtxIdxSize : 0;
            size += hasTEX1MTXIDX ? mtxIdxSize : 0;
            size += hasTEX2MTXIDX ? mtxIdxSize : 0;
            size += hasTEX3MTXIDX ? mtxIdxSize : 0;
            size += hasTEX4MTXIDX ? mtxIdxSize : 0;
            size += hasTEX5MTXIDX ? mtxIdxSize : 0;
            size += hasTEX6MTXIDX ? mtxIdxSize : 0;
            size += hasTEX7MTXIDX ? mtxIdxSize : 0;
            size += hasPOS ? CompSizeNumber(fmt.pos.ComponentFormat) * GetPosCompCount(fmt.tex0.NElements) : 0;
            size += hasNRM ? CompSizeNumber(fmt.nrm.ComponentFormat) * GetNrmCompCount(fmt.tex0.NElements) : 0;
            size += hasNBT ? CompSizeNumber(fmt.nbt.ComponentFormat) * GetNrmCompCount(fmt.tex0.NElements) : 0;
            size += hasCLR0 ? CompSizeColor(fmt.clr0.ComponentFormat) * GetTexCompCount(fmt.tex0.NElements) : 0;
            size += hasCLR1 ? CompSizeColor(fmt.clr1.ComponentFormat) * GetTexCompCount(fmt.tex0.NElements) : 0;
            size += hasTEX0 ? CompSizeNumber(fmt.tex0.ComponentFormat) * GetTexCompCount(fmt.tex0.NElements) : 0;
            size += hasTEX1 ? CompSizeNumber(fmt.tex1.ComponentFormat) * GetTexCompCount(fmt.tex1.NElements) : 0;
            size += hasTEX2 ? CompSizeNumber(fmt.tex2.ComponentFormat) * GetTexCompCount(fmt.tex2.NElements) : 0;
            size += hasTEX3 ? CompSizeNumber(fmt.tex3.ComponentFormat) * GetTexCompCount(fmt.tex3.NElements) : 0;
            size += hasTEX4 ? CompSizeNumber(fmt.tex4.ComponentFormat) * GetTexCompCount(fmt.tex4.NElements) : 0;
            size += hasTEX5 ? CompSizeNumber(fmt.tex5.ComponentFormat) * GetTexCompCount(fmt.tex5.NElements) : 0;
            size += hasTEX6 ? CompSizeNumber(fmt.tex6.ComponentFormat) * GetTexCompCount(fmt.tex6.NElements) : 0;
            size += hasTEX7 ? CompSizeNumber(fmt.tex7.ComponentFormat) * GetTexCompCount(fmt.tex7.NElements) : 0;

            return size;
        }

        private static int GetTexCompCount(ComponentCount componentCount)
        {
            switch (componentCount)
            {
                case ComponentCount.GX_TEX_S: return 1;
                case ComponentCount.GX_TEX_ST: return 2;

                default:
                    throw new ArgumentException();
            }
        }

        private static int GetPosCompCount(ComponentCount componentCount)
        {
            switch (componentCount)
            {
                case ComponentCount.GX_POS_XY: return 2;
                case ComponentCount.GX_POS_XYZ: return 3;

                default:
                    throw new ArgumentException();
            }
        }

        private static int GetNrmCompCount(ComponentCount componentCount)
        {
            switch (componentCount)
            {
                case ComponentCount.GX_NRM_XYZ: return 3;
                case ComponentCount.GX_NRM_NBT: return 9;
                case ComponentCount.GX_NRM_NBT3: throw new NotImplementedException();

                default:
                    throw new ArgumentException();
            }
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
                    throw new ArgumentException();
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
                    throw new ArgumentException();
            }
        }

    }
}