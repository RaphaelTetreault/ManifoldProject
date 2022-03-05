using GameCube.GFZ;
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
    /// <summary>
    /// Class meant for exposing GMA related tests in the Unity Editor.
    /// </summary>
    public static class GmaMenu
    {
        public static void ForeachGma(string title, TestFileIO.FileAction fileAction)
        {
            var settings = GfzProjectWindow.GetSettings();
            var rootFolder = settings.RootFolder;
            var filePaths = Directory.GetFiles(rootFolder, "*.gma", SearchOption.AllDirectories);
            TestFileIO.FileActionLoop(title, filePaths, fileAction);
        }


        [MenuItem(Const.Menu.Manifold + "GMA/LoadSave to Disk")]
        public static void LoadSaveToDisk()
        {
            ForeachGma("Load/Save byte-for-byte", LoadSaveToDisk);
        }
        public static void LoadSaveToDisk(string filePath)
        {
            // Get output parameters
            var settings = GfzProjectWindow.GetSettings();
            var rootPath = settings.RootFolder;
            var outputPath = settings.FileOutput + "/gma";
            var outputFilePath = TestFileIO.LoadSaveToDisk(filePath, rootPath, outputPath, BinarySerializableIO.LoadFile<Gma>, BinarySerializableIO.SaveFile, true);
            LzUtility.CompressAvLzToDisk(outputFilePath, LibGxFormat.AvGame.FZeroGX, true);
        }


        [MenuItem(Const.Menu.Manifold + "GMA/Test Roundtrip Byte For Byte")]
        public static void RoundtripByteForByte()
        {
            ForeachGma("Load/Save byte-for-byte", RoundtripByteForByte);
        }
        public static void RoundtripByteForByte(string filePath)
        {
            GmaTests.TestGmaRoundtripByteForByte(filePath);
            Debug.Log(filePath);
        }


        [MenuItem(Const.Menu.Manifold + "GMA/Import All TEST")]
        public static void LoadSaveToMemory()
        {
            ForeachGma("Load/Save All GMA", LoadSaveGma);
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
