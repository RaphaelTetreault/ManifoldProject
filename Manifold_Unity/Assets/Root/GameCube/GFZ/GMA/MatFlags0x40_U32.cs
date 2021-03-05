using System;

namespace GameCube.GFZ.GMA
{
    /// <summary>
    /// 2019/04/23 - All values 0, 17, 18, 20, 36, 48
    /// </summary>
    [Flags]
    public enum MatFlags0x40_U32 : UInt32
    {
        UNK_FLAG_0 = 1 << 0,
        UNK_FLAG_1 = 1 << 1,
        UNK_FLAG_2 = 1 << 2,
        UNK_FLAG_4 = 1 << 4,
        UNK_FLAG_5 = 1 << 5,
    }
}