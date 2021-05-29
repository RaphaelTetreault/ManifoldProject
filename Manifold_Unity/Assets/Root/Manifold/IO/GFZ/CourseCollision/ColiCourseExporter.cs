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


        public override string ExecuteText => "Export COLI_COURSE";

        public override void Execute() => Export();

        public void Export()
        {
            if (exportHandMade)
            {
                var temp = new ColiScene[]
                   {
                        new ColiScene() {
                            FileName = "COLI_COURSE999",
                            header = new Header()
                            {
                                Format = Header.SerializeFormat.GX,
                            },
                        }
                   };
                var exportedFiles = ExportUtility.ExportSerializable(temp, exportTo, "", allowOverwritingFiles);
                if (exportCompressed)
                {
                    var compressedFiles = GfzUtility.CompressEachAsLZ(exportedFiles, allowOverwritingFiles);
                    ExportUtility.PrintExportsToConsole(this, compressedFiles);
                }
                OSUtility.OpenDirectory(openFolderAfterExport, exportedFiles);
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
