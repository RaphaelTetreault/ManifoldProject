using System;
using System.IO;
using System.Text;

namespace Manifold.IO
{
    /// <summary>
    /// Simple wrapper class for string to encode and decode a C-style null-terminated string.
    /// The inheritor must define the encoding used by the string.
    /// </summary>
    [Serializable]
    public abstract class CString :
        IBinaryAddressable,
        IBinarySerializable,
        IEquatable<CString>
    {
        // CONSTANTS
        public const byte nullTerminator = 0x00;

        // METADATA
        private AddressRange addressRange;


        // FIELDS
        public string value = string.Empty;


        //
        public CString()
        {
            value = string.Empty;
        }

        public CString(string value)
        {
            this.value = value;
        }



        // PROPERTIES
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }
        public int Length => value.Length;
        public abstract Encoding Encoding { get; }


        // METHODS
        public static string ReadCString(BinaryReader reader, Encoding encoding)
        {
            var bytes = new System.Collections.Generic.List<byte>();

            while (true)
            {
                var @byte = reader.ReadByte();

                // If a null character is read, stop
                if (@byte is nullTerminator)
                    break;

                bytes.Add(@byte);
            }

            var str = encoding.GetString(bytes.ToArray());
            return str;
        }

        public static void WriteCString(BinaryWriter writer, string value, Encoding encoding)
        {
            writer.WriteX(value, encoding, false);
            writer.WriteX(nullTerminator);
        }


        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                value = ReadCString(reader, Encoding);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                WriteCString(writer, value, Encoding);
            }
            this.RecordEndAddress(writer);
        }


        public static implicit operator string(CString str)
        {
            return str.value;
        }

        public static implicit operator CString(string str)
        {
            return str;
        }

        public sealed override string ToString()
        {
            return value;
        }

        public bool Equals(CString other)
        {
            // Compares strings
            bool isSameValue = value == other.value;
            return isSameValue;
        }
    }
}
