using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.GMA
{
    [Serializable]
    public class ExtraDisplayListHeader : IBinarySerializable, IBinaryAddressableRange
    {

        #region FIELDS
        

        // consts
        public const int kFifoPaddingSize = 16;

        [Header("Stitch Data")]

        // metadata        
        [SerializeField]
        private AddressRange addressRange;

        // structure
        [Space]
        [SerializeField, Hex("00", 8)]
        int unk_0x00;

        [SerializeField, Hex("04", 8)]
        int unk_0x04;

        [SerializeField, Hex("08", 8)]
        int vertexSize0; // MAT?

        [SerializeField, Hex("0C", 8)]
        int vertexSize1; // TL MAT?

        byte[] fifoPadding;


        #endregion

        #region PROPERTIES


        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        public int VertexSize0 => vertexSize0;

        public int VertexSize1 => vertexSize1;


        #endregion

        #region METHODS


        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref unk_0x00);
                reader.ReadX(ref unk_0x04);
                reader.ReadX(ref vertexSize0);
                reader.ReadX(ref vertexSize1);
                reader.ReadX(ref fifoPadding, kFifoPaddingSize);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(unk_0x00);
            writer.WriteX(unk_0x04);
            writer.WriteX(vertexSize0);
            writer.WriteX(vertexSize1);

            for (int i = 0; i < kFifoPaddingSize; i++)
                writer.WriteX((byte)0x00);
        }


        #endregion

    }
}