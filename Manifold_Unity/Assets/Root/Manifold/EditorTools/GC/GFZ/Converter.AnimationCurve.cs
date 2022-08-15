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

                if (currKeyframe.Time == nextKeyframe.Time)
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
                var keyframe = new Keyframe(keyable.Time, keyable.Value, keyable.TangentIn, keyable.TangentOut);
                keyframes[i] = keyframe;
            }

            return keyframes;
        }

        public static KeyableAttribute[] UnityAnimationCurveToKeyables(UnityEngine.AnimationCurve animationCurve)
        {
            var keyframes = animationCurve.keys;
            var keyables = new KeyableAttribute[keyframes.Length];
            var modes = TangentModesToInterpolationModes(animationCurve);
            for (int i = 0; i < keyables.Length; i++)
            {
                var keyframe = keyframes[i];
                var keyable = new KeyableAttribute()
                {
                    Time = keyframe.time,
                    Value = keyframe.value,
                    EaseMode = modes[i],
                    TangentIn = keyframe.inTangent,
                    TangentOut = keyframe.outTangent,
                };
                keyables[i] = keyable;
            }

            return keyables;
        }

        public static InterpolationMode[] TangentModesToInterpolationModes(UnityEngine.AnimationCurve animationCurve)
        {
            var modes = new InterpolationMode[animationCurve.keys.Length];
            for (int i = 0; i < modes.Length - 1; i++)
            {
                // GFZ: key modes -between- keys must be the same.
                var modeR = AnimationUtility.GetKeyRightTangentMode(animationCurve, i + 0);
                var modeL = AnimationUtility.GetKeyLeftTangentMode(animationCurve, i + 1);
                if (modeL != modeR)
                    Debug.LogWarning("GFZ keyables must have same tangent mode between keyframes! There' may be errors in exported keys.");

                // We only need to keep the key's right tangent mode - it'll apply to the current and next keys
                // ie: key[n].right == key[n+1].left
                modes[i] = UnityToGfzTangentMode(modeR);
            }

            return modes;
        }

        public static void SetGfzTangentsToUnityTangents(KeyableAttribute[] keyables, UnityEngine.AnimationCurve curve)
        {
            for (int i = 0; i < keyables.Length - 1; i++)
            {
                int keyCurr = i;
                int keyNext = i + 1;
                AnimationUtility.TangentMode mode = GfzToUnityTangentMode(keyables[keyCurr].EaseMode);
                // In GFZ, animation key[n]'s mode applies to key[n].rightTangent and key[n+1].leftTanget.
                // Think of it like the mode is inbetween the keys.
                AnimationUtility.SetKeyRightTangentMode(curve, keyCurr, mode);
                AnimationUtility.SetKeyLeftTangentMode(curve, keyNext, mode);
            }
        }

        public static AnimationUtility.TangentMode GfzToUnityTangentMode(InterpolationMode mode)
        {
            switch (mode)
            {
                case InterpolationMode.Constant:
                    return AnimationUtility.TangentMode.Constant;

                case InterpolationMode.Linear:
                    return AnimationUtility.TangentMode.Linear;

                case InterpolationMode.unknown1:
                case InterpolationMode.unknown2:
                    // don't do anything... seems to be a better option than not.
                    // The tangents seem to work fine in default (until I find something off)
                    return AnimationUtility.TangentMode.Free; // Value: 0

                default:
                    throw new Exception($"Unhandled conversiont to {nameof(InteractionMode)}.{mode}");
            }
        }

        public static InterpolationMode UnityToGfzTangentMode(AnimationUtility.TangentMode mode)
        {
            switch (mode)
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
                    throw new Exception($"Unhandled conversion to {nameof(AnimationUtility.TangentMode)}.{mode}");
            }
        }

        public static float EvaluateDefault(UnityEngine.AnimationCurve curve, float time, float @default)
        {
            if (curve.length == 0)
                return @default;

            return curve.Evaluate(time);
        }

        public static void SetGfzTangentMode(this UnityEngine.AnimationCurve curve, AnimationUtility.TangentMode mode)
        {
            for (int i = 0; i < curve.length - 1; i++)
            {
                AnimationUtility.SetKeyRightTangentMode(curve, i + 0, mode);
                AnimationUtility.SetKeyLeftTangentMode(curve, i + 1, mode);
            }
        }
        public static void SetGfzTangentModes(this UnityEngine.AnimationCurve curve, AnimationUtility.TangentMode[] modes)
        {
            for (int i = 0; i < curve.length - 1; i++)
            {
                var mode = modes[i];
                AnimationUtility.SetKeyRightTangentMode(curve, i + 0, mode);
                AnimationUtility.SetKeyLeftTangentMode(curve, i + 1, mode);
            }
        }
        public static void EnforceGfzTangentModes(this UnityEngine.AnimationCurve curve)
        {
            for (int i = 0; i < curve.length - 1; i++)
            {
                var mode = AnimationUtility.GetKeyRightTangentMode(curve, i);
                AnimationUtility.SetKeyRightTangentMode(curve, i + 0, mode);
                AnimationUtility.SetKeyLeftTangentMode(curve, i + 1, mode);
            }
        }

    }
}
