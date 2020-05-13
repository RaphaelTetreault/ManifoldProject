﻿using StarkTools.IO;
using System;
using System.IO;
using UnityEngine;

[Serializable]
public struct CameraPan : IBinarySerializable, IBinaryAddressable
{
    public const int kSizeBytes = 0x54;

    [SerializeField, Hex] long startAddress;
    [SerializeField, Hex] long endAddress;

    public int frameCount;
    public float lerpSpeed;
    public int unk_0x08;
    public CameraPanPoint from;
    public CameraPanPoint to;

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
        startAddress = reader.BaseStream.Position;

        reader.ReadX(ref frameCount);
        reader.ReadX(ref lerpSpeed);
        reader.ReadX(ref unk_0x08);
        reader.ReadX(ref from, true);
        reader.ReadX(ref to, true);

        endAddress = reader.BaseStream.Position;
    }

    public void Serialize(BinaryWriter writer)
    {
        throw new NotImplementedException();
    }
}
