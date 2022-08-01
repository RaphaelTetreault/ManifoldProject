using GameCube.GFZ;
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
//
using Unity.Mathematics;
using Manifold.EditorTools.GC.GFZ.Stage.Track;

namespace Manifold.EditorTools.GC.GFZ
{
    public static partial class GfzAssetTemplates
    {
        public static class MeshTemplates
        {
            public static class DebugTemplates
            {
                public static GcmfTemplate CreateLitVertexColored()
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

                    var template = new GcmfTemplate()
                    {
                        Opaque = 1,
                        Submesh = submesh,
                        TevLayers = tevLayers,
                        TextureHashes = textureHashes,
                        TextureScroll = textureScroll,
                    };

                    Assert.IsTrue(textureHashes.Length == tevLayers.Length);
                    return template;
                }
                public static GcmfTemplate CreateUnlitVertexColored()
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

                    var template = new GcmfTemplate()
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
                // TODO: tev layers wrong -- you didn't really check!

                public static GcmfTemplate CreateRails()
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

                    var template = new GcmfTemplate()
                    {
                        Translucid = 1,
                        Submesh = submesh,
                        TevLayers = tevLayers,
                        TextureHashes = textureHashes,
                        TextureScroll = textureScroll,
                    };

                    Assert.IsTrue(textureHashes.Length == tevLayers.Length);
                    return template;
                }
                public static GcmfTemplate CreateRoadTop()
                {
                    // Not doing isSwappable, tpl index, config index
                    var tevLayers = new TevLayer[]
                    {
                        new TevLayer()
                        {
                            Unk0x00 = 0,
                            MipmapSetting = MipmapSetting.UNK_FLAG_1,
                            WrapMode = TextureWrapMode.mirrorX | TextureWrapMode.repeatY,
                            LodBias = 0,
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
                    };
                    var unknownAlphaOptions = new UnkAlphaOptions() { };
                    var submesh = new Submesh()
                    {
                        RenderFlags = 0,
                        Material = material,
                        UnkAlphaOptions = unknownAlphaOptions,
                    };
                    var template = new GcmfTemplate()
                    {
                        Opaque = 1,
                        Submesh = submesh,
                        TevLayers = tevLayers,
                        TextureHashes = textureHashes,
                        TextureScroll = textureScroll,
                    };

                    Assert.IsTrue(textureHashes.Length == tevLayers.Length);
                    return template;
                }
                public static GcmfTemplate CreateRoadBottom()
                {
                    // Not doing isSwappable, tpl index, config index
                    var tevLayers = new TevLayer[]
                    {
                        new TevLayer()
                        {
                            Unk0x00 = 0,
                            MipmapSetting = MipmapSetting.UNK_FLAG_4,
                            WrapMode = TextureWrapMode.repeatX | TextureWrapMode.repeatY,
                            LodBias = 0,
                            AnisotropicFilter = GXAnisotropy.GX_ANISO_1,
                            Unk0x0C = 0,
                            Unk0x12 = TexFlags0x10.unk4 | TexFlags0x10.unk5,
                        },
                    };
                    var textureHashes = new string[]
                    {
                        "bd3f966c9db76827c5db9a032d11dffa", // st01 tex 11
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
                    };
                    var unknownAlphaOptions = new UnkAlphaOptions() { };
                    var submesh = new Submesh()
                    {
                        RenderFlags = 0,
                        Material = material,
                        UnkAlphaOptions = unknownAlphaOptions,
                    };
                    var template = new GcmfTemplate()
                    {
                        Opaque = 1,
                        Submesh = submesh,
                        TevLayers = tevLayers,
                        TextureHashes = textureHashes,
                        TextureScroll = textureScroll,
                    };

                    Assert.IsTrue(textureHashes.Length == tevLayers.Length);
                    return template;
                }
                public static GcmfTemplate CreateRoadSides()
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
                            AnisotropicFilter = GXAnisotropy.GX_ANISO_4,
                            Unk0x0C = 0,
                            Unk0x12 = TexFlags0x10.unk4 | TexFlags0x10.unk5,
                        },
                    };
                    var textureHashes = new string[]
                    {
                        "d34923c1e44fa9bd58283b123b4a708a", // st01 tex 10
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
                    };
                    var unknownAlphaOptions = new UnkAlphaOptions() { };
                    var submesh = new Submesh()
                    {
                        RenderFlags = 0,
                        Material = material,
                        UnkAlphaOptions = unknownAlphaOptions,
                    };
                    var template = new GcmfTemplate()
                    {
                        //Gcmf = gcmf,
                        Opaque = 1,
                        Submesh = submesh,
                        TevLayers = tevLayers,
                        TextureHashes = textureHashes,
                        TextureScroll = textureScroll,
                    };

                    Assert.IsTrue(textureHashes.Length == tevLayers.Length);
                    return template;
                }
                public static GcmfTemplate CreateRoadEmbelishments()
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
                            Unk0x12 = TexFlags0x10.unk0 | TexFlags0x10.unk4 | TexFlags0x10.unk5,
                        },
                    };
                    var textureHashes = new string[]
                    {
                        "533b7e7a43510b21a883b35e1120c60f", // st01 tex 9
                        "b978ad119120a4cadd428707eefc2a5e", // st01 tex 8
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
                    };
                    var unknownAlphaOptions = new UnkAlphaOptions() { };
                    var submesh = new Submesh()
                    {
                        RenderFlags = 0,
                        Material = material,
                        UnkAlphaOptions = unknownAlphaOptions,
                    };
                    var template = new GcmfTemplate()
                    {
                        //Gcmf = gcmf,
                        Opaque = 1,
                        Submesh = submesh,
                        TevLayers = tevLayers,
                        TextureHashes = textureHashes,
                        TextureScroll = textureScroll,
                    };

                    Assert.IsTrue(textureHashes.Length == tevLayers.Length);
                    return template;
                }
                public static GcmfTemplate CreateLaneDividers()
                {
                    // Not doing isSwappable, tpl index, config index
                    var tevLayers = new TevLayer[]
                    {
                        new TevLayer()
                        {
                            Unk0x00 = 0,
                            MipmapSetting = MipmapSetting.ENABLE_MIPMAP | MipmapSetting.UNK_FLAG_1 | MipmapSetting.UNK_FLAG_2,
                            WrapMode = TextureWrapMode.repeatX | TextureWrapMode.repeatY,
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
                    var textureScroll = new TextureScroll { Fields = new TextureScrollField[12] };
                    textureScroll.Fields[0] = new TextureScrollField(0, 30);

                    var material = new Material
                    {
                        MaterialColor = new GXColor(0x697db2ff),
                        AmbientColor = new GXColor(0x7f7f7fff),
                        SpecularColor = new GXColor(0x00000000),
                        Unk0x10 = 0,
                        Alpha = 255,
                        TevLayerCount = (byte)tevLayers.Length,
                        UnkAlpha0x14 = 0,
                        Unk0x15 = 0,
                    };
                    var unknownAlphaOptions = new UnkAlphaOptions()
                    {
                        Unk0x10 = BlendFactors.unk2 | BlendFactors.unk4,
                    };
                    var submesh = new Submesh()
                    {
                        RenderFlags = RenderFlags.unlit | RenderFlags.customBlendSource | RenderFlags.customBlendDestination,
                        Material = material,
                        UnkAlphaOptions = unknownAlphaOptions,
                    };
                    var template = new GcmfTemplate()
                    {
                        Translucid = 1,
                        Submesh = submesh,
                        TevLayers = tevLayers,
                        TextureHashes = textureHashes,
                        TextureScroll = textureScroll,
                    };

                    Assert.IsTrue(textureHashes.Length == tevLayers.Length);
                    return template;
                }
            }
        }
    }

    public class GcmfTemplate
    {
        public byte Opaque { get; internal set; }
        public byte Translucid { get; internal set; }
        public Submesh Submesh { get; internal set; } = null;
        public TevLayer[] TevLayers { get; internal set; } = new TevLayer[0];
        public string[] TextureHashes { get; internal set; } = new string[0];
        public TextureScroll TextureScroll { get; internal set; } = null;


        private static Gcmf CreateFromTemplates(ref Dictionary<string, ushort> textureHashesToIndex, params GcmfTemplate[] templates)
        {
            ushort opaque = 0;
            ushort translucid = 0;
            var tevLayers = new List<TevLayer>();
            var submeshes = new Submesh[templates.Length];
            // todo: texture indexes and stuff

            ushort tevLayerOffset = 0;
            for (int templateIndex = 0; templateIndex < templates.Length; templateIndex++)
            {
                var template = templates[templateIndex];
                //var textureHashes = template.TextureHashes;

                // TEV LAYERS
                for (ushort tevIndex = 0; tevIndex < template.TevLayers.Length; tevIndex++)
                {
                    // Get texture index//
                    string textureHash = template.TextureHashes[tevIndex];
                    ushort textureIndex = GetTextureHashesIndex(textureHash, ref textureHashesToIndex);
                    // Get TEV index for this GCMF
                    ushort tevLayerIndex = checked((ushort)(tevIndex + tevLayerOffset));
                    TevLayer tevLayer = template.TevLayers[tevIndex];
                    // Assign indexes
                    tevLayer.TplTextureIndex = textureIndex;
                    tevLayer.TevLayerIndex = tevLayerIndex;

                    if (tevIndex == 0)
                        template.Submesh.Material.TevLayerIndex0 = (short)tevLayerIndex;
                    else if (tevIndex == 1)
                        template.Submesh.Material.TevLayerIndex1 = (short)tevLayerIndex;
                    else if (tevIndex == 2)
                        template.Submesh.Material.TevLayerIndex2 = (short)tevLayerIndex;
                }
                tevLayers.AddRange(template.TevLayers);
                tevLayerOffset = (ushort)tevLayers.Count;
                template.Submesh.Material.TevLayerCount = (byte)template.TevLayers.Length;

                // collect submesh
                submeshes[templateIndex] = template.Submesh;

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

        private static void AssignDisplayListsToGcmf(Gcmf gcmf, Tristrip[][] tristrips)
        {
            if (tristrips.Length != gcmf.Submeshes.Length)
                throw new System.ArgumentException("lengths do not match! Did you forget to assign equal tristrip and material templates?");

            for (int i = 0; i < gcmf.Submeshes.Length; i++)
            {
                var frontfacing = new List<Tristrip>();
                var backfacing = new List<Tristrip>();
                foreach (var tristrip in tristrips[i])
                {
                    if (tristrip.isBackFacing)
                        backfacing.Add(tristrip);
                    else
                        frontfacing.Add(tristrip);
                }
                var submesh = gcmf.Submeshes[i];
                submesh.PrimaryFrontFacing = TristripGenerator.TristripsToDisplayLists(frontfacing.ToArray(), GameCube.GFZ.GfzGX.VAT);
                submesh.PrimaryBackFacing = TristripGenerator.TristripsToDisplayLists(backfacing.ToArray(), GameCube.GFZ.GfzGX.VAT);
                submesh.VertexAttributes = TristripToAttribute(tristrips[i]);
            }
        }

        private static AttributeFlags TristripToAttribute(params Tristrip[] tristrips)
        {
            // This function is a big'ol hack

            // Return empty descriptor if nothing
            if (tristrips.IsNullOrEmpty())
                return 0;

            // Else grab attributes from first entry
            var dl = TristripGenerator.TristripsToDisplayLists(new Tristrip[] { tristrips[0] }, GameCube.GFZ.GfzGX.VAT);
            var attributes = dl[0].Attributes;
            return attributes;
        }

        // Modified from: http://www.technologicalutopia.com/sourcecode/xnageometry/boundingsphere.cs.htm
        private static GameCube.GFZ.BoundingSphere CreateBoundingSphereFromPoints(IEnumerable<UnityEngine.Vector3> points, int length)
        {
            if (points == null)
                throw new System.ArgumentNullException(nameof(points));
            if (length <= 0)
                throw new System.ArgumentOutOfRangeException(nameof(length));

            float radius = 0;
            float3 center = new float3();
            float lengthReciprocal = 1f / length;

            // First, we'll find the center of gravity for the point 'cloud'.
            foreach (var point in points)
            {
                float3 pointWeighted = point * lengthReciprocal;
                center += pointWeighted;
            }

            // Calculate the radius of the needed sphere (it equals the distance between the center and the point further away).
            foreach (var point in points)
            {
                float3 centerToPoint = (float3)point - center;
                float distance = math.length(centerToPoint);

                if (distance > radius)
                    radius = distance;
            }

            return new BoundingSphere(center, radius);
        }
        private static BoundingSphere CreateBoundingSphereFromTristrips(IEnumerable<Tristrip> tristrips)
        {
            var points = new List<UnityEngine.Vector3>();
            foreach (var tristrip in tristrips)
                points.AddRange(tristrip.positions);

            var boundingSphere = CreateBoundingSphereFromPoints(points, points.Count);
            return boundingSphere;
        }
        private static BoundingSphere CreateBoundingSphere(Tristrip[][] tristripsCollections)
        {
            // Linearize tristrips
            var allTristrips = new List<Tristrip>();
            foreach (var tristrips in tristripsCollections)
                allTristrips.AddRange(tristrips);

            var boundingSphere = CreateBoundingSphereFromTristrips(allTristrips);

            return boundingSphere;
        }

        public static Gcmf CreateGcmf(GcmfTemplate[] templates, Tristrip[][] tristripsCollection, ref Dictionary<string, ushort> textureHashesToIndex)
        {
            // Combine templates. This means we have all TEV layers and stuff in place
            var gcmf = CreateFromTemplates(ref textureHashesToIndex, templates);

            // Assign display lists: now each submesh has it's geometry, too
            AssignDisplayListsToGcmf(gcmf, tristripsCollection);

            // Create a bounding sphere and assign data everywhere it is used
            var boundingSphere = CreateBoundingSphere(tristripsCollection);
            gcmf.BoundingSphere = boundingSphere;
            foreach (var submesh in gcmf.Submeshes)
                submesh.UnkAlphaOptions.Origin = boundingSphere.origin;

            // We done!
            return gcmf;
        }

        public static TextureScroll CombineTextureScrolls(GcmfTemplate[] templates)
        {
            return null;
        }

        // Get the index of the texture for a future TPL. If texture exists, get existing index. If not, add to list, update index.
        private static ushort GetTextureHashesIndex(string textureHash, ref Dictionary<string, ushort> textureHashToIndex)
        {
            bool hasTexture = textureHashToIndex.ContainsKey(textureHash);
            if (hasTexture)
            {
                ushort index = textureHashToIndex[textureHash];
                return index;
            }
            else
            {
                // The index of the texture is where we place it in the list
                ushort index = checked((ushort)textureHashToIndex.Count);
                textureHashToIndex.Add(textureHash, index);
                return index;
            }
        }
    }
}
