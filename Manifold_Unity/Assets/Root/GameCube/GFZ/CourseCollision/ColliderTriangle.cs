using Manifold.IO;
using System;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// An individual triangle as part of a collider mesh.
    /// </summary>
    [Serializable]
    public sealed class ColliderTriangle :
        IBinaryAddressable,
        IBinarySerializable
    {
        // METADATA
        [UnityEngine.SerializeField]
        private AddressRange addressRange;

        // FEILDS
        /// <summary>
        /// The dot product of dot(normal, vertex0/1/2). All result in the same scalar.
        /// </summary>
        public float dotProduct;
        public float3 normal;
        public float3 vertex0;
        public float3 vertex1;
        public float3 vertex2;
        public float3 precomputed0;
        public float3 precomputed1;
        public float3 precomputed2;


        // PROPERTIES
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }


        // METHODS
        public float3[] GetVerts()
        {
            return new float3[] { vertex0, vertex1, vertex2 };
        }
        public float3[] GetPrecomputes()
        {
            return new float3[] { precomputed0, precomputed1, precomputed2};
        }

        public float3 VertCenter()
        {
            return (vertex0 + vertex1 + vertex2) / 3f;
        }

        public float3 PrecomputeCenter()
        {
            // Division inline since the values are BIG and would
            // lose more precision if summed first.
            return
                precomputed0 / 3f +
                precomputed1 / 3f +
                precomputed2 / 3f;
        }

        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref dotProduct);
                reader.ReadX(ref normal);
                reader.ReadX(ref vertex0);
                reader.ReadX(ref vertex1);
                reader.ReadX(ref vertex2);
                reader.ReadX(ref precomputed0);
                reader.ReadX(ref precomputed1);
                reader.ReadX(ref precomputed2);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                writer.WriteX(dotProduct);
                writer.WriteX(normal);
                writer.WriteX(vertex0);
                writer.WriteX(vertex1);
                writer.WriteX(vertex2);
                writer.WriteX(precomputed0);
                writer.WriteX(precomputed1);
                writer.WriteX(precomputed2);
            }
            this.RecordEndAddress(writer);
        }
    }
}