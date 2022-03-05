using Manifold;
using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.Camera
{
    [Serializable]
    public class CameraPan :
        IBinarySerializable,
        IBinaryAddressable
    {
        // CONSTANTS
        public const int kStructureSize = 0x54;
        public const int kZeroes0x08 = 4;

        // METADATA
        private AddressRange addressRange;

        // FIELDS
        public int frameCount;
        public float lerpSpeed;
        public byte[] zeroes0x08;
        public CameraPanTarget from;
        public CameraPanTarget to;


        // PROPERTIES
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        public int FrameCount
        {
            get => frameCount;
            set => frameCount = value;
        }

        public float LerpSpeed
        {
            get => LerpSpeed;
            set => LerpSpeed = value;
        }

        public byte[] Zeroes0x08
        {
            get => Zeroes0x08;
            set => Zeroes0x08 = value;
        }

        public CameraPanTarget From
        {
            get => From;
            set => From = value;
        }

        public CameraPanTarget To
        {
            get => To;
            set => To = value;
        }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref frameCount);
                reader.ReadX(ref lerpSpeed);
                reader.ReadX(ref zeroes0x08, kZeroes0x08);
                reader.ReadX(ref from);
                reader.ReadX(ref to);
            }
            this.RecordEndAddress(reader);

            // Assertions
            foreach (var @byte in Zeroes0x08)
                Assert.IsTrue(@byte == 0);
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                writer.WriteX(frameCount);
                writer.WriteX(lerpSpeed);
                writer.WriteX(new byte[kZeroes0x08]);
                writer.WriteX(from);
                writer.WriteX(to);
            }
            this.RecordEndAddress(writer);
        }
    }
}
