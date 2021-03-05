using GameCube.GX;

namespace GameCube.GFZ
{
    public static class VertexAttributeTable
    {
        public static readonly GX.VertexAttributeTable GfzVat = new GX.VertexAttributeTable(
            new VertexAttributeFormat[2]
            {
                // VAT 0
                new VertexAttributeFormat()
                {
                    pos = new VertexAttribute(GXCompCnt_Rev2.GX_POS_XYZ, GXCompType.GX_F32),
                    nrm = new VertexAttribute(GXCompCnt_Rev2.GX_NRM_XYZ, GXCompType.GX_F32),
                    nbt = new VertexAttribute(GXCompCnt_Rev2.GX_NRM_NBT, GXCompType.GX_F32), // compressed
                    clr0 = new VertexAttribute(GXCompCnt_Rev2.GX_CLR_RGBA, GXCompType.GX_RGBA8),
                    tex0 = new VertexAttribute(GXCompCnt_Rev2.GX_TEX_ST, GXCompType.GX_F32),
                    tex1 = new VertexAttribute(GXCompCnt_Rev2.GX_TEX_ST, GXCompType.GX_F32),
                    tex2 = new VertexAttribute(GXCompCnt_Rev2.GX_TEX_ST, GXCompType.GX_F32),
                },

                // VAT 1
                new VertexAttributeFormat()
                {
                    pos = new VertexAttribute(GXCompCnt_Rev2.GX_POS_XYZ, GXCompType.GX_S16, 13), // Verified
                    nrm = new VertexAttribute(GXCompCnt_Rev2.GX_NRM_XYZ, GXCompType.GX_S16, 14), // Verified
                    clr0 = new VertexAttribute(GXCompCnt_Rev2.GX_CLR_RGBA, GXCompType.GX_RGBA8), // Verified
                    tex0 = new VertexAttribute(GXCompCnt_Rev2.GX_TEX_ST, GXCompType.GX_S16, 14), // UNVERIFIED if s16 is correct
                    tex1 = new VertexAttribute(GXCompCnt_Rev2.GX_TEX_ST, GXCompType.GX_S16, 14), // ^ same
                    tex2 = new VertexAttribute(GXCompCnt_Rev2.GX_TEX_ST, GXCompType.GX_S16, 14), // ^ same
                },
            }
        );
    }
}