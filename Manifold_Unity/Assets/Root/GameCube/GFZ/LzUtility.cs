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
        /// <summary>
        /// Metadata struct to indicate file status after IO operation.
        /// </summary>
        public struct FileStatus
        {
            /// <summary>
            /// Indicates the IO operation for was successful.
            /// </summary>
            public bool success;

            /// <summary>
            /// The file path of the IO operation.
            /// </summary>
            public string filePath;

            public FileStatus(bool success, string filePath)
            {
                this.success = success;
                this.filePath = filePath;
            }
        }

        /// <summary>
        /// Compresses file at <paramref name="filePath"/> using Amusement Vision LZ compression.
        /// </summary>
        /// <param name="filePath">The file to compress.</param>
        /// <param name="game">Which LZ format to use for this game.</param>
        /// <returns>
        /// A memory stream with the compressed file contents.
        /// </returns>
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
        /// Compresses file at <paramref name="filePath"/> using Amusement Vision LZ compression
        /// and saves it to disk.
        /// </summary>
        /// <param name="filePath">The file to compress.</param>
        /// <param name="game">Which LZ format to use for this game.</param>
        /// <param name="overwriteFiles">Whether the opration can overwrite existing files on disk.</param>
        /// <returns>
        /// Returns a bool indicating the IO operation was successful.
        /// </returns>
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
            }

            return true;
        }

        /// <summary>
        /// Compresses each file in <paramref name="filePaths"/> using Amusement Vision LZ compression
        /// and saves it to disk.
        /// </summary>
        /// <param name="filePaths">The files to compress.</param>
        /// <param name="game">Which LZ format to use for this game.</param>
        /// <param name="overwriteFiles">Whether the opration can overwrite existing files on disk.</param>
        /// <returns>
        /// Returns an iterator for each file specified in <paramref name="filePaths"/>. For each file, the
        /// iterator returns a FileStatus with success and filePath data.
        /// </returns>
        public static IEnumerable<FileStatus> CompressAvLzToDisk(string[] filePaths, AvGame game, bool overwriteFiles)
        {
            for (int i = 0; i < filePaths.Length; i++)
            {
                var filePath = filePaths[i];
                bool success = CompressAvLzToDisk(filePath, game, overwriteFiles);
                yield return new FileStatus(success, filePath);
            }
        }

        /// <summary>
        /// Compresses all files in <paramref name="rootPath"/> using Amusement Vision LZ compression
        /// and saves each file it to disk.
        /// </summary>
        /// <param name="rootPath">The folder to compress files in.</param>
        /// <param name="game">Which LZ format to use for this game.</param>
        /// <param name="overwriteFiles">Whether the opration can overwrite existing files on disk.</param>
        /// <param name="searchOption">The search option used to find files in the specified folder.</param>
        /// <param name="searchPattern">The search pattern used to find files in the specified folder.</param>
        /// <returns>
        /// Returns an iterator for each file found using the search parameters. For each file, the iterator
        /// returns a FileStatus with success and filePath data.
        /// </returns>
        public static IEnumerable<FileStatus> CompressAvLzDirectoryToDisk(string rootPath, AvGame game, bool overwriteFiles, SearchOption searchOption, string searchPattern)
        {
            var filePaths = Directory.GetFiles(rootPath, searchPattern, searchOption);
            return CompressAvLzToDisk(filePaths, game, overwriteFiles);
        }

        /// <summary>
        /// Decompresses file at <paramref name="filePath"/> using Amusement Vision LZ decompression.
        /// </summary>
        /// <param name="filePath">The file to decompress.</param>
        /// <returns>
        /// A memory stream with the decompressed file contents.
        /// </returns>
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
        /// Decompresses file at <paramref name="filePath"/> using Amusement Vision LZ decompression
        /// and saves it to disk.
        /// </summary>
        /// <param name="filePath">The file to decompress.</param>
        /// <param name="overwriteFiles">Whether the opration can overwrite existing files on disk.</param>
        /// <returns>
        /// Returns a bool indicating the IO operation was successful.
        /// </returns>
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

        /// <summary>
        /// Decompresses each file in <paramref name="filePaths"/> using Amusement Vision LZ decompression
        /// and saves it to disk.
        /// </summary>
        /// <param name="filePath">The files to decompress.</param>
        /// <param name="overwriteFiles">Whether the opration can overwrite existing files on disk.</param>
        /// <returns>
        /// Returns an iterator for each file specified in <paramref name="filePaths"/>. For each file, the
        /// iterator returns a FileStatus with success and filePath data.
        /// </returns>
        public static IEnumerable<FileStatus> DecompressAvLzToDisk(string[] filePaths, bool overwriteFiles)
        {
            for (int i = 0; i < filePaths.Length; i++)
            {
                var filePath = filePaths[i];
                bool success = DecompressAvLzToDisk(filePath, overwriteFiles);
                yield return new FileStatus(success, filePath);
            }
        }

        /// <summary>
        /// Deompresses all files in <paramref name="rootPath"/> using Amusement Vision LZ decompression
        /// and saves each file it to disk.
        /// </summary>
        /// <param name="rootPath">The folder to decompress files in.</param>
        /// <param name="overwriteFiles">Whether the opration can overwrite existing files on disk.</param>
        /// <param name="searchOption">The search option used to find files in the specified folder.</param>
        /// <param name="searchPattern">The search pattern used to find files in the specified folder.</param>
        /// <returns>
        /// Returns an iterator for each file found using the search parameters. For each file, the iterator
        /// returns a FileStatus with success and filePath data.
        /// </returns>
        public static IEnumerable<FileStatus> DecompressAvLzDirectoryToDisk(string rootPath, bool overwriteFiles, SearchOption searchOption, string searchPattern = "*.lz")
        {
            var filePaths = Directory.GetFiles(rootPath, searchPattern, searchOption);
            return DecompressAvLzToDisk(filePaths, overwriteFiles);
        }
    }
}