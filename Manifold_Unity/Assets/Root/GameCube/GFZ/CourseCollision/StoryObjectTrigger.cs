using Manifold.IO;
using System;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class StoryObjectTrigger : IBinarySerializable, IBinaryAddressableRange
    {
        [UnityEngine.SerializeField]
        private AddressRange addressRange;

        public ushort zero_0x00;
        public byte rockGroupOrderIndex;
        public byte rockGroupAndDifficulty; // split lower/upper 4 bits
        public float3 story2RockScale;
        public Pointer animationPathPtr;
        public float3 scale;
        public float3 rotation;
        public float3 position;

        // NOTE: used in story 2, object's animation path when triggered? (likely)
        public ArrayPointer keyableArrayPtr;
        public KeyableAttribute[] keyableAttributes;


        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        public byte Difficulty
        {
            get
            {
                // Lower 4 bits are for difficulty
                return (byte)(rockGroupAndDifficulty & 0b00001111);
            }
        }

        public byte RockGroup
        {
            get
            {
                // Upper 4 bits are for group of rocks which fall
                return (byte)(rockGroupAndDifficulty >> 4);
            }
        }

        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref zero_0x00);
                reader.ReadX(ref rockGroupOrderIndex);
                reader.ReadX(ref rockGroupAndDifficulty);
                reader.ReadX(ref story2RockScale);
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
                reader.ReadX(ref keyableAttributes, keyableArrayPtr.Length, true);
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

    }
}
