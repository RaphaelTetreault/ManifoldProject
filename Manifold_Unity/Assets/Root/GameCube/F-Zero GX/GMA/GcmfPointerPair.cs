using GameCube.GX;
using StarkTools.IO;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace GameCube.FZeroGX.GMA
{
    [Serializable]
    public struct GcmfPointerPair : IBinarySerializable, IBinaryAddressable
    {
        /// <summary>
        /// Two's compliment, 0xFFFFFFFF
        /// </summary>
        public const int kNullPtr = -1;

        #region MEMBERS

        // DEBUG
        [SerializeField, Hex(8)]
        long startAddress;

        [SerializeField, Hex(8)]
        long endAddress;

        // DATA
        [Space]
        [SerializeField, Hex("00", 8)]
        int gcmfDataRelPtr;

        [SerializeField, Hex("04", 8)]
        int gcmfNameRelPtr;

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

        public int GcmfDataRelPtr
            => gcmfDataRelPtr;

        public int GcmfNameRelPtr
            => gcmfNameRelPtr;

        #endregion

        #region METHODS

        public void Deserialize(BinaryReader reader)
        {
            startAddress = reader.BaseStream.Position;

            reader.ReadX(ref gcmfDataRelPtr);
            reader.ReadX(ref gcmfNameRelPtr);

            endAddress = reader.BaseStream.Position;
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}