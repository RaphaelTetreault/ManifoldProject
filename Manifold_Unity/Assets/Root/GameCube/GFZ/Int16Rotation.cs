using Manifold.IO;
using System.IO;
using static Unity.Mathematics.math;

namespace GameCube.GFZ
{
    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    public struct Int16Rotation : IBinarySerializable
    {
        // CONSTANTS
        public const float shortMax = short.MaxValue + 1;

        // FIELDS
        private float degrees;
        private float radians;
        private short binary;


        // PROPERTIES
        public float Degrees
        {
            get => degrees;
            set
            {
                degrees = value;
                radians = radians(degrees);
                binary = CompressDegreesToShort(degrees);
            }
        }
        public short Binary
        {
            get => binary;
            set
            {
                binary = value;
                degrees = ShortToDegrees(binary);
                radians = radians(degrees);
            }
        }
        public float Radians
        {
            get => radians;
            set
            {
                radians = value;
                degrees = degrees(radians);
                binary = CompressDegreesToShort(degrees);
            }
        }


        // OPERATIONS
        public static implicit operator Int16Rotation(float degrees)
        {
            return new Int16Rotation() { Degrees = degrees };
        }


        // METHODS

        /// <summary>
        /// Returns a normalized short representing the rotation range -180f to +180f.
        /// </summary>
        /// <param name="value">Float within range of -180f inclusive and +180f exclusive.</param>
        /// <returns></returns>
        public static short CompressDegreesToShort(float value)
        {
            // Things to note:
            // Rotation range is -180 inclusive to +180 exclusive
            // Rotations loop. 270 degrees is the same as -90 degrees.
            // Truncation. A sufficiently large number will either overflow over 180f/0x7FFF
            // or underflow under -180f/0xFFFF. This is okay due to the above noted properties.

            // Normalize float from (-180 through +180) to (-1 through +1)
            float normalized = value / 180f;
            // Multiply by "shortMax" to set range (-32768 through +32767)
            // Overflow and underflow can happen, but that's ok.
            short result = unchecked((short)(normalized * shortMax));

            return result;
        }

        public static float ShortToDegrees(short value)
        {
            // Normalize short from (-32768 to +32767) to (-1f to +1f)
            // Multiply by max rotation value to set range (-180f inclusive to +180f exclusive)
            // I use 180f here, but this is conceptually -pi to +pi.
            float result = value / shortMax * 180f;
            return result;
        }

        public void Deserialize(BinaryReader reader)
        {
            reader.ReadX(ref binary);
            
            // Property assigns values to other backing types.
            Degrees = ShortToDegrees(binary);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(binary);
        }

        public override string ToString()
        {
            return degrees.ToString();
        }
    }
}
