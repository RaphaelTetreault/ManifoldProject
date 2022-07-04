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

namespace Manifold.EditorTools.GC.GFZ
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

                    for (int i = 0; i < textureSeries.Length; i++)
                        SaveTextures(textureSeries, rootPath, false);
                }
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

        private static void SaveTextures(TextureSeries textureSeries, string rootPath, bool overwrite)
        {
            var hasher = MD5.Create();
            var mainTex = CreateTexture2D(textureSeries[0].Texture);
            byte[] png = mainTex.EncodeToPNG();
            var name = HashToString(hasher.ComputeHash(png, 0, png.Length));
            var filePath = $"{rootPath}{name}.png";
            SaveData(filePath, png, overwrite);

            for (int i = 1; i < textureSeries.Length; i++)
            {
                var mipmap = CreateTexture2D(textureSeries[i].Texture);
                var mipmapPng = mipmap.EncodeToPNG();
                var mipmapName = $"{name}_mipmap{i}";
                filePath = $"{rootPath}{mipmapName}.png";
                SaveData(filePath, mipmapPng, overwrite);
            }
        }
    }
}
