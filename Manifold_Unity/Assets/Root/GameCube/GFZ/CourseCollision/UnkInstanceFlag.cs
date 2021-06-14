namespace GameCube.GFZ.CourseCollision
{
    // NOTES:
    // Unique combinations:
    //   0: none.
    //   1: bit0.
    //   3: bit1 + bit0.
    //  64: bit6.
    //  65: bit6 + bit0.
    // 193: bit7 + bit0.
    // 194: bit7 + bit1.
    // 195: bit7 + bit1 + bit0.
    // 256: bit8.

    /// <summary>
    /// 
    /// </summary>
    [System.Flags]
    public enum UnkInstanceFlag : uint
    {
        none = 0,
        bit0 = 1 << 0, //   1
        bit1 = 1 << 1, //   2
        bit6 = 1 << 6, //  64
        bit7 = 1 << 7, // 128
        bit8 = 1 << 8, // 256
    }
}
