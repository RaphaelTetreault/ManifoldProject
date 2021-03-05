using System;
using UnityEngine;

namespace GameCube.GX
{
    [Serializable]
    public struct NormalBinormalTangent
    {
        public Vector3 normal;
        public Vector3 binormal;
        public Vector3 tangent;
    }
}
