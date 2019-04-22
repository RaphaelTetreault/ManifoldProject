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
    // NEW STUFF
    [Serializable]
    public class VertexControlData : IBinarySerializable, IBinaryAddressable, IFile
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

            EndAddress = reader.BaseStream.Position;
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

    }

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
            throw new NotImplementedException();
        }
    }

    [Serializable]
    public class VertexControl_T2 : IBinarySerializable, IBinaryAddressable
    {
        [Header("Vtx Ctrl T2")]
        [SerializeField, Hex(8)] long startAddress;
        [SerializeField, Hex(8)] long endAddress;

        //
        [SerializeField]
        Vector3 position;

        /// <summary>
        /// Not always unit vector
        /// </summary>
        [SerializeField]
        Vector3 normal;

        [SerializeField]
        Vector2 tex0uv;

        [SerializeField]
        Vector2 tex1uv;

        [SerializeField]
        Vector2 tex2uv;

        [SerializeField]
        Color32 color;

        [SerializeField]
        uint unk_0x34;

        [SerializeField]
        uint unk_0x38;

        [SerializeField]
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
            throw new NotImplementedException();
        }
    }

    [Serializable]
    public class VertexControl_T3 : IBinarySerializable, IBinaryAddressable
    {
        public const int kFifoPaddingSize = 12;

        [Header("Vtx Ctrl T3")]
        [SerializeField, Hex(8)] long startAddress;
        [SerializeField, Hex(8)] long endAddress;

        // Count?
        [SerializeField]
        int unk_0x00;

        // Length? Or is the following an array that is FIFO aligned?
        [SerializeField]
        int unk_0x04;

        [SerializeField]
        int unk_0x08;

        [SerializeField]
        int unk_0x0C;

        [SerializeField]
        int unk_0x10;

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

            reader.ReadX(ref unk_0x00);
            reader.ReadX(ref unk_0x04);
            reader.ReadX(ref unk_0x08);
            reader.ReadX(ref unk_0x0C);
            reader.ReadX(ref unk_0x10);
            reader.ReadX(ref fifoPadding, kFifoPaddingSize);

            EndAddress = reader.BaseStream.Position;
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }

    [Serializable]
    public class VertexControl_T4 : IBinarySerializable, IBinaryAddressable
    {
        public const int kFifoPaddingSize = 30;

        [Header("Vtx Ctrl T4")]
        [SerializeField, Hex(8)] long startAddress;
        [SerializeField, Hex(8)] long endAddress;

        // is this it?
        [SerializeField]
        ushort unk_0x00;

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

            reader.ReadX(ref unk_0x00);
            reader.ReadX(ref fifoPadding, kFifoPaddingSize);

            EndAddress = reader.BaseStream.Position;
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
