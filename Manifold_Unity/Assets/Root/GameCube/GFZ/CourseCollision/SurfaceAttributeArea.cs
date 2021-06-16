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
    public class SurfaceAttributeArea :
        IBinaryAddressable,
        IBinarySerializable
    {
        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;

        // FIELDS
        // Default values represent final node.
        public float lengthFrom = -1f;
        public float lengthTo = -1f;
        public float widthLeft = 0;
        public float widthRight = 0;
        public SurfaceAttribute surfaceAttribute = SurfaceAttribute.TerminateCode;
        public byte trackBranchID = 0;
        public ushort zero_0x12;


        // PROPERTIES
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        public static SurfaceAttributeArea Terminator()
        {
            return new SurfaceAttributeArea()
            {
                lengthFrom = -1f,
                lengthTo = -1f,
                widthLeft = 0,
                widthRight = 0,
                surfaceAttribute = SurfaceAttribute.TerminateCode,
                trackBranchID = 0,
                zero_0x12 = 0,
            };
        }

        public static SurfaceAttributeArea[] DefaultArray()
        {
            return new SurfaceAttributeArea[]
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
                reader.ReadX(ref surfaceAttribute);
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
                writer.WriteX(surfaceAttribute);
                writer.WriteX(trackBranchID);
                writer.WriteX(zero_0x12);
            }
            this.RecordEndAddress(writer);
        }

        public override string ToString()
        {
            return 
                $"{nameof(SurfaceAttributeArea)}(" +
                $"{nameof(lengthFrom)}: {lengthFrom}, " +
                $"{nameof(lengthTo)}: {lengthTo}, " +
                $"{nameof(widthLeft)}: {widthLeft}, " +
                $"{nameof(widthRight)}: {widthRight}, " +
                $"{nameof(surfaceAttribute)}: {surfaceAttribute}, " +
                $"{nameof(trackBranchID)}: {trackBranchID}" +
                $")";
        }

    }
}
