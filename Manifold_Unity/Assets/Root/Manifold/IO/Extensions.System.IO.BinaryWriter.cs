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

        /// <summary>
        /// Sets the stream's position to 0.
        /// </summary>
        /// <param name="writer"></param>
        public static void SeekBegin(this BinaryWriter writer)
        {
            writer.BaseStream.Seek(0, SeekOrigin.Begin);
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

        //public static void WriteXCString(this BinaryWriter writer, string value, Encoding encoding)
        //=> BinaryIoUtility.WriteCString(writer, value, encoding);

        //public static void WriteXCString(this BinaryWriter writer, string value)
        //=> BinaryIoUtility.WriteCString(writer, value);

        //
        public static void WriteE<TEnum>(this BinaryWriter writer, TEnum[] value, bool writeLengthHeader) where TEnum : Enum
        {
            BinaryIoUtility.WriteEnum(writer, value, writeLengthHeader);
        }


        #endregion

    }
}