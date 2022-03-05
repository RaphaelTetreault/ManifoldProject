using System;
using System.IO;
using System.Text;
using System.Runtime.CompilerServices;

namespace Manifold.IO
{
    public static partial class BinaryReaderExtensions
    {
        /// <summary>
        /// Mimics the functionality of StreamReader.EndOfStream
        /// </summary>
        /// <param name="reader"></param>
        /// <returns>True when at the end of the stream</returns>
        public static bool IsAtEndOfStream(this BinaryReader reader)
            => StreamExtensions.IsAtEndOfStream(reader.BaseStream);

        public static long AlignTo(this BinaryReader reader, long alignment)
        {
            var bytesToAlign = StreamExtensions.GetLengthOffAlignment(reader.BaseStream, alignment);
            reader.BaseStream.Seek(bytesToAlign, SeekOrigin.Current);
            return bytesToAlign;
        }

        /// <summary>
        /// Sets the stream's position to 0.
        /// </summary>
        /// <param name="reader"></param>
        public static void SeekBegin(this BinaryReader reader)
        {
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
        }

        #region PeekValue

        /// <summary>
        /// Peeks the specified type from the <paramref name="reader"/>'s base stream.
        /// </summary>
        /// <typeparam name="T">The type to peek</typeparam>
        /// <param name="reader">The reader to peek from</param>
        /// <param name="deserializationMethod">The method used to deserialize the value from stream</param>
        /// <returns></returns>
        public static T PeekValue<T>(this BinaryReader reader, Func<BinaryReader, T> deserializationMethod)
        {
            long streamPosition = reader.BaseStream.Position;
            T value = deserializationMethod(reader);
            reader.BaseStream.Seek(streamPosition, SeekOrigin.Begin);
            return value;
        }

        // System type names
        public static byte PeekUInt8(this BinaryReader reader) => reader.PeekValue(BinaryIoUtility.ReadUInt8);
        public static ushort PeekUInt16(this BinaryReader reader) => reader.PeekValue(BinaryIoUtility.ReadUInt16);
        public static uint PeekUInt32(this BinaryReader reader) => reader.PeekValue(BinaryIoUtility.ReadUInt32);
        public static ulong PeekUInt64(this BinaryReader reader) => reader.PeekValue(BinaryIoUtility.ReadUInt64);
        public static sbyte PeekInt8(this BinaryReader reader) => reader.PeekValue(BinaryIoUtility.ReadInt8);
        public static short PeekInt16(this BinaryReader reader) => reader.PeekValue(BinaryIoUtility.ReadInt16);
        public static int PeekInt32(this BinaryReader reader) => reader.PeekValue(BinaryIoUtility.ReadInt32);
        public static long PeekInt64(this BinaryReader reader) => reader.PeekValue(BinaryIoUtility.ReadInt64);

        // Basic type names
        public static byte PeekByte(this BinaryReader reader) => reader.PeekValue(BinaryIoUtility.ReadUInt8);
        public static ushort PeekUshort(this BinaryReader reader) => reader.PeekValue(BinaryIoUtility.ReadUInt16);
        public static uint PeekUInt(this BinaryReader reader) => reader.PeekValue(BinaryIoUtility.ReadUInt32);
        public static ulong PeekUlong(this BinaryReader reader) => reader.PeekValue(BinaryIoUtility.ReadUInt64);
        public static sbyte PeekSbyte(this BinaryReader reader) => reader.PeekValue(BinaryIoUtility.ReadInt8);
        public static short PeekShort(this BinaryReader reader) => reader.PeekValue(BinaryIoUtility.ReadInt16);
        public static int PeekInt(this BinaryReader reader) => reader.PeekValue(BinaryIoUtility.ReadInt32);
        public static long PeekLong(this BinaryReader reader) => reader.PeekValue(BinaryIoUtility.ReadInt64);

        // Floating point numbers
        public static float PeekFloat(this BinaryReader reader) => reader.PeekValue(BinaryIoUtility.ReadFloat);
        public static double PeekDouble(this BinaryReader reader) => reader.PeekValue(BinaryIoUtility.ReadDouble);


        #endregion


        // BinaryIOUtility function forwarding

        #region ReadX_Value

        public static bool ReadX_Bool(this BinaryReader reader)
            => reader.ReadBoolean();

        public static byte ReadX_UInt8(this BinaryReader reader)
            => reader.ReadByte();

        public static sbyte ReadX_Int8(this BinaryReader reader)
            => reader.ReadSByte();

        public static ushort ReadX_UInt16(this BinaryReader reader)
            => BinaryIoUtility.ReadUInt16(reader);

        public static short ReadX_Int16(this BinaryReader reader)
            => BinaryIoUtility.ReadInt16(reader);

        public static uint ReadX_UInt32(this BinaryReader reader)
            => BinaryIoUtility.ReadUInt32(reader);

        public static int ReadX_Int32(this BinaryReader reader)
            => BinaryIoUtility.ReadInt32(reader);

        public static ulong ReadX_UInt64(this BinaryReader reader)
            => BinaryIoUtility.ReadUInt64(reader);

        public static long ReadX_Int64(this BinaryReader reader)
            => BinaryIoUtility.ReadInt64(reader);

        public static float ReadX_Float(this BinaryReader reader)
            => BinaryIoUtility.ReadFloat(reader);

        public static double ReadX_Double(this BinaryReader reader)
            => BinaryIoUtility.ReadDouble(reader);

        public static string ReadX_String(this BinaryReader reader, int lengthBytes, Encoding encoding)
            => BinaryIoUtility.ReadString(reader, lengthBytes, encoding);

        public static TBinarySerializable ReadX_BinarySerializable<TBinarySerializable>(this BinaryReader reader) where TBinarySerializable : IBinarySerializable, new()
            => BinaryIoUtility.ReadBinarySerializable<TBinarySerializable>(reader);
        
        public static TEnum ReadX_Enum<TEnum>(this BinaryReader reader) where TEnum : Enum
            => BinaryIoUtility.ReadEnum<TEnum>(reader);

        #endregion


        #region ReadX(ref value)

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReadX(this BinaryReader reader, ref bool value)
            => BinaryIoUtility.Read(reader, ref value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ReadX(this BinaryReader reader, ref byte value)
            => BinaryIoUtility.Read(reader, ref value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte ReadX(this BinaryReader reader, ref sbyte value)
            => BinaryIoUtility.Read(reader, ref value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReadX(this BinaryReader reader, ref ushort value)
            => BinaryIoUtility.Read(reader, ref value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReadX(this BinaryReader reader, ref short value)
            => BinaryIoUtility.Read(reader, ref value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadX(this BinaryReader reader, ref uint value)
            => BinaryIoUtility.Read(reader, ref value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadX(this BinaryReader reader, ref int value)
            => BinaryIoUtility.Read(reader, ref value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ReadX(this BinaryReader reader, ref ulong value)
            => BinaryIoUtility.Read(reader, ref value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReadX(this BinaryReader reader, ref long value)
            => BinaryIoUtility.Read(reader, ref value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ReadX(this BinaryReader reader, ref float value)
            => BinaryIoUtility.Read(reader, ref value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ReadX(this BinaryReader reader, ref double value)
            => BinaryIoUtility.Read(reader, ref value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadX(this BinaryReader reader, ref string value, int lengthBytes, Encoding encoding) 
           => BinaryIoUtility.Read(reader, ref value, lengthBytes, encoding);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ReadX<T>(this BinaryReader reader, ref T value) where T : IBinarySerializable, new()
            => BinaryIoUtility.ReadBinarySerializable(reader, ref value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum ReadX<TEnum>(this BinaryReader reader, ref TEnum value, bool discard = false) where TEnum : struct, Enum
            =>  BinaryIoUtility.Read(reader, ref value);

        #endregion

        #region ReadX(ref value[], int length)

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool[] ReadX(this BinaryReader reader, ref bool[] value, int length)
            => BinaryIoUtility.Read(reader, length, ref value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ReadX(this BinaryReader reader, ref byte[] value, int length)
            => BinaryIoUtility.Read(reader, length, ref value);

        public static sbyte[] ReadX(this BinaryReader reader, ref sbyte[] value, int length)
            => BinaryIoUtility.Read(reader, length, ref value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort[] ReadX(this BinaryReader reader, ref ushort[] value, int length)
            => BinaryIoUtility.Read(reader, length, ref value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short[] ReadX(this BinaryReader reader, ref short[] value, int length)
            => BinaryIoUtility.Read(reader, length, ref value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint[] ReadX(this BinaryReader reader, ref uint[] value, int length) 
           => BinaryIoUtility.Read(reader, length, ref value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int[] ReadX(this BinaryReader reader, ref int[] value, int length)
            => BinaryIoUtility.Read(reader, length, ref value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong[] ReadX(this BinaryReader reader, ref ulong[] value, int length)
            => BinaryIoUtility.Read(reader, length, ref value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long[] ReadX(this BinaryReader reader, ref long[] value, int length)
            => BinaryIoUtility.Read(reader, length, ref value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float[] ReadX(this BinaryReader reader, ref float[] value, int length)
            => BinaryIoUtility.Read(reader, length, ref value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double[] ReadX(this BinaryReader reader, ref double[] value, int length)
            => BinaryIoUtility.Read(reader, length, ref value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TBinarySerializable[] ReadX<TBinarySerializable>(this BinaryReader reader, ref TBinarySerializable[] value, int length) where TBinarySerializable : IBinarySerializable, new()
            => BinaryIoUtility.Read(reader, length, ref value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum[] ReadX<TEnum>(this BinaryReader reader, int length, ref TEnum[] value) where TEnum : struct, Enum
            =>  BinaryIoUtility.Read(reader, length, ref value);

        #endregion

    }
}
