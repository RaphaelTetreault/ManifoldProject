using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

// NOTE: dependant on UnknownStageData1, review notes in that class.

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class UnknownStageData2 : IBinarySerializable, IBinaryAddressableRange
    {
        public const int elementCount = 6;

        [SerializeField]
        private AddressRange addressRange;

        // TODO: since these arrays are fixed and not 2d, they should
        // be split into 6 different KeyableAttribute[]. Will be easy
        // to name when you know what they do.
        //public ArrayPointer name1Ptr;
        //public ArrayPointer name2Ptr;
        //public ArrayPointer name3Ptr;
        //public ArrayPointer name4Ptr;
        //public ArrayPointer name5Ptr;
        //public ArrayPointer name6Ptr;
        //public KeyableAttribute[] keyableAttributes1;
        //public KeyableAttribute[] keyableAttributes2;
        //public KeyableAttribute[] keyableAttributes3;
        //public KeyableAttribute[] keyableAttributes4;
        //public KeyableAttribute[] keyableAttributes5;
        //public KeyableAttribute[] keyableAttributes6;

        public ArrayPointer[] unkAnimDataPtrs;
        public KeyableAttribute[][] unkAnimData = new KeyableAttribute[elementCount][];

        public UnknownStageData2()
        {
            // Initialize values so no null errors
            unkAnimData = new KeyableAttribute[elementCount][];
            for (int i = 0; i < unkAnimData.Length; i++)
                unkAnimData[i] = new KeyableAttribute[0];
        }

        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                // read 6 array pointers
                reader.ReadX(ref unkAnimDataPtrs, elementCount, true);
            }
            this.RecordEndAddress(reader);
            {
                // Go through each array pointer
                for (int i = 0; i < unkAnimDataPtrs.Length; i++)
                {
                    // load datae for "array" (though all have length == 0)
                    var arrayPointer = unkAnimDataPtrs[i];
                    reader.JumpToAddress(arrayPointer);
                    reader.ReadX(ref unkAnimData[i], arrayPointer.Length, true);
                }
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
