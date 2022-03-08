using Manifold.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace GameCube.Cheats
{
    public sealed class GctCode :
        IBinarySerializable
    {
        public const ulong codeTerminator = 0xE0000000_80008000;

        public string name;
        public ulong[] payload;

        public void Deserialize(BinaryReader reader)
        {
            var lines = new List<ulong>();
            while (true)
            {
                // Read line of code, add to list
                var line = reader.ReadX_UInt64();
                lines.Add(line);

                // If end of code, break loop
                if (line == codeTerminator)
                    break;
            }
            payload = lines.ToArray();
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(payload);
        }

    }

}
