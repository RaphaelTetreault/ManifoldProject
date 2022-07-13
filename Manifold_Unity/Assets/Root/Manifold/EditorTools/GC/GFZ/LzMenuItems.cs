using GameCube.AmusementVision;
using GameCube.GFZ.LZ;
using System.IO;
using UnityEditor;


namespace Manifold.EditorTools.GC.GFZ
{
    public class LzMenuItems
    {
        /// <summary>
        /// Decompresses all F-Zero GX LZ files in selected folder and subfolders
        /// </summary>
        [MenuItem(GfzMenuItems.Lz.DecompressAllAvLz, priority = GfzMenuItems.Lz.DecompressAllAvLzPriority)]
        public static void DecompressAllAvLz()
        {
            var rootPath = EditorUtility.OpenFolderPanel("Select GFZ Root Folder", "", "");

            var title = "Decompressing LZ Files...";
            var filePaths = Directory.GetFiles(rootPath, "*.lz", SearchOption.AllDirectories);
            // Wrap the decompress function into something that comforms to the file action format
            var decompressFile = (FileUtility.FileAction)((string filePath) => { LzUtility.DecompressAvLzToDisk(filePath, true); });
            FileUtility.FileActionLoop(title, filePaths, decompressFile);
        }

        [MenuItem(GfzMenuItems.Lz.DecompressSingleAvLz, priority = GfzMenuItems.Lz.DecompressSingleAvLzPriority)]
        public static void DecompressSingleAvLz()
        {
            var filePath = EditorUtility.OpenFilePanel("Select GFZ Root Folder", "", "");
            if (string.IsNullOrEmpty(filePath))
                return;
            LzUtility.DecompressAvLzToDisk(filePath, true);
        }

        [MenuItem(GfzMenuItems.Lz.CompressSingleFileGx, priority = GfzMenuItems.Lz.CompressSingleFileGxPriority)]
        public static void CompressSingleFileGX()
            => CompressSingleFile("Open File to Compress to LZ (F-Zero GX)", GxGame.FZeroGX);

        [MenuItem(GfzMenuItems.Lz.CompressSingleFileAx, priority = GfzMenuItems.Lz.CompressSingleFileAxPriority)]
        public static void CompressSingleFileAX()
            => CompressSingleFile("Open File to Compress to LZ (F-Zero AX)", GxGame.FZeroAX);

        public static void CompressSingleFile(string title, GxGame game)
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