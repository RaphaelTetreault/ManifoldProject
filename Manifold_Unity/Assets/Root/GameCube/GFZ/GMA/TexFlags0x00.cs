using System;

namespace GameCube.GFZ.GMA
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Notes:
    /// Combinations: 4&5 (x1986), 1&3 (x7), 1&4 (x1)
    /// Flags used: 1, 3, 4, 5, 6
    /// </remarks>
    [Flags]
    public enum TexFlags0x00 : ushort
    {
        /// <summary>
        /// Unused
        /// </summary>
        UNUSED_FLAG_0 = 1 << 0,

        /// <summary>
        /// Based on st24 models, uv scroll. Scroll values stored in TextureScroll class
        /// attached to SceneObjectDynamic. TODO: find how scrolls are indexed.
        /// </summary>
        ENABLE_UV_SCROLL = 1 << 1,

        /// <summary>
        /// Unused
        /// </summary>
        //UNUSED_FLAG_2 = 1 << 2,

        /// <summary>
        /// 7 occurences total. (st21,lz.gma, [75,76,77/130] guide_light*, [1/6])
        /// </summary>
        UNK_FLAG_3 = 1 << 3,

        /// <summary>
        /// Appears to be used whenever tex is for bg reflections
        /// </summary>
        UNK_FLAG_4 = 1 << 4,

        /// <summary>
        /// ..?
        /// </summary>
        UNK_FLAG_5 = 1 << 5,

        /// <summary>
        /// Appears to be used whenever tex is for bg reflections
        /// </summary>
        UNK_FLAG_6 = 1 << 6,

        /// <summary>
        /// Unused
        /// </summary>
        //UNUSED_FLAG_7 = 1 << 7,
    }
}