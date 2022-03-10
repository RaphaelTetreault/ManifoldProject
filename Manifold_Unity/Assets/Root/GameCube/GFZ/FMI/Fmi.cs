using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.FMI
{
    [Serializable]
    public class Fmi :
        IBinaryAddressable,
        IBinarySerializable,
        IFileType
    {
        // CONSTANTS
        private const long kParticlesAbsPtr = 0x0044;
        private const long kAnimationAbsPtr = 0x0208;
        private const long kNameAbsPtr = 0x02A0;

        // FIELDS
        public byte unk_0x00;
        public byte unk_0x01;
        public byte animationCount;
        public byte exhaustCount;
        public byte unk_0x04;
        public float unk_0x05;
        public byte unk_0x06;
        public float unk_0x07;
        public ushort unk_0x08;
        // REFERENCES
        public ExhaustParticle[] particles;
        public ExhaustAnimation[] animations;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public string FileName { get; set; }
        public string FileExtension => ".fmi";


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref unk_0x00);
                reader.ReadX(ref unk_0x01);
                reader.ReadX(ref animationCount);
                reader.ReadX(ref exhaustCount);
                reader.ReadX(ref unk_0x04);
                reader.ReadX(ref unk_0x05);
                reader.ReadX(ref unk_0x06);
                reader.ReadX(ref unk_0x07);
                reader.ReadX(ref unk_0x08);
            }
            this.RecordEndAddress(reader);
            {
                reader.BaseStream.Seek(kParticlesAbsPtr, SeekOrigin.Begin);
                reader.ReadX(ref particles, animationCount);

                reader.BaseStream.Seek(kAnimationAbsPtr, SeekOrigin.Begin);
                reader.ReadX(ref animations, exhaustCount);

                // TODO: read names
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }


    }
}