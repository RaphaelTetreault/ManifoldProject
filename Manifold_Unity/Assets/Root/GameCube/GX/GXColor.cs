using Manifold;
using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GX
{
    public class GXColor :
        IBinarySerializable
    {
        private ComponentType componentType = ComponentType.GX_RGBA8;

        private byte r;
        private byte g;
        private byte b;
        private byte a;

        public byte R { get => r; set => r = value; }
        public byte G { get => g; set => g = value; }
        public byte B { get => b; set => b = value; }
        public byte A { get => a; set => a = value; }
        public ComponentType ComponentType { get => componentType; set => componentType = value; }


        public GXColor()
        {

        }

        public GXColor(ComponentType componentType)
        {
            this.componentType = componentType;
        }


        public void Deserialize(BinaryReader reader)
        {
            switch (componentType)
            {
                case ComponentType.GX_RGB565: ReadRGBA565(reader); break;
                case ComponentType.GX_RGB8: ReadRGB8(reader); break;
                case ComponentType.GX_RGBA4: ReadRGBA4(reader); break;
                case ComponentType.GX_RGBA6: ReadRGBA6(reader); break;
                case ComponentType.GX_RGBA8: ReadRGBA8(reader); break;
                case ComponentType.GX_RGBX8: ReadRGBX8(reader); break;

                default:
                    throw new ArgumentException("Invalid Color type");
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            switch (componentType)
            {
                case ComponentType.GX_RGB565: WriteRGBA565(writer); break;
                case ComponentType.GX_RGB8: WriteRGB8(writer); break;
                case ComponentType.GX_RGBA4: WriteRGBA4(writer); break;
                case ComponentType.GX_RGBA6: WriteRGBA6(writer); break;
                case ComponentType.GX_RGBA8: WriteRGBA8(writer); break;
                case ComponentType.GX_RGBX8: WriteRGBX8(writer); break;

                default:
                    throw new ArgumentException("Invalid Color type");
            }
        }


        private void ReadRGBA565(BinaryReader reader)
        {
            // Left >> & = mask bits you want, right << to make bits max at 255
            var rgb565 = BinaryIoUtility.ReadUInt16(reader);
            r = (byte)(((rgb565 >> 11) & (0b_0001_1111)) * (1 << 3));
            g = (byte)(((rgb565 >> 05) & (0b_0011_1111)) * (1 << 2));
            b = (byte)(((rgb565 >> 00) & (0b_0001_1111)) * (1 << 3));
        }
        private void ReadRGB8(BinaryReader reader)
        {
            r = BinaryIoUtility.ReadUInt8(reader);
            g = BinaryIoUtility.ReadUInt8(reader);
            b = BinaryIoUtility.ReadUInt8(reader);
        }
        private void ReadRGBA4(BinaryReader reader)
        {
            var rgba4 = BinaryIoUtility.ReadUInt16(reader);
            r = (byte)(((rgba4 >> 12) & (0b_0000_1111)) * (1 << 4));
            g = (byte)(((rgba4 >> 08) & (0b_0000_1111)) * (1 << 4));
            b = (byte)(((rgba4 >> 04) & (0b_0000_1111)) * (1 << 4));
            a = (byte)(((rgba4 >> 00) & (0b_0000_1111)) * (1 << 4));
        }
        private void ReadRGBA6(BinaryReader reader)
        {
            // Reconstruct the 24bit color as uint32
            var upper16 = BinaryIoUtility.ReadUInt16(reader);
            var lower8 = BinaryIoUtility.ReadUInt8(reader);
            var rgba6 = (uint)(upper16 << 8) | (lower8);

            r = (byte)(((rgba6 >> 18) & (0b_0011_1111)) * (1 << 2));
            g = (byte)(((rgba6 >> 12) & (0b_0011_1111)) * (1 << 2));
            b = (byte)(((rgba6 >> 06) & (0b_0011_1111)) * (1 << 2));
            a = (byte)(((rgba6 >> 00) & (0b_0011_1111)) * (1 << 2));
        }
        private void ReadRGBA8(BinaryReader reader)
        {
            r = BinaryIoUtility.ReadUInt8(reader);
            g = BinaryIoUtility.ReadUInt8(reader);
            b = BinaryIoUtility.ReadUInt8(reader);
            a = BinaryIoUtility.ReadUInt8(reader);
        }
        private void ReadRGBX8(BinaryReader reader)
        {
            r = BinaryIoUtility.ReadUInt8(reader);
            g = BinaryIoUtility.ReadUInt8(reader);
            b = BinaryIoUtility.ReadUInt8(reader);
            var _ = BinaryIoUtility.ReadUInt8(reader); // discarded
        }


        private void WriteRGBA565(BinaryWriter writer)
        {
            byte r5 = (byte)((r >> 3) & 0b_0001_1111);
            byte g6 = (byte)((g >> 2) & 0b_0011_1111);
            byte b5 = (byte)((b >> 3) & 0b_0001_1111);
            ushort rgb565 = (ushort)(r5 << 11 + g6 << 05 + b5 << 00);
            writer.WriteX(rgb565);
        }
        private void WriteRGB8(BinaryWriter writer)
        {
            writer.WriteX(r);
            writer.WriteX(g);
            writer.WriteX(b);
        }
        private void WriteRGBA4(BinaryWriter writer)
        {
            byte r4 = (byte)((r >> 4) & 0b_0000_1111);
            byte g4 = (byte)((g >> 4) & 0b_0000_1111);
            byte b4 = (byte)((b >> 4) & 0b_0000_1111);
            byte a4 = (byte)((a >> 4) & 0b_0000_1111);
            ushort rgba4 = (ushort)(r4 << 12 + g4 << 08 + b4 << 04 + a4 << 00);
            writer.WriteX(rgba4);
        }
        private void WriteRGBA6(BinaryWriter writer)
        {
            byte r6 = (byte)((r >> 6) & 0b_0011_1111);
            byte g6 = (byte)((g >> 6) & 0b_0011_1111);
            byte b6 = (byte)((b >> 6) & 0b_0011_1111);
            byte a6 = (byte)((a >> 6) & 0b_0011_1111);
            uint rgba6 = (uint)(r6 << 18 + g6 << 12 + b6 << 06 + a6 << 00);
            byte rgba6_hi = (byte)((rgba6 >> 16) & 0b_1111_1111);
            byte rgba6_mi = (byte)((rgba6 >> 08) & 0b_1111_1111);
            byte rgba6_lo = (byte)((rgba6 >> 00) & 0b_1111_1111);
            writer.WriteX(rgba6_hi);
            writer.WriteX(rgba6_mi);
            writer.WriteX(rgba6_lo);
        }
        private void WriteRGBA8(BinaryWriter writer)
        {
            writer.WriteX(r);
            writer.WriteX(g);
            writer.WriteX(b);
            writer.WriteX(a);
        }
        private void WriteRGBX8(BinaryWriter writer)
        {
            writer.WriteX(r);
            writer.WriteX(g);
            writer.WriteX(b);
            writer.WriteX((byte)0xFF);
        }


    }
}
