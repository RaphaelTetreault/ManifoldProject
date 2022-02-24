using System;

namespace GameCube.GFZ.Gma2
{
    /// <summary>
    /// 2019/04/23 - All values 0, 17, 18, 20, 36, 48
    /// </summary>
    [Flags]
    public enum MatFlags0x40 : uint
    {
        unk0 = 1 << 0, // 0x01
        unk1 = 1 << 1, // 0x02
        unk2 = 1 << 2, // 0x04
        unk4 = 1 << 4, // 0x16
        unk5 = 1 << 5, // 0x32
    }
}