using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.Camera
{
    [Serializable]
    public class CameraPanPoint : IBinarySerializable, IBinaryAddressable
    {

        [SerializeField]
        private AddressRange addressRange;

        public Vector3 cameraPosition;
        public Vector3 lookatPosition;
        public float fov;
        public Int16Rotation rotationRoll;
        public ushort zero_0x1E;
        public CameraPanInterpolation interpolation; //20
        public ushort zero_0x22;


        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }


        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref cameraPosition);
                reader.ReadX(ref lookatPosition);
                reader.ReadX(ref fov);
                reader.ReadX(ref rotationRoll, false);
                reader.ReadX(ref zero_0x1E);
                reader.ReadX(ref interpolation);
                reader.ReadX(ref zero_0x22);
            }
            this.RecordEndAddress(reader);
            {
                // Assertions
                Assert.IsTrue(zero_0x1E == 0);
                Assert.IsTrue(zero_0x22 == 0);
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(cameraPosition);
            writer.WriteX(lookatPosition);
            writer.WriteX(fov);
            writer.WriteX(rotationRoll);
            writer.WriteX(zero_0x1E);
            writer.WriteX(interpolation);
            writer.WriteX(zero_0x22);
        }

    }
}
