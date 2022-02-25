using GameCube.GFZ.Gma2;
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

namespace Manifold.EditorTools.GC.GFZ.Gma2
{
    public static class GmaMenu
    {
        [MenuItem(Const.Menu.Manifold + "GMA/Import Single TEST")]
        public static void TextImportGmaFixed()
        {
            var settings = GfzProjectWindow.GetSettings();
            var folder = settings.StageDir;
            var path = Path.Combine(folder, "st01.gma");
            TestImportGma(path);
        }

        public static void TestImportGma(string filepath)
        {
            using (var reader = new BinaryReader(File.OpenRead(filepath)))
            {
                var gma = new Gma();
                gma.Deserialize(reader);

                // 
                int index = 0;
                foreach (var model in gma.Models)
                {
                    Debug.Log($"[{++index}] {model.Name}");
                }
            }
        }


    }
}
