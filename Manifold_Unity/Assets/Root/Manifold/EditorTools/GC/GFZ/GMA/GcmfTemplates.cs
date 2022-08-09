using GameCube.GFZ.GMA;
using GameCube.GFZ.Stage;
using GameCube.GX;
using Manifold.IO;

// NOTE: Always do alpha templates last!


namespace Manifold.EditorTools.GC.GFZ
{
    public static class GcmfTemplates
    {

        public static class Debug
        {
            public static GcmfTemplate CreateLitVertexColored()
            {
                var tevLayers = new TevLayer[0];
                var textureHashes = new string[0];
                var material = new Material()
                {
                    MaterialColor = new GXColor(0xb2b2b2ff, ComponentType.GX_RGBA8),
                    AmbientColor = new GXColor(0x7f7f7fff, ComponentType.GX_RGBA8),
                    SpecularColor = new GXColor(0xFFFFFFFF, ComponentType.GX_RGBA8),
                    Unk0x10 = MatFlags0x10.unk1 | MatFlags0x10.unk3 | MatFlags0x10.unk5,
                };
                var unknownAlphaOptions = new UnkAlphaOptions();
                var submesh = new Submesh()
                {
                    RenderFlags = RenderFlags.doubleSidedFaces,
                    Material = material,
                    UnkAlphaOptions = unknownAlphaOptions,
                };

                var template = new GcmfTemplate()
                {
                    IsTranslucid = false,
                    Submesh = submesh,
                    TevLayers = tevLayers,
                    TextureHashes = textureHashes,
                    TextureScrollFields = null,
                };

                Assert.IsTrue(textureHashes.Length == tevLayers.Length);
                return template;
            }
            public static GcmfTemplate CreateUnlitVertexColored()
            {
                // Not doing isSwappable, tpl index, config index
                var tevLayers = new TevLayer[0];
                var textureHashes = new string[0];

                var material = new Material();
                var unknownAlphaOptions = new UnkAlphaOptions();
                var submesh = new Submesh()
                {
                    RenderFlags = RenderFlags.unlit | RenderFlags.doubleSidedFaces,
                    Material = material,
                    UnkAlphaOptions = unknownAlphaOptions,
                };

                var template = new GcmfTemplate()
                {
                    IsTranslucid = false,
                    Submesh = submesh,
                    TevLayers = tevLayers,
                    TextureHashes = textureHashes,
                    TextureScrollFields = null,
                };

                return template;
            }
        }

        public static class General
        {
            public static GcmfTemplate CreateSlipGX()
            {
                var tevLayers = new TevLayer[]
                {
                        new TevLayer()
                        {
                            Unk0x00 = 0,
                            MipmapSetting = MipmapSetting.ENABLE_MIPMAP | MipmapSetting.UNK_FLAG_1 | MipmapSetting.UNK_FLAG_2,
                            WrapMode = TextureWrapMode.repeatX | TextureWrapMode.repeatY | TextureWrapMode.unk6 | TextureWrapMode.unk7,
                            LodBias = -10,
                            AnisotropicFilter = GXAnisotropy.GX_ANISO_4,
                            Unk0x0C = 0,
                            Unk0x12 = TexFlags0x10.unk4 | TexFlags0x10.unk5,
                        },
                        new TevLayer()
                        {
                            Unk0x00 = TexFlags0x00.ENABLE_UV_SCROLL,
                            MipmapSetting = MipmapSetting.ENABLE_MIPMAP | MipmapSetting.UNK_FLAG_1 | MipmapSetting.UNK_FLAG_2,
                            WrapMode = TextureWrapMode.repeatX | TextureWrapMode.unk7,
                            LodBias = 0,
                            AnisotropicFilter = GXAnisotropy.GX_ANISO_1,
                            Unk0x0C = 0,
                            Unk0x12 = TexFlags0x10.CheapAlphaOnSelf | TexFlags0x10.unk4 | TexFlags0x10.unk5,
                        },
                };
                var textureHashes = new string[]
                {
                        "f51a234a9db86230e837558f6271f8d1", // st03 tex 61, blue squares
                        "c6994c636310879078862e84616a781c", // st03 tex 64, flash
                };
                var textureScrollFields = new TextureScrollField[]
                {
                        new TextureScrollField(-2, 0),
                };
                var material = new Material
                {
                    MaterialColor = new GXColor(0xb2b2b2ff),
                    AmbientColor = new GXColor(0x7f7f7fff),
                    SpecularColor = new GXColor(0x00000000),
                    Unk0x10 = 0,
                    Alpha = 255,
                    UnkAlpha0x14 = -1,
                    Unk0x15 = 0,
                };
                var submesh = new Submesh()
                {
                    RenderFlags = 0,
                    Material = material,
                    UnkAlphaOptions = new UnkAlphaOptions(),
                };

                var template = new GcmfTemplate()
                {
                    IsTranslucid = false,
                    Submesh = submesh,
                    TevLayers = tevLayers,
                    TextureHashes = textureHashes,
                    TextureScrollFields = textureScrollFields,
                };

                Assert.IsTrue(textureHashes.Length == tevLayers.Length);
                return template;
            }
            public static GcmfTemplate CreateSlipAX()
            {
                var tevLayers = new TevLayer[]
                {
                        new TevLayer()
                        {
                            Unk0x00 = 0,
                            MipmapSetting = MipmapSetting.ENABLE_MIPMAP | MipmapSetting.UNK_FLAG_1 | MipmapSetting.UNK_FLAG_2,
                            WrapMode = TextureWrapMode.mirrorX | TextureWrapMode.repeatY | TextureWrapMode.unk6 | TextureWrapMode.unk7,
                            LodBias = -10,
                            AnisotropicFilter = GXAnisotropy.GX_ANISO_4,
                            Unk0x0C = 0,
                            Unk0x12 = TexFlags0x10.unk4 | TexFlags0x10.unk5,
                        },
                        new TevLayer()
                        {
                            Unk0x00 = TexFlags0x00.ENABLE_UV_SCROLL,
                            MipmapSetting = MipmapSetting.ENABLE_MIPMAP | MipmapSetting.UNK_FLAG_1 | MipmapSetting.UNK_FLAG_2,
                            WrapMode = TextureWrapMode.repeatX | TextureWrapMode.mirrorY | TextureWrapMode.unk7,
                            LodBias = 0,
                            AnisotropicFilter = GXAnisotropy.GX_ANISO_1,
                            Unk0x0C = 187,
                            Unk0x12 = TexFlags0x10.CheapAlphaOnSelf | TexFlags0x10.unk4 | TexFlags0x10.unk5,
                        },
                };
                var textureHashes = new string[]
                {
                        "e68fb000589f037b307321f4c44d2f48", // st32 tex 41, large blue squares grid
                        "2e09bff156282e7acc1090c9fc12afa2", // st32 tex 42, flash
                };
                var textureScrollFields = new TextureScrollField[]
                {
                        new TextureScrollField(2, 0),
                };
                var material = new Material
                {
                    MaterialColor = new GXColor(0xb2b2b2ff),
                    AmbientColor = new GXColor(0x7f7f7fff),
                    SpecularColor = new GXColor(0x00000000),
                    Unk0x10 = 0,
                    Alpha = 255,
                    UnkAlpha0x14 = -1,
                    Unk0x15 = 0,
                };
                var submesh = new Submesh()
                {
                    RenderFlags = 0,
                    Material = material,
                    UnkAlphaOptions = new UnkAlphaOptions(),
                };

                var template = new GcmfTemplate()
                {
                    IsTranslucid = false,
                    Submesh = submesh,
                    TevLayers = tevLayers,
                    TextureHashes = textureHashes,
                    TextureScrollFields = textureScrollFields,
                };

                Assert.IsTrue(textureHashes.Length == tevLayers.Length);
                return template;
            }

            public static GcmfTemplate CreateDirtAlpha()
            {
                var tevLayers = new TevLayer[]
                {
                        new TevLayer()
                        {
                            Unk0x00 = 0,
                            MipmapSetting = MipmapSetting.UNK_FLAG_1 | MipmapSetting.UNK_FLAG_2,
                            WrapMode = TextureWrapMode.repeatX | TextureWrapMode.repeatY,
                            LodBias = 20,
                            AnisotropicFilter = GXAnisotropy.GX_ANISO_1,
                            Unk0x0C = 0,
                            Unk0x12 = TexFlags0x10.unk4 | TexFlags0x10.unk5,
                        },
                };
                var textureHashes = new string[]
                {
                        "9e0082712ac9ef3bc7d0344fde1c52a4", // st03 tex 66, color brown
                };
                var material = new Material
                {
                    MaterialColor = new GXColor(0xb2b2b2ff),
                    AmbientColor = new GXColor(0x7f7f7fff),
                    SpecularColor = new GXColor(0x00000000),
                    Unk0x10 = 0,
                    Alpha = 178,
                    UnkAlpha0x14 = 1,
                    Unk0x15 = 0,
                };
                var submesh = new Submesh()
                {
                    RenderFlags = 0,
                    Material = material,
                    UnkAlphaOptions = new UnkAlphaOptions(),
                };

                var template = new GcmfTemplate()
                {
                    IsTranslucid = true,
                    Submesh = submesh,
                    TevLayers = tevLayers,
                    TextureHashes = textureHashes,
                    TextureScrollFields = null,
                };

                Assert.IsTrue(textureHashes.Length == tevLayers.Length);
                return template;
            }
            public static GcmfTemplate CreateDirtNoise()
            {
                var tevLayers = new TevLayer[]
                {
                        new TevLayer()
                        {
                            Unk0x00 = TexFlags0x00.ENABLE_UV_SCROLL,
                            MipmapSetting = MipmapSetting.ENABLE_MIPMAP | MipmapSetting.UNK_FLAG_1 | MipmapSetting.UNK_FLAG_2,
                            WrapMode = TextureWrapMode.repeatX | TextureWrapMode.repeatY | TextureWrapMode.unk7,
                            LodBias = 0,
                            AnisotropicFilter = GXAnisotropy.GX_ANISO_1,
                            Unk0x0C = 0,
                            Unk0x12 = TexFlags0x10.unk4 | TexFlags0x10.unk5,
                        },
                        new TevLayer()
                        {
                            Unk0x00 = TexFlags0x00.ENABLE_UV_SCROLL,
                            MipmapSetting = MipmapSetting.ENABLE_MIPMAP | MipmapSetting.UNK_FLAG_1 | MipmapSetting.UNK_FLAG_2,
                            WrapMode = TextureWrapMode.repeatX | TextureWrapMode.repeatY | TextureWrapMode.unk7,
                            LodBias = 0,
                            AnisotropicFilter = GXAnisotropy.GX_ANISO_1,
                            Unk0x0C = 0,
                            Unk0x12 = TexFlags0x10.CheapAlphaOnSelf | TexFlags0x10.unk4 | TexFlags0x10.unk5,
                        },
                };
                var textureHashes = new string[]
                {
                        "da8e87b2702b9595c731149f6273c199", // st03 tex 59
                        "da8e87b2702b9595c731149f6273c199", // st03 tex 59 (yes, again)
                };
                var textureScrollFields = new TextureScrollField[]
                {
                        new TextureScrollField(+1.0f, +2.0f), //frick, mixed up u and v
                };
                var material = new Material
                {
                    MaterialColor = new GXColor(0xb2b2b2ff),
                    AmbientColor = new GXColor(0x7f7f7fff),
                    SpecularColor = new GXColor(0x00000000),
                    Unk0x10 = 0,
                    Alpha = 255,
                    UnkAlpha0x14 = -1,
                    Unk0x15 = 0,
                };
                var submesh = new Submesh()
                {
                    RenderFlags = 0,
                    Material = material,
                    UnkAlphaOptions = new UnkAlphaOptions(),
                };

                var template = new GcmfTemplate()
                {
                    IsTranslucid = false,
                    Submesh = submesh,
                    TevLayers = tevLayers,
                    TextureHashes = textureHashes,
                    TextureScrollFields = textureScrollFields,
                };

                Assert.IsTrue(textureHashes.Length == tevLayers.Length);
                return template;
            }

            public static GcmfTemplate CreateLavaCrag()
            {
                var tevLayers = new TevLayer[]
                {
                        new TevLayer()
                        {
                            Unk0x00 = 0,
                            MipmapSetting = MipmapSetting.ENABLE_MIPMAP | MipmapSetting.UNK_FLAG_1 | MipmapSetting.UNK_FLAG_2,
                            WrapMode = TextureWrapMode.repeatX | TextureWrapMode.repeatY | TextureWrapMode.unk6 | TextureWrapMode.unk7,
                            LodBias = -10,
                            AnisotropicFilter = GXAnisotropy.GX_ANISO_4,
                            Unk0x0C = 173,
                            Unk0x12 = TexFlags0x10.unk4 | TexFlags0x10.unk5,
                        },
                };
                var textureHashes = new string[]
                {
                        "a9d86932a581a0cbece8275ed6582246", // st43 tex 43, fire/metal
                };
                var material = new Material
                {
                    MaterialColor = new GXColor(0xffcc7fff),
                    AmbientColor = new GXColor(0xffcc7fff),
                    SpecularColor = new GXColor(0x00000000),
                    Unk0x10 = 0,
                    Alpha = 255,
                    UnkAlpha0x14 = -1,
                    Unk0x15 = 0,
                };
                var submesh = new Submesh()
                {
                    RenderFlags = RenderFlags.unlit | RenderFlags.noFog,
                    Material = material,
                    UnkAlphaOptions = new UnkAlphaOptions(),
                };

                var template = new GcmfTemplate()
                {
                    IsTranslucid = false,
                    Submesh = submesh,
                    TevLayers = tevLayers,
                    TextureHashes = textureHashes,
                    TextureScrollFields = null,
                };

                Assert.IsTrue(textureHashes.Length == tevLayers.Length);
                return template;
            }
            public static GcmfTemplate CreateLavaAlpha()
            {
                var tevLayers = new TevLayer[]
                {
                        new TevLayer()
                        {
                            Unk0x00 = 0,
                            MipmapSetting = MipmapSetting.ENABLE_MIPMAP | MipmapSetting.UNK_FLAG_1 | MipmapSetting.UNK_FLAG_2,
                            WrapMode = TextureWrapMode.repeatX | TextureWrapMode.repeatY | TextureWrapMode.unk6 | TextureWrapMode.unk7,
                            LodBias = -10,
                            AnisotropicFilter = GXAnisotropy.GX_ANISO_4,
                            Unk0x0C = 0,
                            Unk0x12 = TexFlags0x10.unk4 | TexFlags0x10.unk5,
                        },
                        new TevLayer()
                        {
                            Unk0x00 = TexFlags0x00.ENABLE_UV_SCROLL,
                            MipmapSetting = MipmapSetting.ENABLE_MIPMAP | MipmapSetting.UNK_FLAG_1 | MipmapSetting.UNK_FLAG_2,
                            WrapMode = TextureWrapMode.repeatY | TextureWrapMode.unk7,
                            LodBias = 0,
                            AnisotropicFilter = GXAnisotropy.GX_ANISO_1,
                            Unk0x0C = 0,
                            Unk0x12 = TexFlags0x10.unk4 | TexFlags0x10.unk5,
                        },
                };
                var textureHashes = new string[]
                {
                        "605ca32b9ef8ce5934f67645031b10ea", // st43 tex 31, red dot
                        "befa86976ddfab6069ce2079503b23e3", // st43 tex 33, white/grey bars
                };
                var textureScrollFields = new TextureScrollField[]
                {
                        new TextureScrollField(0.00f, -1.25f),
                };
                var material = new Material
                {
                    MaterialColor = new GXColor(0xffcc7fff),
                    AmbientColor = new GXColor(0xffcc7fff),
                    SpecularColor = new GXColor(0x00000000),
                    Unk0x10 = 0,
                    Alpha = 255,
                    UnkAlpha0x14 = -1,
                    Unk0x15 = 0,
                };
                var submesh = new Submesh()
                {
                    RenderFlags = RenderFlags.unlit | RenderFlags.noFog | RenderFlags.screenBlend | RenderFlags.additiveBlend,
                    Material = material,
                    UnkAlphaOptions = new UnkAlphaOptions()
                    {
                        BlendFactors = BlendFactors.unk2_UseBlendMode | BlendFactors.unk4_UseBlendMode
                    },
                };

                var template = new GcmfTemplate()
                {
                    IsTranslucid = true,
                    Submesh = submesh,
                    TevLayers = tevLayers,
                    TextureHashes = textureHashes,
                    TextureScrollFields = textureScrollFields,
                };

                Assert.IsTrue(textureHashes.Length == tevLayers.Length);
                return template;
            }

            public static GcmfTemplate CreateRecoverDarkBase()
            {
                var tevLayers = new TevLayer[]
                {
                        new TevLayer()
                        {
                            Unk0x00 = 0,
                            MipmapSetting = MipmapSetting.UNK_FLAG_1,
                            WrapMode = TextureWrapMode.mirrorX | TextureWrapMode.repeatY, // I added mirror X
                            LodBias = 0,
                            AnisotropicFilter = GXAnisotropy.GX_ANISO_1,
                            Unk0x0C = 0,
                            Unk0x12 = TexFlags0x10.unk4 | TexFlags0x10.unk5,
                        },
                        new TevLayer()
                        {
                            Unk0x00 = TexFlags0x00.ENABLE_UV_SCROLL,
                            MipmapSetting = MipmapSetting.ENABLE_MIPMAP | MipmapSetting.UNK_FLAG_1 | MipmapSetting.UNK_FLAG_2,
                            WrapMode = TextureWrapMode.repeatX | TextureWrapMode.repeatY | TextureWrapMode.unk7,
                            LodBias = 0,
                            AnisotropicFilter = GXAnisotropy.GX_ANISO_1,
                            Unk0x0C = 0,
                            Unk0x12 = TexFlags0x10.CheapAlphaOnSelf | TexFlags0x10.unk4 | TexFlags0x10.unk5,
                        },
                };
                var textureHashes = new string[]
                {
                        "22e2e87246525810b8f56b2dc0bc7e24", // st01 tex 6, lined gradient
                        "2b67d7b296e0c2476ed7b6b5e4e19af9", // st01 tex 7, some alpha thing
                };
                var textureScrollFields = new TextureScrollField[]
                {
                        new TextureScrollField(0f, -2f),
                };
                var material = new Material
                {
                    MaterialColor = new GXColor(0xb2b2b2ff),
                    AmbientColor = new GXColor(0x7f7f7fff),
                    SpecularColor = new GXColor(0x00000000),
                    Unk0x10 = 0,
                    Alpha = 255,
                    UnkAlpha0x14 = -1,
                    Unk0x15 = 0,
                };
                var submesh = new Submesh()
                {
                    RenderFlags = RenderFlags.unlit,
                    Material = material,
                    UnkAlphaOptions = new UnkAlphaOptions(),
                };

                var template = new GcmfTemplate()
                {
                    IsTranslucid = false,
                    Submesh = submesh,
                    TevLayers = tevLayers,
                    TextureHashes = textureHashes,
                    TextureScrollFields = textureScrollFields,
                };

                Assert.IsTrue(textureHashes.Length == tevLayers.Length);
                return template;
            }
            public static GcmfTemplate CreateRecoverDarkAlpha()
            {
                var tevLayers = new TevLayer[]
                {
                        new TevLayer()
                        {
                            Unk0x00 = 0,
                            MipmapSetting = MipmapSetting.UNK_FLAG_1,
                            WrapMode = 0,
                            LodBias = 0,
                            AnisotropicFilter = GXAnisotropy.GX_ANISO_1,
                            Unk0x0C = 0,
                            Unk0x12 = TexFlags0x10.unk4 | TexFlags0x10.unk5,
                        },
                        new TevLayer()
                        {
                            Unk0x00 = 0,
                            MipmapSetting = MipmapSetting.ENABLE_MIPMAP | MipmapSetting.UNK_FLAG_1 | MipmapSetting.UNK_FLAG_2 | MipmapSetting.Unk5_AlphaMultiply,
                            WrapMode = TextureWrapMode.repeatX | TextureWrapMode.repeatY | TextureWrapMode.unk7,
                            LodBias = 0,
                            AnisotropicFilter = GXAnisotropy.GX_ANISO_1,
                            Unk0x0C = 184,
                            Unk0x12 = TexFlags0x10.unk4 | TexFlags0x10.unk5,
                        },
                };
                var textureHashes = new string[]
                {
                        "ad883f82349cecff410c061e1c2a6219", // st01 tex 16, red-pink color
                        "bd341ffeea1d7e2e13cebf13e280ba55", // st01 tex  4, alpha square
                };
                var material = new Material
                {
                    MaterialColor = new GXColor(0xffffffff),
                    AmbientColor = new GXColor(0xffffffff),
                    SpecularColor = new GXColor(0x00000000),
                    Unk0x10 = 0,
                    Alpha = 191,
                    UnkAlpha0x14 = -1,
                    Unk0x15 = 0,
                };
                var submesh = new Submesh()
                {
                    RenderFlags = 0,
                    Material = material,
                    UnkAlphaOptions = new UnkAlphaOptions(),
                };

                var template = new GcmfTemplate()
                {
                    IsTranslucid = true,
                    Submesh = submesh,
                    TevLayers = tevLayers,
                    TextureHashes = textureHashes,
                    TextureScrollFields = null,
                };

                Assert.IsTrue(textureHashes.Length == tevLayers.Length);
                return template;
            }

            public static GcmfTemplate CreateRecoverLightSubBase()
            {
                var tevLayers = new TevLayer[]
                {
                        new TevLayer()
                        {
                            Unk0x00 = 0,
                            MipmapSetting = MipmapSetting.ENABLE_MIPMAP | MipmapSetting.UNK_FLAG_1 | MipmapSetting.UNK_FLAG_2,
                            WrapMode = TextureWrapMode.mirrorX | TextureWrapMode.repeatY | TextureWrapMode.unk7,
                            LodBias = 0,
                            AnisotropicFilter = GXAnisotropy.GX_ANISO_1,
                            Unk0x0C = 0,
                            Unk0x12 = TexFlags0x10.unk4 | TexFlags0x10.unk5,
                        },
                };
                var textureHashes = new string[]
                {
                        "b42318832be6f79480973fddd2b4e0ac", // st05 tex 15, mut usused (neat)
                };
                var material = new Material
                {
                    MaterialColor = new GXColor(0xb2b2b2ff),
                    AmbientColor = new GXColor(0x7f7f7fff),
                    SpecularColor = new GXColor(0x00000000),
                    Unk0x10 = 0,
                    Alpha = 255,
                    UnkAlpha0x14 = -1,
                    Unk0x15 = 0,
                };
                var submesh = new Submesh()
                {
                    RenderFlags = RenderFlags.unlit | RenderFlags.customMaterialUseAmbientColor,
                    Material = material,
                    UnkAlphaOptions = new UnkAlphaOptions(),
                };

                var template = new GcmfTemplate()
                {
                    IsTranslucid = false,
                    Submesh = submesh,
                    TevLayers = tevLayers,
                    TextureHashes = textureHashes,
                    TextureScrollFields = null,
                };

                Assert.IsTrue(textureHashes.Length == tevLayers.Length);
                return template;
            }
            public static GcmfTemplate CreateRecoverLightBase()
            {
                // Only difference is a few render flags
                var template = CreateRecoverDarkBase();
                template.Submesh.RenderFlags |= RenderFlags.screenBlend | RenderFlags.additiveBlend;
                template.Submesh.UnkAlphaOptions.BlendFactors |= BlendFactors.unk2_UseBlendMode | BlendFactors.unk4_UseBlendMode;

                return template;
            }
            public static GcmfTemplate CreateRecoverLightAlpha() => CreateRecoverDarkAlpha();

            public static GcmfTemplate CreateTrim() => MuteCity.RoadSides();
        }

        public static class MuteCity
        {

            public static GcmfTemplate RoadRails()
            {
                var tevLayers = new TevLayer[]
                {
                        new TevLayer()
                        {
                            Unk0x00 = 0,
                            MipmapSetting = MipmapSetting.UNK_FLAG_1,
                            WrapMode = TextureWrapMode.repeatX | TextureWrapMode.repeatY,
                            LodBias = 0,
                            AnisotropicFilter = GXAnisotropy.GX_ANISO_1,
                            Unk0x0C = 0,
                            Unk0x12 = TexFlags0x10.unk4 | TexFlags0x10.unk5,
                        },
                        new TevLayer()
                        {
                            Unk0x00 = TexFlags0x00.ENABLE_UV_SCROLL,
                            MipmapSetting = MipmapSetting.UNK_FLAG_1 | MipmapSetting.Unk5_AlphaMultiply,
                            WrapMode = TextureWrapMode.repeatX | TextureWrapMode.repeatY,
                            LodBias = 0,
                            AnisotropicFilter = GXAnisotropy.GX_ANISO_1,
                            Unk0x0C = 0,
                            Unk0x12 = TexFlags0x10.unk4 | TexFlags0x10.unk5,
                        },
                        new TevLayer()
                        {
                            Unk0x00 = 0,
                            MipmapSetting = MipmapSetting.Unk5_AlphaMultiply,
                            WrapMode = TextureWrapMode.repeatX | TextureWrapMode.repeatY,
                            LodBias = 0,
                            AnisotropicFilter = GXAnisotropy.GX_ANISO_1,
                            Unk0x0C = 0,
                            Unk0x12 = TexFlags0x10.unk4 | TexFlags0x10.unk5,
                        },
                };
                var textureHashes = new string[]
                {
                        "ca0853c45448b241aa2b03cbe2b93182", // st01 tex  3
                        "9ed3039353de68dbbf59d8904f7bb00f", // st01 tex 40
                        "390204a0d91287427073649ec4efc80f", // st01 tex  0
                };
                var textureScrollFields = new TextureScrollField[]
                {
                        new TextureScrollField(0, 30),
                };
                var material = new Material
                {
                    MaterialColor = new GXColor(0xb2b2b2ff),
                    AmbientColor = new GXColor(0x7f7f7fff),
                    SpecularColor = new GXColor(0x00000000),
                    Unk0x10 = 0,
                    Alpha = 255,
                    UnkAlpha0x14 = 0,
                    Unk0x15 = 0,
                };
                var unknownAlphaOptions = new UnkAlphaOptions()
                {
                    BlendFactors = BlendFactors.unk2_UseBlendMode | BlendFactors.unk4_UseBlendMode,
                };
                var submesh = new Submesh()
                {
                    RenderFlags = RenderFlags.unlit | RenderFlags.doubleSidedFaces | RenderFlags.screenBlend | RenderFlags.additiveBlend,
                    Material = material,
                    UnkAlphaOptions = unknownAlphaOptions,
                };

                var template = new GcmfTemplate()
                {
                    IsTranslucid = true,
                    Submesh = submesh,
                    TevLayers = tevLayers,
                    TextureHashes = textureHashes,
                    TextureScrollFields = textureScrollFields,
                };

                Assert.IsTrue(textureHashes.Length == tevLayers.Length);
                return template;
            }
            public static GcmfTemplate RoadTop()
            {
                var tevLayers = new TevLayer[]
                {
                        new TevLayer()
                        {
                            Unk0x00 = 0,
                            MipmapSetting = MipmapSetting.ENABLE_MIPMAP | MipmapSetting.UNK_FLAG_1 | MipmapSetting.UNK_FLAG_2,
                            WrapMode = TextureWrapMode.mirrorX | TextureWrapMode.repeatY | TextureWrapMode.unk6 | TextureWrapMode.unk7,
                            LodBias = -10,
                            AnisotropicFilter = GXAnisotropy.GX_ANISO_4,
                            Unk0x0C = 0,
                            Unk0x12 = TexFlags0x10.unk4 | TexFlags0x10.unk5,
                        },
                };
                var textureHashes = new string[]
                {
                        "b42318832be6f79480973fddd2b4e0ac", // st01 tex 2 - mut unused
                                                            //"c8e2ea0bfdbbe3960ca2ec4c8af96b1c", // st01 tex 41 - com
                };
                var material = new Material
                {
                    MaterialColor = new GXColor(0xb2b2b2ff),
                    AmbientColor = new GXColor(0x7f7f7fff),
                    SpecularColor = new GXColor(0xFFFFFFFF),
                    Unk0x10 = MatFlags0x10.unk1 | MatFlags0x10.unk3 | MatFlags0x10.unk5,
                    Alpha = 255,
                    UnkAlpha0x14 = -1,
                    Unk0x15 = 0,
                };
                var submesh = new Submesh()
                {
                    RenderFlags = 0,
                    Material = material,
                    UnkAlphaOptions = new UnkAlphaOptions(),
                };

                var template = new GcmfTemplate()
                {
                    IsTranslucid = false,
                    Submesh = submesh,
                    TevLayers = tevLayers,
                    TextureHashes = textureHashes,
                    TextureScrollFields = null,
                };

                Assert.IsTrue(textureHashes.Length == tevLayers.Length);
                return template;
            }
            public static GcmfTemplate RoadBottom()
            {
                var tevLayers = new TevLayer[]
                {
                        new TevLayer()
                        {
                            Unk0x00 = 0,
                            MipmapSetting = MipmapSetting.ENABLE_MIPMAP | MipmapSetting.UNK_FLAG_1 | MipmapSetting.UNK_FLAG_2,
                            WrapMode = TextureWrapMode.mirrorX | TextureWrapMode.repeatY | TextureWrapMode.unk6 | TextureWrapMode.unk7,
                            LodBias = -10,
                            AnisotropicFilter = GXAnisotropy.GX_ANISO_4,
                            Unk0x0C = 216,
                            Unk0x12 = TexFlags0x10.unk4 | TexFlags0x10.unk5,
                        },
                };
                var textureHashes = new string[]
                {
                        "bd3f966c9db76827c5db9a032d11dffa", // st01 tex 11
                };
                var material = new Material
                {
                    MaterialColor = new GXColor(0xb2b2b2ff),
                    AmbientColor = new GXColor(0x7f7f7fff),
                    SpecularColor = new GXColor(0xFFFFFFFF),
                    Unk0x10 = MatFlags0x10.unk1 | MatFlags0x10.unk3 | MatFlags0x10.unk5,
                    Alpha = 255,
                    UnkAlpha0x14 = -1,
                    Unk0x15 = 0,
                };
                var submesh = new Submesh()
                {
                    RenderFlags = 0,
                    Material = material,
                    UnkAlphaOptions = new UnkAlphaOptions(),
                };

                var template = new GcmfTemplate()
                {
                    IsTranslucid = false,
                    Submesh = submesh,
                    TevLayers = tevLayers,
                    TextureHashes = textureHashes,
                    TextureScrollFields = null,
                };

                Assert.IsTrue(textureHashes.Length == tevLayers.Length);
                return template;
            }
            // TODO: generic
            public static GcmfTemplate RoadSides()
            {
                var tevLayers = new TevLayer[]
                {
                        new TevLayer()
                        {
                            Unk0x00 = 0,
                            MipmapSetting = MipmapSetting.ENABLE_MIPMAP | MipmapSetting.UNK_FLAG_1 | MipmapSetting.UNK_FLAG_2,
                            WrapMode = TextureWrapMode.repeatY | TextureWrapMode.unk6 | TextureWrapMode.unk7,
                            LodBias = -10,
                            AnisotropicFilter = GXAnisotropy.GX_ANISO_4,
                            Unk0x0C = 0,
                            Unk0x12 = TexFlags0x10.unk4 | TexFlags0x10.unk5,
                        },
                };
                var textureHashes = new string[]
                {
                        "d34923c1e44fa9bd58283b123b4a708a", // st01 tex 10
                };
                var material = new Material
                {
                    MaterialColor = new GXColor(0xb2b2b2ff),
                    AmbientColor = new GXColor(0x7f7f7fff),
                    SpecularColor = new GXColor(0xFFFFFFFF),
                    Unk0x10 = MatFlags0x10.unk1 | MatFlags0x10.unk3 | MatFlags0x10.unk5,
                    Alpha = 255,
                    UnkAlpha0x14 = -1,
                    Unk0x15 = 0,
                };
                var submesh = new Submesh()
                {
                    RenderFlags = 0,
                    Material = material,
                    UnkAlphaOptions = new UnkAlphaOptions(),
                };
                var template = new GcmfTemplate()
                {
                    IsTranslucid = false,
                    Submesh = submesh,
                    TevLayers = tevLayers,
                    TextureHashes = textureHashes,
                    TextureScrollFields = null,
                };

                Assert.IsTrue(textureHashes.Length == tevLayers.Length);
                return template;
            }
            public static GcmfTemplate RoadEmbelishments()
            {
                var tevLayers = new TevLayer[]
                {
                        new TevLayer()
                        {
                            Unk0x00 = 0,
                            MipmapSetting = MipmapSetting.ENABLE_MIPMAP | MipmapSetting.UNK_FLAG_1 | MipmapSetting.UNK_FLAG_2,
                            WrapMode = TextureWrapMode.repeatY | TextureWrapMode.unk6 | TextureWrapMode.unk7,
                            LodBias = -10,
                            AnisotropicFilter = GXAnisotropy.GX_ANISO_4,
                            Unk0x0C = 61,
                            Unk0x12 = TexFlags0x10.unk4 | TexFlags0x10.unk5,
                        },
                        new TevLayer()
                        {
                            Unk0x00 = 0,
                            MipmapSetting = MipmapSetting.ENABLE_MIPMAP | MipmapSetting.UNK_FLAG_1 | MipmapSetting.UNK_FLAG_2,
                            WrapMode = TextureWrapMode.mirrorX | TextureWrapMode.mirrorY | TextureWrapMode.unk7,
                            LodBias = -10,
                            AnisotropicFilter = GXAnisotropy.GX_ANISO_1,
                            Unk0x0C = 45,
                            Unk0x12 = TexFlags0x10.CheapAlphaOnSelf | TexFlags0x10.unk4 | TexFlags0x10.unk5,
                        },
                };
                var textureHashes = new string[]
                {
                        "533b7e7a43510b21a883b35e1120c60f", // st01 tex 9
                        "b978ad119120a4cadd428707eefc2a5e", // st01 tex 8
                };
                var material = new Material
                {
                    MaterialColor = new GXColor(0xb2b2b2ff),
                    AmbientColor = new GXColor(0x7f7f7fff),
                    SpecularColor = new GXColor(0xFFFFFFFF),
                    Unk0x10 = MatFlags0x10.unk1 | MatFlags0x10.unk3 | MatFlags0x10.unk5,
                    Alpha = 255,
                    UnkAlpha0x14 = -1,
                    Unk0x15 = 0,
                };
                var submesh = new Submesh()
                {
                    RenderFlags = 0,
                    Material = material,
                    UnkAlphaOptions = new UnkAlphaOptions(),
                };

                var template = new GcmfTemplate()
                {
                    IsTranslucid = false,
                    Submesh = submesh,
                    TevLayers = tevLayers,
                    TextureHashes = textureHashes,
                    TextureScrollFields = null,
                };

                Assert.IsTrue(textureHashes.Length == tevLayers.Length);
                return template;
            }
            public static GcmfTemplate RoadLaneDividers()
            {
                var tevLayers = new TevLayer[]
                {
                        new TevLayer()
                        {
                            Unk0x00 = 0,
                            MipmapSetting = MipmapSetting.ENABLE_MIPMAP | MipmapSetting.UNK_FLAG_1 | MipmapSetting.UNK_FLAG_2,
                            WrapMode = TextureWrapMode.repeatY | TextureWrapMode.unk7,
                            LodBias = -10,
                            AnisotropicFilter = GXAnisotropy.GX_ANISO_1,
                            Unk0x0C = 0,
                            Unk0x12 = TexFlags0x10.unk4 | TexFlags0x10.unk5,
                        },
                };
                var textureHashes = new string[]
                {
                        "8d92bad8c4d1eb2e46aeb25b9e11e9cf", // st01 tex 12
                };
                var material = new Material
                {
                    MaterialColor = new GXColor(0x697db2ff),
                    AmbientColor = new GXColor(0x7f7f7fff),
                    SpecularColor = new GXColor(0x00000000),
                    Unk0x10 = 0,
                    Alpha = 255,
                    UnkAlpha0x14 = -1,
                    Unk0x15 = 0,
                };
                var submesh = new Submesh()
                {
                    RenderFlags = RenderFlags.unlit | RenderFlags.screenBlend | RenderFlags.additiveBlend,
                    Material = material,
                    UnkAlphaOptions = new UnkAlphaOptions()
                    {
                        BlendFactors = BlendFactors.unk2_UseBlendMode | BlendFactors.unk4_UseBlendMode,
                    },
                };
                var template = new GcmfTemplate()
                {
                    IsTranslucid = true,
                    Submesh = submesh,
                    TevLayers = tevLayers,
                    TextureHashes = textureHashes,
                    TextureScrollFields = null,
                };

                Assert.IsTrue(textureHashes.Length == tevLayers.Length);
                return template;
            }

            public static GcmfTemplate[] Road()
            {
                return new GcmfTemplate[]
                {
                    RoadTop(),
                    RoadBottom(),
                    RoadSides(),
                    RoadEmbelishments(),
                    RoadLaneDividers(),
                    RoadRails(),
                };
            }
        }

        public static class MuteCityCOM
        {
            public static GcmfTemplate RoadTopEmbeddedDividers()
            {
                var tevLayers = new TevLayer[]
                {
                        new TevLayer()
                        {
                            Unk0x00 = 0,
                            MipmapSetting = MipmapSetting.ENABLE_MIPMAP | MipmapSetting.UNK_FLAG_1 | MipmapSetting.UNK_FLAG_2,
                            WrapMode = TextureWrapMode.mirrorX | TextureWrapMode.repeatY | TextureWrapMode.unk6 | TextureWrapMode.unk7,
                            LodBias = -20,
                            AnisotropicFilter = GXAnisotropy.GX_ANISO_4,
                            Unk0x0C = 0,
                            Unk0x12 = TexFlags0x10.unk4 | TexFlags0x10.unk5,
                        },
                        new TevLayer()
                        {
                            Unk0x00 = 0,
                            MipmapSetting = MipmapSetting.ENABLE_MIPMAP | MipmapSetting.UNK_FLAG_1 | MipmapSetting.UNK_FLAG_2,
                            WrapMode = TextureWrapMode.mirrorX | TextureWrapMode.mirrorY | TextureWrapMode.unk6 | TextureWrapMode.unk7,
                            LodBias = -10,
                            AnisotropicFilter = GXAnisotropy.GX_ANISO_4,
                            Unk0x0C = 14,
                            Unk0x12 = TexFlags0x10.CheapAlphaOnSelf | TexFlags0x10.unk4 | TexFlags0x10.unk5,
                        },
                };
                var textureHashes = new string[]
                {
                        "c8e2ea0bfdbbe3960ca2ec4c8af96b1c", // st03 tex 75, com surface
                        "ca22dae46426901c8fa217e9112edcaa", // st03 tex 76, com divider width span
                };
                var material = new Material
                {
                    MaterialColor = new GXColor(0xb2b2b2ff),
                    AmbientColor = new GXColor(0x7f7f7fff),
                    SpecularColor = new GXColor(0xFFFFFFFF),
                    Unk0x10 = MatFlags0x10.unk1 | MatFlags0x10.unk3 | MatFlags0x10.unk5,
                    Alpha = 255,
                    UnkAlpha0x14 = -1,
                    Unk0x15 = 0,
                };
                var submesh = new Submesh()
                {
                    RenderFlags = 0,
                    Material = material,
                    UnkAlphaOptions = new UnkAlphaOptions(),
                };

                var template = new GcmfTemplate()
                {
                    IsTranslucid = false,
                    Submesh = submesh,
                    TevLayers = tevLayers,
                    TextureHashes = textureHashes,
                    TextureScrollFields = null,
                };

                Assert.IsTrue(textureHashes.Length == tevLayers.Length);
                return template;
            }
            public static GcmfTemplate RoadTopNoDividers()
            {
                // Remove second tex/tev which is the dividers
                var template = RoadTopEmbeddedDividers();
                template.TextureHashes = new string[] { template.TextureHashes[0] };
                template.TevLayers = new TevLayer[] { template.TevLayers[0] };
                return template;
            }
            public static GcmfTemplate[] Road()
            {
                return new GcmfTemplate[]
                {
                    RoadTopEmbeddedDividers(),
                    MuteCity.RoadBottom(),
                    MuteCity.RoadSides(),
                    MuteCity.RoadEmbelishments(),
                    MuteCity.RoadRails(),
                };
            }
            public static GcmfTemplate[] RoadNoDividers()
            {
                return new GcmfTemplate[]
                {
                    RoadTopNoDividers(),
                    MuteCity.RoadBottom(),
                    MuteCity.RoadSides(),
                    MuteCity.RoadEmbelishments(),
                    MuteCity.RoadRails(),
                };
            }
        }


        public static class Aeropolis
        {

        }

        public static class BigBlue
        {

        }

        public static class CasinoPalace
        {

        }

        public static class CosmoTerminal
        {

        }

        public static class FireField
        {

        }

        public static class GreenPlant
        {

        }

        public static class Lightning
        {

        }

        public static class OuterSpace
        {

        }

        public static class PhantomRoad
        {

        }

        public static class PortTown
        {

        }

        public static class SandOcean
        {

        }
    }
}
