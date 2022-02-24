using System;

namespace GameCube.GFZ.Gma2
{
    /// <summary>
    /// 2019/04/23 - looks like flags - 14 combinations
    /// </summary>
    [Flags]
    public enum MatFlags0x10 : byte
    {
        unk0 = 1 << 0,
        unk1 = 1 << 1,
        unk2 = 1 << 2,
        unk3 = 1 << 3,
        unk4 = 1 << 4,
        unk5 = 1 << 5,
        unk6 = 1 << 6,
        unk7 = 1 << 7,
    }
}