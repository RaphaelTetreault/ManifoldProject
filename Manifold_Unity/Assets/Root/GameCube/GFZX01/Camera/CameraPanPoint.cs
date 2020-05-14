using StarkTools.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZX01.Camera
{
    [Serializable]
    public struct CameraPanPoint : IBinarySerializable, IBinaryAddressable
    {
        [SerializeField, Hex] long startAddress;
        [SerializeField, Hex] long endAddress;

        public Vector3 cameraPosition;
        public Vector3 lookatPosition;
        public float fov;
        public CameraPanModifier modifier;
        public CameraPanMode mode;

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
            reader.ReadX(ref modifier);
            reader.ReadX(ref mode);

            endAddress = reader.BaseStream.Position;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(cameraPosition);
            writer.WriteX(lookatPosition);
            writer.WriteX(fov);
            writer.WriteX(modifier);
            writer.WriteX(mode);
        }
    }
}
