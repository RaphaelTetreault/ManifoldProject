using System.IO;

namespace Manifold.IO
{
    public interface ITextSerializable
    {
        void Deserialize(StreamReader reader);
        void Serialize(StreamWriter writer);
    }
}