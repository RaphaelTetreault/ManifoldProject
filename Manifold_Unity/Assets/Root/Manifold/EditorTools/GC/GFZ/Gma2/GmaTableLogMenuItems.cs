using GameCube.GFZ.Gma2;
using Manifold;
using Manifold.IO;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

using static GameCube.GFZ.Gma2.GmaTableLogger;


namespace Manifold.EditorTools.GC.GFZ.Gma2
{
    internal static class GmaTableLogMenuItems
    {
        private const string menu = Const.Menu.Manifold + "Analysis/GMA/";


        /// <summary>
        /// Forwards most analysis functions 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="outputFileName"></param>
        public static void MenuForward(Action<Gma[], string> action, string outputFileName)
        {
            var settings = GfzProjectWindow.GetSettings();
            var inputPath = settings.RootFolder;
            var outputPath = settings.AnalysisOutput + outputFileName;

            var filePaths = Directory.GetFiles(inputPath, "*.gma", SearchOption.AllDirectories);
            var gmaIterator = BinarySerializableIO.LoadFile<Gma>(filePaths);
            var gmas = FileUtility.IteratorCanceableProgressBar(gmaIterator, filePaths.Length);

            action.Invoke(gmas, outputPath);
            OSUtility.OpenDirectory(settings.AnalysisOutput);
        }



        [MenuItem(menu + "All GMA Log Outputs")]
        public static void MenuPrintGmaAll() => MenuForward(PrintGmaAll, "");

        [MenuItem(menu + "Analyze GMA.GCMF")]
        public static void MenuPrintGcmf() => MenuForward(PrintGcmf, tsvGcmf);

        [MenuItem(menu + "Analyze GMA.TextureConfigs")]
        public static void MenuPrintTextureConfig() => MenuForward(PrintTextureConfigs, tsvTextureConfigs);

        [MenuItem(menu + "Analyze GMA.Submesh.Materials")]
        public static void MenuPrintMaterials() => MenuForward(PrintMaterials, tsvMaterials);



    }
}
