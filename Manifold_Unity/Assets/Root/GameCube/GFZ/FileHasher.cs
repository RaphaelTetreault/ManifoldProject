using Manifold;
using Manifold.IO;
using Manifold.EditorTools;
using Manifold.EditorTools.GC.GFZ;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using UnityEngine;
using UnityEditor;


namespace Manifold.Assets.Root.GameCube.GFZ
{
    public static class FileHasher
    {
        public static StringBuilder MakeHashes(string rootFolder, HashAlgorithm algorithm)
        {
            StringBuilder builder = new StringBuilder();

            // Get all files
            var filePaths = Directory.GetFiles(rootFolder, "*", SearchOption.AllDirectories);
            
            foreach (var filePath in filePaths)
            {
                // Get path starting from root folder
                var relativeFilePath = filePath.Remove(0, rootFolder.Length);
                relativeFilePath = relativeFilePath.Replace("\\", "/");

                // Open file, get hash of contents
                var stream = File.OpenRead(filePath);
                var hashBytes = algorithm.ComputeHash(stream);

                // Write array entry
                // { "path/fileName", "HASH_BYTES" }
                builder.Append("{ \"");
                builder.Append(relativeFilePath);
                builder.Append("\", \"");
                foreach (var @byte in hashBytes)
                    builder.Append(@byte.ToString("x2"));
                builder.Append("\" },");
                builder.AppendLine();
                //builder.AppendLine($"{{ \"{relativeFilePath}\", \"{hashBytes}\" }}");
            }

            return builder;
        }


        [MenuItem("Manifold/Test Hash")]
        public static void TryHash()
        {
            var settings = GfzProjectWindow.GetSettings();
            var inputPath = settings.RootFolder;
            var outputPath = settings.FileOutput;
            var outputFile = outputPath + "md5-hashes.txt";

            var builder = MakeHashes(inputPath, MD5.Create());

            using (var writer = new StreamWriter(File.Create(outputFile)))
            {
                writer.Write(builder.ToString());
            }

            OSUtility.OpenDirectory(outputPath);
        }
    }
}
