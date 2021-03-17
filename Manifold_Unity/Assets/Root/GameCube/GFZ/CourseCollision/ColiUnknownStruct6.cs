using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class ColiUnknownStruct6 : IBinarySerializable, IBinaryAddressableRange
    {
        [SerializeField]
        private AddressRange addressRange;

        public Vector3 position;
        //public float unk_0x00;
        //public float unk_0x04;
        //public float unk_0x08;
        public ushort unk_0x0C;
        public ushort unk_0x0E;
        public ushort unk_0x10;
        public ushort unk_0x12;
        //public float unk_0x14;
        //public float unk_0x18;
        //public float unk_0x1C;
        public Vector3 scaleOrRotation;
        public EnumFlags32 unk_0x20;


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
                //reader.ReadX(ref unk_0x00);
                //reader.ReadX(ref unk_0x04);
                //reader.ReadX(ref unk_0x08);
                reader.ReadX(ref unk_0x0C);
                reader.ReadX(ref unk_0x0E);
                reader.ReadX(ref unk_0x10);
                reader.ReadX(ref unk_0x12);
                reader.ReadX(ref scaleOrRotation);
                //reader.ReadX(ref unk_0x14);
                //reader.ReadX(ref unk_0x18);
                //reader.ReadX(ref unk_0x1C);
                reader.ReadX(ref unk_0x20);
            }
            this.RecordEndAddress(reader);
            {

            }
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

    }
}
