using System.IO;
using UnityEngine;
using UnityEditor;
using StarkTools.IO;
using Boo.Lang;
using System;

namespace Manifold.IO
{
    [CreateAssetMenu(menuName = "Manifold/Export/" + "NEW GMA Exporter")]
    public class GMAExporter : ExecutableScriptableObject,
          IExportable
    {
        [Header("Export Settings")]
        [SerializeField]
        protected ExportUtility.ExportOptions exportOptions
            = ExportUtility.ExportOptions.ExportFiles;

        [SerializeField, BrowseFolderField, Tooltip("Used for ExportOptions.ExportAllOfTypeInFolder")]
        protected string exportFrom = string.Empty;

        [SerializeField, BrowseFolderField]
        protected string exportTo;
        [SerializeField]
        protected string extension = ".gma";
        [SerializeField]
        protected bool preserveFolderStructure = true;
        [SerializeField]
        protected bool exportCompressed = false;

        [SerializeField]
        protected GMASobj[] exportSobjs;


        public override string ExecuteText => "Export GMA";

        public override void Execute() => Export();

        public void Export()
        {
            var exportSobjs = this.exportSobjs;
            switch (exportOptions)
            {
                case ExportUtility.ExportOptions.ExportFiles:
                    break;

                case ExportUtility.ExportOptions.ExportAllOfType:
                    exportSobjs = AssetDatabaseUtility.GetAllOfType<GMASobj>();
                    break;
                case ExportUtility.ExportOptions.ExportAllOfTypeInFolder:
                    exportSobjs = AssetDatabaseUtility.GetAllOfType<GMASobj>(exportFrom);
                    break;

                default:
                    throw new NotImplementedException();
            }
            var exportedFiles = ExportUtility.ExportFiles(exportSobjs, exportTo, extension, preserveFolderStructure);
        
            if (exportCompressed)
            {
                var compressedFiles = GFZX01Utility.CompressEachAsLZ(exportedFiles);
            }
        }
    }

}