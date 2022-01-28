using Manifold;
using Manifold.IO;
using Manifold.IO.GFZ;
using Manifold.EditorTools;
using GameCube.GFZ;
using GameCube.GFZ.CourseCollision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

namespace Manifold.EditorTools.GFZ.Menu
{
    public static class TextLogStage
    {
        public const string ActiveRoot = " (Active Root)";

        /// <summary>
        /// Writes simple log which enumerates all data with ToString() call.
        /// </summary>
        [MenuItem(Const.Menu.logs + "Log All Stages" + ActiveRoot)]
        public static void LogAllStage()
        {
            var settings = GfzProjectWindow.GetSettings();

            foreach (var coliScene in ColiCourseIO.LoadAllStages(settings.StageDir, "Logging Stages..."))
            {
                var outputFile = $"{settings.LogOutput}/log-{coliScene.FileName}.txt";
                using (var log = new TextLogger(outputFile))
                {
                    TextLog.Stage.LogScene(log, coliScene);
                }
            }

            OSUtility.OpenDirectory(settings.LogOutput);
        }
    }
}
