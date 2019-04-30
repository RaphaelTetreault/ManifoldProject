using System.IO;
using System.Text;

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
            var bytesToAlign = reader.BaseStream.Position % alignment;
            reader.BaseStream.Seek(bytesToAlign, SeekOrigin.Current);
            return reader.BaseStream.Position;
        }

        public static long Align(this BinaryWriter writer, long alignment)
        {
            var bytesToAlign = alignment - (writer.BaseStream.Position % alignment);
            for (int i = 0; i < bytesToAlign; i++)
                writer.Write((byte)0x00);

            return writer.BaseStream.Position;
        }

        #region ReadX

        #region ReadX Value

        public static bool ReadX_Bool(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadBool(reader);
        }

        public static byte ReadX_UInt8(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadUInt8(reader);
        }

        public static sbyte ReadX_Int8(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadInt8(reader);
        }

        public static ushort ReadX_UInt16(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadUInt16(reader);
        }

        public static short ReadX_Int16(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadInt16(reader);
        }

        public static uint ReadX_UInt32(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadUInt32(reader);
        }

        public static int ReadX_Int32(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadInt32(reader);
        }

        public static ulong ReadX_UInt64(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadUInt64(reader);
        }

        public static long ReadX_Int64(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadInt64(reader);
        }

        public static float ReadX_Float(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadFloat(reader);
        }

        public static double ReadX_Double(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadDouble(reader);
        }

        public static decimal ReadX_Decimal(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadDecimal(reader);
        }

        public static char ReadX_Char(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadChar(reader, BinaryIoUtility._Encoding);
        }

        public static char ReadX_Char(this BinaryReader reader, Encoding encoding)
        {
            return BinaryIoUtility.ReadChar(reader, encoding);
        }

        public static string ReadX_String(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadString(reader, BinaryIoUtility._Encoding);
        }

        public static string ReadX_String(this BinaryReader reader, int length)
        {
            return BinaryIoUtility.ReadString(reader, BinaryIoUtility._Encoding);
        }

        public static string ReadX_String(this BinaryReader reader, int length, Encoding encoding)
        {
            return BinaryIoUtility.ReadString(reader, encoding);
        }

        public static string ReadX_String(this BinaryReader reader, Encoding encoding)
        {
            return BinaryIoUtility.ReadString(reader, encoding);
        }

        // NEW!
        public static string ReadCString(this BinaryReader reader, Encoding encoding)
        {
            return BinaryIoUtility.ReadCString(reader, encoding);
        }

        public static T ReadX_NewIBinarySerializable<T>(this BinaryReader reader)
            where T : IBinarySerializable, new()
        {
            return BinaryIoUtility.ReadNewIBinarySerializable<T>(reader);
        }

#if NET_4_7_3
        public static TEnum ReadX_Enum<TEnum>(this BinaryReader reader)
            where TEnum : System.Enum
        {
            return BinaryIoUtility.ReadEnum<TEnum>(reader);
        }
#endif

        #endregion

        #region ReadX Array

        public static bool[] ReadX_BoolArray(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadBoolArray(reader);
        }

        public static byte[] ReadX_Uint8Array(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadUint8Array(reader);
        }

        public static sbyte[] ReadX_Int8Array(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadInt8Array(reader);
        }

        public static ushort[] ReadX_Uint16Array(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadUint16Array(reader);
        }

        public static short[] ReadX_Int16Array(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadInt16Array(reader);
        }

        public static uint[] ReadX_Uint32Array(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadUint32Array(reader);
        }

        public static int[] ReadX_Int32Array(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadInt32Array(reader);
        }

        public static ulong[] ReadX_Uint64Array(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadUint64Array(reader);
        }

        public static long[] ReadX_Int64Array(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadInt64Array(reader);
        }

        public static float[] ReadX_FloatArray(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadFloatArray(reader);
        }

        public static double[] ReadX_DoubleArray(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadDoubleArray(reader);
        }

        public static decimal[] ReadX_DecimalArray(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadDecimalArray(reader);
        }

        public static string[] ReadX_StringArray(this BinaryReader reader)
        {
            return BinaryIoUtility.ReadStringArray(reader, BinaryIoUtility._Encoding);
        }

        public static string[] ReadX_StringArray(this BinaryReader reader, Encoding encoding)
        {
            return BinaryIoUtility.ReadStringArray(reader, encoding);
        }

        public static T[] ReadX_NewIBinarySerializableArray<T>(this BinaryReader reader)
            where T : IBinarySerializable, new()
        {
            return BinaryIoUtility.ReadNewIBinarySerializableArray<T>(reader);
        }

#if NET_4_7_3
        public static TEnum[] ReadX_EnumArray<TEnum>(this BinaryReader reader)
            where TEnum : System.Enum
        {
            return BinaryIoUtility.ReadEnumArray<TEnum>(reader);
        }
#endif

        #endregion

        #region ReadX Array Length

        public static bool[] ReadX_BoolArray(this BinaryReader reader, int length)
        {
            return BinaryIoUtility.ReadBoolArray(reader, length);
        }

        public static byte[] ReadX_Uint8Array(this BinaryReader reader, int length)
        {
            return BinaryIoUtility.ReadUint8Array(reader, length);
        }

        public static sbyte[] ReadX_Int8Array(this BinaryReader reader, int length)
        {
            return BinaryIoUtility.ReadInt8Array(reader, length);
        }

        public static ushort[] ReadX_Uint16Array(this BinaryReader reader, int length)
        {
            return BinaryIoUtility.ReadUint16Array(reader, length);
        }

        public static short[] ReadX_Int16Array(this BinaryReader reader, int length)
        {
            return BinaryIoUtility.ReadInt16Array(reader, length);
        }

        public static uint[] ReadX_Uint32Array(this BinaryReader reader, int length)
        {
            return BinaryIoUtility.ReadUint32Array(reader, length);
        }

        public static int[] ReadX_Int32Array(this BinaryReader reader, int length)
        {
            return BinaryIoUtility.ReadInt32Array(reader, length);
        }

        public static ulong[] ReadX_Uint64Array(this BinaryReader reader, int length)
        {
            return BinaryIoUtility.ReadUint64Array(reader, length);
        }

        public static long[] ReadX_Int64Array(this BinaryReader reader, int length)
        {
            return BinaryIoUtility.ReadInt64Array(reader, length);
        }

        public static float[] ReadX_FloatArray(this BinaryReader reader, int length)
        {
            return BinaryIoUtility.ReadFloatArray(reader, length);
        }

        public static double[] ReadX_DoubleArray(this BinaryReader reader, int length)
        {
            return BinaryIoUtility.ReadDoubleArray(reader, length);
        }

        public static decimal[] ReadX_DecimalArray(this BinaryReader reader, int length)
        {
            return BinaryIoUtility.ReadDecimalArray(reader, length);
        }

        public static string[] ReadX_StringArray(this BinaryReader reader, int length)
        {
            return BinaryIoUtility.ReadStringArray(reader, length, BinaryIoUtility._Encoding);
        }

        public static string[] ReadX_StringArray(this BinaryReader reader, int length, Encoding encoding)
        {
            return BinaryIoUtility.ReadStringArray(reader, length, encoding);
        }

        public static T[] ReadX_NewIBinarySerializable<T>(this BinaryReader reader, int length)
            where T : IBinarySerializable, new()
        {
            return BinaryIoUtility.ReadNewIBinarySerializableArray<T>(reader, length);
        }

#if NET_4_7_3
        public static TEnum[] ReadX_EnumArray<TEnum>(this BinaryReader reader, int length)
            where TEnum : System.Enum
        {
            return BinaryIoUtility.ReadEnumArray<TEnum>(reader, length);
        }
#endif

        #endregion

        #region ReadX Value Ref

        public static bool ReadX(this BinaryReader reader, ref bool value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        public static byte ReadX(this BinaryReader reader, ref byte value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        public static sbyte ReadX(this BinaryReader reader, ref sbyte value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        public static ushort ReadX(this BinaryReader reader, ref ushort value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        public static short ReadX(this BinaryReader reader, ref short value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        public static uint ReadX(this BinaryReader reader, ref uint value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        public static int ReadX(this BinaryReader reader, ref int value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        public static ulong ReadX(this BinaryReader reader, ref ulong value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        public static long ReadX(this BinaryReader reader, ref long value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        public static float ReadX(this BinaryReader reader, ref float value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        public static double ReadX(this BinaryReader reader, ref double value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        public static decimal ReadX(this BinaryReader reader, ref decimal value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        public static char ReadX(this BinaryReader reader, ref char value)
        {
            return BinaryIoUtility.Read(reader, ref value, BinaryIoUtility._Encoding);
        }

        public static char ReadX(this BinaryReader reader, ref char value, Encoding encoding)
        {
            return BinaryIoUtility.Read(reader, ref value, encoding);
        }

        public static string ReadX(this BinaryReader reader, ref string value)
        {
            return BinaryIoUtility.Read(reader, ref value, BinaryIoUtility._Encoding);
        }

        public static string ReadX(this BinaryReader reader, ref string value, Encoding encoding)
        {
            return BinaryIoUtility.Read(reader, ref value, encoding);
        }

        public static string ReadX(this BinaryReader reader, int length, ref string value)
        {
            return BinaryIoUtility.Read(reader, ref value, BinaryIoUtility._Encoding);
        }

        public static string ReadX(this BinaryReader reader, int length, ref string value, Encoding encoding)
        {
            return BinaryIoUtility.Read(reader, ref value, encoding);
        }

        // NEW!
        public static string ReadXCString(this BinaryReader reader,ref string value, Encoding encoding)
        {
            return value = BinaryIoUtility.ReadCString(reader, encoding);
        }

        public static T ReadX<T>(this BinaryReader reader, ref T value, bool createNewInstance)
            where T : IBinarySerializable, new()
        {
            return value = BinaryIoUtility.Read(reader, ref value, createNewInstance);
        }

#if NET_4_7_3
        public static TEnum ReadX<TEnum>(this BinaryReader reader, ref TEnum value)
            where TEnum : struct, System.Enum
        {
            return value = BinaryIoUtility.Read(reader, ref value);
        }
#endif

        #endregion

        #region ReadX Array Ref

        public static bool[] ReadX(this BinaryReader reader, ref bool[] value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        public static byte[] ReadX(this BinaryReader reader, ref byte[] value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        public static sbyte[] ReadX(this BinaryReader reader, ref sbyte[] value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        public static ushort[] ReadX(this BinaryReader reader, ref ushort[] value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        public static short[] ReadX(this BinaryReader reader, ref short[] value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        public static uint[] ReadX(this BinaryReader reader, ref uint[] value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        public static int[] ReadX(this BinaryReader reader, ref int[] value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        public static ulong[] ReadX(this BinaryReader reader, ref ulong[] value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        public static long[] ReadX(this BinaryReader reader, ref long[] value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        public static float[] ReadX(this BinaryReader reader, ref float[] value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        public static double[] ReadX(this BinaryReader reader, ref double[] value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        public static decimal[] ReadX(this BinaryReader reader, ref decimal[] value)
        {
            return BinaryIoUtility.Read(reader, ref value);
        }

        public static string[] ReadX(this BinaryReader reader, ref string[] value)
        {
            return BinaryIoUtility.Read(reader, ref value, BinaryIoUtility._Encoding);
        }

        public static string[] ReadX(this BinaryReader reader, ref string[] value, Encoding encoding)
        {
            return BinaryIoUtility.Read(reader, ref value, encoding);
        }

        public static T[] ReadX<T>(this BinaryReader reader, ref T[] value, bool createNewInstances)
            where T : IBinarySerializable, new()
        {
            return BinaryIoUtility.Read(reader, ref value, createNewInstances);
        }

#if NET_4_7_3
        public static TEnum[] ReadX<TEnum>(this BinaryReader reader, ref TEnum[] value)
            where TEnum : struct, System.Enum
        {
            return value = BinaryIoUtility.ReadEnum(reader, ref value);
        }
#endif

        #endregion

        #region ReadX Array Ref Length

        public static bool[] ReadX(this BinaryReader reader, ref bool[] value, int length)
        {
            return BinaryIoUtility.Read(reader, length, ref value);
        }

        public static byte[] ReadX(this BinaryReader reader, ref byte[] value, int length)
        {
            return BinaryIoUtility.Read(reader, length, ref value);
        }

        public static sbyte[] ReadX(this BinaryReader reader, ref sbyte[] value, int length)
        {
            return BinaryIoUtility.Read(reader, length, ref value);
        }

        public static ushort[] ReadX(this BinaryReader reader, ref ushort[] value, int length)
        {
            return BinaryIoUtility.Read(reader, length, ref value);
        }

        public static short[] ReadX(this BinaryReader reader, ref short[] value, int length)
        {
            return BinaryIoUtility.Read(reader, length, ref value);
        }

        public static uint[] ReadX(this BinaryReader reader, ref uint[] value, int length)
        {
            return BinaryIoUtility.Read(reader, length, ref value);
        }

        public static int[] ReadX(this BinaryReader reader, ref int[] value, int length)
        {
            return BinaryIoUtility.Read(reader, length, ref value);
        }

        public static ulong[] ReadX(this BinaryReader reader, ref ulong[] value, int length)
        {
            return BinaryIoUtility.Read(reader, length, ref value);
        }

        public static long[] ReadX(this BinaryReader reader, ref long[] value, int length)
        {
            return BinaryIoUtility.Read(reader, length, ref value);
        }

        public static float[] ReadX(this BinaryReader reader, ref float[] value, int length)
        {
            return BinaryIoUtility.Read(reader, length, ref value);
        }

        public static double[] ReadX(this BinaryReader reader, ref double[] value, int length)
        {
            return BinaryIoUtility.Read(reader, length, ref value);
        }

        public static decimal[] ReadX(this BinaryReader reader, ref decimal[] value, int length)
        {
            return BinaryIoUtility.Read(reader, length, ref value);
        }

        public static string[] ReadX(this BinaryReader reader, ref string[] value, int length)
        {
            return BinaryIoUtility.Read(reader, ref value, BinaryIoUtility._Encoding);
        }

        public static string[] ReadX(this BinaryReader reader, ref string[] value, int length, Encoding encoding)
        {
            return BinaryIoUtility.Read(reader, ref value, encoding);
        }

        public static T[] ReadX<T>(this BinaryReader reader, ref T[] value, int length, bool createNewInstances)
            where T : IBinarySerializable, new()
        {
            return BinaryIoUtility.Read(reader, length, ref value, createNewInstances);
        }

#if NET_4_7_3
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

        public static void WriteX(this BinaryWriter writer, bool value)
        {
            BinaryIoUtility.Write(writer, value);
        }

        public static void WriteX(this BinaryWriter writer, byte value)
        {
            BinaryIoUtility.Write(writer, value);
        }

        public static void WriteX(this BinaryWriter writer, sbyte value)
        {
            BinaryIoUtility.Write(writer, value);
        }

        public static void WriteX(this BinaryWriter writer, ushort value)
        {
            BinaryIoUtility.Write(writer, value);
        }

        public static void WriteX(this BinaryWriter writer, short value)
        {
            BinaryIoUtility.Write(writer, value);
        }

        public static void WriteX(this BinaryWriter writer, uint value)
        {
            BinaryIoUtility.Write(writer, value);
        }

        public static void WriteX(this BinaryWriter writer, int value)
        {
            BinaryIoUtility.Write(writer, value);
        }

        public static void WriteX(this BinaryWriter writer, ulong value)
        {
            BinaryIoUtility.Write(writer, value);
        }

        public static void WriteX(this BinaryWriter writer, long value)
        {
            BinaryIoUtility.Write(writer, value);
        }

        public static void WriteX(this BinaryWriter writer, float value)
        {
            BinaryIoUtility.Write(writer, value);
        }

        public static void WriteX(this BinaryWriter writer, double value)
        {
            BinaryIoUtility.Write(writer, value);
        }

        public static void WriteX(this BinaryWriter writer, decimal value)
        {
            BinaryIoUtility.Write(writer, value);
        }

        public static void WriteX(this BinaryWriter writer, char value)
        {
            BinaryIoUtility.Write(writer, value, BinaryIoUtility._Encoding);
        }

        public static void WriteX(this BinaryWriter writer, char value, Encoding encoding)
        {
            BinaryIoUtility.Write(writer, value, encoding);
        }

        public static void WriteX<T>(this BinaryWriter writer, T value)
            where T : IBinarySerializable, new()
        {
            BinaryIoUtility.Write(writer, value);
        }

#if NET_4_7_3
        public static void WriteX<TEnum>(this BinaryWriter writer, TEnum value, EnumCompression enumCompression = EnumCompression.none)
            where TEnum : struct, System.Enum
        {
            BinaryIoUtility.WriteEnum(writer, value);
        }
#endif

        #endregion

        #region WriteX Array

        public static void WriteX(this BinaryWriter writer, bool[] value, bool writeLengthHeader)
        {
            BinaryIoUtility.Write(writer, value, writeLengthHeader);
        }

        public static void WriteX(this BinaryWriter writer, byte[] value, bool writeLengthHeader)
        {
            BinaryIoUtility.Write(writer, value, writeLengthHeader);
        }

        public static void WriteX(this BinaryWriter writer, sbyte[] value, bool writeLengthHeader)
        {
            BinaryIoUtility.Write(writer, value, writeLengthHeader);
        }

        public static void WriteX(this BinaryWriter writer, ushort[] value, bool writeLengthHeader)
        {
            BinaryIoUtility.Write(writer, value, writeLengthHeader);
        }

        public static void WriteX(this BinaryWriter writer, short[] value, bool writeLengthHeader)
        {
            BinaryIoUtility.Write(writer, value, writeLengthHeader);
        }

        public static void WriteX(this BinaryWriter writer, uint[] value, bool writeLengthHeader)
        {
            BinaryIoUtility.Write(writer, value, writeLengthHeader);
        }

        public static void WriteX(this BinaryWriter writer, int[] value, bool writeLengthHeader)
        {
            BinaryIoUtility.Write(writer, value, writeLengthHeader);
        }

        public static void WriteX(this BinaryWriter writer, ulong[] value, bool writeLengthHeader)
        {
            BinaryIoUtility.Write(writer, value, writeLengthHeader);
        }

        public static void WriteX(this BinaryWriter writer, long[] value, bool writeLengthHeader)
        {
            BinaryIoUtility.Write(writer, value, writeLengthHeader);
        }

        public static void WriteX(this BinaryWriter writer, float[] value, bool writeLengthHeader)
        {
            BinaryIoUtility.Write(writer, value, writeLengthHeader);
        }

        public static void WriteX(this BinaryWriter writer, double[] value, bool writeLengthHeader)
        {
            BinaryIoUtility.Write(writer, value, writeLengthHeader);
        }

        public static void WriteX(this BinaryWriter writer, decimal[] value, bool writeLengthHeader)
        {
            BinaryIoUtility.Write(writer, value, writeLengthHeader);
        }

        public static void WriteX(this BinaryWriter writer, string value, bool writeLengthHeader)
        {
            BinaryIoUtility.Write(writer, value, BinaryIoUtility._Encoding, writeLengthHeader);
        }

        public static void WriteX(this BinaryWriter writer, string value, Encoding encoding, bool writeLengthHeader)
        {
            BinaryIoUtility.Write(writer, value, encoding, writeLengthHeader);
        }

        public static void WriteX(this BinaryWriter writer, string[] value, bool writeLengthHeader)
        {
            BinaryIoUtility.Write(writer, value, BinaryIoUtility._Encoding, writeLengthHeader);
        }

        public static void WriteX(this BinaryWriter writer, string[] value, Encoding encoding, bool writeLengthHeader)
        {
            BinaryIoUtility.Write(writer, value, encoding, writeLengthHeader);
        }

        public static void WriteX<T>(this BinaryWriter writer, T[] value, bool writeLengthHeader)
            where T : IBinarySerializable, new()
        {
            BinaryIoUtility.Write(writer, value, writeLengthHeader);
        }

        public static void WriteXCString(this BinaryWriter writer, string value, Encoding encoding)
        {
            BinaryIoUtility.WriteCString(writer, value, encoding);
        }

        public static void WriteXCString(this BinaryWriter writer, string value)
        {
            BinaryIoUtility.WriteCString(writer, value);
        }

#if NET_4_7_3
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