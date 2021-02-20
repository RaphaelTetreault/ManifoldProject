using System.IO;
using System.Text;
using System.Security.Cryptography;
using StarkTools.IO;

namespace Manifold.Conversion
{
    public static class HashSerializables
    {
        public static string Hash(IBinarySerializable self)
        {
            // Convert IBinarySerializable into hash
            var sha1 = MD5.Create();
            var writer = new BinaryWriter(new MemoryStream());
            self.Serialize(writer);
            var hash = sha1.ComputeHash(writer.BaseStream);

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
