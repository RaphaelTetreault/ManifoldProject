namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    public class TrackIndexTable : IndexTable
    {
        public static int kListCount = 64;
        public override int ListCount => kListCount;
    }
}



//using Manifold.IO;
//using System.IO;

//namespace GameCube.GFZ.CourseCollision
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    public class TrackIndexesTable :
//        IBinarySeralizableReference
//    {
//        // CONSTANTS
//        public const int kCountLists = 64;

//        // METADATA
//        [UnityEngine.SerializeField]
//        private AddressRange addressRange;

//        // FIELDS
//        public Pointer[] trackIndexesPtrs = new Pointer[0];
//        public IndexList[] trackIndexArray = new IndexList[0];

//        // PROPERTIES
//        public AddressRange AddressRange
//        {
//            get => addressRange;
//            set => addressRange = value;
//        }

//        // METHODS
//        public void Deserialize(BinaryReader reader)
//        {
//            // Read index arrays
//            this.RecordStartAddress(reader);
//            {
//                reader.ReadX(ref indexArrayPtrs, kCountLists, false);
//            }
//            this.RecordEndAddress(reader);
//            {

//                for (int i = 0; i < kCountLists; i++)
//                {
//                    var indexArrayPtr = indexArrayPtrs[i];
//                    if (indexArrayPtr.IsNotNullPointer)
//                    {
//                        var indexList = indexLists[i];
//                        reader.JumpToAddress(indexArrayPtr);
//                        reader.ReadX(ref indexList, false);
//                    }
//                }
//            }
//            this.SetReaderToEndAddress(reader);
//        }
//        //public void Deserialize(BinaryReader reader)
//        //{
//        //    this.RecordStartAddress(reader);
//        //    {
//        //        reader.ReadX(ref trackIndexesPtrs, kNumEntries, true);
//        //    }
//        //    this.RecordEndAddress(reader);
//        //    {
//        //        trackIndexArray = new TrackIndexes[64];
//        //        for (int i = 0; i < trackIndexesPtrs.Length; i++)
//        //        {
//        //            var pointer = trackIndexesPtrs[i];
//        //            if (pointer.IsNotNullPointer)
//        //            {
//        //                reader.JumpToAddress(pointer);
//        //                reader.ReadX(ref trackIndexArray[i], true);
//        //            }
//        //            else
//        //            {
//        //                trackIndexArray[i] = new TrackIndexes();
//        //            }
//        //        }
//        //    }
//        //    this.SetReaderToEndAddress(reader);
//        //}

//        public void Serialize(BinaryWriter writer)
//        {
//            throw new System.NotImplementedException();
//        }

//        public AddressRange SerializeReference(BinaryWriter writer)
//        {
//            throw new System.NotImplementedException();
//        }
//    }
//}
