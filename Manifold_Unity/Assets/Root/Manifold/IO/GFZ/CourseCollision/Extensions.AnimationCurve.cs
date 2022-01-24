namespace Manifold.IO.GFZ.CourseCollision
{
    public static class AnimationCurveExtensions
    {
        public static float EvaluateDefault(this UnityEngine.AnimationCurve curve, float time, float @default)
        {
            if (time == 0f)
                return @default;

            return curve.Evaluate(time);
        }
    }
}
