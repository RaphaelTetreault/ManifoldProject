using Manifold;
using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// NOTE: this is probably the format indexed for runtime use of the data.
    /// Itself, it is basically checkpoint data ("points") pointing back to
    /// the actual track stucture itself ("transform"). Thus, transform is
    /// referenced by many of these multiple times, while points are unique
    /// per TrackNode. If the track has branching paths, element[0] of the
    /// array points to an averaged node/position. Other elements beyond 
    /// the first indicate the specific branched path and it's own data.
    /// </summary>
    [Serializable]
    public class TrackNode :
        IBinaryAddressable,
        IBinarySerializable,
        IHasReference
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
            // Validate pointer instance relationship
            Assert.ValidateReferencePointer(checkpoints, checkpointsPtr);
            Assert.ReferencePointer(segment, segmentPtr);

            // This type should never have any nulls
            Assert.IsTrue(checkpointsPtr.IsNotNullPointer);
            Assert.IsTrue(segmentPtr.IsNotNullPointer);
        }

        public override string ToString()
        {
            return 
                $"{nameof(TrackNode)}(" +
                $"{nameof(segmentPtr)}: 0x{segmentPtr}, " +
                $"{nameof(checkpoints)}: {checkpoints.Length}" +
                $")";
        }
    }
}
