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
    public class VertexControlHeader : IBinarySerializable, IBinaryAddressable, IFile
    {
        public const int kFifoPaddingSize = 12;

        [Header("Skl Vtx")]
        [SerializeField] string name;
        [SerializeField, HideInInspector] string fileName;
        [SerializeField, Hex] long startAddress;
        [SerializeField, Hex] long endAddress;
        [Space]

        #region MEMBERS

        [SerializeField, Hex("00 -", 8)]
        int vertexCount;

        [SerializeField, Hex("04 -", 8)]
        int unk_type1_relPtr;

        [SerializeField, Hex("08 -", 8)]
        int unk_type2_relPtr;

        [SerializeField, Hex("0C -", 8)]
        int unk_type3_relPtr;

        [SerializeField, Hex("10 -", 8)]
        int unk_type4_relPtr;

        byte[] fifoPadding;

        //
        VertexControl_T1[] vtx1;
        VertexControl_T2[] vtx2;
        VertexControl_T3 vtx3;
        VertexControl_T4 vtx4;

        #endregion

        #region PROPERTIES

        public int VertexCount => vertexCount;

        public int Unk_type1_relPtr => unk_type1_relPtr;

        public int Unk_type2_relPtr => unk_type2_relPtr;

        public int Unk_type3_relPtr => unk_type3_relPtr;

        public int Unk_type4_relPtr => unk_type4_relPtr;

        #endregion

        // Metadata
        public string FileName
        {
            get => fileName;
            set => fileName = value;
        }
        public string ModelName
        {
            get => name;
            set => name = value;
        }
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

            reader.ReadX(ref vertexCount);
            reader.ReadX(ref unk_type1_relPtr);
            reader.ReadX(ref unk_type2_relPtr);
            reader.ReadX(ref unk_type3_relPtr);
            reader.ReadX(ref unk_type4_relPtr);
            reader.ReadX(ref fifoPadding, kFifoPaddingSize);

            EndAddress = reader.BaseStream.Position;
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

    }
}
