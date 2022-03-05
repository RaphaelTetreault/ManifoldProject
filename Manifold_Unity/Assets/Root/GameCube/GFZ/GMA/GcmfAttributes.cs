namespace GameCube.GFZ.GMA
{
    /// <summary>
    /// Describes attributes embedded in the GCMF data.
    /// </summary>
    [System.Flags]
    public enum GcmfAttributes : uint
    {
        /// <summary>
        /// Vertices are stored in 16-bit compressed fixed-point number format using GameCube GX VAT.
        /// </summary>
        is16Bit = 1 << 0, //0x01

        /// <summary>
        /// Called "Stitching Model" in the debug menu. Has associated transform matrices.
        /// </summary>
        isStitchingModel = 1 << 2, //0x04

        /// <summary>
        /// Called "Skin Model" in the debug menu. Has associated transform matrices and indexed vertices.
        /// </summary>
        isSkinModel = 1 << 3, //0x08

        /// <summary>
        /// Called "Effective Model" in the debug menu. Has physics-driven indexed vertices.
        /// </summary>
        isEffectiveModel = 1 << 4, // 0x10
    }
}
