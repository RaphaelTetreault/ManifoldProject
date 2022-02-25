using System;
using Unity.Mathematics;

namespace GameCube.GX
{
    [Serializable]
    public struct NormalBinormalTangent
    {
        public float3 normal;
        public float3 binormal;
        public float3 tangent;
    }
}
