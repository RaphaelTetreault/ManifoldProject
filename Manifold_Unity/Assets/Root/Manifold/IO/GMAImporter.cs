using System.IO;
using UnityEngine;

namespace Manifold.IO.GFZX01
{
    [CreateAssetMenu(menuName = "Manifold/Import/" + "NEW GMA Importer")]
    public class GMAImporter : ExecutableScriptableObject,
        IImportable
    {
        [Header("Import Settings")]
        [SerializeField]
        protected SearchOption fileSearchOption = SearchOption.AllDirectories;

        [SerializeField]
        protected string searchPattern = "*.GMA OR *.GMA.LZ";

        [SerializeField, BrowseFolderField("Assets/")]
        protected string importPath;

        [SerializeField, BrowseFolderField("Assets/")]
        protected string importDestination;

        [Header("Import Files")]
        [SerializeField]
        protected string[] importFiles;


        public override string ExecuteText => "Import GMA";

        public override void Execute() => Import();

        public void Import()
        {
            var importFiles = Directory.GetFiles(importPath, searchPattern, fileSearchOption);
            var importFilesUncompressed = GFZX01Utility.DecompressAnyLZ(importFiles);
            ImportUtility.ImportFilesAs<GMASobj>(importFilesUncompressed, importPath, importDestination);
        }


    }
}