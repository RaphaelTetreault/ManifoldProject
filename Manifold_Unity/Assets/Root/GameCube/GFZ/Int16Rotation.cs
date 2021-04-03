using Manifold.IO;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ
{
    [System.Serializable]
    public struct Int16Rotation : IBinarySerializable
    {
        public const float shortMax = short.MaxValue + 1;
        // TODO: should you use the constant or hard-code for ease of reading?
        public const float angleMax = 180f;

        [SerializeField]
        private float value;
        [SerializeField]
        private short binary;

        public float Value => value;
        public short Binary => binary;

        public Int16Rotation(float value)
        {
            this.value = value;
            this.binary = FloatToShort(value);
        }

        public static implicit operator float(Int16Rotation shortRotation)
        {
            return shortRotation.value;
        }

        public static implicit operator Int16Rotation(float value)
        {
            return new Int16Rotation(value);
        }

        public static short FloatToShort(float value)
        {
            // Deconstruct rotation into various components
            float sign = Mathf.Sign(value);
            float abs = Mathf.Abs(value);
            float range360 = abs % 360;
            float signedRange360 = sign * range360;

            // Constrain rotations to -180f to +180f range
            // When abs value is greater than 180, get same rotation
            // with opposite sign. Using the sign ensures +/- ranges
            // are properly mirrored.
            // Examples. Sample rotation value +/- 350
            // ex: (+) 350 > 180: +350 - (+1 * 360) == +350 - 360 = -10
            // ex: (-) 350 > 180: -350 - (-1 * 360) == -350 + 360 = +10
            float range180 = (range360 > 180f)
                ? signedRange360 - (sign * 360f)
                : signedRange360;

            // Normalize float from 360f to 0-1
            float normalized = range180 / 180f;
            // Multiply by ushort max to set range 0-65535
            short result = (short)(normalized * shortMax);

            return result;
        }

        public static float UshortToFloat(short value)
        {
            // Normalize short from (-32768 to +32767) to (-1f to +1f)
            // Multiply by max rotation value to set range (-180f inclusive to +180f exclusive)
            // I use 180f here, but this is conceptually -pi to +pi.
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
            writer.WriteX(binary);
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
