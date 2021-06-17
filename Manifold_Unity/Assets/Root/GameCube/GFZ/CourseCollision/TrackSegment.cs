using Manifold.IO;
using System;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class TrackSegment :
        IBinaryAddressable,
        IBinarySerializable,
        ISerializedBinaryAddressableReferer
    {
        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;
        public float4x4 localMatrix;
        public float4x4 worldMatrix;
        public int depth;

        // FIELDS
        public TrackTopologyMetadata topologyMetadata;
        public TrackProperty trackProperty;
        public TrackPerimeterOptions perimeterOptions;
        public TrackPipeCylinderOptions pipeCylinderOptions;
        public Pointer generalTopologyPtr;
        public Pointer hairpinCornerTopologyPtr;
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
        public TrackUnkOption2 unk_0x4C; // 0, 1, 2, 3
        // REFERENCE FIELDS
        public TopologyParameters trackSegment;
        public TrackCornerTopology hairpinCornerTopology;
        public int[] childIndexes = new int[0];
        //public TrackSegment[] graph = new TrackSegment[0];


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
                reader.ReadX(ref topologyMetadata);
                reader.ReadX(ref trackProperty);
                reader.ReadX(ref perimeterOptions);
                reader.ReadX(ref pipeCylinderOptions);
                reader.ReadX(ref generalTopologyPtr);
                reader.ReadX(ref hairpinCornerTopologyPtr);
                reader.ReadX(ref childrenPtrs);
                reader.ReadX(ref localScale);
                reader.ReadX(ref localRotation);
                reader.ReadX(ref localPosition); //AvEditorUtil.InvertX(ref localPosition);
                reader.ReadX(ref unk_0x38);
                reader.ReadX(ref unk_0x39);
                reader.ReadX(ref unk_0x3A);
                reader.ReadX(ref unk_0x3B);
                reader.ReadX(ref railHeightRight);
                reader.ReadX(ref railHeightLeft);
                reader.ReadX(ref zero_0x44);
                reader.ReadX(ref zero_0x48);
                reader.ReadX(ref unk_0x4C);
            }
            this.RecordEndAddress(reader);
            {
                // Read Topology
                reader.JumpToAddress(generalTopologyPtr);
                reader.ReadX(ref trackSegment, true);

                // Read hairpin turn
                if (hairpinCornerTopologyPtr.IsNotNullPointer)
                {
                    reader.JumpToAddress(hairpinCornerTopologyPtr);
                    reader.ReadX(ref hairpinCornerTopology, true);
                }

                // TODO: make this functional
                // Create a matrix for convenience
                localMatrix = float4x4.TRS(localPosition, quaternion.EulerXYZ(localRotation), localScale);
                // Update values based on children
                //worldMatrix = localMatrix;

                //// Read children recusively
                //if (childrenPtrs.IsNotNullPointer)
                //{
                //    reader.JumpToAddress(childrenPtrs);
                //    reader.ReadX(ref children, childrenPtrs.Length, true);

                //    foreach (var child in children)
                //    {
                //        child.worldMatrix = worldMatrix * child.localMatrix;
                //        child.parent = this;
                //    }

                //    // Calculate depth by recursively evaluating lineage
                //    var ancestor = parent;
                //    while (ancestor != null)
                //    {
                //        depth++;
                //        ancestor = ancestor.parent;
                //    }
                //}

                // Assertions
                Assert.IsTrue(zero_0x44 == 0);
                Assert.IsTrue(zero_0x48 == 0);
            }
            this.SetReaderToEndAddress(reader);
        }

        //// TODO: remove
        //public void SetChildIndex(TrackSegment parent)
        //{
        //    foreach (var child in parent.children)
        //    {
        //        child.depth = parent.depth + 1;
        //        SetChildIndex(child);
        //    }
        //}

        public void SetChildPointers(TrackSegment[] children)
        {
            children.GetArrayPointer();

            // Assert that children are truly sequential.
            // This is a HARD requirement of ArrayPointer.
            // Iterate from 0 - (n-1). If length == 1, no looping.
            for (int i = 0; i < children.Length - 1; i++)
            {
                var curr = children[i + 0];
                var next = children[i + 1];
                // The end address of the current child must be the same as the next child
                Assert.IsTrue(curr.AddressRange.startAddress == next.AddressRange.endAddress);
            }
        }

        public TrackSegment[] GetChildren(BinaryReader reader)
        {
            // Read children recusively
            var children = new TrackSegment[0];
            if (childrenPtrs.IsNotNullPointer)
            {
                // NOTE: children are always consecutive (ArrayPointer)
                reader.JumpToAddress(childrenPtrs);
                reader.ReadX(ref children, childrenPtrs.Length, true);
            }
            return children;
        }


        public void Serialize(BinaryWriter writer)
        {
            {
                // Child pointers are handled by ColiScene due to the
                // recursive nature of this type.
                // See "SetChildPointers(TrackSegment[] children)"

                generalTopologyPtr = trackSegment.GetPointer();
                hairpinCornerTopologyPtr = hairpinCornerTopology.GetPointer();
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(topologyMetadata);
                writer.WriteX(trackProperty);
                writer.WriteX(perimeterOptions);
                writer.WriteX(pipeCylinderOptions);
                writer.WriteX(generalTopologyPtr);
                writer.WriteX(hairpinCornerTopologyPtr);
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
                writer.WriteX(unk_0x4C);
            }
            this.RecordEndAddress(writer);
        }

        public void ValidateReferences()
        {
            // Topology assertions
            if (topologyMetadata == TrackTopologyMetadata.IsTransformParent)
                Assert.IsTrue(childrenPtrs.IsNotNullPointer);
            // TODO: more edge cases to assert

            // Ensure rail flags AND height properties coincide
            bool hasRailLeft = perimeterOptions.HasFlag(TrackPerimeterOptions.hasRailHeightLeft);
            bool hasRailRight = perimeterOptions.HasFlag(TrackPerimeterOptions.hasRailHeightRight);
            Assert.IsFalse(hasRailLeft ^ railHeightRight > 0);
            Assert.IsFalse(hasRailRight ^ railHeightLeft > 0);

            // Ensure that if there is a turn that one of the two flags for it are set
            bool hasTurnLeft = perimeterOptions.HasFlag(TrackPerimeterOptions.has90TurnLeft);
            bool hasTurnRight = perimeterOptions.HasFlag(TrackPerimeterOptions.has90TurnRight);
            bool isTurnNotNull = hairpinCornerTopology != null;
            Assert.IsTrue(hasTurnLeft || hasTurnRight && isTurnNotNull);
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

    }
}
