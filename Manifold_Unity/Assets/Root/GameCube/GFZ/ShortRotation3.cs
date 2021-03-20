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
        public EnumFlags16 unkFlags;

        public Vector3 AsVector3
            => new Vector3(x, y, z);

        public Quaternion AsQuaternion
            => Quaternion.Euler(AsVector3);



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
            reader.ReadX(ref unkFlags);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(x);
            writer.WriteX(y);
            writer.WriteX(z);
            writer.WriteX((ushort)0);
        }

        public override string ToString()
        {
            return $"({x.ToString("0.0")},{y.ToString("0.0")},{z.ToString("0.0")})";
        }
    }
}
