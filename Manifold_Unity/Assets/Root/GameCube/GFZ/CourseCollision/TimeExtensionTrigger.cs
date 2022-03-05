using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    // 2022/01/27: previously ArcadeCheckpointTrigger

    /// <summary>
    /// A checkpoint used in the F-Zero AX style arcade mode. When passed through, the
    /// game will add extend the remaining race time. This is used in the AX cup courses
    /// Port Town [Cylinder Wave], Lightning [Thunder Road], and Green Plant [Spiral].
    /// 
    /// TODO: it is unclear where the actual time is defined per checkpoint or course.
    /// </summary>
    [Serializable]
    public class TimeExtensionTrigger :
        IBinaryAddressable,
        IBinarySerializable
    {
        // FIELDS
        public TransformPRXS transform;
        public TimeExtensionOption option;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }


        //METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref transform);
                reader.ReadX(ref option);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                writer.WriteX(transform);
                writer.WriteX(option);
            }
            this.RecordEndAddress(writer);
        }

        public override string ToString()
        {
            return
                $"{nameof(TimeExtensionTrigger)}(" +
                $"{nameof(option)}: {option}, " +
                $"{transform}" +
                $")";
        }

    }
}
