using Manifold.IO;
using System;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class Transform : IBinarySerializable, IBinaryAddressableRange
    {
        // metadata
        [UnityEngine.SerializeField]
        private AddressRange addressRange;

        // structure
        [UnityEngine.SerializeField]
        private float3 position;
        [UnityEngine.SerializeField]
        private Int16Rotation3 decomposedRotation;
        [UnityEngine.SerializeField]
        private UnknownTransformOption unknownOption;
        [UnityEngine.SerializeField]
        private ObjectActiveOverride objectActiveOverride;
        [UnityEngine.SerializeField]
        private float3 scale;

        //
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        public float3 Position
        {
            get => position;
            set => position = value;
        }

        public float3 RotationEuler
        {
            get => decomposedRotation.EulerAngles;
        }

        public quaternion Rotation
        {
            get => decomposedRotation.Rotation;
        }

        public float3 Scale
        {
            get => scale;
            set => scale = value;
        }

        public Int16Rotation3 DecomposedRotation
        {
            get => decomposedRotation;
            set => decomposedRotation = value;
        }

        public UnknownTransformOption UnknownOption
        {
            get => unknownOption;
            set => unknownOption = value;
        }

        public ObjectActiveOverride ObjectActiveOverride
        {
            get => objectActiveOverride;
            set => objectActiveOverride = value;
        }

        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref position);
                reader.ReadX(ref decomposedRotation, true);
                reader.ReadX(ref unknownOption);
                reader.ReadX(ref objectActiveOverride);
                reader.ReadX(ref scale);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(position);
            writer.WriteX(decomposedRotation);
            writer.WriteX(objectActiveOverride);
            writer.WriteX(unknownOption);
            writer.WriteX(scale);
        }
    }
}