using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.CourseCollision
{
    public enum TrackTransformHierarchyDepth : byte
    {
        UNK_0 = 1 << 0,
        UNK_1 = 1 << 1,
        UNK_2 = 1 << 2,
        UNK_3 = 1 << 3,
        UNK_4 = 1 << 4,
        UNK_5 = 1 << 5,
        UNK_6 = 1 << 6,
        UNK_7 = 1 << 7,
    }

    [Serializable]
    public class TrackTransform : IBinarySerializable, IBinaryAddressable
    {

        #region MEMBERS

        public const byte kStructureSize = 0x50;
        public const byte kHasChildren = 0x0C;
        public const byte kSizeOfZero0x44 = 12;

        [SerializeField, Hex] long startAddress;
        [SerializeField, Hex] long endAddress;

        public TrackTransformHierarchyDepth hierarchyDepth;
        public byte zero_0x01;
        public byte hasChildren;
        public byte zero_0x03;
        public int topologyParamsAbsPtr;
        public uint unk_0x08_absPtr;
        [Hex(8)]
        public int childCount;
        [Hex(8)]
        public int childrenAbsPtr;
        public Vector3 localScale;
        public Vector3 localRotation;
        public Vector3 localPosition;
        public uint unk_0x38;
        public float unk_0x3C;
        public float unk_0x40;
        public uint zero_0x44;
        public uint zero_0x48;
        public uint unk_0x4C;

        public ExtraTransform extraTransform;
        public TopologyParameters topologyParameters;
        public TrackTransform[] children = new TrackTransform[0];

        #endregion

        #region PROPERTIES

        public long StartAddress
        {
            get => startAddress;
            set => startAddress = value;
        }

        public long EndAddress
        {
            get => endAddress;
            set => endAddress = value;
        }

        #endregion

        #region METHODS

        public void Deserialize(BinaryReader reader)
        {
            startAddress = reader.BaseStream.Position;

            reader.ReadX(ref hierarchyDepth);
            reader.ReadX(ref zero_0x01);
            reader.ReadX(ref hasChildren);
            reader.ReadX(ref zero_0x03);
            reader.ReadX(ref topologyParamsAbsPtr);
            reader.ReadX(ref unk_0x08_absPtr);
            reader.ReadX(ref childCount);
            reader.ReadX(ref childrenAbsPtr);
            reader.ReadX(ref localScale);
            reader.ReadX(ref localRotation);
            reader.ReadX(ref localPosition); //AvEditorUtil.InvertX(ref localPosition);
            reader.ReadX(ref unk_0x38);
            reader.ReadX(ref unk_0x3C);
            reader.ReadX(ref unk_0x40);
            reader.ReadX(ref zero_0x44);
            reader.ReadX(ref zero_0x48);
            reader.ReadX(ref unk_0x4C);

            endAddress = reader.BaseStream.Position;

            // Read Topology
            reader.BaseStream.Seek(topologyParamsAbsPtr, SeekOrigin.Begin);
            reader.ReadX(ref topologyParameters, true);

            // Read Extra Transform
            if (unk_0x08_absPtr != 0)
            {
                reader.BaseStream.Seek(unk_0x08_absPtr, SeekOrigin.Begin);
                reader.ReadX(ref extraTransform, true);
            }

            // Read Children (recursive)
            children = new TrackTransform[childCount];
            for (int i = 0; i < children.Length; i++)
            {
                var offset = kStructureSize * i;
                reader.BaseStream.Seek(childrenAbsPtr + offset, SeekOrigin.Begin);
                reader.ReadX(ref children[i], true);
            }

            reader.BaseStream.Seek(endAddress, SeekOrigin.Begin);
        }

        public void Serialize(BinaryWriter writer)
        {
            // TODO
            // Write casted 0s in place of zero_0x##

            writer.WriteX(hierarchyDepth);
            writer.WriteX(zero_0x01);
            writer.WriteX(hasChildren);
            writer.WriteX(zero_0x03);
            writer.WriteX(topologyParamsAbsPtr);
            writer.WriteX(unk_0x08_absPtr);
            writer.WriteX(childCount);
            writer.WriteX(childrenAbsPtr);
            writer.WriteX(localScale);
            writer.WriteX(localRotation);
            writer.WriteX(localPosition);
            writer.WriteX(unk_0x38);
            writer.WriteX(unk_0x3C);
            writer.WriteX(unk_0x40);
            writer.WriteX(zero_0x44);
            writer.WriteX(zero_0x48);
            writer.WriteX(unk_0x4C);
            //for (int i = 0; i < kSizeOfZero0x44; i++)
            //    writer.WriteX((byte)0);
        }

        #endregion

    }
}
