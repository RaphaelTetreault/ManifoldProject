using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class CollisionMeshIndices : IBinarySerializable, IBinaryAddressable
    {
        [SerializeField, Hex] long startAddress;
        [SerializeField, Hex] long endAddress;

        public const int kIdxPtrArraySize = 256; // 0x100

        public Pointer[] indexArraysPtrs = new Pointer[kIdxPtrArraySize];
        public ushort[][] indices = new ushort[kIdxPtrArraySize][];


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


        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            reader.ReadX(ref indexArraysPtrs, kIdxPtrArraySize, false);
            this.RecordEndAddress(reader);

            // Should always be init to const size
            System.Diagnostics.Debug.Assert(indexArraysPtrs.Length == kIdxPtrArraySize);

            // TODO:  not recording length of other data due to [][]
            for (int pointerIndex = 0; pointerIndex < indexArraysPtrs.Length; pointerIndex++)
            {
                var pointer = indexArraysPtrs[pointerIndex];
                if (pointer.address > 0)
                {
                    //Debug.Log($"ptr{pointerIndex:000}:{pointer.HexAddress}");
                    reader.JumpToAddress(pointer);
                    indices[pointerIndex] = ColiCourseUtil.ReadShortArray(reader);
                }
                else
                {
                    indices[pointerIndex] = new ushort[0];
                }
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }
    }
}
