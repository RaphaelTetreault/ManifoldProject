using Manifold.IO;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ
{
    [System.Serializable]
    public struct ShortRotation3 : IBinarySerializable
    {
        public ShortRotation x;
        public ShortRotation y;
        public ShortRotation z;

        public static implicit operator Vector3(ShortRotation3 value)
        {
            var vector3 = new Vector3(value.x, value.y, value.z);
            return vector3;
        }

        public static implicit operator ShortRotation3(Vector3 value)
        {
            var rotation3 = new ShortRotation3()
            {
                x = value.x,
                y = value.y,
                z = value.z,
            };
            return rotation3;
        }

        public void Deserialize(BinaryReader reader)
        {
            reader.ReadX(ref x, true);
            reader.ReadX(ref y, true);
            reader.ReadX(ref z, true);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(x);
            writer.WriteX(y);
            writer.WriteX(z);
        }

        public override string ToString()
        {
            return $"({x.ToString("0.00")},{y.ToString("0.00")},{z.ToString("0.00")})";
        }
    }
}
