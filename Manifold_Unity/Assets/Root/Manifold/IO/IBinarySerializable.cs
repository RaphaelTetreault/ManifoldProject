using System.IO;

namespace Manifold.IO
{
    public interface IBinarySerializable
    {
        void Deserialize(BinaryReader reader);
        void Serialize(BinaryWriter writer);
    }
}