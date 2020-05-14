using System.IO;

namespace StarkTools.IO
{
    public interface IBinarySerializable
    {
        void Deserialize(BinaryReader reader);
        void Serialize(BinaryWriter writer);
    }
}