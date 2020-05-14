namespace GameCube.GFZX01.Camera
{
    [System.Flags]
    public enum CameraPanModifier : ushort
    {
        _1 = 1 << 0,
        _2 = 1 << 1,
        _3 = 1 << 2,
        _4 = 1 << 3,
        _5 = 1 << 4,
        _6 = 1 << 5,
        _7 = 1 << 6,
        _8 = 1 << 7,
        _9 = 1 << 8,
        _10 = 1 << 9,
        _11 = 1 << 10,
        Rotate10 = 1 << 11,
        Rotate30 = 1 << 12,
        Rotate45 = 1 << 13,
        Rotate90 = 1 << 14,
        Rotate180 = 1 << 15,
    }
}
