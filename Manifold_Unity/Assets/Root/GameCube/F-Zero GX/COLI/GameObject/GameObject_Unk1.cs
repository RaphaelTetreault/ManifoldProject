using StarkTools.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.FZeroGX.COLI_COURSE
{
    [Serializable]
    public class ObjectTable_Unk1 : IBinarySerializable, IBinaryAddressable
    {

        #region MEMBERS

        [SerializeField, Hex] long startAddress;
        [SerializeField, Hex] long endAddress;



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



            endAddress = reader.BaseStream.Position;
        }

        public void Serialize(BinaryWriter writer)
        {

        }

        #endregion

    }
}