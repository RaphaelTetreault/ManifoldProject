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
        renderSecondaryCW = 1 << 2,
        renderSecondaryCCW = 1 << 3,
    }
}