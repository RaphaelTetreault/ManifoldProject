using System;
using System.IO;

namespace Manifold.IO
{
    [Serializable]
    public struct Offset : IBinarySerializable
    {
        public int offset;

        public void SeekAddress(BinaryReader reader, SeekOrigin seekOrigin = SeekOrigin.Current)
        {
            reader.BaseStream.Seek(offset, seekOrigin);
        }

        public void Deserialize(BinaryReader reader)
        {
            reader.ReadX(ref offset);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(offset);
        }
    }
}
