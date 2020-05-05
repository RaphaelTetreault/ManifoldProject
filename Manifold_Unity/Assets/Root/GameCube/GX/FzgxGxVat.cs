using GameCube.GX;

namespace GameCube.FZeroGX
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
                    nbt = new GxVtxAttr(GXCompCnt_Rev2.GX_NRM_NBT, GXCompType.GX_U8), // index
                    // not 100% sure on colour, all I've seen are alpha=FF
                    clr0 = new GxVtxAttr(GXCompCnt_Rev2.GX_CLR_RGBA, GXCompType.GX_RGBA8),
                    tex0 = new GxVtxAttr(GXCompCnt_Rev2.GX_TEX_ST, GXCompType.GX_F32),
                    tex1 = new GxVtxAttr(GXCompCnt_Rev2.GX_TEX_ST, GXCompType.GX_F32),
                    tex2 = new GxVtxAttr(GXCompCnt_Rev2.GX_TEX_ST, GXCompType.GX_F32),
                },

                // VAT 1
                new GxVtxAttrFmt()
                {
                    pos = new GxVtxAttr(GXCompCnt_Rev2.GX_POS_XYZ, GXCompType.GX_S16),
                    nrm = new GxVtxAttr(GXCompCnt_Rev2.GX_NRM_XYZ, GXCompType.GX_S16),
                    clr0 = new GxVtxAttr(GXCompCnt_Rev2.GX_CLR_RGBA, GXCompType.GX_RGBA8), // same as VAT0?
                    tex0 = new GxVtxAttr(GXCompCnt_Rev2.GX_TEX_ST, GXCompType.GX_S16), // uncheck is s16 is correct
                    tex1 = new GxVtxAttr(GXCompCnt_Rev2.GX_TEX_ST, GXCompType.GX_S16), // same
                    tex2 = new GxVtxAttr(GXCompCnt_Rev2.GX_TEX_ST, GXCompType.GX_S16), // remember fifo padding
                },
            }
        );
    }
}