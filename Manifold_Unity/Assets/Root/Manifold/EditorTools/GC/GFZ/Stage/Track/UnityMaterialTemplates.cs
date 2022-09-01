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
        public const string shaderTex1Opaque = shadersAssetsPath + "UnlitTex1Opaque.shader";
        public const string shadergraphOp1Tex = shadersAssetsPath + "tex0opaque.shadergraph";
        public const string shadergraphTl1Tex = shadersAssetsPath + "tl_1Tex.shadergraph";
        public const string shadergraphOp2TexAdd = shadersAssetsPath + "op_2TexScreen.shadergraph";
        public const string shadergraphMutRails = shadersAssetsPath + "mut_rails.shadergraph";


        [MenuItem("Manifold/Materials Test")]
        public static void CreateAllMaterials()
        {
            MuteCity.CreateAllMaterials();
            MuteCityCOM.CreateAllMaterials();
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

            public static string CreateRoadTop() => CreateMaterial(shadergraphOp1Tex, GcmfTemplates.MuteCity.RoadTop());
            public static string CreateRoadBottom() => CreateMaterial(shadergraphOp1Tex, GcmfTemplates.MuteCity.RoadBottom());
            public static string CreateRoadSides() => CreateMaterial(shadergraphOp1Tex, GcmfTemplates.MuteCity.RoadSides());
            public static string CreateRoadEmbelishments() => CreateMaterial(shadergraphOp2TexAdd, GcmfTemplates.MuteCity.RoadEmbelishments());
            public static string CreateRoadRails() => CreateMaterial(shadergraphMutRails, GcmfTemplates.MuteCity.RoadRails());
            public static string CreateRoadLaneDividers() => CreateMaterial(shadergraphTl1Tex, GcmfTemplates.MuteCity.RoadLaneDividers());
        }
        public static class MuteCityCOM
        {
            public static void CreateAllMaterials()
            {
                CreateRoadTopNoDividers();
                CreateRoadTopEmbeddedDividers();
            }

            public static string CreateRoadTopNoDividers() => CreateMaterial(shadergraphOp1Tex, GcmfTemplates.MuteCityCOM.RoadTopNoDividers());
            public static string CreateRoadTopEmbeddedDividers() => CreateMaterial(shadergraphOp2TexAdd, GcmfTemplates.MuteCityCOM.RoadTopEmbeddedDividers());
        }

    }
}
