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
    public struct TransformMatrixIndexes8 : IBinarySerializable
    {
        public const int kMatrixCount = 8;

        [SerializeField, Hex(2)]
        byte[] matrixIndexes;

        public void Deserialize(BinaryReader reader)
        {
            reader.ReadX(ref matrixIndexes, kMatrixCount);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(matrixIndexes, false);
        }

        public override string ToString()
        {
            var mtx0 = matrixIndexes[0];
            var mtx1 = matrixIndexes[1];
            var mtx2 = matrixIndexes[2];
            var mtx3 = matrixIndexes[3];
            var mtx4 = matrixIndexes[4];
            var mtx5 = matrixIndexes[5];
            var mtx6 = matrixIndexes[6];
            var mtx7 = matrixIndexes[7];
            return $"MTX IDX({mtx0}, {mtx1}, {mtx2}, {mtx3}, {mtx4}, {mtx5}, {mtx6}, {mtx7})";
        }
    }
}