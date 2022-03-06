using Manifold;
using Manifold.IO;
using System;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ.Stage
{
    // TODO: rename rocks => boulders?
    // TODO: confirm ALL use cases. Story 1AX, 2, 5?

    /// <summary>
    /// A trigger for special Story Mode objects.
    /// </summary>
    [Serializable]
    public class StoryObjectTrigger :
        IBinaryAddressable,
        IBinarySerializable,
        IHasReference
    {
        // FIELDS
        public ushort zero_0x00;
        public byte rockGroupOrderIndex;
        public byte rockGroupAndDifficulty; // split lower/upper 4 bits, see properties
        public float3 story2RockScale; // object/rock scale
        public Pointer storyObjectPathPtr;
        public float3 scale;    // trigger scale
        public float3 rotation; // trigger rotation
        public float3 position; // trigger position
        // FIELDS (deserialized from pointers)
        public StoryObjectPath storyObjectPath;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }

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
                if (storyObjectPathPtr.IsNotNull)
                {
                    // Read array pointer
                    reader.JumpToAddress(storyObjectPathPtr);
                    reader.ReadX(ref storyObjectPath);
                }
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            {
                storyObjectPathPtr = storyObjectPath.GetPointer();
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

        public void ValidateReferences()
        {
            Assert.ReferencePointer(storyObjectPath, storyObjectPathPtr);
        }

        public override string ToString()
        {
            return
                $"{nameof(StoryObjectTrigger)}(" +
                $"{nameof(RockGroup)}: {RockGroup}, " +
                $"{nameof(rockGroupOrderIndex)}: {rockGroupOrderIndex}, " +
                $"{nameof(Difficulty)}: {Difficulty}, " +
                $"Has {nameof(storyObjectPath)}: {storyObjectPathPtr.IsNotNull}, " +
                $"{nameof(position)}(x:{position.x:0.0}, y:{position.y:0.0}, z:{position.z:0.0}), " +
                $"{nameof(rotation)}(x:{rotation.x:0.0}, y:{rotation.y:0.0}, z:{rotation.z:0.0}), " +
                $"{nameof(scale)}(x:{scale.x:0.0}, y:{scale.y:0.0}, z:{scale.z:0.0})" +
                $")";
        }

    }
}
