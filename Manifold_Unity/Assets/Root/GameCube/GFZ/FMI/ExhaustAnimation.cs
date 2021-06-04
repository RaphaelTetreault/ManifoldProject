using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.FMI
{
    [Serializable]
    public class ExhaustAnimation : IBinarySerializable, IBinaryAddressable
    {

        #region FIELDS

        [SerializeField, HideInInspector]
        private AddressRange addressRange;
        public string name;

        // structure
        public Vector3 position;
        public int unk_0x0C;
        public int animType;


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
                reader.ReadX(ref position);
                reader.ReadX(ref unk_0x0C);
                reader.ReadX(ref animType);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }


        #endregion

    }
}