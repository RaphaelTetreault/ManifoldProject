namespace Manifold
{
    public static class AnimationCurveExtensions
    {
        public static float EvaluateDefault(this UnityEngine.AnimationCurve curve, float time, float @default)
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
        public static float EvaluateNormalized(this UnityEngine.AnimationCurve curve, float normalizedTime)
        {
            var isNormalized = normalizedTime >= 0f && normalizedTime <= 1f;
            if (!isNormalized)
            {
                var msg = $"Argument {nameof(normalizedTime)} is not within 0 and 1 (both inclusive)! {nameof(normalizedTime)} = {normalizedTime}";
                throw new System.ArgumentException(msg);
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


        public static float GetMaxTime(this UnityEngine.AnimationCurve curve)
        {
            var hasKeys = curve.keys.Length > 0;
            if (!hasKeys)
            {
                var msg = $"Curve is empty! Cannot evaluate.";
                throw new System.ArgumentException(msg);
            }

            // Get max time value, normalize input time
            var lastIndex = curve.keys.Length - 1;
            var maxTime = curve.keys[lastIndex].time;
            return maxTime;
        }

        public static float GetMinTime(this UnityEngine.AnimationCurve curve)
        {
            var hasKeys = curve.keys.Length > 0;
            if (!hasKeys)
            {
                var msg = $"Curve is empty! Cannot evaluate.";
                throw new System.ArgumentException(msg);
            }

            var minTime = curve.keys[0].time;
            return minTime;
        }

    }
}
