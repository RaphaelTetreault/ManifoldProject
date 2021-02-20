using GameCube.GFZ.CourseCollision;
using Manifold.IO;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;

namespace Manifold.IO.GFZ.CourseCollision
{
    [CreateAssetMenu(menuName = MenuConst.GfzCourseCollision + "COLI Importer")]
    public class ColiImporter : ExecutableScriptableObject,
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
        protected string searchPattern = "COLI_COURSE*";

        [SerializeField]
        protected bool isAX;

        [Header("Import Files")]
        [SerializeField]
        protected string[] importFiles;

        #endregion

        public override string ExecuteText => "Import COLI";

        public override void Execute() => Import();

        public void Import()
        {
            importFiles = Directory.GetFiles(importFrom, searchPattern, fileSearchOption);
            importFiles = UnityPathUtility.EnforceUnitySeparators(importFiles);
            var importFilesUncompressed = isAX
                ? GFZX01Utility.DecompressEachLZ(importFiles, LibGxFormat.AvGame.FZeroAX)
                : GFZX01Utility.DecompressEachLZ(importFiles);
            ImportUtility.ImportManyAs<ColiSceneSobj>(importFilesUncompressed, importFrom, importTo);
        }
    }
}
