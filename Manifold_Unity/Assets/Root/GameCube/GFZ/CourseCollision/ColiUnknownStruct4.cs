using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class ColiUnknownStruct4 : IBinarySerializable, IBinaryAddressableRange
    {
        public const int elementCount = 6;

        [SerializeField]
        private AddressRange addressRange;

        public int unk_0x00;
        public float unk_0x04; // Unknown Struct 7?
        public float unk_0x08; // Unknown Struct 7?
        public int unk_0x0C;
        public int unk_0x10;


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
                reader.ReadX(ref unk_0x04);
                reader.ReadX(ref unk_0x08);
                reader.ReadX(ref unk_0x0C);
                reader.ReadX(ref unk_0x10);
            }
            this.RecordEndAddress(reader);
            {
                //Debug.Log($"{unk_0x00}, {unk_0x04:0.00}, {unk_0x08:0.00}, {unk_0x0C}, {unk_0x10}");

                // Raph: 2021/03/04: assertions held true for GX JP and AX 
                UnityEngine.Assertions.Assert.IsTrue(unk_0x00 == 2);
                UnityEngine.Assertions.Assert.IsTrue(unk_0x0C == 0);
                UnityEngine.Assertions.Assert.IsTrue(unk_0x10 == 0);
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

    }
}
