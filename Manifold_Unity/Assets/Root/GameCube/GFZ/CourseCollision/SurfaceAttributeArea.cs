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
        public float lengthFrom;
        public float lengthTo;
        public float widthLeft;
        public float widthRight;
        public SurfaceAttribute surfaceAttribute;
        public byte trackBranchID;
        public ushort zero_0x12;


        // PROPERTIES
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
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

    }
}
