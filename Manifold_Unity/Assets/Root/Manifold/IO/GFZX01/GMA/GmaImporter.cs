using StarkTools.IO;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;

namespace Manifold.IO.GFZX01.GMA
{
    [CreateAssetMenu(menuName = MenuConst.GFZX01_GMA + "GMA Importer")]
    public class GmaImporter : ExecutableScriptableObject,
        IImportable
    {
        #region MEMBERS

        [Header("Import Settings")]
        [SerializeField, BrowseFolderField("Assets/")]
        [FormerlySerializedAs("importPath")]
        protected string importFrom;
        
        [SerializeField, BrowseFolderField("Assets/")]
        [FormerlySerializedAs("importDestination")]
        protected string importTo;

        [SerializeField]
        protected SearchOption fileSearchOption = SearchOption.AllDirectories;

        [SerializeField]
        protected string searchPattern = "*.GMA*";
        [Header("Import Files")]
        [SerializeField]
        protected string[] importFiles;

        #endregion

        public override string ExecuteText => "Import GMA";

        public override void Execute() => Import();

        public void Import()
        {
            importFiles = Directory.GetFiles(importFrom, searchPattern, fileSearchOption);
            importFiles = UnityPathUtility.EnforceUnitySeparators(importFiles);
            var importFilesUncompressed = GFZX01Utility.DecompressEachLZ(importFiles);
            ImportUtility.ImportManyAs<GMASobj>(importFilesUncompressed, importFrom, importTo);
        }

    }
}