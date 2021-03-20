using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

// NOTE: When unk_0x0C is (0, 0, 0), then there is no pointer in ColiCourse header at 0x80
// Review notes in UnknownStageData2

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class UnknownStageData1 : IBinarySerializable, IBinaryAddressableRange
    {
        [SerializeField]
        private AddressRange addressRange;

        public UnknownStageData1Enum unk_0x00; // 0x##000000 - looks like highest byte flags? Big endian?
        public UnknownFloatPair unk_0x04;
        public Vector3 unk_0x0C; // Looks like scale? Maybe not a vector.
        public float unk_0x18;
        private int zero_0x1C;
        private int zero_0x20;


        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }


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
            throw new NotImplementedException();
        }

    }
}
