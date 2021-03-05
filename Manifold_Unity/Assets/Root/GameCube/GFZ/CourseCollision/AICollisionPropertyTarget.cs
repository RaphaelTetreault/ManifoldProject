using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class AICollisionPropertyTarget : IBinarySerializable, IBinaryAddressableRange
    {

        #region FIELDS


        [SerializeField]
        private AddressRange addressRange;

        public float lengthFrom;
        public float lengthTo;
        public float widthLeft;
        public float widthRight;
        public AICollisionProperty aiCollisionProperty;
        public byte trackBranchID;
        [HideInInspector]
        public byte zero_0x12;
        [HideInInspector]
        public byte zero_0x13;


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
            reader.ReadX(ref lengthFrom);
            reader.ReadX(ref lengthTo);
            reader.ReadX(ref widthLeft);
            reader.ReadX(ref widthRight);
            reader.ReadX(ref aiCollisionProperty);
            reader.ReadX(ref trackBranchID);
            reader.ReadX(ref zero_0x12);
            reader.ReadX(ref zero_0x13);
            this.RecordEndAddress(reader);

            System.Diagnostics.Debug.Assert(zero_0x12 == 0);
            System.Diagnostics.Debug.Assert(zero_0x13 == 0);
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }


        #endregion

    }
}
