using StarkTools.IO;
using System.IO;
using UnityEngine;

namespace Manifold.IO.GFZX01.StaffGhost
{
    [CreateAssetMenu(menuName = MenuConst.GFZX01_StaffGhost + "StaffGhost Importer")]

    public class StaffGhostImporter : ExecutableScriptableObject,
        IImportable
        {
            #region MEMBERS

            [Header("Import Settings")]
            [SerializeField, BrowseFolderField("Assets/")]
            protected string importFrom;

            [SerializeField, BrowseFolderField("Assets/")]
            protected string importTo;

            [SerializeField]
            protected SearchOption fileSearchOption = SearchOption.AllDirectories;

            [SerializeField]
            protected string searchPattern = "*.bin";
            [Header("Import Files")]
            [SerializeField]
            protected string[] importFiles;

            #endregion

            public override string ExecuteText => "Import Staff Ghost";

            public override void Execute() => Import();

            public void Import()
            {
                importFiles = Directory.GetFiles(importFrom, searchPattern, fileSearchOption);
                importFiles = UnityPathUtility.EnforceUnitySeparators(importFiles);
                //var importFilesUncompressed = GFZX01Utility.DecompressEachLZ(importFiles);
                ImportUtility.ImportManyAs<StaffGhostSobj>(importFiles, importFrom, importTo);
            }
        }
}
