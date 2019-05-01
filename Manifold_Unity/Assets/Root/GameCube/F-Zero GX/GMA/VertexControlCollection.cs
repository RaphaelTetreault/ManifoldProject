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
    public class VertexControlCollection : IBinarySerializable //, IBinaryAddressable
    {
        [Header("Vertex Control Collection")]
        [SerializeField] VertexControlHeader vertexControlHeader;
        [SerializeField] VertexControl_T1[] t1;
        [SerializeField] VertexControl_T2[] t2;
        [SerializeField] VertexControl_T3 t3;
        [SerializeField] VertexControl_T4 t4;

        public VertexControlCollection() { }

        public VertexControlCollection(VertexControlHeader vertexControlHeader)
        {
            this.vertexControlHeader = vertexControlHeader;
        }

        public VertexControl_T1[] T1 => t1;
        public VertexControl_T2[] T2 => t2;
        public VertexControl_T3 T3 => t3;
        public VertexControl_T4 T4 => t4;


        public void Deserialize(BinaryReader reader)
        {
            var count = vertexControlHeader.VertexCount;
            var rootAddress = vertexControlHeader.StartAddress;

            reader.BaseStream.Seek(rootAddress + vertexControlHeader.Unk_type1_relPtr, SeekOrigin.Begin);
            reader.ReadX(ref t1, count, true);
            reader.BaseStream.Seek(rootAddress + vertexControlHeader.Unk_type2_relPtr, SeekOrigin.Begin);
            reader.ReadX(ref t2, count, true);
            reader.BaseStream.Seek(rootAddress + vertexControlHeader.Unk_type3_relPtr, SeekOrigin.Begin);
            reader.ReadX(ref t3, true);
            reader.BaseStream.Seek(rootAddress + vertexControlHeader.Unk_type4_relPtr, SeekOrigin.Begin);
            reader.ReadX(ref t4, true);
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}