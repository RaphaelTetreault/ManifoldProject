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
            //var inputPaths = Directory.GetFiles(input, "*.tpl", SearchOption.AllDirectories);
            var inputPaths = Directory.GetFiles(input, "bg_por_s.tpl", SearchOption.AllDirectories);

            var rootPath = "./Assets/gfzj01/tpl-test3/";
            Directory.CreateDirectory(rootPath);

            int tplCount = 0;
            int tplTotal = inputPaths.Length;

            var tplEnumerable = BinarySerializableIO.LoadFile<Tpl>(inputPaths);
            foreach (var tpl in tplEnumerable)
            {
                bool cancel = ProgressBar.ShowIndexed(tplCount, tplTotal, "TPL Test", inputPaths[tplCount]);
                if (cancel) break;
                tplCount++;

                int texArrayIndex = 0;
                foreach (var textureSeries in tpl.TextureSeries)
                {
                    for (int i = 0; i < textureSeries.Length; i++)
                    {
                        var texture = textureSeries[i].Texture;
                        var texture2D = CreateTexture2D(texture);

                        string name = string.Empty;
                        //string name = $"{tpl.FileName}_tex{texArrayIndex}_mip{i}";
                        SaveTexture2DWithHash(texture2D, rootPath, name);
                    }
                    texArrayIndex++;
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

        public static void SaveTexture2DWithHash(Texture2D texture2D, string rootPath, string name = "")
        {
            byte[] png = texture2D.EncodeToPNG();
            byte[] hash = MD5.Create().ComputeHash(png, 0, png.Length);
            string printName = String.IsNullOrEmpty(name)
                ? HashToString(hash) : name;
            string filePath = $"{rootPath}/{printName}.png";

            using (var writer = new BinaryWriter(File.Create(filePath)))
            {
                writer.Write(png);
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
    }
}
