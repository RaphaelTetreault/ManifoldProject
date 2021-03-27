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
        private ShortRotation3 shortRotation3;
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
            get => shortRotation3.AsVector3;
            //set => shortRotation3 = value;
        }

        public Quaternion Rotation
        {
            get => shortRotation3.AsQuaternion;
        }

        public Vector3 Scale
        {
            get => scale;
            set => scale = value;
        }

        public short RotX => shortRotation3.x.Backing;
        public short RotY => shortRotation3.y.Backing;
        public short RotZ => shortRotation3.z.Backing;
        public EnumFlags16 UnkFlags => shortRotation3.unkFlags;


        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref position);
                reader.ReadX(ref shortRotation3, true);
                reader.ReadX(ref scale);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(position);
            writer.WriteX(shortRotation3);
            writer.WriteX(scale);

            // Write values pointed at by, update ptrs above
            throw new NotImplementedException();
        }
    }
}