using GameCube.GFZ.CourseCollision;
using Manifold.IO;
using Manifold.IO.GFZ;
using UnityEditor;

namespace Manifold.EditorTools.GFZ.Menu
{
    public static class TextLogStageMenu
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
                    StageTextLogger.LogScene(log, coliScene);
                }
            }

            OSUtility.OpenDirectory(settings.LogOutput);
        }
    }
}
