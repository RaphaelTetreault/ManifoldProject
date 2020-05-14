using UnityEngine;

namespace Manifold.IO.GFZX01
{
    [CreateAssetMenu(menuName = "Manifold/Export/" + "livecam_stage EX Exporter")]
    public class LiveCameraStageExExporter : ExecutableScriptableObject,
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

        [Header("Preferences")]
        [SerializeField]
        protected bool openFolderAfterExport = true;

        [Header("Exports")]
        [SerializeField]
        protected LiveCameraStageExSobj[] exportSobjs;



        public override string ExecuteText => "Export livecam_stage";

        public override void Execute() => Export();

        public void Export()
        {
            exportSobjs = IOUtility.GetSobjByOption(exportSobjs, exportOptions, exportFrom);

            var exportedFiles = ExportUtility.ExportFiles(exportSobjs, exportTo, "bin", allowOverwritingFiles, preserveFolderStructure);
            ExportUtility.PrintExportsToConsole(this, exportedFiles);
            if (openFolderAfterExport)
            {
                IOUtility.OpenFileFolder(exportedFiles);
            }
        }

    }
}