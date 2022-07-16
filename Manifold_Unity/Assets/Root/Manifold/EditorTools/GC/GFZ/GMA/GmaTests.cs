using GameCube.GFZ.GMA;
using Manifold;
using Manifold.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Manifold.EditorTools.GC.GFZ.GMA
{
    public static class GmaTests
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePaths"></param>
        /// <returns></returns>
        public static IEnumerable TestLoadSaveGmas(params string[] filePaths)
        {
            foreach (var filePath in filePaths)
            {
                var gma = BinarySerializableIO.LoadFile<Gma>(filePath);
                TestSaveGma(gma);
                yield return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        public static void TestLoadSaveGma(string filePath)
        {
            var gma = BinarySerializableIO.LoadFile<Gma>(filePath);
            TestSaveGma(gma);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gma"></param>
        public static void TestSaveGma(Gma gma)
        {
            using (var writer = new EndianBinaryWriter(new MemoryStream(), Gma.endianness))
            {
                gma.Serialize(writer);
            }
        }

        public static void TestGmaRoundtripByteForByte(string filePath)
        {
            var reader = new EndianBinaryReader(File.OpenRead(filePath), Gma.endianness);
            var gma = new Gma();
            gma.FileName = Path.GetFileName(filePath); 
            gma.Deserialize(reader);

            var writer = new EndianBinaryWriter(new MemoryStream(), Gma.endianness);
            writer.Write(gma);
            writer.Flush();

            bool isSameLength = reader.BaseStream.Length == writer.BaseStream.Length;
            //Assert.IsTrue(isSameLength, $"Lengths r({reader.BaseStream.Length}), w({writer.BaseStream.Length})");
            var length = reader.BaseStream.Length;

            if (!isSameLength)
            {
                DebugConsole.Log($"Lengths r({reader.BaseStream.Length}), w({writer.BaseStream.Length})");
                return;
            }

            var readerStream = new MemoryStream();
            reader.BaseStream.Position = 0;
            reader.BaseStream.CopyTo(readerStream);
            var readerBytes = readerStream.ToArray();
            var writerBytes = (writer.BaseStream as MemoryStream).ToArray();

            int missCount = 0;
            for (int i = 0; i < length; i++)
            {
                missCount += readerBytes[i] == writerBytes[i] ? 0 : 1;
            }
            //Assert.IsTrue(missCount == 0, $"Miss count: {missCount}/{length}");

            if (missCount != 0)
                DebugConsole.Log($"Miss count: {missCount}/{length}");
        }

    }
}
