// GXAttr
namespace GameCube.GX
{
    /// <summary>
    /// Name of vertex attribute or array. Attributes are listed in the ascending order vertex data is required to be sent to the GP.
    /// 
    /// Notes:
    /// Tells GX what to expect from oncoming vertex information.
    /// That data provided should be 32-byte aligned. Refer to GX FIFO.
    /// </summary>
    public enum Attribute
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
