using GameCube.GX;

namespace GameCube.GFZ
{
    /// <summary>
    /// GX data values for GFZ games.
    /// </summary>
    public static class GfzGX
    {
        /// <summary>
        /// GFZ's Vertex Attribute Table
        /// </summary>
        public static VertexAttributeTable VAT { get => vat; }

        private static readonly VertexAttributeTable vat = new(
            new VertexAttributeFormat[2]
            {
                // VAT 0
                new VertexAttributeFormat()
                {
                    pos = new VertexAttribute(ComponentCount.GX_POS_XYZ, ComponentType.GX_F32),
                    nrm = new VertexAttribute(ComponentCount.GX_NRM_XYZ, ComponentType.GX_F32),
                    nbt = new VertexAttribute(ComponentCount.GX_NRM_NBT, ComponentType.GX_F32), // compressed
                    clr0 = new VertexAttribute(ComponentCount.GX_CLR_RGBA, ComponentType.GX_RGBA8),
                    tex0 = new VertexAttribute(ComponentCount.GX_TEX_ST, ComponentType.GX_F32),
                    tex1 = new VertexAttribute(ComponentCount.GX_TEX_ST, ComponentType.GX_F32),
                    tex2 = new VertexAttribute(ComponentCount.GX_TEX_ST, ComponentType.GX_F32),
                },

                // VAT 1
                new VertexAttributeFormat()
                {
                    pos = new VertexAttribute(ComponentCount.GX_POS_XYZ, ComponentType.GX_S16, 13), // Verified
                    nrm = new VertexAttribute(ComponentCount.GX_NRM_XYZ, ComponentType.GX_S16, 14), // Verified
                    clr0 = new VertexAttribute(ComponentCount.GX_CLR_RGBA, ComponentType.GX_RGBA8), // Verified
                    tex0 = new VertexAttribute(ComponentCount.GX_TEX_ST, ComponentType.GX_S16, 14), // UNVERIFIED if s16 is correct
                    tex1 = new VertexAttribute(ComponentCount.GX_TEX_ST, ComponentType.GX_S16, 14), // ^ same
                    tex2 = new VertexAttribute(ComponentCount.GX_TEX_ST, ComponentType.GX_S16, 14), // ^ same
                },
            }
        );
    }
}