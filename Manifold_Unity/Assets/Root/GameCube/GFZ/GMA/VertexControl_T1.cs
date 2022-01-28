using Manifold.EditorTools;
using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.GMA
{
    [Serializable]
    public class VertexControl_T1 : IBinarySerializable, IBinaryAddressable
    {

        #region FIELDS


        [Header("Vtx Ctrl T1")]
        [SerializeField]
        private AddressRange addressRange;

        // This is a guess
        [SerializeField, LabelPrefix("00")]
        Vector3 position;

        [SerializeField, LabelPrefix("0C")]
        Vector3 normal;

        [SerializeField, Hex("18", 8)]
        uint unk_0x18;

        [SerializeField, LabelPrefix("1C")]
        float unk_0x1C;


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
                reader.ReadX(ref normal);
                reader.ReadX(ref unk_0x18);
                reader.ReadX(ref unk_0x1C);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(position);
            writer.WriteX(normal);
            writer.WriteX(unk_0x18);
            writer.WriteX(unk_0x1C);
        }


        #endregion

    }
}