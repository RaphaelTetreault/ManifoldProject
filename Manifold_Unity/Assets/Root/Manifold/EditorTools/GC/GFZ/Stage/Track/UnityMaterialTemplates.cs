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
                material.SetTexture("_MainTex", texture);
            }
            return material;
        }
        private static string CreateMaterial(string shader, GcmfTemplate gcmfTemplate, string name)
        {
            var material = CreateFromGcmfTemplate(shader, gcmfTemplate);
            // Save material
            string materialPath = GetMaterialsPath() + name;
            AssetDatabaseUtility.CreateAsset(material, materialPath);
            //
            return materialPath;
        }

        public static class MuteCity
        {
            public static void CreateAllMaterials()
            {
                CreateRoadMaterial();
            }

            public static string CreateRoadMaterial(string name = "mat_mut_road_a.mat")
                => CreateMaterial(shaderTex1Opaque, GcmfTemplates.MuteCity.RoadTop(), name);
        }
        public static class MuteCityCOM
        {
            public static void CreateAllMaterials()
            {
                CreateRoadMaterial();
            }

            public static string CreateRoadMaterial(string name = "mat_com_road_b.mat")
                => CreateMaterial(shaderTex1Opaque, GcmfTemplates.MuteCityCOM.RoadTopNoDividers(), name);
        }

    }
}
