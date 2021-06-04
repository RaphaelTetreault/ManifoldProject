// TODO: use/make relptr struct?

using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.GMA
{
    [Serializable]
    public struct GcmfPointerPair : IBinarySerializable, IBinaryAddressable
    {

        #region FIELDS


        /// <summary>
        /// Two's compliment, 0xFFFFFFFF
        /// </summary>
        public const int kNullPtr = -1;
        public const int BinarySize = 0x08;

        // DEBUG
        [SerializeField]
        private AddressRange addressRange;

        // DATA
        [Space]
        [SerializeField, Hex("00", 8)]
        public int gcmfDataRelPtr;

        [SerializeField, Hex("04", 8)]
        public int gcmfNameRelPtr;


        #endregion

        #region PROPERTIES


        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        public int GcmfDataRelPtr
            => gcmfDataRelPtr;

        public int GcmfNameRelPtr
            => gcmfNameRelPtr;


        #endregion

        #region METHODS


        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref gcmfDataRelPtr);
                reader.ReadX(ref gcmfNameRelPtr);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(gcmfDataRelPtr);
            writer.WriteX(gcmfNameRelPtr);
        }


        #endregion

    }
}