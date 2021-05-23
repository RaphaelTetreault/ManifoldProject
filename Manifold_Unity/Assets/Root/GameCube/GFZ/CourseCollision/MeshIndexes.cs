using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class MeshIndexes : IBinarySerializable, IBinaryAddressableRange
    {
        [SerializeField]
        private AddressRange addressRange;

        public const int kIndexArrayPtrsSize = 256; // 0x100

        public Pointer[] indexArrayPtrs = new Pointer[kIndexArrayPtrsSize];
        public Array2D<ushort> indexes = new Array2D<ushort>();
        public ushort largestIndex;


        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }


        public void Deserialize(BinaryReader reader)
        {
            // Read index arrays
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref indexArrayPtrs, kIndexArrayPtrsSize, false);
            }
            this.RecordEndAddress(reader);

            // Should always be init to const size
            Assert.IsTrue(indexArrayPtrs.Length == kIndexArrayPtrsSize);

            // TODO: make this thing pretty
            for (int pointerIndex = 0; pointerIndex < indexArrayPtrs.Length; pointerIndex++)
            {
                ushort[] indexesSet = new ushort[0];

                var pointer = indexArrayPtrs[pointerIndex];
                if (pointer.IsNotNullPointer)
                {
                    reader.JumpToAddress(pointer);
                    indexesSet = ColiCourseUtility.ReadUShortArray(reader);
                }

                indexes.AppendArray(indexesSet);
            }

            // Calculate largest index. Needed to construct vertices.
            largestIndex = GetLargestIndex(indexes.GetArrays());
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        private ushort GetLargestIndex(ushort[][] indexesArray)
        {
            // Find the largest known index to use as tri/quad array size
            // The game probably just reads indices dynamically using address + index * tri/quad size

            // Record largest idnex
            ushort largestIndex = 0;

            // Iterate through all indices to find largest
            foreach (var indexes in indexesArray)
            {
                foreach (var index in indexes)
                {
                    if (index > largestIndex)
                    {
                        largestIndex = index;
                    }
                }
            }

            // Indices are n-1, so to compentsate ++n
            return ++largestIndex;
        }

    }
}
