using StarkTools.IO;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace GameCube.FZeroGX.GMA
{
    [Serializable]
    public class TransformMatrix3x4Collection : IBinarySerializable, IBinaryAddressable
    {
        [Header("Transform Matrix3x4 Collection")]
        [SerializeField, Hex] long startAddress;
        [SerializeField, Hex] long endAddress;
        byte[] fifoPadding;

        #region MEMBERS

        [SerializeField, Space]
        TransformMatrix3x4[] matrices;

        #endregion

        public TransformMatrix3x4Collection() { }

        public TransformMatrix3x4Collection(int matrixCount)
        {
            SetMatrixCount(matrixCount);
        }

        #region PROPERTIES

        public TransformMatrix3x4[] Matrices
            => matrices;

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


        public void Deserialize(BinaryReader reader)
        {
            startAddress = reader.BaseStream.Position;

            if (matrices != null && matrices.Length > 0)
            {
                reader.ReadX(ref matrices, MatrixCount, false);
                reader.ReadX(ref fifoPadding, FifoPaddingSize);
                foreach (var padByte in fifoPadding)
                    Assert.IsTrue(padByte == 0);
            }
            endAddress = reader.BaseStream.Position;
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
        }
    }
}