using Manifold.IO;
using System.IO;
using System;
using UnityEngine;

namespace GameCube.GFZ.CourseCollision.Animation
{
    /// <summary>
    /// This structure appears to be a Maya (4.?) KeyableAttribute
    /// </summary>
    [Serializable]
    public class KeyableAttribute : IBinarySerializable, IBinaryAddressable
    {

        #region MEMBERS

        [SerializeField, Hex] long startAddress;
        [SerializeField, Hex] long endAddress;

        /// <summary>
        /// All values: 1, 2, or 3.
        /// </summary>
        public EaseMode easeMode;
        public float time;
        public float value;
        public float zTangentIn;
        public float zTangentOut;

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

            reader.ReadX(ref easeMode);
            reader.ReadX(ref time);
            reader.ReadX(ref value);
            reader.ReadX(ref zTangentIn);
            reader.ReadX(ref zTangentOut);

            endAddress = reader.BaseStream.Position;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(easeMode);
            writer.WriteX(time);
            writer.WriteX(value);
            writer.WriteX(zTangentIn);
            writer.WriteX(zTangentOut);
        }

        #endregion

    }
}