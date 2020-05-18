using StarkTools.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZX01.FMI
{
    [Serializable]
    public class Fmi : IBinarySerializable, IBinaryAddressable, IFile
    {
        private const long kParticlesAbsPtr = 0x0044;
        private const long kAnimationAbsPtr = 0x0208;
        private const long kNameAbsPtr = 0x02A0;

        [SerializeField] string fileName;
        [SerializeField, Hex] long startAddress;
        [SerializeField, Hex] long endAddress;

        public byte unk_0x00;
        public byte unk_0x01;
        public byte animationCount;
        public byte exhaustCount;
        public byte unk_0x04;
        public float unk_0x05;
        public byte unk_0x06;
        public float unk_0x07;
        public ushort unk_0x08;

        public ExhaustParticle[] particles;
        public ExhaustAnimation[] animations;

        #region PROPERTIES

        public string FileName
        {
            get => fileName;
            set => fileName = value;
        }

        public long StartAddress
        {
            get => startAddress;
            set => startAddress = value;
        }

        public long EndAddress
        {
            get => endAddress;
            set => endAddress = value;
        }

        #endregion

        public void Deserialize(BinaryReader reader)
        {
            startAddress = reader.BaseStream.Position;
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
            endAddress = reader.BaseStream.Position;
            {
                reader.BaseStream.Seek(kParticlesAbsPtr, SeekOrigin.Begin);
                reader.ReadX(ref particles, animationCount, true);

                reader.BaseStream.Seek(kAnimationAbsPtr, SeekOrigin.Begin);
                reader.ReadX(ref animations, exhaustCount, true);

                // TODO: read names
            }
            reader.BaseStream.Seek(endAddress, SeekOrigin.Begin);
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}