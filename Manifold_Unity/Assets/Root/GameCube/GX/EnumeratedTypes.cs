/// <summary>
/// A C# equivalent of GXEnums.h from the Nintendo GameCube SDK.
/// Comments taken from Nintendo GameCube SDK manual and code GXEnums.h comments.
/// </summary>
namespace GameCube.GX
{
    // A
    /// <summary>
    /// Alpha combine control.
    /// </summary>
    public enum GXAlphaOp
    {
        GX_AOP_AND,
        GX_AOP_OR,
        GX_AOP_XOR,
        GX_AOP_XNOR,
        GX_MAX_ALPHAOP,
    }

    /// <summary>
    /// Texture Environment (Tev) control.
    /// </summary>
    public enum GXAlphaReadMode
    {
        /// <summary>
        /// Always read 0x00
        /// </summary>
        GX_READ_00,
        /// <summary>
        /// Always read 0xFF
        /// </summary>
        GX_READ_FF,
        /// <summary>
        /// Always read the real alpha value
        /// </summary>
        GX_READ_NONE,
    }

    /// <summary>
    /// Maximum anisotropic filter control.
    /// </summary>
    public enum GXAnisotropy
    {
        GX_ANISO_1,
        GX_ANISO_2,
        GX_ANISO_4,
        GX_MAX_ANISOTROPY,
    }

    /// <summary>
    /// Lighting attenuation control.
    /// </summary>
    public enum GXAttnFn
    {
        /// <summary>
        /// use specular attenuation
        /// </summary>
        GX_AF_SPEC = 0,
        /// <summary>
        /// use distance/spotlight attenuation
        /// </summary>
        GX_AF_SPOT = 1,
        /// <summary>
        /// attenuation is off
        /// </summary>
        GX_AF_NONE,
    }

    /// <summary>
    /// Name of vertex attribute or array. Attributes are listed in the ascending order vertex data is required to be sent to the GP.
    /// 
    /// Notes:
    /// Tells GX what to expect from oncoming vertex information.
    /// The data provided should be 32-byte aligned. Refer to GX FIFO.
    /// 
    /// There appears to be conflict between this and some information in the 
    /// "Vertex and primitive data" Nintendo SDK manual. The manual says
    /// GX_VA_NRM and GX_VA_NBT both share a value of 10, but that's not what
    /// the enum here had, and I recall copying from the SDK enum script.
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

    /// <summary>
    /// Type of attribute reference.
    /// </summary>
    public enum GXAttrType
    {
        GX_NONE,
        GX_DIRECT,
        GX_INDEX8,
        GX_INDEX16,
    }

    // B
    /// <summary>
    /// Blending controls.
    /// </summary>
    public enum GXBlendFactor
    {
        /// <summary>
        /// 0.0
        /// </summary>
        GX_BL_ZERO,
        /// <summary>
        /// 1.0
        /// </summary>
        GX_BL_ONE,
        /// <summary>
        /// Source color
        /// </summary>
        GX_BL_SRCCLR,
        /// <summary>
        /// 1.0 - (source color)
        /// </summary>
        GX_BL_INVSRCCLR,
        /// <summary>
        /// source alpha
        /// </summary>
        GX_BL_SRCALPHA,
        /// <summary>
        /// 1.0 - (source alpha)
        /// </summary>
        GX_BL_INVSRCALPHA,
        /// <summary>
        /// FrameBuffer alpha
        /// </summary>
        GX_BL_DSTALPHA,
        /// <summary>
        /// 1.0 - (FrameBuffer alpha)
        /// </summary>
        GX_BL_INVDSTALPHA,

        /// <summary>
        /// Same as GX_BL_SRCCLR. Source color
        /// </summary>
        GX_BL_DSTCLR = GX_BL_SRCCLR,
        /// <summary>
        /// Same as GX_BL_INVSRCCLR. 1.0 (source color)
        /// </summary>
        GX_BL_INVDSTCLR = GX_BL_INVSRCCLR,
    }

    /// <summary>
    /// Blending type.
    /// </summary>
    public enum GXBlendMode_Rev1
    {
        /// <summary>
        /// Write input directly to EFB
        /// </summary>
        GX_BM_NONE,
        /// <summary>
        /// Blend using blending equation
        /// </summary>
        GX_BM_BLEND,
        /// <summary>
        /// Blend using bitwise operation
        /// </summary>
        GX_BM_LOGIC,
        GX_MAX_BLENDMODE,
    }

    /// <summary>
    /// Blending type.
    /// </summary>
    public enum GXBlendMode_Rev2
    {
        /// <summary>
        /// Write input directly to EFB
        /// </summary>
        GX_BM_NONE,
        /// <summary>
        /// Blend using blending equation
        /// </summary>
        GX_BM_BLEND,
        /// <summary>
        /// Blend using bitwise operation
        /// </summary>
        GX_BM_LOGIC,
        /// <summary>
        /// (HW2 only)
        /// Input subtracts from existing pixel
        /// </summary>
        GX_BM_SUBTRACT,
        GX_MAX_BLENDMODE,
    }

    /// <summary>
    /// Boolean type.
    /// </summary>
    public enum GXBool
    {
        GX_FALSE = 0,
        GX_TRUE = 1,
        GX_DISABLE = 0,
        GX_ENABLE = 1,
    }

    // C
    /// <summary>
    /// Name of color channel used for lighting and to specify raster color input to a Tev stage.
    /// </summary>
    public enum GXChannelID
    {
        GX_COLOR0 = 0,
        GX_COLOR1,
        GX_ALPHA0,
        GX_ALPHA1,
        /// <summary>
        /// Color 0 + Alpha 0
        /// </summary>
        GX_COLOR0A0,
        /// <summary>
        /// Color 1 + Alpha 1
        /// </summary>
        GX_COLOR1A1,
        /// <summary>
        /// RGBA = 0
        /// </summary>
        GX_COLOR_ZERO,
        /// <summary>
        /// bump alpha 0-248, RGB=0
        /// </summary>
        GX_ALPHA_BUMP,
        /// <summary>
        /// normalized bump alpha, 0-255, RGB=0
        /// </summary>
        GX_ALPHA_BUMPN,

        GX_COLOR_NULL = 0xFF,
    }
    /// <summary>
    /// Color index texture format types.
    /// </summary>
    public enum GXCITexFmt
    {
        GX_TF_C4 = 0x8,
        GX_TF_C8 = 0x9,
        GX_TF_C14X2 = 0xa,
    }

    /// <summary>
    /// Clipping modes. Note that they are backwards of the typical enable/disable enums. This is by design.
    /// </summary>
    public enum GXClipMode
    {
        // Note: these are (by design) backwards of typical enable/disables!
        GX_CLIP_ENABLE = 0,
        GX_CLIP_DISABLE = 1,
    }

    /// <summary>
    /// Source of incoming color.
    /// </summary>
    public enum GXColorSrc
    {
        GX_SRC_REG = 0,
        GX_SRC_VTX,
    }

    /// <summary>
    /// Compare types.
    /// </summary>
    public enum GXCompare
    {
        GX_NEVER,
        GX_LESS,
        GX_EQUAL,
        GX_LEQUAL,
        GX_GREATER,
        GX_NEQUAL,
        GX_GEQUAL,
        GX_ALWAYS
    }

    /// <summary>
    /// GX Component Count
    /// </summary>
    public enum GXCompCnt_Rev1
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
        /// not really used, just to make api consistent
        /// </summary>
        GX_NRM_NBT = 1,


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
    /// Controls whether all lines, only even lines, or only odd lines are copied from the EFB
    /// </summary>
    public enum GXCopyMode
    {
        GX_COPY_PROGRESSIVE = 0,
        GX_COPY_INTLC_EVEN = 2,
        GX_COPY_INTLC_ODD = 3,
    }

    /// <summary>
    /// Backface culling modes.
    /// </summary>
    public enum GXCullMode
    {
        GX_CULL_NONE,
        GX_CULL_FRONT,
        GX_CULL_BACK,
        GX_CULL_ALL,
    }

    // D
    /// <summary>
    /// DiffuseFunction
    /// </summary>
    public enum GXDiffuseFn
    {
        GX_DF_NONE = 0,
        GX_DF_SIGN,
        GX_DF_CLAMP,
    }

    /// <summary>
    /// Type of the brightness decreasing function by distance.
    /// </summary>
    public enum GXDistAttnFn
    {
        GX_DA_OFF,
        GX_DA_GENTLE,
        GX_DA_MEDIUM,
        GX_DA_STEEP,
    }

    // F
    /// <summary>
    /// Frame buffer clamp modes on copy.
    /// </summary>
    public enum GXFBClamp
    {
        GX_CLAMP_NONE,
        GX_CLAMP_TOP,
        GX_CLAMP_BOTTOM,
    }

    /// <summary>
    /// Fog equation control.
    /// </summary>
    public enum GXFogType
    {
        GX_FOG_NONE = 0,
        GX_FOG_LIN = 2,
        GX_FOG_EXP = 4,
        GX_FOG_EXP2 = 5,
        GX_FOG_REVEXP = 6,
        GX_FOG_REVEXP2 = 7,
    }

    // G
    /// <summary>
    /// Gamma values.
    /// </summary>
    public enum GXGamma
    {
        GX_GM_1_0,
        GX_GM_1_7,
        GX_GM_2_2,
    }

    // I
    /// <summary>
    /// Indirect texture "bump" alpha select.  Indicates which offset component should provide the "bump" alpha output for the given TEV stage.  Bump alpha is not available for TEV stage 0.
    /// </summary>
    public enum GXIndTexAlphaSel
    {
        GX_ITBA_OFF,
        GX_ITBA_S,
        GX_ITBA_T,
        GX_ITBA_U,
        GX_MAX_ITBALPHA,
    }
    /// <summary>
    /// Indirect texture bias select.  Indicates which components of the indirect offset should receive a bias value.  The bias is fixed at -128 for GX_ITF_8 and +1 for the other formats.  The bias happens prior to the indirect matrix multiply.
    /// </summary>
    public enum GXIndTexBiasSel
    {
        GX_ITB_NONE,
        GX_ITB_S,
        GX_ITB_T,
        GX_ITB_ST,
        GX_ITB_U,
        GX_ITB_SU,
        GX_ITB_TU,
        GX_ITB_STU,
        GX_MAX_ITBIAS,
    }

    /// <summary>
    /// Indirect texture formats.  Bits for the indirect offsets are extracted from the high end of each component byte.  Bits for the bump alpha are extraced off the low end of the byte.  For GX_ITF_8, the byte is duplicated for the offset and the bump alpha.
    /// </summary>
    public enum GXIndTexFormat
    {
        /// <summary>
        /// 8 bit texture offsets
        /// </summary>
        GX_ITF_8,
        /// <summary>
        /// 5 bit texture offsets
        /// </summary>
        GX_ITF_5,
        /// <summary>
        /// 4 bit texture offsets
        /// </summary>
        GX_ITF_4,
        /// <summary>
        /// 3 bit texture offsets
        /// </summary>
        GX_ITF_3,
        GX_MAX_ITFORMAT,
    }

    /// <summary>
    /// Indirect texture stage ID.  Specifies which of the four indirect hardware stages to use.
    /// </summary>
    public enum GXIndTexStageID
    {
        /// <summary>
        /// Indirect Texture Stage Names
        /// </summary>
        GX_INDTEXSTAGE0,
        GX_INDTEXSTAGE1,
        GX_INDTEXSTAGE2,
        GX_INDTEXSTAGE3,
        GX_MAX_INDTEXSTAGE,
    }

    /// <summary>
    /// Indirect texture wrap value.  This indicates whether the regular texture coordinate should be wrapped before being added to the offset.  GX_ITW_OFF specifies no wrapping.  GX_ITW_0 will zero out the regular texture coordinate.
    /// </summary>
    public enum GXIndTexWrap
    {
        /// <summary>
        /// no wrapping
        /// </summary>
        GX_ITW_OFF,
        /// <summary>
        /// wrap 256
        /// </summary>
        GX_ITW_256,
        /// <summary>
        /// wrap 128
        /// </summary>
        GX_ITW_128,
        /// <summary>
        /// wrap 64
        /// </summary>
        GX_ITW_64,
        /// <summary>
        /// wrap 32
        /// </summary>
        GX_ITW_32,
        /// <summary>
        /// wrap 16
        /// </summary>
        GX_ITW_16,
        /// <summary>
        /// wrap 0
        /// </summary>
        GX_ITW_0,
        GX_MAX_ITWRAP,
    }

    /// <summary>
    /// Indirect texture scale value.  Specifies an additional scale value that may be applied to the texcoord used for an indirect initial lookup (not a TEV stage regular lookup).  The scale value is a fraction; thus GX_ITS_32 means to divide the texture coordinate values by 32.
    /// </summary>
    public enum GXIndTexScale
    {
        /// <summary>
        /// Scale by 1
        /// </summary>
        GX_ITS_1,
        /// <summary>
        /// Scale by 1/2
        /// </summary>
        GX_ITS_2,
        /// <summary>
        /// Scale by 1/4
        /// </summary>
        GX_ITS_4,
        /// <summary>
        /// Scale by 1/8
        /// </summary>
        GX_ITS_8,
        /// <summary>
        /// Scale by 1/16
        /// </summary>
        GX_ITS_16,
        /// <summary>
        /// Scale by 1/32
        /// </summary>
        GX_ITS_32,
        /// <summary>
        /// Scale by 1/64
        /// </summary>
        GX_ITS_64,
        /// <summary>
        /// Scale by 1/128
        /// </summary>
        GX_ITS_128,
        /// <summary>
        /// Scale by 1/256
        /// </summary>
        GX_ITS_256,
        GX_MAX_ITSCALE,
    }

    /// <summary>
    /// Indirect texture matrix ID.  Indicates which indirect texture matrix and associated scale value should be used for a given TEV stage offset computation.   Three static matrices are available as well as two types of dynamic matrices.   Each dynamic matrix shares the scale values used with the static matrices.
    /// </summary>
    public enum GXIndTexMtxID
    {
        /// <summary>
        /// Specifies a matrix of all zeros.
        /// </summary>
        GX_ITM_OFF,
        /// <summary>
        /// Specifies indirect matrix 0, indirect scale 0.
        /// </summary>
        GX_ITM_0,
        /// <summary>
        /// Specifies indirect matrix 1, indirect scale 1.
        /// </summary>
        GX_ITM_1,
        /// <summary>
        /// Specifies indirect matrix 2, indirect scale 2.
        /// </summary>
        GX_ITM_2,
        /// <summary>
        /// Specifies dynamic S-type matrix, indirect scale 0.
        /// </summary>
        GX_ITM_S0 = 5,
        /// <summary>
        /// Specifies dynamic S-type matrix, indirect scale 1.
        /// </summary>
        GX_ITM_S1,
        /// <summary>
        /// Specifies dynamic S-type matrix, indirect scale 2.
        /// </summary>
        GX_ITM_S2,
        /// <summary>
        /// Specifies dynamic T-type matrix, indirect scale 0.
        /// </summary>
        GX_ITM_T0 = 9,
        /// <summary>
        /// Specifies dynamic T-type matrix, indirect scale 1.
        /// </summary>
        GX_ITM_T1,
        /// <summary>
        /// Specifies dynamic T-type matrix, indirect scale 2.
        /// </summary>
        GX_ITM_T2,
    }

    // L
    /// <summary>
    /// Name of light.
    /// </summary>
    public enum GXLightID
    {
        GX_LIGHT0 = 0x001,
        GX_LIGHT1 = 0x002,
        GX_LIGHT2 = 0x004,
        GX_LIGHT3 = 0x008,
        GX_LIGHT4 = 0x010,
        GX_LIGHT5 = 0x020,
        GX_LIGHT6 = 0x040,
        GX_LIGHT7 = 0x080,
        GX_MAX_LIGHT = 0x100,
        GX_LIGHT_NULL = 0x000,
    }

    /// <summary>
    /// WARNING: UNSURE IF ORDER IS CORRECT, IT DIFFERS FROM liboGC
    /// </summary>
    public enum GXLogicOp
    {
        GX_LO_CLEAR,
        GX_LO_AND,
        GX_LO_REVAND,
        GX_LO_COPY,
        GX_LO_INVAND,
        GX_LO_NOOP,
        GX_LO_XOR,
        GX_LO_OR,
        GX_LO_NOR,
        GX_LO_EQUIV,
        GX_LO_INV,
        GX_LO_REVOR,
        GX_LO_INVCOPY,
        GX_LO_INVOR,
        GX_LO_NAND,
        GX_LO_SET
    }

    // M
    /// <summary>
    /// Miscellaneous control setting values.
    /// </summary>
    public enum GXMiscToken
    {
        GX_MT_XF_FLUSH = 1,
        GX_MT_DL_SAVE_CONTEXT = 2,
        GX_MT_NULL = 0,
    }

    // P
    /// <summary>
    /// Performance counter 0 metrics.
    /// </summary>
    public enum GXPerf0
    {
        GX_PERF0_VERTICES,
        GX_PERF0_CLIP_VTX,
        GX_PERF0_CLIP_CLKS,
        GX_PERF0_XF_WAIT_IN,
        GX_PERF0_XF_WAIT_OUT,
        GX_PERF0_XF_XFRM_CLKS,
        GX_PERF0_XF_LIT_CLKS,
        GX_PERF0_XF_BOT_CLKS,
        GX_PERF0_XF_REGLD_CLKS,
        GX_PERF0_XF_REGRD_CLKS,
        GX_PERF0_CLIP_RATIO,

        GX_PERF0_TRIANGLES,
        GX_PERF0_TRIANGLES_CULLED,
        GX_PERF0_TRIANGLES_PASSED,
        GX_PERF0_TRIANGLES_SCISSORED,
        GX_PERF0_TRIANGLES_0TEX,
        GX_PERF0_TRIANGLES_1TEX,
        GX_PERF0_TRIANGLES_2TEX,
        GX_PERF0_TRIANGLES_3TEX,
        GX_PERF0_TRIANGLES_4TEX,
        GX_PERF0_TRIANGLES_5TEX,
        GX_PERF0_TRIANGLES_6TEX,
        GX_PERF0_TRIANGLES_7TEX,
        GX_PERF0_TRIANGLES_8TEX,
        GX_PERF0_TRIANGLES_0CLR,
        GX_PERF0_TRIANGLES_1CLR,
        GX_PERF0_TRIANGLES_2CLR,

        GX_PERF0_QUAD_0CVG,
        GX_PERF0_QUAD_NON0CVG,
        GX_PERF0_QUAD_1CVG,
        GX_PERF0_QUAD_2CVG,
        GX_PERF0_QUAD_3CVG,
        GX_PERF0_QUAD_4CVG,
        GX_PERF0_AVG_QUAD_CNT,

        GX_PERF0_CLOCKS,
        GX_PERF0_NONE,
    }

    /// <summary>
    /// Performance counter 1 metrics.
    /// </summary>
    public enum GXPerf1
    {
        GX_PERF1_TEXELS,
        GX_PERF1_TX_IDLE,
        GX_PERF1_TX_REGS,
        GX_PERF1_TX_MEMSTALL,
        GX_PERF1_TC_CHECK1_2,
        GX_PERF1_TC_CHECK3_4,
        GX_PERF1_TC_CHECK5_6,
        GX_PERF1_TC_CHECK7_8,
        GX_PERF1_TC_MISS,

        GX_PERF1_VC_ELEMQ_FULL,
        GX_PERF1_VC_MISSQ_FULL,
        GX_PERF1_VC_MEMREQ_FULL,
        GX_PERF1_VC_STATUS7,
        GX_PERF1_VC_MISSREP_FULL,
        GX_PERF1_VC_STREAMBUF_LOW,
        GX_PERF1_VC_ALL_STALLS,
        GX_PERF1_VERTICES,

        GX_PERF1_FIFO_REQ,
        GX_PERF1_CALL_REQ,
        GX_PERF1_VC_MISS_REQ,
        GX_PERF1_CP_ALL_REQ,

        GX_PERF1_CLOCKS,
        GX_PERF1_NONE,
    }

    /// <summary>
    /// Frame buffer pixel formats.
    /// </summary>
    public enum GXPixelFmt
    {
        GX_PF_RGB8_Z24,
        GX_PF_RGBA6_Z24,
        GX_PF_RGB565_Z16,
        GX_PF_Z24,
        GX_PF_Y8,
        GX_PF_U8,
        GX_PF_V8,
        GX_PF_YUV420,
    }

    /// <summary>
    /// Position-Normal matrix index.
    /// </summary>
    public enum GXPosNrmMtx
    {
        GX_PNMTX0 = 0,
        GX_PNMTX1 = 3,
        GX_PNMTX2 = 6,
        GX_PNMTX3 = 9,
        GX_PNMTX4 = 12,
        GX_PNMTX5 = 15,
        GX_PNMTX6 = 18,
        GX_PNMTX7 = 21,
        GX_PNMTX8 = 24,
        GX_PNMTX9 = 27,

    }

    /// <summary>
    /// Primitive type.
    /// </summary>
    public enum GXPrimitive
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
    /// Projection type.
    /// </summary>
    public enum GXProjectionType
    {
        GX_PERSPECTIVE,
        GX_ORTHOGRAPHIC,
    }

    /// <summary>
    /// Post-transform texture matrix index.
    /// Revision 2 or greater only
    /// </summary>
    public enum GXPTTexMtx
    {
        GX_PTTEXMTX0 = 64,
        GX_PTTEXMTX1 = 67,
        GX_PTTEXMTX2 = 70,
        GX_PTTEXMTX3 = 73,
        GX_PTTEXMTX4 = 76,
        GX_PTTEXMTX5 = 79,
        GX_PTTEXMTX6 = 82,
        GX_PTTEXMTX7 = 85,
        GX_PTTEXMTX8 = 88,
        GX_PTTEXMTX9 = 91,
        GX_PTTEXMTX10 = 94,
        GX_PTTEXMTX11 = 97,
        GX_PTTEXMTX12 = 100,
        GX_PTTEXMTX13 = 103,
        GX_PTTEXMTX14 = 106,
        GX_PTTEXMTX15 = 109,
        GX_PTTEXMTX16 = 112,
        GX_PTTEXMTX17 = 115,
        GX_PTTEXMTX18 = 118,
        GX_PTTEXMTX19 = 121,
        GX_PTIDENTITY = 125,
    }

    // S
    /// <summary>
    /// Type of the function for the illumination distribution on spotlight.
    /// </summary>
    public enum GXSpotFn
    {
        GX_SP_OFF = 0,
        GX_SP_FLAT,
        GX_SP_COS,
        GX_SP_COS2,
        GX_SP_SHARP,
        GX_SP_RING1,
        GX_SP_RING2,
    }

    // T
    /// <summary>
    /// Texture Environment (Tev) input control.
    /// </summary>
    public enum GXTevAlphaArg_Rev1
    {
        GX_CA_APREV,
        GX_CA_A0,
        GX_CA_A1,
        GX_CA_A2,
        GX_CA_TEXA,
        GX_CA_RASA,
        GX_CA_ONE,
        GX_CA_ZERO,
    }

    /// <summary>
    /// Texture Environment (Tev) input control.
    /// </summary>
    public enum GXTevAlphaArg_Rev2
    {
        GX_CA_APREV,
        GX_CA_A0,
        GX_CA_A1,
        GX_CA_A2,
        GX_CA_TEXA,
        GX_CA_RASA,
        GX_CA_KONST,
        GX_CA_ZERO,
    }

    /// <summary>
    /// Tev bias values.
    /// </summary>
    public enum GXTevBias
    {
        GX_TB_ZERO,
        GX_TB_ADDHALF,
        GX_TB_SUBHALF,
        GX_MAX_TEVBIAS,
    }

    /// <summary>
    /// Texture Environment (Tev) clamp modes.
    /// </summary>
    public enum GXTevClampMode
    {
        GX_TC_LINEAR,
        GX_TC_GE,
        GX_TC_EQ,
        GX_TC_LE,
        GX_MAX_TEVCLAMPMODE
    }

    /// <summary>
    /// Texture Environment (Tev) input control.
    /// </summary>
    public enum GXTevColorArg_Rev1
    {
        GX_CC_CPREV,
        GX_CC_APREV,
        GX_CC_C0,
        GX_CC_A0,
        GX_CC_C1,
        GX_CC_A1,
        GX_CC_C2,
        GX_CC_A2,
        GX_CC_TEXC,
        GX_CC_TEXA,
        GX_CC_RASC,
        GX_CC_RASA,
        GX_CC_ONE,
        GX_CC_HALF,
        GX_CC_QUARTER,
        GX_CC_ZERO,

        GX_CC_TEXRRR,
        GX_CC_TEXGGG,
        GX_CC_TEXBBB
    }

    /// <summary>
    /// Texture Environment (Tev) input control.
    /// </summary>
    public enum GXTevColorArg_Rev2
    {
        GX_CC_CPREV,
        GX_CC_APREV,
        GX_CC_C0,
        GX_CC_A0,
        GX_CC_C1,
        GX_CC_A1,
        GX_CC_C2,
        GX_CC_A2,
        GX_CC_TEXC,
        GX_CC_TEXA,
        GX_CC_RASC,
        GX_CC_RASA,
        GX_CC_ONE,
        GX_CC_HALF,
        GX_CC_KONST,
        GX_CC_ZERO
    }

    /// <summary>
    /// TEV color channel components.
    /// GX Revision 2 or later only
    /// </summary>
    public enum GXTevColorChan
    {
        GX_CH_RED = 0,
        GX_CH_GREEN,
        GX_CH_BLUE,
        GX_CH_ALPHA
    }

    /// <summary>
    /// TEV constant alpha input selection.
    /// GX Revision 2 or later only
    /// </summary>
    public enum GXTevKAlphaSel
    {
        GX_TEV_KASEL_1 = 0x00,
        GX_TEV_KASEL_7_8 = 0x01,
        GX_TEV_KASEL_3_4 = 0x02,
        GX_TEV_KASEL_5_8 = 0x03,
        GX_TEV_KASEL_1_2 = 0x04,
        GX_TEV_KASEL_3_8 = 0x05,
        GX_TEV_KASEL_1_4 = 0x06,
        GX_TEV_KASEL_1_8 = 0x07,

        GX_TEV_KASEL_K0_R = 0x10,
        GX_TEV_KASEL_K1_R = 0x11,
        GX_TEV_KASEL_K2_R = 0x12,
        GX_TEV_KASEL_K3_R = 0x13,
        GX_TEV_KASEL_K0_G = 0x14,
        GX_TEV_KASEL_K1_G = 0x15,
        GX_TEV_KASEL_K2_G = 0x16,
        GX_TEV_KASEL_K3_G = 0x17,
        GX_TEV_KASEL_K0_B = 0x18,
        GX_TEV_KASEL_K1_B = 0x19,
        GX_TEV_KASEL_K2_B = 0x1A,
        GX_TEV_KASEL_K3_B = 0x1B,
        GX_TEV_KASEL_K0_A = 0x1C,
        GX_TEV_KASEL_K1_A = 0x1D,
        GX_TEV_KASEL_K2_A = 0x1E,
        GX_TEV_KASEL_K3_A = 0x1F
    }

    /// <summary>
    /// Texture Environment "konstant" color register name.
    /// </summary>
    public enum GXTevKColorID
    {
        GX_KCOLOR0 = 0,
        GX_KCOLOR1,
        GX_KCOLOR2,
        GX_KCOLOR3,
        GX_MAX_KCOLOR
    }

    /// <summary>
    /// TEV constant color input selection.
    /// GX Revision 2 or later only
    /// </summary>
    public enum GXTevKColorSel
    {
        GX_TEV_KCSEL_1 = 0x00,
        GX_TEV_KCSEL_7_8 = 0x01,
        GX_TEV_KCSEL_3_4 = 0x02,
        GX_TEV_KCSEL_5_8 = 0x03,
        GX_TEV_KCSEL_1_2 = 0x04,
        GX_TEV_KCSEL_3_8 = 0x05,
        GX_TEV_KCSEL_1_4 = 0x06,
        GX_TEV_KCSEL_1_8 = 0x07,

        GX_TEV_KCSEL_K0 = 0x0C,
        GX_TEV_KCSEL_K1 = 0x0D,
        GX_TEV_KCSEL_K2 = 0x0E,
        GX_TEV_KCSEL_K3 = 0x0F,
        GX_TEV_KCSEL_K0_R = 0x10,
        GX_TEV_KCSEL_K1_R = 0x11,
        GX_TEV_KCSEL_K2_R = 0x12,
        GX_TEV_KCSEL_K3_R = 0x13,
        GX_TEV_KCSEL_K0_G = 0x14,
        GX_TEV_KCSEL_K1_G = 0x15,
        GX_TEV_KCSEL_K2_G = 0x16,
        GX_TEV_KCSEL_K3_G = 0x17,
        GX_TEV_KCSEL_K0_B = 0x18,
        GX_TEV_KCSEL_K1_B = 0x19,
        GX_TEV_KCSEL_K2_B = 0x1A,
        GX_TEV_KCSEL_K3_B = 0x1B,
        GX_TEV_KCSEL_K0_A = 0x1C,
        GX_TEV_KCSEL_K1_A = 0x1D,
        GX_TEV_KCSEL_K2_A = 0x1E,
        GX_TEV_KCSEL_K3_A = 0x1F
    }

    /// <summary>
    /// Texture Environment (Tev) control.
    /// </summary>
    public enum GXTevMode
    {
        GX_MODULATE,
        GX_DECAL,
        GX_BLEND,
        GX_REPLACE,
        GX_PASSCLR
    }

    /// <summary>
    /// Texture Environment (Tev) control.
    /// </summary>
    public enum GXTevOp_Rev1
    {
        GX_TEV_ADD,
        GX_TEV_SUB
    }

    /// <summary>
    /// Texture Environment (Tev) control.
    /// </summary>
    public enum GXTevOp_Rev2
    {
        GX_TEV_ADD = 0,
        GX_TEV_SUB = 1,

        GX_TEV_COMP_R8_GT = 8,
        GX_TEV_COMP_R8_EQ = 9,
        GX_TEV_COMP_GR16_GT = 10,
        GX_TEV_COMP_GR16_EQ = 11,
        GX_TEV_COMP_BGR24_GT = 12,
        GX_TEV_COMP_BGR24_EQ = 13,
        GX_TEV_COMP_RGB8_GT = 14,
        GX_TEV_COMP_RGB8_EQ = 15,

        /// <summary>
        /// for alpha channel
        /// </summary>
        GX_TEV_COMP_A8_GT = GX_TEV_COMP_RGB8_GT,
        /// <summary>
        /// for alpha channel
        /// </summary>
        GX_TEV_COMP_A8_EQ = GX_TEV_COMP_RGB8_EQ,
    }

    /// <summary>
    /// Texture Environment source/destination register name.  GX_TEVPREV is used as the default register for passing results from one stage to another. GX_TEVPREV must be used as the output register in the last active Tev stage.
    /// </summary>
    public enum GXTevRegID
    {
        GX_TEVPREV = 0,
        GX_TEVREG0,
        GX_TEVREG1,
        GX_TEVREG2,
        GX_MAX_TEVREG
    }

    /// <summary>
    /// Texture Environment (Tev)  control.
    /// </summary>
    public enum GXTevScale
    {
        GX_CS_SCALE_1,
        GX_CS_SCALE_2,
        GX_CS_SCALE_4,
        GX_CS_DIVIDE_2,
        GX_MAX_TEVSCALE
    }

    /// <summary>
    /// Texture Environment (Tev) stage name.
    /// </summary>
    public enum GXTevStageID
    {
        GX_TEVSTAGE0,
        GX_TEVSTAGE1,
        GX_TEVSTAGE2,
        GX_TEVSTAGE3,
        GX_TEVSTAGE4,
        GX_TEVSTAGE5,
        GX_TEVSTAGE6,
        GX_TEVSTAGE7,
        GX_TEVSTAGE8,
        GX_TEVSTAGE9,
        GX_TEVSTAGE10,
        GX_TEVSTAGE11,
        GX_TEVSTAGE12,
        GX_TEVSTAGE13,
        GX_TEVSTAGE14,
        GX_TEVSTAGE15,
        GX_MAX_TEVSTAGE
    }

    /// <summary>
    /// TEV color swap table entries.
    /// GX Revision 2 or later only
    /// </summary>
    public enum GXTevSwapSel
    {
        GX_TEV_SWAP0 = 0,
        GX_TEV_SWAP1,
        GX_TEV_SWAP2,
        GX_TEV_SWAP3,
        GX_MAX_TEVSWAP
    }

    /// <summary>
    /// Size of texture cache regions.
    /// </summary>
    public enum GXTexCacheSize
    {
        GX_TEXCACHE_32K,
        GX_TEXCACHE_128K,
        GX_TEXCACHE_512K,
        GX_TEXCACHE_NONE
    }

    /// <summary>
    /// Name of texture coordinate.
    /// </summary>
    public enum GXTexCoordID
    {
        /// <summary>
        /// generated texture coordinate 0
        /// </summary>
        GX_TEXCOORD0 = 0x0,
        /// <summary>
        /// generated texture coordinate 1
        /// </summary>
        GX_TEXCOORD1,
        /// <summary>
        /// generated texture coordinate 2
        /// </summary>
        GX_TEXCOORD2,
        /// <summary>
        /// generated texture coordinate 3
        /// </summary>
        GX_TEXCOORD3,
        /// <summary>
        /// generated texture coordinate 4
        /// </summary>
        GX_TEXCOORD4,
        /// <summary>
        /// generated texture coordinate 5
        /// </summary>
        GX_TEXCOORD5,
        /// <summary>
        /// generated texture coordinate 6
        /// </summary>
        GX_TEXCOORD6,
        /// <summary>
        /// generated texture coordinate 7
        /// </summary>
        GX_TEXCOORD7,
        GX_MAX_TEXCOORD = 8,
        GX_TEXCOORD_NULL = 0xff
    }

    /// <summary>
    /// Texture filter types.
    /// </summary>
    public enum GXTexFilter
    {
        GX_NEAR,
        GX_LINEAR,
        GX_NEAR_MIP_NEAR,
        GX_LIN_MIP_NEAR,
        GX_NEAR_MIP_LIN,
        GX_LIN_MIP_LIN
    }

    /// <summary>
    /// RGB, RGBA, Intensity, Intensity/Alpha, Compressed, and Z texture format types.   See GXCITexFmt for color index formats.
    ///
    /// HW2 adds additional formats that are only useable with GXSetTexCopyDst.In addition to the regular formats, these CTF formats specify how data is copied out of the EFB into a texture in main memory.  In order to actually use that texture, one must specify a non-copy format of matching size.For example, if copying using GX_CTF_RG8, one would apply the resulting texture using GX_TF_IA8.
    /// </summary>
    public enum GXTexFmt_Rev1
    {
        GX_TF_I4 = 0x0,
        GX_TF_I8 = 0x1,
        GX_TF_IA4 = 0x2,
        GX_TF_IA8 = 0x3,
        GX_TF_RGB565 = 0x4,
        GX_TF_RGB5A3 = 0x5,
        GX_TF_RGBA8 = 0x6,
        GX_TF_CMPR = 0xE,
        GX_TF_A8 = (0x20 | GX_TF_I8),
        GX_TF_Z8 = (0x10 | GX_TF_I8),
        GX_TF_Z16 = (0x10 | GX_TF_IA8),
        GX_TF_Z24X8 = (0x10 | GX_TF_RGBA8)
    }

    /// <summary>
    /// RGB, RGBA, Intensity, Intensity/Alpha, Compressed, and Z texture format types.   See GXCITexFmt for color index formats.
    ///
    /// HW2 adds additional formats that are only useable with GXSetTexCopyDst.In addition to the regular formats, these CTF formats specify how data is copied out of the EFB into a texture in main memory.  In order to actually use that texture, one must specify a non-copy format of matching size.For example, if copying using GX_CTF_RG8, one would apply the resulting texture using GX_TF_IA8.
    /// </summary>
    public enum GXTexFmt_Rev2
    {
        // defines
        _GX_TF_CTF = 0x20, /* copy-texture-format only */
        _GX_TF_ZTF = 0x10, /* Z-texture-format */
        // end defines

        GX_TF_I4 = 0x0,
        GX_TF_I8 = 0x1,
        GX_TF_IA4 = 0x2,
        GX_TF_IA8 = 0x3,
        GX_TF_RGB565 = 0x4,
        GX_TF_RGB5A3 = 0x5,
        GX_TF_RGBA8 = 0x6,
        GX_TF_CMPR = 0xE,

        GX_CTF_R4 = 0x0 | _GX_TF_CTF,
        GX_CTF_RA4 = 0x2 | _GX_TF_CTF,
        GX_CTF_RA8 = 0x3 | _GX_TF_CTF,
        GX_CTF_YUVA8 = 0x6 | _GX_TF_CTF,
        GX_CTF_A8 = 0x7 | _GX_TF_CTF,
        GX_CTF_R8 = 0x8 | _GX_TF_CTF,
        GX_CTF_G8 = 0x9 | _GX_TF_CTF,
        GX_CTF_B8 = 0xA | _GX_TF_CTF,
        GX_CTF_RG8 = 0xB | _GX_TF_CTF,
        GX_CTF_GB8 = 0xC | _GX_TF_CTF,

        GX_TF_Z8 = 0x1 | _GX_TF_ZTF,
        GX_TF_Z16 = 0x3 | _GX_TF_ZTF,
        GX_TF_Z24X8 = 0x6 | _GX_TF_ZTF,

        GX_CTF_Z4 = 0x0 | _GX_TF_ZTF | _GX_TF_CTF,
        GX_CTF_Z8M = 0x9 | _GX_TF_ZTF | _GX_TF_CTF,
        GX_CTF_Z8L = 0xA | _GX_TF_ZTF | _GX_TF_CTF,
        GX_CTF_Z16L = 0xC | _GX_TF_ZTF | _GX_TF_CTF,

        GX_TF_A8 = GX_CTF_A8 // to keep compatibility
    }

    /// <summary>
    /// Texture coordinate source parameter.
    /// </summary>
    public enum GXTexGenSrc
    {
        GX_TG_POS = 0,
        GX_TG_NRM,
        GX_TG_BINRM,
        GX_TG_TANGENT,
        GX_TG_TEX0,
        GX_TG_TEX1,
        GX_TG_TEX2,
        GX_TG_TEX3,
        GX_TG_TEX4,
        GX_TG_TEX5,
        GX_TG_TEX6,
        GX_TG_TEX7,
        GX_TG_TEXCOORD0,
        GX_TG_TEXCOORD1,
        GX_TG_TEXCOORD2,
        GX_TG_TEXCOORD3,
        GX_TG_TEXCOORD4,
        GX_TG_TEXCOORD5,
        GX_TG_TEXCOORD6,
        GX_TG_COLOR0,
        GX_TG_COLOR1
    }

    /// <summary>
    /// Texture coordinate generation method.
    /// </summary>
    public enum GXTexGenType
    {
        GX_TG_MTX3x4 = 0,
        GX_TG_MTX2x4,
        GX_TG_BUMP0,
        GX_TG_BUMP1,
        GX_TG_BUMP2,
        GX_TG_BUMP3,
        GX_TG_BUMP4,
        GX_TG_BUMP5,
        GX_TG_BUMP6,
        GX_TG_BUMP7,
        GX_TG_SRTG
    }

    /// <summary>
    /// Texture map name.
    /// </summary>
    public enum GXTexMapID
    {
        GX_TEXMAP0,
        GX_TEXMAP1,
        GX_TEXMAP2,
        GX_TEXMAP3,
        GX_TEXMAP4,
        GX_TEXMAP5,
        GX_TEXMAP6,
        GX_TEXMAP7,
        GX_MAX_TEXMAP,

        GX_TEXMAP_NULL = 0xff,
        /// <summary>
        /// mask: disables texture look up
        /// </summary>
        GX_TEX_DISABLE = 0x100
    }

    /// <summary>
    /// Texture matrix index.
    /// </summary>
    public enum GXTexMtx
    {
        GX_TEXMTX0 = 30,
        GX_TEXMTX1 = 33,
        GX_TEXMTX2 = 36,
        GX_TEXMTX3 = 39,
        GX_TEXMTX4 = 42,
        GX_TEXMTX5 = 45,
        GX_TEXMTX6 = 48,
        GX_TEXMTX7 = 51,
        GX_TEXMTX8 = 54,
        GX_TEXMTX9 = 57,
        GX_IDENTITY = 60
    }

    /// <summary>
    /// Texture matrix type.
    /// </summary>
    public enum GXTexMtxType
    {
        GX_MTX3x4 = 0,
        GX_MTX2x4
    }

    /// <summary>
    /// Texture offset values.
    /// </summary>
    public enum GXTexOffset
    {
        GX_TO_ZERO,
        GX_TO_SIXTEENTH,
        GX_TO_EIGHTH,
        GX_TO_FOURTH,
        GX_TO_HALF,
        GX_TO_ONE,
        GX_MAX_TEXOFFSET
    }

    /// <summary>
    /// Texture coordinate controls.
    /// </summary>
    public enum GXTexWrapMode
    {
        GX_CLAMP,
        GX_REPEAT,
        GX_MIRROR,
        GX_MAX_TEXWRAPMODE
    }

    /// <summary>
    /// Name of Texture Look-Up Table (TLUT) in texture memory.  Each table GX_TLUT0-15  contains 256 entries, 16b per entry.  GX_BIGTLUT0-3 contains 1024 entries, 16b per entry.  Used for configuring texture memory in GXInit.
    /// </summary>
    public enum GXTlut
    {
        // default 256-entry TLUTs
        GX_TLUT0 = 0,
        GX_TLUT1,
        GX_TLUT2,
        GX_TLUT3,
        GX_TLUT4,
        GX_TLUT5,
        GX_TLUT6,
        GX_TLUT7,
        GX_TLUT8,
        GX_TLUT9,
        GX_TLUT10,
        GX_TLUT11,
        GX_TLUT12,
        GX_TLUT13,
        GX_TLUT14,
        GX_TLUT15,
        GX_BIGTLUT0,
        GX_BIGTLUT1,
        GX_BIGTLUT2,
        GX_BIGTLUT3,
    }

    /// <summary>
    /// Texture Look-Up Table (TLUT) formats.
    /// </summary>
    public enum GXTlutFmt
    {
        GX_TL_IA8 = 0x0,
        GX_TL_RGB565 = 0x1,
        GX_TL_RGB5A3 = 0x2,
        GX_MAX_TLUTFMT
    }

    /// <summary>
    /// Size of the Texture Look-Up Table (TLUT) in texture memory.
    /// </summary>
    public enum GXTlutSize
    {
        GX_TLUT_16 = 1, // number of 16 entry blocks.
        GX_TLUT_32 = 2,
        GX_TLUT_64 = 4,
        GX_TLUT_128 = 8,
        GX_TLUT_256 = 16,
        GX_TLUT_512 = 32,
        GX_TLUT_1K = 64,
        GX_TLUT_2K = 128,
        GX_TLUT_4K = 256,
        GX_TLUT_8K = 512,
        GX_TLUT_16K = 1024
    }

    /// <summary>
    /// Vertex cache performance counter parameters.
    /// </summary>
    public enum GXVCachePerf
    {
        GX_VC_POS,
        GX_VC_NRM,
        GX_VC_CLR0,
        GX_VC_CLR1,
        GX_VC_TEX0,
        GX_VC_TEX1,
        GX_VC_TEX2,
        GX_VC_TEX3,
        GX_VC_TEX4,
        GX_VC_TEX5,
        GX_VC_TEX6,
        GX_VC_TEX7,
        GX_VC_ALL = 0xf
    }

    // V
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

    // W
    /// <summary>
    /// Verify levels.
    /// </summary>
    public enum GXWarningLevel
    {
        /// <summary>
        /// Disable runtime warnings.
        /// </summary>
        GX_WARN_NONE,
        /// <summary>
        /// Check and report only severe warnings.
        /// </summary>
        GX_WARN_SEVERE,
        /// <summary>
        /// Check and report only severe and medium warnings.
        /// </summary>
        GX_WARN_MEDIUM,
        /// <summary>
        /// Check and report all warnings.
        /// </summary>
        GX_WARN_ALL,
    }

    // Z
    /// <summary>
    /// Compressed Z formats.
    /// </summary>
    public enum GXZFmt16
    {
        GX_ZC_LINEAR,
        GX_ZC_NEAR,
        GX_ZC_MID,
        GX_ZC_FAR
    }

    /// <summary>
    /// Z texture operations.
    /// </summary>
    public enum GXZTexOp
    {
        GX_ZT_DISABLE,
        GX_ZT_ADD,
        GX_ZT_REPLACE,
        GX_MAX_ZTEXOP
    }
}