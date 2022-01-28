using Manifold.IO;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.GMA
{
    [CreateAssetMenu(menuName = Const.Menu.GfzGMA + "GMA Exporter")]
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
        protected GmaSobj[] exportSobjs;


        public override string ExecuteText => "Export GMA";

        public override void Execute() => Export();

        public void Export()
        {
            exportSobjs = AssetDatabaseUtility.GetSobjByOption(exportSobjs, exportOptions, exportFrom);
            var exportedFiles = ExportUtility.ExportFiles(exportSobjs, exportTo, "gma", preserveFolderStructure);
            ExportUtility.PrintExportsToConsole(this, exportedFiles);

            if (exportCompressed)
            {
                var compressedFiles = GfzUtility.CompressEachAsLZ(exportedFiles);
                ExportUtility.PrintExportsToConsole(this, compressedFiles);
            }

            OSUtility.OpenDirectory(openFolderAfterExport, exportedFiles);
        }
    }

}