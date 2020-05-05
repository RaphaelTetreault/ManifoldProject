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
    // GameCube VAT
    [Serializable]
    public class GxVtxAttrTable
    {
        [SerializeField] GxVtxAttrFmt[] gxVertexAttributeFormats = new GxVtxAttrFmt[8];

        public GxVtxAttrFmt[] GxVertexAttributeFormats
            => gxVertexAttributeFormats;

        public GxVtxAttrTable(params GxVtxAttrFmt[] formats)
        {
            if (formats.Length > 8)
                throw new ArgumentOutOfRangeException();

            // Update formats
            for (int i = 0; i < formats.Length; i++)
                GxVertexAttributeFormats[i] = formats[i];
            // Clear old refs
            for (int i = formats.Length; i < GxVertexAttributeFormats.Length; i++)
                GxVertexAttributeFormats[i] = null;
        }

        public void SetVtxAttrFmt(GXVtxFmt index, /*/GXAttrType vcd, GXAttr attribute,/*/ GXCompCnt_Rev2 nElements, GXCompType format, int nFracBits = 0)
        {
            //var value = new GxVertexAttribute(vcd, attribute, nElements, format, nFracBits);
            //GxVertexAttributeFormats[(int)index].SetAttr(value);
        }
    }

}