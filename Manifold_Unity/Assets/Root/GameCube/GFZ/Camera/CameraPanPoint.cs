using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.Camera
{
    [Serializable]
    public class CameraPanPoint : IBinarySerializable, IBinaryAddressableRange
    {

        #region FIELDS


        private const float reciprocal = 180f / (ushort.MaxValue + 1);

        [SerializeField]
        private AddressRange addressRange;

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


        #endregion

        #region PROPERTIES


        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }


        #endregion

        #region METHODS


        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref cameraPosition);
                reader.ReadX(ref lookatPosition);
                reader.ReadX(ref fov);
                reader.ReadX(ref rotation);
                reader.ReadX(ref zero_0x1E);
                reader.ReadX(ref interpolation);
                reader.ReadX(ref zero_0x22);
            }
            this.RecordEndAddress(reader);

            // Convert -128 through 127 to -180.0f through 180.0f
            Rotation = rotation * reciprocal;

            // Assertions
            Assert.IsTrue(zero_0x1E == 0);
            Assert.IsTrue(zero_0x22 == 0);
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


        #endregion

    }
}
