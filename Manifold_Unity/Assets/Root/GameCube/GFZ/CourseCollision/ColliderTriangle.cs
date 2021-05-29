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
        IBinarySeralizableReference
    {
        // METADATA
        [UnityEngine.SerializeField]
        private AddressRange addressRange;

        // FEILDS
        public float unk_0x00;
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

        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref unk_0x00);
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
            writer.WriteX(unk_0x00);
            writer.WriteX(normal);
            writer.WriteX(vertex0);
            writer.WriteX(vertex1);
            writer.WriteX(vertex2);
            writer.WriteX(precomputed0);
            writer.WriteX(precomputed1);
            writer.WriteX(precomputed2);
        }

        public AddressRange SerializeWithReference(BinaryWriter writer)
        {
            this.RecordStartAddress(writer.BaseStream);
            Serialize(writer);
            this.RecordEndAddress(writer.BaseStream);
            return addressRange;
        }

    }
}