using StarkTools.IO;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace GameCube.GX
{

    [Serializable]
    public struct GxDisplayCommand : IBinarySerializable
    {
        [SerializeField] public GXPrimitive primitive;
        [SerializeField] public GXVtxFmt vertexFormat;
        public ushort command;

        public void Deserialize(BinaryReader reader)
        {
            reader.ReadX(ref command);
            primitive = (GXPrimitive)(command & 0b_00000000_11111000); // 5 highest bits
            vertexFormat = (GXVtxFmt)(command & 0b_00000000_00000111); // 3 lowest bits
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(command);
        }
    }
}