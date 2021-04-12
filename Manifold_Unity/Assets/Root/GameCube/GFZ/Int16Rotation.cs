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
            this.binary = FloatToShortRange180(value);
        }

        public static implicit operator float(Int16Rotation shortRotation)
        {
            return shortRotation.value;
        }

        public static implicit operator Int16Rotation(float value)
        {
            return new Int16Rotation(value);
        }

        /// <summary>
        /// Returns a normalized short representing the rotation range -180f to +180f.
        /// </summary>
        /// <param name="value">Float within range of -180f inclusive and +180f exclusive.</param>
        /// <returns></returns>
        public static short FloatToShortRange180(float value)
        {
            // WOULD BE PROPER, BUT OUR DECOMPOSER ONLY HAS VALUES IN RANGE
            //// Deconstruct rotation into various components
            //float sign = Mathf.Sign(value);
            //float abs = Mathf.Abs(value);
            //float range360 = abs % 360;
            //float signedRange360 = sign * range360;
            //
            //// Constrain rotations to -180f to +180f range
            //// When abs value is greater than 180, get same rotation
            //// with opposite sign. Using the sign ensures +/- ranges
            //// are properly mirrored.
            //// Examples. Sample rotation value +/- 350
            //// ex: (+) 350 > 180: +350 - (+1 * 360) == +350 - 360 = -10
            //// ex: (-) 350 > 180: -350 - (-1 * 360) == -350 + 360 = +10
            //float range180 = (range360 > 180f)
            //    ? signedRange360 - (sign * 360f)
            //    : signedRange360;
            //
            // Normalize float from -180 through +180 to -1 through +1
            //float normalized = range180 / 180f;
            // Multiply by "shortMax" to set range -32768 through +32767
            //short result = (short)(normalized * shortMax);

            // ASSERT WE ARE DEALING WITH PROPER VALUES
            // Check range, -180 inclusive, +180 exclusive
            Assert.IsTrue(value <  +180f);
            Assert.IsTrue(value >= -180f);
            // Normalize float from -180 through +180 to -1 through +1
            float normalized = value / 180f;
            // Multiply by "shortMax" to set range -32768 through +32767
            short result = (short)(normalized * shortMax);

            return result;
        }

        public static float ShortToFloat(short value)
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
            value = ShortToFloat(binary);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(binary);
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }
}
