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
    public struct ColiUnknownStruct2 : IBinarySerializable
    {
        public int unk_0x00;
        public Vector3 position; // position?
        public int unk_0x10; // ShortRotation3!? 
        public int zero_0x14; // This would mean Z rotation isn't used...?
        public Vector3 scale; // scale? always 1,1,1

        public void Deserialize(BinaryReader reader)
        {
            reader.ReadX(ref unk_0x00);
            reader.ReadX(ref position);
            reader.ReadX(ref unk_0x10);
            reader.ReadX(ref zero_0x14);
            reader.ReadX(ref scale);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(unk_0x00);
            writer.WriteX(position);
            writer.WriteX(unk_0x10);
            writer.WriteX(zero_0x14);
            writer.WriteX(scale);
        }
    }
}
