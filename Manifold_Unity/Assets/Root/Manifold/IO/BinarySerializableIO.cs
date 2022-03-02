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
        /// 
        /// </summary>
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
        /// <param name="filePath"></param>
        /// <param name="binarySerializable"></param>
        public static void SaveFile<TBinarySerializable>(TBinarySerializable binarySerializable, string filePath)
            where TBinarySerializable : IBinarySerializable, IFile
        {
            using (var writer = new BinaryWriter(File.OpenWrite(filePath)))
            {
                binarySerializable.FileName = Path.GetFileName(filePath);
                binarySerializable.Serialize(writer);
            }
        }

        /// <summary>
        /// 
        /// </summary>
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
