using System;
using System.IO;
using Manifold.IO;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public struct UnknownObjectBitfield :
        IBinarySerializable
    {
        [NumFormat(numDigits: 8, numBase: 16)]
        public uint data32;
        [NumFormat(numDigits: 4, numBase: 16)]
        public ushort data16a;
        [NumFormat(numDigits: 4, numBase: 16)]
        public ushort data16b;
        [NumFormat(numDigits: 2, numBase: 16)]
        public byte data8a;
        [NumFormat(numDigits: 2, numBase: 16)]
        public byte data8b;
        [NumFormat(numDigits: 2, numBase: 16)]
        public byte data8c;
        [NumFormat(numDigits: 2, numBase: 16)]
        public byte data8d;

        public void Deserialize(BinaryReader reader)
        {
            reader.ReadX(ref data32);

            data16a = (ushort)((data32 >> 8 * 2) & 0xFFFF);
            data16b = (ushort)((data32 >> 8 * 0) & 0xFFFF);

            data8a = (byte)((data32 >> 8 * 3) & 0xFF);
            data8b = (byte)((data32 >> 8 * 2) & 0xFF);
            data8c = (byte)((data32 >> 8 * 1) & 0xFF);
            data8d = (byte)((data32 >> 8 * 0) & 0xFF);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(data32);
        }

        public override string ToString()
        {
            return $"{data32:x8}";
        }

    }
}
