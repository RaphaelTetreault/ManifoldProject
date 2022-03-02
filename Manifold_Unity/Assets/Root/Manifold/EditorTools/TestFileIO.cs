using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Manifold.EditorTools
{
    public static class TestFileIO
    {
        public delegate void FileAction(string filePath);
        public delegate TValue LoadFile<TValue>(string filePath);
        public delegate void SaveFile<TValue>(TValue value, string filePath);

        /// <summary>
        /// Calls the <paramref name="fileAction"/> on every file in <paramref name="filePaths"/>.
        /// </summary>
        /// <param name="title">The title that appears on the progress bar.</param>
        /// <param name="filePaths">The files paths to use in the supplied action.</param>
        /// <param name="fileAction">The action to perform using each file path.</param>
        public static void FileActionLoop(string title, string[] filePaths, FileAction fileAction)
        {
            int index = 0;
            float count = filePaths.Length;
            int countFormat = count.ToString().Length;
            foreach (var filePath in filePaths)
            {
                index++;
                string info = $"[{index.ToString().PadLeft(countFormat)}/{count}] {filePath}";
                float progress = index / count;
                bool cancel = EditorUtility.DisplayCancelableProgressBar(title, info, progress);
                if (cancel)
                    break;

                fileAction(filePath);
            }
            EditorUtility.ClearProgressBar();
        }

        /// <summary>
        /// Loads the file at <paramref name="filePath"/> and saves it to <paramref name="outputPath"/>. A
        /// carbon copy of the loaded file is saved in the same output directory.
        /// </summary>
        /// <typeparam name="T">The type used by the load and save functions.</typeparam>
        /// <param name="filePath">The file path to load from.</param>
        /// <param name="rootPath">The root path of the loaded file. Used to make absolute path a relative path.</param>
        /// <param name="outputPath">The output directory for files.</param>
        /// <param name="load">The function which loads and returns the structure.</param>
        /// <param name="save">The action that saves the structure to disk.</param>
        public static void LoadSaveToDisk<T>(string filePath, string rootPath, string outputPath, LoadFile<T> load, SaveFile<T> save)
        {
            // Get directory to file relative to rootFolder
            var folderPath = Path.GetDirectoryName(filePath);
            folderPath = folderPath.Remove(0, rootPath.Length);
            // Get file parameters
            var name = Path.GetFileNameWithoutExtension(filePath);
            var ext = Path.GetExtension(filePath);
            // Create path to loaded file, saved file
            var inputFilePath = Path.Combine(outputPath, folderPath, $"{name}{ext}");
            var outputFilePath = Path.Combine(outputPath, folderPath, $"{name}.export{ext}");
            // Make sure the directory exists for the output files
            var outputDirectory = Path.GetDirectoryName(inputFilePath);
            if (!Directory.Exists(outputDirectory))
                Directory.CreateDirectory(outputDirectory);

            //
            var value = load(filePath);
            save(value, outputFilePath);

            // Make copy of file
            File.Copy(filePath, inputFilePath, true);

            Debug.Log($"Read '{filePath}', Wrote '{outputFilePath}'");
        }

    }
}
