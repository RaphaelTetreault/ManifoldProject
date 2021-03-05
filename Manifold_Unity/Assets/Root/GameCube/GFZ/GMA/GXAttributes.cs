using GameCube.GX;

namespace GameCube.GFZ.GMA
{
    /// <summary>
    /// Flag form of Gx.GXAttr
    /// </summary>
    [System.Flags]
    public enum GXAttributes : uint
    {
        /// <summary>
        /// position/normal matrix index
        /// Note: used for character skinning
        /// </summary>
        GX_VA_PNMTXIDX = 1 << GXAttr.GX_VA_PNMTXIDX,
        /// <summary>
        /// texture 0 matrix index
        /// Note: used for character skinning
        /// </summary>
        GX_VA_TEX0MTXIDX = 1 << GXAttr.GX_VA_TEX0MTXIDX,
        /// <summary>
        /// texture 1 matrix index
        /// Note: used for character skinning
        /// </summary>
        GX_VA_TEX1MTXIDX = 1 << GXAttr.GX_VA_TEX1MTXIDX,
        /// <summary>
        /// texture 2 matrix index
        /// Note: used for character skinning
        /// </summary>
        GX_VA_TEX2MTXIDX = 1 << GXAttr.GX_VA_TEX2MTXIDX,
        /// <summary>
        /// texture 3 matrix index
        /// Note: used for character skinning
        /// </summary>
        GX_VA_TEX3MTXIDX = 1 << GXAttr.GX_VA_TEX3MTXIDX,
        /// <summary>
        /// texture 4 matrix index
        /// Note: used for character skinning
        /// </summary>
        GX_VA_TEX4MTXIDX = 1 << GXAttr.GX_VA_TEX4MTXIDX,
        /// <summary>
        /// texture 5 matrix index
        /// Note: used for character skinning
        /// </summary>
        GX_VA_TEX5MTXIDX = 1 << GXAttr.GX_VA_TEX5MTXIDX,
        /// <summary>
        /// texture 6 matrix index
        /// Note: used for character skinning
        /// </summary>
        GX_VA_TEX6MTXIDX = 1 << GXAttr.GX_VA_TEX6MTXIDX,
        /// <summary>
        /// texture 7 matrix index
        /// Note: used for character skinning
        /// </summary>
        GX_VA_TEX7MTXIDX = 1 << GXAttr.GX_VA_TEX7MTXIDX,

        /// <summary>
        /// position
        /// </summary>
        GX_VA_POS = 1 << GXAttr.GX_VA_POS,
        /// <summary>
        /// normal
        /// </summary>
        GX_VA_NRM = 1 << GXAttr.GX_VA_NRM,

        /// <summary>
        /// color 0
        /// </summary>
        GX_VA_CLR0 = 1 << GXAttr.GX_VA_CLR0,
        /// <summary>
        /// color 1
        /// </summary>
        GX_VA_CLR1 = 1 << GXAttr.GX_VA_CLR1,

        /// <summary>
        /// input texture coordinate 0
        /// </summary>
        GX_VA_TEX0 = 1 << GXAttr.GX_VA_TEX0,
        /// <summary>
        /// input texture coordinate 1
        /// </summary>
        GX_VA_TEX1 = 1 << GXAttr.GX_VA_TEX1,
        /// <summary>
        /// input texture coordinate 2
        /// </summary>
        GX_VA_TEX2 = 1 << GXAttr.GX_VA_TEX2,
        /// <summary>
        /// input texture coordinate 3
        /// </summary>
        GX_VA_TEX3 = 1 << GXAttr.GX_VA_TEX3,
        /// <summary>
        /// input texture coordinate 4
        /// </summary>
        GX_VA_TEX4 = 1 << GXAttr.GX_VA_TEX4,
        /// <summary>
        /// input texture coordinate 5
        /// </summary>
        GX_VA_TEX5 = 1 << GXAttr.GX_VA_TEX5,
        /// <summary>
        /// input texture coordinate 6
        /// </summary>
        GX_VA_TEX6 = 1 << GXAttr.GX_VA_TEX6,
        /// <summary>
        /// input texture coordinate 7
        /// </summary>
        GX_VA_TEX7 = 1 << GXAttr.GX_VA_TEX7,

        /// <summary>
        /// position matrix array pointer
        /// </summary>
        GX_VA_POS_MTX_ARRAY = 1 << GXAttr.GX_VA_POS_MTX_ARRAY,
        /// <summary>
        /// normal matrix array pointer
        /// </summary>
        GX_VA_NRM_MTX_ARRAY = 1 << GXAttr.GX_VA_NRM_MTX_ARRAY,
        /// <summary>
        /// texture matrix array pointer
        /// </summary>
        GX_VA_TEX_MTX_ARRAY = 1 << GXAttr.GX_VA_TEX_MTX_ARRAY,
        /// <summary>
        /// light matrix array pointer
        /// </summary>
        GX_VA_LIGHT_ARRAY = 1 << GXAttr.GX_VA_LIGHT_ARRAY,
        /// <summary>
        /// normal, bi-normal, tangent 
        /// </summary>
        GX_VA_NBT = 1 << GXAttr.GX_VA_NBT,
    }
}