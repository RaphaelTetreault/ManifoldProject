using System;
using System.IO;
using System.Text;

namespace Manifold.IO
{
    public static partial class BinaryWriterExtensions
    {
        public static long WriteAlignment(this BinaryWriter writer, long alignment, byte paddingValue = 0x00)
        {
            var bytesToAlign = StreamExtensions.GetLengthOfAlignment(writer.BaseStream, alignment);
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

        public static void WriteX<TBinarySerializable>(this BinaryWriter writer, TBinarySerializable value) where TBinarySerializable : IBinarySerializable
        => BinaryIoUtility.Write(writer, value);

        // HACK: discard lets us use the name WriteX without conflicting with the above method
        public static void WriteX<TEnum>(this BinaryWriter writer, TEnum value, byte _ = 0) where TEnum : Enum
        => BinaryIoUtility.Write(writer, value);



        public static void WriteX(this BinaryWriter writer, bool[] value)
        => BinaryIoUtility.Write(writer, value);

        public static void WriteX(this BinaryWriter writer, byte[] value)
        => BinaryIoUtility.Write(writer, value);

        public static void WriteX(this BinaryWriter writer, sbyte[] value)
        => BinaryIoUtility.Write(writer, value);

        public static void WriteX(this BinaryWriter writer, ushort[] value)
        => BinaryIoUtility.Write(writer, value);

        public static void WriteX(this BinaryWriter writer, short[] value)
        => BinaryIoUtility.Write(writer, value);

        public static void WriteX(this BinaryWriter writer, uint[] value)
        => BinaryIoUtility.Write(writer, value);

        public static void WriteX(this BinaryWriter writer, int[] value)
        => BinaryIoUtility.Write(writer, value);

        public static void WriteX(this BinaryWriter writer, ulong[] value)
        => BinaryIoUtility.Write(writer, value);

        public static void WriteX(this BinaryWriter writer, long[] value)
        => BinaryIoUtility.Write(writer, value);

        public static void WriteX(this BinaryWriter writer, float[] value)
        => BinaryIoUtility.Write(writer, value);

        public static void WriteX(this BinaryWriter writer, double[] value)
        => BinaryIoUtility.Write(writer, value);

        public static void WriteX(this BinaryWriter writer, string value, Encoding encoding, bool writeLengthBytes)
        => BinaryIoUtility.Write(writer, value, encoding, writeLengthBytes);

        public static void WriteX<TBinarySerializable>(this BinaryWriter writer, TBinarySerializable[] value) where TBinarySerializable : IBinarySerializable
        => BinaryIoUtility.Write(writer, value);
        
        public static void WriteX<TEnum>(this BinaryWriter writer, TEnum[] value, byte _ = 0) where TEnum : Enum
        => BinaryIoUtility.Write(writer, value);

    }
}