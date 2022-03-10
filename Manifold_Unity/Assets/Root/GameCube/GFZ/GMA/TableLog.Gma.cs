using Manifold.IO;
using System.IO;

namespace GameCube.GFZ.GMA
{
    public class GmaTableLogger
    {
        // Names of files generated
        public static readonly string tsvGcmf = $"{nameof(Gma)}-{nameof(Gcmf)}.tsv";
        public static readonly string tsvTextureConfigs = $"{nameof(Gma)}-{nameof(TextureConfig)}.tsv";
        public static readonly string tsvMaterials = $"{nameof(Gma)}-{nameof(Material)}.tsv";

        public static void PrintGmaAll(Gma[] gmas, string outputDirectory)
        {
            {
                var fileName = Path.Combine(outputDirectory, tsvGcmf);
                PrintGcmf(gmas, fileName);
            }
            {
                var fileName = Path.Combine(outputDirectory, tsvTextureConfigs);
                PrintTextureConfigs(gmas, fileName);
            }
            {
                var fileName = Path.Combine(outputDirectory, tsvMaterials);
                PrintMaterials(gmas, fileName);
            }
        }

        public static void PrintGcmf(Gma[] gmas, string outputFileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(outputFileName))
            {
                // Write header
                writer.WriteNextCol("FileName");
                writer.WriteNextCol("Address");
                writer.WriteNextCol(nameof(Model.Name));
                writer.WriteNextCol("Model Index");
                writer.WriteNextCol(nameof(Gcmf.Attributes));
                writer.WriteNextCol(nameof(Gcmf.BoundingSphere));
                writer.WriteNextCol(nameof(Gcmf.TextureCount));
                writer.WriteNextCol(nameof(Gcmf.OpaqueMaterialCount));
                writer.WriteNextCol(nameof(Gcmf.TranslucidMaterialCount));
                writer.WriteNextCol(nameof(Gcmf.BoneCount));
                writer.WriteNextCol(nameof(Gcmf.GcmfTexturesSize));
                writer.WriteNextCol(nameof(Gcmf.SkinnedVertexDescriptor));
                writer.WriteNextCol(nameof(Gcmf.Submeshes));
                writer.WriteNextCol(nameof(Gcmf.SkinnedVerticesA));
                writer.WriteNextCol(nameof(Gcmf.SkinnedVerticesB));
                writer.WriteNextCol(nameof(Gcmf.SkinBoneBindings));
                writer.WriteNextCol(nameof(Gcmf.UnkBoneIndices));
                writer.WriteNextRow();

                foreach (var gma in gmas)
                {
                    int modelIndex = 0;
                    foreach (var model in gma.Models)
                    {
                        var gcmf = model.Gcmf;
                        writer.WriteNextCol(gma.FileName);
                        writer.WriteNextCol(gcmf.AddressRange.PrintStartAddress());
                        writer.WriteNextCol(model.Name);
                        writer.WriteNextCol(modelIndex++);
                        writer.WriteNextCol(gcmf.Attributes);
                        writer.WriteNextCol(gcmf.BoundingSphere);
                        writer.WriteNextCol(gcmf.TextureCount);
                        writer.WriteNextCol(gcmf.OpaqueMaterialCount);
                        writer.WriteNextCol(gcmf.TranslucidMaterialCount);
                        writer.WriteNextCol(gcmf.BoneCount);
                        writer.WriteNextCol(gcmf.GcmfTexturesSize);
                        writer.WriteNextCol(gcmf.SkinnedVertexDescriptor is null);
                        writer.WriteNextCol(gcmf.Submeshes.Length);
                        writer.WriteNextCol(gcmf.SkinnedVerticesA.Length);
                        writer.WriteNextCol(gcmf.SkinnedVerticesB.Length);
                        writer.WriteNextCol(gcmf.SkinBoneBindings.Length);
                        writer.WriteNextCol(gcmf.UnkBoneIndices.Length);
                        writer.WriteNextRow();
                    }
                }
                writer.Flush();
            }
        }

        public static void PrintTextureConfigs(Gma[] gmas, string outputFileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(outputFileName))
            {
                // Write header
                writer.WriteNextCol("FileName");
                writer.WriteNextCol("Address");
                writer.WriteNextCol(nameof(Model.Name));
                writer.WriteNextCol("Model Index");
                writer.WriteNextCol("Tex Index");
                writer.WriteNextCol(nameof(TextureConfig.Unk0x00));
                writer.WriteNextCol(nameof(TextureConfig.MipmapSetting));
                writer.WriteNextCol(nameof(TextureConfig.WrapMode));
                writer.WriteNextCol(nameof(TextureConfig.TplTextureIndex));
                writer.WriteNextCol(nameof(TextureConfig.Unk0x06));
                writer.WriteNextCol(nameof(TextureConfig.AnisotropicFilter));
                writer.WriteNextCol(nameof(TextureConfig.Unk0x0C));
                writer.WriteNextCol(nameof(TextureConfig.IsSwappableTexture));
                writer.WriteNextCol(nameof(TextureConfig.ConfigIndex));
                writer.WriteNextCol(nameof(TextureConfig.Unk0x10));
                writer.WriteNextRow();

                foreach (var gma in gmas)
                {
                    int modelIndex = 0;
                    foreach (var model in gma.Models)
                    {
                        int textureConfigIndex = 0;
                        foreach (var textureConfif in model.Gcmf.TextureConfigs)
                        {
                            writer.WriteNextCol(gma.FileName);
                            writer.WriteNextCol(textureConfif.AddressRange.PrintStartAddress());
                            writer.WriteNextCol(model.Name);
                            writer.WriteNextCol(modelIndex);
                            writer.WriteNextCol(textureConfigIndex++);
                            writer.WriteNextCol(textureConfif.Unk0x00);
                            writer.WriteNextCol(textureConfif.MipmapSetting);
                            writer.WriteNextCol(textureConfif.WrapMode);
                            writer.WriteNextCol(textureConfif.TplTextureIndex);
                            writer.WriteNextCol(textureConfif.Unk0x06);
                            writer.WriteNextCol(textureConfif.AnisotropicFilter);
                            writer.WriteNextCol(textureConfif.Unk0x0C);
                            writer.WriteNextCol(textureConfif.IsSwappableTexture);
                            writer.WriteNextCol(textureConfif.ConfigIndex);
                            writer.WriteNextCol(textureConfif.Unk0x10);
                            writer.WriteNextRow();
                        }
                        modelIndex++;
                    }
                }
                writer.Flush();
            }
        }

        public static void PrintMaterials(Gma[] gmas, string outputFileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(outputFileName))
            {
                // Write header
                writer.WriteNextCol("FileName");
                writer.WriteNextCol("Address");
                writer.WriteNextCol(nameof(Model.Name));
                writer.WriteNextCol("Model Index");
                writer.WriteNextCol("Material Index");
                writer.WriteNextCol(nameof(Material.Unk0x02));
                writer.WriteNextCol(nameof(Material.Unk0x03));
                writer.WriteNextCol(nameof(Material.Color0));
                writer.WriteNextCol(nameof(Material.Color1));
                writer.WriteNextCol(nameof(Material.Color2));
                writer.WriteNextCol(nameof(Material.Unk0x10));
                writer.WriteNextCol(nameof(Material.Unk0x11));
                writer.WriteNextCol(nameof(Material.TextureCount));
                writer.WriteNextCol(nameof(Material.DisplayListRenderFlags));
                writer.WriteNextCol(nameof(Material.Unk0x14));
                writer.WriteNextCol(nameof(Material.Unk0x15));
                writer.WriteNextCol(nameof(Material.TextureIndex0));
                writer.WriteNextCol(nameof(Material.TextureIndex1));
                writer.WriteNextCol(nameof(Material.TextureIndex2));
                writer.WriteNextCol(nameof(Material.VertexAttributes));
                writer.WriteNextRow();

                foreach (var gma in gmas)
                {
                    int modelIndex = 0;
                    foreach (var model in gma.Models)
                    {
                        int submeshIndex = 0;
                        foreach (var submesh in model.Gcmf.Submeshes)
                        {
                            writer.WriteNextCol(gma.FileName);
                            writer.WriteNextCol(submesh.AddressRange.PrintStartAddress());
                            writer.WriteNextCol(model.Name);
                            writer.WriteNextCol(modelIndex);
                            writer.WriteNextCol(submeshIndex++);
                            writer.WriteNextCol(submesh.Material.Unk0x02);
                            writer.WriteNextCol(submesh.Material.Unk0x03);
                            writer.WriteNextCol(submesh.Material.Color0);
                            writer.WriteNextCol(submesh.Material.Color1);
                            writer.WriteNextCol(submesh.Material.Color2);
                            writer.WriteNextCol(submesh.Material.Unk0x10);
                            writer.WriteNextCol(submesh.Material.Unk0x11);
                            writer.WriteNextCol(submesh.Material.TextureCount);
                            writer.WriteNextCol(submesh.Material.DisplayListRenderFlags);
                            writer.WriteNextCol(submesh.Material.Unk0x14);
                            writer.WriteNextCol(submesh.Material.Unk0x15);
                            writer.WriteNextCol(submesh.Material.TextureIndex0);
                            writer.WriteNextCol(submesh.Material.TextureIndex1);
                            writer.WriteNextCol(submesh.Material.TextureIndex2);
                            writer.WriteNextCol(submesh.Material.VertexAttributes);
                            writer.WriteNextRow();
                        }
                        modelIndex++;
                    }
                }
                writer.Flush();
            }
        }

    }
}
