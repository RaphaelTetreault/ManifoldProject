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
    public struct GxVtxAttr
    {
        [SerializeField] public bool enabled;
        //[SerializeField] public GXAttrType vcd;
        //[SerializeField] public GXAttr attribute;
        [SerializeField] public GXCompCnt_Rev2 nElements;
        [SerializeField] public GXCompType componentFormat;
        [SerializeField] public int nFracBits;

        public GxVtxAttr(/*/GXAttrType vcd, GXAttr attribute,/*/ GXCompCnt_Rev2 nElements, GXCompType format, int nFracBits = 0)
        {
            // Assert that we aren't shifting more bits than we have
            if (format == GXCompType.GX_S8 | format == GXCompType.GX_U8)
                Assert.IsTrue(nFracBits < 8);
            if (format == GXCompType.GX_S16 | format == GXCompType.GX_U16)
                Assert.IsTrue(nFracBits < 16);

            this.enabled = true;
            //this.vcd = vcd;
            //this.attribute = attribute;
            this.nElements = nElements;
            this.componentFormat = format;
            this.nFracBits = nFracBits;
        }
    }
}