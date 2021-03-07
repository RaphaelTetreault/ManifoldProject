using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class StoryObjectTrigger : IBinarySerializable, IBinaryAddressableRange
    {
        [SerializeField]
        private AddressRange addressRange;

        public byte unk_0x00;
        public byte unk_0x01;
        public byte unk_0x02;
        public byte unk_0x03;
        public Vector3 unk_0x04; //...?
        public Pointer animationPathPtr;
        public Vector3 scale;
        public Vector3 rotation;
        public Vector3 position;

        // NOTE: used in story 2, object's animation path when triggered? (likely)
        public ArrayPointer keyableArrayPtr;
        public KeyableAttribute[] keyableAttributes;


        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }


        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref unk_0x00);
                reader.ReadX(ref unk_0x01);
                reader.ReadX(ref unk_0x02);
                reader.ReadX(ref unk_0x03);
                reader.ReadX(ref unk_0x04);
                reader.ReadX(ref animationPathPtr);
                reader.ReadX(ref scale);
                reader.ReadX(ref rotation);
                reader.ReadX(ref position);
            }
            this.RecordEndAddress(reader);
            {
                // Read array pointer
                reader.JumpToAddress(animationPathPtr);
                reader.ReadX(ref keyableArrayPtr);
                // Read array
                reader.JumpToAddress(keyableArrayPtr);
                reader.ReadX(ref keyableAttributes, keyableArrayPtr.length, true);
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

    }
}
