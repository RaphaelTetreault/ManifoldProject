using GameCube.GFZ.TPL;
using GameCube.GX.Texture;
using Manifold;
using Manifold.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            var rootPath = "./Assets/gfzj01/tpl-test/";
            //AssetDatabaseUtility.CreateDirectory(rootPath);
            //Directory.CreateDirectory(rootPath);

            int count = 0;
            int total = inputPaths.Length;

            var tplEnumerable = BinarySerializableIO.LoadFile<Tpl>(inputPaths);
            foreach (var tpl in tplEnumerable)
            {
                bool cancel = ProgressBar.ShowIndexed(count, total, "TPL Test", inputPaths[count]);
                if (cancel) break;
                count++;

                int texIndex = 0;
                foreach (var texture in tpl.Textures)
                {
                    var name = $"{tpl.FileName}_{texIndex++}_{texture.Format}";
                    var fullPath = Path.Combine(rootPath, name);
                    CreateTexture(texture, fullPath);
                }
            }
            ProgressBar.Clear();
            AssetDatabase.Refresh();
        }

        public static void CreateTexture(GameCube.GX.Texture.Texture texture, string relPath)
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

            byte[] png = unityTexture.EncodeToPNG();
            using (var writer = new BinaryWriter(File.Create(relPath + ".png")))
            {
                writer.Write(png);
            }
        }
    }
}
