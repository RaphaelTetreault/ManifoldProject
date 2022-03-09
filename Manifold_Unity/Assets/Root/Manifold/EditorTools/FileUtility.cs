using Manifold.IO;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Manifold.EditorTools
{
    public static class FileUtility
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
        /// <param name="saveCopy"></param>
        public static string LoadSaveToDisk<T>(string filePath, string rootPath, string outputPath, LoadFile<T> load, SaveFile<T> save, bool saveCopy = true)
        {
            // Get directory to file relative to rootFolder
            var folderPath = Path.GetDirectoryName(filePath);
            folderPath = folderPath.Remove(0, rootPath.Length);
            // Get file parameters
            var name = Path.GetFileNameWithoutExtension(filePath);
            var ext = Path.GetExtension(filePath);
            // Create path to loaded file, saved file
            var copyFilePath = Path.Combine(outputPath, folderPath, $"{name}.copy{ext}");
            var saveFilePath = Path.Combine(outputPath, folderPath, $"{name}{ext}");
            // Make sure the directory exists for the output files
            var outputDirectory = Path.GetDirectoryName(copyFilePath);
            if (!Directory.Exists(outputDirectory))
                Directory.CreateDirectory(outputDirectory);

            // Save
            var value = load(filePath);
            save(value, saveFilePath);
            
            // Make copy of file
            if (saveCopy)
            {
                File.Copy(filePath, copyFilePath, true);
            }

            Debug.Log($"Read '{filePath}', Wrote '{saveFilePath}'");
            
            return saveFilePath;
        }

        /// <summary>
        /// Displays a cancealable progress bar for each iteration, returning a final array once complete or iterrupted.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="iterator">The iterator to iterate through.</param>
        /// <param name="count">How many items are in the iterator.</param>
        /// <returns></returns>
        public static T[] IteratorCanceableProgressBar<T>(IEnumerable<T> iterator, int count)
            where T : IFileType
        {
            T[] values = new T[count];
            int index = 0;
            int countFormat = count.ToString().Length;
            foreach (var item in iterator)
            {
                // Copy value in
                values[index] = item;

                index++;
                string info = $"[{index.ToString().PadLeft(countFormat)}/{count}] {item.FileName}";
                float progress = (float)index / count;
                bool cancel = EditorUtility.DisplayCancelableProgressBar("Loading GMA Files", info, progress);
                if (cancel)
                    break;
            }
            EditorUtility.ClearProgressBar();
            return values;
        }
    }
}
