namespace GameCube.LibGxTexture
{
    class GxTexturePaletteFormatCodecRGB5A3 : GxTexturePaletteFormatCodec
    {
        public override int BitsPerPixel
        {
            get { return 16; }
        }

        public override bool IsSupported
        {
            get { return false; }
        }
    }
}
