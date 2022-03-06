using Manifold.IO;
using System;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ.Stage
{
    /// <summary>
    /// Represents a transformation. This is used on scene objects specifically. TRXS is 
    /// indicative of the values stored. T:translation, R:rotation, X:extra, S: scale.
    /// As noted, it contains "extra" bits.
    /// </summary>
    [Serializable]
    public class TransformTRXS :
        IBinarySerializable,
        IBinaryAddressable,
        IDeepCopyable<TransformTRXS>
    {
        // FIELDS
         private float3 position;
         private CompressedRotation compressedRotation;
         private UnknownTransformOption unknownOption;
         private ObjectActiveOverride objectActiveOverride;
         private float3 scale;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public CompressedRotation CompressedRotation
        {
            get => compressedRotation;
            set => compressedRotation = value;
        }
        public ObjectActiveOverride ObjectActiveOverride
        {
            get => objectActiveOverride;
            set => objectActiveOverride = value;
        }
        public float3 Position
        {
            get => position;
            set => position = value;
        }
        public quaternion Rotation
        {
            get => compressedRotation.Quaternion;
        }
        public float3 RotationEuler
        {
            get => compressedRotation.Eulers;
        }
        public float3 Scale
        {
            get => scale;
            set => scale = value;
        }
        public UnknownTransformOption UnknownOption
        {
            get => unknownOption;
            set => unknownOption = value;
        }


        // METHODS
        public TransformTRXS CreateDeepCopy()
        {
            var newInstance = new TransformTRXS()
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
                $"{nameof(TransformTRXS)}(" +
                $"{nameof(Position)}(x:{position.x:0.0}, y:{position.y:0.0}, z:{position.z:0.0}), " +
                $"{nameof(RotationEuler)}(x:{euler.x:0.0}, y:{euler.y:0.0}, z:{euler.z:0.0}), " +
                $"{nameof(Scale)}(x:{scale.x:0.0}, y:{scale.y:0.0}, z:{scale.z:0.0}), " +
                $"{nameof(unknownOption)} {unknownOption}, " +
                $"{nameof(objectActiveOverride)}: {objectActiveOverride}" +
                $")";
        }

    }
}