using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Manifold.IO
{
    public static class BinarySerializableIO
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TBinarySerializable"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static TBinarySerializable LoadFile<TBinarySerializable>(string filePath)
            where TBinarySerializable : IBinarySerializable, IFile, new()
        {
            using (var reader = new BinaryReader(File.OpenRead(filePath)))
            {
                var binarySerializable = new TBinarySerializable();
                binarySerializable.FileName = Path.GetFileName(filePath);
                binarySerializable.Deserialize(reader);
                return binarySerializable;
            }
        }

        /// <summary>
        /// Loads file at and stores valie in <paramref name="binarySerializable"/> without creating a new instance.
        /// </summary>
        /// <typeparam name="TBinarySerializable">A type implementing the IBinarySerialize interface.</typeparam>
        /// <param name="filePath">The file path to load from.</param>
        /// <param name="binarySerializable">The instance to deserialize the file to.</param>
        /// <returns></returns>
        public static TBinarySerializable LoadFile<TBinarySerializable>(string filePath, TBinarySerializable binarySerializable)
            where TBinarySerializable : IBinarySerializable, IFile
        {
            using (var reader = new BinaryReader(File.OpenRead(filePath)))
            {
                binarySerializable.FileName = Path.GetFileName(filePath);
                binarySerializable.Deserialize(reader);
                return binarySerializable;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TBinarySerializable"></typeparam>
        /// <param name="filePaths"></param>
        /// <param name="binarySerializables"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IEnumerable<TBinarySerializable> LoadFile<TBinarySerializable>(string[] filePaths, TBinarySerializable[] binarySerializables)
            where TBinarySerializable : IBinarySerializable, IFile, new()
        {
            // Make sure arays are same length
            if (filePaths.Length != binarySerializables.Length)
                throw new ArgumentException($"{nameof(filePaths)} and {nameof(binarySerializables)} must have the same length.");

            // Make sure binarySerializables passed in are not null
            foreach (var binarySerializable in binarySerializables)
                if (binarySerializable is null)
                    throw new ArgumentException($"All instances in {nameof(binarySerializables)} must not be null.");

            // Load
            for (int i = 0; i < filePaths.Length; i++)
            {
                var value = LoadFile(filePaths[i], binarySerializables[i]);
                yield return value;
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TBinarySerializable"></typeparam>
        /// <param name="filePaths"></param>
        /// <returns></returns>
        public static IEnumerable<TBinarySerializable> LoadFile<TBinarySerializable>(params string[] filePaths)
            where TBinarySerializable : IBinarySerializable, IFile, new()
        {
            foreach (string filePath in filePaths)
            {
                var value = LoadFile<TBinarySerializable>(filePath);
                yield return value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TBinarySerializable"></typeparam>
        /// <param name="binarySerializable"></param>
        /// <param name="filePath"></param>
        public static void SaveFile<TBinarySerializable>(TBinarySerializable binarySerializable, string filePath)
            where TBinarySerializable : IBinarySerializable, IFile
        {
            using (var writer = new BinaryWriter(File.Create(filePath)))
            {
                binarySerializable.FileName = Path.GetFileName(filePath);
                binarySerializable.Serialize(writer);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TBinarySerializable"></typeparam>
        /// <param name="rootPath"></param>
        /// <param name="binarySerializables"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="IOException"></exception>
        public static IEnumerable SaveFile<TBinarySerializable>(string rootPath, TBinarySerializable[] binarySerializables)
            where TBinarySerializable : IBinarySerializable, IFile
        {
            // Do a sanity check on parameters before serializing
            foreach (var binarySerializable in binarySerializables)
            {
                if (binarySerializable is null)
                {
                    var msg = "Cannot serialize null value!";
                    throw new NullReferenceException(msg);
                }

                if (string.IsNullOrEmpty(binarySerializable.FileName))
                {
                    var msg = $"Cannot serialize without a filename! (Assign name to {nameof(binarySerializable.FileName)})";
                    throw new IOException(msg);
                }
            }

            foreach (var binarySerializable in binarySerializables)
            {
                var filePath = Path.Combine(rootPath, binarySerializable.FileName);
                SaveFile(binarySerializable, filePath);
                yield return null;
            }
        }
    }
}
