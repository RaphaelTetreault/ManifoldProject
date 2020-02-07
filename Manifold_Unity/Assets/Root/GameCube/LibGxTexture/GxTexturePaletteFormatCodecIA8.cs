namespace GameCube.LibGxTexture
{
    class GxTexturePaletteFormatCodecIA8 : GxTexturePaletteFormatCodec
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
