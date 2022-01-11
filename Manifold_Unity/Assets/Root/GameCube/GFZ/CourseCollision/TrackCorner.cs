using Manifold.IO;
using System;
using System.IO;


namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// This special metadata defines a 90-degree angle as part of the track geometry.
    /// It is assumed this is necessary for AI rather than anything else.
    /// </summary>
    [Serializable]
    public class TrackCorner :
        IBinaryAddressable,
        IBinarySerializable
    {
        // METADATA
        [UnityEngine.SerializeField]
        private AddressRange addressRange;

        // FIELDS
        public TransformMatrix3x4 matrix3x4; // never null
        public float width;
        private byte const_0x34; // Const: 0x02
        private byte zero_0x35; // Const: 0x00
        public TrackPerimeterOptions perimeterOptions;
        private byte zero_0x37; // Const: 0x00


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
                reader.ReadX(ref matrix3x4, true);
                reader.ReadX(ref width);
                reader.ReadX(ref const_0x34);
                reader.ReadX(ref zero_0x35);
                reader.ReadX(ref perimeterOptions);
                reader.ReadX(ref zero_0x37);
            }
            this.RecordEndAddress(reader);
            {
                Assert.IsTrue(const_0x34 == 0x02);
                Assert.IsTrue(zero_0x35 == 0x00);
                Assert.IsTrue(zero_0x37 == 0x00);
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            {
                Assert.IsTrue(matrix3x4 != null);
                Assert.IsTrue(const_0x34 == 0x02, $"{nameof(const_0x34)} is not 0x02! Is: {const_0x34}");
                Assert.IsTrue(zero_0x35 == 0x00);
                Assert.IsTrue(zero_0x37 == 0x00);
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(matrix3x4);
                writer.WriteX(width);
                writer.WriteX(const_0x34);
                writer.WriteX(zero_0x35);
                writer.WriteX(perimeterOptions);
                writer.WriteX(zero_0x37);
            }
            this.RecordEndAddress(writer);
        }
    }
}