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
    /// 
    /// </summary>
    [Serializable]
    public class TopologyParameters :
        IBinaryAddressable,
        IBinarySerializable,
        ISerializedBinaryAddressableReferer
    {
        // CONSTANTS
        public const int kCurveCount = 9;

        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;

        // FIELDS
        public ArrayPointer2D curvePtrs2D = new ArrayPointer2D(kCurveCount);
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
                reader.ReadX(ref curvePtrs2D);
            }
            this.RecordEndAddress(reader);
            {
                // Init array
                animationCurves = new AnimationCurve[kCurveCount];

                for (int i = 0; i < animationCurves.Length; i++)
                {
                    var arrayPointer = curvePtrs2D.ArrayPointers[i];
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
                    var keyables = AnimationCurveUtility.EnforceNoDuplicateTimes(animationCurve.keyableAttributes);
                    var keyframes = AnimationCurveUtility.KeyablesToKeyframes(keyables);
                    unityCurves[i] = new UnityEngine.AnimationCurve(keyframes);

                    // 
                    AnimationCurveUtility.SetGfzTangentsToUnityTangents(keyables, unityCurves[i]);

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
                curvePtrs2D = new ArrayPointer2D(pointers);
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(curvePtrs2D);
            }
            this.RecordEndAddress(writer);
        }

        public void ValidateReferences()
        {
            // Assert array information
            Assert.IsTrue(animationCurves != null);
            Assert.IsTrue(animationCurves.Length == kCurveCount);
            Assert.IsTrue(curvePtrs2D.Length == kCurveCount);

            // Assert array items
            for (int i = 0; i < kCurveCount; i++)
            {
                // Simplify access
                var animationCurve = animationCurves[i];
                var animationCurvePtrs = curvePtrs2D.ArrayPointers[i];

                // Ensure each item in 2D array is not true null
                Assert.IsTrue(animationCurve != null);
                // Ensure data matches up
                Assert.IsTrue(animationCurve.Length == animationCurvePtrs.Length);
                Assert.IsTrue(animationCurve.AddressRange.GetPointer() == animationCurvePtrs.Pointer);

                if (animationCurve.Length > 0)
                {
                    // Assert IS TRUE
                    Assert.IsTrue(animationCurvePtrs.IsNotNullPointer);
                    Assert.IsTrue(animationCurvePtrs.Length == 0);
                }
                else
                {
                    // Assert IS FALSE
                    Assert.IsFalse(animationCurvePtrs.IsNotNullPointer);
                    Assert.IsFalse(animationCurvePtrs.Length == 0);
                }
            }
        }


    }
}
