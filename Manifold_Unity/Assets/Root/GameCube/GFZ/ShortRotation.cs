using Manifold.IO;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ
{
    [System.Serializable]
    public struct ShortRotation : IBinarySerializable
    {
        private const float shortToFloat = 360f / (ushort.MaxValue);
        private const float floatToshort = 1f / shortToFloat;

        [SerializeField]
        private float value;

        public float Value => value;

        public ShortRotation(float value)
        {
            this.value = value;
        }

        public static implicit operator float(ShortRotation shortRotation)
        {
            return shortRotation.value;
        }

        public static implicit operator ShortRotation(float value)
        {
            return new ShortRotation(value);
        }

        public void Deserialize(BinaryReader reader)
        {
            short serializedValue = 0;
            reader.ReadX(ref serializedValue);
            // Convert -128 through 127 to -180.0f through 180.0f
            value = serializedValue * shortToFloat;
        }

        public void Serialize(BinaryWriter writer)
        {
            var rotation = (short)(value * floatToshort);
            writer.WriteX(rotation);

            // THIS SERIALIZATION IS NOT TESTED
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return value.ToString();
        }

        public string ToString(string format)
        {
            return value.ToString(format);
        }
    }
}
