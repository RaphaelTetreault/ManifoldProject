using Manifold.IO;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ
{
    [System.Serializable]
    public struct Uint16Rotation : IBinarySerializable
    {
        public const float ushortMax = ushort.MaxValue;
        public const float angleMax = 360f;

        [SerializeField]
        private float value;
        [SerializeField]
        private ushort binary;

        public float Value => value;
        public ushort Binary => binary;

        public Uint16Rotation(float value)
        {
            binary = 0;
            this.value = value;
        }

        public static implicit operator float(Uint16Rotation shortRotation)
        {
            return shortRotation.value;
        }

        public static implicit operator Uint16Rotation(float value)
        {
            return new Uint16Rotation(value);
        }

        public static ushort FloatToUshort(float value)
        {
            // Get sign, absolute value, and clamp to range 0-360
            float sign = Mathf.Sign(value);
            float abs = Mathf.Abs(value);
            float range360 = abs % 360;

            // Keep rotations positive for normalization process
            bool isNegativeRotation = sign < 0;
            float positiveRange360 = isNegativeRotation
                ? 360f + range360
                : range360;

            // 
            //float rangeTwoPi = (positiveRange360 > 180f)
            //    ? range360Signed + 360f
            //    : range360Signed;

            // Normalize float from 360f to 0-1
            float normalized = positiveRange360 / 360f;
            // Multiply by ushort max to set range 0-65535
            ushort result = (ushort)(normalized * ushortMax);

            return result;
        }

        public static float UshortToFloat(ushort value)
        {
            // Normalize ushort from 0-65535 to 0.0f-1.0f
            float normalized = value / ushortMax;
            // Multiply by max rotation value 360f
            float rotation = normalized * angleMax;
            // Constrain to -180 through 180 degree range (-pi to +pi)
            // This range has rotations working as expected
            // Anything above pi is in the -pi range
            float result = (rotation >= 180f)
                ? (rotation - 360f)
                : rotation;

            return result;
        }

        public void Deserialize(BinaryReader reader)
        {
            reader.ReadX(ref binary);
            value = UshortToFloat(binary);
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return value.ToString();
        }

        public string ToString(string decimalFormat)
        {
            return value.ToString(decimalFormat);
        }
    }
}
