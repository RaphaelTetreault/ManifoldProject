using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;

namespace Manifold.Spline
{
    public struct BezierCurve
    {
        public BezierControlPointMode mode;
        public float3 p0;
        public float3 p1;
        public float3 p2;
        public float3 p3;
    }
}
