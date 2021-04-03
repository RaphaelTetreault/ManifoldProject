using Manifold.IO;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ
{
    [System.Serializable]
    public struct Int16Rotation : IBinarySerializable
    {
        public const float shortMax = short.MaxValue;
        public const float angleMax = 180f;

        [SerializeField]
        private float value;
        [SerializeField]
        private short binary;

        public float Value => value;
        public short Binary => binary;

        public Int16Rotation(float value)
        {
            binary = 0;
            this.value = value;
        }

        public static implicit operator float(Int16Rotation shortRotation)
        {
            return shortRotation.value;
        }

        public static implicit operator Int16Rotation(float value)
        {
            return new Int16Rotation(value);
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
            ushort result = (ushort)(normalized * shortMax);

            return result;
        }

        public static float UshortToFloat(short value)
        {
            //// Normalize ushort from 0-65535 to 0.0f-1.0f
            //float normalized = value / ushortMax;
            //// Multiply by max rotation value 360f
            //float result = normalized * angleMax;
            //// Constrain to -180 through 180 degree range (-pi to +pi)
            //// This range has rotations working as expected
            //// Anything above pi is in the -pi range
            //float result = (rotation >= 180f)
            //    ? (rotation - 360f)
            //    : rotation;

            // Normalize ushort from 0-65535 to 0.0f-1.0f
            float result = value / shortMax * angleMax;
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
