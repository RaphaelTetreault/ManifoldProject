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
        public const int kFifoPaddingSize = 12;

        [Header("Vtx Ctrl T3")]
        [SerializeField, Hex(8)] long startAddress;
        [SerializeField, Hex(8)] long endAddress;
        [SerializeField, Hex(8)] int count;

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

            //var value = 0;
            //while (!reader.EndOfStream())
            //{
            //    reader.ReadX(ref count);
            //    if (count == GcmfProperties.kGCMF)
            //        break;

            //    for (int i = 0; i < count; i++)
            //    {
            //        reader.ReadX(ref value);
            //        if (value == GcmfProperties.kGCMF)
            //            break;
            //    }
            //}

            //reader.BaseStream.Position -= 4;

            //reader.ReadX(ref unk_0x00);
            //reader.ReadX(ref fifoPadding, kFifoPaddingSize);

            EndAddress = reader.BaseStream.Position;
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
