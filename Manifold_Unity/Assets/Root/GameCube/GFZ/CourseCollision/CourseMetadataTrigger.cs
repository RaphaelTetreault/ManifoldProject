using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class CourseMetadataTrigger : IBinarySerializable, IBinaryAddressableRange
    {
        [SerializeField]
        private AddressRange addressRange;

        public Vector3 position;
        public UshortQuaternion shortRotation3;
        public Vector3 positionOrScale;
        public CourseMetadataType course;

        //
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        public Vector3 PositionFrom
        {
            get => position;
        }

        public Vector3 PositionTo
        {
            get => positionOrScale;
        }

        public Vector3 Scale
        {
            get => positionOrScale;
        }

        public Vector3 PositionFromX
        {
            get => new Vector3(-position.x, position.y, position.z);
        }

        public Vector3 PositionToX
        {
            get => new Vector3(-positionOrScale.x, positionOrScale.y, positionOrScale.z);
        }

        public Quaternion Rotation => shortRotation3.AsQuaternion;
        public Vector3 RotationEuler => shortRotation3.AsVector3;

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

    }
}
