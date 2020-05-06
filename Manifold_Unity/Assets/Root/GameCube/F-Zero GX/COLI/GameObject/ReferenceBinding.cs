using StarkTools.IO;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameCube.FZeroGX.COLI_COURSE
{
    [Serializable]
    public class ReferenceBinding : IBinarySerializable, IBinaryAddressable
    {

        #region MEMBERS

        [SerializeField, Hex] long startAddress;
        [SerializeField, Hex] long endAddress;

        public uint unk_0x00;
        public uint nameRelPtr;
        public uint unk_0x08;
        public float unk_0x0C;

        public string name;

        #endregion

        #region PROPERTIES

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

        #endregion

        #region METHODS

        public void Deserialize(BinaryReader reader)
        {
            startAddress = reader.BaseStream.Position;

            reader.ReadX(ref unk_0x00);
            reader.ReadX(ref nameRelPtr);
            reader.ReadX(ref unk_0x08);
            reader.ReadX(ref unk_0x0C);

            endAddress = reader.BaseStream.Position;

            // read string name
            throw new NotImplementedException();
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(unk_0x00);
            writer.WriteX(nameRelPtr);
            writer.WriteX(unk_0x08);
            writer.WriteX(unk_0x0C);

            // write ptr into name table
            throw new NotImplementedException();
        }

        #endregion

    }
}