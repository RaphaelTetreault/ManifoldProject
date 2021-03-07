//using Manifold.IO;
//using System;
//using System.IO;
//using UnityEngine;

//namespace GameCube.GFZ.CourseCollision
//{
//    [Serializable]
//    public class ColiUnknownStruct7 : IBinarySerializable, IBinaryAddressableRange
//    {
//        [SerializeField]
//        private AddressRange addressRange;

//        public float unk_0x00;
//        public float unk_0x04;
//        public float unk_0x08;
//        public int unk_0x0C;
//        public int unk_0x10;
//        public float unk_0x14;
//        public float unk_0x18;
//        public float unk_0x1C;
//        public EnumFlags32 unk_0x20;


//        public AddressRange AddressRange
//        {
//            get => addressRange;
//            set => addressRange = value;
//        }


//        public void Deserialize(BinaryReader reader)
//        {
//            this.RecordStartAddress(reader);
//            {
//                reader.ReadX(ref unk_0x00);
//                reader.ReadX(ref unk_0x04);
//                reader.ReadX(ref unk_0x08);
//                reader.ReadX(ref unk_0x0C);
//                reader.ReadX(ref unk_0x10);
//                reader.ReadX(ref unk_0x14);
//                reader.ReadX(ref unk_0x18);
//                reader.ReadX(ref unk_0x1C);
//                reader.ReadX(ref unk_0x20);
//            }
//            this.RecordEndAddress(reader);
//            {

//            }
//        }

//        public void Serialize(BinaryWriter writer)
//        {
//            throw new NotImplementedException();
//        }

//    }
//}
