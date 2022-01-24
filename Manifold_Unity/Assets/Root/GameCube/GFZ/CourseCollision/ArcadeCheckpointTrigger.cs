using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// A checkpoint used in the F-Zero AX style arcade mode. When passed through, the
    /// game will add extend the remaining race time. This is used in the AX cup courses
    /// Port Town [Cylinder Wave], Lightning [Thunder Road], and Green Plant [Spiral].
    /// 
    /// TODO: it is unclear where the actual time is defined per checkpoint or course.
    /// </summary>
    [Serializable]
    public class ArcadeCheckpointTrigger :
        IBinaryAddressable,
        IBinarySerializable
    {
        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;

        // FIELDS
        public TransformPRXS transform;
        public ArcadeCheckpointType type;


        // PROPERTIES
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }


        //METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref transform, true);
                reader.ReadX(ref type);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                writer.WriteX(transform);
                writer.WriteX(type);
            }
            this.RecordEndAddress(writer);
        }

        public override string ToString()
        {
            return
                $"{nameof(ArcadeCheckpointTrigger)}(" +
                $"{nameof(type)}: {type}, " +
                $"{transform}" +
                $")";
        }

    }
}
