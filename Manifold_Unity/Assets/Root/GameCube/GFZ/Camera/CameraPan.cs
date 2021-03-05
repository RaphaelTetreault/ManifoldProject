using Manifold.IO;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;

namespace GameCube.GFZ.Camera
{
    [Serializable]
    public class CameraPan : IBinarySerializable, IBinaryAddressableRange
    {
        public const int kSizeBytes = 0x54;

        [SerializeField, HideInInspector]
        private AddressRange addressRange;

        public int frameCount;
        public float lerpSpeed;
        [HideInInspector]
        public int zero_0x08;
        public CameraPanPoint from;
        public CameraPanPoint to;

        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);

            reader.ReadX(ref frameCount);
            reader.ReadX(ref lerpSpeed);
            reader.ReadX(ref zero_0x08);
            reader.ReadX(ref from, true);
            reader.ReadX(ref to, true);

            this.RecordEndAddress(reader);

            // Assertions
            Assert.IsTrue(zero_0x08 == 0);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(frameCount);
            writer.WriteX(lerpSpeed);
            writer.WriteX(zero_0x08);
            writer.WriteX(from);
            writer.WriteX(to);
        }
    }
}
