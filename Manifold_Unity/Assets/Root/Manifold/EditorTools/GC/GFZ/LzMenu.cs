using GameCube.GFZ;
using LibGxFormat;
using LibGxFormat.Lz;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;


namespace Manifold.EditorTools.GC.GFZ
{
    public class LzMenu
    {
        public const string GfzMenu = Const.Menu.Manifold + "Utility/";

        /// <summary>
        /// Decompresses all F-Zero GX LZ files in selected folder and subfolders
        /// </summary>
        [MenuItem(GfzMenu + "Decompress All LZ Files in Subdirectories")]
        public static void DecompressAllAvLz()
        {
            var rootPath = EditorUtility.OpenFolderPanel("Select GFZ Root Folder", "", "");

            var title = "Decompressing LZ Files...";
            var filePaths = Directory.GetFiles(rootPath, "*.lz", SearchOption.AllDirectories);
            // Wrap the decompress function into something that comforms to the file action format
            var decompressFile = (TestFileIO.FileAction)((string filePath) => { LzUtility.DecompressAvLzToDisk(filePath, true); });
            TestFileIO.FileActionLoop(title, filePaths, decompressFile);
        }



        [MenuItem(GfzMenu + "Compress single AV LZ (GX)")]
        public static void CompressSingleFileGX()
            => CompressSingleFile("Open File to Compress to LZ (F-Zero GX)", AvGame.FZeroGX);

        [MenuItem(GfzMenu + "Compress single AV LZ (AX)")]
        public static void CompressSingleFileAX()
            => CompressSingleFile("Open File to Compress to LZ (F-Zero AX)", AvGame.FZeroAX);

        public static void CompressSingleFile(string title, AvGame game)
        {
            var settings = GfzProjectWindow.GetSettings();
            var openPath = settings.FileOutput;
            var filePath = EditorUtility.OpenFilePanel(title, openPath, "");
            if (string.IsNullOrEmpty(filePath))
                return;
            LzUtility.CompressAvLzToDisk(filePath, game, true);
        }

    }
}