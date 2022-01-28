using GameCube.GFZ.CourseCollision;
using Manifold.IO;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;

namespace Manifold.EditorTools.GC.GFZ.CourseCollision
{
    [CreateAssetMenu(menuName = Const.Menu.GfzCourseCollision + "COLI Importer")]
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

            // 
            //var importFilesUncompressed = isAX
            //GfzUtility.DecompressEachLZ(importFiles, LibGxFormat.AvGame.FZeroAX);

            // TODO: implement check for non LZ files

            var importFilesUncompressed = GfzUtility.DecompressEachLZ(importFiles);
            ImportUtility.ImportManyAs<ColiSceneSobj>(importFilesUncompressed, importFrom, importTo);
        }
    }
}
