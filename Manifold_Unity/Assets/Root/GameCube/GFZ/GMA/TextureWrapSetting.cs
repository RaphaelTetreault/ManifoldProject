namespace GameCube.GFZ.GMA
{
    [System.Flags]
    public enum TextureWrapSetting : byte
    {
        UNK_FLAG_0 = 1 << 0,
        UNK_FLAG_1 = 1 << 1,
        REPEAT_X = 1 << 2,
        MIRROR_X = 1 << 3,
        REPEAT_Y = 1 << 4,
        MIRROR_Y = 1 << 5,
        UNK_FLAG_6 = 1 << 6,
        UNK_FLAG_7 = 1 << 7,
    }
}