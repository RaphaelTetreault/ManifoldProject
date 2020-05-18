using StarkTools.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZX01.FMI
{
    [Serializable]
    public class ExhaustAnimation : IBinarySerializable, IBinaryAddressable
    {
        [SerializeField, Hex] long startAddress;
        [SerializeField, Hex] long endAddress;

        public Vector3 position;
        public int unk_0x0C;
        public int animType;

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

        public void Deserialize(BinaryReader reader)
        {
            startAddress = reader.BaseStream.Position;
            {
                reader.ReadX(ref position);
                reader.ReadX(ref unk_0x0C);
                reader.ReadX(ref animType);
            }
            endAddress = reader.BaseStream.Position;
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}