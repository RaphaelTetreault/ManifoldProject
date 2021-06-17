using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    // NOTES:
    // Indexes 0-3 ARE USED
    // Indexes 4-11 unused, always (0f, 0f)

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class UnknownSceneObjectData :
        IBinaryAddressable,
        IBinarySerializable,
        ISerializedBinaryAddressableReferer
    {
        // CONSTANTS
        public const int kCount = 12;

        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;

        // FIELDS
        public Pointer[] unkPtrs;
        // REFERENCE FIELDS
        public UnknownSceneObjectFloatPair[] unk = new UnknownSceneObjectFloatPair[0];


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
                reader.ReadX(ref unkPtrs, kCount, true);
            }
            this.RecordEndAddress(reader);
            {
                unk = new UnknownSceneObjectFloatPair[kCount];
                for (int i = 0; i < kCount; i++)
                {
                    var pointer = unkPtrs[i];
                    if (pointer.IsNotNullPointer)
                    {
                        reader.JumpToAddress(pointer);
                        reader.ReadX(ref unk[i], true);
                    }
                }
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            {
                unkPtrs = unk.GetPointers();
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(unkPtrs, false);
            }
            this.RecordEndAddress(writer);
        }

        public void ValidateReferences()
        {
            for (int i = 0; i < kCount; i++)
            {
                if (unkPtrs[i].IsNotNullPointer)
                    Assert.IsTrue(unk[i] != null);

                if (unk[i] != null)
                    Assert.IsTrue(unkPtrs[i].IsNotNullPointer);
            }
        }

        public override string ToString()
        {
            return $"{nameof(UnknownSceneObjectData)} (ptr wrapper type)";
        }
    }
}