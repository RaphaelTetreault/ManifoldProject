using Manifold;
using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// Defines an object with skeletal animations.
    /// </summary>
    [Serializable]
    public class SkeletalAnimator :
        IBinaryAddressable,
        IBinarySerializable,
        IHasReference
    {
        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;

        // FIELDS
        public uint zero_0x00;
        public uint zero_0x04;
        public uint one_0x08; // Always 1. Bool?
        public Pointer propertiesPtr;
        // REFERENCE FIELDS
        public SkeletalProperties properties;


        // PROPERTIES
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref zero_0x00);
                reader.ReadX(ref zero_0x04);
                reader.ReadX(ref one_0x08);
                reader.ReadX(ref propertiesPtr);
            }
            this.RecordEndAddress(reader);
            {
                // 2021/06/16: should ALWAYS exist
                Assert.IsTrue(propertiesPtr.IsNotNull);
                reader.JumpToAddress(propertiesPtr);
                reader.ReadX(ref properties, true);
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            {
                propertiesPtr = properties.GetPointer();
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(zero_0x00);
                writer.WriteX(zero_0x04);
                writer.WriteX(one_0x08);
                writer.WriteX(propertiesPtr);
            }
            this.RecordEndAddress(writer);
        }

        public void ValidateReferences()
        {
            Assert.ReferencePointer(properties, propertiesPtr);
        }

        public override string ToString()
        {
            return $"{nameof(SkeletalAnimator)}";
        }
    }
}