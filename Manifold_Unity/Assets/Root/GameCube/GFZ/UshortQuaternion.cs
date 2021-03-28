using Manifold.IO;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ
{
    [System.Serializable]
    public struct UshortQuaternion : IBinarySerializable
    {
        public ushort x;
        public ushort y;
        public ushort z;
        public EnumFlags16 unkFlags;

        public float fx;
        public float fy;
        public float fz;
        public float fw;

        public Quaternion rotation;

        public Vector3 AsVector3
            => AsQuaternion.eulerAngles;

        public Quaternion AsQuaternion
            => new Quaternion(fx, fy, fz, fw);



        public static implicit operator Vector3(UshortQuaternion value)
        {
            var vector3 = new Vector3(value.x, value.y, value.z);
            return vector3;
        }

        //public static implicit operator ShortRotation3(Vector3 value)
        //{
        //    var rotation3 = new ShortRotation3()
        //    {
        //        x = value.x,
        //        y = value.y,
        //        z = value.z,
        //    };
        //    return rotation3;
        //}

        public void Deserialize(BinaryReader reader)
        {
            reader.ReadX(ref x);
            reader.ReadX(ref y);
            reader.ReadX(ref z);
            reader.ReadX(ref unkFlags);

            // Normalize ushort from 0-65535 to 0.0f-1.0f
            // Multiply by max rotation value 360f
            float ushortMax = ushort.MaxValue;
            fx = x / ushortMax * 360f;
            fy = y / ushortMax * 360f;
            fz = z / ushortMax * 360f;

            // Reset
            rotation = Quaternion.identity;
            // Apply rotation in discrete sequence. Yes, really.
            rotation = Quaternion.Euler(fx, 0, 0) * rotation;
            rotation = Quaternion.Euler(0, fy, 0) * rotation;
            rotation = Quaternion.Euler(0, 0, fz) * rotation;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(x);
            writer.WriteX(y);
            writer.WriteX(z);
            writer.WriteX(unkFlags);

            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return $"({x.ToString("0.0")},{y.ToString("0.0")},{z.ToString("0.0")})";
        }
    }
}
