using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class Transform : IBinarySerializable, IBinaryAddressableRange
    {
        // metadata
        [SerializeField]
        private AddressRange addressRange;

        // structure
        [SerializeField]
        private Vector3 position;
        [SerializeField]
        private Uint16Rotation3 ushortQuaternion;
        [SerializeField]
        private Vector3 scale;

        //
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        public Vector3 Position
        {
            get => position;
            set => position = value;
        }

        public Vector3 RotationEuler
        {
            get => ushortQuaternion.EulerAngles;
            //set => shortRotation3 = value;
        }

        public Quaternion Rotation
        {
            get => ushortQuaternion.rotation;
        }

        public Vector3 Scale
        {
            get => scale;
            set => scale = value;
        }

        public Uint16Rotation3 UshortQuaternion => ushortQuaternion;

        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref position);
                reader.ReadX(ref ushortQuaternion, true);
                reader.ReadX(ref scale);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(position);
            writer.WriteX(ushortQuaternion);
            writer.WriteX(scale);

            // Write values pointed at by, update ptrs above
            throw new NotImplementedException();
        }
    }
}