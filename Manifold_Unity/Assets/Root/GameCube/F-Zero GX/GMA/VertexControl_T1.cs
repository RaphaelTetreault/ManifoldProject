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
    public class VertexControl_T1 : IBinarySerializable, IBinaryAddressable
    {
        [Header("Vtx Ctrl T1")]
        [SerializeField, Hex(8)] long startAddress;
        [SerializeField, Hex(8)] long endAddress;

        // This is a guess
        [SerializeField]
        Vector3 position;

        [SerializeField]
        Vector3 normal;

        [SerializeField]
        uint unk_0x18;

        [SerializeField]
        float unk_0x1C;

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

            reader.ReadX(ref position);
            reader.ReadX(ref normal);
            reader.ReadX(ref unk_0x18);
            reader.ReadX(ref unk_0x1C);

            EndAddress = reader.BaseStream.Position;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(position);
            writer.WriteX(normal);
            writer.WriteX(unk_0x18);
            writer.WriteX(unk_0x1C);
        }
    }
}