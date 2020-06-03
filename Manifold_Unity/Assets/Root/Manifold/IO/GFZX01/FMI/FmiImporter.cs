using GameCube.GFZX01.FMI;
using StarkTools.IO;
using System;
using System.IO;
using UnityEngine;

namespace Manifold.IO.GFZX01.FMI
{
    [UnityEngine.CreateAssetMenu(menuName = MenuConst.GFZX01_FMI + "FMI Importer")]
    public class FmiImporter : ExecutableScriptableObject,
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
        protected string searchPattern = "*.FMI";

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
            //var importFilesUncompressed = GFZX01Utility.DecompressEachLZ(importFiles);
            ImportUtility.ImportManyAs<FmiSobj>(importFiles, importFrom, importTo);
        }

    }
}