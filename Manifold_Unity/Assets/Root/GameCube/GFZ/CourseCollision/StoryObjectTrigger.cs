using Manifold.IO;
using System;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class StoryObjectTrigger :
        IBinarySeralizableReference
    {
        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;

        // FIELDS
        public ushort zero_0x00;
        public byte rockGroupOrderIndex;
        public byte rockGroupAndDifficulty; // split lower/upper 4 bits
        public float3 story2RockScale;
        public Pointer storyObjectPathPtr;
        public float3 scale;
        public float3 rotation;
        public float3 position;
        // FIELDS (deserialized from pointers)
        public StoryObjectPath storyObjectPath; // NOTE: used in story 2, object's animation path when triggered? (likely)


        // PROPERTIES
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


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref zero_0x00);
                reader.ReadX(ref rockGroupOrderIndex);
                reader.ReadX(ref rockGroupAndDifficulty);
                reader.ReadX(ref story2RockScale);
                reader.ReadX(ref storyObjectPathPtr);
                reader.ReadX(ref scale);
                reader.ReadX(ref rotation);
                reader.ReadX(ref position);
            }
            this.RecordEndAddress(reader);
            {
                if (storyObjectPathPtr.IsNotNullPointer)
                {
                    // Read array pointer
                    reader.JumpToAddress(storyObjectPathPtr);
                    reader.ReadX(ref storyObjectPath, true);
                }
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            {
                writer.InlineDesc(ColiCourseUtility.SerializeVerbose, -1, storyObjectPath);
                storyObjectPathPtr = storyObjectPath.SerializeWithReference(writer).GetPointer();
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(zero_0x00);
                writer.WriteX(rockGroupOrderIndex);
                writer.WriteX(rockGroupAndDifficulty);
                writer.WriteX(story2RockScale);
                writer.WriteX(storyObjectPathPtr);
                writer.WriteX(scale);
                writer.WriteX(rotation);
                writer.WriteX(position);
            }
            this.RecordEndAddress(writer);
        }

        public AddressRange SerializeWithReference(BinaryWriter writer)
        {
            Serialize(writer);
            return addressRange;
        }
    }
}
