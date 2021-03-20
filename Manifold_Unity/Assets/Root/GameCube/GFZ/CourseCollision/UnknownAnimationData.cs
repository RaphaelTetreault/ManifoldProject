//using Manifold.IO;
//using System;
//using System.IO;
//using UnityEngine;

//namespace GameCube.GFZ.CourseCollision
//{
//    [Serializable]
//    public class UnknownAnimationData : IBinarySerializable, IBinaryAddressableRange
//    {
//        [SerializeField]
//        private AddressRange addressRange;

//        public ArrayPointer keyableAttributesPtr;
//        public KeyableAttribute[] keyableAttributes;


//        public AddressRange AddressRange
//        {
//            get => addressRange;
//            set => addressRange = value;
//        }


//        public void Deserialize(BinaryReader reader)
//        {
//            this.RecordStartAddress(reader);
//            {
//                reader.ReadX(ref keyableAttributesPtr);
//            }
//            this.RecordEndAddress(reader);
//            {
//                reader.JumpToAddress(keyableAttributesPtr);
//                reader.ReadX(ref keyableAttributes, keyableAttributesPtr.length, true);
//            }
//            this.SetReaderToEndAddress(reader);
//        }

//        public void Serialize(BinaryWriter writer)
//        {
//            throw new NotImplementedException();
//        }

//    }
//}
