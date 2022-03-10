using Manifold;
using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.Stage
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
    public sealed class TrackNode :
        IBinaryAddressable,
        IBinarySerializable,
        IHasReference,
        ITextPrintable
    {
        // FIELDS
        private ArrayPointer checkpointsPtr;
        private Pointer segmentPtr;
        // FIELDS (deserialized from pointers)
        private Checkpoint[] checkpoints;
        private TrackSegment segment;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public Checkpoint[] Checkpoints { get => checkpoints; set => checkpoints = value; }
        public ArrayPointer CheckpointsPtr { get => checkpointsPtr; set => checkpointsPtr = value; }
        public TrackSegment Segment { get => segment; set => segment = value; }
        public Pointer SegmentPtr { get => segmentPtr; set => segmentPtr = value; }


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
                reader.ReadX(ref checkpoints, checkpointsPtr.length);

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
            Assert.IsTrue(checkpointsPtr.IsNotNull);
            Assert.IsTrue(segmentPtr.IsNotNull);
        }

        public string PrintSingleLine()
        {
            return $"{nameof(TrackNode)}({Checkpoints}[{checkpoints.Length}])";
        }

        public void PrintMultiLine(System.Text.StringBuilder builder, int indentLevel = 0, string indent = "\t")
        {
            builder.AppendLineIndented(indent, indentLevel, PrintSingleLine());
        }

        public override string ToString() => PrintSingleLine();

    }
}
