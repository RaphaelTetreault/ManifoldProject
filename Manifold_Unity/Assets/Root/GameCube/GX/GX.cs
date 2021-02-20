namespace GameCube.GX
{
    /// <summary>
    /// Primitive type.
    /// </summary>
    public enum GXPrimitive : byte
    {
        GX_QUADS                    = 0x80, // 0b10000000
        GX_TRIANGLES                = 0x90, // 0b10010000
        GX_TRIANGLESTRIP            = 0x98, // 0b10011000
        GX_TRIANGLEFAN              = 0xA0, // 0b10100000
        GX_LINES                    = 0xA8, // 0b10101000
        GX_LINESTRIP                = 0xB0, // 0b10110000
        GX_POINTS                   = 0xB8, // 0b10111000
    }

    /// <summary>
    /// Vertex format number.
    /// </summary>
    public enum GXVtxFmt
    {
        GX_VTXFMT0 = 0,
        GX_VTXFMT1,
        GX_VTXFMT2,
        GX_VTXFMT3,
        GX_VTXFMT4,
        GX_VTXFMT5,
        GX_VTXFMT6,
        GX_VTXFMT7,
        GX_MAX_VTXFMT,
    }

    /// <summary>
    /// GX Component Type
    /// Related to GXVtxFmt
    /// </summary>
    public enum GXCompType
    {
        /// <summary>
        /// Unsigned 8-bit integer
        /// </summary>
        GX_U8 = 0,
        /// <summary>
        /// Signed 8-bit integer
        /// </summary>
        GX_S8,
        /// <summary>
        /// Unsigned 16-bit integer
        /// </summary>
        GX_U16,
        /// <summary>
        /// Signed 16-bit integer
        /// </summary>
        GX_S16,
        /// <summary>
        /// 32-bit floating-point
        /// </summary>
        GX_F32,

        /// <summary>
        /// 16-bit RGB
        /// </summary>
        GX_RGB565 = 0,
        /// <summary>
        /// 24-bit RGB
        /// </summary>
        GX_RGB8,
        /// <summary>
        /// 32-bit RGBX
        /// </summary>
        GX_RGBX8,
        /// <summary>
        /// 16-bit RGBA
        /// </summary>
        GX_RGBA4,
        /// <summary>
        /// 24-bit RGBA
        /// </summary>
        GX_RGBA6,
        /// <summary>
        /// 32-bit RGBA
        /// </summary>
        GX_RGBA8,
    }

    /// <summary>
    /// GX Component Count
    /// </summary>
    public enum GXCompCnt_Rev2
    {
        /// <summary>
        /// X,Y position
        /// </summary>
        GX_POS_XY = 0,
        /// <summary>
        /// X,Y,Z position
        /// </summary>
        GX_POS_XYZ = 1,


        /// <summary>
        /// X,Y,Z normal
        /// </summary>
        GX_NRM_XYZ = 0,
        /// <summary>
        /// Normal, Binormal, Tangent
        /// one index per NBT
        /// </summary>
        GX_NRM_NBT = 1,
        /// <summary>
        /// Normal, Binormal, Tangent x3 (HW2 only)
        /// one index per each of N/B/T
        /// </summary>
        GX_NRM_NBT3 = 2,

        /// <summary>
        /// RGB color
        /// </summary>
        GX_CLR_RGB = 0,
        /// <summary>
        /// RGBA color
        /// </summary>
        GX_CLR_RGBA = 1,

        /// <summary>
        /// One texture dimension
        /// </summary>
        GX_TEX_S = 0,
        /// <summary>
        /// Two texture dimensions
        /// </summary>
        GX_TEX_ST = 1,
    }

    /// <summary>
    /// Name of vertex attribute or array. Attributes are listed in the ascending order vertex data is required to be sent to the GP.
    /// 
    /// Notes:
    /// Tells GX what to expect from oncoming vertex information.
    /// That data provided should be 32-byte aligned. Refer to GX FIFO.
    /// </summary>
    public enum GXAttr
    {
        /// <summary>
        /// position/normal matrix index
        /// </summary>
        GX_VA_PNMTXIDX = 0,
        /// <summary>
        /// texture 0 matrix index
        /// </summary>
        GX_VA_TEX0MTXIDX,
        /// <summary>
        /// texture 1 matrix index
        /// </summary>
        GX_VA_TEX1MTXIDX,
        /// <summary>
        /// texture 2 matrix index
        /// </summary>
        GX_VA_TEX2MTXIDX,
        /// <summary>
        /// texture 3 matrix index
        /// </summary>
        GX_VA_TEX3MTXIDX,
        /// <summary>
        /// texture 4 matrix index
        /// </summary>
        GX_VA_TEX4MTXIDX,
        /// <summary>
        /// texture 5 matrix index
        /// </summary>
        GX_VA_TEX5MTXIDX,
        /// <summary>
        /// texture 6 matrix index
        /// </summary>
        GX_VA_TEX6MTXIDX,
        /// <summary>
        /// texture 7 matrix index
        /// </summary>
        GX_VA_TEX7MTXIDX,

        /// <summary>
        /// position
        /// </summary>
        GX_VA_POS,
        /// <summary>
        /// normal
        /// </summary>
        GX_VA_NRM,

        /// <summary>
        /// color 0
        /// </summary>
        GX_VA_CLR0,
        /// <summary>
        /// color 1
        /// </summary>
        GX_VA_CLR1,

        /// <summary>
        /// input texture coordinate 0
        /// </summary>
        GX_VA_TEX0,
        /// <summary>
        /// input texture coordinate 1
        /// </summary>
        GX_VA_TEX1,
        /// <summary>
        /// input texture coordinate 2
        /// </summary>
        GX_VA_TEX2,
        /// <summary>
        /// input texture coordinate 3
        /// </summary>
        GX_VA_TEX3,
        /// <summary>
        /// input texture coordinate 4
        /// </summary>
        GX_VA_TEX4,
        /// <summary>
        /// input texture coordinate 5
        /// </summary>
        GX_VA_TEX5,
        /// <summary>
        /// input texture coordinate 6
        /// </summary>
        GX_VA_TEX6,
        /// <summary>
        /// input texture coordinate 7
        /// </summary>
        GX_VA_TEX7,

        /// <summary>
        /// position matrix array pointer
        /// </summary>
        GX_VA_POS_MTX_ARRAY,
        /// <summary>
        /// normal matrix array pointer
        /// </summary>
        GX_VA_NRM_MTX_ARRAY,
        /// <summary>
        /// texture matrix array pointer
        /// </summary>
        GX_VA_TEX_MTX_ARRAY,
        /// <summary>
        /// light matrix array pointer
        /// </summary>
        GX_VA_LIGHT_ARRAY,
        /// <summary>
        /// normal, bi-normal, tangent 
        /// </summary>
        GX_VA_NBT,
        /// <summary>
        /// maximum number of vertex attributes
        /// </summary>
        GX_VA_MAX_ATTR,

        /// <summary>
        /// NULL attribute (to mark end of lists)
        /// </summary>
        GX_VA_NULL = 0xff,
    }

}