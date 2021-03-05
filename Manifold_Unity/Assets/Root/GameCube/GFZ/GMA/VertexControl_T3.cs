using Manifold.IO;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace GameCube.GFZ.GMA
{
    [Serializable]
    public class VertexControl_T3 : IBinarySerializable, IBinaryAddressableRange
    {
        // NOTE
        // Total count appears to be something like
        // VertexControlHeader.VertexCount / (countIndividual / matrices.Length)
        // Because this (appears) to consistently produce whole numbers

        public const int kFifoPaddingSize = 12;

        [Header("Vtx Ctrl T3")]
        [SerializeField] AddressRange addressRange;
        [SerializeField, Hex(8)] int arrayCount;
        [SerializeField, Hex(8)] int addressCount;
        [SerializeField, Hex(8)] long size;

        [Space]
        [SerializeField]
        IntArray[] intArrays;

        byte[] fifoPadding;


        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                List<IntArray> temp = new List<IntArray>();

                arrayCount = addressCount = 0;
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
            }
            this.RecordEndAddress(reader);

            size = this.GetBinarySize();
        }

        public void Serialize(BinaryWriter writer)
        {
            foreach (var value in intArrays)
            {
                writer.WriteX(value);
            }
            writer.Align(GameCube.GX.GXUtility.GX_FIFO_ALIGN);
        }
    }
}
