using GameCube.GX;

namespace GameCube.GFZX01
{
    public static class FzgxGxVat
    {

        public static readonly GxVtxAttrTable FzgxVAT = new GxVtxAttrTable(
            new GxVtxAttrFmt[2]
            {
                // VAT 0
                new GxVtxAttrFmt()
                {
                    pos = new GxVtxAttr(GXCompCnt_Rev2.GX_POS_XYZ, GXCompType.GX_F32),
                    nrm = new GxVtxAttr(GXCompCnt_Rev2.GX_NRM_XYZ, GXCompType.GX_F32),
                    nbt = new GxVtxAttr(GXCompCnt_Rev2.GX_NRM_NBT, GXCompType.GX_F32), // compressed
                    clr0 = new GxVtxAttr(GXCompCnt_Rev2.GX_CLR_RGBA, GXCompType.GX_RGBA8),
                    tex0 = new GxVtxAttr(GXCompCnt_Rev2.GX_TEX_ST, GXCompType.GX_F32),
                    tex1 = new GxVtxAttr(GXCompCnt_Rev2.GX_TEX_ST, GXCompType.GX_F32),
                    tex2 = new GxVtxAttr(GXCompCnt_Rev2.GX_TEX_ST, GXCompType.GX_F32),
                },

                // VAT 1
                new GxVtxAttrFmt()
                {
                    pos = new GxVtxAttr(GXCompCnt_Rev2.GX_POS_XYZ, GXCompType.GX_S16, 8), // Unverified
                    nrm = new GxVtxAttr(GXCompCnt_Rev2.GX_NRM_XYZ, GXCompType.GX_S16, 14),
                    clr0 = new GxVtxAttr(GXCompCnt_Rev2.GX_CLR_RGBA, GXCompType.GX_RGBA8), // same as VAT0?
                    tex0 = new GxVtxAttr(GXCompCnt_Rev2.GX_TEX_ST, GXCompType.GX_S16, 14), // uncheck is s16 is correct
                    tex1 = new GxVtxAttr(GXCompCnt_Rev2.GX_TEX_ST, GXCompType.GX_S16, 14), // same
                    tex2 = new GxVtxAttr(GXCompCnt_Rev2.GX_TEX_ST, GXCompType.GX_S16, 14), // remember fifo padding
                },
            }
        );
    }
}