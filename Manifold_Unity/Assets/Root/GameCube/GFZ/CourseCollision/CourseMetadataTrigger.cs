using Manifold.IO;
using System;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class CourseMetadataTrigger :
        IBinarySeralizableReference
    {
        [UnityEngine.SerializeField]
        private AddressRange addressRange;

        public float3 position;
        public Int16Rotation3 shortRotation3;
        public float3 positionOrScale;
        public CourseMetadataType course;

        //
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        public float3 PositionFrom
        {
            get => position;
        }

        public float3 PositionTo
        {
            get => positionOrScale;
        }

        public float3 Scale
        {
            get => positionOrScale;
        }

        public float3 PositionFromX
        {
            get => new float3(-position.x, position.y, position.z);
        }

        public float3 PositionToX
        {
            get => new float3(-positionOrScale.x, positionOrScale.y, positionOrScale.z);
        }

        public quaternion Rotation => shortRotation3.Rotation;
        public float3 RotationEuler => shortRotation3.EulerAngles;

        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref position);
                reader.ReadX(ref shortRotation3, true);
                reader.ReadX(ref positionOrScale);
                reader.ReadX(ref course);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public AddressRange SerializeReference(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
