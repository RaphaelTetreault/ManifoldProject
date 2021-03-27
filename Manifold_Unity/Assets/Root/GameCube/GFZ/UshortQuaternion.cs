using Manifold.IO;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ
{
    [System.Serializable]
    public struct UshortQuaternion : IBinarySerializable
    {
        //public ShortRotation x;
        //public ShortRotation y;
        //public ShortRotation z;
        //public EnumFlags16 unkFlags;

        [SerializeField] public ushort x;
        [SerializeField] public ushort y;
        [SerializeField] public ushort z;
        [SerializeField] public ushort flags;

        public float fx;
        public float fy;
        public float fz;
        public float fw;

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
            reader.ReadX(ref flags);

            float max = ushort.MaxValue;
            fx = x / max;
            fy = y / max;
            fz = z / max;

            // Rededrive W component of quaternion
            float powX = Mathf.Pow(fx, 2);
            float powY = Mathf.Pow(fy, 2);
            float powZ = Mathf.Pow(fz, 2);
            fw = 1f - (powX + powY + powZ);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(x);
            writer.WriteX(y);
            writer.WriteX(z);
            writer.WriteX(flags);

            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return $"({x.ToString("0.0")},{y.ToString("0.0")},{z.ToString("0.0")})";
        }
    }
}
