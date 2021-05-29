namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    public class MeshIndexTable : IndexTable
    {
        public const int kListCount = 256;
        public override int ListCount => kListCount;
    }
}

// KEEPING
// Keeping an old copy in case the polymorphism thing doesn't pan out

//using Manifold.IO;
//using System;
//using System.IO;

//namespace GameCube.GFZ.CourseCollision
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    [Serializable]
//    public class MeshIndexes :
//        IBinarySeralizableReference
//    {
//        // CONSTANTS
//        public const int kCountPtrs = 256; // 0x100

//        // METADATA
//        [UnityEngine.SerializeField] private AddressRange addressRange;
//        public ushort largestIndex;

//        // FIELDS
//        public Pointer[] indexArrayPtrs = new Pointer[kCountPtrs];
//        public IndexList[] indexLists = new IndexList[kCountPtrs];

//        // PROPERTIES
//        public AddressRange AddressRange
//        {
//            get => addressRange;
//            set => addressRange = value;
//        }


//        // METHODS
//        private ushort GetLargestIndex(IndexList[] indexLists)
//        {
//            // Find the largest known index to use as tri/quad array size
//            // The game probably just reads indices dynamically using address + index * tri/quad size

//            // Record largest idnex
//            ushort largestIndex = 0;

//            // Iterate through all indices to find largest
//            foreach (var indexList in indexLists)
//            {
//                foreach (var index in indexList.Indexes)
//                {
//                    if (index > largestIndex)
//                    {
//                        largestIndex = index;
//                    }
//                }
//            }

//            // Indices are n-1, so to compentsate ++n
//            return ++largestIndex;
//        }

//        public void Deserialize(BinaryReader reader)
//        {
//            // Read index arrays
//            this.RecordStartAddress(reader);
//            {
//                reader.ReadX(ref indexArrayPtrs, kCountPtrs, false);
//            }
//            this.RecordEndAddress(reader);
//            {
//                // Should always be init to const size
//                Assert.IsTrue(indexArrayPtrs.Length == kCountPtrs);
//                Assert.IsTrue(indexLists.Length == kCountPtrs);

//                for (int i = 0; i < kCountPtrs; i++)
//                {
//                    var indexArrayPtr = indexArrayPtrs[i];
//                    if (indexArrayPtr.IsNotNullPointer)
//                    {
//                        var indexList = indexLists[i];
//                        reader.JumpToAddress(indexArrayPtr);
//                        reader.ReadX(ref indexList, false);
//                    }
//                }

//                // Calculate largest index. Needed to construct vertices.
//                largestIndex = GetLargestIndex(indexLists);
//            }
//            this.SetReaderToEndAddress(reader);
//        }

//        public void Serialize(BinaryWriter writer)
//        {
//            writer.Comment<MeshIndexes>(ColiCourseUtility.SerializeVerbose);

//            this.RecordStartAddress(writer);
//            {
//                // Should always be init to const size
//                Assert.IsTrue(indexArrayPtrs.Length == kCountPtrs);
//                Assert.IsTrue(indexLists.Length == kCountPtrs);

//                for (int i = 0; i < indexLists.Length; i++)
//                {
//                    var indexList = indexLists[i];
//                    indexArrayPtrs[i] = indexList.SerializeReference(writer).GetPointer();
//                }
//            }
//            this.RecordEndAddress(writer);
//        }

//        public AddressRange SerializeReference(BinaryWriter writer)
//        {
//            Serialize(writer);
//            return addressRange;
//        }

//    }
//}
