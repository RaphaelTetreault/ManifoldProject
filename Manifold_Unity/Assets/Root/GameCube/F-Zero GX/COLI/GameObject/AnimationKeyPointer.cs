using StarkTools.IO;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameCube.FZeroGX.COLI_COURSE
{
    [Serializable]
    public class AnimationKeyPointer : IBinarySerializable, IBinaryAddressable
    {

        #region MEMBERS

        [SerializeField, Hex] long startAddress;
        [SerializeField, Hex] long endAddress;

        public uint unk_0x00;
        public uint unk_0x04;
        public uint unk_0x08;
        public uint unk_0x0C;
        public int keyCount;
        public uint keyAbsPtr;

        public AnimationKey[] keysFrames;

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
            {
                reader.ReadX(ref unk_0x00);
                reader.ReadX(ref unk_0x04);
                reader.ReadX(ref unk_0x08);
                reader.ReadX(ref unk_0x0C);
                reader.ReadX(ref keyCount);
                reader.ReadX(ref keyAbsPtr);
            }
            endAddress = reader.BaseStream.Position;
            {
                keysFrames = new AnimationKey[keyCount];
                if (keyAbsPtr > 0)
                {
                    reader.BaseStream.Seek(keyAbsPtr, SeekOrigin.Begin);
                    reader.ReadX(ref keysFrames, keyCount, true);
                }
            }
            reader.BaseStream.Seek(endAddress, SeekOrigin.Begin);
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}