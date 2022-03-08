// GXPrimitive
namespace GameCube.GX
{
    /// <summary>
    /// Primitive type.
    /// </summary>
    public enum Primitive : byte
    {
        GX_QUADS            = 0x80, // 0b10000000
        GX_TRIANGLES        = 0x90, // 0b10010000
        GX_TRIANGLESTRIP    = 0x98, // 0b10011000
        GX_TRIANGLEFAN      = 0xA0, // 0b10100000
        GX_LINES            = 0xA8, // 0b10101000
        GX_LINESTRIP        = 0xB0, // 0b10110000
        GX_POINTS           = 0xB8, // 0b10111000
    }
}
