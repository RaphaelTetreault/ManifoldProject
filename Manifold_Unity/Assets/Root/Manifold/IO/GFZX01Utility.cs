using StarkTools.IO;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Manifold.IO
{
    public class GFZX01Utility
    {
        public static void DecompressAv(string importFile, LibGxFormat.AvGame game, bool saveDecompressed, out string filePath)
        {
            using (var fileStream = File.Open(importFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var file = new MemoryStream();
                LibGxFormat.Lz.Lz.UnpackAvLz(fileStream, file, game);

                if (saveDecompressed)
                {
                    // See if files has been decompressed before
                    var dir = Path.GetDirectoryName(importFile);
                    var filename = Path.GetFileNameWithoutExtension(importFile);
                    // out param
                    filePath = UnityPathUtility.CombineSystemPath(dir, filename);

                    using (var writer = File.Create(filePath, (int)file.Length))
                    {
                        file.Seek(0, SeekOrigin.Begin);
                        file.CopyTo(writer);
                        file.Flush();
                    }
                }
                else
                {
                    filePath = string.Empty;
                }
            }
        }//

        public static string[] DecompressAnyLZ(string[] importFiles)
        {
            const string compressedExt = ".lz";
            var importFilesList = new List<string>(importFiles);

            for (int i = 0; i < importFilesList.Count; i++)
            //foreach (var importFile in importFiles)
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
                    DecompressAv(importFile, LibGxFormat.AvGame.FZeroGX, true, out outputFilePath);
                    // save reference to newly output file
                    importFilesList[i] = outputFilePath;
                }
            }

            // Remove all compressed file names/paths
            importFilesList.RemoveAll(i => i.EndsWith(compressedExt));

            return importFilesList.ToArray();
        }

    }
}