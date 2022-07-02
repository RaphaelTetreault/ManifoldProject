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

            int count = 0;
            int total = inputPaths.Length;

            var tplEnumerable = BinarySerializableIO.LoadFile<Tpl>(inputPaths);
            foreach (var tpl in tplEnumerable)
            {
                bool cancel = ProgressBar.ShowIndexed(count, total, "TPL Test", inputPaths[count]);
                if (cancel) break;
                count++;

                int texArrayIndex = 0;
                foreach (var textureArray in tpl.TextureAndMipmaps)
                {
                    for (int i = 0; i < textureArray.Length; i++)
                    {
                        var texture = textureArray[i];
                        if (texture is null)
                            continue;
                        string name = $"{tpl.FileName}_tex{texArrayIndex}_mip{i}";
                        CreateTexture(texture, rootPath, name);
                    }
                    texArrayIndex++;
                }
            }
            ProgressBar.Clear();
            AssetDatabase.Refresh();
        }

        public static void CreateTexture(GameCube.GX.Texture.Texture texture, string rootPath, string name)
        {
            var unityTexture = new Texture2D(texture.Width, texture.Height);
            for (int h = 0; h < texture.Height; h++)
            {
                for (int w = 0; w < texture.Width; w++)
                {
                    //var texColor = texture[w, h];
                    var texColor = texture.Pixels[w + h * texture.Width];
                    var color = new Color32(texColor.r, texColor.g, texColor.b, texColor.a);
                    unityTexture.SetPixel(w, (texture.Height - 1 - h), color); // unsure if flip is error from here
                }
            }


            // TODO: separate from above - make texture, and save/hash it
            byte[] png = unityTexture.EncodeToPNG();
            byte[] hash = MD5.Create().ComputeHash(png, 0, png.Length);
            //string name = HashToString(hash);
            string filePath = $"{rootPath}/{name}.png";

            if (File.Exists(filePath))
            {
                //Debug.Log(filePath);
                return;
            }

            using (var writer = new BinaryWriter(File.Create(filePath)))
            {
                writer.Write(png);
            }
        }

        private static string HashToString(byte[] hash)
        {
            var builder = new System.Text.StringBuilder();
            foreach (var value in hash)
            {
                builder.Append($"{value:x2}");
            }
            return builder.ToString();
        }
    }
}
