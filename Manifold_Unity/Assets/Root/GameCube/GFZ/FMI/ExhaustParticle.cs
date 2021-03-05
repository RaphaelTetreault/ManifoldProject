using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.FMI
{
    [Serializable]
    public class ExhaustParticle : IBinarySerializable, IBinaryAddressableRange
    {

        #region MEMBERS


        // metadata
        [SerializeField]
        private AddressRange addressRange;

        //structure
        public Vector3 position;
        public uint unk_0x0C;
        public uint unk_0x10;
        public float scaleMin;
        public float scaleMax;
        [Tooltip("Engine Color of Normal Acceleration")]
        public Color32 colorMin;
        [Tooltip("Engine Color of Strong Acceleration")]
        public Color32 colorMax;


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
                reader.ReadX(ref unk_0x10);
                reader.ReadX(ref scaleMin);
                reader.ReadX(ref scaleMax);
                reader.ReadX(ref colorMin);
                reader.ReadX(ref colorMax);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }


        #endregion

    }
}