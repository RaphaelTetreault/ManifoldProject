using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public struct MinMax :
        IBinarySerializable
    {
        public float min;
        public float max;

        public MinMax(float min, float max)
        {
            this.min = min;
            this.max = max;
        }

        public void Deserialize(BinaryReader reader)
        {
            reader.ReadX(ref min);
            reader.ReadX(ref max);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(min);
            writer.WriteX(max);
        }
    }
}
