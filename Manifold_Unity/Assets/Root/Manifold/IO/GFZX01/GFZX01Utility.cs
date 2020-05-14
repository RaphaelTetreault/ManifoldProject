using StarkTools.IO;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LibGxFormat;
using LibGxFormat.Lz;

namespace Manifold.IO.GFZX01
{
    public class GFZX01Utility
    {
        public const string compressedExt = ".lz";

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

        public static string[] DecompressEachLZ(string[] importFiles)
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
                    DecompressAv(importFile, AvGame.FZeroGX, true, out outputFilePath);
                    // save reference to newly output file
                    importFilesList[i] = outputFilePath;
                }
            }

            // Remove all compressed file names/paths
            importFilesList.RemoveAll(i => i.EndsWith(compressedExt));

            return importFilesList.ToArray();
        }


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