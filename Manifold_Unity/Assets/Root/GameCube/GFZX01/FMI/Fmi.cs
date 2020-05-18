using StarkTools.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZX01.FMI
{
    [Serializable]
    public class Fmi : IBinarySerializable, IBinaryAddressable
    {
        [SerializeField, Hex] long startAddress;
        [SerializeField, Hex] long endAddress;

        public uint unk_0x00;
        public uint unk_0x04;
        public int animationCount;
        public int exhaustCount;
        public uint unk_0x10;
        public float unk_0x14;
        public uint unk_0x18;
        public float unk_0x1C;
        public uint unk_0x20;
        public ExhaustParticle[] particles;
        public ExhaustAnimation[] animations;

        #region PROPERTIES

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
                reader.ReadX(ref unk_0x04);
                reader.ReadX(ref animationCount);
                reader.ReadX(ref exhaustCount);
                reader.ReadX(ref unk_0x10);
                reader.ReadX(ref unk_0x14);
                reader.ReadX(ref unk_0x18);
                reader.ReadX(ref unk_0x1C);
                reader.ReadX(ref unk_0x20);
                reader.ReadX(ref particles, animationCount, true);
                reader.ReadX(ref animations, exhaustCount, true);
            }
            endAddress = reader.BaseStream.Position;
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}