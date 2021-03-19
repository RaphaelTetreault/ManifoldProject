using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.GMA
{
    [Serializable]
    public class GcmfTransformMatrices : IBinarySerializable, IBinaryAddressableRange
    {

        #region FIELDS


        [Header("Gcmf Transform Matrices")]
        [SerializeField]
        private AddressRange addressRange;
        [SerializeField, Hex]
        private int matrixCount;
        private byte[] fifoPadding;

        [SerializeField, Space]
        TransformMatrix3x4[] matrices;


        #endregion

        #region CONSTRUCTORS


        public GcmfTransformMatrices() { }

        public GcmfTransformMatrices(int matrixCount)
        {
            SetMatrixCount(matrixCount);
        }


        #endregion

        #region PROPERTIES


        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        public TransformMatrix3x4[] Matrices
            => matrices;

        public int MatrixCount
        {
            get
            {
                return (matrices == null) ? 0 : matrices.Length;
            }
        }

        public int FifoPaddingSize
        {
            get
            {
                // Since structure is 0x30 each, we have padding when count is uneven
                return ((MatrixCount % 2) > 0) ? 0x10 : 0x00;
            }
        }


        #endregion

        #region METHODS


        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);

            if (matrices != null && matrices.Length > 0)
            {
                reader.ReadX(ref matrices, MatrixCount, false);
                reader.ReadX(ref fifoPadding, FifoPaddingSize);
                foreach (var padByte in fifoPadding)
                    Assert.IsTrue(padByte == 0);
            }

            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            foreach (var matrix in matrices)
            {
                writer.WriteX(matrix);
            }

            for (int i = 0; i < FifoPaddingSize; i++)
                writer.WriteX((byte)0x00);
        }

        public void SetMatrixCount(int matrixCount)
        {
            matrices = new TransformMatrix3x4[matrixCount];
            this.matrixCount = matrixCount;
        }


        #endregion

    }
}