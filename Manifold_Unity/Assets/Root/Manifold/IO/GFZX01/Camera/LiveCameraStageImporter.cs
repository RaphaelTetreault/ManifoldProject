using System.IO;
using UnityEngine;

namespace Manifold.IO.GFZX01.Camera
{
    [CreateAssetMenu(menuName = MenuConst.GFZX01_Camera + "livecam_stage Importer")]
    public class LiveCameraStageImporter : ExecutableScriptableObject,
    IImportable
    {
        [Header("Import Settings")]
        [SerializeField]
        protected SearchOption fileSearchOption = SearchOption.AllDirectories;

        [SerializeField]
        protected string searchPattern = "livecam_stage*.bin";

        [SerializeField, BrowseFolderField("Assets/")]
        protected string importPath;

        [SerializeField, BrowseFolderField("Assets/")]
        protected string importDestination;

        [Header("Import Files")]
        [SerializeField]
        protected string[] importFiles;

        public override string ExecuteText => "Import livecam_stage";

        public override void Execute() => Import();

        public void Import()
        {
            importFiles = Directory.GetFiles(importPath, searchPattern, fileSearchOption);
            importFiles = UnityPathUtility.EnforceUnitySeparators(importFiles);
            ImportUtility.ImportManyAs<LiveCameraStageSobj>(importFiles, importPath, importDestination);
        }
    }
}
