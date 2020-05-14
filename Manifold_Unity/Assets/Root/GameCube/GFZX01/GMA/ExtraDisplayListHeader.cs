using StarkTools.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZX01.GMA
{
    [Serializable]
    public class ExtraDisplayListHeader : IBinarySerializable, IBinaryAddressable
    {
        public const int kFifoPaddingSize = 16;

        [Header("Stitch Data")]
        [SerializeField, Hex] long startAddress;
        [SerializeField, Hex] long endAddress;
        [Space]

        #region MEMBERS

        [SerializeField, Hex("00", 8)]
        int unk_0x00;

        [SerializeField, Hex("04", 8)]
        int unk_0x04;

        [SerializeField, Hex("08", 8)]
        int vertexSize0; // MAT?

        [SerializeField, Hex("0C", 8)]
        int vertexSize1; // TL MAT?

        byte[] fifoPadding;

        #endregion

        #region PROPERTIES

        public int VertexSize0 => vertexSize0;
        public int VertexSize1 => vertexSize1;

        #endregion

        // Metadata

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

        public void Deserialize(BinaryReader reader)
        {
            StartAddress = reader.BaseStream.Position;

            reader.ReadX(ref unk_0x00);
            reader.ReadX(ref unk_0x04);
            reader.ReadX(ref vertexSize0);
            reader.ReadX(ref vertexSize1);
            reader.ReadX(ref fifoPadding, kFifoPaddingSize);

            EndAddress = reader.BaseStream.Position;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(unk_0x00);
            writer.WriteX(unk_0x04);
            writer.WriteX(vertexSize0);
            writer.WriteX(vertexSize1);

            for (int i = 0; i < kFifoPaddingSize; i++)
                writer.WriteX((byte)0x00);
        }
    }
}