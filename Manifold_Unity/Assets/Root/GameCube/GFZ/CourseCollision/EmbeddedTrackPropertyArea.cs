using Manifold;
using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// Descriptor for where along the track a desireble surface attribute is located
    /// such as boost plates, jump plates, and heal strips. Likely for use by AI.
    /// </summary>
    [Serializable]
    public class EmbeddedTrackPropertyArea :
        IBinaryAddressable,
        IBinarySerializable
    {
        // FIELDS
        // Default values represent final node.
        public float lengthFrom = -1f;
        public float lengthTo = -1f;
        public float widthLeft = 0;
        public float widthRight = 0;
        public EmbeddedTrackPropertyType propertyType = EmbeddedTrackPropertyType.TerminateCode;
        public byte trackBranchID = 0;
        public ushort zero_0x12;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }

        public static EmbeddedTrackPropertyArea Terminator()
        {
            return new EmbeddedTrackPropertyArea()
            {
                lengthFrom = -1f,
                lengthTo = -1f,
                widthLeft = 0,
                widthRight = 0,
                propertyType = EmbeddedTrackPropertyType.TerminateCode,
                trackBranchID = 0,
                zero_0x12 = 0,
            };
        }

        public static EmbeddedTrackPropertyArea[] DefaultArray()
        {
            return new EmbeddedTrackPropertyArea[]
            {
                Terminator(),
            };
        }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref lengthFrom);
                reader.ReadX(ref lengthTo);
                reader.ReadX(ref widthLeft);
                reader.ReadX(ref widthRight);
                reader.ReadX(ref propertyType);
                reader.ReadX(ref trackBranchID);
                reader.ReadX(ref zero_0x12);
            }
            this.RecordEndAddress(reader);
            {
                Assert.IsTrue(zero_0x12 == 0);
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            {
                Assert.IsTrue(zero_0x12 == 0);
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(lengthFrom);
                writer.WriteX(lengthTo);
                writer.WriteX(widthLeft);
                writer.WriteX(widthRight);
                writer.WriteX(propertyType);
                writer.WriteX(trackBranchID);
                writer.WriteX(zero_0x12);
            }
            this.RecordEndAddress(writer);
        }

        public override string ToString()
        {
            return 
                $"{nameof(EmbeddedTrackPropertyArea)}(" +
                $"{nameof(lengthFrom)}: {lengthFrom}, " +
                $"{nameof(lengthTo)}: {lengthTo}, " +
                $"{nameof(widthLeft)}: {widthLeft}, " +
                $"{nameof(widthRight)}: {widthRight}, " +
                $"{nameof(propertyType)}: {propertyType}, " +
                $"{nameof(trackBranchID)}: {trackBranchID}" +
                $")";
        }

    }
}
