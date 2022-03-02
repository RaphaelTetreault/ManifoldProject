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
        public static void AllFileLoadLoop(string title, Action<string> loadAction)
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
                string info = $"[{index.ToString().PadLeft(countFormat)}/{count}] {filePath}";
                float progress = index / count;
                bool cancel = EditorUtility.DisplayCancelableProgressBar(title, info, progress);
                if (cancel)
                    break;

                loadAction(filePath);
            }
            EditorUtility.ClearProgressBar();
        }


        //[MenuItem(Const.Menu.Manifold + "GMA/Import Single TEST")]
        //public static void TestImportGmaFixed()
        //{
        //    var settings = GfzProjectWindow.GetSettings();
        //    var folder = settings.StageDir;
        //    var path = Path.Combine(folder, "st01.gma");
        //    LoadGma(path);
        //}

        [MenuItem(Const.Menu.Manifold + "GMA/LoadSave to Disk")]
        public static void LoadSaveToDisk()
        {
            AllFileLoadLoop("Load/Save byte-for-byte", LoadSaveToDisk);
        }
        // TODO: make this generic! Other files can use this
        public static void LoadSaveToDisk(string filePath)
        {
            // Get output parameters
            var settings = GfzProjectWindow.GetSettings();
            var folder = settings.FileOutput;
            // Get file names
            var name = Path.GetFileNameWithoutExtension(filePath);
            var ext = Path.GetExtension(filePath);
            var inputFilePath = Path.Combine(folder, $"{name}{ext}");
            var outputFilePath = Path.Combine(folder, $"{name}-export{ext}");
            // Load GMA, save a copy
            var gma = GmaIO.LoadGMA(filePath);
            File.Copy(filePath, inputFilePath, true);
            // Save own GMA
            GmaIO.SaveGMA(gma, outputFilePath);
            Debug.Log($"Read '{filePath}', Wrote '{outputFilePath}'");
        }


        [MenuItem(Const.Menu.Manifold + "GMA/Test Roundtrip Byte For Byte")]
        public static void RoundtripByteForByte()
        {
            AllFileLoadLoop("Load/Save byte-for-byte", RoundtripByteForByte);
        }
        public static void RoundtripByteForByte(string filePath)
        {
            GmaTests.TestGmaRoundtripByteForByte(filePath);
            Debug.Log(filePath);
        }


        [MenuItem(Const.Menu.Manifold + "GMA/Import All TEST")]
        public static void LoadSaveToMemory()
        {
            AllFileLoadLoop("Load/Save All GMA", LoadSaveGma);
        }
        public static void LoadSaveGma(string filePath)
        {
            GmaTests.TestLoadSaveGma(filePath);
            Debug.Log(filePath);
        }

        //public static void LoadGma(string filePath)
        //{
        //    GmaIO.LoadGMA(filePath);
        //    Debug.Log(filePath);
        //}

    }
}
