using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// NOTE: this is probably the format indexed for runtime use of the data.
    /// Itself, it is basically checkpoint data ("points") pointing back to
    /// the actual track stucture itself ("transform"). Thus, transform is
    /// referenced by many of these multiple times, while points are unqiue
    /// per TrackNode. If branched, points is an array of a point on each branch
    /// and an avergaed node acting sort of as the center (always index 0).
    /// </summary>
    [Serializable]
    public class TrackNode :
        IBinaryAddressable,
        IBinarySerializable,
        ISerializedBinaryAddressableReferer
    {
        // METADATA
        [UnityEngine.SerializeField]
        private AddressRange addressRange;

        // FIELDS
        public ArrayPointer checkpointsPtr;
        public Pointer segmentPtr;
        // FIELDS (deserialized from pointers)
        public TrackCheckpoint[] checkpoints;
        public TrackSegment segment;


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
                reader.ReadX(ref checkpointsPtr);
                reader.ReadX(ref segmentPtr);
            }
            this.RecordEndAddress(reader);
            {
                // Get point
                reader.JumpToAddress(checkpointsPtr);
                reader.ReadX(ref checkpoints, checkpointsPtr.Length, true);

                // Get transform
                // NOTE: since this data is referenced many times, I instead
                // build a list in ColiScene.
                // TODO: link the references? Does this work in Unity when
                // serialized?
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            {
                checkpointsPtr = checkpoints.GetArrayPointer();
                segmentPtr = segment.GetPointer();
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(checkpointsPtr);
                writer.WriteX(segmentPtr);
            }
            this.RecordEndAddress(writer);
        }

        public void ValidateReferences()
        {
            Assert.IsTrue(checkpointsPtr.IsNotNullPointer);
            Assert.IsTrue(segmentPtr.IsNotNullPointer);
            Assert.IsTrue(checkpoints != null);
            Assert.IsTrue(segment != null);
        }
    }
}
