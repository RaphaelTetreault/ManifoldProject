using GameCube.GX;

namespace GameCube.FZeroGX
{
    public static class FZeroGX
    {
        // TODO
        // pass GXAttrType at render time (where is index array?)

        private static readonly GxVertexAttributeFormat vaf0 = new GxVertexAttributeFormat()
        {
            pos = new GxVertexAttribute(GXAttrType.GX_DIRECT, GXAttr.GX_VA_POS, GXCompCnt_Rev2.GX_POS_XYZ, GXCompType.GX_F32, 0),
            nrm = new GxVertexAttribute(GXAttrType.GX_DIRECT, GXAttr.GX_VA_NRM, GXCompCnt_Rev2.GX_NRM_XYZ, GXCompType.GX_F32, 0),
            clr0 = new GxVertexAttribute(GXAttrType.GX_DIRECT, GXAttr.GX_VA_CLR0, GXCompCnt_Rev2.GX_CLR_RGBA, GXCompType.GX_F32, 0),
            tex0 = new GxVertexAttribute(GXAttrType.GX_DIRECT, GXAttr.GX_VA_TEX0, GXCompCnt_Rev2.GX_TEX_ST, GXCompType.GX_F32, 0),
            tex1 = new GxVertexAttribute(GXAttrType.GX_DIRECT, GXAttr.GX_VA_TEX1, GXCompCnt_Rev2.GX_TEX_ST, GXCompType.GX_F32, 0),
            tex2 = new GxVertexAttribute(GXAttrType.GX_DIRECT, GXAttr.GX_VA_TEX2, GXCompCnt_Rev2.GX_TEX_ST, GXCompType.GX_F32, 0),
            nbt = new GxVertexAttribute(GXAttrType.GX_DIRECT, GXAttr.GX_VA_NBT, GXCompCnt_Rev2.GX_NRM_NBT, GXCompType.GX_F32, 0),
        };

        private static readonly GxVertexAttributeFormat vaf1 = new GxVertexAttributeFormat()
        {
            pos = new GxVertexAttribute(GXAttrType.GX_INDEX16, GXAttr.GX_VA_POS, GXCompCnt_Rev2.GX_POS_XYZ, GXCompType.GX_S16, 0),
            nrm = new GxVertexAttribute(GXAttrType.GX_INDEX16, GXAttr.GX_VA_NRM, GXCompCnt_Rev2.GX_POS_XYZ, GXCompType.GX_S16, 0),
            tex0 = new GxVertexAttribute(GXAttrType.GX_INDEX16, GXAttr.GX_VA_TEX0, GXCompCnt_Rev2.GX_TEX_ST, GXCompType.GX_S16, 0),
        };

        public static readonly GxVertexAttributeTable VAT = new GxVertexAttributeTable(vaf0, vaf1);
    }
}