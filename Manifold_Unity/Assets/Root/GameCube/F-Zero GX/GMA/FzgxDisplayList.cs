using StarkTools.IO;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

using GameCube.GX;

namespace GameCube.FZeroGX.GMA
{
    [Serializable]
    public class FzgxDisplayList : IBinarySerializable, IBinaryAddressable
    {
        [Header("Display List")]
        [SerializeField, Hex] long startAddress;
        [SerializeField, Hex] long endAddress;

        [SerializeField, Hex] int size; 
        [SerializeField, HideInInspector] byte[] data; 

        #region MEMBERS

        [SerializeField] GxDisplayCommand displayCommand;
        [SerializeField] short nElements;
        [SerializeField] GxVtx[] vertices;

        #endregion

        public FzgxDisplayList() { }

        public FzgxDisplayList(int size)
        {
            this.size = size;
        }

        #region PROPERTIES

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

        #endregion


        public void Deserialize(BinaryReader reader)
        {
            startAddress = reader.BaseStream.Position;

            // Read real
            //reader.ReadX(ref displayCommand, true);
            //throw new NotImplementedException();
            //reader.ReadX(ref vertices, true);

            // Read fake
            reader.ReadX(ref data, size);

            endAddress = reader.BaseStream.Position;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(data, false);
        }

    }
}