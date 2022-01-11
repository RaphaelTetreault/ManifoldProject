using Manifold.IO;
using System;
using System.IO;

// NOTES
// Uses single animation key with values from UnkStageRanges
// GX: this data does not exist on Mute City or Sand Ocean for ? reasons.
// AX: this data does not exist on Mute City, Sand Ocean, NULL, Phantom Road, and Big Blue (Story)
// In both cases, Mute City (COM) _does_ have this data, including COM story.
//
// Animation data
// Indexes 0-5, length 6, values are:
// 0: Fog.fogRange.near
// 1: Fog.fogRange.far
// 2: Fog.colorRGB.x - color R
// 3: Fog.colorRGB.y - color G
// 4: Fog.colorRGB.z - color B
// 5: (always 0)
//
// Get animation curves [6] easily with helper funtion Fog.GetAnimationCurves().
//
// NOTE: there is always exactly 1 key per animation curve EXCEPT for CH8 4/6 GX (of course...) where
//       there are 0 keys, and CPDB AX, where ther are 2 keys (see note below). ALSO, the time for each
//       key is almost always 0. There is a case of 0.0333 in CH4 GX and CH6 AX and a case of 60f in
//       CPDB AX (where there are 2 keys per anim curve).

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class FogCurves :
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
        public FogCurves()
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

        public AnimationCurve FogCurveNear
        {
            get => animationCurves[0];
            set => animationCurves[0] = value;
        }
        public AnimationCurve FogCurveFar
        {
            get => animationCurves[1];
            set => animationCurves[1] = value;
        }
        public AnimationCurve FogCurveR
        {
            get => animationCurves[2];
            set => animationCurves[2] = value;
        }
        public AnimationCurve FogCurveG
        {
            get => animationCurves[3];
            set => animationCurves[3] = value;
        }
        public AnimationCurve FogCurveB
        {
            get => animationCurves[4];
            set => animationCurves[4] = value;
        }
        public AnimationCurve FogCurveUnk
        {
            get => animationCurves[5];
            set => animationCurves[5] = value;
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

        public void ValidateReferences()
        {
            // Ensure we have the correct amount of animation curves before indexing
            Assert.IsTrue(animationCurves.Length == kCurveCount);

            // Each curve should have 1 or more keys. In reality, this is not true.
            // All the used data in the final game is like this (except ST44 where
            // [5/6] is missing]). Suffice to say, OUR data should conform to this.
            
            // TO TEST: try an anim curve with multiple keys and interpolate between
            // 2 or more colours.
            foreach (var animationCurve in animationCurves)
            {
                // TODO: warn, don't assert
                //Assert.IsTrue(animationCurve.Length > 0);
            }
        }
    }
}
