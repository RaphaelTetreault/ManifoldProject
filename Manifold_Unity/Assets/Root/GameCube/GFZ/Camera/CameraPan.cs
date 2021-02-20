using Manifold.IO;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;

namespace GameCube.GFZ.Camera
{
    [Serializable]
    public class CameraPan : IBinarySerializable, IBinaryAddressable
    {
        public const int kSizeBytes = 0x54;

        [HideInInspector, SerializeField, Hex] long startAddress;
        [HideInInspector, SerializeField, Hex] long endAddress;

        public int frameCount;
        public float lerpSpeed;
        [HideInInspector]
        public int zero_0x08;
        public CameraPanPoint from;
        public CameraPanPoint to;

        public long StartAddress
        {
            get => startAddress;
            set => startAddress = value;
        }

        public long EndAddress
        {
            get => endAddress;
            set => endAddress = value;
        }

        public void Deserialize(BinaryReader reader)
        {
            startAddress = reader.BaseStream.Position;

            reader.ReadX(ref frameCount);
            reader.ReadX(ref lerpSpeed);
            reader.ReadX(ref zero_0x08);
            Assert.IsTrue(zero_0x08 == 0);
            reader.ReadX(ref from, true);
            reader.ReadX(ref to, true);

            endAddress = reader.BaseStream.Position;
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
