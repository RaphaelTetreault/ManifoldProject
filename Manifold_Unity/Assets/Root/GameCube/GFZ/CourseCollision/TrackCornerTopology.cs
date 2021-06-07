using Manifold.IO;
using System;
using System.IO;


namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class TrackCornerTopology :
        IBinaryAddressable,
        IBinarySerializable
    {
        // METADATA
        [UnityEngine.SerializeField]
        private AddressRange addressRange;

        // FIELDS
        public TransformMatrix3x4 matrix3x4; // never null
        public float unkRotation; // range: -90.00 to +180.0 // rotation
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
                reader.ReadX(ref unkRotation);
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
                Assert.IsTrue(const_0x34 == 0x02);
                Assert.IsTrue(zero_0x35 == 0x00);
                Assert.IsTrue(zero_0x37 == 0x00);
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(matrix3x4);
                writer.WriteX(unkRotation);
                writer.WriteX(const_0x34);
                writer.WriteX(zero_0x35);
                writer.WriteX(perimeterOptions);
                writer.WriteX(zero_0x37);
            }
            this.RecordEndAddress(writer);
        }
    }
}