using System;
using UnityEngine;

namespace Manifold.IO.GFZX01.GMA
{
    [CreateAssetMenu(menuName = MenuConst.GFZX01_GMA + "GMA Exporter")]
    public class GmaExporter : ExecutableScriptableObject,
          IExportable
    {
        [Header("Export Settings")]
        [SerializeField]
        protected IOOption exportOptions = IOOption.selectedFiles;

        [SerializeField, BrowseFolderField, Tooltip("Used for exportOptions")]
        protected string exportFrom = string.Empty;

        [SerializeField, BrowseFolderField]
        protected string exportTo;
        
        [SerializeField]
        protected bool preserveFolderStructure = true;
        
        [SerializeField]
        protected bool exportCompressed = false;
        
        [Header("Preferences")]
        [SerializeField]
        protected bool openFolderAfterExport = true;

        [Header("Exports")]
        [SerializeField]
        protected GMASobj[] exportSobjs;


        public override string ExecuteText => "Export GMA";

        public override void Execute() => Export();

        public void Export()
        {
            exportSobjs = IOUtility.GetSobjByOption(exportSobjs, exportOptions, exportFrom);
            var exportedFiles = ExportUtility.ExportFiles(exportSobjs, exportTo, "gma", preserveFolderStructure);
            ExportUtility.PrintExportsToConsole(this, exportedFiles);

            if (exportCompressed)
            {
                var compressedFiles = GFZX01Utility.CompressEachAsLZ(exportedFiles);
                ExportUtility.PrintExportsToConsole(this, compressedFiles);
            }

            IOUtility.OpenDirectoryIf(openFolderAfterExport, exportedFiles);
        }
    }

}