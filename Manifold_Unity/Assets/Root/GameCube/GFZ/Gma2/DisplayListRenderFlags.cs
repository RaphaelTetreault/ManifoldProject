using System;

namespace GameCube.GFZ.Gma2
{
    /// <summary>
    /// 0x13
    /// </summary>
    [Flags]
    public enum DisplayListRenderFlags : byte
    {
        renderPrimaryOpaque = 1 << 0,
        renderPrimaryTranslucid = 1 << 1,
        renderSecondaryOpaque = 1 << 2,
        renderSecondaryTranslucid = 1 << 3,
    }
}