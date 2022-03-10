using Manifold;
using Manifold.IO;
using System;
using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ.Stage
{
    /// <summary>
    /// Defines a segment of track.
    /// </summary>
    [Serializable]
    public sealed class TrackSegment :
        IBinaryAddressable,
        IBinarySerializable,
        IHasReference,
        ITextPrintable
    {
        // FIELDS
        private TrackSegmentType segmentType;
        private TrackEmbeddedPropertyType embeddedPropertyType;
        private TrackPerimeterFlags perimeterFlags;
        private TrackPipeCylinderFlags pipeCylinderFlags;
        private Pointer animationCurvesTrsPtr;
        private Pointer trackCornerPtr;
        private ArrayPointer childrenPtr;
        private float3 localScale;
        private float3 localRotation;
        private float3 localPosition;
        private byte unk_0x38; // mixed flags
        private byte unk_0x39; // exclusive flags
        private byte unk_0x3A; // mixed flags
        private byte unk_0x3B; // mixed flags
        private float railHeightRight;
        private float railHeightLeft;
        private uint zero_0x44; // zero confirmed
        private uint zero_0x48; // zero confirmed
        private int branchIndex; // 0, 1, 2, 3
        // REFERENCE FIELDS
        private AnimationCurveTRS animationCurveTRS;
        private TrackCorner trackCorner;
        private TrackSegment[] children;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public int Depth { get; set; }
        public bool IsRoot { get; set; }
        public TrackSegment Parent { get; set; }
        public TrackSegmentType SegmentType { get => segmentType; set => segmentType = value; }
        public TrackEmbeddedPropertyType EmbeddedPropertyType { get => embeddedPropertyType; set => embeddedPropertyType = value; }
        public TrackPerimeterFlags PerimeterFlags { get => perimeterFlags; set => perimeterFlags = value; }
        public TrackPipeCylinderFlags PipeCylinderFlags { get => pipeCylinderFlags; set => pipeCylinderFlags = value; }
        public Pointer AnimationCurvesTrsPtr { get => animationCurvesTrsPtr; set => animationCurvesTrsPtr = value; }
        public Pointer TrackCornerPtr { get => trackCornerPtr; set => trackCornerPtr = value; }
        public ArrayPointer ChildrenPtr { get => childrenPtr; set => childrenPtr = value; }
        public float3 LocalScale { get => localScale; set => localScale = value; }
        public float3 LocalRotation { get => localRotation; set => localRotation = value; }
        public float3 LocalPosition { get => localPosition; set => localPosition = value; }
        public byte Unk_0x38 { get => unk_0x38; set => unk_0x38 = value; }
        public byte Unk_0x39 { get => unk_0x39; set => unk_0x39 = value; }
        public byte Unk_0x3A { get => unk_0x3A; set => unk_0x3A = value; }
        public byte Unk_0x3B { get => unk_0x3B; set => unk_0x3B = value; }
        public float RailHeightRight { get => railHeightRight; set => railHeightRight = value; }
        public float RailHeightLeft { get => railHeightLeft; set => railHeightLeft = value; }
        public int BranchIndex { get => branchIndex; set => branchIndex = value; }
        public AnimationCurveTRS AnimationCurveTRS { get => animationCurveTRS; set => animationCurveTRS = value; }
        public TrackCorner TrackCorner { get => trackCorner; set => trackCorner = value; }
        public TrackSegment[] Children { get => children; set => children = value; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref segmentType);
                reader.ReadX(ref embeddedPropertyType);
                reader.ReadX(ref perimeterFlags);
                reader.ReadX(ref pipeCylinderFlags);
                reader.ReadX(ref animationCurvesTrsPtr);
                reader.ReadX(ref trackCornerPtr);
                reader.ReadX(ref childrenPtr);
                reader.ReadX(ref localScale);
                reader.ReadX(ref localRotation);
                reader.ReadX(ref localPosition);
                reader.ReadX(ref unk_0x38);
                reader.ReadX(ref unk_0x39);
                reader.ReadX(ref unk_0x3A);
                reader.ReadX(ref unk_0x3B);
                reader.ReadX(ref railHeightRight);
                reader.ReadX(ref railHeightLeft);
                reader.ReadX(ref zero_0x44);
                reader.ReadX(ref zero_0x48);
                reader.ReadX(ref branchIndex);
            }
            this.RecordEndAddress(reader);
            {
                // Read animation curves
                reader.JumpToAddress(animationCurvesTrsPtr);
                reader.ReadX(ref animationCurveTRS);

                // Read corner transform
                if (trackCornerPtr.IsNotNull)
                {
                    reader.JumpToAddress(trackCornerPtr);
                    reader.ReadX(ref trackCorner);
                }

                // Assertions
                Assert.IsTrue(zero_0x44 == 0);
                Assert.IsTrue(zero_0x48 == 0);

                // We are a root segment if out depth is 0
                IsRoot = Depth == 0;

                DeserializeChildrenRecursively(reader);
            }
            this.SetReaderToEndAddress(reader);
        }

        /// <summary>
        /// Deserializes children using the supplied <paramref name="reader"/>. This method assigns
        /// the deserialized children to this instance.
        /// </summary>
        /// <param name="reader">The reader to deserialize children from. Must be same used to deserialize this instance.</param>
        /// <returns>All children of this instance. Result can be of size 0. Result will not be null.</returns>
        private void DeserializeChildrenRecursively(BinaryReader reader, int depth = 0)
        {
            var children = new TrackSegment[0];
            if (childrenPtr.IsNotNull)
            {
                // NOTE: children are always sequential (ArrayPointer)
                reader.JumpToAddress(childrenPtr);
                reader.ReadX(ref children, childrenPtr.length);
            }

            this.children = children;

            foreach (var child in children)
            {
                child.Parent = this;
                child.Depth = depth + 1;
                child.DeserializeChildrenRecursively(reader);
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            {
                animationCurvesTrsPtr = animationCurveTRS.GetPointer();
                trackCornerPtr = trackCorner.GetPointer();
                childrenPtr = children.GetArrayPointer();
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(segmentType);
                writer.WriteX(embeddedPropertyType);
                writer.WriteX(perimeterFlags);
                writer.WriteX(pipeCylinderFlags);
                writer.WriteX(animationCurvesTrsPtr);
                writer.WriteX(trackCornerPtr);
                writer.WriteX(childrenPtr);
                writer.WriteX(localScale);
                writer.WriteX(localRotation);
                writer.WriteX(localPosition);
                writer.WriteX(unk_0x38);
                writer.WriteX(unk_0x39);
                writer.WriteX(unk_0x3A);
                writer.WriteX(unk_0x3B);
                writer.WriteX(railHeightRight);
                writer.WriteX(railHeightLeft);
                writer.WriteX(zero_0x44);
                writer.WriteX(zero_0x48);
                writer.WriteX(branchIndex);
            }
            this.RecordEndAddress(writer);
        }

        public void ValidateReferences()
        {
            // Assert that children are truly sequential.
            // This is a HARD requirement of ArrayPointer.
            // Iterate from 0 to (n-1). If length == 1, no looping.
            for (int i = 0; i < children.Length - 1; i++)
            {
                var curr = children[i + 0];
                var next = children[i + 1];
                // The end address of the current child must be the same as the next child
                var currAddress = curr.AddressRange.endAddress;
                var nextAddress = next.AddressRange.startAddress;

                // TODO: not true for final entry...
                // 2022/03/06: if this fails, you have to look into it. I uncommented this in the refactoring process.
                Assert.IsTrue(currAddress == nextAddress, $"Curr[{i}]:{currAddress}, Next[{i+1}]:{nextAddress}");
            }

            // Make sure references and pointers line up right
            Assert.ReferencePointer(animationCurveTRS, animationCurvesTrsPtr);
            Assert.ReferencePointer(trackCorner, trackCornerPtr);

            // 2021/12/21: NOT SURE ABOUT THIS, FAILS ON ST43
            // 2022/01/23: looks like if there are 2 nodes side-by-side and one beneath one of the
            //          of the other children, this is valid. Very weird. ST43 nodes 14/16: 14.1 and 14.2
            //if (topologyMetadata == TrackSegmentMetadata.IsTransformParent)
            //    Assert.IsTrue(childrenPtrs.IsNotNullPointer);
            // TODO: more edge cases to assert

            // Ensure rail flags AND height properties coincide
            bool hasRailLeft = perimeterFlags.HasFlag(TrackPerimeterFlags.hasRailHeightLeft);
            bool hasRailRight = perimeterFlags.HasFlag(TrackPerimeterFlags.hasRailHeightRight);
            // Both true or false, but not one of either.
            Assert.IsFalse(hasRailLeft ^ railHeightLeft > 0);
            Assert.IsFalse(hasRailRight ^ railHeightRight > 0);

            // Ensure that if there is a turn that one of the two flags for it are set
            if (trackCornerPtr.IsNotNull)
            {
                bool hasTurnLeft = perimeterFlags.HasFlag(TrackPerimeterFlags.isLeftTurn);
                bool hasTurnRight = perimeterFlags.HasFlag(TrackPerimeterFlags.isRightTurn);
                Assert.IsTrue(hasTurnLeft || hasTurnRight);
            } 
        }

        /// <summary>
        /// Gets an directed acyclic graph of this object and it's children in an order
        /// appropriate for serialization (satisfies the ArrayPointer ordering constraint).
        /// </summary>
        /// <returns>
        /// Returns all track segments from root (this) downwards. The resulting graph
        /// is ordered as: parent (0), parent's children (1), parent's grand-children (2), 
        /// and so-on. Each group is sequential with any given 'children' field being
        /// serialized together, resulting in a valid ArrayPointer for their location
        /// in the file binary.
        /// </returns>
        public TrackSegment[] GetGraphSerializableOrder()
        {
            var trackSegmentHierarchy = new List<TrackSegment>();

            // Add root/parent as first element
            trackSegmentHierarchy.Add(this);

            // Kick off recursive collection of TrackSegments
            GetChildrenRecursively(this, trackSegmentHierarchy);

            // Return our list/array which is ready to be serialized to disk
            return trackSegmentHierarchy.ToArray();
        }

        public TrackSegment[] GetGraphHierarchyOrder()
        {
            var trackSegmentHierarchy = new List<TrackSegment>();

            // Add root/parent as first element
            trackSegmentHierarchy.Add(this);

            // Kick off recursive collection of TrackSegments
            foreach (var child in children)
            {
                var childHierarchy = child.GetGraphHierarchyOrder();
                trackSegmentHierarchy.AddRange(childHierarchy);
            }

            // Return our list/array which is ready to be serialized to disk
            return trackSegmentHierarchy.ToArray();
        }

        /// <summary>
        /// Gets all children of <paramref name="parent"/> and adds it to the list <paramref name="segmentGraph"/>.
        /// After collecting children, the function recursively adds each child's children to the graph.
        /// </summary>
        /// <param name="parent">The graph element to collect children from.</param>
        /// <param name="segmentGraph">The graph to assign children to.</param>
        private static void GetChildrenRecursively(TrackSegment parent, List<TrackSegment> segmentGraph)
        {
            // If null or empth, nothing left to do.
            if (parent.children.IsNullOrEmpty())
                return;

            // Add children sequentially, needed to store ArrayPointer correctly
            segmentGraph.AddRange(parent.children);

            // Then add each child's children sequentially, recursively
            foreach (var child in parent.children)
            {
                GetChildrenRecursively(child, segmentGraph);
            }
        }



        public void PrintMultiLine(System.Text.StringBuilder builder, int indentLevel = 0, string indent = "\t")
        {         
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(TrackSegment)} ({nameof(Depth)}: {Depth})");
            indentLevel++;
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(segmentType)}: {segmentType}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(embeddedPropertyType)}: {embeddedPropertyType}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(perimeterFlags)}: {perimeterFlags}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(segmentType)}: {segmentType}");
            ////
            //builder.AppendLineIndented(indent, indentLevel, $"{nameof(animationCurvesTrsPtr)}: {animationCurvesTrsPtr}");
            //builder.AppendLineIndented(indent, indentLevel, $"{nameof(trackCornerPtr)}: {trackCornerPtr}");
            //builder.AppendLineIndented(indent, indentLevel, $"{nameof(childrenPtr)}: {childrenPtr}");
            //
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(localScale)}: {localScale}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(localRotation)}: {localRotation}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(localPosition)}: {localPosition}");
            //
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(unk_0x38)}: {unk_0x38}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(unk_0x39)}: {unk_0x39}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(unk_0x3A)}: {unk_0x3A}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(unk_0x3B)}: {unk_0x3B}");
            //
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(zero_0x44)}: {zero_0x44}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(zero_0x48)}: {zero_0x48}");
            //
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(railHeightRight)}: {railHeightRight}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(railHeightLeft)}: {railHeightLeft}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(branchIndex)}: {branchIndex}");
            //
            builder.AppendLineIndented(indent, indentLevel, TrackCorner);
        }

        public string PrintSingleLine()
        {
            return $"{nameof(TrackSegment)}({nameof(Children)}[{Children.Length}])";
        }

        public override string ToString() => PrintSingleLine();

    }
}
