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
    public class KeyableAttribute : IBinarySerializable, IBinaryAddressableRange
    {

        #region FIELDS


        [SerializeField]
        private AddressRange addressRange;

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


        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }


        #endregion

        #region METHODS

        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref easeMode);
                reader.ReadX(ref time);
                reader.ReadX(ref value);
                reader.ReadX(ref zTangentIn);
                reader.ReadX(ref zTangentOut);
            }
            this.RecordEndAddress(reader);
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