using Manifold.IO;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ
{
    [System.Serializable]
    public struct Uint16Rotation3 : IBinarySerializable
    {
        public Uint16Rotation x;
        public Uint16Rotation y;
        public Uint16Rotation z;
        public EnumFlags16 unkFlags;

        [SerializeField]
        private Quaternion rotation;
        [SerializeField]
        private Vector3 eulerAngles;

        public Vector3 EulerAngles => eulerAngles;
        public Quaternion Rotation => rotation;

        //public Uint16Rotation3()
        //{
        //    x = y = z = 0f;
        //    unkFlags = 0;

        //    eulerAngles = Vector3.zero;
        //    rotation = Quaternion.identity;
        //}

        public Uint16Rotation3(Uint16Rotation x, Uint16Rotation y, Uint16Rotation z, EnumFlags16 flags = 0)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.unkFlags = flags;

            eulerAngles = new Vector3(x, y, z);
            rotation = Quaternion.Euler(eulerAngles);
        }

        public static implicit operator Quaternion(Uint16Rotation3 value)
        {
            return value.rotation;
        }

        public static implicit operator Vector3(Uint16Rotation3 value)
        {
            return value.eulerAngles;
        }

        public static implicit operator Uint16Rotation3(Vector3 value)
        {
            var ushortRotation = new Uint16Rotation3()
            {

            };
            return ushortRotation;
        }

        public static implicit operator Uint16Rotation3(Quaternion value)
        {
            var ushortRotation = new Uint16Rotation3()
            {

            };
            return ushortRotation;
        }

        public void Deserialize(BinaryReader reader)
        {
            {
                reader.ReadX(ref x, false);
                reader.ReadX(ref y, false);
                reader.ReadX(ref z, false);
                reader.ReadX(ref unkFlags);
            }
            //
            {
                eulerAngles = new Vector3(x, y, z);
                rotation = Quaternion.Euler(eulerAngles);
            }
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
            var euler = rotation.eulerAngles;
            return $"({euler.x:0.0}, {euler.x:0.0}, {euler.x:0.0})";
        }
    }
}
