using System;
using System.IO;
using System.Text;

namespace Manifold.IO
{
    [Serializable]
    public class CString :
        IBinaryAddressable,
        IBinarySerializable
    {
        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;
        public Encoding encoding = Encoding.ASCII;

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
        public static string ReadCString(BinaryReader reader)
        {
            var encoding = Encoding.ASCII;
            var stringBuilder = new StringBuilder();

            // Continue while not at end of stream
            //while (!reader.IsAtEndOfStream())
            while (true)
            {
                var character = BinaryIoUtility.ReadChar(reader, encoding);
                // If a null character is read, stop
                if (character == (char)0x00)
                    break;
                stringBuilder.Append(character);
            }

            return stringBuilder.ToString();
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
                value = ReadCString(reader);
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
    }
}
