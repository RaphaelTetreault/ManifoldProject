using Manifold.IO;
using System;
using System.IO;


namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class TrackCornerTopology : IBinarySerializable, IBinaryAddressableRange
    {

        [UnityEngine.SerializeField]
        private AddressRange addressRange;

        //public float unk_0x00; // range: -1.000 to +1.000
        //public float unk_0x04; // range: -0.110 to +1.000
        //public float unk_0x08; // range: -1.000 to +1.000
        //public float unk_0x0C; // range: -2090. to +1417. // rotation...?

        //public float unk_0x10; // range: -0.600 to +1.000
        //public float unk_0x14; // range: ~0.000 to +1.000
        //public float unk_0x18; // range: -0.999 to +0.999
        //public float unk_0x1C; // range: -650.0 to +350.0 // rotation...?

        //public float unk_0x20; // range: -1.000 to +1.000
        //public float unk_0x24; // range: -0.410 to +0.750
        //public float unk_0x28; // range: -1.000 to +1.000
        //public float unk_0x2C; // range: -1976. to +3068. // rotation...?

        // Is transform 3x4?
        //public float3 xNormal;
        //public float x;
        //public float3 yNormal;
        //public float y;
        //public float3 zNormal;
        //public float z;
        //public Matrix4x4 matrix;

        /// <summary>
        /// If this doesn't work right, use TransformMatrix3X4Col
        /// </summary>
        public TransformMatrix3x4 matrix3x4;
        public float unkRotation; // range: -90.00 to +180.0 // rotation
        private byte const_0x34; // Const: 0x02
        private byte zero_0x35; // Const: 0x00
        public TrackPerimeterOptions perimeterOptions;
        private byte zero_0x37; // Const: 0x00

        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }


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
            //writer.WriteX();
            throw new NotImplementedException();
        }

    }
}