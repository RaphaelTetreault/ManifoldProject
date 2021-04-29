using Manifold.IO;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class TopologyExtra : IBinarySerializable, IBinaryAddressableRange
    {

        [SerializeField]
        private AddressRange addressRange;

        public float unk_0x00; // range: -1.000 to +1.000
        public float unk_0x04; // range: -0.110 to +1.000
        public float unk_0x08; // range: -1.000 to +1.000
        public float unk_0x0C; // range: -2090. to +1417. // rotation...?

        public float unk_0x10; // range: -0.600 to +1.000
        public float unk_0x14; // range: ~0.000 to +1.000
        public float unk_0x18; // range: -0.999 to +0.999
        public float unk_0x1C; // range: -650.0 to +350.0 // rotation...?

        public float unk_0x20; // range: -1.000 to +1.000
        public float unk_0x24; // range: -0.410 to +0.750
        public float unk_0x28; // range: -1.000 to +1.000
        public float unk_0x2C; // range: -1976. to +3068. // rotation...?

        public float unk_0x30; // range: -90.00 to +180.0 // rotation
        public byte unk_0x34; // Const: 0x02
        public byte zero_0x35; // Const: 0x00
        public byte unk_0x36; // 20, 28, 44 (!) same as "has children" flag
        public byte zero_0x37; // Const: 0x00

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
                reader.ReadX(ref unk_0x14);
                reader.ReadX(ref unk_0x18);
                reader.ReadX(ref unk_0x1C);
                reader.ReadX(ref unk_0x20);
                reader.ReadX(ref unk_0x24);
                reader.ReadX(ref unk_0x28);
                reader.ReadX(ref unk_0x2C);
                reader.ReadX(ref unk_0x30);
                reader.ReadX(ref unk_0x34);
                reader.ReadX(ref zero_0x35);
                reader.ReadX(ref unk_0x36);
                reader.ReadX(ref zero_0x37);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            //writer.WriteX();
            throw new NotImplementedException();
        }

    }
}