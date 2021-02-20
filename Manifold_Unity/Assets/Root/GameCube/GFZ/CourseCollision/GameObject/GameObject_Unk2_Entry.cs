using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class SkeletalProperties : IBinarySerializable, IBinaryAddressable
    {

        #region MEMBERS

        [SerializeField, Hex] long startAddress;
        [SerializeField, Hex] long endAddress;

        /// <summary>
        /// Values: 0, 3, 7, 10, 15, 20, 50, 60
        /// </summary>
        public uint unk_0x00;
        public EnumFlags32 unk_0x04;
        public EnumFlags32 unk_0x08;
        public uint zero_0x0C;
        public uint zero_0x10;
        public uint zero_0x14;
        public uint zero_0x18;

        #endregion

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

        #region METHODS

        public void Deserialize(BinaryReader reader)
        {
            startAddress = reader.BaseStream.Position;

            reader.ReadX(ref unk_0x00);
            reader.ReadX(ref unk_0x04);
            reader.ReadX(ref unk_0x08);
            reader.ReadX(ref zero_0x0C);
            reader.ReadX(ref zero_0x10);
            reader.ReadX(ref zero_0x14);
            reader.ReadX(ref zero_0x18);

            endAddress = reader.BaseStream.Position;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(unk_0x00);
            writer.WriteX(unk_0x04);
            writer.WriteX(unk_0x08);
            writer.WriteX(zero_0x0C);
            writer.WriteX(zero_0x10);
            writer.WriteX(zero_0x14);
            writer.WriteX(zero_0x18);
        }

        #endregion

    }
}