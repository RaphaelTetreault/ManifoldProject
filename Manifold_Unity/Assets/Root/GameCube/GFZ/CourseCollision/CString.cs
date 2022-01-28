using System;
using System.IO;
using System.Text;

namespace Manifold.IO
{
    /// <summary>
    /// Simple wrapper class for string to encode and decode a C-style null-terminated
    /// string in SHIFT_JIS format.
    /// </summary>
    [Serializable]
    public class CString :
        IBinaryAddressable,
        IBinarySerializable,
        IEquatable<CString>
    {
        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;
        public Encoding encoding = Encoding.GetEncoding("shift_jis");

        // FIELDS
        public string value = string.Empty;


        // PROPERTIES
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        public int Length => value.Length;


        // METHODS
        public static string ReadCString(BinaryReader reader, Encoding encoding)
        {
            var bytes = new System.Collections.Generic.List<byte>();

            while (true)
            {
                var @byte = reader.ReadByte();

                // If a null character is read, stop
                if (@byte == 0x00)
                    break;

                bytes.Add(@byte);
            }

            var str = encoding.GetString(bytes.ToArray());
            return str;
        }

        public static void WriteCString(BinaryWriter writer, string value, Encoding encoding)
        {
            BinaryIoUtility.Write(writer, value, encoding, false);
            BinaryIoUtility.Write(writer, (byte)0x00);
        }


        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                value = ReadCString(reader, encoding);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                WriteCString(writer, value, encoding);
            }
            this.RecordEndAddress(writer);
        }


        public static implicit operator string(CString cString)
        {
            return cString.value;
        }

        public static implicit operator CString(string str)
        {
            return new CString() { value = str };
        }

        public override string ToString()
        {
            return value;
        }

        public bool Equals(CString other)
        {
            return (value == other.value);
        }
    }
}
