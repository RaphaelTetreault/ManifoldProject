using GameCube.GX;

namespace GameCube.GFZX01
{
    public static class VAT
    {
        public static readonly GxVtxAttrTable GFZX01_VAT = new GxVtxAttrTable(
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
                    pos = new GxVtxAttr(GXCompCnt_Rev2.GX_POS_XYZ, GXCompType.GX_S16, 13), // Verified
                    nrm = new GxVtxAttr(GXCompCnt_Rev2.GX_NRM_XYZ, GXCompType.GX_S16, 14),
                    clr0 = new GxVtxAttr(GXCompCnt_Rev2.GX_CLR_RGBA, GXCompType.GX_RGBA8),
                    tex0 = new GxVtxAttr(GXCompCnt_Rev2.GX_TEX_ST, GXCompType.GX_S16, 14), // uncheck if s16 is correct
                    tex1 = new GxVtxAttr(GXCompCnt_Rev2.GX_TEX_ST, GXCompType.GX_S16, 14), // same
                    tex2 = new GxVtxAttr(GXCompCnt_Rev2.GX_TEX_ST, GXCompType.GX_S16, 14), // same
                },
            }
        );
    }
}