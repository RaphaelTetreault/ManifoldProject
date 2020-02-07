﻿using System;

namespace GameCube.LibGxTexture
{
    /// <summary>IA4 (4 bits intensity / 4 bits alpha) pixel format.</summary>
    class GxTextureFormatCodecIA8 : GxTextureFormatCodec
    {
        public override int TileWidth
        {
            get { return 4; }
        }

        public override int TileHeight
        {
            get { return 4; }
        }

        public override int BitsPerPixel
        {
            get { return 16; }
        }

        public override int PaletteCount
        {
            get { return 0; }
        }

        public override bool IsSupported
        {
            get { return false; }
        }

        protected override void DecodeTile(byte[] dst, int dstPos, int stride, byte[] src, int srcPos)
        {
            throw new NotImplementedException();
        }

        protected override bool EncodingWantsGrayscale
        {
            get { throw new NotImplementedException(); }
        }

        protected override bool EncodingWantsDithering
        {
            get { throw new NotImplementedException(); }
        }

        protected override ColorRGBA TrimColor(ColorRGBA color)
        {
            throw new NotImplementedException();
        }

        protected override void EncodeTile(byte[] src, int srcPos, int stride, byte[] dst, int dstPos)
        {
            throw new NotImplementedException();
        }
    }
}
