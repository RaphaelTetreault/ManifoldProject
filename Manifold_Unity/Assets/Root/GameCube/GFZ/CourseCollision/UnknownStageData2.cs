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

        // TODO: accessors which name the animation curves?

        public ArrayPointer[] animationCurvePtrs;
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
            // Store current address since we will have to come back and overwrite it
            var structureAddress = writer.GetPositionAsPointer();

            // Serialize structure with null/garbage pointers 
            SerializeStructure(writer);
            // Serialize references
            SerializeReferences(writer);
            // Got back to structure, rewrite with real pointers
            writer.JumpToAddress(structureAddress);
            SerializeStructure(writer);

            // Go back to end of stream
            writer.SeekEnd();

        }

        private void SerializeStructure(BinaryWriter writer)
        {
            // There shoul ALWAYS be 6 animation curves, even if some are "null"
            Assert.IsTrue(animationCurves.Length == kElementCount);

            this.RecordStartAddress(writer);
            {
                // Write all array pointers in succession
                foreach (var arrayPointer in animationCurvePtrs)
                    writer.WriteX(arrayPointer);
            }
            this.RecordEndAddress(writer);
        }

        private void SerializeReferences(BinaryWriter writer)
        {
            // There shoul ALWAYS be 6 animation curve pointers, even if some are "null"
            Assert.IsTrue(animationCurvePtrs.Length == kElementCount);

            for (int i = 0; i < animationCurves.Length; i++)
            {
                // Make ref easy
                var animationCurve = animationCurves[i];

                // Write ASCII comment header
                if (ColiCourseUtility.SerializeVerbose)
                {
                    writer.CommentNewLine(true, ' ');
                    writer.CommentNewLine(true, '-');
                    writer.CommentType<AnimationCurve>(true);
                    writer.CommentPtr(ColiCourseUtility.Pointer, true);
                    writer.CommentCnt(animationCurve.Length, true, format:"x8");
                    writer.CommentCnt(animationCurve.Length, true);
                    writer.CommentNewLine(true, '-');
                }

                // Serialize structure
                var pointer = animationCurve.SerializeWithReference(writer).GetPointer();
                // Assign array pointer for this animation curve
                animationCurvePtrs[i] = new ArrayPointer(animationCurve.Length, pointer.address);
            }
        }

        public AddressRange SerializeWithReference(BinaryWriter writer)
        {
            // According to notes, this structure can be null.
            // If so, return new pointer un-init.
            // This is a protection against an init but empty struct.
            bool isNotNull = false;
            foreach (var animationCurve in animationCurves)
            {
                if (animationCurve.Length != 0)
                {
                    isNotNull = true;
                    break;
                }
            }

            if (isNotNull)
            {
                Serialize(writer);
                return addressRange;
            }
            else
            {
                return new AddressRange();
            }
        }

    }
}
