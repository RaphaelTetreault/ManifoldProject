using Manifold.IO;
using System;
using System.IO;

// NOTES
// Uses single animation key with values from UnkStageRanges
// GX: this data does not exist on Mute City or Sand Ocean for ? reasons.
// AX: this data does not exist on NULL, Phantom Road, Mute City, Sand Ocean, and Big Blue (Story)
// In both cases, Mute City (COM) _does_ have this data, including COM story.
//
// Animation data
// Indexes 1-6 are:
// 0: UnkStageRanges.min
// 1: UnkStageRanges.max
// 2: float4.x
// 3: float4.y
// 4: float4.z
// 5: float4.w (always 0)
// NOTE: there is only 1 key per animation curve. Also, there is always exactly 1, except for
//       CH8 4/6 GX (of course...) where there is 0, and CPDB AX, where ther are 2 (see note below).
// Also, the time for each key is almost always 0. There is a case of 0.0333 in CH4 GX and CH6 AX
// and a case of 60f in CPDB AX (where there are 2 keys per anim curve).

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class UnkStageAnimationCurves :
        IBinaryAddressable,
        IBinarySerializable,
        ISerializedBinaryAddressableReferer
    {
        // CONSTANTS
        public const int kCurveCount = 6;

        // FIELDS
        [UnityEngine.SerializeField] private AddressRange addressRange;

        // FIELDS
        public ArrayPointer[] animationCurvePtrs;
        // REFERENCE FIELDS
        public AnimationCurve[] animationCurves = new AnimationCurve[kCurveCount];

        // CONSTRUCTORS
        public UnkStageAnimationCurves()
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

        // TODO: accessors which name the animation curves?



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

        public void ValidateReferences()
        {
            throw new NotImplementedException();
        }
    }
}
