using Manifold.IO;
using System;
using System.IO;

// NOTE: dependant on UnknownStageData1, review notes in that class.

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class UnknownStageData2 :
        IBinarySeralizableReference
    {
        // CONSTANTS
        public const int kElementCount = 6;

        // FIELDS
        [UnityEngine.SerializeField]
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
        public ArrayPointer[] animationCurvePtrs;
        // needs to be Array2D<> or encapsulated class
        //public KeyableAttribute[][] unkAnimData = new KeyableAttribute[kElementCount][];
        public AnimationCurve[] animationCurves = new AnimationCurve[kElementCount];

        public UnknownStageData2()
        {
            // Initialize values so no null errors
            for (int i = 0; i < animationCurves.Length; i++)
                animationCurves[i] = new AnimationCurve(0);
        }


        // PROPERTIES
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                // read 6 array pointers
                reader.ReadX(ref animationCurvePtrs, kElementCount, true);
            }
            this.RecordEndAddress(reader);
            {
                // Go through each array pointer
                for (int i = 0; i < animationCurvePtrs.Length; i++)
                {
                    var arrayPointer = animationCurvePtrs[i];
                    var animationCurve = new AnimationCurve(arrayPointer.Length);

                    reader.JumpToAddress(arrayPointer);
                    reader.ReadX(ref animationCurve, false);

                    animationCurves[i] = animationCurve;
                }
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public AddressRange SerializeWithReference(BinaryWriter writer)
        {
            // can bu null?
            // if so, return new pointer un-init
            throw new NotImplementedException();
        }
    }
}
