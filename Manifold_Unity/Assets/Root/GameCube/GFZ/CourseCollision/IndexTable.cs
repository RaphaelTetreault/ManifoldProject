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

        public bool IsEmpty => !HasIndexes;
        public bool HasIndexes => largestIndex > 1;

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
            // Print out comments if required/
            // Do one big check since there are many useless calls otherwise.
            if (ColiCourseUtility.SerializeVerbose)
            {
                // Gather some metadata
                var index = ColiCourseUtility.Index;
                var listCount = 0;
                foreach (var indexList in indexLists)
                    listCount += indexList.Length > 0 ? 1 : 0;
                // Since there is inheritance, get type dynamically.
                var type = GetType();

                // Write comment
                writer.CommentAlign(true);
                writer.CommentNewLine(true, padding: '-');
                writer.Comment(type.Name, true);
                if (type == typeof(MeshIndexTable))
                {
                    writer.Comment($"T:{(StaticMeshColliderProperty)index,14}", true);
                    writer.Comment($"Index:{index,10}", true);
                }
                writer.CommentPtr(ColiCourseUtility.Pointer, true, padding: ' ');
                writer.Comment($"Lists:{listCount,10}", true);
                writer.CommentNewLine(true, padding: '-');
            }

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
                        indexArrayPtrs[i] = indexList.SerializeWithReference(writer).GetPointer();
                    }
                }
            }
            this.RecordEndAddress(writer);
        }

        public AddressRange SerializeWithReference(BinaryWriter writer)
        {
            if (IsEmpty)
            {
                ColiCourseUtility.Index++;
                // table has no mesh, so save nothing
                return new AddressRange();
            }
            else
            {
                // mesh has data, do the thing
                Serialize(writer);
                ColiCourseUtility.Index++;
                return addressRange;
            }
        }

    }
}
