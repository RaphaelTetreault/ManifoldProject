using System;

namespace GameCube.GFZ.Gma2
{
    /// <summary>
    /// 0x13
    /// </summary>
    [Flags]
    public enum DisplayListRenderFlags : byte
    {
        renderCW = 1 << 0,
        renderCCW = 1 << 1,
        renderSkinnedCW = 1 << 2,
        renderSkinnedCCW = 1 << 3,
    }
}