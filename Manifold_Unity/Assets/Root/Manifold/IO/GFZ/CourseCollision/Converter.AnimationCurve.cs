using GameCube.GFZ.CourseCollision;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Manifold.IO.GFZ
{
    public static class AnimationCurveConverter
    {
        public static UnityEngine.AnimationCurve ToUnity(this GameCube.GFZ.CourseCollision.AnimationCurve curve)
        {
            var keyframes = KeyablesToKeyframes(curve.keyableAttributes);
            return new UnityEngine.AnimationCurve(keyframes);
        }

        public static GameCube.GFZ.CourseCollision.AnimationCurve ToGfz(this UnityEngine.AnimationCurve curve)
        {
            var keyables = UnityAnimationCurveToKeyables(curve);
            var gfzCurve = new GameCube.GFZ.CourseCollision.AnimationCurve(keyables);
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
                var keyframe = new Keyframe(keyable.time, keyable.value, keyable.zTangentIn, keyable.zTangentOut);
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
                var modeL = AnimationUtility.GetKeyLeftTangentMode(animationCurve, i);
                var modeR = AnimationUtility.GetKeyRightTangentMode(animationCurve, i);
                if (modeL != modeR)
                    throw new ArgumentException("GFZ keyables must have same tangent mode for L/R tangents!");

                var keyable = new KeyableAttribute()
                {
                    time = keyframe.time,
                    value = keyframe.value,
                    easeMode = UnityToGfzTangentMode(modeL),
                    zTangentIn = keyframe.inTangent,
                    zTangentOut = keyframe.outTangent,
                };
                keyables[i] = keyable;
            }

            return keyables;
        }

        public static void SetGfzTangentsToUnityTangents(KeyableAttribute[] keyables, UnityEngine.AnimationCurve curve)
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
                        throw new System.NotImplementedException($"New value {(int)keyables[i].easeMode}");
                }

                //// Set tangent type in Unity's format
                //UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, i, mode);
                //UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, i, mode);
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

        //public static Vector3 EvaluateScale( float t)
        //{
        //    const float defaultValue = 1f;

        //    var curveX = unityCurves[0];
        //    var curveY = unityCurves[1];
        //    var curveZ = unityCurves[2];

        //    var scale = new Vector3(
        //        EvaluateDefault(curveX, t, defaultValue),
        //        EvaluateDefault(curveY, t, defaultValue),
        //        EvaluateDefault(curveZ, t, defaultValue)
        //        );

        //    return scale;
        //}

        //public static Vector3 EvaluateRotationEuler(float t)
        //{
        //    const float defaultValue = 0f;

        //    var curveX = unityCurves[3];
        //    var curveY = unityCurves[4];
        //    var curveZ = unityCurves[5];

        //    var rotationEuler = new Vector3(
        //        EvaluateDefault(curveX, t, defaultValue),
        //        EvaluateDefault(curveY, t, defaultValue),
        //        EvaluateDefault(curveZ, t, defaultValue)
        //        );

        //    return rotationEuler;
        //}

        //public static Quaternion EvaluateRotation(float t)
        //{
        //    var quaternion = Quaternion.Euler(EvaluateRotationEuler(t));
        //    return quaternion;
        //}

        //public static Vector3 EvaluatePosition(float t)
        //{
        //    const float defaultValue = 0f;

        //    var curveX = unityCurves[6];
        //    var curveY = unityCurves[7];
        //    var curveZ = unityCurves[8];

        //    var scale = new Vector3(
        //        EvaluateDefault(curveX, t, defaultValue),
        //        EvaluateDefault(curveY, t, defaultValue),
        //        EvaluateDefault(curveZ, t, defaultValue)
        //        );

        //    return scale;
        //}

        public static float EvaluateDefault(UnityEngine.AnimationCurve curve, float time, float @default)
        {
            if (curve.length == 0)
                return @default;

            return curve.Evaluate(time);
        }


    }
}
