using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public struct ColiUnknownStruct1 : IBinarySerializable
    {
        public float unk_0x00;
        public float unk_0x04;
        public float unk_0x08;
        public float unk_0x0C;
        public int unk_0x10;
        public int unk_0x14;

        public void Deserialize(BinaryReader reader)
        {
            reader.ReadX(ref unk_0x00);
            reader.ReadX(ref unk_0x04);
            reader.ReadX(ref unk_0x08);
            reader.ReadX(ref unk_0x0C);
            reader.ReadX(ref unk_0x10);
            reader.ReadX(ref unk_0x14);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(unk_0x00);
            writer.WriteX(unk_0x04);
            writer.WriteX(unk_0x08);
            writer.WriteX(unk_0x0C);
            writer.WriteX(unk_0x10);
            writer.WriteX(unk_0x14);
        }
    }
}
