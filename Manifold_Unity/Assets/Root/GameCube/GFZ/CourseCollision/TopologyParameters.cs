using Manifold.IO;
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
    public class TopologyParameters : IBinarySerializable, IBinaryAddressableRange
    {

        public const int kCurveCount = 9;

        [UnityEngine.SerializeField]
        private AddressRange addressRange;

        public ArrayPointer2D curvePtrs2D = new ArrayPointer2D(kCurveCount);

        // uhg, Unity can't serealize 2d arrays, so had to implement the following:
        //public Array2D<KeyableAttribute> keyablesArray2D = new Array2D<KeyableAttribute>();
        //public KeyableAttribute[][] keyablesArray2D = new KeyableAttribute[kCurveCount][];

        public AnimationCurve[] animationCurves = new AnimationCurve[0];

        // At some point, maybe move out of class? Keep it vanilla for portability.
        public UnityEngine.AnimationCurve[] unityCurves = new UnityEngine.AnimationCurve[kCurveCount];


        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }


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

                for (int i  = 0; i  < animationCurves.Length; i++)
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

            // Convert to Unity
            {
                // Convert from animation curves from Gfz to Unity formats
                for (int i = 0; i < animationCurves.Length; i++)
                {
                    var animationCurve = animationCurves[i];
                    var keyables = EnforceNoDuplicateTimes(animationCurve.keyableAttributes);
                    var keyframes = KeyablesToKeyframes(keyables);
                    unityCurves[i] = new UnityEngine.AnimationCurve(keyframes);

                    // Disabling this makes the tangents work better...?
                    SetGfzTangentsToUnityTangents(keyables, unityCurves[i]);

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
            writer.WriteX(curvePtrs2D);

            throw new NotImplementedException();
        }


        public KeyableAttribute[] EnforceNoDuplicateTimes(KeyableAttribute[] keyables)
        {
            var removeIndexes = new List<int>();
            for (int i = 0; i < keyables.Length - 1; i++)
            {
                int currIndex = i;
                int nextIndex = i + 1;
                var currKeyframe = keyables[currIndex];
                var nextKeyframe = keyables[nextIndex];

                if (currKeyframe.time == nextKeyframe.time)
                {
                    // Remove first of 2 indexes
                    removeIndexes.Add(currIndex);
                }
            }

            // Remove duplicates
            // TODO: make this a function?
            bool hasKeyframeTimeDuplicates = removeIndexes.Count > 0;
            if (hasKeyframeTimeDuplicates)
            {
                // Make a new list for unique keyframe times - no times are duplicates
                var uniqueKeyframeTimes = new List<KeyableAttribute>(keyables);

                // Invert order of list. As we remove items, the index/length changes unless
                // we iterate through the list backwards.
                removeIndexes.Reverse();

                // Remove each duplicate time via index
                for (int i = 0; i < removeIndexes.Count; i++)
                {
                    var index = removeIndexes[i];
                    uniqueKeyframeTimes.RemoveAt(index);
                }

                // Re-assign new list to used parameters
                DebugConsole.Log($"Removed duplicate keyframe times! Total: {removeIndexes.Count}");
                keyables = uniqueKeyframeTimes.ToArray();
            }

            return keyables;
        }

        public Keyframe[] KeyablesToKeyframes(KeyableAttribute[] keyables)
        {
            var keyframes = new Keyframe[keyables.Length];
            for (int i = 0; i < keyframes.Length; i++)
            {
                var key = keyables[i];
                var keyframe = new Keyframe(key.time, key.value, key.zTangentIn, key.zTangentOut);
                keyframes[i] = keyframe;
            }

            return keyframes;
        }

        public void SetGfzTangentsToUnityTangents(KeyableAttribute[] keyables, UnityEngine.AnimationCurve curve)
        {
            for (int i = 0; i < keyables.Length; i++)
            {
                UnityEditor.AnimationUtility.TangentMode mode;

                switch (keyables[i].easeMode)
                {
                    case InterpolationMode.Constant:
                        mode = UnityEditor.AnimationUtility.TangentMode.Constant;
                        UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, i, mode);
                        UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, i, mode);
                        break;

                    case InterpolationMode.Linear:
                        mode = UnityEditor.AnimationUtility.TangentMode.Linear;
                        UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, i, mode);
                        UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, i, mode);
                        break;

                    case InterpolationMode.unknown1:
                    case InterpolationMode.unknown2:
                        // don't do anything... seems to be a better option than not.
                        // The tangents seem to work fine in default (until I find something off)
                        break;

                    //case InterpolationMode.unknown1:
                    //    mode = UnityEditor.AnimationUtility.TangentMode.Auto;
                    //    break;

                    //case InterpolationMode.unknown2:
                    //    mode = UnityEditor.AnimationUtility.TangentMode.ClampedAuto;
                    //    break;

                    default:
                        throw new NotImplementedException($"New value {(int)keyables[i].easeMode}");
                }

                //// Set tangent type in Unity's format
                //UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, i, mode);
                //UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, i, mode);
            }
        }


    }
}
