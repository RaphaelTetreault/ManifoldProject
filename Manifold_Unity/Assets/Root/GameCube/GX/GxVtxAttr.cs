using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace GameCube.GX
{
    [Serializable]
    public class GxVtxAttr
    {
        [SerializeField] public bool enabled;
        [SerializeField] public GXCompCnt_Rev2 nElements;
        [SerializeField] public GXCompType componentFormat;
        [SerializeField] public int nFracBits;

        public GxVtxAttr(GXCompCnt_Rev2 nElements, GXCompType format, int nFracBits = 0)
        {
            // Assert that we aren't shifting more bits than we have
            if (format == GXCompType.GX_S8 | format == GXCompType.GX_U8)
                Assert.IsTrue(nFracBits < 8);
            if (format == GXCompType.GX_S16 | format == GXCompType.GX_U16)
                Assert.IsTrue(nFracBits < 16);

            this.enabled = true;
            this.nElements = nElements;
            this.componentFormat = format;
            this.nFracBits = nFracBits;
        }
    }
}