namespace GameCube.GFZ.GMA
{
    // if 0&1, enable 2
    /// <summary>
    /// MIPMAP Settings
    /// Look at: GXTexFilter
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    [System.Flags]
    public enum MipmapSetting : byte
    {
        /// <summary>
        /// 2019/04/03 VERIFIED: Enable (large preview) mipmaps
        /// </summary>
        ENABLE_MIPMAP = 1 << 0,
        /// <summary>
        /// 2019/04/03 THEORY: when only flag, "use custom mip-map"
        /// See: bg_san_s [39/41] tex [3/8] - RIVER01
        /// See: bg_big [118/120] tex [1/1] - OCE_OCEAN_C14_B_ltmp2
        /// See: any recovery pad
        /// </summary>
        UNK_FLAG_1 = 1 << 1, // Working together?
        UNK_FLAG_2 = 1 << 2, // Working together?
        /// <summary>
        /// 2019/04/03 VERIFIED: Enable Mipmap NEAR
        /// </summary>
        ENABLE_NEAR = 1 << 3,
        /// <summary>
        /// Height map? Blend? (they are greyscale)
        /// Low occurences: 188 for tracks and st2 boulders
        /// </summary>
        UNK_FLAG_4 = 1 << 4,
        /// <summary>
        /// Used as alpha mask? (likely?) Perhaps some mip preservation stuff.
        /// </summary>
        UNK_FLAG_5 = 1 << 5,
        /// <summary>
        /// Total occurences = 3. Only MCSO, on a single geometry set.
        /// </summary>
        UNK_FLAG_6 = 1 << 6,
        /// <summary>
        /// On many vehicles
        /// </summary>
        UNK_FLAG_7 = 1 << 7,
    }
}