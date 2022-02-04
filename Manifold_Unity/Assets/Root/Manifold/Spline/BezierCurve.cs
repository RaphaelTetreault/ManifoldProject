using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;

namespace Manifold.Spline
{

    public struct BezierPoint
    {
        public BezierControlPointMode mode;
        public float3 point;
        public float3 control;
        public float width;
        public float roll;
    }

}
