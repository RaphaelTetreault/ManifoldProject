using UnityEngine;
using Unity.Mathematics;

namespace Manifold
{
    public class CatmullRomSpline :
        ISpline
    {
        public const float UniformAlpha = 0f;
        public const float CentripetalAlpha = 0.5f;
        public const float ChordalAlpha = 1f;

        /// <summary>
        /// Get the time value appropriate for a Catmull-Rom spline
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        public static float GetTime(float3 from, float3 to, float alpha)
        {
            // Get the difference between the two points
            float3 delta = to - from;

            // delta to the power of two, each componenet summed
            float squaredSum =
                delta.x * delta.x +
                delta.y * delta.y +
                delta.z * delta.z;

            // 
            float power = alpha * 0.5f;
            float time = math.pow(squaredSum, power);

            return time;
        }

        /// <summary>
        /// Perform a single step of Barry and Goldman's pyramidal formulation
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float3 BarryGoldmanPyramidalFormulationStep(Vector3 p0, Vector3 p1, float t0, float t1, float t)
        {
            // Single iteration of Barry and Goldman's pyramidal formulation
            // https://en.wikipedia.org/wiki/File:Cubic_Catmull-Rom_formulation.png

            var value =
                (t1 - t) / (t1 - t0) * p0 +
                (t - t0) / (t1 - t0) * p1;
            
            return value;
        }

        /// <summary>
        /// Perform Barry and Goldman's pyramidal formulation for the provided points
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float3 BarryGoldmanPyramidalFormulation(float3 p0, float3 p1, float3 p2, float3 p3, float t0, float t1, float t2, float t3, float t)
        {
            // Barry and Goldman's pyramidal formulation
            // https://en.wikipedia.org/wiki/File:Cubic_Catmull-Rom_formulation.png

            // Formulation between 4 points provided
            float3 a1 = BarryGoldmanPyramidalFormulationStep(p0, p1, t0, t1, t);
            float3 a2 = BarryGoldmanPyramidalFormulationStep(p1, p2, t1, t2, t);
            float3 a3 = BarryGoldmanPyramidalFormulationStep(p2, p3, t2, t3, t);
            // Formulation between above 3 intervals
            float3 b1 = BarryGoldmanPyramidalFormulationStep(a1, a2, t0, t2, t);
            float3 b2 = BarryGoldmanPyramidalFormulationStep(a2, a3, t1, t3, t);
            // Formulation between above 2 intervals
            float3 c = BarryGoldmanPyramidalFormulationStep(b1, b2, t1, t2, t);

            return c;
        }

        /// <summary>
        /// Sample the Catmull-Rom spline at time <paramref name="t"/>
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="alpha"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float3 Evaluate(float3 p0, float3 p1, float3 p2, float3 p3, float alpha, float t)
        {
            // Get times for these points (unpacking tuple)
            (float t0, float t1, float t2, float t3) = GetPointTime(p0, p1, p2, p3, alpha);

            float catmullRomTime = math.lerp(t1, t2, t);

            float3 point = BarryGoldmanPyramidalFormulation(p0, p1, p2, p3, t0, t1, t2, t3, catmullRomTime);
            return point;
        }


        /// <summary>
        /// Sample the Catmull-Rom spline <paramref name="nIterations"/> times
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="alpha"></param>
        /// <param name="nIterations"></param>
        /// <returns></returns>
        public static float3[] EvaluateMany(float3 p0, float3 p1, float3 p2, float3 p3, float alpha, int nIterations)
        {
            float3[] points = new float3[nIterations];

            // Get times for these points (unpacking tuple)
            (float t0, float t1, float t2, float t3) = GetPointTime(p0, p1, p2, p3, alpha);

            // 
            var timeMax = nIterations - 1;
            for (int i = 0; i < nIterations; i++)
            {
                // Iteration Time
                float it = (float)i / timeMax;
                float catmullRomTime = math.lerp(t1, t2, it);
                points[i] = BarryGoldmanPyramidalFormulation(p0, p1, p2, p3, t0, t1, t2, t3, catmullRomTime);
            }

            return points;
        }


        public static (float t0, float t1, float t2, float t3) GetPointTime(float3 p0, float3 p1, float3 p2, float3 p3, float alpha)
        {
            // Get the times for the 4 points priovided
            float t0 = 0f;
            float t1 = GetTime(p0, p1, alpha);
            float t2 = GetTime(p1, p2, alpha) + t1;
            float t3 = GetTime(p2, p3, alpha) + t2;
            return (t0, t1, t2, t3);
        }




        public static (float3 position, float3 tangent) GetPositionDirectionHack(float3 p0, float3 p1, float3 p2, float3 p3, float alpha, float t)
        {
            // Get times for these points (unpacking tuple)
            (float t0, float t1, float t2, float t3) = GetPointTime(p0, p1, p2, p3, alpha);

            float catmullRomTime = math.lerp(t1, t2, t);

            float3 point = BarryGoldmanPyramidalFormulation(p0, p1, p2, p3, t0, t1, t2, t3, catmullRomTime);
            float3 pointNext = BarryGoldmanPyramidalFormulation(p0, p1, p2, p3, t0, t1, t2, t3, catmullRomTime + 0.0001f);
            float3 tangent = pointNext - point;

            return (point, tangent);
        }

    }
}