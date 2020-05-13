using Manifold.IO;
using Manifold.IO.GFZX01;
using System;
using System.IO;
using UnityEngine;

[CreateAssetMenu(menuName = "Manifold/Import/" + "livecam_stage Importer")]
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
        var importFilesUncompressed = GFZX01Utility.DecompressEachLZ(importFiles);
        ImportUtility.ImportManyAs<LiveCameraStageSobj>(importFilesUncompressed, importPath, importDestination);
    }
}
