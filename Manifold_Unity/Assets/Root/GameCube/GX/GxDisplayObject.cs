using StarkTools.IO;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace GameCube.GX
{
    [Serializable]
    public class GxDisplayObject : IBinarySerializable, IBinaryAddressable
    {
        [Header("GxDisplayList")]
        [SerializeField, Hex(8)] long startAddress;
        [SerializeField, Hex(8)] long endAddress;

        [Space]
        [SerializeField, LabelPrefix("00 -")]
        public GxDisplayCommand gxDisplayCommand;

        [SerializeField, Hex("01 -", 2)]
        public byte vertCount;

        [SerializeField, LabelPrefix("02 -")]
        public GxVtx[] verts;

        /// <summary>
        /// 2020-05-04 Raph: I think this needs to be an index 0-7
        /// </summary>
        [SerializeField, HideInInspector]
        public GxVtxAttrTable vat;

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

        public GxDisplayObject() { }

        public GxDisplayObject(GxVtxAttrTable vat)
        {
            this.vat = vat;
        }

        public void Deserialize(BinaryReader reader)
        {
            startAddress = reader.BaseStream.Position;

            reader.ReadX(ref gxDisplayCommand, true);
            reader.ReadX(ref vertCount);

            // Init vertex VAT references
            verts = new GxVtx[vertCount];
            for (int i = 0; i < verts.Length; i++)
            {
                var vertexFormat = gxDisplayCommand.vertexFormat;
                var vatIndex = (int)vertexFormat;
                verts[i] = new GxVtx()
                {
                    vertAttr = vat.GxVertexAttributeFormats[vatIndex],
                };
                reader.ReadX(ref verts[i], false);
            }

            endAddress = reader.BaseStream.Position;
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }

}