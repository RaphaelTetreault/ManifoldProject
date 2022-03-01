using Manifold;
using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// A grid of index lists. Grid is used as base class for other *Grid types
    /// to index collider triangles/quads (static meshes) and checkpoint nodes.
    /// </summary>
    [Serializable]
    public abstract class IndexGrid :
        IBinaryAddressable,
        IBinarySerializable,
        IHasReference
    {
        // "CONSTANTS"
        public int Count => SubdivisionsX * SubdivisionsZ;
        public abstract int SubdivisionsX { get; }
        public abstract int SubdivisionsZ { get; }


        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;
        [UnityEngine.SerializeField] private ushort largestIndex;
        [UnityEngine.SerializeField] private bool hasIndexes;

        // FIELDS
        public Pointer[] indexListPtrs;
        // REFERENCE FIELDS
        public IndexList[] indexLists;


        public IndexGrid()
        {
            // Initialize array to default/const size.
            // Requires inheriter to finalize count.
            indexLists = new IndexList[Count];
            for (int i = 0; i < indexLists.Length; i++)
            {
                indexLists[i] = new IndexList();
            }
            //
            //indexListPtrs = new Pointer[Count];
        }


        // PROPERTIES
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        public bool HasIndexes => hasIndexes;
        public ushort LargestIndex => largestIndex;
        public ushort IndexesLength
        {
            get
            {
                // Returns index length based on metadata
                return hasIndexes
                    ? (ushort)(largestIndex + 1)
                    : (ushort)(0);
            }
        }


        // METHODS
        private bool HasAnyIndexes(IndexList[] indexLists)
        {
            foreach (var indexList in indexLists)
            {
                if (indexList.Length > 0)
                    return true;
            }

            return false;
        }

        private ushort GetLargestIndex(IndexList[] indexLists)
        {
            // Find the largest known index to use as tri/quad array size
            // The game probably just reads indices dynamically using address + index * tri/quad size

            // Record largest idnex
            ushort largestIndex = 0;

            // Iterate through all indices to find largest
            foreach (var indexList in indexLists)
            {
                foreach (var index in indexList.Indexes)
                {
                    if (index > largestIndex)
                    {
                        largestIndex = index;
                    }
                }
            }

            return largestIndex;
        }

        public void Deserialize(BinaryReader reader)
        {
            // Read index arrays
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref indexListPtrs, Count);
            }
            this.RecordEndAddress(reader);
            {
                indexLists = new IndexList[Count];

                // Should always be init to const size by now
                Assert.IsTrue(indexListPtrs.Length == Count);
                Assert.IsTrue(indexLists.Length == Count);

                for (int i = 0; i < Count; i++)
                {
                    // init value
                    indexLists[i] = new IndexList();

                    var indexArrayPtr = indexListPtrs[i];
                    if (indexArrayPtr.IsNotNull)
                    {
                        reader.JumpToAddress(indexArrayPtr);
                        indexLists[i].Deserialize(reader);
                    }
                }

                // Calculate metadata
                largestIndex = GetLargestIndex(indexLists);
                hasIndexes = HasAnyIndexes(indexLists);
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            {
                // Ensure we have the correct amount of lists before indexing
                Assert.IsTrue(indexLists.Length == Count);

                // Construct Pointer[]
                var pointers = new Pointer[Count];
                for (int i = 0; i < pointers.Length; i++)
                    pointers[i] = indexLists[i].GetPointer();
                indexListPtrs = pointers;
            }
            this.RecordStartAddress(writer);
            {
                //ValidateReferences();

                // Write all pointers
                for (int ptrIndex = 0; ptrIndex < indexListPtrs.Length; ptrIndex++)
                {
                    var ptr = indexListPtrs[ptrIndex];
                    writer.WriteX(ptr);
                }
            }
            this.RecordEndAddress(writer);
        }

        public void ValidateReferences()
        {
            // Should always be init to const size
            Assert.IsTrue(indexListPtrs.Length == Count);
            Assert.IsTrue(indexLists.Length == Count);

            for (int i = 0; i < Count; i++)
            {
                var indexList = indexLists[i];
                var pointer = indexListPtrs[i];

                if (indexList.Length != 0)
                    Assert.ReferencePointer(indexList, pointer);
            }
        }
    }
}
