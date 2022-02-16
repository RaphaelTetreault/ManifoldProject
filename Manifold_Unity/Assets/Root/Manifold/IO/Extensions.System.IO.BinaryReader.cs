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
        public static byte PeekUint8(this BinaryReader reader) => reader.PeekValue(BinaryIoUtility.ReadUInt8);
        public static ushort PeekUint16(this BinaryReader reader) => reader.PeekValue(BinaryIoUtility.ReadUInt16);
        public static uint PeekUint32(this BinaryReader reader) => reader.PeekValue(BinaryIoUtility.ReadUInt32);
        public static ulong PeekUint64(this BinaryReader reader) => reader.PeekValue(BinaryIoUtility.ReadUInt64);
        public static sbyte PeekInt8(this BinaryReader reader) => reader.PeekValue(BinaryIoUtility.ReadInt8);
        public static short PeekInt16(this BinaryReader reader) => reader.PeekValue(BinaryIoUtility.ReadInt16);
        public static int PeekInt32(this BinaryReader reader) => reader.PeekValue(BinaryIoUtility.ReadInt32);
        public static long PeekInt64(this BinaryReader reader) => reader.PeekValue(BinaryIoUtility.ReadInt64);

        // Basic type names
        public static byte PeekByte(this BinaryReader reader) => reader.PeekValue(BinaryIoUtility.ReadUInt8);
        public static ushort PeekUshort(this BinaryReader reader) => reader.PeekValue(BinaryIoUtility.ReadUInt16);
        public static uint PeekUint(this BinaryReader reader) => reader.PeekValue(BinaryIoUtility.ReadUInt32);
        public static ulong PeekUlong(this BinaryReader reader) => reader.PeekValue(BinaryIoUtility.ReadUInt64);
        public static sbyte PeekSbyte(this BinaryReader reader) => reader.PeekValue(BinaryIoUtility.ReadInt8);
        public static short PeekShort(this BinaryReader reader) => reader.PeekValue(BinaryIoUtility.ReadInt16);
        public static int PeekInt(this BinaryReader reader) => reader.PeekValue(BinaryIoUtility.ReadInt32);
        public static long PeekLong(this BinaryReader reader) => reader.PeekValue(BinaryIoUtility.ReadInt64);

        // Floating point numbers
        public static float PeekFloat(this BinaryReader reader) => reader.PeekValue(BinaryIoUtility.ReadFloat);
        public static double PeekDouble(this BinaryReader reader) => reader.PeekValue(BinaryIoUtility.ReadDouble);
        public static decimal PeekDecimal(this BinaryReader reader) => reader.PeekValue(BinaryIoUtility.ReadDecimal);


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

        public static decimal ReadX_Decimal(this BinaryReader reader)
            => BinaryIoUtility.ReadDecimal(reader);

        public static char ReadX_Char(this BinaryReader reader)
            => BinaryIoUtility.ReadChar(reader, BinaryIoUtility.Encoding);

        public static char ReadX_Char(this BinaryReader reader, Encoding encoding)
            => BinaryIoUtility.ReadChar(reader, encoding);

        public static string ReadX_String(this BinaryReader reader)
            => BinaryIoUtility.ReadString(reader, BinaryIoUtility.Encoding);

        public static string ReadX_String(this BinaryReader reader, int length)
            => BinaryIoUtility.ReadString(reader, BinaryIoUtility.Encoding);

        public static string ReadX_String(this BinaryReader reader, int length, Encoding encoding)
            => BinaryIoUtility.ReadString(reader, encoding);

        public static string ReadX_String(this BinaryReader reader, Encoding encoding)
            => BinaryIoUtility.ReadString(reader, encoding);

        public static T ReadX_NewIBinarySerializable<T>(this BinaryReader reader) where T : IBinarySerializable, new()
            => BinaryIoUtility.ReadNewIBinarySerializable<T>(reader);
        
        public static TEnum ReadX_Enum<TEnum>(this BinaryReader reader) where TEnum : System.Enum
            => BinaryIoUtility.ReadEnum<TEnum>(reader);

        #endregion


        #region UNUSED
        #region ReadX_ValueArray

        public static bool[] ReadX_BoolArray(this BinaryReader reader)
            => BinaryIoUtility.ReadBoolArray(reader);

        public static byte[] ReadX_Uint8Array(this BinaryReader reader)
            => BinaryIoUtility.ReadUint8Array(reader);

        public static sbyte[] ReadX_Int8Array(this BinaryReader reader)
            => BinaryIoUtility.ReadInt8Array(reader);

        public static ushort[] ReadX_Uint16Array(this BinaryReader reader)
            => BinaryIoUtility.ReadUint16Array(reader);

        public static short[] ReadX_Int16Array(this BinaryReader reader)
            => BinaryIoUtility.ReadInt16Array(reader);

        public static uint[] ReadX_Uint32Array(this BinaryReader reader)
            => BinaryIoUtility.ReadUint32Array(reader);

        public static int[] ReadX_Int32Array(this BinaryReader reader)
            => BinaryIoUtility.ReadInt32Array(reader);

        public static ulong[] ReadX_Uint64Array(this BinaryReader reader)
            => BinaryIoUtility.ReadUint64Array(reader);

        public static long[] ReadX_Int64Array(this BinaryReader reader)
            => BinaryIoUtility.ReadInt64Array(reader);

        public static float[] ReadX_FloatArray(this BinaryReader reader)
            => BinaryIoUtility.ReadFloatArray(reader);

        public static double[] ReadX_DoubleArray(this BinaryReader reader)
            => BinaryIoUtility.ReadDoubleArray(reader);

        public static decimal[] ReadX_DecimalArray(this BinaryReader reader)
            => BinaryIoUtility.ReadDecimalArray(reader);

        public static string[] ReadX_StringArray(this BinaryReader reader)
            => BinaryIoUtility.ReadStringArray(reader, BinaryIoUtility.Encoding);

        public static string[] ReadX_StringArray(this BinaryReader reader, Encoding encoding)
            => BinaryIoUtility.ReadStringArray(reader, encoding);

        public static T[] ReadX_NewIBinarySerializableArray<T>(this BinaryReader reader) where T : IBinarySerializable, new()
            => BinaryIoUtility.ReadNewIBinarySerializableArray<T>(reader);

        public static TEnum[] ReadX_EnumArray<TEnum>(this BinaryReader reader) where TEnum : System.Enum
            => BinaryIoUtility.ReadEnumArray<TEnum>(reader);
        

        #endregion

        #region ReadX_ValueArray(int length)

        public static bool[] ReadX_BoolArray(this BinaryReader reader, int length)
            => BinaryIoUtility.ReadBoolArray(reader, length);

        public static byte[] ReadX_Uint8Array(this BinaryReader reader, int length)
            => BinaryIoUtility.ReadUint8Array(reader, length);

        public static sbyte[] ReadX_Int8Array(this BinaryReader reader, int length)
            => BinaryIoUtility.ReadInt8Array(reader, length);

        public static ushort[] ReadX_Uint16Array(this BinaryReader reader, int length)
            => BinaryIoUtility.ReadUint16Array(reader, length);

        public static short[] ReadX_Int16Array(this BinaryReader reader, int length)
            => BinaryIoUtility.ReadInt16Array(reader, length);

        public static uint[] ReadX_Uint32Array(this BinaryReader reader, int length)
            => BinaryIoUtility.ReadUint32Array(reader, length);

        public static int[] ReadX_Int32Array(this BinaryReader reader, int length)
            => BinaryIoUtility.ReadInt32Array(reader, length);

        public static ulong[] ReadX_Uint64Array(this BinaryReader reader, int length)
            => BinaryIoUtility.ReadUint64Array(reader, length);

        public static long[] ReadX_Int64Array(this BinaryReader reader, int length)
            => BinaryIoUtility.ReadInt64Array(reader, length);

        public static float[] ReadX_FloatArray(this BinaryReader reader, int length)
            => BinaryIoUtility.ReadFloatArray(reader, length);

        public static double[] ReadX_DoubleArray(this BinaryReader reader, int length)
            => BinaryIoUtility.ReadDoubleArray(reader, length);

        public static decimal[] ReadX_DecimalArray(this BinaryReader reader, int length)
            => BinaryIoUtility.ReadDecimalArray(reader, length);

        public static string[] ReadX_StringArray(this BinaryReader reader, int length)
            => BinaryIoUtility.ReadStringArray(reader, length, BinaryIoUtility.Encoding);

        public static string[] ReadX_StringArray(this BinaryReader reader, int length, Encoding encoding)
            => BinaryIoUtility.ReadStringArray(reader, length, encoding);

        public static T[] ReadX_NewIBinarySerializable<T>(this BinaryReader reader, int length) where T : IBinarySerializable, new()
            => BinaryIoUtility.ReadNewIBinarySerializableArray<T>(reader, length);

        public static TEnum[] ReadX_EnumArray<TEnum>(this BinaryReader reader, int length) where TEnum : System.Enum
            => BinaryIoUtility.ReadEnumArray<TEnum>(reader, length);

        #endregion
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
        public static decimal ReadX(this BinaryReader reader, ref decimal value)
            => BinaryIoUtility.Read(reader, ref value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ReadX(this BinaryReader reader, ref char value)
            => BinaryIoUtility.Read(reader, ref value, BinaryIoUtility.Encoding);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ReadX(this BinaryReader reader, ref char value, Encoding encoding)
            => BinaryIoUtility.Read(reader, ref value, encoding);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadX(this BinaryReader reader, ref string value)
            => BinaryIoUtility.Read(reader, ref value, BinaryIoUtility.Encoding);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadX(this BinaryReader reader, ref string value, Encoding encoding)
            => BinaryIoUtility.Read(reader, ref value, encoding);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadX(this BinaryReader reader, int length, ref string value)
            => BinaryIoUtility.Read(reader, ref value, BinaryIoUtility.Encoding);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadX(this BinaryReader reader, int length, ref string value, Encoding encoding) 
           => BinaryIoUtility.Read(reader, ref value, encoding);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ReadX<T>(this BinaryReader reader, ref T value, bool createNewInstance) where T : IBinarySerializable, new()
            => BinaryIoUtility.Read(reader, ref value, createNewInstance);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum ReadX<TEnum>(this BinaryReader reader, ref TEnum value) where TEnum : struct, System.Enum
            =>  BinaryIoUtility.Read(reader, ref value);

        #endregion

        #region ReadX(ref value[])

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool[] ReadX(this BinaryReader reader, ref bool[] value) 
           => BinaryIoUtility.Read(reader, ref value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ReadX(this BinaryReader reader, ref byte[] value) 
           => BinaryIoUtility.Read(reader, ref value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte[] ReadX(this BinaryReader reader, ref sbyte[] value)
            => BinaryIoUtility.Read(reader, ref value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort[] ReadX(this BinaryReader reader, ref ushort[] value)
            => BinaryIoUtility.Read(reader, ref value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short[] ReadX(this BinaryReader reader, ref short[] value)
            => BinaryIoUtility.Read(reader, ref value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint[] ReadX(this BinaryReader reader, ref uint[] value)
            => BinaryIoUtility.Read(reader, ref value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int[] ReadX(this BinaryReader reader, ref int[] value)
            => BinaryIoUtility.Read(reader, ref value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong[] ReadX(this BinaryReader reader, ref ulong[] value)
            => BinaryIoUtility.Read(reader, ref value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long[] ReadX(this BinaryReader reader, ref long[] value)
            => BinaryIoUtility.Read(reader, ref value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float[] ReadX(this BinaryReader reader, ref float[] value)
            => BinaryIoUtility.Read(reader, ref value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double[] ReadX(this BinaryReader reader, ref double[] value) 
           => BinaryIoUtility.Read(reader, ref value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal[] ReadX(this BinaryReader reader, ref decimal[] value)
            => BinaryIoUtility.Read(reader, ref value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string[] ReadX(this BinaryReader reader, ref string[] value)
            => BinaryIoUtility.Read(reader, ref value, BinaryIoUtility.Encoding);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string[] ReadX(this BinaryReader reader, ref string[] value, Encoding encoding)
            => BinaryIoUtility.Read(reader, ref value, encoding);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] ReadX<T>(this BinaryReader reader, ref T[] value, bool createNewInstances) where T : IBinarySerializable, new()
            => BinaryIoUtility.Read(reader, ref value, createNewInstances);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum[] ReadX<TEnum>(this BinaryReader reader, ref TEnum[] value) where TEnum : struct, System.Enum
            => BinaryIoUtility.ReadEnum(reader, ref value);

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
        public static decimal[] ReadX(this BinaryReader reader, ref decimal[] value, int length)
            => BinaryIoUtility.Read(reader, length, ref value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string[] ReadX(this BinaryReader reader, ref string[] value, int length)
            => BinaryIoUtility.Read(reader, ref value, BinaryIoUtility.Encoding);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string[] ReadX(this BinaryReader reader, ref string[] value, int length, Encoding encoding)
            => BinaryIoUtility.Read(reader, ref value, encoding);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] ReadX<T>(this BinaryReader reader, ref T[] value, int length, bool createNewInstances) where T : IBinarySerializable, new()
            => BinaryIoUtility.Read(reader, length, ref value, createNewInstances);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum[] ReadX<TEnum>(this BinaryReader reader, int length, ref TEnum[] value) where TEnum : struct, System.Enum
            =>  BinaryIoUtility.Read(reader, length, ref value);

        #endregion

    }
}
