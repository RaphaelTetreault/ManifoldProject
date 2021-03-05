// GXCompType
namespace GameCube.GX
{
    /// <summary>
    /// GX Component Type
    /// Related to GXVtxFmt
    /// </summary>
    public enum ComponentType
    {
        /// <summary>
        /// Unsigned 8-bit integer
        /// </summary>
        GX_U8 = 0,
        /// <summary>
        /// Signed 8-bit integer
        /// </summary>
        GX_S8,
        /// <summary>
        /// Unsigned 16-bit integer
        /// </summary>
        GX_U16,
        /// <summary>
        /// Signed 16-bit integer
        /// </summary>
        GX_S16,
        /// <summary>
        /// 32-bit floating-point
        /// </summary>
        GX_F32,

        /// <summary>
        /// 16-bit RGB
        /// </summary>
        GX_RGB565 = 0,
        /// <summary>
        /// 24-bit RGB
        /// </summary>
        GX_RGB8,
        /// <summary>
        /// 32-bit RGBX
        /// </summary>
        GX_RGBX8,
        /// <summary>
        /// 16-bit RGBA
        /// </summary>
        GX_RGBA4,
        /// <summary>
        /// 24-bit RGBA
        /// </summary>
        GX_RGBA6,
        /// <summary>
        /// 32-bit RGBA
        /// </summary>
        GX_RGBA8,
    }
}
