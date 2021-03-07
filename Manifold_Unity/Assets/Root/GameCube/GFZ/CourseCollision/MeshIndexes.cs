using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class MeshIndexes : IBinarySerializable, IBinaryAddressableRange
    {

        #region FIELDS


        [SerializeField]
        private AddressRange addressRange;

        public const int kIndexArrayPtrsSize = 256; // 0x100

        public Pointer[] indexArrayPtrs = new Pointer[kIndexArrayPtrsSize];
        public ushort[][] indexes = new ushort[kIndexArrayPtrsSize][];


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
            // Read index arrays
            this.RecordStartAddress(reader);
            reader.ReadX(ref indexArrayPtrs, kIndexArrayPtrsSize, false);
            this.RecordEndAddress(reader);

            // Should always be init to const size
            System.Diagnostics.Debug.Assert(indexArrayPtrs.Length == kIndexArrayPtrsSize);

            // TODO:  not recording length of other data due to [][]
            for (int pointerIndex = 0; pointerIndex < indexArrayPtrs.Length; pointerIndex++)
            {
                var pointer = indexArrayPtrs[pointerIndex];
                if (pointer.IsNotNullPointer)
                {
                    //Debug.Log($"ptr{pointerIndex:000}:{pointer.HexAddress}");
                    reader.JumpToAddress(pointer);
                    indexes[pointerIndex] = ColiCourseUtility.ReadUShortArray(reader);
                }
                else
                {
                    indexes[pointerIndex] = new ushort[0];
                }
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }


        #endregion

    }
}
