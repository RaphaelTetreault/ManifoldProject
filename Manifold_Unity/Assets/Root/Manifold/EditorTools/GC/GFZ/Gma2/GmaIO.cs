using GameCube.GFZ.Gma2;
using Manifold;
using Manifold.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Manifold.EditorTools.GC.GFZ.Gma2
{
    public static class GmaIO
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static Gma LoadGMA(string filePath)
        {
            using (var reader = new BinaryReader(File.OpenRead(filePath)))
            {
                var gma = new Gma();
                gma.FileName = Path.GetFileName(filePath);
                gma.Deserialize(reader);
                return gma;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePaths"></param>
        /// <returns></returns>
        public static IEnumerable<Gma> LoadGMAs(params string[] filePaths)
        {
            foreach (string filePath in filePaths)
            {
                var gma = LoadGMA(filePath);
                yield return gma;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="gma"></param>
        public static void SaveGMA(Gma gma, string filePath)
        {
            using (var writer = new BinaryWriter(File.OpenWrite(filePath)))
            {
                gma.FileName = Path.GetFileName(filePath);
                gma.Serialize(writer);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootPath"></param>
        /// <param name="gmas"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="IOException"></exception>
        public static IEnumerable SaveGMAs(string rootPath, Gma[] gmas)
        {
            // Do a sanity check on parameters before serializing
            foreach (var gma in gmas)
            {
                if (gma is null)
                {
                    var msg = "Cannot serialize null GMA!";
                    throw new NullReferenceException(msg);
                }

                if (string.IsNullOrEmpty(gma.FileName))
                {
                    var msg = $"Cannot serialize GMA without a filename! (Assign name to {nameof(gma)}.{nameof(gma.FileName)})";
                    throw new IOException(msg);
                }
            }

            foreach (var gma in gmas)
            {
                var filePath = Path.Combine(rootPath, gma.FileName);
                SaveGMA(gma, filePath);
                yield return null;
            }
        }
    }
}
