using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;

namespace Manifold.Spline
{
    public static class BezierSpline
    {

        public static float3 GetPoint(float3 p0, float3 p1, float3 p2, float3 p3, float t)
        {
            t = math.clamp(t, 0, 1);
            float oneMinusT = 1f - t;
            return
                oneMinusT * oneMinusT * oneMinusT * p0 +
                3f * oneMinusT * oneMinusT * t * p1 +
                3f * oneMinusT * t * t * p2 +
                t * t * t * p3;
        }

        public static float3 GetFirstDerivative(float3 p0, float3 p1, float3 p2, float3 p3, float t)
        {
            t = math.clamp(t, 0, 1);
            float oneMinusT = 1f - t;
            return
                3f * oneMinusT * oneMinusT * (p1 - p0) +
                6f * oneMinusT * t * (p2 - p1) +
                3f * t * t * (p3 - p2);
        }




        //public static float3 GetPoint(float3 p0, float3 p1, float3 p2, float t)
        //{
        //    t = math.clamp(t, 0f, 1f);
        //    float invT = 1f - t;

        //    float3 quadraticBezierPoint =
        //        invT * invT * p0 +
        //        2f * invT * t * p1 +
        //        t * t * p2;

        //    return quadraticBezierPoint;

        //    //var a = math.lerp(p0, p1, t);
        //    //var b = math.lerp(p1, p2, t);
        //    //var p = math.lerp(a, b, t);
        //    //return p;
        //}

        //public static float3 GetFirstDerivative(float3 p0, float3 p1, float3 p2, float t)
        //{
        //    float3 firstDerivitive =
        //        2f * (1f - t) * (p1 - p0) +
        //        2f * t * (p2 - p1);

        //    return firstDerivitive;
        //}


    }
}
