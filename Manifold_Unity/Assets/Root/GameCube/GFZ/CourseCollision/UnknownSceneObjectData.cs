using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
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
        public Pointer[] unkAbsPtr;
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
                reader.ReadX(ref unkAbsPtr, kCount, true);
            }
            this.RecordEndAddress(reader);
            {
                unk = new UnknownSceneObjectFloatPair[kCount];
                for (int i = 0; i < kCount; i++)
                {
                    var pointer = unkAbsPtr[i];
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
                unkAbsPtr = unk.GetPointers();
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(unkAbsPtr, false);
            }
            this.RecordEndAddress(writer);
        }

        public void ValidateReferences()
        {
            // TODO: null check in GetPointer(s) etc
            // this way you can be confident in asserting object reference and pointer
            throw new NotImplementedException();
        }
    }
}