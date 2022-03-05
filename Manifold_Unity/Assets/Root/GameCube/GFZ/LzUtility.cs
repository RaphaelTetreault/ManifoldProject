using LibGxFormat;
using LibGxFormat.Lz;
using System.Collections.Generic;
using System.IO;

namespace GameCube.GFZ
{
    /// <summary>
    /// Static methods to compress and decompress GFZ LZ archives.
    /// </summary>
    public static class LzUtility
    {
        public struct FileStatus
        {
            public bool success;
            public string filePath;

            public FileStatus(bool success, string filePath)
            {
                this.success = success;
                this.filePath = filePath;
            }
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
        public static bool CompressAvLzToDisk(string filePath, AvGame game, bool overwriteFiles)
        {
            var outputPath = $"{filePath}.lz";

            // Prevent overwriting files if we don't want that
            if (!overwriteFiles && File.Exists(outputPath))
            {
                return false;
            }

            var compressedFile = CompressAvLz(filePath, game);

            // Save stream from RAM to disk
            // File.Create will clear any existing file data
            using (var writer = File.Create(outputPath, (int)compressedFile.Length))
            {
                // Go to start of stream, then copy it over
                compressedFile.Seek(0, SeekOrigin.Begin);
                compressedFile.CopyTo(writer);
                //lzStream.Flush(); // is this line necessary?
            }

            return true;
        }


        public static IEnumerable<FileStatus> CompressAvLzToDisk(string[] filePaths, AvGame game, bool overwriteFiles)
        {
            for (int i = 0; i < filePaths.Length; i++)
            {
                var filePath = filePaths[i];
                bool success = CompressAvLzToDisk(filePath, game, overwriteFiles);
                yield return new FileStatus(success, filePath);
            }
        }


        public static IEnumerable<FileStatus> CompressAvLzDirectoryToDisk(string rootPath, AvGame game, bool overwriteFiles, SearchOption searchOption, string searchPattern)
        {
            var filePaths = Directory.GetFiles(rootPath, searchPattern, searchOption);
            return CompressAvLzToDisk(filePaths, game, overwriteFiles);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
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



        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="overwriteFiles"></param>
        /// <returns></returns>
        public static bool DecompressAvLzToDisk(string filePath, bool overwriteFiles)
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
            var decompressedFileStream = DecompressAvLz(filePath);

            // Save stream from RAM to disk
            // File.Create will clear any existing file data
            using (var writer = File.Create(outputPath, (int)decompressedFileStream.Length))
            {
                decompressedFileStream.Seek(0, SeekOrigin.Begin);
                decompressedFileStream.CopyTo(writer);
            }

            return true;
        }

        public static IEnumerable<FileStatus> DecompressAvLzToDisk(string[] filePaths, bool overwriteFiles)
        {
            for (int i = 0; i < filePaths.Length; i++)
            {
                var filePath = filePaths[i];
                bool success = DecompressAvLzToDisk(filePath, overwriteFiles);
                yield return new FileStatus(success, filePath);
            }
        }


        public static IEnumerable<FileStatus> DecompressAvLzDirectoryToDisk(string rootPath, bool overwriteFiles, SearchOption searchOption)
        {
            var filePaths = Directory.GetFiles(rootPath, "*.lz", searchOption);
            return DecompressAvLzToDisk(filePaths, overwriteFiles);
        }
    }
}