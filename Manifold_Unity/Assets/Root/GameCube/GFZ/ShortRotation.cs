using Manifold.IO;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ
{
    [System.Serializable]
    public struct ShortRotation : IBinarySerializable
    {
        private const float shortToFloat = (short.MaxValue + 1) * 180f;
        private const float floatToShort = 1f / shortToFloat;

        [SerializeField]
        public float value;
        [SerializeField]
        private short backing;

        public float Value => value;
        public short Backing => backing;

        //public ShortRotation(short value)
        //{
        //   backing = value;
        //   this.value = value * shortToFloat;
        //}

        public ShortRotation(float value)
        {
            backing = 0;
            this.value = value;
        }

        public static implicit operator float(ShortRotation shortRotation)
        {
            return shortRotation.value;
        }

        //public static implicit operator ShortRotation(float value)
        //{
        //    return new ShortRotation(value);
        //}

        public void Deserialize(BinaryReader reader)
        {
            //short serializedValue = 0;
            reader.ReadX(ref backing);
            // Convert -32768 through 32767 to -180.0f incl. through 180.0f excl.
            value = backing / (float)(short.MaxValue + 1) * 180f;
        }

        public void Serialize(BinaryWriter writer)
        {
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
