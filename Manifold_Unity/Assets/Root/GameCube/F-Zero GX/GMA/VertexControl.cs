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
    public class VertexControlHeader : IBinarySerializable, IBinaryAddressable
    {
        public const int kFifoPaddingSize = 12;

        [Header("Vertex Control Header")]
        [SerializeField, Hex] long startAddress;
        [SerializeField, Hex] long endAddress;
        [Space]

        #region MEMBERS

        [SerializeField, Hex("00", 8)]
        int vertexCount;

        [SerializeField, Hex("04", 8)]
        int vertexControlT1RelPtr;

        [SerializeField, Hex("08", 8)]
        int vertexControlT2RelPtr;

        [SerializeField, Hex("0C", 8)]
        int vertexControlT3RelPtr;

        [SerializeField, Hex("10", 8)]
        int vertexControlT4RelPtr;

        byte[] fifoPadding;

        #endregion

        #region PROPERTIES

        public int VertexCount => vertexCount;

        public int VertexControlT1RelPtr => vertexControlT1RelPtr;

        public int VertexControlT2RelPtr => vertexControlT2RelPtr;

        public int VertexControlT3RelPtr => vertexControlT3RelPtr;

        public int VertexControlT4RelPtr => vertexControlT4RelPtr;

        #endregion

        #region IBinaryAddressable

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

        #region IBinarySerializable

        public void Deserialize(BinaryReader reader)
        {
            StartAddress = reader.BaseStream.Position;

            reader.ReadX(ref vertexCount);
            reader.ReadX(ref vertexControlT1RelPtr);
            reader.ReadX(ref vertexControlT2RelPtr);
            reader.ReadX(ref vertexControlT3RelPtr);
            reader.ReadX(ref vertexControlT4RelPtr);
            reader.ReadX(ref fifoPadding, kFifoPaddingSize);

            EndAddress = reader.BaseStream.Position;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(vertexCount);
            writer.WriteX(vertexControlT1RelPtr);
            writer.WriteX(vertexControlT2RelPtr);
            writer.WriteX(vertexControlT3RelPtr);
            writer.WriteX(vertexControlT4RelPtr);
            var align = writer.Align(GameCube.GX.GxUtility.GX_FIFO_ALIGN);
            Assert.IsTrue(align == kFifoPaddingSize);
        }

        #endregion

    }
}
