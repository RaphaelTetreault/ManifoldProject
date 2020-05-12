﻿using System.IO;
using UnityEngine;
using GameCube.FZeroGX.COLI_COURSE;

namespace Manifold.IO.GFZX01
{
    [CreateAssetMenu(menuName = "Manifold/Import/" + "NEW COLI Importer")]
    public class ColiImporter : ExecutableScriptableObject,
        IImportable
    {
        [Header("Import Settings")]
        [SerializeField]
        protected SearchOption fileSearchOption = SearchOption.AllDirectories;

        [SerializeField]
        protected string searchPattern = "COLI_COURSE*";

        [SerializeField, BrowseFolderField("Assets/")]
        protected string importPath;

        [SerializeField, BrowseFolderField("Assets/")]
        protected string importDestination;

        [Header("Import Files")]
        [SerializeField]
        protected string[] importFiles;

        public override string ExecuteText => "Import COLI";

        public override void Execute() => Import();

        public void Import()
        {
            importFiles = Directory.GetFiles(importPath, searchPattern, fileSearchOption);
            importFiles = UnityPathUtility.EnforceUnitySeparators(importFiles);
            var importFilesUncompressed = GFZX01Utility.DecompressEachLZ(importFiles);
            ImportUtility.ImportManyAs<ColiSceneSobj>(importFilesUncompressed, importPath, importDestination);
        }
    }
}