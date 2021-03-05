using System;

namespace GameCube.GFZ.GMA
{
    /// <summary>
    /// 0x13
    /// </summary>
    [Flags]
    public enum RenderDisplayListSetting : byte
    {
        RENDER_DISPLAY_LIST_0 = 1 << 0,
        RENDER_DISPLAY_LIST_1 = 1 << 1,
        RENDER_EX_DISPLAY_LIST_0 = 1 << 2,
        RENDER_EX_DISPLAY_LIST_1 = 1 << 3,
        UNUSED_FLAG_4 = 1 << 4,
        UNUSED_FLAG_5 = 1 << 5,
        UNUSED_FLAG_6 = 1 << 6,
        UNUSED_FLAG_7 = 1 << 7,
    }
}