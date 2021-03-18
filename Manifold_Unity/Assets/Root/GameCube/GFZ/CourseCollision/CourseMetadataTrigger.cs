using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class CourseMetadataTrigger : IBinarySerializable, IBinaryAddressableRange
    {
        // 
        private const float shortToFloat = 360f / (ushort.MaxValue);
        private const float floatToshort = 1f / shortToFloat;

        [SerializeField]
        private AddressRange addressRange;

        public Vector3 position;
        public ShortRotation3 shortRotation3;
        //public ushort rotationX;
        //public ushort rotationY;
        //public ushort rotationZ;
        public ushort zero_0x12;
        public Vector3 positionOrScale;
        public CourseMetadataFlag metadata;

        //
        public Quaternion rotation;
        public Vector3 rotationEuler;

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

        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref position);
                reader.ReadX(ref shortRotation3, true);
                //reader.ReadX(ref rotationX);
                //reader.ReadX(ref rotationY);
                //reader.ReadX(ref rotationZ);
                reader.ReadX(ref zero_0x12);
                reader.ReadX(ref positionOrScale);
                reader.ReadX(ref metadata);
            }
            this.RecordEndAddress(reader);
            {
                // Copy data over to better structures
                rotationEuler = shortRotation3;
                rotation = Quaternion.Euler(rotationEuler);
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

    }
}
