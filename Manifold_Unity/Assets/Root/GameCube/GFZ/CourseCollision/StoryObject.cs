using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

using GameCube.GFZ.CourseCollision.Animation;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class StoryObject : IBinarySerializable, IBinaryAddressableRange
    {
        [SerializeField]
        private AddressRange addressRange;

        public byte unk_0x00;
        public byte unk_0x01;
        public byte unk_0x02;
        public byte unk_0x03;
        public Vector3 unk_0x04;
        public Pointer unkPtr_0x10; // is this a pre
        public Vector3 scale;
        public Vector3 rotation;
        public Vector3 position;

        // Make this a sub-structure?
        // NOTE: these pointers appear unique (not duplicate)
        public ArrayPointer[] subArrays;
        public KeyableAttribute[][] keyableAttributes;


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
                reader.ReadX(ref unkPtr_0x10);
                reader.ReadX(ref scale);
                reader.ReadX(ref rotation);
                reader.ReadX(ref position);
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
