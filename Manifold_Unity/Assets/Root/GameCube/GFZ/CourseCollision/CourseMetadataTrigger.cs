using Manifold.IO;
using System;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class CourseMetadataTrigger :
        IBinaryAddressable,
        IBinarySerializable
    {
        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;

        // FIELDS
        public Transform transform;
        public CourseMetadataType courseMetadata;


        // PROPERTIES
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        // PROPERTIES USED TO MAKE SENSE OF THIS NONSENSE
        public float3 Position => transform.Position;
        public float3 PositionFrom => transform.Position;
        public float3 PositionTo => transform.Scale;
        public float3 Scale => transform.Scale;
        public float3 ScaleBigBlueOrdeal => transform.Scale * 27.5f;
        public float3 ScaleCapsule => transform.Scale * 10f;
        public quaternion Rotation => transform.Rotation;
        public float3 RotationEuler => transform.RotationEuler;


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref transform, true);
                reader.ReadX(ref courseMetadata);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                writer.WriteX(transform);
                writer.WriteX(courseMetadata);
            }
            this.RecordEndAddress(writer);
        }

    }
}
