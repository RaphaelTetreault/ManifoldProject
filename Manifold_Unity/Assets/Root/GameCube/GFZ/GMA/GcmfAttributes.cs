namespace GameCube.GFZ.GMA
{
    /// <summary>
    /// Code from https://github.com/bobjrsenior/GxUtils/blob/master/GxUtils/LibGxFormat/Gma/Gcmf.cs
    /// </summary>
    [System.Flags]
    public enum GcmfAttributes : uint
    {
        /// <summary>
        /// Vertices are stored in 16-bit compressed floating point number format using GameCube GX conventions.
        /// </summary>
        IS_16_BIT = 1 << 0, //0x01

        UNUSED_1 = 1 << 1,

        /// <summary>
        /// Called "Stitching Model" in the debug menu. Has associated transform matrices.
        /// </summary>
        IS_STITCHING_MODEL = 1 << 2, //0x04

        /// <summary>
        /// Called "Skin Model" in the debug menu. Has associated transform matrices and indexed vertices.
        /// </summary>
        IS_SKIN_MODEL = 1 << 3, //0x08

        /// <summary>
        /// Called "Effective Model" in the debug menu. Has physics-driven indexed vertices.
        /// </summary>
        IS_EFFECTIVE_MODEL = 1 << 4, // 0x10

        UNUSED_5 = 1 << 5,

        UNUSED_6 = 1 << 6,

        UNUSED_7 = 1 << 7,
    }
}
