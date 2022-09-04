using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public static class UnityMaterialTemplates
    {
        public const string shadersAssetsPath = "Assets/Root/Manifold/EditorTools/GC/GFZ/Shaders/";
        
        // Generic Opaque Materials
        public const string shadergraph_mt_1Tex = shadersAssetsPath + "mt_1Tex.shadergraph";
        public const string shadergraph_mt_2TexAdd = shadersAssetsPath + "mt_2TexAdd.shadergraph";
        public const string shadergraph_mt_2TexMulitply = shadersAssetsPath + "mt_2TexMultiply.shadergraph";
        // Generic Translucid Materials
        public const string shadergraph_tl_1Tex = shadersAssetsPath + "tl_1Tex.shadergraph";
        public const string shadergraph_tl_1TexAlpha = shadersAssetsPath + "tl_2TexAlpha.shadergraph";
        public const string shadergraph_tl_2TexMultiplyAdditive = shadersAssetsPath + "tl_2TexMultiplyAdditive.shadergraph";
        public const string shadergraph_tl_2TexMultiplyMultiply = shadersAssetsPath + "tl_2TexMultiplyMultiply.shadergraph";

        // RECOVER
        public const string embedRecoverLightAlpha = shadergraph_tl_2TexMultiplyAdditive;
        public const string embedRecoverLightBase = shadergraph_tl_2TexMultiplyAdditive;
        public const string embedRecoverLightSubBase = shadergraph_mt_1Tex;
        public const string embedRecoverDarkAlpha = shadergraph_tl_2TexMultiplyAdditive;
        public const string embedRecoverDarkBase = shadergraph_mt_1Tex;
        // DIRT
        public const string embedDirtAlpha = shadergraph_tl_1Tex;
        public const string embedDirtNoise = shadergraph_mt_1Tex; // looks better than 2 tex multiply
        // LAVA
        public const string embedLavaAlpha = shadergraph_tl_2TexMultiplyAdditive;
        public const string embedLavaCarg = shadergraph_mt_1Tex;
        // SLIP
        public const string embedSlipLight = shadergraph_mt_2TexAdd;
        public const string embedSlipDarkThin = shadergraph_mt_2TexAdd;
        public const string embedSlipDarkWide = shadergraph_mt_2TexAdd;

        // MUTE CITY
        public const string mutRoadRails = shadersAssetsPath + "mut_rails.shadergraph";
        // Outer Space
        public const string metRoadTop = shadersAssetsPath + "met_top.shadergraph";


        [MenuItem(GfzMenuItems.Materials.CreateEditorMaterials, priority = GfzMenuItems.Materials.Priority.CreateEditorMaterials)]
        public static void CreateAllMaterials()
        {
            General.CreateAllMaterials();
            MuteCity.CreateAllMaterials();
            MuteCityCOM.CreateAllMaterials();
            OuterSpace.CreateAllMaterials();
        }

        private static string GetAssetsPath()
        {
            var settings = GfzProjectWindow.GetSettings();
            var assetsPath = settings.AssetsWorkingDirectory;
            return assetsPath;
        }
        private static string GetTexturesPath()
        {
            var assetsPath = GetAssetsPath();
            var texturesPath = assetsPath + "/tpl/textures/";
            return texturesPath;
        }
        private static string GetMaterialsPath()
        {
            var assetsPath = GetAssetsPath();
            var materialsPath = assetsPath + "/mat/";
            return materialsPath;
        }
        private static Material CreateMaterialFromShader(string shaderPath)
        {
            var shader = AssetDatabase.LoadAssetAtPath<Shader>(shaderPath);
            var material = new UnityEngine.Material(shader);
            return material;
        }
        private static Material CreateFromGcmfTemplate(string shader, GcmfTemplate gcmfTemplate)
        {
            var material = CreateMaterialFromShader(shader);
            string textureDirectory = GetTexturesPath();
            for (int i = 0; i < gcmfTemplate.TextureHashes.Length; i++)
            {
                string textureName = gcmfTemplate.TextureHashes[i];
                string texturePath = textureDirectory + textureName + ".png";
                var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
                material.SetTexture($"_Tex{i}", texture);
            }

            var alpha = gcmfTemplate.Submesh.Material.Alpha / 255f;
            material.SetFloat($"_Alpha", alpha);

            return material;
        }
        private static string CreateMaterial(string shader, GcmfTemplate gcmfTemplate)
        {
            var material = CreateFromGcmfTemplate(shader, gcmfTemplate);
            // Save material
            string materialName = $"mat_{gcmfTemplate.Name}.mat";
            string materialPath = GetMaterialsPath() + materialName;
            AssetDatabaseUtility.CreateAsset(material, materialPath);
            //
            return materialPath;
        }

        public static Material LoadMaterial(GcmfTemplate gcmfTemplate)
        {
            string basePath = GetAssetsPath();
            string assetName = gcmfTemplate.Name;
            string assetPath = $"{basePath}/mat/mat_{assetName}.mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
            return material;
        }
        public static Material[] LoadMaterials(GcmfTemplate[] gcmfTemplates)
        {
            var materials = new Material[gcmfTemplates.Length];
            for (int i = 0; i < materials.Length; i++)
                materials[i] = LoadMaterial(gcmfTemplates[i]);

            return materials;
        }

        public class General
        {
            public static void CreateAllMaterials()
            {
                CreateSlipLight();
                CreateSlipDarkThin();
                CreateSlipDarkWide();
                CreateRecoverLightAlpha();
                CreateRecoverLightBase();
                CreateRecoverLightSubBase();
                CreateRecoverDarkAlpha();
                CreateRecoverDarkBase();
                CreateLavaAlpha();
                CreateLavaCrag();
                CreateDirtAlpha();
                CreateDirtNoise();
            }
            public static string CreateSlipLight() => CreateMaterial(embedSlipLight, GcmfTemplates.General.SlipLight());
            public static string CreateSlipDarkThin() => CreateMaterial(embedSlipDarkThin, GcmfTemplates.General.SlipDarkThin());
            public static string CreateSlipDarkWide() => CreateMaterial(embedSlipDarkWide, GcmfTemplates.General.SlipDarkWide());
            public static string CreateRecoverLightAlpha() => CreateMaterial(embedRecoverLightAlpha, GcmfTemplates.General.RecoverLightAlpha());
            public static string CreateRecoverLightBase() => CreateMaterial(embedRecoverLightBase, GcmfTemplates.General.RecoverLightBase());
            public static string CreateRecoverLightSubBase() => CreateMaterial(embedRecoverLightSubBase, GcmfTemplates.General.RecoverLightSubBase());
            public static string CreateRecoverDarkAlpha() => CreateMaterial(embedRecoverDarkAlpha, GcmfTemplates.General.RecoverDarkAlpha());
            public static string CreateRecoverDarkBase() => CreateMaterial(embedRecoverDarkBase, GcmfTemplates.General.RecoverDarkBase());
            public static string CreateLavaAlpha() => CreateMaterial(embedLavaAlpha, GcmfTemplates.General.LavaAlpha());
            public static string CreateLavaCrag() => CreateMaterial(embedLavaCarg, GcmfTemplates.General.LavaCrag());
            public static string CreateDirtAlpha() => CreateMaterial(embedDirtAlpha, GcmfTemplates.General.DirtAlpha());
            public static string CreateDirtNoise() => CreateMaterial(embedDirtNoise, GcmfTemplates.General.DirtNoise());
            public static string CreateTrim() => CreateMaterial(shadergraph_mt_1Tex, GcmfTemplates.General.Trim());
        }
        public static class MuteCity
        {
            public static void CreateAllMaterials()
            {
                CreateRoadTop();
                CreateRoadBottom();
                CreateRoadSides();
                CreateRoadEmbelishments();
                CreateRoadRails();
                CreateRoadLaneDividers();
            }

            public static string CreateRoadTop() => CreateMaterial(shadergraph_mt_1Tex, GcmfTemplates.MuteCity.RoadTop());
            public static string CreateRoadBottom() => CreateMaterial(shadergraph_mt_1Tex, GcmfTemplates.MuteCity.RoadBottom());
            public static string CreateRoadSides() => CreateMaterial(shadergraph_mt_1Tex, GcmfTemplates.MuteCity.RoadSides());
            public static string CreateRoadEmbelishments() => CreateMaterial(shadergraph_mt_2TexAdd, GcmfTemplates.MuteCity.RoadEmbelishments());
            public static string CreateRoadRails() => CreateMaterial(mutRoadRails, GcmfTemplates.MuteCity.RoadRails());
            public static string CreateRoadLaneDividers() => CreateMaterial(shadergraph_tl_1Tex, GcmfTemplates.MuteCity.RoadLaneDividers());
        }
        public static class MuteCityCOM
        {
            public static void CreateAllMaterials()
            {
                CreateRoadTopNoDividers();
                CreateRoadTopEmbeddedDividers();
            }

            public static string CreateRoadTopNoDividers() => CreateMaterial(shadergraph_mt_1Tex, GcmfTemplates.MuteCityCOM.RoadTopNoDividers());
            public static string CreateRoadTopEmbeddedDividers() => CreateMaterial(shadergraph_mt_2TexAdd, GcmfTemplates.MuteCityCOM.RoadTopEmbeddedDividers());
        }

        public static class OuterSpace
        {
            public static void CreateAllMaterials()
            {
                BottomAndSides();
                CurbAndLaneDividerSlope();
                CurbAndLaneDividerTop();
                EndCap();
                RailsAngle();
                RailsLights();
                Top();
            }

            public static string BottomAndSides() => CreateMaterial(shadergraph_mt_1Tex, GcmfTemplates.OuterSpace.BottomAndSides());
            public static string CurbAndLaneDividerSlope() => CreateMaterial(shadergraph_mt_1Tex, GcmfTemplates.OuterSpace.CurbAndLaneDividerSlope());
            public static string CurbAndLaneDividerTop() => CreateMaterial(shadergraph_mt_1Tex, GcmfTemplates.OuterSpace.CurbAndLaneDividerTop());
            public static string EndCap() => CreateMaterial(shadergraph_mt_1Tex, GcmfTemplates.OuterSpace.EndCap());
            public static string RailsAngle() => CreateMaterial(shadergraph_mt_1Tex, GcmfTemplates.OuterSpace.RailsAngle());
            public static string RailsLights() => CreateMaterial(shadergraph_mt_1Tex, GcmfTemplates.OuterSpace.RailsLights());
            public static string Top() => CreateMaterial(metRoadTop, GcmfTemplates.OuterSpace.Top());
        }

    }
}
