using GameCube.GFZ.GMA;
using GameCube.GFZ.Stage;
using GameCube.GFZ.TPL;
using GameCube.GX;
using GameCube.GX.Texture;
using Manifold.IO;
using Manifold.EditorTools.GC.GFZ;
using Manifold.EditorTools.GC.GFZ.TPL;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Manifold.EditorTools.GC.GFZ
{
    public static partial class GfzAssetTemplates
    {
        public static class MeshTemplates
        {
            public static class DebugTemplates
            {
                public static MeshTemplate CreateLitVertexColored()
                {
                    // Not doing isSwappable, tpl index, config index
                    var tevLayers = new TevLayer[0];
                    var textureHashes = new string[0];
                    TextureScroll textureScroll = null;

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

                    var template = new MeshTemplate()
                    {
                        Opaque = 1,
                        Submesh = submesh,
                        TevLayers = tevLayers,
                        TextureHashes = textureHashes,
                        TextureScroll = textureScroll,
                    };

                    return template;
                }
                public static MeshTemplate CreateUnlitVertexColored()
                {
                    // Not doing isSwappable, tpl index, config index
                    var tevLayers = new TevLayer[0];
                    var textureHashes = new string[0];
                    TextureScroll textureScroll = null;

                    var material = new Material();
                    var unknownAlphaOptions = new UnkAlphaOptions();
                    var submesh = new Submesh()
                    {
                        RenderFlags = RenderFlags.unlit | RenderFlags.doubleSidedFaces,
                        Material = material,
                        UnkAlphaOptions = unknownAlphaOptions,
                    };

                    var template = new MeshTemplate()
                    {
                        Opaque = 1,
                        Submesh = submesh,
                        TevLayers = tevLayers,
                        TextureHashes = textureHashes,
                        TextureScroll = textureScroll,
                    };

                    return template;
                }
            }

            public static class MuteCity
            {
                public static MeshTemplate CreateRail()
                {
                    // Not doing isSwappable, tpl index, config index
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
                            // TEMP
                            TplTextureIndex = 3,
                        },
                        new TevLayer()
                        {
                            Unk0x00 = 0, // TexFlags0x00.ENABLE_UV_SCROLL,
                            MipmapSetting = MipmapSetting.UNK_FLAG_5,
                            WrapMode = TextureWrapMode.repeatX | TextureWrapMode.repeatY,
                            LodBias = 0,
                            AnisotropicFilter = GXAnisotropy.GX_ANISO_1,
                            Unk0x0C = 0,
                            Unk0x12 = TexFlags0x10.unk4 | TexFlags0x10.unk5,
                            // TEMP
                            TplTextureIndex = 40,
                        },
                        new TevLayer()
                        {
                            Unk0x00 = 0,
                            MipmapSetting = MipmapSetting.UNK_FLAG_5,
                            WrapMode = TextureWrapMode.repeatX | TextureWrapMode.repeatY,
                            LodBias = 0,
                            AnisotropicFilter = GXAnisotropy.GX_ANISO_1,
                            Unk0x0C = 0,
                            Unk0x12 = TexFlags0x10.unk4 | TexFlags0x10.unk5,
                            // TEMP
                            TplTextureIndex = 0,
                        },
                    };
                    var textureHashes = new string[]
                    {
                        "ca0853c45448b241aa2b03cbe2b93182", // st01 tex  3
                        "9ed3039353de68dbbf59d8904f7bb00f", // st01 tex 40
                        "390204a0d91287427073649ec4efc80f", // st01 tex  0
                    };
                    var textureScroll = new TextureScroll { Fields = new TextureScrollField[12] };
                    textureScroll.Fields[0] = new TextureScrollField(0, 30);

                    var material = new Material
                    {
                        MaterialColor = new GameCube.GX.GXColor(0xb2b2b2ff),
                        AmbientColor = new GameCube.GX.GXColor(0x7f7f7fff),
                        SpecularColor = new GameCube.GX.GXColor(0x00000000),
                        Unk0x10 = 0,
                        Alpha = 255,
                        TevLayerCount = (byte)tevLayers.Length,
                        //MaterialDestination = 0, // Resolved based on display lists at serialize time
                        UnkAlpha0x14 = 0,
                        Unk0x15 = 0,
                        TevLayerIndex0 = 0,
                        TevLayerIndex1 = 1,
                        TevLayerIndex2 = 2,
                    };
                    var unknownAlphaOptions = new UnkAlphaOptions()
                    {
                        // origin?
                        Unk0x10 = BlendFactors.unk2 | BlendFactors.unk4,
                    };
                    var submesh = new Submesh()
                    {
                        RenderFlags = RenderFlags.unlit | RenderFlags.doubleSidedFaces | RenderFlags.customBlendSource | RenderFlags.customBlendDestination,
                        Material = material,
                        UnkAlphaOptions = unknownAlphaOptions,
                        // GX attributes
                    };

                    var template = new MeshTemplate()
                    {
                        Translucid = 1,
                        Submesh = submesh,
                        TevLayers = tevLayers,
                        TextureHashes = textureHashes,
                        TextureScroll = textureScroll,
                    };

                    return template;
                }
                public static MeshTemplate CreateRoadTop()
                {
                    // Not doing isSwappable, tpl index, config index
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
                            // TEMP
                            TplTextureIndex = 2, // 41 is com
                        },
                    };
                    var textureHashes = new string[]
                    {
                        "c148529dca41e47c9d2ff15fe0d294bb", // st01 tex 2 - mut unused
                        //"083a0143d9afb40b27fcbe39bf95b50f", // st01 tex 41 - com
                    };
                    var textureScroll = new TextureScroll { Fields = new TextureScrollField[12] };
                    textureScroll.Fields[0] = new TextureScrollField(0, 30);

                    var material = new Material
                    {
                        MaterialColor = new GXColor(0xb2b2b2ff),
                        AmbientColor = new GXColor(0x7f7f7fff),
                        SpecularColor = new GXColor(0xFFFFFFFF),
                        Unk0x10 = MatFlags0x10.unk1 | MatFlags0x10.unk3 | MatFlags0x10.unk5,
                        Alpha = 255,
                        TevLayerCount = (byte)tevLayers.Length,
                        UnkAlpha0x14 = -1,
                        Unk0x15 = 0,
                        TevLayerIndex0 = 0,
                        TevLayerIndex1 = -1,
                        TevLayerIndex2 = -1,
                    };
                    var unknownAlphaOptions = new UnkAlphaOptions() { };
                    var submesh = new Submesh()
                    {
                        RenderFlags = 0,
                        Material = material,
                        UnkAlphaOptions = unknownAlphaOptions,
                    };
                    var template = new MeshTemplate()
                    {
                        //Gcmf = gcmf,
                        Opaque = 1,
                        Submesh = submesh,
                        TevLayers = tevLayers,
                        TextureHashes = textureHashes,
                        TextureScroll = textureScroll,
                    };

                    return template;
                }
                public static MeshTemplate CreateRoadBottom()
                {
                    // Not doing isSwappable, tpl index, config index
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
                            // TEMP
                            TplTextureIndex = 11, // 41 is com
                        },
                    };
                    var textureHashes = new string[]
                    {
                        "069afe4a631dbe56398811fb0ef0b8f0", // st01 tex 3
                    };
                    var textureScroll = new TextureScroll { Fields = new TextureScrollField[12] };
                    textureScroll.Fields[0] = new TextureScrollField(0, 30);

                    var material = new Material
                    {
                        MaterialColor = new GXColor(0xb2b2b2ff),
                        AmbientColor = new GXColor(0x7f7f7fff),
                        SpecularColor = new GXColor(0xFFFFFFFF),
                        Unk0x10 = MatFlags0x10.unk1 | MatFlags0x10.unk3 | MatFlags0x10.unk5,
                        Alpha = 255,
                        TevLayerCount = (byte)tevLayers.Length,
                        UnkAlpha0x14 = -1,
                        Unk0x15 = 0,
                        TevLayerIndex0 = 0,
                        TevLayerIndex1 = -1,
                        TevLayerIndex2 = -1,
                    };
                    var unknownAlphaOptions = new UnkAlphaOptions() { };
                    var submesh = new Submesh()
                    {
                        RenderFlags = 0,
                        Material = material,
                        UnkAlphaOptions = unknownAlphaOptions,
                    };
                    var template = new MeshTemplate()
                    {
                        //Gcmf = gcmf,
                        Opaque = 1,
                        Submesh = submesh,
                        TevLayers = tevLayers,
                        TextureHashes = textureHashes,
                        TextureScroll = textureScroll,
                    };

                    return template;
                }
                public static MeshTemplate CreateRoadSides()
                {
                    // Not doing isSwappable, tpl index, config index
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
                            // TEMP
                            TplTextureIndex = 10,
                        },
                    };
                    var textureHashes = new string[]
                    {
                        "069afe4a631dbe56398811fb0ef0b8f0", // st01 tex 3
                    };
                    var textureScroll = new TextureScroll { Fields = new TextureScrollField[12] };
                    textureScroll.Fields[0] = new TextureScrollField(0, 30);

                    var material = new Material
                    {
                        MaterialColor = new GXColor(0xb2b2b2ff),
                        AmbientColor = new GXColor(0x7f7f7fff),
                        SpecularColor = new GXColor(0xFFFFFFFF),
                        Unk0x10 = MatFlags0x10.unk1 | MatFlags0x10.unk3 | MatFlags0x10.unk5,
                        Alpha = 255,
                        TevLayerCount = (byte)tevLayers.Length,
                        UnkAlpha0x14 = -1,
                        Unk0x15 = 0,
                        TevLayerIndex0 = 0,
                        TevLayerIndex1 = -1,
                        TevLayerIndex2 = -1,
                    };
                    var unknownAlphaOptions = new UnkAlphaOptions() { };
                    var submesh = new Submesh()
                    {
                        RenderFlags = 0,
                        Material = material,
                        UnkAlphaOptions = unknownAlphaOptions,
                    };
                    var template = new MeshTemplate()
                    {
                        //Gcmf = gcmf,
                        Opaque = 1,
                        Submesh = submesh,
                        TevLayers = tevLayers,
                        TextureHashes = textureHashes,
                        TextureScroll = textureScroll,
                    };

                    return template;
                }

            }
        }
    }

    public class MeshTemplate
    {
        public byte Opaque { get; internal set; }
        public byte Translucid { get; internal set; }
        public Submesh Submesh { get; internal set; } = null;
        public TevLayer[] TevLayers { get; internal set; } = new TevLayer[0];
        public string[] TextureHashes { get; internal set; } = new string[0];
        public TextureScroll TextureScroll { get; internal set; } = null;

        public static Gcmf CombineTemplates(params MeshTemplate[] templates)
        {
            ushort opaque = 0;
            ushort translucid = 0;
            var tevLayers = new List<TevLayer>();
            var submeshes = new Submesh[templates.Length];
            // todo: texture indexes and stuff

            short tevLayerOffset = 0;
            for (int i = 0; i < templates.Length; i++)
            {
                var template = templates[i];

                // Collect tev layers
                tevLayers.AddRange(template.TevLayers);
                // MAYBE: auto assign tev layer here? ignore if -1?

                // Offset material/mesh tev layers
                template.Submesh.Material.OffsetTevLayerIndices(tevLayerOffset);
                tevLayerOffset = (short)tevLayers.Count;
                // collect submesh
                submeshes[i] = template.Submesh;

                // TEMP: sort how many opaque / translucid
                opaque += template.Opaque;
                translucid += template.Translucid;
            }

            // Solved elsewhere: attributes, bounding sphere
            var gcmf = new Gcmf
            {
                TextureConfigsCount = (ushort)tevLayers.Count,
                OpaqueMaterialCount = opaque,
                TranslucidMaterialCount = translucid,
                TevLayers = tevLayers.ToArray(),
                Submeshes = submeshes,
            };
            gcmf.PatchTevLayerIndexes();

            return gcmf;
        }
    }

    public class GcmfTemplate
    {
        public string FileName { get; internal set; }
        public Pointer GcmfPtr { get; internal set; }

        //public Gcmf GetGcmf(string filePath)
        //{
        //    using (var reader = new EndianBinaryReader(File.OpenRead(filePath), Gma.endianness))
        //    {
        //        var gcmf = new Gcmf();
        //        reader.JumpToAddress(GcmfPtr);
        //        gcmf.Deserialize(reader);
        //        gcmf.Submeshes


        //        return gcmf;
        //    }
        //}

        /// <summary>
        /// Get the hashes for all textures used in GCMF.
        /// </summary>
        /// <param name="gcmf"></param>
        /// <param name="textureInfo"></param>
        /// <param name="rootDirectory"></param>
        /// <param name="TplTextureToTextureHash"></param>
        /// <returns></returns>
        public string[] GetGcmfTextureHashes(Gcmf gcmf, TextureInfo textureInfo, string rootDirectory, TplTextureToTextureHash TplTextureToTextureHash)
        {
            // Get file path to TPL where textures are stored
            var sourceTplFilePaths = Directory.GetFiles(rootDirectory, $"{textureInfo.SourceFileName}.tpl", SearchOption.AllDirectories);
            Assert.IsTrue(sourceTplFilePaths.Length == 1);
            var filePath = sourceTplFilePaths[0];

            // Load TPL
            var tpl = new Tpl();
            using (var reader = new EndianBinaryReader(File.OpenRead(filePath), Tpl.endianness))
                tpl.Deserialize(reader);

            // Get all the texture hashes of textures used in this TPL
            var lookup = TplTextureToTextureHash.GetDictionary();
            var tplTextureHashes = lookup[textureInfo.SourceFileName];

            // Get hashes only for textures used by GCMF
            int numTextures = gcmf.TevLayers.Length;
            var hashes = new string[numTextures];
            for (int i = 0; i < numTextures; i++)
            {
                var textureConfig = gcmf.TevLayers[i];
                var textureIndex = textureConfig.TplTextureIndex;
                var textureHash = tplTextureHashes.TextureHashes[textureIndex];
                hashes[i] = textureHash;
            }
            return hashes;
        }

    }
}
