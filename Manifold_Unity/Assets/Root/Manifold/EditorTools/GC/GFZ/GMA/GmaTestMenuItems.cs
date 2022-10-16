using GameCube.AmusementVision;
using GameCube.GFZ.GMA;
using GameCube.GFZ.LZ;
using Manifold.IO;
using System.IO;
using UnityEditor;
using UnityEngine;
using static Manifold.IO.BinarySerializableIO;

namespace Manifold.EditorTools.GC.GFZ.GMA
{
    /// <summary>
    /// Class meant for exposing GMA related tests in the Unity Editor.
    /// </summary>
    public static class GmaTestMenuItems
    {
        public static void ForeachGma(string title, string rootDirectory, FileUtility.FileAction fileAction)
        {
            var filePaths = Directory.GetFiles(rootDirectory, "*.gma", SearchOption.AllDirectories);
            FileUtility.FileActionLoop(title, filePaths, fileAction);
        }
        public static void ForeachGma(string title, FileUtility.FileAction fileAction)
        {
            var settings = GfzProjectWindow.GetSettings();
            var rootFolder = settings.SourceDirectory;
            ForeachGma(title, rootFolder, fileAction);
        }


        [MenuItem(GfzMenuItems.GMA.LoadSaveToDisk, priority = GfzMenuItems.GMA.Priority.LoadSaveToDisk)]
        public static void LoadSaveToDisk()
        {
            ForeachGma("Load/Save to Disk", LoadSaveToDisk);
        }
        public static void LoadSaveToDisk(string filePath)
        {
            // Get output parameters
            var settings = GfzProjectWindow.GetSettings();
            var rootPath = settings.SourceDirectory;
            var outputPath = settings.FileOutput + "/gma";
            var outputFilePath = FileUtility.LoadSaveToDisk(filePath, rootPath, outputPath, LoadFile<Gma>, SaveFile, true);
            LzUtility.CompressAvLzToDisk(outputFilePath, GxGame.FZeroGX, true);
        }


        [MenuItem(GfzMenuItems.GMA.LoadSaveToMemory, priority = GfzMenuItems.GMA.Priority.LoadSaveToMemory)]
        public static void LoadSaveToMemory()
        {
            ForeachGma("Load/Save (in RAM)", LoadSaveToMemory);
        }
        public static void LoadSaveToMemory(string filePath)
        {
            var gma = LoadFile<Gma>(filePath);
            SaveTo(gma, new MemoryStream());
            Debug.Log(filePath);
        }

    }
}
