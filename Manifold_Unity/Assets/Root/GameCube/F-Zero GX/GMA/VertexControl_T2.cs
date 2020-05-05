﻿using StarkTools.IO;
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
    public class VertexControl_T2 : IBinarySerializable, IBinaryAddressable
    {
        [Header("Vtx Ctrl T2")]
        [SerializeField, Hex(8)] long startAddress;
        [SerializeField, Hex(8)] long endAddress;

        //
        [SerializeField, LabelPrefix("00")]
        Vector3 position;

        /// <summary>
        /// Not always unit vector
        /// </summary>
        [SerializeField, LabelPrefix("0C")]
        Vector3 normal;

        [SerializeField, LabelPrefix("18")]
        Vector2 tex0uv;

        [SerializeField, LabelPrefix("20")]
        Vector2 tex1uv;

        [SerializeField, LabelPrefix("28")]
        Vector2 tex2uv;

        [SerializeField, LabelPrefix("30")]
        Color32 color;

        [SerializeField, Hex("34", 8)]
        uint unk_0x34;

        [SerializeField, Hex("38", 8)]
        uint unk_0x38;

        [SerializeField, Hex("3C", 8)]
        uint unk_0x3C;


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
            reader.ReadX(ref tex0uv);
            reader.ReadX(ref tex1uv);
            reader.ReadX(ref tex2uv);
            reader.ReadX(ref color);
            reader.ReadX(ref unk_0x34);
            reader.ReadX(ref unk_0x38);
            reader.ReadX(ref unk_0x3C);

            EndAddress = reader.BaseStream.Position;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(position);
            writer.WriteX(normal);
            writer.WriteX(tex0uv);
            writer.WriteX(tex1uv);
            writer.WriteX(tex2uv);
            writer.WriteX(color);
            writer.WriteX(unk_0x34);
            writer.WriteX(unk_0x38);
            writer.WriteX(unk_0x3C);
        }
    }
}