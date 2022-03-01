using Manifold;
using Manifold.IO;
using System;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GX
{
    [Serializable]
    public struct NormalBinormalTangent :
        IBinarySerializable
    {
        public float3 normal;
        public float3 binormal;
        public float3 tangent;

        public void Deserialize(BinaryReader reader)
        {
            reader.ReadX(ref normal);
            reader.ReadX(ref binormal);
            reader.ReadX(ref tangent);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(normal);
            writer.WriteX(binormal);
            writer.WriteX(tangent);
        }
    }
}
