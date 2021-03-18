using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class Trigger : IBinarySerializable, IBinaryAddressableRange
    {
        private const float shortToFloat = 360f / (ushort.MaxValue);
        private const float floatToshort = 1f / shortToFloat;

        [SerializeField]
        private AddressRange addressRange;

        public Vector3 position;
        //public ShortRotation3 rotation;
        public ushort rotationX;
        public ushort rotationY;
        public ushort rotationZ;
        public ushort zero_0x12;
        public Vector3 scale;
        public EnumFlags16 unk_0x20;
        public EnumFlags16 unk_0x22;

        //
        public Quaternion rotation;
        public Vector3 rotationEuler;

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
                //reader.ReadX(ref rotation, true);
                reader.ReadX(ref rotationX);
                reader.ReadX(ref rotationY);
                reader.ReadX(ref rotationZ);
                reader.ReadX(ref zero_0x12);
                reader.ReadX(ref scale);
                reader.ReadX(ref unk_0x20);
                reader.ReadX(ref unk_0x22);
            }
            this.RecordEndAddress(reader);
            {
                // Volumes used are 10x10x10
                // Since we use a 1x1x1 cube, multiply x10
                scale *= 10f;

                //
                rotationEuler = new Vector3(
                    rotationX * shortToFloat,
                    rotationY * shortToFloat,
                    rotationZ * shortToFloat
                    );
                rotation = Quaternion.Euler(rotationEuler);
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            // scale /= 10f;

            throw new NotImplementedException();
        }

    }
}
