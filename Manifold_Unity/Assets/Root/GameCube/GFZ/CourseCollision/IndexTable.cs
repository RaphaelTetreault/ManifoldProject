using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public abstract class IndexTable :
        IBinarySeralizableReference
    {
        public static int Index { get; set; } = 0;
        public static void ResetDebugIndex()
        {
            Index = 0;
        }

        // "CONSTANTS"
        public abstract int ListCount { get; }

        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;
        public ushort largestIndex;

        // FIELDS
        public Pointer[] indexArrayPtrs = new Pointer[0];
        public IndexList[] indexLists = new IndexList[0];

        // PROPERTIES
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        public bool IsEmpty => largestIndex <= 1;

        // METHODS
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

            // Indices are n-1, so to compentsate ++n
            return ++largestIndex;
        }

        public void Deserialize(BinaryReader reader)
        {
            // Read index arrays
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref indexArrayPtrs, ListCount, true);
            }
            this.RecordEndAddress(reader);
            {
                indexLists = new IndexList[ListCount];

                // Should always be init to const size by now
                Assert.IsTrue(indexArrayPtrs.Length == ListCount);
                Assert.IsTrue(indexLists.Length == ListCount);

                for (int i = 0; i < ListCount; i++)
                {
                    // init value
                    indexLists[i] = new IndexList();

                    var indexArrayPtr = indexArrayPtrs[i];
                    if (indexArrayPtr.IsNotNullPointer)
                    {
                        var indexList = indexLists[i];
                        reader.JumpToAddress(indexArrayPtr);
                        reader.ReadX(ref indexList, false);
                    }
                }

                // Calculate largest index.
                // Needed to construct vertices. Not sure about other type...
                largestIndex = GetLargestIndex(indexLists);
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            // Since there is inheritance, get type dynamically.
            writer.Comment(GetType().Name, ColiCourseUtility.SerializeVerbose);
            writer.Comment($"Index:{Index++:00}", ColiCourseUtility.SerializeVerbose);

            this.RecordStartAddress(writer);
            {
                // Should always be init to const size
                Assert.IsTrue(indexArrayPtrs.Length == ListCount);
                Assert.IsTrue(indexLists.Length == ListCount);

                for (int i = 0; i < indexLists.Length; i++)
                {
                    var indexList = indexLists[i];
                    if (indexList.Length > 0)
                    {
                        indexArrayPtrs[i] = indexList.SerializeReference(writer).GetPointer();
                    }
                }
            }
            this.RecordEndAddress(writer);

            //
            writer.CommentNewLine(ColiCourseUtility.SerializeVerbose);
        }

        public AddressRange SerializeReference(BinaryWriter writer)
        {
            if (IsEmpty)
            {
                Index++;
                // table has no mesh, so save nothing
                return new AddressRange();
            }
            else
            {
                // mesh has data, do the thing
                Serialize(writer);
                return addressRange;
            }
        }

    }
}
