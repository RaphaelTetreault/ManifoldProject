using Manifold.IO;
using System;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ.Stage
{
    /// <summary>
    /// Represents a transformation. This is used on scene objects specifically (where
    /// hierarchy is not important - flat structures, no trees).
    /// </summary>
    [Serializable]
    public class TransformPRXS :
        IBinarySerializable,
        IBinaryAddressable,
        IDeepCopyable<TransformPRXS>
    {
        // FIELDS
         private float3 position;
         private CompressedRotation compressedRotation;
         private UnknownTransformOption unknownOption;
         private ObjectActiveOverride objectActiveOverride;
         private float3 scale;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }

        public float3 Position
        {
            get => position;
            set => position = value;
        }

        public float3 RotationEuler
        {
            get => compressedRotation.Eulers;
        }

        public quaternion Rotation
        {
            get => compressedRotation.Quaternion;
        }

        public float3 Scale
        {
            get => scale;
            set => scale = value;
        }

        public CompressedRotation CompressedRotation
        {
            get => compressedRotation;
            set => compressedRotation = value;
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
        public TransformPRXS CreateDeepCopy()
        {
            var newInstance = new TransformPRXS()
            {
                position = position,
                compressedRotation = compressedRotation,
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
                reader.ReadX(ref compressedRotation);
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
                writer.WriteX(compressedRotation);
                writer.WriteX(unknownOption);
                writer.WriteX(objectActiveOverride);
                writer.WriteX(scale);
            }
            this.RecordEndAddress(writer);
        }

        public override string ToString()
        {
            var euler = RotationEuler;
            return
                $"{nameof(TransformPRXS)}(" +
                $"{nameof(Position)}(x:{position.x:0.0}, y:{position.y:0.0}, z:{position.z:0.0}), " +
                $"{nameof(RotationEuler)}(x:{euler.x:0.0}, y:{euler.y:0.0}, z:{euler.z:0.0}), " +
                $"{nameof(Scale)}(x:{scale.x:0.0}, y:{scale.y:0.0}, z:{scale.z:0.0}), " +
                $"{nameof(unknownOption)} {unknownOption}, " +
                $"{nameof(objectActiveOverride)}: {objectActiveOverride}" +
                $")";
        }

    }
}