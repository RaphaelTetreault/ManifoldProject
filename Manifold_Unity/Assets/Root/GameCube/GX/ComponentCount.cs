// GXCompCount
namespace GameCube.GX
{
    /// <summary>
    /// GX Component Count
    /// </summary>
    public enum ComponentCount
    {
        /// <summary>
        /// X,Y position
        /// </summary>
        GX_POS_XY = 0,
        /// <summary>
        /// X,Y,Z position
        /// </summary>
        GX_POS_XYZ = 1,


        /// <summary>
        /// X,Y,Z normal
        /// </summary>
        GX_NRM_XYZ = 0,
        /// <summary>
        /// Normal, Binormal, Tangent
        /// one index per NBT
        /// </summary>
        GX_NRM_NBT = 1,
        /// <summary>
        /// Normal, Binormal, Tangent x3 (HW2 only)
        /// one index per each of N/B/T
        /// </summary>
        GX_NRM_NBT3 = 2,

        /// <summary>
        /// RGB color
        /// </summary>
        GX_CLR_RGB = 0,
        /// <summary>
        /// RGBA color
        /// </summary>
        GX_CLR_RGBA = 1,

        /// <summary>
        /// One texture dimension
        /// </summary>
        GX_TEX_S = 0,
        /// <summary>
        /// Two texture dimensions
        /// </summary>
        GX_TEX_ST = 1,
    }
}
