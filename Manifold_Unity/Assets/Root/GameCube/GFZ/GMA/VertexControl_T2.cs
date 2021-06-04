using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.GMA
{
    [Serializable]
    public class VertexControl_T2 : IBinarySerializable, IBinaryAddressable
    {
        [Header("Vtx Ctrl T2")]
        [SerializeField]
        private AddressRange addressRange;

        //
        [SerializeField, LabelPrefix("00")]
        Vector3 position;

        /// <summary>
        /// Not always unit vector
        /// </summary>
        [SerializeField, LabelPrefix("0C")]
        Vector3 normal;

        [SerializeField, LabelPrefix("18")]
        Vector2 tex0uv;

        [SerializeField, LabelPrefix("20")]
        Vector2 tex1uv;

        [SerializeField, LabelPrefix("28")]
        Vector2 tex2uv;

        [SerializeField, LabelPrefix("30")]
        Color32 color;

        [SerializeField, Hex("34", 8)]
        uint unk_0x34;

        [SerializeField, Hex("38", 8)]
        uint unk_0x38;

        [SerializeField, Hex("3C", 8)]
        uint unk_0x3C;


        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }


        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref position);
                reader.ReadX(ref normal);
                reader.ReadX(ref tex0uv);
                reader.ReadX(ref tex1uv);
                reader.ReadX(ref tex2uv);
                reader.ReadX(ref color);
                reader.ReadX(ref unk_0x34);
                reader.ReadX(ref unk_0x38);
                reader.ReadX(ref unk_0x3C);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(position);
            writer.WriteX(normal);
            writer.WriteX(tex0uv);
            writer.WriteX(tex1uv);
            writer.WriteX(tex2uv);
            writer.WriteX(color);
            writer.WriteX(unk_0x34);
            writer.WriteX(unk_0x38);
            writer.WriteX(unk_0x3C);
        }

    }
}