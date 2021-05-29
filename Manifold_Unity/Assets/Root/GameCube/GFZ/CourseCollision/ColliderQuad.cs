using Manifold.IO;
using System;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class ColliderQuad :
        IBinarySeralizableReference
    {
        // METADATA
        [UnityEngine.SerializeField]
        private AddressRange addressRange;

        // FIELDS

        // Normal's quaternion rotation theta? https://en.wikipedia.org/wiki/Quaternions_and_spatial_rotation#Using_quaternion_as_rotations
        // Value range: -3613.961 through 3595.046, avg: -11 (basically 0)
        // Could possibly be "bounding sphere" radius/diameter from avg of all positions?
        public float unk_0x00;
        public float3 normal;
        public float3 vertex0;
        public float3 vertex1;
        public float3 vertex2;
        public float3 vertex3;
        public float3 precomputed0;
        public float3 precomputed1;
        public float3 precomputed2;
        public float3 precomputed3;


        // PROPERTIES
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }


        // METHODS
        public float3[] GetVerts()
        {
            return new float3[] { vertex0, vertex1, vertex2, vertex3 };
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
                reader.ReadX(ref vertex3);
                reader.ReadX(ref precomputed0);
                reader.ReadX(ref precomputed1);
                reader.ReadX(ref precomputed2);
                reader.ReadX(ref precomputed3);
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
            writer.WriteX(vertex3);
            writer.WriteX(precomputed0);
            writer.WriteX(precomputed1);
            writer.WriteX(precomputed2);
            writer.WriteX(precomputed3);
        }

        public AddressRange SerializeReference(BinaryWriter writer)
        {
            this.RecordStartAddress(writer.BaseStream);
            Serialize(writer);
            this.RecordEndAddress(writer.BaseStream);
            return addressRange;
        }

    }
}