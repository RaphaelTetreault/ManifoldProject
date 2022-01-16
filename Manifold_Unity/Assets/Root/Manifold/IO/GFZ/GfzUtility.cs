using LibGxFormat;
using LibGxFormat.Lz;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Manifold.IO.GFZ
{
    public class GfzUtility
    {
        /// <summary>
        /// Decompresses all F-Zero GX LZ files in selected folder and subfolders
        /// </summary>
        [MenuItem(Const.Menu.allRegions + "Decompress All LZ")]
        public static void DecompressAllGxAvLzAtRoot() => DecompressAllAvLzAtRoot();

        ///// <summary>
        ///// Decompresses all F-Zero AX LZ files in selected folder and subfolders
        ///// </summary>
        //[MenuItem(Const.Menu.ax + "Decompress All LZ")]
        //public static void DecompressAllAxAvLzAtRoot() => DecompressAllAvLzAtRoot(AvGame.FZeroAX);


        public static void DecompressAllAvLzAtRoot(SearchOption searchOption = SearchOption.AllDirectories)
        {
            var rootPath = EditorUtility.OpenFolderPanel("Select GFZ Root Folder", "", "");
            DecompressAvLzFolderToDisk(rootPath, "Decompressing LZ Files...", SearchOption.AllDirectories);
        }


        /// <summary>
        /// Compresses file at <paramref name="filePath"/> using Amusement Vision LZ
        /// compression. File is returned as MemoryStream.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="game"></param>
        /// <returns></returns>
        public static MemoryStream CompressAvLz(string filePath, AvGame game)
        {
            var compressedFile = new MemoryStream();
            using (var inputFile = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                Lz.PackAvLz(inputFile, compressedFile, game);
                compressedFile.Flush();
            }

            return compressedFile;
        }

        /// <summary>
        /// Compresses an existing file with Amusement Vision LZ compression and saves it to disk.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="game"></param>
        /// <param name="overwriteFiles"></param>
        /// <returns></returns>
        public static bool CompressAvLzToDisk(string filePath, AvGame game, bool overwriteFiles = true)
        {
            var outputPath = $"{filePath}.lz";

            // Prevent overwriting files if we don't want that
            if (!overwriteFiles && File.Exists(outputPath))
            {
                return false;
            }

            var compressedFile = CompressAvLz(outputPath, game);

            // Save stream from RAM to disk
            // File.Create will clear any existing file data
            using (var writer = File.Create(outputPath, (int)compressedFile.Length))
            {
                // Go to start of stream, then copy it over
                //lzStream.Seek(0, SeekOrigin.Begin);
                compressedFile.CopyTo(writer);
                //lzStream.Flush(); // is this line necessary?
            }

            return true;
        }

        /// <summary>
        /// Compresses existing files with Amusement Vision LZ compressio and saves it to disk.
        /// </summary>
        /// <param name="filePaths"></param>
        /// <param name="game"></param>
        /// <param name="overwriteFiles"></param>
        public static bool[] CompressAvLzToDisk(string[] filePaths, AvGame game, bool overwriteFiles = true)
        {
            var successes = new bool[filePaths.Length];

            for (int i = 0; i < filePaths.Length; i++)
            {
                var filePath = filePaths[i];
                bool success = CompressAvLzToDisk(filePath, game, overwriteFiles);
                successes[i] = success;
            }

            return successes;
        }


        public static MemoryStream DecompressAvLz(string filePath)
        {
            var decompressedFile = new MemoryStream();
            using (var inputFile = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                Lz.UnpackAvLz(inputFile, decompressedFile, AvGame.FZeroGX); // AX or GX will work ;)
                decompressedFile.Flush();
            }

            return decompressedFile;
        }

        public static bool DecompressAvLzToDisk(string filePath, bool overwriteFiles = true)
        {
            var path = Path.GetDirectoryName(filePath);
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var outputPath = Path.Combine(path, fileName);

            // Prevent overwriting files if we don't want that
            if (!overwriteFiles && File.Exists(outputPath))
            {
                return false;
            }

            // Decompress LZ file at 'filePath'
            var decompressedFile = DecompressAvLz(filePath);

            // Save stream from RAM to disk
            // File.Create will clear any existing file data
            using (var writer = File.Create(outputPath, (int)decompressedFile.Length))
            {
                decompressedFile.CopyTo(writer);
            }

            return true;
        }

        public static bool[] DecompressAvLzToDisk(string[] filePaths, bool overwriteFiles = true)
        {
            var successes = new bool[filePaths.Length];

            for (int i = 0; i < filePaths.Length; i++)
            {
                var filePath = filePaths[i];
                bool success = DecompressAvLzToDisk(filePath, overwriteFiles);
                successes[i] = success;
            }

            return successes;
        }

        /// <summary>
        /// Decompresses all LZ files starting from root folder.
        /// </summary>
        /// <param name="rootPath"></param>
        /// <param name="processTitle"></param>
        /// <param name="avGame"></param>
        /// <param name="searchOption"></param>
        /// <exception cref="IOException"></exception>
        public static void DecompressAvLzFolderToDisk(string rootPath, string processTitle, SearchOption searchOption)
        {
            // Find all files within folder matching this format
            var filePaths = Directory.GetFiles(rootPath, "*.lz", searchOption);
            int index = 0;
            int length = filePaths.Length;
            int charWidth = length.ToString().Length; // how long the max print index is

            foreach (var filePath in filePaths)
            {
                // TEMP? Skip *.arc.lz files, break in AX load. ./chara/arc/
                //if (filePath.Contains(".arc.lz"))
                //    continue;

                Debug.Log(filePath);
                index++;
                // Progress bar
                var display = filePath.Remove(0, rootPath.Length);
                var displayIndex = index.ToString().PadLeft(charWidth);
                var info = $"[{displayIndex}/{length}] {display}";
                var progress = (float)index / length;
                var cancel = EditorUtility.DisplayCancelableProgressBar(processTitle, info, progress);
                if (cancel)
                    break;

                // Get name of file to create
                var path = Path.GetDirectoryName(filePath);
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                var outputPath = Path.Combine(path, fileName);

                // Write file if it does not exist
                bool fileDoesNotExist = !File.Exists(outputPath);
                if (fileDoesNotExist)
                {
                    bool success = DecompressAvLzToDisk(filePath);
                    if (!success)
                    {
                        throw new IOException("Failed to save file. This should not happen.");
                    }
                }
            }
            EditorUtility.ClearProgressBar();
        }


        public const string compressedExt = ".lz";

        [System.Obsolete]
        public static MemoryStream DecompressAv(string importFile, LibGxFormat.AvGame game, bool saveDecompressed, out string filePath)
        {
            using (var fileStream = File.Open(importFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var decompressedFile = new MemoryStream();
                Lz.UnpackAvLz(fileStream, decompressedFile, game);

                if (saveDecompressed)
                {
                    // See if files has been decompressed before
                    var dir = Path.GetDirectoryName(importFile);
                    var filename = Path.GetFileNameWithoutExtension(importFile);
                    // out param
                    filePath = UnityPathUtility.CombineSystemPath(dir, filename);

                    using (var writer = File.Create(filePath, (int)decompressedFile.Length))
                    {
                        decompressedFile.Seek(0, SeekOrigin.Begin);
                        decompressedFile.CopyTo(writer);
                        decompressedFile.Flush();
                    }
                }
                else
                {
                    filePath = string.Empty;
                }

                return decompressedFile;
            }
        }//

        [System.Obsolete]
        public static string[] DecompressEachLZ(string[] importFiles, AvGame avGame = AvGame.FZeroGX)
        {
            var importFilesList = new List<string>(importFiles);

            for (int i = 0; i < importFilesList.Count; i++)
            {
                var importFile = importFilesList[i];

                var requireDecompress = importFile.EndsWith(compressedExt);
                var importFileDecompressed = importFile.Remove(importFile.Length - compressedExt.Length);
                var fileNotAlreadDecompressed = !File.Exists(importFileDecompressed);

                if (requireDecompress && fileNotAlreadDecompressed)
                {
                    string outputFilePath = string.Empty;
                    // Need to fix AvGame params to make sense...
                    // Save the decompressed file so next time we run this there is no decompression going on
                    DecompressAv(importFile, avGame, true, out outputFilePath);
                    // save reference to newly output file
                    importFilesList[i] = outputFilePath;
                }
            }

            // Remove all compressed file names/paths
            importFilesList.RemoveAll(i => i.EndsWith(compressedExt));

            return importFilesList.ToArray();
        }

        [System.Obsolete]
        public static MemoryStream CompressAv(string exportFile, AvGame game, bool saveCompressed, out string filePath)
        {
            using (var fileStream = File.Open(exportFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var compressedFile = new MemoryStream();
                Lz.PackAvLz(fileStream, compressedFile, game);

                if (saveCompressed)
                {
                    // See if files has been compressed before
                    var dir = Path.GetDirectoryName(exportFile);
                    var filename = $"{exportFile}{compressedExt}";
                    // out param
                    filePath = UnityPathUtility.CombineSystemPath(dir, filename);

                    using (var writer = File.Create(filePath, (int)compressedFile.Length))
                    {
                        compressedFile.Seek(0, SeekOrigin.Begin);
                        compressedFile.CopyTo(writer);
                        compressedFile.Flush();
                    }
                }
                else
                {
                    filePath = string.Empty;
                }

                return compressedFile;
            }
        }

        [System.Obsolete]
        public static string[] CompressEachAsLZ(string[] exportFiles, bool overwriteFiles = false)
        {
            var exportFilesList = new List<string>(exportFiles);

            for (int i = 0; i < exportFilesList.Count; i++)
            {
                var exportFile = exportFilesList[i];
                var exportFileCompressed = $"{exportFile}{compressedExt}";

                // Fail if we DON'T want to overwrite files but file exists
                if (!overwriteFiles)
                {
                    if (File.Exists(exportFileCompressed))
                    {
                        Debug.LogError($"Permission not set to overwrite \"{exportFile}\"!");
                        // set file as null so it can be removed from the list of exported files
                        exportFile = null;
                        continue;
                    }
                }

                string outputFilePath = string.Empty;
                // Need to fix AvGame params to make sense...
                // Save the decompressed file so next time we run this there is no decompression going on
                CompressAv(exportFile, AvGame.FZeroGX, true, out outputFilePath);
                // save reference to newly output file
                exportFilesList[i] = outputFilePath;
            }

            // Remove all failed file names/paths
            exportFilesList.RemoveAll(i => i == null);

            return exportFilesList.ToArray();
        }
    }
}