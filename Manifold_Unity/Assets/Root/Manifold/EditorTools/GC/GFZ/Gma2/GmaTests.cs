using GameCube.GFZ.Gma2;
using Manifold;
using Manifold.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Manifold.EditorTools.GC.GFZ.Gma2
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
                var gma = GmaIO.LoadGMA(filePath);
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
            var gma = GmaIO.LoadGMA(filePath);
            TestSaveGma(gma);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gma"></param>
        public static void TestSaveGma(Gma gma)
        {
            using (var writer = new BinaryWriter(new MemoryStream()))
            {
                gma.Serialize(writer);
            }
        }

    }
}
