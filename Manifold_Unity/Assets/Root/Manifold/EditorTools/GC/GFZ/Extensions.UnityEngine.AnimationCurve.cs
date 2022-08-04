using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Manifold.EditorTools
{
    public static class AnimationCurveExtensions
    {
        /// <summary>
        /// Evaluates <paramref name="curve"/> and returns <paramref name="default"/> if the
        /// curve is null or empty.
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="time"></param>
        /// <param name="default"></param>
        /// <returns></returns>
        public static float EvaluateDefault(this AnimationCurve curve, float time, float @default)
        {
            if (curve is null || curve.length == 0)
                return @default;

            return curve.Evaluate(time);
        }

        /// <summary>
        /// Evaluates animation curve as if the max time of the curve was 1f.
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="normalizedTime"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"></exception>
        public static float EvaluateNormalized(this AnimationCurve curve, float normalizedTime)
        {
            //var isNormalized = normalizedTime >= 0f && normalizedTime <= 1f;
            //if (!isNormalized)
            //{
            //    var msg = $"Argument {nameof(normalizedTime)} is not within 0 and 1 (both inclusive)! {nameof(normalizedTime)} = {normalizedTime}";
            //    //throw new System.ArgumentException(msg);
            //    DebugConsole.Log(msg);
            //}

            //var hasKeys = curve.keys.Length > 0;
            //if (!hasKeys)
            //{
            //    var msg = $"Curve is empty! Cannot evaluate.";
            //    throw new System.ArgumentException(msg);
            //}

            //var minTime = GetMinTime(curve);
            //var minTimeIsZero = minTime == 0f;
            //if (!minTimeIsZero)
            //{
            //    var msg = $"Min time value of curve is not 0f! AnimationCurve.keys[0].time = {minTime}";
            //    throw new System.ArgumentException(msg);
            //}

            float minTime = GetMinTime(curve);
            float maxTime = GetMaxTime(curve);
            float range = maxTime - minTime;
            float time = normalizedTime * range + minTime;
            float value = curve.Evaluate(time);
            return value;
        }

        public static float EvaluateMin(this AnimationCurve curve)
        {
            float minTime = curve.GetMinTime();
            float value = curve.Evaluate(minTime);
            return value;
        }
        public static float EvaluateMax(this AnimationCurve curve)
        {
            float maxTime = curve.GetMaxTime();
            float value = curve.Evaluate(maxTime);
            return value;
        }

        public static float FastEvaluateNormalized(this AnimationCurve curve, float normalizedTime)
        {
            // Assumes a min time of 0f
            var maxTime = GetMaxTime(curve);
            var time = maxTime * normalizedTime;
            var value = curve.Evaluate(time);
            return value;
        }


        public static float SafeGetMaxTime(this AnimationCurve curve)
        {
            var hasKeys = curve.keys.Length > 0;
            if (!hasKeys)
            {
                var msg = $"Curve is empty! Cannot evaluate.";
                throw new System.ArgumentException(msg);
            }

            // Get max time value, normalize input time
            return GetMaxTime(curve);
        }

        public static float SafeGetMinTime(this AnimationCurve curve)
        {
            var hasKeys = curve.keys.Length > 0;
            if (!hasKeys)
            {
                var msg = $"Curve is empty! Cannot evaluate.";
                throw new System.ArgumentException(msg);
            }

            return GetMinTime(curve);
        }

        public static AnimationCurve GetCopy(this AnimationCurve curve)
        {
            return new AnimationCurve(curve.keys);
        }


        public static float GetMinTime(this AnimationCurve curve)
            => KeyframeUtility.GetMinTime(curve.keys);

        public static float GetMaxTime(this AnimationCurve curve)
            => KeyframeUtility.GetMaxTime(curve.keys);

        public static AnimationCurve CreateInverted(this AnimationCurve curve)
            => new AnimationCurve(KeyframeUtility.InvertedKeys(curve.keys));

        public static AnimationCurve CreateValueOffset(this AnimationCurve curve, float valueOffset)
            => new AnimationCurve(KeyframeUtility.OffsetKeyValues(curve.keys, valueOffset));

        public static Keyframe[] GetRenormalizedKeyRangeAndTangents(this AnimationCurve curve, float newMinTime, float newMaxTime)
            => KeyframeUtility.GetRenormalizedKeyRangeAndTangents(curve.keys, newMinTime, newMaxTime);

        public static AnimationCurve SetKeyTangents(this AnimationCurve curve, float inOutTangents)
            => new AnimationCurve(KeyframeUtility.SetKeyTangents(curve.keys, inOutTangents));

        public static void SetKeyTangents(this AnimationCurve curve, int index, float inOutTangent)
        {
            var key = curve.keys[index];
            key.inTangent = inOutTangent;
            key.outTangent = inOutTangent;
            curve.MoveKey(index, key);
        }

        public static void SmoothTangents(this AnimationCurve curve, float weight = 1 / 3f)
        {
            for (int i = 0; i < curve.length; i++)
                curve.SmoothTangents(i, weight);
        }
    }
}
