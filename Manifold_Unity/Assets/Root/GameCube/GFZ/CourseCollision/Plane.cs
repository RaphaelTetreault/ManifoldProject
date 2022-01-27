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
        /// <summary>
        /// The dot product of this plane. 'dot(direction, position)'
        /// </summary>
        public float dotProduct;
        /// <summary>
        /// The facing direction of this plane.
        /// </summary>
        public float3 direction;
        /// <summary>
        /// The origin position of this plane.
        /// </summary>
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

        /// <summary>
        /// Computes and stores the dotProduct of this Plane.
        /// </summary>
        public void ComputeDotProduct()
        {
            float dotProduct =
                direction.x * position.x +
                direction.y * position.y +
                direction.z * position.z;

            this.dotProduct = dotProduct;
        }
    }
}
