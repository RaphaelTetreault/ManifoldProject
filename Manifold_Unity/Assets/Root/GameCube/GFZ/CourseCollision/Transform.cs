using Manifold.IO;
using System;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// Represents a transformation. This is used on scene objects specifically (where
    /// hierarchy is not important - flat structures, no trees).
    /// </summary>
    [Serializable]
    public class Transform :
        IBinarySerializable,
        IBinaryAddressable,
        IDeepCopyable<Transform>
    {
        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;

        // FIELDS
        [UnityEngine.SerializeField] private float3 position;
        [UnityEngine.SerializeField] private Int16Rotation3 decomposedRotation;
        [UnityEngine.SerializeField] private UnknownTransformOption unknownOption;
        [UnityEngine.SerializeField] private ObjectActiveOverride objectActiveOverride;
        [UnityEngine.SerializeField] private float3 scale;


        // PROPERTIES
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


        // METHODS
        public Transform CreateDeepCopy()
        {
            var newInstance = new Transform()
            {
                position = position,
                decomposedRotation = decomposedRotation,
                scale = scale,
                unknownOption = unknownOption,
                objectActiveOverride = objectActiveOverride,
            };
            return newInstance;
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
            this.RecordStartAddress(writer);
            {
                writer.WriteX(position);
                writer.WriteX(decomposedRotation);
                writer.WriteX(objectActiveOverride);
                writer.WriteX(unknownOption);
                writer.WriteX(scale);
            }
            this.RecordEndAddress(writer);
        }

        public override string ToString()
        {
            var euler = RotationEuler;
            return
                $"{nameof(Transform)}(" +
                $"{nameof(Position)}(x:{position.x:0.0}, y:{position.y:0.0}, z:{position.z:0.0}), " +
                $"{nameof(RotationEuler)}(x:{euler.x:0.0}, y:{euler.y:0.0}, z:{euler.z:0.0}), " +
                $"{nameof(Scale)}(x:{scale.x:0.0}, y:{scale.y:0.0}, z:{scale.z:0.0}), " +
                $"{nameof(unknownOption)} {unknownOption}, " +
                $"{nameof(objectActiveOverride)}: {objectActiveOverride}" +
                $")";
        }

    }
}