using StarkTools.IO;
using System.IO;
using System;
using UnityEngine;

namespace GameCube.FZeroGX.COLI_COURSE
{
    [Serializable]
    public class AnimationKey : IBinarySerializable, IBinaryAddressable
    {

        #region MEMBERS

        [SerializeField, Hex] long startAddress;
        [SerializeField, Hex] long endAddress;

        /// <summary>
        /// All values: 1, 2, or 3.
        /// </summary>
        public uint unk_0x00;
        public float time;
        public Vector3 vector;

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
            reader.ReadX(ref time);
            reader.ReadX(ref vector);

            endAddress = reader.BaseStream.Position;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(unk_0x00);
            writer.WriteX(time);
            writer.WriteX(vector);
        }

        #endregion

    }
}