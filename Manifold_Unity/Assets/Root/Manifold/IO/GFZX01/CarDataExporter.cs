using GameCube.FZeroGX.CarData;
using UnityEngine;

namespace Manifold.IO.GFZX01
{
    [CreateAssetMenu(menuName = "Manifold/Export/" + "NEW CarData Exporter")]
    public class CarDataExporter : ExecutableScriptableObject,
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
        protected CarDataSobj[] exportSobjs;



        public override string ExecuteText => "Export CarData";

        public override void Execute() => Export();

        public void Export()
        {
            exportSobjs = IOUtility.GetSobjByOption(exportSobjs, exportOptions, exportFrom);

            var exportedFiles = ExportUtility.ExportFiles(exportSobjs, exportTo, "", allowOverwritingFiles, preserveFolderStructure);
            ExportUtility.PrintExportsToConsole(this, exportedFiles);
            if (openFolderAfterExport)
            {
                IOUtility.OpenFileFolder(exportedFiles);
            }

            if (exportCompressed)
            {
                var compressedFiles = GFZX01Utility.CompressEachAsLZ(exportedFiles, allowOverwritingFiles);
                ExportUtility.PrintExportsToConsole(this, compressedFiles);
            }
        }

    }
}