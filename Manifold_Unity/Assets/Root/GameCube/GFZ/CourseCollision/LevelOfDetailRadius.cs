using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using Manifold.IO;

namespace GameCube.GFZ.CourseCollision
{
    [System.Serializable]
    public struct LevelOfDetailRadius : IBinarySerializable
    {
        public uint data32;
        public ushort data16a;
        public ushort data16b;
        public byte data8a;
        public byte data8b;
        public byte data8c;
        public byte data8d;
        public void Deserialize(BinaryReader reader)
        {
            reader.ReadX(ref data32);

            data16a = (ushort)((data32 >> 8*2) & 0xFFFF);
            data16b = (ushort)((data32 >> 8*0) & 0xFFFF);

            data8a = (byte)((data32 >> 8*3) & 0xFFFF);
            data8a = (byte)((data32 >> 8*2) & 0xFFFF);
            data8a = (byte)((data32 >> 8*1) & 0xFFFF);
            data8a = (byte)((data32 >> 8*0) & 0xFFFF);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(data32);
        }

    }
}
