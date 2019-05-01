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
    public struct IntArray : IBinarySerializable
    {
        public int count;
        public int[] addresses;

        public void Deserialize(BinaryReader reader)
        {
            reader.ReadX(ref count);
            reader.ReadX(ref addresses, count);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(count);
            writer.WriteX(addresses, false);
        }
    }

    [Serializable]
    public class VertexControl_T3 : IBinarySerializable, IBinaryAddressable
    {
        // NOTE
        // Total count appears to be something like
        // VertexControlHeader.VertexCount / (countIndividual / matrices.Length)
        // Because this (appears) to consistently produce whole numbers

        public const int kFifoPaddingSize = 12;

        [Header("Vtx Ctrl T3")]
        [SerializeField, Hex(8)] long startAddress;
        [SerializeField, Hex(8)] long endAddress;
        [SerializeField, Hex(8)] int arrayCount;
        [SerializeField, Hex(8)] int addressCount;
        [SerializeField, Hex(8)] long size;

        [Space]
        [SerializeField]
        IntArray[] intArrays;

        byte[] fifoPadding;


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

            List<IntArray> temp = new List<IntArray>();

            arrayCount = addressCount = 0;
            var value = 0;
            while (!reader.EndOfStream())
            {
                var numEntries = 0;
                reader.ReadX(ref numEntries);
                reader.BaseStream.Position -= 4;

                if (numEntries > 0x30 || numEntries < 0)
                    break;

                var intArray = new IntArray();
                reader.ReadX(ref intArray, false);
                temp.Add(intArray);

                arrayCount++;
                addressCount += numEntries;
            }

            intArrays = temp.ToArray();

            EndAddress = reader.BaseStream.Position;
            size = endAddress - startAddress;
        }

        public void Serialize(BinaryWriter writer)
        {
            foreach (var value in intArrays)
            {
                writer.WriteX(value);
            }
            writer.Align(GameCube.GX.GxUtility.GX_FIFO_ALIGN);
        }
    }
}
