using Manifold.IO;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;

namespace GameCube.GFZ.Camera
{
    [Serializable]
    public class CameraPanPoint : IBinarySerializable, IBinaryAddressable
    {
        private const float reciprocal = 180f / (ushort.MaxValue + 1);

        [HideInInspector, SerializeField, Hex] long startAddress;
        [HideInInspector, SerializeField, Hex] long endAddress;

        public Vector3 cameraPosition;
        public Vector3 lookatPosition;
        public float fov;
        [Range(180f, -180f)]
        public float Rotation;
        [HideInInspector]
        public short rotation;
        [HideInInspector]
        public ushort zero_0x1E;
        public CameraPanInterpolation interpolation; //20
        [HideInInspector]
        public ushort zero_0x22;

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

            reader.ReadX(ref cameraPosition);
            reader.ReadX(ref lookatPosition);
            reader.ReadX(ref fov);
            reader.ReadX(ref rotation);
            reader.ReadX(ref zero_0x1E);
            Assert.IsTrue(zero_0x1E == 0);
            reader.ReadX(ref interpolation);
            reader.ReadX(ref zero_0x22);
            Assert.IsTrue(zero_0x22 == 0);

            endAddress = reader.BaseStream.Position;

            // Convert -128 through 127 to -180 through 180
            Rotation = rotation * reciprocal;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(cameraPosition);
            writer.WriteX(lookatPosition);
            writer.WriteX(fov);
            var rotation = (short)(Rotation / reciprocal);
            writer.WriteX(rotation);
            writer.WriteX(zero_0x1E);
            writer.WriteX(interpolation);
            writer.WriteX(zero_0x22);
        }
    }
}
