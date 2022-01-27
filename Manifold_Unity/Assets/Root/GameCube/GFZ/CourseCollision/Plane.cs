using Manifold.IO;
using System;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public struct Plane : 
        IBinarySerializable
        // Add IBinaryAddressable?
    {
        public float dotProduct;
        public float3 direction;
        public float3 position;

        public void Deserialize(BinaryReader reader)
        {
            reader.ReadX(ref dotProduct);
            reader.ReadX(ref direction);
            reader.ReadX(ref position);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(dotProduct);
            writer.WriteX(direction);
            writer.WriteX(position);
        }
    }
}
