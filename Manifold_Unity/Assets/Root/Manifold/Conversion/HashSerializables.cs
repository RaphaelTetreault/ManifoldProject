using System.IO;
using System.Text;
using System.Security.Cryptography;
using Manifold.IO;

namespace Manifold.Conversion
{
    public static class HashSerializables
    {
        public static string Hash(HashAlgorithm hashAlgorithm, IBinarySerializable binarySerializable)
        {
            // Convert IBinarySerializable into hash
            var writer = new BinaryWriter(new MemoryStream());
            binarySerializable.Serialize(writer);
            writer.Flush();
            writer.BaseStream.Seek(0, SeekOrigin.Begin);
            var hash = hashAlgorithm.ComputeHash(writer.BaseStream);

            // then to string representation
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("x2"));
            }

            return builder.ToString();
        }


    }
}
