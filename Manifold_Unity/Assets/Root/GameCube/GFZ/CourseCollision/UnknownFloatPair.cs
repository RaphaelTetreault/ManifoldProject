using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public struct UnknownFloatPair : IBinarySerializable
    {
        public float a;
        public float b;

        public void Deserialize(BinaryReader reader)
        {
            reader.ReadX(ref a);
            reader.ReadX(ref b);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(a);
            writer.WriteX(b);
        }
    }
}
