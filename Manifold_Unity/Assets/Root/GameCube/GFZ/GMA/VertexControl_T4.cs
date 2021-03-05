using GameCube.GX;
using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.GMA
{
    [Serializable]
    public class VertexControl_T4 : IBinarySerializable, IBinaryAddressableRange
    {
        [Header("Vtx Ctrl T4")]
        [SerializeField]
        private AddressRange addressRange;

        [SerializeField, Hex(8)] int matrixCount;

        [SerializeField]
        ushort[] matrixIndexes;

        byte[] fifoPadding;


        public VertexControl_T4() { }

        public VertexControl_T4(int matrixCount)
        {
            this.matrixCount = matrixCount;
        }


        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref matrixIndexes, matrixCount);
                reader.Align(GXUtility.GX_FIFO_ALIGN);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(matrixIndexes, false);
            writer.Align(GXUtility.GX_FIFO_ALIGN);
        }
    }
}
