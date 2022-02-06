using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public interface IPositionEvaluable
    {
        public float3 EvaluatePosition(double time);
    }
}
