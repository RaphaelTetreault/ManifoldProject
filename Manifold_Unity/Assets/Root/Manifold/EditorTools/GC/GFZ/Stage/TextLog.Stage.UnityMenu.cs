﻿using GameCube.GFZ.Stage;
using Manifold.IO;
using Manifold.EditorTools;
using System.IO;
using System.Text;
using UnityEditor;
using Newtonsoft.Json;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public static class TextLogStageMenu
    {
        /// <summary>
        /// Writes simple log which enumerates all data with ToString() call.
        /// </summary>
        [MenuItem(Const.Menu.logs + "Log All Stages (Active Root)")]
        public static void LogAllStage()
        {
            var settings = GfzProjectWindow.GetSettings();
            var inputPath = settings.SourceStageDirectory;
            var inputPaths = Directory.GetFiles(inputPath, "COLI_COURSE???");
            var scenes = BinarySerializableIO.LoadFile<Scene>(inputPaths);

            var stringBuilder = new StringBuilder();

            int count = 0;
            int total = inputPaths.Length;
            foreach (var scene in scenes)
            {
                var outputFile = $"{settings.LogOutput}log-{scene.FileName}.txt";
                var cancel = ProgressBar.ShowIndexed(count++, total, "Logging Scenes", outputFile);
                if (cancel) break;

                using (var file = File.CreateText(outputFile))
                {
                    stringBuilder.Clear();
                    scene.PrintMultiLine(stringBuilder);
                    file.Write(stringBuilder);
                }
            }

            OSUtility.OpenDirectory(settings.LogOutput);
            ProgressBar.Clear();
        }

        /// <summary>
        /// Writes simple log which enumerates all data with ToString() call.
        /// </summary>
        [MenuItem(Const.Menu.logs + "TEST - JSON")]
        public static void TestJson()
        {
            var settings = GfzProjectWindow.GetSettings();
            var inputPath = settings.SourceStageDirectory;
            var inputPaths = Directory.GetFiles(inputPath, "COLI_COURSE???");
            var scenes = BinarySerializableIO.LoadFile<Scene>(inputPaths);

            int count = 0;
            int total = inputPaths.Length;
            foreach (var scene in scenes)
            {
                var outputFile = $"{settings.LogOutput}{scene.FileName}.json";
                var cancel = ProgressBar.ShowIndexed(count++, total, "JSON-ing Scenes", outputFile);
                if (cancel) break;

                using (var file = File.CreateText(outputFile))
                {
                    string json = UnityEngine.JsonUtility.ToJson(scene, true);
                    file.Write(json);
                }
            }

            OSUtility.OpenDirectory(settings.LogOutput);
            ProgressBar.Clear();
        }
    }
}
