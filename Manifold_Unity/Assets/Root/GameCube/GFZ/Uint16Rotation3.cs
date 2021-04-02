using Manifold.IO;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ
{
    [System.Serializable]
    public struct Uint16Rotation3 : IBinarySerializable
    {
        public const float ushortMax = ushort.MaxValue;
        public const float angleMax = 360f;

        public ushort x;
        public ushort y;
        public ushort z;
        public EnumFlags16 unkFlags;

        public Quaternion rotation;
        public Vector3 eulerAngles;

        public Vector3 EulerAngles => eulerAngles;
        public Quaternion Rotation => rotation;


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

        public static ushort FloatToUshort(float value)
        {
            // Normalize float from 0.0f-360.0f to 0.0f-1.0f
            // Multiply by ushort max to set range 0-65535
            ushort result = (ushort)(value / angleMax * ushortMax);
            return result;
        }

        public static float UshortToFloat(ushort value)
        {
            // Normalize ushort from 0-65535 to 0.0f-1.0f
            // Multiply by max rotation value 360f
            float result = value / ushortMax * angleMax;
            result = result >= 180f
                ? result - 360f
                : result;

            return result;
        }

        public static Quaternion UshortXyzToQuaternion(ushort x, ushort y, ushort z)
        {
            // Unpacked ushort to float
            float fx = UshortToFloat(x);
            float fy = UshortToFloat(y);
            float fz = UshortToFloat(z);

            // Reconstruct rotation from partial data
            var rotation = Quaternion.identity;
            // Apply rotation in discrete sequence. Yes, really.
            rotation = Quaternion.Euler(fx, 0, 0) * rotation;
            rotation = Quaternion.Euler(0, fy, 0) * rotation;
            rotation = Quaternion.Euler(0, 0, fz) * rotation;

            return rotation;
        }

        public static (ushort ux, ushort uy, ushort uz) QuaternionToUshortXyz(Quaternion value)
        {


            float rotationZ = 0f;
            float rotationY = 0f;
            float rotationX = 0f;

            ushort ux = FloatToUshort(rotationX);
            ushort uy = FloatToUshort(rotationY);
            ushort uz = FloatToUshort(rotationZ);

            return (ux, uy, uz);
        }

        public void Deserialize(BinaryReader reader)
        {
            // DESERIALIZE COMPRESSED DATA
            {
                reader.ReadX(ref x);
                reader.ReadX(ref y);
                reader.ReadX(ref z);
                reader.ReadX(ref unkFlags);
            }
            // CONVERT COMPRESSED DATA
            {
                // Reconstruct rotation from partial data
                rotation = UshortXyzToQuaternion(x, y, z);
                // Store eulerAngles rather than reconstructing upon request
                eulerAngles = rotation.eulerAngles;
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
