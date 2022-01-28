using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Manifold.IO
{
    public static class BinaryIoUtility
    {
        #region CONSTS

        public const int SizeofBool = 1;
        public const int SizeofInt8 = 1;
        public const int SizeofInt16 = 2;
        public const int SizeofInt32 = 4;
        public const int SizeofInt64 = 8;
        public const int SizeofUint8 = 1;
        public const int SizeofUint16 = 2;
        public const int SizeofUint32 = 4;
        public const int SizeofUint64 = 8;
        public const int SizeofFloat = 4;
        public const int SizeofDouble = 8;
        public const int SizeofDecimal = 16;

        #endregion

        #region FIELDS

        private static readonly Stack<Encoding> _encodingStack = new Stack<Encoding>();
        private static readonly Stack<bool> _endianessStack = new Stack<bool>();

        #endregion

        #region PROPERTIES

        /// <summary>
        /// The stride used to align the stream to when calling AlignTo method
        /// </summary>
        public static int ByteAlignment { get; set; } = 4;

        /// <summary>
        /// The current endianness used for read/write operations
        /// </summary>
        public static bool IsLittleEndian { get; set; } = false;

        /// <summary>
        /// The current encoding used for read/write operations
        /// </summary>
        public static Encoding Encoding { get; set; } = Encoding.Unicode;

        #endregion

        #region METHODS

        /// <summary>
        /// Pops the last pushed System.Text.Encoding
        /// </summary>
        public static void PopEncoding()
        {
            Encoding = _encodingStack.Pop();
        }

        /// <summary>
        /// Pops the last pushed endianness
        /// </summary>
        public static void PopEndianess()
        {
            IsLittleEndian = _endianessStack.Pop();
        }

        /// <summary>
        /// Pushes a System.Text.Encoding to a private stack. Subsequent calls to read
        /// or write strings will use this encoding.
        /// </summary>
        /// <param name="encoding"></param>
        public static void PushEncoding(Encoding encoding)
        {
            _encodingStack.Push(Encoding);
            Encoding = encoding;
        }

        /// <summary>
        /// Pushes an endianness to a private stack. Subsequent calls to read
        /// or write will use this endianness.
        /// </summary>
        /// <param name="isLittleEndian"></param>
        public static void PushEndianess(bool isLittleEndian)
        {
            _endianessStack.Push(IsLittleEndian);
            IsLittleEndian = isLittleEndian;
        }

        /// <summary>
        /// Pushes an endianness to a private stack. Subsequent calls to read
        /// or write will use this endianness.
        /// </summary>
        /// <param name="isLittleEndian"></param>
        public static void PushEndianness(Endianness endianness)
        {
            _endianessStack.Push(IsLittleEndian);

            var value = endianness == Endianness.LittleEndian;
            IsLittleEndian = value;
        }

        #endregion

        #region READ

        #region Read Value

        public static bool ReadBool(BinaryReader binaryReader)
        {
            return binaryReader.ReadBoolean();
        }

        public static byte ReadUInt8(BinaryReader binaryReader)
        {
            return binaryReader.ReadByte();
        }

        public static sbyte ReadInt8(BinaryReader binaryReader)
        {
            return binaryReader.ReadSByte();
        }

        public static short ReadInt16(BinaryReader binaryReader)
        {
            byte[] bytes = binaryReader.ReadBytes(SizeofInt16);

            if (BitConverter.IsLittleEndian ^ IsLittleEndian)
                Array.Reverse(bytes);

            return BitConverter.ToInt16(bytes, 0);
        }

        public static ushort ReadUInt16(BinaryReader binaryReader)
        {
            byte[] bytes = binaryReader.ReadBytes(SizeofUint16);

            if (BitConverter.IsLittleEndian ^ IsLittleEndian)
                Array.Reverse(bytes);

            return BitConverter.ToUInt16(bytes, 0);
        }

        public static int ReadInt32(BinaryReader binaryReader)
        {
            byte[] bytes = binaryReader.ReadBytes(SizeofInt32);

            if (BitConverter.IsLittleEndian ^ IsLittleEndian)
                Array.Reverse(bytes);

            return BitConverter.ToInt32(bytes, 0);
        }

        public static uint ReadUInt32(BinaryReader binaryReader)
        {
            byte[] bytes = binaryReader.ReadBytes(SizeofUint32);

            if (BitConverter.IsLittleEndian ^ IsLittleEndian)
                Array.Reverse(bytes);

            return BitConverter.ToUInt32(bytes, 0);
        }

        public static long ReadInt64(BinaryReader binaryReader)
        {
            byte[] bytes = binaryReader.ReadBytes(SizeofInt64);

            if (BitConverter.IsLittleEndian ^ IsLittleEndian)
                Array.Reverse(bytes);

            return BitConverter.ToInt64(bytes, 0);
        }

        public static ulong ReadUInt64(BinaryReader binaryReader)
        {
            byte[] bytes = binaryReader.ReadBytes(SizeofUint64);

            if (BitConverter.IsLittleEndian ^ IsLittleEndian)
                Array.Reverse(bytes);

            return BitConverter.ToUInt64(bytes, 0);
        }

        public static float ReadFloat(BinaryReader binaryReader)
        {
            byte[] bytes = binaryReader.ReadBytes(SizeofFloat);

            if (BitConverter.IsLittleEndian ^ IsLittleEndian)
                Array.Reverse(bytes);

            return BitConverter.ToSingle(bytes, 0);
        }

        public static double ReadDouble(BinaryReader binaryReader)
        {
            byte[] bytes = binaryReader.ReadBytes(SizeofDouble);

            if (BitConverter.IsLittleEndian ^ IsLittleEndian)
                Array.Reverse(bytes);

            return BitConverter.ToDouble(bytes, 0);
        }

        public static decimal ReadDecimal(BinaryReader binaryReader)
        {
            bool isLittleEndian = binaryReader.ReadBoolean();
            byte[] bytes = binaryReader.ReadBytes(SizeofDecimal);

            if (BitConverter.IsLittleEndian ^ isLittleEndian)
                Array.Reverse(bytes);

            // Merge 4 bytes into 1 int, then 4 ints into 1 decimal
            return new decimal(new int[]
            {
                BitConverter.ToInt32(bytes, 0),
                BitConverter.ToInt32(bytes, 4),
                BitConverter.ToInt32(bytes, 8),
                BitConverter.ToInt32(bytes, 12),
            });
        }

        public static char ReadChar(BinaryReader binaryReader, Encoding encoding)
        {
            int lengthOfChar = encoding.IsSingleByte ? 1 : 2;
            byte[] bytes = binaryReader.ReadBytes(lengthOfChar);

            if (lengthOfChar > 1)
            {
                // 2 bytes
                if (BitConverter.IsLittleEndian ^ IsLittleEndian)
                    Array.Reverse(bytes);
            }
            else
            {
                // Create LittleEndian array as char is 2 bytes in C#
                bytes = new byte[] { 0, bytes[0] };

                if (BitConverter.IsLittleEndian ^ IsLittleEndian)
                    Array.Reverse(bytes);
            }

            return BitConverter.ToChar(bytes, 0);
        }

        public static char ReadChar(BinaryReader binaryReader)
        {
            return ReadChar(binaryReader, Encoding);
        }

        public static string ReadString(BinaryReader binaryReader, int length, Encoding encoding)
        {
            char[] value = new char[length];

            for (int i = 0; i < length; i++)
            {
                value[i] = ReadChar(binaryReader, encoding);
            }

            return new string(value);
        }

        public static string ReadString(BinaryReader binaryReader, int length)
        {
            return ReadString(binaryReader, length, Encoding);
        }

        public static string ReadString(BinaryReader binaryReader, Encoding encoding)
        {
            int length = ReadInt32(binaryReader);
            return ReadString(binaryReader, length, encoding);
        }

        public static string ReadString(BinaryReader binaryReader)
        {
            int length = ReadInt32(binaryReader);
            return ReadString(binaryReader, length, Encoding);
        }

        // TODO
        //public static string ReadCString(BinaryReader binaryReader, Encoding encoding)
        //{
        //    var value = new StringBuilder();
        //    char c;
        //    while ((c = ReadChar(binaryReader, encoding)) != (char)0 && !binaryReader.IsAtEndOfStream())
        //    {
        //        value.Append(c);
        //    }
        //    return value.ToString();
        //}

        public static T ReadNewIBinarySerializable<T>(BinaryReader binaryReader) where T : IBinarySerializable, new()
        {
            T value = new T();
            value.Deserialize(binaryReader);

            return value;
        }

        // NEW!
        // EXCEPTION: non-destructive, load values from stream but doesn't make a new reference

        public static T ReadIBinarySerializable<T>(BinaryReader binaryReader, T value) where T : IBinarySerializable
        {
            value.Deserialize(binaryReader);
            return value;
        }

        /// <summary>
        /// TODO: Errors: InvalidCastError when enum doesn't use proper type (enum : ushort) uses EC.int
        /// </summary>

        public static TEnum ReadEnum<TEnum>(BinaryReader binaryReader) where TEnum : Enum
        {
            var type = Enum.GetUnderlyingType(typeof(TEnum));

            // Uncomment for when C# 9.0 can be used in Unity
            //switch (type)
            //{
            //    // Signed backing type
            //    case sbyte:     return (TEnum)(object)ReadInt8(binaryReader);
            //    case short:     return (TEnum)(object)ReadInt16(binaryReader);
            //    case int:       return (TEnum)(object)ReadInt32(binaryReader);
            //    case long:      return (TEnum)(object)ReadInt64(binaryReader);
            //    // Unsigned backing type
            //    case byte:      return (TEnum)(object)ReadUInt8(binaryReader);
            //    case ushort:    return (TEnum)(object)ReadUInt16(binaryReader);
            //    case uint:      return (TEnum)(object)ReadUInt32(binaryReader);
            //    case ulong:     return (TEnum)(object)ReadUInt64(binaryReader);
            //    
            //    default: throw new NotImplementedException();
            //}

            if (type == typeof(int))
            {
                int value = ReadInt32(binaryReader);
                return (TEnum)(object)value;
            }
            else if (type == typeof(uint))
            {
                uint value = ReadUInt32(binaryReader);
                return (TEnum)(object)value;
            }
            else if (type == typeof(short))
            {
                short value = ReadInt16(binaryReader);
                return (TEnum)(object)value;
            }
            else if (type == typeof(ushort))
            {
                ushort value = ReadUInt16(binaryReader);
                return (TEnum)(object)value;
            }
            else if (type == typeof(sbyte))
            {
                sbyte value = ReadInt8(binaryReader);
                return (TEnum)(object)value;
            }
            else if (type == typeof(byte))
            {
                byte value = ReadUInt8(binaryReader);
                return (TEnum)(object)value;
            }
            else if (type == typeof(long))
            {
                long value = ReadInt64(binaryReader);
                return (TEnum)(object)value;
            }
            else if (type == typeof(ulong))
            {
                ulong value = ReadUInt64(binaryReader);
                return (TEnum)(object)value;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region Read Ref

        public static bool Read(BinaryReader binaryReader, ref bool value)
        {
            // Not referencing own code for performance
            return value = binaryReader.ReadBoolean();
        }

        public static byte Read(BinaryReader binaryReader, ref byte value)
        {
            // Optimized
            return value = ReadUInt8(binaryReader);
        }

        public static sbyte Read(BinaryReader binaryReader, ref sbyte value)
        {
            // Optimized
            return value = binaryReader.ReadSByte();
        }

        public static short Read(BinaryReader binaryReader, ref short value)
        {
            return value = ReadInt16(binaryReader);
        }

        public static ushort Read(BinaryReader binaryReader, ref ushort value)
        {
            return value = ReadUInt16(binaryReader);
        }

        public static int Read(BinaryReader binaryReader, ref int value)
        {
            return value = ReadInt32(binaryReader);
        }

        public static uint Read(BinaryReader binaryReader, ref uint value)
        {
            return value = ReadUInt32(binaryReader);
        }

        public static long Read(BinaryReader binaryReader, ref long value)
        {
            return value = ReadInt64(binaryReader);
        }

        public static ulong Read(BinaryReader binaryReader, ref ulong value)
        {
            return value = ReadUInt64(binaryReader);
        }

        public static float Read(BinaryReader binaryReader, ref float value)
        {
            return value = ReadFloat(binaryReader);
        }

        public static double Read(BinaryReader binaryReader, ref double value)
        {
            return value = ReadDouble(binaryReader);
        }

        public static decimal Read(BinaryReader binaryReader, ref decimal value)
        {
            return value = ReadDecimal(binaryReader);
        }

        public static char Read(BinaryReader binaryReader, ref char value, Encoding encoding)
        {
            return value = ReadChar(binaryReader, encoding);
        }

        public static char Read(BinaryReader binaryReader, ref char value)
        {
            return value = ReadChar(binaryReader, Encoding);
        }

        public static string Read(BinaryReader binaryReader, ref string value, int length, Encoding encoding)
        {
            return value = ReadString(binaryReader, length, encoding);
        }

        public static string Read(BinaryReader binaryReader, ref string value, int length)
        {
            return value = ReadString(binaryReader, length, Encoding);
        }

        public static string Read(BinaryReader binaryReader, ref string value, Encoding encoding)
        {
            int length = ReadInt32(binaryReader);
            return value = ReadString(binaryReader, length, encoding);
        }

        public static string Read(BinaryReader binaryReader, ref string value)
        {
            int length = ReadInt32(binaryReader);
            return value = ReadString(binaryReader, length, Encoding);
        }

        // TODO
        public static T Read<T>(BinaryReader binaryReader, ref T value, bool createNewInstance) where T : IBinarySerializable, new()
        {
            if (createNewInstance)
                return value = ReadNewIBinarySerializable<T>(binaryReader);
            else
                return value = ReadIBinarySerializable<T>(binaryReader, value);
        }

        public static TEnum Read<TEnum>(BinaryReader binaryReader, ref TEnum value) where TEnum : Enum
        {
            return value = ReadEnum<TEnum>(binaryReader);
        }

        #endregion

        public static T[] ReadNewArray<T>(BinaryReader binaryReader, int length, Func<BinaryReader, T> method)
        {
            T[] array = new T[length];

            for (int i = 0; i < array.Length; ++i)
                array[i] = method(binaryReader);

            return array;
        }

        public static T[] ReadArray<T>(BinaryReader binaryReader, Func<BinaryReader, T, T> method, T[] array)
        {
            for (int i = 0; i < array.Length; ++i)
                array[i] = method(binaryReader, array[i]);

            return array;
        }

        #region Read Array Length

        public static bool[] ReadBoolArray(BinaryReader binaryReader, int length)
        {
            return ReadNewArray(binaryReader, length, ReadBool);
        }

        public static byte[] ReadUint8Array(BinaryReader binaryReader, int length)
        {
            return binaryReader.ReadBytes(length);
        }

        public static sbyte[] ReadInt8Array(BinaryReader binaryReader, int length)
        {
            return ReadNewArray(binaryReader, length, ReadInt8);
        }

        public static short[] ReadInt16Array(BinaryReader binaryReader, int length)
        {
            return ReadNewArray(binaryReader, length, ReadInt16);
        }

        public static ushort[] ReadUint16Array(BinaryReader binaryReader, int length)
        {
            return ReadNewArray(binaryReader, length, ReadUInt16);
        }

        public static int[] ReadInt32Array(BinaryReader binaryReader, int length)
        {
            return ReadNewArray(binaryReader, length, ReadInt32);
        }

        public static uint[] ReadUint32Array(BinaryReader binaryReader, int length)
        {
            return ReadNewArray(binaryReader, length, ReadUInt32);
        }

        public static long[] ReadInt64Array(BinaryReader binaryReader, int length)
        {
            return ReadNewArray(binaryReader, length, ReadInt64);
        }

        public static ulong[] ReadUint64Array(BinaryReader binaryReader, int length)
        {
            return ReadNewArray(binaryReader, length, ReadUInt64);
        }

        public static float[] ReadFloatArray(BinaryReader binaryReader, int length)
        {
            return ReadNewArray(binaryReader, length, ReadFloat);
        }

        public static double[] ReadDoubleArray(BinaryReader binaryReader, int length)
        {
            return ReadNewArray(binaryReader, length, ReadDouble);
        }

        public static decimal[] ReadDecimalArray(BinaryReader binaryReader, int length)
        {
            return ReadNewArray(binaryReader, length, ReadDecimal);
        }

        public static string[] ReadStringArray(BinaryReader binaryReader, int length, Encoding encoding)
        {
            string[] array = new string[length];
            int entryLength;
            for (int i = 0; i < array.Length; ++i)
            {
                entryLength = ReadInt32(binaryReader);
                array[i] = ReadString(binaryReader, entryLength, encoding);
            }
            return array;
        }

        public static string[] ReadStringArray(BinaryReader binaryReader, int length)
        {
            return ReadStringArray(binaryReader, length, Encoding);
        }

        public static T[] ReadNewIBinarySerializableArray<T>(BinaryReader binaryReader, int length) where T : IBinarySerializable, new()
        {
            return ReadNewArray(binaryReader, length, ReadNewIBinarySerializable<T>);
        }

        // TODO
        public static T[] ReadIBinarySerializableArray<T>(BinaryReader binaryReader, T[] array) where T : IBinarySerializable, new()
        {
            return ReadArray(binaryReader, ReadIBinarySerializable, array);
        }

        public static TEnum[] ReadEnumArray<TEnum>(BinaryReader binaryReader, int length) where TEnum : Enum
        {
            TEnum[] array = new TEnum[length];

            for (int i = 0; i < array.Length; ++i)
                array[i] = ReadEnum<TEnum>(binaryReader);

            return array;
        }

        #endregion

        #region Read Array

        public static bool[] ReadBoolArray(BinaryReader binaryReader)
        {
            int length = ReadInt32(binaryReader);
            return ReadNewArray(binaryReader, length, ReadBool);
        }

        public static byte[] ReadUint8Array(BinaryReader binaryReader)
        {
            int length = ReadInt32(binaryReader);
            return binaryReader.ReadBytes(length);
        }

        public static sbyte[] ReadInt8Array(BinaryReader binaryReader)
        {
            int length = ReadInt32(binaryReader);
            return ReadNewArray(binaryReader, length, ReadInt8);
        }

        public static short[] ReadInt16Array(BinaryReader binaryReader)
        {
            int length = ReadInt32(binaryReader);
            return ReadNewArray(binaryReader, length, ReadInt16);
        }

        public static ushort[] ReadUint16Array(BinaryReader binaryReader)
        {
            int length = ReadInt32(binaryReader);
            return ReadNewArray(binaryReader, length, ReadUInt16);

        }

        public static int[] ReadInt32Array(BinaryReader binaryReader)
        {
            int length = ReadInt32(binaryReader);
            return ReadNewArray(binaryReader, length, ReadInt32);
        }

        public static uint[] ReadUint32Array(BinaryReader binaryReader)
        {
            int length = ReadInt32(binaryReader);
            return ReadNewArray(binaryReader, length, ReadUInt32);
        }

        public static long[] ReadInt64Array(BinaryReader binaryReader)
        {
            int length = ReadInt32(binaryReader);
            return ReadNewArray(binaryReader, length, ReadInt64);
        }

        public static ulong[] ReadUint64Array(BinaryReader binaryReader)
        {
            int length = ReadInt32(binaryReader);
            return ReadNewArray(binaryReader, length, ReadUInt64);
        }

        public static float[] ReadFloatArray(BinaryReader binaryReader)
        {
            int length = ReadInt32(binaryReader);
            return ReadNewArray(binaryReader, length, ReadFloat);
        }

        public static double[] ReadDoubleArray(BinaryReader binaryReader)
        {
            int length = ReadInt32(binaryReader);
            return ReadNewArray(binaryReader, length, ReadDouble);
        }

        public static decimal[] ReadDecimalArray(BinaryReader binaryReader)
        {
            int length = ReadInt32(binaryReader);
            return ReadNewArray(binaryReader, length, ReadDecimal);
        }

        public static string[] ReadStringArray(BinaryReader binaryReader, Encoding encoding)
        {
            int length = ReadInt32(binaryReader);
            return ReadStringArray(binaryReader, length, Encoding);
        }

        public static string[] ReadStringArray(BinaryReader binaryReader)
        {
            int length = ReadInt32(binaryReader);
            return ReadStringArray(binaryReader, length, Encoding);
        }

        public static T[] ReadNewIBinarySerializableArray<T>(BinaryReader binaryReader) where T : IBinarySerializable, new()
        {
            int length = ReadInt32(binaryReader);
            return ReadNewArray(binaryReader, length, ReadNewIBinarySerializable<T>);
        }


        public static TEnum[] ReadEnumArray<TEnum>(BinaryReader binaryReader) where TEnum : Enum
        {
            int length = ReadInt32(binaryReader);
            return ReadEnumArray<TEnum>(binaryReader, length);
        }

        #endregion

        #region Read Array Ref Length

        public static bool[] Read(BinaryReader binaryReader, int length, ref bool[] value)
        {
            return value = ReadNewArray(binaryReader, length, ReadBool);
        }

        public static byte[] Read(BinaryReader binaryReader, int length, ref byte[] value)
        {
            return value = binaryReader.ReadBytes(length);
        }

        public static sbyte[] Read(BinaryReader binaryReader, int length, ref sbyte[] value)
        {
            return value = ReadNewArray(binaryReader, length, ReadInt8);
        }

        public static short[] Read(BinaryReader binaryReader, int length, ref short[] value)
        {
            return value = ReadNewArray(binaryReader, length, ReadInt16);
        }

        public static ushort[] Read(BinaryReader binaryReader, int length, ref ushort[] value)
        {
            return value = ReadNewArray(binaryReader, length, ReadUInt16);
        }

        public static int[] Read(BinaryReader binaryReader, int length, ref int[] value)
        {
            return value = ReadNewArray(binaryReader, length, ReadInt32);
        }

        public static uint[] Read(BinaryReader binaryReader, int length, ref uint[] value)
        {
            return value = ReadNewArray(binaryReader, length, ReadUInt32);
        }

        public static long[] Read(BinaryReader binaryReader, int length, ref long[] value)
        {
            return value = ReadNewArray(binaryReader, length, ReadInt64);
        }

        public static ulong[] Read(BinaryReader binaryReader, int length, ref ulong[] value)
        {
            return value = ReadNewArray(binaryReader, length, ReadUInt64);
        }

        public static float[] Read(BinaryReader binaryReader, int length, ref float[] value)
        {
            return value = ReadNewArray(binaryReader, length, ReadFloat);
        }

        public static double[] Read(BinaryReader binaryReader, int length, ref double[] value)
        {
            return value = ReadNewArray(binaryReader, length, ReadDouble);
        }

        public static decimal[] Read(BinaryReader binaryReader, int length, ref decimal[] value)
        {
            return value = ReadNewArray(binaryReader, length, ReadDecimal);
        }

        public static string[] Read(BinaryReader binaryReader, int length, ref string[] value, Encoding encoding)
        {
            return value = ReadStringArray(binaryReader, length, encoding);
        }

        public static string[] Read(BinaryReader binaryReader, int length, ref string[] value)
        {
            return value = ReadStringArray(binaryReader, length, Encoding);
        }

        // TODO
        public static T[] Read<T>(BinaryReader binaryReader, int length, ref T[] value, bool createNew) where T : IBinarySerializable, new()
        {
            if (createNew)
                return value = ReadNewArray(binaryReader, length, ReadNewIBinarySerializable<T>);
            else
                return ReadArray(binaryReader, ReadIBinarySerializable<T>, value);
        }

        public static TEnum[] Read<TEnum>(BinaryReader binaryReader, int length, ref TEnum[] value) where TEnum : Enum
        {
            return value = ReadEnumArray<TEnum>(binaryReader, length);
        }

        #endregion

        #region Read Array Ref

        public static bool[] Read(BinaryReader binaryReader, ref bool[] value)
        {
            int length = ReadInt32(binaryReader);
            return value = ReadNewArray(binaryReader, length, ReadBool);
        }

        public static byte[] Read(BinaryReader binaryReader, ref byte[] value)
        {
            int length = ReadInt32(binaryReader);
            return value = binaryReader.ReadBytes(length);
        }

        public static sbyte[] Read(BinaryReader binaryReader, ref sbyte[] value)
        {
            int length = ReadInt32(binaryReader);
            return value = ReadNewArray(binaryReader, length, ReadInt8);
        }

        public static short[] Read(BinaryReader binaryReader, ref short[] value)
        {
            int length = ReadInt32(binaryReader);
            return value = ReadNewArray(binaryReader, length, ReadInt16);
        }

        public static ushort[] Read(BinaryReader binaryReader, ref ushort[] value)
        {
            int length = ReadInt32(binaryReader);
            return value = ReadNewArray(binaryReader, length, ReadUInt16);
        }

        public static int[] Read(BinaryReader binaryReader, ref int[] value)
        {
            int length = ReadInt32(binaryReader);
            return value = ReadNewArray(binaryReader, length, ReadInt32);
        }

        public static uint[] Read(BinaryReader binaryReader, ref uint[] value)
        {
            int length = ReadInt32(binaryReader);
            return value = ReadNewArray(binaryReader, length, ReadUInt32);
        }

        public static long[] Read(BinaryReader binaryReader, ref long[] value)
        {
            int length = ReadInt32(binaryReader);
            return value = ReadNewArray(binaryReader, length, ReadInt64);
        }

        public static ulong[] Read(BinaryReader binaryReader, ref ulong[] value)
        {
            int length = ReadInt32(binaryReader);
            return value = ReadNewArray(binaryReader, length, ReadUInt64);
        }

        public static float[] Read(BinaryReader binaryReader, ref float[] value)
        {
            int length = ReadInt32(binaryReader);
            return value = ReadNewArray(binaryReader, length, ReadFloat);
        }

        public static double[] Read(BinaryReader binaryReader, ref double[] value)
        {
            int length = ReadInt32(binaryReader);
            return value = ReadNewArray(binaryReader, length, ReadDouble);
        }

        public static decimal[] Read(BinaryReader binaryReader, ref decimal[] value)
        {
            int length = ReadInt32(binaryReader);
            return value = ReadNewArray(binaryReader, length, ReadDecimal);
        }

        public static string[] Read(BinaryReader binaryReader, ref string[] value, Encoding encoding)
        {
            int length = ReadInt32(binaryReader);
            return value = ReadStringArray(binaryReader, length, encoding);
        }

        public static string[] Read(BinaryReader binaryReader, ref string[] value)
        {
            int length = ReadInt32(binaryReader);
            return value = ReadStringArray(binaryReader, length, Encoding);
        }

        public static T[] Read<T>(BinaryReader binaryReader, ref T[] value) where T : IBinarySerializable, new()
        {
            int length = ReadInt32(binaryReader);
            return value = ReadNewArray(binaryReader, length, ReadNewIBinarySerializable<T>);
        }

        // TODO
        public static T[] Read<T>(BinaryReader binaryReader, ref T[] value, bool createNewInstances) where T : IBinarySerializable, new()
        {
            int length = ReadInt32(binaryReader);

            if (createNewInstances)
                return value = ReadNewArray(binaryReader, length, ReadNewIBinarySerializable<T>);
            else
                return ReadArray(binaryReader, ReadIBinarySerializable<T>, value);
        }

        public static TEnum[] ReadEnum<TEnum>(BinaryReader binaryReader, ref TEnum[] value) where TEnum : Enum
        {
            int length = ReadInt32(binaryReader);
            return value = ReadEnumArray<TEnum>(binaryReader, length);
        }

        #endregion

        #endregion

        #region WRITE

        #region Write Value

        public static void Write(BinaryWriter writer, bool value)
        {
            writer.Write(value);
        }

        public static void Write(BinaryWriter writer, byte value)
        {
            writer.Write(value);
        }

        public static void Write(BinaryWriter writer, sbyte value)
        {
            writer.Write(value);
        }

        public static void Write(BinaryWriter writer, ushort value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian ^ IsLittleEndian)
                Array.Reverse(bytes);

            writer.Write(bytes);
        }

        public static void Write(BinaryWriter writer, short value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian ^ IsLittleEndian)
                Array.Reverse(bytes);

            writer.Write(bytes);
        }

        public static void Write(BinaryWriter writer, uint value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian ^ IsLittleEndian)
                Array.Reverse(bytes);

            writer.Write(bytes);
        }

        public static void Write(BinaryWriter writer, int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian ^ IsLittleEndian)
                Array.Reverse(bytes);

            writer.Write(bytes);
        }

        public static void Write(BinaryWriter writer, ulong value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian ^ IsLittleEndian)
                Array.Reverse(bytes);

            writer.Write(bytes);
        }

        public static void Write(BinaryWriter writer, long value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian ^ IsLittleEndian)
                Array.Reverse(bytes);

            writer.Write(bytes);
        }

        public static void Write(BinaryWriter writer, float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian ^ IsLittleEndian)
                Array.Reverse(bytes);

            writer.Write(bytes);
        }

        public static void Write(BinaryWriter writer, double value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian ^ IsLittleEndian)
                Array.Reverse(bytes);

            writer.Write(bytes);
        }

        public static void Write(BinaryWriter writer, decimal value)
        {
            // Since we can't do BitConverter.GetBytes(decimal), we save the endianess
            // to disk so we can recover it ourselves
            bool isLittleEndian = BitConverter.IsLittleEndian ^ IsLittleEndian;
            writer.Write(isLittleEndian);

            writer.Write(value);
        }

        public static void Write(BinaryWriter writer, char value, Encoding encoding)
        {
            byte[] bytes = encoding.GetBytes(new char[] { value }, 0, 1);

            if (BitConverter.IsLittleEndian ^ IsLittleEndian)
                Array.Reverse(bytes);

            writer.Write(bytes);
        }

        public static void Write<T>(BinaryWriter writer, T value) where T : IBinarySerializable
        {
            value.Serialize(writer);
        }

        public static void WriteEnum<TEnum>(BinaryWriter writer, TEnum value) where TEnum : Enum
        {
            var type = Enum.GetUnderlyingType(typeof(TEnum));

            if (type == typeof(int))
            {
                int writeValue = (int)(object)value;
                writer.WriteX(writeValue);
            }
            else if (type == typeof(uint))
            {
                uint writeValue = (uint)(object)value;
                writer.WriteX(writeValue);
            }
            else if (type == typeof(short))
            {
                short writeValue = (short)(object)value;
                writer.WriteX(writeValue);
            }
            else if (type == typeof(ushort))
            {
                ushort writeValue = (ushort)(object)value;
                writer.WriteX(writeValue);
            }
            else if (type == typeof(sbyte))
            {
                sbyte writeValue = (sbyte)(object)value;
                writer.WriteX(writeValue);
            }
            else if (type == typeof(byte))
            {
                byte writeValue = (byte)(object)value;
                writer.WriteX(writeValue);
            }
            else if (type == typeof(long))
            {
                long writeValue = (long)(object)value;
                writer.WriteX(writeValue);
            }
            else if (type == typeof(ulong))
            {
                ulong writeValue = (ulong)(object)value;
                writer.WriteX(writeValue);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region Write Array

        public static void WriteArray<T>(BinaryWriter writer, T[] value, Action<BinaryWriter, T> method)
        {
            T[] array = new T[value.Length];

            for (int i = 0; i < array.Length; ++i)
                method(writer, value[i]);
        }

        public static void Write(BinaryWriter writer, bool[] value, bool writeLengthHeader)
        {
            if (writeLengthHeader)
                Write(writer, value.Length);

            WriteArray(writer, value, Write);
        }

        public static void Write(BinaryWriter writer, byte[] value, bool writeLengthHeader)
        {
            if (writeLengthHeader)
                Write(writer, value.Length);

            writer.Write(value);
        }

        public static void Write(BinaryWriter writer, sbyte[] value, bool writeLengthHeader)
        {
            if (writeLengthHeader)
                Write(writer, value.Length);

            WriteArray(writer, value, Write);
        }

        public static void Write(BinaryWriter writer, ushort[] value, bool writeLengthHeader)
        {
            if (writeLengthHeader)
                Write(writer, value.Length);

            WriteArray(writer, value, Write);
        }

        public static void Write(BinaryWriter writer, short[] value, bool writeLengthHeader)
        {
            if (writeLengthHeader)
                Write(writer, value.Length);

            WriteArray(writer, value, Write);
        }

        public static void Write(BinaryWriter writer, uint[] value, bool writeLengthHeader)
        {
            if (writeLengthHeader)
                Write(writer, value.Length);

            WriteArray(writer, value, Write);
        }

        public static void Write(BinaryWriter writer, int[] value, bool writeLengthHeader)
        {
            if (writeLengthHeader)
                Write(writer, value.Length);

            WriteArray(writer, value, Write);
        }

        public static void Write(BinaryWriter writer, ulong[] value, bool writeLengthHeader)
        {
            if (writeLengthHeader)
                Write(writer, value.Length);

            WriteArray(writer, value, Write);
        }

        public static void Write(BinaryWriter writer, long[] value, bool writeLengthHeader)
        {
            if (writeLengthHeader)
                Write(writer, value.Length);

            WriteArray(writer, value, Write);
        }

        public static void Write(BinaryWriter writer, float[] value, bool writeLengthHeader)
        {
            if (writeLengthHeader)
                Write(writer, value.Length);

            WriteArray(writer, value, Write);
        }

        public static void Write(BinaryWriter writer, double[] value, bool writeLengthHeader)
        {
            if (writeLengthHeader)
                Write(writer, value.Length);

            WriteArray(writer, value, Write);
        }

        public static void Write(BinaryWriter writer, decimal[] value, bool writeLengthHeader)
        {
            // Since Write(Decimal) stores an extra byte for endianness, this method
            // could be optimized to store that endianness only once. This would save
            // n-1 bytes. However, Decimal[] use is very uncommon and so isn't priority.

            if (writeLengthHeader)
                Write(writer, value.Length);

            WriteArray(writer, value, Write);
        }

        public static void Write(BinaryWriter writer, string value, Encoding encoding, bool writeLengthHeader)
        {
            // I can use value.Length to calc the length of UTF7 strings
            // But probably just best to avoid saving in this format anyway
            if (encoding == Encoding.UTF7)
                throw new NotImplementedException();

            if (writeLengthHeader)
                Write(writer, value.Length);

            var chars = value.ToCharArray();
            foreach (var character in chars)
            {
                writer.WriteX(character, encoding);
            }

            //byte[] bytes = encoding.GetBytes(value.ToCharArray());
            //writer.Write(bytes);
        }

        //// TODO
        //public static void WriteCString(BinaryWriter writer, string value, Encoding encoding)
        //{
        //    Write(writer, value, encoding, false);
        //    Write(writer, (byte)0x00);
        //}

        //// TODO
        //public static void WriteCString(BinaryWriter writer, string value)
        //{
        //    Write(writer, value, Encoding, false);
        //    Write(writer, (byte)0x00);
        //}

        public static void Write(BinaryWriter writer, string[] value, Encoding encoding, bool writeLengthHeader)
        {
            if (writeLengthHeader)
                Write(writer, value.Length);

            for (int i = 0; i < value.Length; ++i)
            {
                Write(writer, value[i], encoding, writeLengthHeader);
            }
        }

        public static void Write(BinaryWriter writer, string[] value, bool writeLengthHeader)
        {
            Write(writer, value, Encoding, writeLengthHeader);
        }

        public static void Write<T>(BinaryWriter writer, T[] value, bool writeLengthHeader) where T : IBinarySerializable
        {
            if (writeLengthHeader)
                Write(writer, value.Length);

            for (int i = 0; i < value.Length; ++i)
                value[i].Serialize(writer);
        }

        public static void WriteEnum<TEnum>(BinaryWriter writer, TEnum[] value, bool writeLengthHeader) where TEnum : Enum
        {
            if (writeLengthHeader)
                Write(writer, value.Length);

            for (int i = 0; i < value.Length; ++i)
                WriteEnum(writer, value[i]);
        }

        #endregion

        #endregion
    }
}