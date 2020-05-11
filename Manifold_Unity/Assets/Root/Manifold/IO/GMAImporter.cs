using Boo.Lang;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEditor;
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
            const string compressedExt = ".lz";
            var importFiles = new List<string>(Directory.GetFiles(importPath, searchPattern, fileSearchOption));

            for (int i = 0; i < importFiles.Count; i++)
            //foreach (var importFile in importFiles)
            {
                var importFile = importFiles[i];

                var requireDecompress = importFile.EndsWith(compressedExt);
                var importFileDecompressed = importFile.Remove(importFile.Length - compressedExt.Length);
                var fileNotAlreadDecompressed = !File.Exists(importFileDecompressed);

                if (requireDecompress && fileNotAlreadDecompressed)
                {
                    string outputFilePath = string.Empty;
                    // Need to fix AvGame params to make sense...
                    // Save the decompressed file so next time we run this there is no decompression going on
                    ImportUtility.DecompressAv(importFile, LibGxFormat.AvGame.FZeroGX, true, out outputFilePath);
                    // save reference to newly output file
                    importFiles[i] = outputFilePath;
                }
            }

            foreach (var importFile in importFiles)
            {
                var isLz = importFile.EndsWith(compressedExt);
                if (isLz)
                {
                    importFiles.Remove(importFile);
                }
            }

            var finalList = importFiles.ToArray();
            ImportUtility.ImportFilesAs<GMASobj>(finalList, importPath, importDestination);

            // Save possible changes to variable 'importFiles'
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }


        public static string[] GetDecompressedFilesOnly(string[] importFiles, string compressedExt)
        {
            var importFilesList = new List<string>(importFiles);

            for (int i = 0; i < importFilesList.Count; i++)
            //foreach (var importFile in importFiles)
            {
                var importFile = importFilesList[i];

                var requireDecompress = importFile.EndsWith(compressedExt);
                var importFileDecompressed = importFile.Remove(importFile.Length - compressedExt.Length);
                var fileNotAlreadDecompressed = !File.Exists(importFileDecompressed);

                if (requireDecompress && fileNotAlreadDecompressed)
                {
                    string outputFilePath = string.Empty;
                    // Need to fix AvGame params to make sense...
                    // Save the decompressed file so next time we run this there is no decompression going on
                    ImportUtility.DecompressAv(importFile, LibGxFormat.AvGame.FZeroGX, true, out outputFilePath);
                    // save reference to newly output file
                    importFilesList[i] = outputFilePath;
                }
            }

            foreach (var importFile in importFilesList)
            {
                var isLz = importFile.EndsWith(compressedExt);
                if (isLz)
                {
                    importFilesList.Remove(importFile);
                }
            }

            return importFilesList.ToArray();
        }
    }
}