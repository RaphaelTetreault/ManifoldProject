﻿using StarkTools.IO;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;

namespace GameCube.GFZX01.GMA
{
    [Serializable]
    public class GcmfTransformMatrices : IBinarySerializable, IBinaryAddressable
    {
        [Header("Gcmf Transform Matrices")]
        [SerializeField, Hex] long startAddress;
        [SerializeField, Hex] long endAddress;
        [SerializeField, Hex] int matrixCount;
        byte[] fifoPadding;

        #region MEMBERS

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
            this.matrixCount = matrixCount;
        }
    }
}