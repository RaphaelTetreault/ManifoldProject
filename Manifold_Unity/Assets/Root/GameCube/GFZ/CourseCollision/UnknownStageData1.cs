using Manifold.IO;
using System;
using System.IO;
using Unity.Mathematics;

// NOTE: When unk_0x0C is (0, 0, 0), then there is no pointer in ColiCourse header at 0x80
// Review notes in UnknownStageData2

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class UnknownStageData1 :
        IBinaryAddressable,
        IBinarySerializable
    {
        // METADATA
        [UnityEngine.SerializeField]
        private AddressRange addressRange;

        // FIELDS
        public UnknownStageData1Enum unk_0x00; // 0x##000000 - looks like highest byte flags?
        // is the below not "unknown coli struct 1"? (plane?)
        public UnknownFloatPair unk_0x04;
        public float3 unk_0x0C; // Looks like scale? Maybe not a vector.
        public float unk_0x18;
        private int zero_0x1C;
        private int zero_0x20;


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
                reader.ReadX(ref unk_0x00);
                reader.ReadX(ref unk_0x04, true);
                reader.ReadX(ref unk_0x0C);
                reader.ReadX(ref unk_0x18);
                reader.ReadX(ref zero_0x1C);
                reader.ReadX(ref zero_0x20);
            }
            this.RecordEndAddress(reader);
            {
                Assert.IsTrue(zero_0x1C == 0);
                Assert.IsTrue(zero_0x20 == 0);
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            {
                Assert.IsTrue(zero_0x1C == 0);
                Assert.IsTrue(zero_0x20 == 0);
            }
            this.RecordStartAddress(writer);
            {
               writer.WriteX(unk_0x00);
               writer.WriteX(unk_0x04);
               writer.WriteX(unk_0x0C);
               writer.WriteX(unk_0x18);
               writer.WriteX(zero_0x1C);
               writer.WriteX(zero_0x20);
            }
            this.RecordEndAddress(writer);
        }

    }
}
