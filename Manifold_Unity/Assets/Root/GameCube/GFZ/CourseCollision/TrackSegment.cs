using Manifold;
using Manifold.IO;
using System;
using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// Defines a segment of track.
    /// </summary>
    [Serializable]
    public class TrackSegment :
        IBinaryAddressable,
        IBinarySerializable,
        IHasReference
    {
        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;
        //public float4x4 localMatrix;
        //public float4x4 worldMatrix;
        public int depth;
        public bool isRoot;

        // FIELDS
        public TrackSegmentType segmentType;
        public TrackEmbeddedPropertyType embeddedPropertyType;
        public TrackPerimeterFlags perimeterFlags;
        public TrackPipeCylinderFlags pipeCylinderFlags;
        public Pointer trackCurvesPtr;
        public Pointer trackCornerPtr;
        public ArrayPointer childrenPtrs;
        public float3 localScale;
        public float3 localRotation;
        public float3 localPosition;
        [NumFormat(format0: "{0}", numDigits: 8, numBase: 2)]
        public byte unk_0x38; // mixed flags
        [NumFormat(format0: "{0}", numDigits: 8, numBase: 2)]
        public byte unk_0x39; // exclusive flags
        [NumFormat(format0: "{0}", numDigits: 8, numBase: 2)]
        public byte unk_0x3A; // mixed flags
        [NumFormat(format0: "{0}", numDigits: 8, numBase: 2)]
        public byte unk_0x3B; // mixed flags
        public float railHeightRight;
        public float railHeightLeft;
        public uint zero_0x44; // zero confirmed
        public uint zero_0x48; // zero confirmed
        public int branchIndex; // 0, 1, 2, 3
        // REFERENCE FIELDS
        public TrackCurves trackCurves;
        public TrackCorner trackCorner;
        // HACK?
        public int[] childIndexes = new int[0];

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
                reader.ReadX(ref segmentType);
                reader.ReadX(ref embeddedPropertyType);
                reader.ReadX(ref perimeterFlags);
                reader.ReadX(ref pipeCylinderFlags);
                reader.ReadX(ref trackCurvesPtr);
                reader.ReadX(ref trackCornerPtr);
                reader.ReadX(ref childrenPtrs);
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
                // Read Topology
                reader.JumpToAddress(trackCurvesPtr);
                reader.ReadX(ref trackCurves, true);

                // Read hairpin turn
                if (trackCornerPtr.IsNotNull)
                {
                    reader.JumpToAddress(trackCornerPtr);
                    reader.ReadX(ref trackCorner, true);
                }

                // TODO: make this functional
                // Create a matrix for convenience
                //localMatrix = float4x4.TRS(localPosition, quaternion.EulerXYZ(localRotation), localScale);
                // Update values based on children
                //worldMatrix = localMatrix;

                // Assertions
                Assert.IsTrue(zero_0x44 == 0);
                Assert.IsTrue(zero_0x48 == 0);
            }
            this.SetReaderToEndAddress(reader);
        }

        public void SetChildPointers(TrackSegment[] children)
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
                //Assert.IsTrue(currAddress == nextAddress, $"Curr[{i}]:{currAddress}, Next[{i+1}]:{nextAddress}");
            }

            // Create a pointer given the children
            if (childIndexes.Length > 0)
            {
                var index0 = childIndexes[0];
                var arrayBaseChild = children[index0];
                childrenPtrs = new ArrayPointer(
                    childIndexes.Length,
                    arrayBaseChild.GetPointer());
            }
            else
            {
                childrenPtrs = new ArrayPointer();
            }
        }

        public TrackSegment[] GetChildren(BinaryReader reader)
        {
            // Read children recusively
            var children = new TrackSegment[0];
            if (childrenPtrs.IsNotNull)
            {
                // NOTE: children are always sequential (ArrayPointer)
                reader.JumpToAddress(childrenPtrs);
                reader.ReadX(ref children, childrenPtrs.Length, true);
            }

            //
            this.childSegments = children;

            return children;
        }

        public TrackSegment[] GetChildren(TrackSegment[] allTrackSegments)
        {
            var children = new System.Collections.Generic.List<TrackSegment>();
            foreach (var index in childIndexes)
            {
                children.Add(allTrackSegments[index]);
            }
            return children.ToArray();
        }

        public void Serialize(BinaryWriter writer)
        {
            {
                // Child pointers are handled by ColiScene due to the
                // recursive nature of this type.
                // See "SetChildPointers(TrackSegment[] children)"

                trackCurvesPtr = trackCurves.GetPointer();
                trackCornerPtr = trackCorner.GetPointer();
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(segmentType);
                writer.WriteX(embeddedPropertyType);
                writer.WriteX(perimeterFlags);
                writer.WriteX(pipeCylinderFlags);
                writer.WriteX(trackCurvesPtr);
                writer.WriteX(trackCornerPtr);
                writer.WriteX(childrenPtrs);
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
            // Make sure references and pointers line up right
            Assert.ReferencePointer(trackCurves, trackCurvesPtr);
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

        public override string ToString()
        {
            var builder = new System.Text.StringBuilder();
            builder.Append(nameof(TrackSegment));
            builder.Append("(Children [");
            builder.Append(childIndexes.Length);
            builder.Append("]");
            foreach (var childIndex in childIndexes)
            {
                builder.Append(", ");
                builder.Append(childIndex);
            }
            builder.Append(")");

            return builder.ToString();
        }

        public string ToString2()
        {
            var builder = new System.Text.StringBuilder();
            builder.Append($"{nameof(TrackSegment)}");
            //
            builder.Append($"\n\t{nameof(segmentType)}: {segmentType}");
            builder.Append($"\n\t{nameof(embeddedPropertyType)}: {embeddedPropertyType}");
            builder.Append($"\n\t{nameof(perimeterFlags)}: {perimeterFlags}");
            builder.Append($"\n\t{nameof(segmentType)}: {segmentType}");
            //
            builder.Append($"\n\t{nameof(trackCurvesPtr)}: {trackCurvesPtr}");
            builder.Append($"\n\t{nameof(trackCornerPtr)}: {trackCornerPtr}");
            builder.Append($"\n\t{nameof(childrenPtrs)}: {childrenPtrs}");
            //
            builder.Append($"\n\t{nameof(localScale)}: {localScale}");
            builder.Append($"\n\t{nameof(localRotation)}: {localRotation}");
            builder.Append($"\n\t{nameof(localPosition)}: {localPosition}");
            //
            builder.Append($"\n\t{nameof(unk_0x38)}: {unk_0x38}");
            builder.Append($"\n\t{nameof(unk_0x39)}: {unk_0x39}");
            builder.Append($"\n\t{nameof(unk_0x3A)}: {unk_0x3A}");
            builder.Append($"\n\t{nameof(unk_0x3B)}: {unk_0x3B}");
            //
            builder.Append($"\n\t{nameof(zero_0x44)}: {zero_0x44}");
            builder.Append($"\n\t{nameof(zero_0x48)}: {zero_0x48}");
            //
            builder.Append($"\n\t{nameof(railHeightRight)}: {railHeightRight}");
            builder.Append($"\n\t{nameof(railHeightLeft)}: {railHeightLeft}");
            builder.Append($"\n\t{nameof(branchIndex)}: {branchIndex}");

            return builder.ToString();
        }



        // 2022/01/25: trying to work without Unity serialization messing things up
        // this means we can store a recursive structure (while state is active).
        public TrackSegment[] childSegments;

        public TrackSegment[] GetChildrenArrayPointerOrdered()
        {
            var trackSegmentHierarchy = new List<TrackSegment>();
            // Add root/parent as first element
            trackSegmentHierarchy.Add(this);
            // Kick off recursive collection of TrackSegments
            GetChildrenRecursively(this, trackSegmentHierarchy);
            // Return our list/array which is ready to be serialized to disk
            return trackSegmentHierarchy.ToArray();
        }

        public static void GetChildrenRecursively(TrackSegment parent, List<TrackSegment> segments)
        {
            if (parent.childSegments == null)
                return;

            // Add children sequentially, needed to store ArrayPointer correctly
            segments.AddRange(parent.childSegments);

            // Then add each child's children sequentially, recursively
            foreach (var childSegment in parent.childSegments)
            {
                GetChildrenRecursively(childSegment, segments);
            }
        }





    }
}
