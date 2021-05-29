using System;
using System.IO;
using System.Text;

namespace Manifold.IO
{
    public static partial class BinaryWriterExtensions
    {
        public static long AlignTo(this BinaryWriter writer, long alignment, byte paddingValue = 0x00)
        {
            var bytesToAlign = StreamExtensions.GetLengthOffAlignment(writer.BaseStream, alignment);
            for (int i = 0; i < bytesToAlign; i++)
                writer.Write(paddingValue);

            return bytesToAlign;
        }


        // BinaryIOUtility function forwarding

        #region WriteX(value)


        public static void WriteX(this BinaryWriter writer, bool value)
        => BinaryIoUtility.Write(writer, value);

        public static void WriteX(this BinaryWriter writer, byte value)
        => BinaryIoUtility.Write(writer, value);

        public static void WriteX(this BinaryWriter writer, sbyte value)
        => BinaryIoUtility.Write(writer, value);

        public static void WriteX(this BinaryWriter writer, ushort value)
        => BinaryIoUtility.Write(writer, value);

        public static void WriteX(this BinaryWriter writer, short value)
        => BinaryIoUtility.Write(writer, value);

        public static void WriteX(this BinaryWriter writer, uint value)
        => BinaryIoUtility.Write(writer, value);

        public static void WriteX(this BinaryWriter writer, int value)
        => BinaryIoUtility.Write(writer, value);

        public static void WriteX(this BinaryWriter writer, ulong value)
        => BinaryIoUtility.Write(writer, value);

        public static void WriteX(this BinaryWriter writer, long value)
        => BinaryIoUtility.Write(writer, value);

        public static void WriteX(this BinaryWriter writer, float value)
        => BinaryIoUtility.Write(writer, value);

        public static void WriteX(this BinaryWriter writer, double value)
        => BinaryIoUtility.Write(writer, value);

        public static void WriteX(this BinaryWriter writer, decimal value)
        => BinaryIoUtility.Write(writer, value);

        public static void WriteX(this BinaryWriter writer, char value)
        => BinaryIoUtility.Write(writer, value, BinaryIoUtility.Encoding);

        public static void WriteX(this BinaryWriter writer, char value, Encoding encoding)
        => BinaryIoUtility.Write(writer, value, encoding);

        public static void WriteX<T>(this BinaryWriter writer, T value) where T : IBinarySerializable, new()
        {
            BinaryIoUtility.Write(writer, value);
        }

        // HACK: discard lets us use the name WriteX without conflicting with the above method
        public static void WriteX<TEnum>(this BinaryWriter writer, TEnum value, int _ = 0) where TEnum : struct, Enum
        {
            BinaryIoUtility.WriteEnum(writer, value);
        }


        #endregion

        #region WriteX(value[])


        public static void WriteX(this BinaryWriter writer, bool[] value, bool writeLengthHeader)
        => BinaryIoUtility.Write(writer, value, writeLengthHeader);

        public static void WriteX(this BinaryWriter writer, byte[] value, bool writeLengthHeader)
        => BinaryIoUtility.Write(writer, value, writeLengthHeader);

        public static void WriteX(this BinaryWriter writer, sbyte[] value, bool writeLengthHeader)
        => BinaryIoUtility.Write(writer, value, writeLengthHeader);

        public static void WriteX(this BinaryWriter writer, ushort[] value, bool writeLengthHeader)
        => BinaryIoUtility.Write(writer, value, writeLengthHeader);

        public static void WriteX(this BinaryWriter writer, short[] value, bool writeLengthHeader)
        => BinaryIoUtility.Write(writer, value, writeLengthHeader);

        public static void WriteX(this BinaryWriter writer, uint[] value, bool writeLengthHeader)
        => BinaryIoUtility.Write(writer, value, writeLengthHeader);

        public static void WriteX(this BinaryWriter writer, int[] value, bool writeLengthHeader)
        => BinaryIoUtility.Write(writer, value, writeLengthHeader);

        public static void WriteX(this BinaryWriter writer, ulong[] value, bool writeLengthHeader)
        => BinaryIoUtility.Write(writer, value, writeLengthHeader);

        public static void WriteX(this BinaryWriter writer, long[] value, bool writeLengthHeader)
        => BinaryIoUtility.Write(writer, value, writeLengthHeader);

        public static void WriteX(this BinaryWriter writer, float[] value, bool writeLengthHeader)
        => BinaryIoUtility.Write(writer, value, writeLengthHeader);

        public static void WriteX(this BinaryWriter writer, double[] value, bool writeLengthHeader)
        => BinaryIoUtility.Write(writer, value, writeLengthHeader);

        public static void WriteX(this BinaryWriter writer, decimal[] value, bool writeLengthHeader)
        => BinaryIoUtility.Write(writer, value, writeLengthHeader);

        public static void WriteX(this BinaryWriter writer, string value, bool writeLengthHeader)
        => BinaryIoUtility.Write(writer, value, BinaryIoUtility.Encoding, writeLengthHeader);

        public static void WriteX(this BinaryWriter writer, string value, Encoding encoding, bool writeLengthHeader)
        => BinaryIoUtility.Write(writer, value, encoding, writeLengthHeader);

        public static void WriteX(this BinaryWriter writer, string[] value, bool writeLengthHeader)
        => BinaryIoUtility.Write(writer, value, BinaryIoUtility.Encoding, writeLengthHeader);

        public static void WriteX(this BinaryWriter writer, string[] value, Encoding encoding, bool writeLengthHeader)
        => BinaryIoUtility.Write(writer, value, encoding, writeLengthHeader);

        public static void WriteX<T>(this BinaryWriter writer, T[] value, bool writeLengthHeader) where T : IBinarySerializable, new()
        => BinaryIoUtility.Write(writer, value, writeLengthHeader);

        public static void WriteXCString(this BinaryWriter writer, string value, Encoding encoding)
        => BinaryIoUtility.WriteCString(writer, value, encoding);

        public static void WriteXCString(this BinaryWriter writer, string value)
        => BinaryIoUtility.WriteCString(writer, value);

        //
        public static void WriteE<TEnum>(this BinaryWriter writer, TEnum[] value, bool writeLengthHeader) where TEnum : Enum
        {
            BinaryIoUtility.WriteEnum(writer, value, writeLengthHeader);
        }


        #endregion


        // Added the below extensions to help debug file outputs
        public static void Comment(this BinaryWriter writer, string message, bool doWrite = true, int alignment = 16, byte padding = (byte)' ')
        {
            // Allow option to write or not. Prevents a lot of if statements.
            if (!doWrite)
                return;

            var bytes = Encoding.ASCII.GetBytes(message);

            writer.AlignTo(alignment, padding);
            writer.WriteX(bytes, false);
            writer.AlignTo(alignment, padding);
        }
        public static void Comment<T>(this BinaryWriter writer, bool doWrite = true, int alignment = 16, byte padding = (byte)' ')
        {
            // Allow option to write or not. Prevents a lot of if statements.
            if (!doWrite)
                return;

            var message = typeof(T).Name;
            var bytes = Encoding.ASCII.GetBytes(message);

            writer.AlignTo(alignment, padding);
            writer.WriteX(bytes, false);
            writer.AlignTo(alignment, padding);
        }

        public static void CommentNewLine(this BinaryWriter writer, bool doWrite = true, int alignment = 16, byte padding = (byte)' ')
        {
            writer.AlignTo(alignment, padding);
            for (int i = 0; i < alignment; i++)
                writer.WriteX(padding);
        }

        //public static void PointerComment(this BinaryWriter writer, int address, int alignment = 16, byte padding = (byte)' ')
        //{
        //    writer.AlignTo(alignment, padding);
        //    for (int i = 0; i < alignment; i++)
        //        writer.WriteX(padding);

        //    var message = $"pointer {address:x8}";
        //    var bytes = Encoding.ASCII.GetBytes(message);

        //    writer.WriteX(bytes, false);
        //    writer.AlignTo(alignment, padding);
        //}

        public static void CommentPointer(this BinaryWriter writer, int address, bool doWrite, int alignment = 16, byte padding = (byte)' ')
        {
            // Allow option to write or not. Prevents a lot of if statements.
            if (!doWrite)
                return;

            //writer.AlignTo(alignment, padding);
            //for (int i = 0; i < alignment; i++)
            //    writer.WriteX(padding);

            var message = $"pointer {address:x8}";
            var bytes = Encoding.ASCII.GetBytes(message);

            writer.WriteX(bytes, false);
            writer.AlignTo(alignment, padding);
        }

        public static void PointerComment(this BinaryWriter writer, IPointer pointer, bool doWrite, int alignment = 16, byte padding = (byte)' ')
        {
            CommentPointer(writer, pointer.Address, doWrite, alignment, padding);
        }

        public static void PointerComment(this BinaryWriter writer, AddressRange addresRange, bool doWrite, int alignment = 16, byte padding = (byte)' ')
        {
            CommentPointer(writer, (int)addresRange.startAddress, doWrite, alignment, padding);
        }

    }
}