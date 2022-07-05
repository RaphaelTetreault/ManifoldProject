using GameCube.GFZ.TPL;
using GameCube.GX.Texture;
using Manifold;
using Manifold.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.TPL
{
    public static class TplMenuItems
    {
        [MenuItem("Manifold/Tpl Test Load")]
        public static void TestTplRead()
        {
            var settings = GfzProjectWindow.GetSettings();
            var input = settings.SourceDirectory;
            var inputPaths = Directory.GetFiles(input, "*.tpl", SearchOption.AllDirectories);
            //var inputPaths = Directory.GetFiles(input, "arobin_sv0.tpl", SearchOption.AllDirectories);

            // TODO: make not hard-coded
            //var rootPath = "./Assets/gfzj01/tpl-test-4/";
            var rootPath = "./Assets/gfzj01/tpl-test/";
            Directory.CreateDirectory(rootPath);

            int tplCount = 0;
            int tplTotal = inputPaths.Length;
            var tplEnumerable = BinarySerializableIO.LoadFile<Tpl>(inputPaths);
            foreach (var tpl in tplEnumerable)
            {
                bool cancel = ProgressBar.ShowIndexed(tplCount, tplTotal, "TPL Test", inputPaths[tplCount]);
                if (cancel) break;
                tplCount++;

                foreach (var textureSeries in tpl.TextureSeries)
                {
                    if (textureSeries is null)
                        continue;

                    SaveTextures(textureSeries, rootPath, false);
                }
            }
            ProgressBar.Clear();
            AssetDatabase.Refresh();
        }


        /// <summary>
        /// Some gross code that imports both PNG textures and makes TPL scriptable objects
        /// with reference to textures, mipmaps, and source blocks
        /// </summary>
        [MenuItem("Manifold/Tpl Sobj Test Load")]
        public static void NewTest()
        {
            var settings = GfzProjectWindow.GetSettings();
            var input = settings.SourceDirectory;
            var inputPaths = Directory.GetFiles(input, "*.tpl", SearchOption.AllDirectories);

            // TODO: make not hard-coded
            var rootPath = "./Assets/gfzj01/tpl-test/";
            var sobjPath = "Assets/gfzj01/tpl-sobjs/";
            Directory.CreateDirectory(rootPath);
            Directory.CreateDirectory(sobjPath);

            int tplCount = 0;
            int tplTotal = inputPaths.Length;
            var tplEnumerable = BinarySerializableIO.LoadFile<Tpl>(inputPaths);
            foreach (var tpl in tplEnumerable)
            {
                bool cancel = ProgressBar.ShowIndexed(tplCount, tplTotal, "TPL Test", inputPaths[tplCount]);
                if (cancel) break;
                tplCount++;

                int index = 0;
                var tplTextureAndMipmaps = new TextureAndMipmaps[tpl.TextureSeries.Length];
                foreach (var textureSeries in tpl.TextureSeries)
                {
                    if (textureSeries is null)
                        continue;

                    var filePaths = SaveTextures(textureSeries, rootPath, false);

                    var tplTextureFields = new TextureField[textureSeries.Length];
                    for (int i = 0; i < textureSeries.Length; i++)
                    {
                        var filePath = filePaths[i].Substring(2); // remove "./"
                        var texture2d = AssetDatabase.LoadAssetAtPath<Texture2D>(filePath);
                        tplTextureFields[i] = new TextureField()
                        {
                            Texture = texture2d,
                            TextureFormat = textureSeries[i].Texture.Format,
                            IsValid = textureSeries[i].IsValid,
                        };
                    }
                    tplTextureAndMipmaps[index++] = new TextureAndMipmaps()
                    {
                        TextureFields = tplTextureFields,
                        SourceAddressRange = textureSeries.AddressRange,
                    };
                }
                var tplSobj = TplSobj.New(tplTextureAndMipmaps);
                tplSobj.SourceFileName = $"{tpl.FileName}.tpl";

                var sobjDest = $"{sobjPath}{tpl.FileName}.asset";
                if (AssetDatabase.LoadAssetAtPath<TplSobj>(sobjDest) != null)
                    Debug.Log($"Conflict! {tpl.FileName}");
                    //AssetDatabase.DeleteAsset(sobjDest);
                AssetDatabase.CreateAsset(tplSobj, sobjDest);
            }
            ProgressBar.Clear();
            AssetDatabase.Refresh();
        }

        public static Texture2D CreateTexture2D(GameCube.GX.Texture.Texture texture)
        {
            var texture2D = new Texture2D(texture.Width, texture.Height);
            for (int h = 0; h < texture.Height; h++)
            {
                for (int w = 0; w < texture.Width; w++)
                {
                    var texColor = texture.Pixels[w + h * texture.Width];
                    var color = new Color32(texColor.r, texColor.g, texColor.b, texColor.a);
                    texture2D.SetPixel(w, (texture.Height - 1 - h), color); // unsure if flip is error from here
                }
            }
            return texture2D;
        }

        public static void SaveData(string filePath, byte[] data, bool overwrite)
        {
            if (!overwrite)
                if (File.Exists(filePath))
                    return;

            using (var writer = new BinaryWriter(File.Create(filePath)))
            {
                writer.Write(data);
            }
        }

        private static string HashToString(byte[] hash)
        {
            var builder = new StringBuilder();
            foreach (var value in hash)
            {
                builder.Append($"{value:x2}");
            }
            return builder.ToString();
        }

        private static string[] SaveTextures(TextureSeries textureSeries, string rootPath, bool overwrite)
        {
            var outputNames = new string[textureSeries.Length];

            var hasher = MD5.Create();
            var mainTex = CreateTexture2D(textureSeries[0].Texture);
            byte[] png = mainTex.EncodeToPNG();
            var name = HashToString(hasher.ComputeHash(png, 0, png.Length));
            var filePath = $"{rootPath}{name}.png";
            SaveData(filePath, png, overwrite);
            outputNames[0] = filePath;

            for (int i = 1; i < textureSeries.Length; i++)
            {
                var mipmap = CreateTexture2D(textureSeries[i].Texture);
                var mipmapPng = mipmap.EncodeToPNG();
                var mipmapName = $"{name}_mipmap{i}";
                filePath = $"{rootPath}{mipmapName}.png";
                SaveData(filePath, mipmapPng, overwrite);
                outputNames[i] = filePath;
            }

            return outputNames;
        }
    }
}
