using StarkTools.IO;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;

namespace Manifold.IO.GFZX01.Camera
{
    [CreateAssetMenu(menuName = MenuConst.GFZX01_Camera + "livecam_stage Importer")]
    public class LiveCameraStageImporter : ExecutableScriptableObject,
    IImportable
    {
        #region MEMBERS

        [Header("Import Settings")]
        [SerializeField]
        protected string searchPattern = "livecam_stage*.bin";

        [SerializeField, BrowseFolderField("Assets/")]
        [FormerlySerializedAs("importPath")]
        protected string importFrom;

        [SerializeField, BrowseFolderField("Assets/")]
        [FormerlySerializedAs("importPath")]
        protected string importTo;

        [SerializeField]
        protected SearchOption fileSearchOption = SearchOption.AllDirectories;

        [Header("Import Files")]
        [SerializeField]
        protected string[] importFiles;

        #endregion

        public override string ExecuteText => "Import livecam_stage";

        public override void Execute() => Import();

        public void Import()
        {
            importFiles = Directory.GetFiles(importFrom, searchPattern, fileSearchOption);
            importFiles = UnityPathUtility.EnforceUnitySeparators(importFiles);
            ImportUtility.ImportManyAs<LiveCameraStageSobj>(importFiles, importFrom, importTo);
        }
    }
}
