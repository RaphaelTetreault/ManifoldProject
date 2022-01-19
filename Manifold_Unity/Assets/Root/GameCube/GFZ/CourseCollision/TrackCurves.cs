using Manifold.IO;
using Manifold.IO.GFZ;
using System;
using System.Collections.Generic;
using System.IO;

// TODO: move Unity-specific functions to Manifold script!
using UnityEngine;


namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// Defines the tracks position, rotation, and scale using 9 animation curves,
    /// each defining the X, Y, and Z properties. Their values are (asummed to be)
    /// multiplied with an associated Transform. Hierarchies of these exists, each
    /// being multiplied with it's parent up a tree structure.
    /// </summary>
    [Serializable]
    public class TrackCurves :
        IBinaryAddressable,
        IBinarySerializable,
        ISerializedBinaryAddressableReferer
    {
        // CONSTANTS
        public const int kCurveCount = 9;

        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;

        // FIELDS
        public ArrayPointer2D curvesPtr2D = new ArrayPointer2D(kCurveCount);
        // REFERENCE FIELDS
        public AnimationCurve[] animationCurves = new AnimationCurve[0];

        // TODO: remove when solved.
        // At some point, maybe move out of class? Keep it vanilla for portability.
        public UnityEngine.AnimationCurve[] unityCurves = new UnityEngine.AnimationCurve[kCurveCount];


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
                reader.ReadX(ref curvesPtr2D);
            }
            this.RecordEndAddress(reader);
            {
                // Init array
                animationCurves = new AnimationCurve[kCurveCount];

                for (int i = 0; i < animationCurves.Length; i++)
                {
                    var arrayPointer = curvesPtr2D.ArrayPointers[i];
                    if (arrayPointer.IsNotNullPointer)
                    {
                        // Deserialization is a bit different. Init array length here.
                        var animationCurve = new AnimationCurve(arrayPointer.Length);

                        // Read values
                        reader.JumpToAddress(arrayPointer);
                        reader.ReadX(ref animationCurve, false); // do not create new instance

                        // Assign curve to array
                        animationCurves[i] = animationCurve;
                    }
                    else
                    {
                        animationCurves[i] = new AnimationCurve(0);
                    }
                }
            }
            this.SetReaderToEndAddress(reader);

            // TEMP
            // Convert to Unity
            {
                // Convert from animation curves from Gfz to Unity formats
                for (int i = 0; i < animationCurves.Length; i++)
                {
                    var animationCurve = animationCurves[i];
                    var keyables = AnimationCurveConverter.EnforceNoDuplicateTimes(animationCurve.keyableAttributes);
                    var keyframes = AnimationCurveConverter.KeyablesToKeyframes(keyables);
                    unityCurves[i] = new UnityEngine.AnimationCurve(keyframes);

                    // 
                    AnimationCurveConverter.SetGfzTangentsToUnityTangents(keyables, unityCurves[i]);

                    // TEST - re-apply key values.
                    // Not being respected by Unity?
                    for (int j = 0; j < unityCurves[i].length; j++)
                    {
                        unityCurves[i].keys[j].inTangent = keyframes[j].inTangent;
                        unityCurves[i].keys[j].outTangent = keyframes[j].outTangent;
                    }
                }
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            {
                // Ensure we have the correct amount of animation curves before indexing
                Assert.IsTrue(animationCurves.Length == kCurveCount);

                // Construct ArrayPointer2D for animation curves
                var pointers = new ArrayPointer[kCurveCount];
                for (int i = 0; i < pointers.Length; i++)
                    pointers[i] = animationCurves[i].GetArrayPointer();

                curvesPtr2D = new ArrayPointer2D(pointers);
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(curvesPtr2D);
            }
            this.RecordEndAddress(writer);
        }

        public void ValidateReferences()
        {
            // Assert array information
            Assert.IsTrue(animationCurves != null);
            Assert.IsTrue(animationCurves.Length == kCurveCount);
            Assert.IsTrue(curvesPtr2D.Length == kCurveCount);

            // Assert each animation curve
            for (int i = 0; i < kCurveCount; i++)
            {
                var animCurve = animationCurves[i];
                var pointer = curvesPtr2D.arrayPointers[i];
                Assert.ReferencePointer(animCurve, pointer);
            }
        }

    }
}
