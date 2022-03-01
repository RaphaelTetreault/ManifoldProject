using GameCube.GFZ.Gma2;
using Manifold;
using Manifold.IO;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Gma2
{
    public static class GmaMenu
    {
        [MenuItem(Const.Menu.Manifold + "GMA/Import All TEST")]
        public static void TestImportAllGma()
        {
            var settings = GfzProjectWindow.GetSettings();
            var folder = settings.RootFolder;
            var filePaths = Directory.GetFiles(folder, "*.gma", SearchOption.AllDirectories);
            int index = 0;
            float count = filePaths.Length;
            int countFormat = count.ToString().Length;
            foreach (var filePath in filePaths)
            {
                index++;
                string title = $"Test Load GMA ({count})";
                string info = $"[{index.ToString().PadLeft(countFormat)}/{count}] {filePath}";
                float progress = index / count;
                bool cancel = EditorUtility.DisplayCancelableProgressBar(title, info, progress);
                if (cancel)
                    break;

                GmaTests.TestLoadSaveGma(filePath);
            }
            EditorUtility.ClearProgressBar();
        }

        [MenuItem(Const.Menu.Manifold + "GMA/Import Single TEST")]
        public static void TestImportGmaFixed()
        {
            var settings = GfzProjectWindow.GetSettings();
            var folder = settings.StageDir;
            var path = Path.Combine(folder, "st01.gma");
            LoadGma(path);
        }









        public static void LoadGma(string filePath)
        {
            GmaIO.LoadGMA(filePath);
            Debug.Log(filePath);
        }


        public static void TestLoadAllGma(params string[] filePaths)
        {
            foreach (var filePath in filePaths)
            {
                LoadGma(filePath);
            }
        }

    }
}
