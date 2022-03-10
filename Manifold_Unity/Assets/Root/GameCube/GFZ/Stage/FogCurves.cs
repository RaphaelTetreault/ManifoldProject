using Manifold;
using Manifold.IO;
using System;
using System.IO;

// NOTES
// Uses single animation key with values from UnkStageRanges
// GX: this data does not exist on Mute City or Sand Ocean
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

namespace GameCube.GFZ.Stage
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public sealed class FogCurves :
        IBinaryAddressable,
        IBinarySerializable,
        IHasReference,
        ITextPrintable
    {
        // CONSTANTS
        public const int kCurveCount = 6;

        // FIELDS
        public ArrayPointer[] animationCurvesPtrs;
        // REFERENCE FIELDS
        public AnimationCurve[] animationCurves = new AnimationCurve[kCurveCount];


        // CONSTRUCTORS
        public FogCurves()
        {
            // Initialize values so no null errors
            for (int i = 0; i < animationCurves.Length; i++)
                animationCurves[i] = new AnimationCurve(0);
        }


        // INDEXERS
        public AnimationCurve this[int index] { get => animationCurves[index]; set => animationCurves[index] = value; }

        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public AnimationCurve FogCurveNear { get => animationCurves[0]; set => animationCurves[0] = value; }
        public AnimationCurve FogCurveFar { get => animationCurves[1]; set => animationCurves[1] = value; }
        public AnimationCurve FogCurveR { get => animationCurves[2]; set => animationCurves[2] = value; }
        public AnimationCurve FogCurveG { get => animationCurves[3]; set => animationCurves[3] = value; }
        public AnimationCurve FogCurveB { get => animationCurves[4]; set => animationCurves[4] = value; }
        public AnimationCurve FogCurveUnk { get => animationCurves[5]; set => animationCurves[5] = value; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            AddressRange.RecordStartAddress(reader);
            {
                // read 6 array pointers
                reader.ReadX(ref animationCurvesPtrs, kCurveCount);
            }
            AddressRange.RecordEndAddress(reader);
            {
                // Go through each array pointer
                for (int i = 0; i < animationCurvesPtrs.Length; i++)
                {
                    var arrayPointer = animationCurvesPtrs[i];
                    var animationCurve = new AnimationCurve(arrayPointer.length);

                    reader.JumpToAddress(arrayPointer);
                    animationCurve.Deserialize(reader);

                    animationCurves[i] = animationCurve;
                }
            }
            reader.JumpToAddress(AddressRange.endAddress);
        }

        public void Serialize(BinaryWriter writer)
        {
            {
                // Ensure we have the correct amount of animation curves before indexing
                Assert.IsTrue(animationCurves.Length == kCurveCount);
                // Construct ArrayPointer2D for animation curves
                animationCurvesPtrs = new ArrayPointer[kCurveCount];
                for (int i = 0; i < animationCurvesPtrs.Length; i++)
                    animationCurvesPtrs[i] = animationCurves[i].GetArrayPointer();
            }
            AddressRange.RecordStartAddress(writer);
            {
                writer.WriteX(animationCurvesPtrs);
            }
            AddressRange.RecordEndAddress(writer);
        }

        public void ValidateReferences()
        {
            // Ensure we have the correct amount of animation curves before indexing
            Assert.IsTrue(animationCurves.Length == kCurveCount);
            for (int i = 0; i < kCurveCount; i++)
            {
                //Assert.ReferencePointer(animationCurves[i], animationCurvesPtrs[i]);
            }

            // Each curve should have 1 or more keys. In reality, this is not true.
            // All the used data in the final game is like this (except ST44 where
            // [5/6] is missing]). Suffice to say, OUR data should conform to this.

            // TO TEST: try an anim curve with multiple keys and interpolate between
            // 2 or more colours.
            //foreach (var animationCurve in animationCurves)
            //{
            //    // TODO: warn, don't assert
            //    //Assert.IsTrue(animationCurve.Length > 0);
            //}
        }

        public void PrintMultiLine(System.Text.StringBuilder builder, int indentLevel = 0, string indent = "\t")
        {
            builder.AppendLineIndented(indent, indentLevel, nameof(FogCurves));
            indentLevel++;
            // NEAR / FAR
            builder.AppendLineIndented(indent, indentLevel, nameof(FogCurveNear));
            builder.AppendLineIndented(indent, indentLevel + 1, FogCurveNear);
            builder.AppendLineIndented(indent, indentLevel, nameof(FogCurveFar));
            builder.AppendLineIndented(indent, indentLevel + 1, FogCurveFar);
            // RGB
            builder.AppendLineIndented(indent, indentLevel, nameof(FogCurveR));
            builder.AppendLineIndented(indent, indentLevel + 1, FogCurveR);
            builder.AppendLineIndented(indent, indentLevel, nameof(FogCurveG));
            builder.AppendLineIndented(indent, indentLevel + 1, FogCurveG);
            builder.AppendLineIndented(indent, indentLevel, nameof(FogCurveB));
            builder.AppendLineIndented(indent, indentLevel + 1, FogCurveB);
            // UNUSED / UNKNOWN
            builder.AppendLineIndented(indent, indentLevel, nameof(FogCurveUnk));
            builder.AppendLineIndented(indent, indentLevel + 1, FogCurveUnk);
        }

        public string PrintSingleLine()
        {
            return $"{nameof(FogCurves)}";
        }

        public override string ToString() => PrintSingleLine();

    }
}
