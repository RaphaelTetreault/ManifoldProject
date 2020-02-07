using System;

namespace GameCube.LibGxTexture
{
    /// <summary>
    /// Base class to be implemented by all texture palette formats supported by GxTexture.
    /// </summary>
    abstract class GxTexturePaletteFormatCodec
    {
        /// <summary>Number of bits used by every color of a palette in the specified texture palette format.</summary>
        public abstract int BitsPerPixel { get; }

        /// <summary>true if encoding and decoding from/to this format is supported.</summary>
        public abstract bool IsSupported { get; }

        /// <summary>Get the codec associated to the given texture palette format.</summary>
        /// <param name="palFmt">The texture palette format.</param>
        /// <returns>The codec associated to the given texture palette format.</returns>
        public static GxTexturePaletteFormatCodec GetCodec(GxTexturePaletteFormat palFmt)
        {
            switch (palFmt)
            {
                case GxTexturePaletteFormat.IA8: return new GxTexturePaletteFormatCodecIA8();
                case GxTexturePaletteFormat.RGB565: return new GxTexturePaletteFormatCodecRGB565();
                case GxTexturePaletteFormat.RGB5A3: return new GxTexturePaletteFormatCodecRGB5A3();
                default:
                    throw new ArgumentOutOfRangeException("palFmt");
            }
        }
    }
}
