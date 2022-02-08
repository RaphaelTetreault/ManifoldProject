using UnityEngine;

namespace Manifold
{
    public static class AnimationCurveExtensions
    {
        public static float EvaluateDefault(this AnimationCurve curve, float time, float @default)
        {
            if (time == 0f)
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
            var isNormalized = normalizedTime >= 0f && normalizedTime <= 1f;
            if (!isNormalized)
            {
                var msg = $"Argument {nameof(normalizedTime)} is not within 0 and 1 (both inclusive)! {nameof(normalizedTime)} = {normalizedTime}";
                //throw new System.ArgumentException(msg);
                DebugConsole.Log(msg);
            }

            var hasKeys = curve.keys.Length > 0;
            if (!hasKeys)
            {
                var msg = $"Curve is empty! Cannot evaluate.";
                throw new System.ArgumentException(msg);
            }

            var minTime = GetMinTime(curve);
            var minTimeIsZero = minTime == 0f;
            if (!minTimeIsZero)
            {
                var msg = $"Min time value of curve is not 0f! AnimationCurve.keys[0].time = {minTime}";
                throw new System.ArgumentException(msg);
            }

            var maxTime = GetMaxTime(curve);
            var time = maxTime * normalizedTime;
            var value = curve.Evaluate(time);
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



        public static float GetMinTime(this AnimationCurve curve)
        {
            // Can error if keys.length is [0]

            var minTime = curve.keys[0].time;
            return minTime;
        }

        public static float GetMaxTime(this AnimationCurve curve)
        {
            // Can error if keys.length is [0]

            // Get max time value, normalize input time
            var lastIndex = curve.keys.Length - 1;
            var maxTime = curve.keys[lastIndex].time;
            return maxTime;
        }




        public static AnimationCurve GetCopy(this AnimationCurve curve)
        {
            return new AnimationCurve(curve.keys);
        }


        public static AnimationCurve GetInverted(this AnimationCurve curve)
        {
            var keys = curve.keys;
            var invertedKeys = new Keyframe[keys.Length];

            for (int i = 0; i < keys.Length; i++)
            {
                // copy all misc data
                invertedKeys[i] = keys[i];
                // Invert values
                invertedKeys[i].value = -keys[i].value;
                invertedKeys[i].inTangent = -keys[i].inTangent;
                invertedKeys[i].outTangent = -keys[i].outTangent;
            }

            return new AnimationCurve(invertedKeys);
        }

        public static AnimationCurve GetOffset(this AnimationCurve curve, float valueOffset)
        {
            var keys = curve.keys;
            var invertedKeys = new Keyframe[keys.Length];

            for (int i = 0; i < keys.Length; i++)
            {
                // copy all misc data
                invertedKeys[i] = keys[i];
                // Invert values
                invertedKeys[i].value += valueOffset;
            }

            return new AnimationCurve(invertedKeys);
        }

    }
}
