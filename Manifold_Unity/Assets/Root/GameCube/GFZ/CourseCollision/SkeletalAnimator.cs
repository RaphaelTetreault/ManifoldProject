using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class SkeletalAnimator : IBinarySerializable, IBinaryAddressableRange
    {
        // METADATA
        [UnityEngine.SerializeField]
        private AddressRange addressRange;

        // FIELDS
        public uint zero_0x00;
        public uint zero_0x04;
        public uint one_0x08; // Always 1. Bool?
        public Pointer propertiesPtr;
        // FIELDS (deserialized pointers)
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
                if (propertiesPtr.IsNotNullPointer)
                {
                    reader.JumpToAddress(propertiesPtr);
                    reader.ReadX(ref properties, true);
                }
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(zero_0x00);
            writer.WriteX(zero_0x04);
            writer.WriteX(one_0x08);
            writer.WriteX(propertiesPtr);

            throw new NotImplementedException();
        }

    }
}