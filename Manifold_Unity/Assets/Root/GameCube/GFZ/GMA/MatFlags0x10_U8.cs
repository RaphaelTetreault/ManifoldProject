using System;

namespace GameCube.GFZ.GMA
{
    /// <summary>
    /// 2019/04/23 - looks like flags - 14 combinations
    /// </summary>
    [Flags]
    public enum MatFlags0x10_U8 : byte
    {
        UNK_FLAG_0 = 1 << 0,
        UNK_FLAG_1 = 1 << 1,
        UNK_FLAG_2 = 1 << 2,
        UNK_FLAG_3 = 1 << 3,
        UNK_FLAG_4 = 1 << 4,
        UNK_FLAG_5 = 1 << 5,
        UNK_FLAG_6 = 1 << 6,
        UNK_FLAG_7 = 1 << 7,
    }
}