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
        public SearchOption fileSearchOption = SearchOption.AllDirectories;

        [SerializeField]
        public string searchPattern = "*.GMA";

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
            importFiles = Directory.GetFiles(importPath, searchPattern, fileSearchOption);
            ImportUtility.ImportFilesAs<GMASobj>(importFiles, importPath, importDestination);
        }
    }
}