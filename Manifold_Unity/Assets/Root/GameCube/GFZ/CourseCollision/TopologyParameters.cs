using Manifold.IO;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// TODO: replace counts/absptr with ArrayPointers? Make new [][]Pointer struct?

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class TopologyParameters : IBinarySerializable, IBinaryAddressableRange
    {

        public const int kCurveCount = 9;

        [SerializeField]
        private AddressRange addressRange;

        public ArrayPointer2D curvePtrs2D = new ArrayPointer2D(kCurveCount);

        // uhg, Unity can't serealize 2d arrays, so had to implement the following:
        public Array2D<KeyableAttribute> keyablesArray2D = new Array2D<KeyableAttribute>();
        //public KeyableAttribute[][] keyablesArray2D = new KeyableAttribute[kCurveCount][];

        // At some point, maybe move out of class? Keep it vanilla for portability.
        public UnityEngine.AnimationCurve[] curves = new UnityEngine.AnimationCurve[kCurveCount];


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
                foreach (var arrayPointer in curvePtrs2D.ArrayPointers)
                {
                    var array = new KeyableAttribute[0];
                    if (arrayPointer.IsNotNullPointer)
                    {
                        reader.JumpToAddress(arrayPointer);
                        reader.ReadX(ref array, arrayPointer.Length, true);
                        //reader.ReadX(ref keyablesArray2D[index], arrayPointer.length, true);
                    }
                    keyablesArray2D.AppendArray(array);
                }
            }
            // Convert to Unity
            {
                // Convert from animation curves from Gfz to Unity formats
                for (int i = 0; i < keyablesArray2D.Length; i++)
                {
                    //var keyables = EnforceNoDuplicateTimes(keyablesArray2D[i]);
                    var keyables = EnforceNoDuplicateTimes(keyablesArray2D.GetArray(i));
                    var keyframes = KeyablesToKeyframes(keyables);
                    curves[i] = new UnityEngine.AnimationCurve(keyframes);
                    SetGfzTangentsToUnityTangets(keyables, curves[i]);

                    // TEST - re-apply key values.
                    // Not being respected by Unity?
                    for (int j = 0; j < curves[i].length; j++)
                    {
                        curves[i].keys[j].inTangent = keyframes[j].inTangent;
                        curves[i].keys[j].outTangent = keyframes[j].outTangent;
                    }
                }
            }
            this.SetReaderToEndAddress(reader);
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
                Debug.Log($"Removed duplicate keyframe times! Total: {removeIndexes.Count}");
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

        public void SetGfzTangentsToUnityTangets(KeyableAttribute[] keyables, UnityEngine.AnimationCurve curve)
        {
            for (int i = 0; i < keyables.Length; i++)
            {
                UnityEditor.AnimationUtility.TangentMode mode;

                switch (keyables[i].easeMode)
                {
                    case InterpolationMode.Constant:
                        mode = UnityEditor.AnimationUtility.TangentMode.Constant;
                        break;

                    case InterpolationMode.Linear:
                        mode = UnityEditor.AnimationUtility.TangentMode.Linear;
                        break;

                    case InterpolationMode.unknown1:
                        mode = UnityEditor.AnimationUtility.TangentMode.ClampedAuto;
                        break;

                    case InterpolationMode.unknown2:
                        mode = UnityEditor.AnimationUtility.TangentMode.ClampedAuto;
                        break;

                    default:
                        throw new NotImplementedException($"New value {(int)keyables[i].easeMode}");
                }

                // Set tangent type in Unity's format
                UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, i, mode);
                UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, i, mode);
            }
        }


    }
}
