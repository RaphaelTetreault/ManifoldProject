using Manifold.IO;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class TrackTransform : IBinarySerializable, IBinaryAddressableRange
    {

        [Flags]
        public enum TrackUnkFlag2 : byte
        {
            None = 0,
            Unused_0 = 1 << 0,
            UNK_1 = 1 << 1,
            UNK_2 = 1 << 2,
            UNK_3 = 1 << 3,
            UNK_4 = 1 << 4,
            UNK_5 = 1 << 5,
            UNK_6 = 1 << 6,
            UNK_7 = 1 << 7,
        }

        [Flags]
        public enum TrackUnkFlag3 : byte
        {
            None = 0,
            Unused0 = 1 << 0,
            Unused1 = 1 << 1,
            hasRailHeightRight = 1 << 2,
            hasRailHeightLeft  = 1 << 3,
            UNK_4 = 1 << 4,
            UNK_5 = 1 << 5,
            Unused6 = 1 << 6,
            Unused7 = 1 << 7,
        }

        public enum TrackUnkOption1 : byte
        {
            option_0,
            option_1,
            option_2,
            option_3,
        }

        public enum TrackUnkOption2 : uint
        {
            option_0,
            option_1,
            option_2,
            option_3,
            option_4,
        }

        #region FIELDS


        public const byte kStructureSize = 0x50;
        public const byte kHasChildren = 0x0C;

        [SerializeField]
        private AddressRange addressRange;

        
        public TrackUnkFlag1 unk_0x00; // 0, 1, 2, 4, 8
        public TrackUnkFlag2 unk_0x01; // 0, 2, 4, 8, 16, 32, 64, 128
        public TrackUnkFlag3 unk_0x02; // 0, 4, 8, 12, 20, 28, 44
        public TrackUnkOption1 unk_0x03; // 0, 1, 2 (interpolation?)
        public Pointer transformTopologyPtr; // SEGMENT TRANSFORM?
        public Pointer sliceTopologyPtr; // ???
        public ArrayPointer childrenPtr;
        public Vector3 localScale;
        public Vector3 localRotation;
        public Vector3 localPosition;
        public ushort unk_0x38; // lots of values
        public ushort unk_0x3A; // lots of values
        public float railHeightRight;
        public float railHeightLeft;
        public uint zero_0x44; // zero confirmed
        public uint zero_0x48; // zero confirmed
        public TrackUnkOption2 unk_0x4C; // 0, 1, 2, 3 (interpolation?)

        public TopologyParameters transformTopology; // transform anim data 
        public TopologyExtra sliceTopology;
        public TrackTransform[] children = new TrackTransform[0];
        public Matrix4x4 localMatrix;
        public Matrix4x4 worldMatrix;
        public int depth;

        #endregion

        #region PROPERTIES


        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }


        #endregion

        #region METHODS


        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref unk_0x00);
                reader.ReadX(ref unk_0x01);
                reader.ReadX(ref unk_0x02);
                reader.ReadX(ref unk_0x03);
                reader.ReadX(ref transformTopologyPtr);
                reader.ReadX(ref sliceTopologyPtr);
                reader.ReadX(ref childrenPtr);
                reader.ReadX(ref localScale);
                reader.ReadX(ref localRotation);
                reader.ReadX(ref localPosition); //AvEditorUtil.InvertX(ref localPosition);
                reader.ReadX(ref unk_0x38);
                reader.ReadX(ref unk_0x3A);
                reader.ReadX(ref railHeightRight);
                reader.ReadX(ref railHeightLeft);
                reader.ReadX(ref zero_0x44);
                reader.ReadX(ref zero_0x48);
                reader.ReadX(ref unk_0x4C);
            }
            this.RecordEndAddress(reader);
            {
                // Read Topology
                reader.JumpToAddress(transformTopologyPtr);
                reader.ReadX(ref transformTopology, true);

                // Read Extra Transform
                if (sliceTopologyPtr.IsNotNullPointer)
                {
                    reader.JumpToAddress(sliceTopologyPtr);
                    reader.ReadX(ref sliceTopology, true);
                }

                // Create a matrix for convinience
                localMatrix.SetTRS(localPosition, Quaternion.Euler(localRotation), localScale);
                worldMatrix = localMatrix;

                // Read children recusively
                if (childrenPtr.IsNotNullPointer)
                {
                    reader.JumpToAddress(childrenPtr);
                    reader.ReadX(ref children, childrenPtr.length, true);

                    foreach (var child in children)
                    {
                        child.worldMatrix = worldMatrix * child.localMatrix;
                        SetChildIndex(child);
                    }
                }


            }
            this.SetReaderToEndAddress(reader);
        }

        public void SetChildIndex(TrackTransform parent)
        {
            foreach (var child in parent.children)
            {
                child.depth = parent.depth + 1;
                SetChildIndex(child);
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            // TODO
            // Write casted 0s in place of zero_0x##

            writer.WriteX(unk_0x00);
            writer.WriteX(unk_0x01);
            writer.WriteX(unk_0x02);
            writer.WriteX(unk_0x03);
            writer.WriteX(transformTopologyPtr);
            writer.WriteX(sliceTopology);
            writer.WriteX(childrenPtr);
            writer.WriteX(localScale);
            writer.WriteX(localRotation);
            writer.WriteX(localPosition);
            writer.WriteX(unk_0x38);
            writer.WriteX(railHeightRight);
            writer.WriteX(railHeightLeft);
            writer.WriteX(zero_0x44);
            writer.WriteX(zero_0x48);
            writer.WriteX(unk_0x4C);

            throw new NotImplementedException();
        }


        #endregion

    }
}
