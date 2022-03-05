namespace GameCube.GFZ.GMA
{
    /// <summary>
    /// 
    /// </summary>
    [System.Flags]
    public enum TextureWrapMode : byte
    {
        unk0 = 1 << 0,
        unk1 = 1 << 1,
        repeatX = 1 << 2,
        mirrorX = 1 << 3,
        repeatY = 1 << 4,
        mirrorY = 1 << 5,
        unk6 = 1 << 6,
        unk7 = 1 << 7,
    }
}