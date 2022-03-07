using GameCube.GFZ.Stage;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Manifold.EditorTools.GC.GFZ
{
    public static class AnimationCurveConverter
    {
        public static UnityEngine.AnimationCurve ToUnity(this GameCube.GFZ.Stage.AnimationCurve curve)
        {
            if (curve == null)
                return null;

            var keyframes = KeyablesToKeyframes(curve.KeyableAttributes);
            return new UnityEngine.AnimationCurve(keyframes);
        }

        public static GameCube.GFZ.Stage.AnimationCurve ToGfz(this UnityEngine.AnimationCurve curve)
        {
            if (curve == null)
                return null;

            var keyables = UnityAnimationCurveToKeyables(curve);
            var gfzCurve = new GameCube.GFZ.Stage.AnimationCurve(keyables);
            return gfzCurve;
        }

        public static KeyableAttribute[] EnforceNoDuplicateTimes(KeyableAttribute[] keyables)
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

                // TFollowing line triggers on stock stage 43
                //DebugConsole.Log($"Removed duplicate keyframe times! Total: {removeIndexes.Count}");
                
                // Re-assign new list to used parameters
                keyables = uniqueKeyframeTimes.ToArray();
            }

            return keyables;
        }

        public static Keyframe[] KeyablesToKeyframes(KeyableAttribute[] keyables)
        {
            var keyframes = new Keyframe[keyables.Length];
            for (int i = 0; i < keyframes.Length; i++)
            {
                var keyable = keyables[i];
                var keyframe = new Keyframe(keyable.time, keyable.value, keyable.tangentIn, keyable.tangentOut);
                keyframes[i] = keyframe;
            }

            return keyframes;
        }

        public static KeyableAttribute[] UnityAnimationCurveToKeyables(UnityEngine.AnimationCurve animationCurve)
        {
            var keyframes = animationCurve.keys;
            var keyables = new KeyableAttribute[keyframes.Length];
            for (int i = 0; i < keyables.Length; i++)
            {
                var keyframe = keyframes[i];

                // Must be the same for GFZ!
                // 2022/01/31: might just be that prev.out and curr.in must be same.
                var modeL = AnimationUtility.GetKeyLeftTangentMode(animationCurve, i);
                var modeR = AnimationUtility.GetKeyRightTangentMode(animationCurve, i);
                if (modeL != modeR)
                    throw new ArgumentException("GFZ keyables must have same tangent mode for L/R tangents!");

                var keyable = new KeyableAttribute()
                {
                    time = keyframe.time,
                    value = keyframe.value,
                    easeMode = UnityToGfzTangentMode(modeL),
                    tangentIn = keyframe.inTangent,
                    tangentOut = keyframe.outTangent,
                };
                keyables[i] = keyable;
            }

            return keyables;
        }

        public static void SetGfzTangentsToUnityTangents(KeyableAttribute[] keyables, UnityEngine.AnimationCurve curve)
        {
            for (int i = 0; i < keyables.Length; i++)
            {
                AnimationUtility.TangentMode mode;

                switch (keyables[i].easeMode)
                {
                    case InterpolationMode.Constant:
                        mode = AnimationUtility.TangentMode.Constant;
                        AnimationUtility.SetKeyLeftTangentMode(curve, i, mode);
                        AnimationUtility.SetKeyRightTangentMode(curve, i, mode);
                        break;

                    case InterpolationMode.Linear:
                        mode = AnimationUtility.TangentMode.Linear;
                        AnimationUtility.SetKeyLeftTangentMode(curve, i, mode);
                        AnimationUtility.SetKeyRightTangentMode(curve, i, mode);
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
            }
        }

        public static InterpolationMode UnityToGfzTangentMode(AnimationUtility.TangentMode tangentMode)
        {
            switch (tangentMode)
            {
                case AnimationUtility.TangentMode.Constant:
                    return InterpolationMode.Constant;

                case AnimationUtility.TangentMode.Linear:
                    return InterpolationMode.Linear;

                // Not 100% on this.
                case AnimationUtility.TangentMode.Free:
                case AnimationUtility.TangentMode.Auto:
                case AnimationUtility.TangentMode.ClampedAuto:
                    return InterpolationMode.unknown1;

                default:
                    throw new NotImplementedException();
            }
        }

        public static float EvaluateDefault(UnityEngine.AnimationCurve curve, float time, float @default)
        {
            if (curve.length == 0)
                return @default;

            return curve.Evaluate(time);
        }


    }
}
