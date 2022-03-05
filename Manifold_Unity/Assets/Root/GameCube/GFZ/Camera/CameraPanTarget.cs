using Manifold;
using Manifold.EditorTools;
using Manifold.IO;
using System;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ.Camera
{
    [Serializable]
    public class CameraPanTarget :
        IBinarySerializable,
        IBinaryAddressable
    {
        // METADATA
        private AddressRange addressRange;

        // FIELDS
        public float3 cameraPosition;
        public float3 lookAtPosition;
        public float fieldOfView;
        public Int16Rotation rotationRoll;
        public ushort zero_0x1E;
        public CameraPanInterpolation interpolation;
        public ushort zero_0x22;


        // PROPERTIES
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        public float3 CameraPosition
        {
            get => cameraPosition;
            set => cameraPosition = value;
        }

        public float3 LookAtPosition
        {
            get => lookAtPosition;
            set => lookAtPosition = value;
        }

        public float FieldOfView
        {
            get => fieldOfView;
            set => fieldOfView = value;
        }

        public Int16Rotation RotationRoll
        {
            get => rotationRoll;
            set => rotationRoll = value;
        }

        public ushort Zero_0x1E
        {
            get => zero_0x1E;
            set => zero_0x1E = value;
        }

        public CameraPanInterpolation Interpolation
        {
            get => interpolation;
            set => interpolation = value;
        }

        public ushort Zero_0x22
        {
            get => zero_0x22;
            set => zero_0x22 = value;
        }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref cameraPosition);
                reader.ReadX(ref lookAtPosition);
                reader.ReadX(ref fieldOfView);
                reader.ReadX(ref rotationRoll);
                reader.ReadX(ref zero_0x1E);
                reader.ReadX(ref interpolation);
                reader.ReadX(ref zero_0x22);
            }
            this.RecordEndAddress(reader);

            // Assertions
            Assert.IsTrue(Zero_0x1E == 0);
            Assert.IsTrue(Zero_0x22 == 0);
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                writer.WriteX(cameraPosition);
                writer.WriteX(lookAtPosition);
                writer.WriteX(fieldOfView);
                writer.WriteX(rotationRoll);
                writer.WriteX(zero_0x1E);
                writer.WriteX(interpolation);
                writer.WriteX(zero_0x22);
            }
            this.RecordEndAddress(writer);
        }

    }
}
