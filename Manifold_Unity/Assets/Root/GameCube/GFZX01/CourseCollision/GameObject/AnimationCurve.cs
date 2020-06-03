using GameCube.GFZX01.CourseCollision.Animation;
using StarkTools.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZX01.CourseCollision
{
    [Serializable]
    public class AnimationCurve : IBinarySerializable, IBinaryAddressable
    {

        #region MEMBERS

        [SerializeField, Hex] long startAddress;
        [SerializeField, Hex] long endAddress;

        public uint unk_0x00;
        public uint unk_0x04;
        public uint unk_0x08;
        public uint unk_0x0C;
        public int keyableCount;
        public uint keyableAbsPtr;

        public KeyableAttribute[] keyableAttributes;

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
                reader.ReadX(ref keyableCount);
                reader.ReadX(ref keyableAbsPtr);
            }
            endAddress = reader.BaseStream.Position;
            {
                keyableAttributes = new KeyableAttribute[keyableCount];
                if (keyableCount > 0)
                {
                    reader.BaseStream.Seek(keyableAbsPtr, SeekOrigin.Begin);
                    reader.ReadX(ref keyableAttributes, keyableCount, true);
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