using System.ComponentModel;

namespace GameCube.LibGxTexture
{
    /// <summary>
    /// Gx palette formats (along with TPL identifiers).
    /// </summary>
    public enum GxTexturePaletteFormat
    {
        [Description("Grayscale, 256 tones")]
        IA8 = 0,
        [Description("Color, 65536 colors")]
        RGB565 = 1,
        [Description("Color + Transparency, 65535 combinations")]
        RGB5A3 = 2
    };
}

