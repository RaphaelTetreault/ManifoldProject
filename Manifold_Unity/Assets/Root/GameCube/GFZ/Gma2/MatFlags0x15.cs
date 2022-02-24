using System;

namespace GameCube.GFZ.Gma2
{
    /// <summary>
    /// 2019/04/23 - Definitely flags
    /// </summary>
    [Flags]
    public enum MatFlags0x15 : byte
    {
        unk0 = 1 << 0,
        unk1 = 1 << 1,
        unk2 = 1 << 2,
        unk3 = 1 << 3,
        //UNUSED_FLAG_4 = 1 << 4,
        unk5 = 1 << 5,
        unk6 = 1 << 6,
        //UNUSED_FLAG_7 = 1 << 7,
    }
}