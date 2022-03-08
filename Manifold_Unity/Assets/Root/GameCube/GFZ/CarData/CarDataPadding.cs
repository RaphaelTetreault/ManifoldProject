using Manifold;
using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CarData
{
    [Serializable]
    public struct CarDataPadding :
        IBinarySerializable
    {
        public const int PaddingCount = 0x26;

        public byte[] Padding => new byte[]
        {
            0x00, 0x30, 0x31, 0x32,
            0x33, 0x34, 0x35, 0x36,
            0x37, 0x38, 0x39, 0x30,
            0x31, 0x32, 0x33, 0x34,
            0x35, 0x36, 0x37, 0x38,
            0x39, 0x30, 0x31, 0x32,
            0x33, 0x34, 0x35, 0x36,
            0x37, 0x38, 0x39, 0x30,
            0x31, 0x32, 0x33, 0x34,
            0x35, 0x00,
        };

        public void Deserialize(BinaryReader reader)
        {
            // Read what should be padding
            var buffer = new byte[0];
            reader.ReadX(ref buffer, PaddingCount);

            // Assert padding
            var expectedValue = Padding;
            for (int i = 0; i < PaddingCount; i++)
            {
                Assert.IsTrue(buffer[i] == expectedValue[i]);
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Padding);
        }
    }
}