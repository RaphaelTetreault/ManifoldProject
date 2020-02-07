using UnityEngine;

namespace GameCube.LibGxTexture
{
    /// <summary>
    /// A lightweight structure for representing a color as its 8-bit color components.
    /// </summary>
    public struct ColorRGBA
    {
        /// <summary>The red component (0-255) of the color.</summary>
        public byte r;
        /// <summary>The green component (0-255) of the color.</summary>
        public byte g;
        /// <summary>The blue component (0-255) of the color.</summary>
        public byte b;
        /// <summary>The alpha component (0-255) of the color.</summary>
        public byte a;

        // Enable conversion between this and Unity colors
        public static explicit operator ColorRGBA(Color32 c)
        {
            return new ColorRGBA(c.r, c.g, c.b, c.a);
        }
        public static explicit operator Color32(ColorRGBA c)
        {
            return new Color32(c.r, c.g, c.b, c.a);
        }
        public static explicit operator ColorRGBA(Color c)
        {
            Color32 c32 = c;
            return new ColorRGBA(c32.r, c32.g, c32.b, c32.a);
        }
        public static explicit operator Color(ColorRGBA c)
        {
            return new Color32(c.r, c.g, c.b, c.a);
        }

        /// <summary>Create a RGBA color from its color components.</summary>
        /// <param name="red">The red component (0-255) of the color.</param>
        /// <param name="green">The green component (0-255) of the color.</param>
        /// <param name="blue">The blue component (0-255) of the color.</param>
        /// <param name="alpha">The alpha component (0-255) of the color.</param>
        public ColorRGBA(byte red, byte green, byte blue, byte alpha)
        {
            this.r = red;
            this.g = green;
            this.b = blue;
            this.a = alpha;
        }

        /// <summary>Creates a color from its intensity (grayscale) and alpha components.</summary>
        /// <param name="intensity">The intensity component (0-255) of the color.</param>
        /// <param name="alpha">The alpha component (0-255) of the color.</param>
        public static ColorRGBA FromIntensityAlpha(byte intensity, byte alpha)
        {
            return new ColorRGBA(intensity, intensity, intensity, alpha);
        }


        /// <summary>Get the grayscale intensity of this color.</summary>
        /// <returns>The grayscale intensity of this color.</returns>
        public byte Intensity()
        {
            // 30% red, 59% green, 11% blue is the standard way to calculate the intensity of a color
            return (byte)((r * 0.30) + (g * 0.59) + (b * 0.11));
        }

        /// <summary>
        /// Read a RGBA8 color from a byte array.
        /// </summary>
        /// <param name="src">The source byte array to read the color from.</param>
        /// <param name="srcIndex">The initial position in the source byte array.</param>
        /// <returns>An instance of the color read from the byte array.</returns>
        public static ColorRGBA Read(byte[] src, int srcIndex)
        {
            return new ColorRGBA(src[srcIndex + 0], src[srcIndex + 1], src[srcIndex + 2], src[srcIndex + 3]);
        }

        /// <summary>
        /// Write a RGBA8 color to a byte array.
        /// </summary>
        /// <param name="dst">The destination byte array to write the color to.</param>
        /// <param name="dstIndex">The initial position in the destination byte array.</param>
        public void Write(byte[] dst, int dstIndex)
        {
            dst[dstIndex + 0] = r;
            dst[dstIndex + 1] = g;
            dst[dstIndex + 2] = b;
            dst[dstIndex + 3] = a;
        }
    };
}
