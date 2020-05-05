using System.IO;
using System.Text;
using System.Runtime.CompilerServices;

namespace StarkTools.IO
{
    public static partial class BinaryIoExtensions
    {
        /// <summary>
        /// Mimics the functionality of StreamReader.EndOfStream
        /// </summary>
        /// <param name="reader"></param>
        /// <returns>True when at then of the of the stream</returns>
        public static bool EndOfStream(this BinaryReader reader)
        {
            return !(reader.BaseStream.Position < reader.BaseStream.Length);
        }

        public static long Align(this BinaryReader reader, long alignment)
        {
            var bytesToAlign = alignment - (reader.BaseStream.Position % alignment);
            bytesToAlign = (bytesToAlign == alignment) ? 0 : bytesToAlign;
            reader.BaseStream.Seek(bytesToAlign, SeekOrigin.Current);
            return bytesToAlign;
        }

        public static long Align(this BinaryWriter writer, long alignment, byte padding = 0x00)
        {
            var bytesToAlign = alignment - (writer.BaseStream.Position % alignment);
            bytesToAlign = (bytesToAlign == alignment) ? 0 : bytesToAlign;
            for (int i = 0; i < bytesToAlign; i++)
                writer.Write(padding);

            return bytesToAlign;
        }

        // 2020-05-05
        public static byte PeekByte(this BinaryReader reader)
        {
            byte b = ReadX_UInt8(reader);
            reader.BaseStream.Position--;
            return b;
        }

        #region ReadX

        #region ReadX Value


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReadX_Bool(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadBool(reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ReadX_UInt8(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadUInt8(reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte ReadX_Int8(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadInt8(reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReadX_UInt16(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadUInt16(reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReadX_Int16(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadInt16(reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadX_UInt32(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadUInt32(reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadX_Int32(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadInt32(reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ReadX_UInt64(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadUInt64(reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReadX_Int64(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadInt64(reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ReadX_Float(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadFloat(reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ReadX_Double(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadDouble(reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal ReadX_Decimal(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadDecimal(reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ReadX_Char(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadChar(reader, BinaryIoUtility._Encoding);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ReadX_Char(this BinaryReader reader, Encoding encoding)
        {
            return BinaryIoUtility.ReadChar(reader, encoding);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadX_String(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadString(reader, BinaryIoUtility._Encoding);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadX_String(this BinaryReader reader, int length)
        {
            return BinaryIoUtility.ReadString(reader, BinaryIoUtility._Encoding);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadX_String(this BinaryReader reader, int length, Encoding encoding)
        {
            return BinaryIoUtility.ReadString(reader, encoding);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadX_String(this BinaryReader reader, Encoding encoding)
        {
            return BinaryIoUtility.ReadString(reader, encoding);
        }

        // NEW!
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadCString(this BinaryReader reader, Encoding encoding)
        {
            return BinaryIoUtility.ReadCString(reader, encoding);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ReadX_NewIBinarySerializable<T>(this BinaryReader reader)
    where T : IBinarySerializable, new()
        {
            return BinaryIoUtility.ReadNewIBinarySerializable<T>(reader);
        }

#if NET_4_7_3
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum ReadX_Enum<TEnum>(this BinaryReader reader)
    where TEnum : System.Enum
        {
            return BinaryIoUtility.ReadEnum<TEnum>(reader);
        }
#endif

        #endregion

        #region ReadX Array

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool[] ReadX_BoolArray(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadBoolArray(reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ReadX_Uint8Array(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadUint8Array(reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte[] ReadX_Int8Array(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadInt8Array(reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort[] ReadX_Uint16Array(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadUint16Array(reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short[] ReadX_Int16Array(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadInt16Array(reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint[] ReadX_Uint32Array(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadUint32Array(reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int[] ReadX_Int32Array(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadInt32Array(reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong[] ReadX_Uint64Array(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadUint64Array(reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long[] ReadX_Int64Array(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadInt64Array(reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float[] ReadX_FloatArray(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadFloatArray(reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double[] ReadX_DoubleArray(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadDoubleArray(reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal[] ReadX_DecimalArray(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadDecimalArray(reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string[] ReadX_StringArray(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadStringArray(reader, BinaryIoUtility._Encoding);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string[] ReadX_StringArray(this BinaryReader reader, Encoding encoding)
        {
            return BinaryIoUtility.ReadStringArray(reader, encoding);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] ReadX_NewIBinarySerializableArray<T>(this BinaryReader reader)
    where T : IBinarySerializable, new()
        {
            return BinaryIoUtility.ReadNewIBinarySerializableArray<T>(reader);
        }

#if NET_4_7_3
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum[] ReadX_EnumArray<TEnum>(this BinaryReader reader)
    where TEnum : System.Enum
        {
            return BinaryIoUtility.ReadEnumArray<TEnum>(reader);
        }
#endif

        #endregion

        #region ReadX Array Length

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool[] ReadX_BoolArray(this BinaryReader reader, int length)
        {
            return BinaryIoUtility.ReadBoolArray(reader, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ReadX_Uint8Array(this BinaryReader reader, int length)
        {
            return BinaryIoUtility.ReadUint8Array(reader, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte[] ReadX_Int8Array(this BinaryReader reader, int length)
        {
            return BinaryIoUtility.ReadInt8Array(reader, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort[] ReadX_Uint16Array(this BinaryReader reader, int length)
        {
            return BinaryIoUtility.ReadUint16Array(reader, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short[] ReadX_Int16Array(this BinaryReader reader, int length)
        {
            return BinaryIoUtility.ReadInt16Array(reader, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint[] ReadX_Uint32Array(this BinaryReader reader, int length)
        {
            return BinaryIoUtility.ReadUint32Array(reader, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int[] ReadX_Int32Array(this BinaryReader reader, int length)
        {
            return BinaryIoUtility.ReadInt32Array(reader, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong[] ReadX_Uint64Array(this BinaryReader reader, int length)
        {
            return BinaryIoUtility.ReadUint64Array(reader, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long[] ReadX_Int64Array(this BinaryReader reader, int length)
        {
            return BinaryIoUtility.ReadInt64Array(reader, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float[] ReadX_FloatArray(this BinaryReader reader, int length)
        {
            return BinaryIoUtility.ReadFloatArray(reader, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double[] ReadX_DoubleArray(this BinaryReader reader, int length)
        {
            return BinaryIoUtility.ReadDoubleArray(reader, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal[] ReadX_DecimalArray(this BinaryReader reader, int length)
        {
            return BinaryIoUtility.ReadDecimalArray(reader, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string[] ReadX_StringArray(this BinaryReader reader, int length)
        {
            return BinaryIoUtility.ReadStringArray(reader, length, BinaryIoUtility._Encoding);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string[] ReadX_StringArray(this BinaryReader reader, int length, Encoding encoding)
        {
            return BinaryIoUtility.ReadStringArray(reader, length, encoding);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] ReadX_NewIBinarySerializable<T>(this BinaryReader reader, int length)
    where T : IBinarySerializable, new()
        {
            return BinaryIoUtility.ReadNewIBinarySerializableArray<T>(reader, length);
        }

#if NET_4_7_3
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum[] ReadX_EnumArray<TEnum>(this BinaryReader reader, int length)
    where TEnum : System.Enum
        {
            return BinaryIoUtility.ReadEnumArray<TEnum>(reader, length);
        }
#endif

        #endregion

        #region ReadX Value Ref

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReadX(this BinaryReader reader, ref bool value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ReadX(this BinaryReader reader, ref byte value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte ReadX(this BinaryReader reader, ref sbyte value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReadX(this BinaryReader reader, ref ushort value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReadX(this BinaryReader reader, ref short value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadX(this BinaryReader reader, ref uint value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadX(this BinaryReader reader, ref int value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ReadX(this BinaryReader reader, ref ulong value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReadX(this BinaryReader reader, ref long value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ReadX(this BinaryReader reader, ref float value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ReadX(this BinaryReader reader, ref double value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal ReadX(this BinaryReader reader, ref decimal value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ReadX(this BinaryReader reader, ref char value)
        {
            return BinaryIoUtility.Read(reader, ref value, BinaryIoUtility._Encoding);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ReadX(this BinaryReader reader, ref char value, Encoding encoding)
        {
            return BinaryIoUtility.Read(reader, ref value, encoding);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadX(this BinaryReader reader, ref string value)
        {
            return BinaryIoUtility.Read(reader, ref value, BinaryIoUtility._Encoding);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadX(this BinaryReader reader, ref string value, Encoding encoding)
        {
            return BinaryIoUtility.Read(reader, ref value, encoding);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadX(this BinaryReader reader, int length, ref string value)
        {
            return BinaryIoUtility.Read(reader, ref value, BinaryIoUtility._Encoding);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadX(this BinaryReader reader, int length, ref string value, Encoding encoding)
        {
            return BinaryIoUtility.Read(reader, ref value, encoding);
        }

        // NEW!
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadXCString(this BinaryReader reader, ref string value, Encoding encoding)
        {
            return value = BinaryIoUtility.ReadCString(reader, encoding);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ReadX<T>(this BinaryReader reader, ref T value, bool createNewInstance)
    where T : IBinarySerializable, new()
        {
            return value = BinaryIoUtility.Read(reader, ref value, createNewInstance);
        }

#if NET_4_7_3
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum ReadX<TEnum>(this BinaryReader reader, ref TEnum value)
    where TEnum : struct, System.Enum
        {
            return value = BinaryIoUtility.Read(reader, ref value);
        }
#endif

        #endregion

        #region ReadX Array Ref

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool[] ReadX(this BinaryReader reader, ref bool[] value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ReadX(this BinaryReader reader, ref byte[] value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte[] ReadX(this BinaryReader reader, ref sbyte[] value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort[] ReadX(this BinaryReader reader, ref ushort[] value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short[] ReadX(this BinaryReader reader, ref short[] value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint[] ReadX(this BinaryReader reader, ref uint[] value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int[] ReadX(this BinaryReader reader, ref int[] value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong[] ReadX(this BinaryReader reader, ref ulong[] value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long[] ReadX(this BinaryReader reader, ref long[] value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float[] ReadX(this BinaryReader reader, ref float[] value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double[] ReadX(this BinaryReader reader, ref double[] value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal[] ReadX(this BinaryReader reader, ref decimal[] value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string[] ReadX(this BinaryReader reader, ref string[] value)
        {
            return BinaryIoUtility.Read(reader, ref value, BinaryIoUtility._Encoding);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string[] ReadX(this BinaryReader reader, ref string[] value, Encoding encoding)
        {
            return BinaryIoUtility.Read(reader, ref value, encoding);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] ReadX<T>(this BinaryReader reader, ref T[] value, bool createNewInstances)
    where T : IBinarySerializable, new()
        {
            return BinaryIoUtility.Read(reader, ref value, createNewInstances);
        }

#if NET_4_7_3
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum[] ReadX<TEnum>(this BinaryReader reader, ref TEnum[] value)
    where TEnum : struct, System.Enum
        {
            return value = BinaryIoUtility.ReadEnum(reader, ref value);
        }
#endif

        #endregion

        #region ReadX Array Ref Length

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool[] ReadX(this BinaryReader reader, ref bool[] value, int length)
        {
            return BinaryIoUtility.Read(reader, length, ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ReadX(this BinaryReader reader, ref byte[] value, int length)
        {
            return BinaryIoUtility.Read(reader, length, ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte[] ReadX(this BinaryReader reader, ref sbyte[] value, int length)
        {
            return BinaryIoUtility.Read(reader, length, ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort[] ReadX(this BinaryReader reader, ref ushort[] value, int length)
        {
            return BinaryIoUtility.Read(reader, length, ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short[] ReadX(this BinaryReader reader, ref short[] value, int length)
        {
            return BinaryIoUtility.Read(reader, length, ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint[] ReadX(this BinaryReader reader, ref uint[] value, int length)
        {
            return BinaryIoUtility.Read(reader, length, ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int[] ReadX(this BinaryReader reader, ref int[] value, int length)
        {
            return BinaryIoUtility.Read(reader, length, ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong[] ReadX(this BinaryReader reader, ref ulong[] value, int length)
        {
            return BinaryIoUtility.Read(reader, length, ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long[] ReadX(this BinaryReader reader, ref long[] value, int length)
        {
            return BinaryIoUtility.Read(reader, length, ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float[] ReadX(this BinaryReader reader, ref float[] value, int length)
        {
            return BinaryIoUtility.Read(reader, length, ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double[] ReadX(this BinaryReader reader, ref double[] value, int length)
        {
            return BinaryIoUtility.Read(reader, length, ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal[] ReadX(this BinaryReader reader, ref decimal[] value, int length)
        {
            return BinaryIoUtility.Read(reader, length, ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string[] ReadX(this BinaryReader reader, ref string[] value, int length)
        {
            return BinaryIoUtility.Read(reader, ref value, BinaryIoUtility._Encoding);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string[] ReadX(this BinaryReader reader, ref string[] value, int length, Encoding encoding)
        {
            return BinaryIoUtility.Read(reader, ref value, encoding);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] ReadX<T>(this BinaryReader reader, ref T[] value, int length, bool createNewInstances)
    where T : IBinarySerializable, new()
        {
            return BinaryIoUtility.Read(reader, length, ref value, createNewInstances);
        }

#if NET_4_7_3
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum[] ReadX<TEnum>(this BinaryReader reader, int length, ref TEnum[] value)
    where TEnum : struct, System.Enum
        {
            return value = BinaryIoUtility.Read(reader, length, ref value);
        }
#endif

        #endregion

        #endregion

        #region WriteX

        #region WriteX Value

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteX(this BinaryWriter writer, bool value)
        {
            BinaryIoUtility.Write(writer, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteX(this BinaryWriter writer, byte value)
        {
            BinaryIoUtility.Write(writer, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteX(this BinaryWriter writer, sbyte value)
        {
            BinaryIoUtility.Write(writer, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteX(this BinaryWriter writer, ushort value)
        {
            BinaryIoUtility.Write(writer, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteX(this BinaryWriter writer, short value)
        {
            BinaryIoUtility.Write(writer, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteX(this BinaryWriter writer, uint value)
        {
            BinaryIoUtility.Write(writer, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteX(this BinaryWriter writer, int value)
        {
            BinaryIoUtility.Write(writer, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteX(this BinaryWriter writer, ulong value)
        {
            BinaryIoUtility.Write(writer, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteX(this BinaryWriter writer, long value)
        {
            BinaryIoUtility.Write(writer, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteX(this BinaryWriter writer, float value)
        {
            BinaryIoUtility.Write(writer, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteX(this BinaryWriter writer, double value)
        {
            BinaryIoUtility.Write(writer, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteX(this BinaryWriter writer, decimal value)
        {
            BinaryIoUtility.Write(writer, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteX(this BinaryWriter writer, char value)
        {
            BinaryIoUtility.Write(writer, value, BinaryIoUtility._Encoding);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteX(this BinaryWriter writer, char value, Encoding encoding)
        {
            BinaryIoUtility.Write(writer, value, encoding);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteX<T>(this BinaryWriter writer, T value)
    where T : IBinarySerializable, new()
        {
            BinaryIoUtility.Write(writer, value);
        }

#if NET_4_7_3
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteX<TEnum>(this BinaryWriter writer, TEnum value, EnumCompression enumCompression = EnumCompression.none)
    where TEnum : struct, System.Enum
        {
            BinaryIoUtility.WriteEnum(writer, value);
        }
#endif

        #endregion

        #region WriteX Array

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteX(this BinaryWriter writer, bool[] value, bool writeLengthHeader)
        {
            BinaryIoUtility.Write(writer, value, writeLengthHeader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteX(this BinaryWriter writer, byte[] value, bool writeLengthHeader)
        {
            BinaryIoUtility.Write(writer, value, writeLengthHeader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteX(this BinaryWriter writer, sbyte[] value, bool writeLengthHeader)
        {
            BinaryIoUtility.Write(writer, value, writeLengthHeader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteX(this BinaryWriter writer, ushort[] value, bool writeLengthHeader)
        {
            BinaryIoUtility.Write(writer, value, writeLengthHeader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteX(this BinaryWriter writer, short[] value, bool writeLengthHeader)
        {
            BinaryIoUtility.Write(writer, value, writeLengthHeader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteX(this BinaryWriter writer, uint[] value, bool writeLengthHeader)
        {
            BinaryIoUtility.Write(writer, value, writeLengthHeader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteX(this BinaryWriter writer, int[] value, bool writeLengthHeader)
        {
            BinaryIoUtility.Write(writer, value, writeLengthHeader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteX(this BinaryWriter writer, ulong[] value, bool writeLengthHeader)
        {
            BinaryIoUtility.Write(writer, value, writeLengthHeader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteX(this BinaryWriter writer, long[] value, bool writeLengthHeader)
        {
            BinaryIoUtility.Write(writer, value, writeLengthHeader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteX(this BinaryWriter writer, float[] value, bool writeLengthHeader)
        {
            BinaryIoUtility.Write(writer, value, writeLengthHeader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteX(this BinaryWriter writer, double[] value, bool writeLengthHeader)
        {
            BinaryIoUtility.Write(writer, value, writeLengthHeader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteX(this BinaryWriter writer, decimal[] value, bool writeLengthHeader)
        {
            BinaryIoUtility.Write(writer, value, writeLengthHeader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteX(this BinaryWriter writer, string value, bool writeLengthHeader)
        {
            BinaryIoUtility.Write(writer, value, BinaryIoUtility._Encoding, writeLengthHeader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteX(this BinaryWriter writer, string value, Encoding encoding, bool writeLengthHeader)
        {
            BinaryIoUtility.Write(writer, value, encoding, writeLengthHeader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteX(this BinaryWriter writer, string[] value, bool writeLengthHeader)
        {
            BinaryIoUtility.Write(writer, value, BinaryIoUtility._Encoding, writeLengthHeader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteX(this BinaryWriter writer, string[] value, Encoding encoding, bool writeLengthHeader)
        {
            BinaryIoUtility.Write(writer, value, encoding, writeLengthHeader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteX<T>(this BinaryWriter writer, T[] value, bool writeLengthHeader)
    where T : IBinarySerializable, new()
        {
            BinaryIoUtility.Write(writer, value, writeLengthHeader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteXCString(this BinaryWriter writer, string value, Encoding encoding)
        {
            BinaryIoUtility.WriteCString(writer, value, encoding);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteXCString(this BinaryWriter writer, string value)
        {
            BinaryIoUtility.WriteCString(writer, value);
        }

#if NET_4_7_3
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteE<TEnum>(this BinaryWriter writer, TEnum[] value, bool writeLengthHeader)
    where TEnum : System.Enum
        {
            BinaryIoUtility.WriteEnum(writer, value, writeLengthHeader);
        }
#endif

        #endregion

        #endregion
    }
}