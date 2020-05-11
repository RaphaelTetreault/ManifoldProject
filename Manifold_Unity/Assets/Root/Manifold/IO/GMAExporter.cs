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
        [SerializeField]
        [BrowseFolderField]
        [Tooltip("Used for ExportOptions.ExportAllOfTypeInFolder")]
        protected string exportSource = string.Empty;

        [SerializeField]
        [BrowseFolderField]
        protected string exportDestination;
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
                    exportSobjs = ExportUtility.LoadAllOfTypeFromAssetDatabase<GMASobj>();
                    break;
                case ExportUtility.ExportOptions.ExportAllOfTypeInFolder:
                    exportSobjs = ExportUtility.LoadAllOfTypeFromAssetDatabase<GMASobj>();
                    break;

                default:
                    throw new NotImplementedException();
            }
            ExportUtility.ExportFilesFrom<GMASobj>(exportSobjs, exportDestination, extension, preserveFolderStructure);
        
            if (!exportCompressed)
            {
                throw new NotImplementedException();
            }
        }
    }

}