using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using Manifold.IO;

namespace Manifold.Conversion
{
    public static class HashUtility
    {
        public static string HashBinary(HashAlgorithm hashAlgorithm, IBinarySerializable binarySerializable)
        {
            // Convert IBinarySerializable into hash
            var writer = new BinaryWriter(new MemoryStream());
            binarySerializable.Serialize(writer);
            writer.Flush();
            writer.SeekBegin();
            var hash = hashAlgorithm.ComputeHash(writer.BaseStream);

            // then to string representation
            var hashString = ByteArrayToString(hash);
            return hashString;
        }


        public static string SerializableToStreamString(IBinarySerializable binarySerializable)
        {
            // Convert IBinarySerializable into byte stream
            var writer = new BinaryWriter(new MemoryStream());
            binarySerializable.Serialize(writer);
            writer.Flush();
            writer.SeekBegin();
            // then to string representation
            var streamString = StreamToString(writer.BaseStream);
            return streamString;
        }

        public static string ByteArrayToString(byte[] array)
        {
            // then to string representation
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < array.Length; i++)
            {
                builder.Append(array[i].ToString("x2"));
            }
            return builder.ToString();
        }

        public static string StreamToString(Stream stream)
        {
            // then to string representation
            StringBuilder builder = new StringBuilder();
            stream.Seek(0, SeekOrigin.Begin);
            while (!stream.IsAtEndOfStream())
            {
                builder.Append(stream.ReadByte().ToString("x2"));
            }
            return builder.ToString();
        }

    }
}
