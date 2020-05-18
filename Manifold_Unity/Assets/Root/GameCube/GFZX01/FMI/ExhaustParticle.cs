using StarkTools.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZX01.FMI
{
    [Serializable]
    public class ExhaustParticle : IBinarySerializable, IBinaryAddressable
    {
        [SerializeField, Hex] long startAddress;
        [SerializeField, Hex] long endAddress;

        public Vector3 position;
        public uint unk_0x0C;
        public uint unk_0x10;
        public float scaleMin;
        public float scaleMax;
        [Header("Engine Color of Normal Acceleration")]
        public Color32 colorMin;
        [Header("Engine Color of Strong Acceleration")]
        public Color32 colorMax;

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
                reader.ReadX(ref unk_0x10);
                reader.ReadX(ref scaleMin);
                reader.ReadX(ref scaleMax);
                reader.ReadX(ref colorMin);
                reader.ReadX(ref colorMax);
            }
            endAddress = reader.BaseStream.Position;
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }
    }
}