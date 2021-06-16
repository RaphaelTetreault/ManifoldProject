using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public struct Range :
        IBinarySerializable
    {
        public float near;
        public float far;

        public Range(float near, float far)
        {
            this.near = near;
            this.far = far;
        }

        public void Deserialize(BinaryReader reader)
        {
            reader.ReadX(ref near);
            reader.ReadX(ref far);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(near);
            writer.WriteX(far);
        }

        public override string ToString()
        {
            return $"Range({nameof(near)}: {near:0}, {nameof(far)}: {far:0})";
        }

    }
}
