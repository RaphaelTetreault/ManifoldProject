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
        IBinaryAddressable,
        IBinarySerializable
    {
        // CONSTANTS
        public const int kCurveCount = 6;

        // FIELDS
        [UnityEngine.SerializeField]
        private AddressRange addressRange;

        // TODO: accessors which name the animation curves?

        public ArrayPointer[] animationCurvePtrs;
        public AnimationCurve[] animationCurves = new AnimationCurve[kCurveCount];

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
                reader.ReadX(ref animationCurvePtrs, kCurveCount, true);
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
            {
                // Ensure we have the correct amount of animation curves before indexing
                Assert.IsTrue(animationCurves.Length == kCurveCount);
                // Construct ArrayPointer2D for animation curves
                animationCurvePtrs = new ArrayPointer[kCurveCount];
                for (int i = 0; i < animationCurvePtrs.Length; i++)
                    animationCurvePtrs[i] = animationCurves[i].GetArrayPointer();
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(animationCurvePtrs, false);
            }
            this.RecordEndAddress(writer);
        }

        private void SerializeStructure(BinaryWriter writer)
        {
            // There shoul ALWAYS be 6 animation curves, even if some are "null"
            Assert.IsTrue(animationCurves.Length == kCurveCount);

            this.RecordStartAddress(writer);
            {
                // Write all array pointers in succession
                foreach (var arrayPointer in animationCurvePtrs)
                    writer.WriteX(arrayPointer);
            }
            this.RecordEndAddress(writer);
        }

    }
}
