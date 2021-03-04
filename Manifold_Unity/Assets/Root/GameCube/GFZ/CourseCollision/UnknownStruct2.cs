using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// Only available in Sand Ocean Lateral Shift
    /// GX: 6 instances, AX: 9 instances
    /// </summary>
    [Serializable]
    public struct UnknownStruct2 : IBinarySerializable
    {
        public int unk_0x00;
        public Vector3 unk_0x04;// position?
        public int unk_0x10;
        public int zero_0x14;
        public Vector3 unk_0x18; // scale? always 1,1,1

        public void Deserialize(BinaryReader reader)
        {
            reader.ReadX(ref unk_0x00);
            reader.ReadX(ref unk_0x04);
            reader.ReadX(ref unk_0x10);
            reader.ReadX(ref zero_0x14);
            reader.ReadX(ref unk_0x18);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(unk_0x00);
            writer.WriteX(unk_0x04);
            writer.WriteX(unk_0x10);
            writer.WriteX(zero_0x14);
            writer.WriteX(unk_0x18);
        }
    }
}
