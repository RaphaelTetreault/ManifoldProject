using GameCube.GFZ.CourseCollision;
using UnityEngine;

namespace Manifold.IO.GFZ.CourseCollision
{

    [CreateAssetMenu(menuName = Const.Menu.GfzCourseCollision + "COLI Exporter")]
    public class ColiCourseExporter : ExecutableScriptableObject,
        IExportable
    {
        [Header("Export Settings")]
        [SerializeField]
        protected IOOption exportOptions = IOOption.selectedFiles;

        [SerializeField, BrowseFolderField, Tooltip("Used for ExportOptions.ExportAllOfTypeInFolder")]
        protected string exportFrom = string.Empty;

        [SerializeField, BrowseFolderField]
        protected string exportTo;

        [SerializeField]
        protected bool allowOverwritingFiles = true;

        [SerializeField]
        protected bool preserveFolderStructure = true;

        [SerializeField]
        protected bool exportCompressed = false;

        [Header("Preferences")]
        [SerializeField]
        protected bool openFolderAfterExport = true;

        [Header("Exports")]
        [SerializeField]
        protected ColiSceneSobj[] exportSobjs;

        [Header("Testing")]
        [SerializeField]
        protected bool exportHandMade;
        [SerializeField]
        protected bool serializeVerbose = true;

        [Header("Export Overrides")]
        [SerializeField] protected bool exclude;

        public override string ExecuteText => "Export COLI_COURSE";

        public override void Execute() => Export();

        public void Export()
        {
            ColiCourseUtility.SerializeVerbose = serializeVerbose;

            if (exportHandMade)
            {
                throw new System.NotImplementedException();
            }
            else
            {
                exportSobjs = AssetDatabaseUtility.GetSobjByOption(exportSobjs, exportOptions, exportFrom);
                var exportedFilesX = ExportUtility.ExportFiles(exportSobjs, exportTo, "", allowOverwritingFiles, preserveFolderStructure);
                //ExportUtility.PrintExportsToConsole(this, exportedFilesX);
                OSUtility.OpenDirectory(openFolderAfterExport, exportedFilesX);
            }



        }
    }
}
