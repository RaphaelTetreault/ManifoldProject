using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.CompilerServices;

namespace Manifold.IO
{
    public static class BinaryIoUtility
    {
        #region CONSTS

        public const int
            BoolSize = 1,
            Uint8Size = 1,
            Int8Size = 1,
            Uint16Size = 2,
            Int16Size = 2,
            Uint32Size = 4,
            Int32Size = 4,
            Uint64Size = 8,
            Int64Size = 8,
            FloatSize = 4,
            DoubleSize = 8,
            DecimalSize = 16; // is this const for Decimal used?

        #endregion

        #region MEMBERS

        private static Stack<Encoding> _encodingStack = new Stack<Encoding>();
        private static Stack<bool> _endianessStack = new Stack<bool>();

        #endregion

        #region PROPERTIES

        /// <summary>
        /// The stride used to align the stream to when calling AlignTo method
        /// </summary>
        public static int _ByteAlignment
        { get; set; } = 4;

        /// <summary>
        /// The current endianness used for read/write operations
        /// </summary>
        public static bool _IsLittleEndian
        { get; set; } = false;

        /// <summary>
        /// The current encoding used for read/write operations
        /// </summary>
        public static Encoding _Encoding
        { get; set; } = Encoding.Unicode;

        #endregion

        #region METHODS

        /// <summary>
        /// Aligns the current stream to the selected number of bytes by writing
        /// padding
        /// </summary>
        /// <param name="binaryWriter"></param>
        /// <param name="padding"></param>
        public static void AlignTo(BinaryWriter binaryWriter, byte padding)
        {
            int paddingSize = NumBytesToAlign(binaryWriter.BaseStream);

            if (paddingSize > 0)
            {
                WritePadding(binaryWriter, padding, paddingSize);
            }
        }

        /// <summary>
        /// Calculates the number of bytes needed to align stream based on _ByteAlignment
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static int NumBytesToAlign(Stream stream)
        {
            return (int)(stream.Length % _ByteAlignment);
        }

        /// <summary>
        /// Pops the last pushed System.Text.Encoding
        /// </summary>
        public static void PopEncoding()
        {
            _Encoding = _encodingStack.Pop();
        }

        /// <summary>
        /// Pops the last pushed endianness
        /// </summary>
        public static void PopEndianess()
        {
            _IsLittleEndian = _endianessStack.Pop();
        }

        /// <summary>
        /// Pushes a System.Text.Encoding to a private stack. Subsequent calls to read
        /// or write strings will use this encoding.
        /// </summary>
        /// <param name="encoding"></param>
        public static void PushEncoding(Encoding encoding)
        {
            _encodingStack.Push(_Encoding);
            _Encoding = encoding;
        }

        /// <summary>
        /// Pushes an endianness to a private stack. Subsequent calls to read
        /// or write will use this endianness.
        /// </summary>
        /// <param name="isLittleEndian"></param>
        public static void PushEndianess(bool isLittleEndian)
        {
            _endianessStack.Push(_IsLittleEndian);
            _IsLittleEndian = isLittleEndian;
        }

        /// <summary>
        /// Writes padding bytes to stream
        /// </summary>
        /// <param name="binaryWriter"></param>
        /// <param name="padding"></param>
        /// <param name="paddingSize"></param>
        public static void WritePadding(BinaryWriter binaryWriter, byte padding, int paddingSize)
        {
            for (int i = 0; i < paddingSize; i++)
            {
                binaryWriter.WriteX(padding);
            }
        }

        #endregion

        #region READ

        #region Read Value

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReadBool(BinaryReader binaryReader)
        {
            return binaryReader.ReadBoolean();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ReadUInt8(BinaryReader binaryReader)
        {
            return binaryReader.ReadByte();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte ReadInt8(BinaryReader binaryReader)
        {
            return binaryReader.ReadSByte();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReadInt16(BinaryReader binaryReader)
        {
            byte[] bytes = binaryReader.ReadBytes(Int16Size);

            if (BitConverter.IsLittleEndian ^ _IsLittleEndian)
                Array.Reverse(bytes);

            return BitConverter.ToInt16(bytes, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReadUInt16(BinaryReader binaryReader)
        {
            byte[] bytes = binaryReader.ReadBytes(Uint16Size);

            if (BitConverter.IsLittleEndian ^ _IsLittleEndian)
                Array.Reverse(bytes);

            return BitConverter.ToUInt16(bytes, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadInt32(BinaryReader binaryReader)
        {
            byte[] bytes = binaryReader.ReadBytes(Int32Size);

            if (BitConverter.IsLittleEndian ^ _IsLittleEndian)
                Array.Reverse(bytes);

            return BitConverter.ToInt32(bytes, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadUInt32(BinaryReader binaryReader)
        {
            byte[] bytes = binaryReader.ReadBytes(Uint32Size);

            if (BitConverter.IsLittleEndian ^ _IsLittleEndian)
                Array.Reverse(bytes);

            return BitConverter.ToUInt32(bytes, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReadInt64(BinaryReader binaryReader)
        {
            byte[] bytes = binaryReader.ReadBytes(Int64Size);

            if (BitConverter.IsLittleEndian ^ _IsLittleEndian)
                Array.Reverse(bytes);

            return BitConverter.ToInt64(bytes, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ReadUInt64(BinaryReader binaryReader)
        {
            byte[] bytes = binaryReader.ReadBytes(Uint64Size);

            if (BitConverter.IsLittleEndian ^ _IsLittleEndian)
                Array.Reverse(bytes);

            return BitConverter.ToUInt64(bytes, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ReadFloat(BinaryReader binaryReader)
        {
            byte[] bytes = binaryReader.ReadBytes(FloatSize);

            if (BitConverter.IsLittleEndian ^ _IsLittleEndian)
                Array.Reverse(bytes);

            return BitConverter.ToSingle(bytes, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ReadDouble(BinaryReader binaryReader)
        {
            byte[] bytes = binaryReader.ReadBytes(DoubleSize);

            if (BitConverter.IsLittleEndian ^ _IsLittleEndian)
                Array.Reverse(bytes);

            return BitConverter.ToDouble(bytes, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal ReadDecimal(BinaryReader binaryReader)
        {
            bool isLittleEndian = binaryReader.ReadBoolean();
            byte[] bytes = binaryReader.ReadBytes(DecimalSize);

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ReadChar(BinaryReader binaryReader, Encoding encoding)
        {
            int lengthOfChar = encoding.IsSingleByte ? 1 : 2;
            byte[] bytes = binaryReader.ReadBytes(lengthOfChar);

            if (lengthOfChar > 1)
            {
                // 2 bytes
                if (BitConverter.IsLittleEndian ^ _IsLittleEndian)
                    Array.Reverse(bytes);
            }
            else
            {
                // Create LittleEndian array as char is 2 bytes in C#
                bytes = new byte[] { 0, bytes[0] };

                if (BitConverter.IsLittleEndian ^ _IsLittleEndian)
                    Array.Reverse(bytes);
            }

            return BitConverter.ToChar(bytes, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ReadChar(BinaryReader binaryReader)
        {
            return ReadChar(binaryReader, _Encoding);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadString(BinaryReader binaryReader, int length, Encoding encoding)
        {
            char[] value = new char[length];

            for (int i = 0; i < length; i++)
            {
                value[i] = ReadChar(binaryReader, encoding);
            }

            return new string(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadString(BinaryReader binaryReader, int length)
        {
            return ReadString(binaryReader, length, _Encoding);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadString(BinaryReader binaryReader, Encoding encoding)
        {
            int length = ReadInt32(binaryReader);
            return ReadString(binaryReader, length, encoding);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadString(BinaryReader binaryReader)
        {
            int length = ReadInt32(binaryReader);
            return ReadString(binaryReader, length, _Encoding);
        }

        // NEW!
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadCString(BinaryReader binaryReader, Encoding encoding)
        {
            var value = new StringBuilder();
            char c;
            while ((c = ReadChar(binaryReader, encoding)) != (char)0 && !binaryReader.EndOfStream())
            {
                value.Append(c);
            }
            return value.ToString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ReadNewIBinarySerializable<T>(BinaryReader binaryReader)
    where T : IBinarySerializable, new()
        {
            T value = new T();
            value.Deserialize(binaryReader);

            return value;
        }

        // NEW!
        // EXCEPTION: non-destructive, load values from stream but doesn't make a new reference
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ReadIBinarySerializable<T>(BinaryReader binaryReader, T value)
    where T : IBinarySerializable
        {
            value.Deserialize(binaryReader);
            return value;
        }

        /// <summary>
        /// TODO: Errors: InvalidCastError when enum doesn't use proper type (enum : ushort) uses EC.int
        /// </summary>

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum ReadEnum<TEnum>(BinaryReader binaryReader)
    where TEnum : Enum
        {
            var type = Enum.GetUnderlyingType(typeof(TEnum));

            /**/
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Read(BinaryReader binaryReader, ref bool value)
        {
            // Not referencing own code for performance
            return value = binaryReader.ReadBoolean();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte Read(BinaryReader binaryReader, ref byte value)
        {
            // Optimized
            return value = ReadUInt8(binaryReader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte Read(BinaryReader binaryReader, ref sbyte value)
        {
            // Optimized
            return value = binaryReader.ReadSByte();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short Read(BinaryReader binaryReader, ref short value)
        {
            return value = ReadInt16(binaryReader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort Read(BinaryReader binaryReader, ref ushort value)
        {
            return value = ReadUInt16(binaryReader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Read(BinaryReader binaryReader, ref int value)
        {
            return value = ReadInt32(binaryReader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Read(BinaryReader binaryReader, ref uint value)
        {
            return value = ReadUInt32(binaryReader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Read(BinaryReader binaryReader, ref long value)
        {
            return value = ReadInt64(binaryReader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Read(BinaryReader binaryReader, ref ulong value)
        {
            return value = ReadUInt64(binaryReader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Read(BinaryReader binaryReader, ref float value)
        {
            return value = ReadFloat(binaryReader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Read(BinaryReader binaryReader, ref double value)
        {
            return value = ReadDouble(binaryReader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal Read(BinaryReader binaryReader, ref decimal value)
        {
            return value = ReadDecimal(binaryReader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char Read(BinaryReader binaryReader, ref char value, Encoding encoding)
        {
            return value = ReadChar(binaryReader, encoding);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char Read(BinaryReader binaryReader, ref char value)
        {
            return value = ReadChar(binaryReader, _Encoding);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Read(BinaryReader binaryReader, ref string value, int length, Encoding encoding)
        {
            return value = ReadString(binaryReader, length, encoding);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Read(BinaryReader binaryReader, ref string value, int length)
        {
            return value = ReadString(binaryReader, length, _Encoding);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Read(BinaryReader binaryReader, ref string value, Encoding encoding)
        {
            int length = ReadInt32(binaryReader);
            return value = ReadString(binaryReader, length, encoding);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Read(BinaryReader binaryReader, ref string value)
        {
            int length = ReadInt32(binaryReader);
            return value = ReadString(binaryReader, length, _Encoding);
        }

        // NEW!
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Read<T>(BinaryReader binaryReader, ref T value, bool createNewInstance)
    where T : IBinarySerializable, new()
        {
            if (createNewInstance)
                return value = ReadNewIBinarySerializable<T>(binaryReader);
            else
                return value = ReadIBinarySerializable<T>(binaryReader, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum Read<TEnum>(BinaryReader binaryReader, ref TEnum value)
    where TEnum : Enum
        {
            return value = ReadEnum<TEnum>(binaryReader);
        }

        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] ReadNewArray<T>(BinaryReader binaryReader, int length, Func<BinaryReader, T> method)
        {
            T[] array = new T[length];

            for (int i = 0; i < array.Length; ++i)
                array[i] = method(binaryReader);

            return array;
        }

        // NEW!
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] ReadArray<T>(BinaryReader binaryReader, int length, Func<BinaryReader, T, T> method, T[] array)
        {
            for (int i = 0; i < array.Length; ++i)
                array[i] = method(binaryReader, array[i]);

            return array;
        }

        #region Read Array Length

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool[] ReadBoolArray(BinaryReader binaryReader, int length)
        {
            return ReadNewArray(binaryReader, length, ReadBool);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ReadUint8Array(BinaryReader binaryReader, int length)
        {
            return binaryReader.ReadBytes(length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte[] ReadInt8Array(BinaryReader binaryReader, int length)
        {
            return ReadNewArray(binaryReader, length, ReadInt8);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short[] ReadInt16Array(BinaryReader binaryReader, int length)
        {
            return ReadNewArray(binaryReader, length, ReadInt16);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort[] ReadUint16Array(BinaryReader binaryReader, int length)
        {
            return ReadNewArray(binaryReader, length, ReadUInt16);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int[] ReadInt32Array(BinaryReader binaryReader, int length)
        {
            return ReadNewArray(binaryReader, length, ReadInt32);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint[] ReadUint32Array(BinaryReader binaryReader, int length)
        {
            return ReadNewArray(binaryReader, length, ReadUInt32);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long[] ReadInt64Array(BinaryReader binaryReader, int length)
        {
            return ReadNewArray(binaryReader, length, ReadInt64);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong[] ReadUint64Array(BinaryReader binaryReader, int length)
        {
            return ReadNewArray(binaryReader, length, ReadUInt64);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float[] ReadFloatArray(BinaryReader binaryReader, int length)
        {
            return ReadNewArray(binaryReader, length, ReadFloat);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double[] ReadDoubleArray(BinaryReader binaryReader, int length)
        {
            return ReadNewArray(binaryReader, length, ReadDouble);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal[] ReadDecimalArray(BinaryReader binaryReader, int length)
        {
            return ReadNewArray(binaryReader, length, ReadDecimal);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string[] ReadStringArray(BinaryReader binaryReader, int length)
        {
            return ReadStringArray(binaryReader, length, _Encoding);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] ReadNewIBinarySerializableArray<T>(BinaryReader binaryReader, int length)
    where T : IBinarySerializable, new()
        {
            return ReadNewArray(binaryReader, length, ReadNewIBinarySerializable<T>);
        }

        // NEW!
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] ReadIBinarySerializableArray<T>(BinaryReader binaryReader, int length, T[] array)
    where T : IBinarySerializable, new()
        {
            return ReadArray<T>(binaryReader, length, ReadIBinarySerializable<T>, array);
        }

#if NET_4_7_3
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum[] ReadEnumArray<TEnum>(BinaryReader binaryReader, int length)
    where TEnum : Enum
        {
            TEnum[] array = new TEnum[length];

            for (int i = 0; i < array.Length; ++i)
                array[i] = ReadEnum<TEnum>(binaryReader);

            return array;
        }
#endif

        #endregion

        #region Read Array

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool[] ReadBoolArray(BinaryReader binaryReader)
        {
            int length = ReadInt32(binaryReader);
            return ReadNewArray(binaryReader, length, ReadBool);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ReadUint8Array(BinaryReader binaryReader)
        {
            int length = ReadInt32(binaryReader);
            return binaryReader.ReadBytes(length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte[] ReadInt8Array(BinaryReader binaryReader)
        {
            int length = ReadInt32(binaryReader);
            return ReadNewArray(binaryReader, length, ReadInt8);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short[] ReadInt16Array(BinaryReader binaryReader)
        {
            int length = ReadInt32(binaryReader);
            return ReadNewArray(binaryReader, length, ReadInt16);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort[] ReadUint16Array(BinaryReader binaryReader)
        {
            int length = ReadInt32(binaryReader);
            return ReadNewArray(binaryReader, length, ReadUInt16);

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int[] ReadInt32Array(BinaryReader binaryReader)
        {
            int length = ReadInt32(binaryReader);
            return ReadNewArray(binaryReader, length, ReadInt32);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint[] ReadUint32Array(BinaryReader binaryReader)
        {
            int length = ReadInt32(binaryReader);
            return ReadNewArray(binaryReader, length, ReadUInt32);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long[] ReadInt64Array(BinaryReader binaryReader)
        {
            int length = ReadInt32(binaryReader);
            return ReadNewArray(binaryReader, length, ReadInt64);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong[] ReadUint64Array(BinaryReader binaryReader)
        {
            int length = ReadInt32(binaryReader);
            return ReadNewArray(binaryReader, length, ReadUInt64);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float[] ReadFloatArray(BinaryReader binaryReader)
        {
            int length = ReadInt32(binaryReader);
            return ReadNewArray(binaryReader, length, ReadFloat);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double[] ReadDoubleArray(BinaryReader binaryReader)
        {
            int length = ReadInt32(binaryReader);
            return ReadNewArray(binaryReader, length, ReadDouble);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal[] ReadDecimalArray(BinaryReader binaryReader)
        {
            int length = ReadInt32(binaryReader);
            return ReadNewArray(binaryReader, length, ReadDecimal);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string[] ReadStringArray(BinaryReader binaryReader, Encoding encoding)
        {
            int length = ReadInt32(binaryReader);
            return ReadStringArray(binaryReader, length, _Encoding);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string[] ReadStringArray(BinaryReader binaryReader)
        {
            int length = ReadInt32(binaryReader);
            return ReadStringArray(binaryReader, length, _Encoding);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] ReadNewIBinarySerializableArray<T>(BinaryReader binaryReader)
    where T : IBinarySerializable, new()
        {
            int length = ReadInt32(binaryReader);
            return ReadNewArray(binaryReader, length, ReadNewIBinarySerializable<T>);
        }

#if NET_4_7_3
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum[] ReadEnumArray<TEnum>(BinaryReader binaryReader)
    where TEnum : Enum
        {
            int length = ReadInt32(binaryReader);
            return ReadEnumArray<TEnum>(binaryReader, length);
        }
#endif

        #endregion

        #region Read Array Ref Length

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool[] Read(BinaryReader binaryReader, int length, ref bool[] value)
        {
            return value = ReadNewArray(binaryReader, length, ReadBool);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] Read(BinaryReader binaryReader, int length, ref byte[] value)
        {
            return value = binaryReader.ReadBytes(length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte[] Read(BinaryReader binaryReader, int length, ref sbyte[] value)
        {
            return value = ReadNewArray(binaryReader, length, ReadInt8);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short[] Read(BinaryReader binaryReader, int length, ref short[] value)
        {
            return value = ReadNewArray(binaryReader, length, ReadInt16);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort[] Read(BinaryReader binaryReader, int length, ref ushort[] value)
        {
            return value = ReadNewArray(binaryReader, length, ReadUInt16);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int[] Read(BinaryReader binaryReader, int length, ref int[] value)
        {
            return value = ReadNewArray(binaryReader, length, ReadInt32);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint[] Read(BinaryReader binaryReader, int length, ref uint[] value)
        {
            return value = ReadNewArray(binaryReader, length, ReadUInt32);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long[] Read(BinaryReader binaryReader, int length, ref long[] value)
        {
            return value = ReadNewArray(binaryReader, length, ReadInt64);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong[] Read(BinaryReader binaryReader, int length, ref ulong[] value)
        {
            return value = ReadNewArray(binaryReader, length, ReadUInt64);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float[] Read(BinaryReader binaryReader, int length, ref float[] value)
        {
            return value = ReadNewArray(binaryReader, length, ReadFloat);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double[] Read(BinaryReader binaryReader, int length, ref double[] value)
        {
            return value = ReadNewArray(binaryReader, length, ReadDouble);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal[] Read(BinaryReader binaryReader, int length, ref decimal[] value)
        {
            return value = ReadNewArray(binaryReader, length, ReadDecimal);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string[] Read(BinaryReader binaryReader, int length, ref string[] value, Encoding encoding)
        {
            return value = ReadStringArray(binaryReader, length, encoding);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string[] Read(BinaryReader binaryReader, int length, ref string[] value)
        {
            return value = ReadStringArray(binaryReader, length, _Encoding);
        }

        // NEW!
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] Read<T>(BinaryReader binaryReader, int length, ref T[] value, bool createNew)
    where T : IBinarySerializable, new()
        {
            if (createNew)
                return value = ReadNewArray(binaryReader, length, ReadNewIBinarySerializable<T>);
            else
                return ReadArray(binaryReader, length, ReadIBinarySerializable<T>, value);
        }

#if NET_4_7_3
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum[] Read<TEnum>(BinaryReader binaryReader, int length, ref TEnum[] value)
    where TEnum : Enum
        {
            return value = ReadEnumArray<TEnum>(binaryReader, length);
        }
#endif

        #endregion

        #region Read Array Ref

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool[] Read(BinaryReader binaryReader, ref bool[] value)
        {
            int length = ReadInt32(binaryReader);
            return value = ReadNewArray(binaryReader, length, ReadBool);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] Read(BinaryReader binaryReader, ref byte[] value)
        {
            int length = ReadInt32(binaryReader);
            return value = binaryReader.ReadBytes(length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte[] Read(BinaryReader binaryReader, ref sbyte[] value)
        {
            int length = ReadInt32(binaryReader);
            return value = ReadNewArray(binaryReader, length, ReadInt8);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short[] Read(BinaryReader binaryReader, ref short[] value)
        {
            int length = ReadInt32(binaryReader);
            return value = ReadNewArray(binaryReader, length, ReadInt16);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort[] Read(BinaryReader binaryReader, ref ushort[] value)
        {
            int length = ReadInt32(binaryReader);
            return value = ReadNewArray(binaryReader, length, ReadUInt16);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int[] Read(BinaryReader binaryReader, ref int[] value)
        {
            int length = ReadInt32(binaryReader);
            return value = ReadNewArray(binaryReader, length, ReadInt32);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint[] Read(BinaryReader binaryReader, ref uint[] value)
        {
            int length = ReadInt32(binaryReader);
            return value = ReadNewArray(binaryReader, length, ReadUInt32);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long[] Read(BinaryReader binaryReader, ref long[] value)
        {
            int length = ReadInt32(binaryReader);
            return value = ReadNewArray(binaryReader, length, ReadInt64);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong[] Read(BinaryReader binaryReader, ref ulong[] value)
        {
            int length = ReadInt32(binaryReader);
            return value = ReadNewArray(binaryReader, length, ReadUInt64);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float[] Read(BinaryReader binaryReader, ref float[] value)
        {
            int length = ReadInt32(binaryReader);
            return value = ReadNewArray(binaryReader, length, ReadFloat);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double[] Read(BinaryReader binaryReader, ref double[] value)
        {
            int length = ReadInt32(binaryReader);
            return value = ReadNewArray(binaryReader, length, ReadDouble);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal[] Read(BinaryReader binaryReader, ref decimal[] value)
        {
            int length = ReadInt32(binaryReader);
            return value = ReadNewArray(binaryReader, length, ReadDecimal);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string[] Read(BinaryReader binaryReader, ref string[] value, Encoding encoding)
        {
            int length = ReadInt32(binaryReader);
            return value = ReadStringArray(binaryReader, length, encoding);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string[] Read(BinaryReader binaryReader, ref string[] value)
        {
            int length = ReadInt32(binaryReader);
            return value = ReadStringArray(binaryReader, length, _Encoding);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] Read<T>(BinaryReader binaryReader, ref T[] value)
    where T : IBinarySerializable, new()
        {
            int length = ReadInt32(binaryReader);
            return value = ReadNewArray(binaryReader, length, ReadNewIBinarySerializable<T>);
        }

        // NEW!
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] Read<T>(BinaryReader binaryReader, ref T[] value, bool createNewInstances)
    where T : IBinarySerializable, new()
        {
            int length = ReadInt32(binaryReader);

            if (createNewInstances)
                return value = ReadNewArray(binaryReader, length, ReadNewIBinarySerializable<T>);
            else
                return ReadArray(binaryReader, length, ReadIBinarySerializable<T>, value);
        }

#if NET_4_7_3
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum[] ReadEnum<TEnum>(BinaryReader binaryReader, ref TEnum[] value)
    where TEnum : Enum
        {
            int length = ReadInt32(binaryReader);
            return value = ReadEnumArray<TEnum>(binaryReader, length);
        }
#endif

        #endregion

        #endregion

        #region WRITE

        #region Write Value

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(BinaryWriter writer, bool value)
        {
            writer.Write(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(BinaryWriter writer, byte value)
        {
            writer.Write(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(BinaryWriter writer, sbyte value)
        {
            writer.Write(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(BinaryWriter writer, ushort value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian ^ _IsLittleEndian)
                Array.Reverse(bytes);

            writer.Write(bytes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(BinaryWriter writer, short value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian ^ _IsLittleEndian)
                Array.Reverse(bytes);

            writer.Write(bytes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(BinaryWriter writer, uint value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian ^ _IsLittleEndian)
                Array.Reverse(bytes);

            writer.Write(bytes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(BinaryWriter writer, int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian ^ _IsLittleEndian)
                Array.Reverse(bytes);

            writer.Write(bytes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(BinaryWriter writer, ulong value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian ^ _IsLittleEndian)
                Array.Reverse(bytes);

            writer.Write(bytes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(BinaryWriter writer, long value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian ^ _IsLittleEndian)
                Array.Reverse(bytes);

            writer.Write(bytes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(BinaryWriter writer, float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian ^ _IsLittleEndian)
                Array.Reverse(bytes);

            writer.Write(bytes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(BinaryWriter writer, double value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian ^ _IsLittleEndian)
                Array.Reverse(bytes);

            writer.Write(bytes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(BinaryWriter writer, decimal value)
        {
            // Since we can't do BitConverter.GetBytes(decimal), we save the endianess
            // to disk so we can recover it ourselves
            bool isLittleEndian = BitConverter.IsLittleEndian ^ _IsLittleEndian;
            writer.Write(isLittleEndian);

            writer.Write(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(BinaryWriter writer, char value, Encoding encoding)
        {
            byte[] bytes = encoding.GetBytes(new char[] { value }, 0, 1);

            if (BitConverter.IsLittleEndian ^ _IsLittleEndian)
                Array.Reverse(bytes);

            writer.Write(bytes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write<T>(BinaryWriter writer, T value)
    where T : IBinarySerializable
        {
            value.Serialize(writer);
        }

#if NET_4_7_3
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteEnum<TEnum>(BinaryWriter writer, TEnum value)
    where TEnum : Enum
        {
            var type = Enum.GetUnderlyingType(typeof(TEnum));

            /**/
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
#endif

        #endregion

        #region Write Array

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteArray<T>(BinaryWriter writer, T[] value, Action<BinaryWriter, T> method)
        {
            T[] array = new T[value.Length];

            for (int i = 0; i < array.Length; ++i)
                method(writer, value[i]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(BinaryWriter writer, bool[] value, bool writeLengthHeader)
        {
            if (writeLengthHeader)
                Write(writer, value.Length);

            WriteArray(writer, value, Write);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(BinaryWriter writer, byte[] value, bool writeLengthHeader)
        {
            if (writeLengthHeader)
                Write(writer, value.Length);

            writer.Write(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(BinaryWriter writer, sbyte[] value, bool writeLengthHeader)
        {
            if (writeLengthHeader)
                Write(writer, value.Length);

            WriteArray(writer, value, Write);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(BinaryWriter writer, ushort[] value, bool writeLengthHeader)
        {
            if (writeLengthHeader)
                Write(writer, value.Length);

            WriteArray(writer, value, Write);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(BinaryWriter writer, short[] value, bool writeLengthHeader)
        {
            if (writeLengthHeader)
                Write(writer, value.Length);

            WriteArray(writer, value, Write);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(BinaryWriter writer, uint[] value, bool writeLengthHeader)
        {
            if (writeLengthHeader)
                Write(writer, value.Length);

            WriteArray(writer, value, Write);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(BinaryWriter writer, int[] value, bool writeLengthHeader)
        {
            if (writeLengthHeader)
                Write(writer, value.Length);

            WriteArray(writer, value, Write);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(BinaryWriter writer, ulong[] value, bool writeLengthHeader)
        {
            if (writeLengthHeader)
                Write(writer, value.Length);

            WriteArray(writer, value, Write);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(BinaryWriter writer, long[] value, bool writeLengthHeader)
        {
            if (writeLengthHeader)
                Write(writer, value.Length);

            WriteArray(writer, value, Write);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(BinaryWriter writer, float[] value, bool writeLengthHeader)
        {
            if (writeLengthHeader)
                Write(writer, value.Length);

            WriteArray(writer, value, Write);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(BinaryWriter writer, double[] value, bool writeLengthHeader)
        {
            if (writeLengthHeader)
                Write(writer, value.Length);

            WriteArray(writer, value, Write);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(BinaryWriter writer, decimal[] value, bool writeLengthHeader)
        {
            // Since Write(Decimal) stores an extra byte for endianness, this method
            // could be optimized to store that endianness only once. This would save
            // n-1 bytes. However, Decimal[] use is very uncommon and so isn't priority.

            if (writeLengthHeader)
                Write(writer, value.Length);

            WriteArray(writer, value, Write);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
                writer.WriteX(character);
            }

            //byte[] bytes = encoding.GetBytes(value.ToCharArray());
            //writer.Write(bytes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteCString(BinaryWriter writer, string value, Encoding encoding)
        {
            Write(writer, value, encoding, false);
            Write(writer, (byte)0x00);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteCString(BinaryWriter writer, string value)
        {
            Write(writer, value, _Encoding, false);
            Write(writer, (byte)0x00);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(BinaryWriter writer, string[] value, Encoding encoding, bool writeLengthHeader)
        {
            if (writeLengthHeader)
                Write(writer, value.Length);

            for (int i = 0; i < value.Length; ++i)
            {
                Write(writer, value[i], encoding, writeLengthHeader);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(BinaryWriter writer, string[] value, bool writeLengthHeader)
        {
            Write(writer, value, _Encoding, writeLengthHeader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write<T>(BinaryWriter writer, T[] value, bool writeLengthHeader)
    where T : IBinarySerializable
        {
            if (writeLengthHeader)
                Write(writer, value.Length);

            for (int i = 0; i < value.Length; ++i)
                value[i].Serialize(writer);
        }

#if NET_4_7_3
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteEnum<TEnum>(BinaryWriter writer, TEnum[] value, bool writeLengthHeader)
    where TEnum : Enum
        {
            if (writeLengthHeader)
                Write(writer, value.Length);

            for (int i = 0; i < value.Length; ++i)
                WriteEnum(writer, value[i]);
        }
#endif

        #endregion

        #endregion
    }
}