using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class ArcadeCheckpointTrigger : IBinarySerializable, IBinaryAddressableRange
    {
        [SerializeField]
        private AddressRange addressRange;

        public Vector3 position;
        public ShortRotation3 shortRotation3;
        public Vector3 scale;
        public ArcadeCheckpointType type;

        public Quaternion Rotation => shortRotation3.AsQuaternion;
        public Vector3 RotationEuler => shortRotation3.AsVector3;

        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }


        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref position);
                reader.ReadX(ref shortRotation3, true);
                reader.ReadX(ref scale);
                reader.ReadX(ref type);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

    }
}
