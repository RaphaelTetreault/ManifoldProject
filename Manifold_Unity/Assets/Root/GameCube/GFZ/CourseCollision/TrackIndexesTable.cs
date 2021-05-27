using Manifold.IO;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ.CourseCollision
{
    public class TrackIndexesTable : IBinarySerializable, IBinaryAddressableRange
    {
        // Constants
        public const int kNumEntries = 64;

        // Metadata
        [UnityEngine.SerializeField]
        private AddressRange addressRange;

        // Fields
        public Pointer[] trackIndexesPtrs = new Pointer[0];
        public TrackIndexes[] trackIndexArray = new TrackIndexes[0];

        // Properties
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        // Methods
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref trackIndexesPtrs, kNumEntries, true);
            }
            this.RecordEndAddress(reader);
            {
                trackIndexArray = new TrackIndexes[64];
                for (int i = 0; i < trackIndexesPtrs.Length; i++)
                {
                    var pointer = trackIndexesPtrs[i];
                    if (pointer.IsNotNullPointer)
                    {
                        reader.JumpToAddress(pointer);
                        reader.ReadX(ref trackIndexArray[i], true);
                    }
                    else
                    {
                        trackIndexArray[i] = new TrackIndexes();
                    }
                }
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

    }
}
