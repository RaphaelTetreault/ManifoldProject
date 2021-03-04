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

        public const int kIdxPtrArraySize = 0x100; // 256

        public int[] indicesAbsPtrs = new int[kIdxPtrArraySize];
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
            reader.ReadX(ref indicesAbsPtrs, kIdxPtrArraySize);
            this.RecordEndAddress(reader);

            // Should always be init to const size
            System.Diagnostics.Debug.Assert(indicesAbsPtrs.Length == kIdxPtrArraySize);

            // not recording length of other data due to [][]
            for (int pointerIndex = 0; pointerIndex < indicesAbsPtrs.Length; pointerIndex++)
            {
                var absPtr = indicesAbsPtrs[pointerIndex];
                if (absPtr > 0)
                {
                    reader.BaseStream.Seek(absPtr, SeekOrigin.Begin);
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
