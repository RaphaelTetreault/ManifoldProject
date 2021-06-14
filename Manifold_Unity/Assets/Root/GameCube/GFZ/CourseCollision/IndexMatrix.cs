using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public abstract class IndexMatrix :
        IBinaryAddressable,
        IBinarySerializable,
        ISerializedBinaryAddressableReferer
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
        public Pointer[] indexArrayPtrs = new Pointer[0];
        // REFERENCE FIELDS
        public IndexList[] indexLists;


        public IndexMatrix()
        {
            // Initialize array to default/const size.
            // Requires inheriter to finalize count.
            indexLists = new IndexList[Count];
            for (int i = 0; i < indexLists.Length; i++)
            {
                indexLists[i] = new IndexList();
            }
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
                reader.ReadX(ref indexArrayPtrs, Count, true);
            }
            this.RecordEndAddress(reader);
            {
                indexLists = new IndexList[Count];

                // Should always be init to const size by now
                Assert.IsTrue(indexArrayPtrs.Length == Count);
                Assert.IsTrue(indexLists.Length == Count);

                for (int i = 0; i < Count; i++)
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

                // Calculate metadata
                largestIndex = GetLargestIndex(indexLists);
                hasIndexes = HasAnyIndexes(indexLists);
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            //// Print out comments if required.
            //// Do one big check since there are many useless calls otherwise.
            //if (ColiCourseUtility.SerializeVerbose)
            //{
            //    // Gather some metadata
            //    var type = GetType(); // Since there is inheritance, get type dynamically.
            //    var index = ColiCourseUtility.Index; // setting up data in static class to get print index
            //    var listCount = 0;
            //    foreach (var indexList in indexLists)
            //        listCount += indexList.Length > 0 ? 1 : 0;

            //    // Write this data type and associated metadata
            //    writer.CommentAlign(true);
            //    writer.CommentNewLine(true, padding: '-');
            //    writer.Comment(type.Name, true);
            //    if (type == typeof(StaticMeshTableIndexes))
            //    {
            //        writer.CommentLineWide("T:", $"{(StaticMeshColliderProperty)index}", true);
            //        writer.CommentIdx(index, true);
            //    }
            //    writer.CommentPtr(ColiCourseUtility.Pointer, true, padding: ' ');
            //    writer.CommentLineWide("Lists:", $"{listCount}", true);
            //    writer.CommentLineWide("LargestIdx:", $"{largestIndex}", true);
            //    writer.CommentNewLine(true, padding: '-');
            //}


            {
                // Ensure we have the correct amount of lists before indexing
                Assert.IsTrue(indexLists.Length == Count);

                // Construct Pointer[]
                var pointers = new Pointer[Count];
                for (int i = 0; i < pointers.Length; i++)
                    pointers[i] = indexLists[i].GetPointer();
                indexArrayPtrs = pointers;
            }
            this.RecordStartAddress(writer);
            {
                ValidateReferences();

                for (int i = 0; i < indexLists.Length; i++)
                {
                    var indexList = indexLists[i];
                    if (indexList.Length > 0)
                    {
                        // TODO: resolve pointer
                        // ALSO, StaticMeshTable PROBABLY maps collision into 16x16 table as does checkpoints!!!
                        throw new NotImplementedException();
                        //indexArrayPtrs[i] = indexList.SerializeWithReference(writer).GetPointer();
                    }
                }
            }
            this.RecordEndAddress(writer);
        }

        public void ValidateReferences()
        {
            // Should always be init to const size
            Assert.IsTrue(indexArrayPtrs.Length == Count);
            Assert.IsTrue(indexLists.Length == Count);
        }
    }
}
