using GameCube.GX;
using StarkTools.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZX01.GMA
{
    [Serializable]
    public class VertexControl_T4 : IBinarySerializable, IBinaryAddressable
    {
        [Header("Vtx Ctrl T4")]
        [SerializeField, Hex(8)] long startAddress;
        [SerializeField, Hex(8)] long endAddress;
        [SerializeField, Hex(8)] int matrixCount;

        [SerializeField]
        ushort[] matrixIndexes;

        byte[] fifoPadding;


        public VertexControl_T4() { }

        public VertexControl_T4(int matrixCount)
        {
            this.matrixCount = matrixCount;
        }


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

            reader.ReadX(ref matrixIndexes, matrixCount);
            reader.Align(GxUtility.GX_FIFO_ALIGN);

            EndAddress = reader.BaseStream.Position;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(matrixIndexes, false);
            writer.Align(GxUtility.GX_FIFO_ALIGN);
        }
    }
}
