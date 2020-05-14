﻿using Manifold.IO;
using System.IO;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Manifold/Import/" + "livecam_stage EX Importer")]
public class LiveCameraStageExImporter : ExecutableScriptableObject,
    IImportable
{
    [Header("Import Settings")]
    [SerializeField]
    protected SearchOption fileSearchOption = SearchOption.AllDirectories;

    [SerializeField]
    protected string searchPattern = "livecam_stage*.bin";

    [SerializeField, BrowseFolderField("Assets/")]
    protected string importFrom;

    [SerializeField, BrowseFolderField("Assets/")]
    protected string importTo;

    [Header("Import Files")]
    [SerializeField]
    protected string[] importFiles;

    public override string ExecuteText => "Import livecam_stage";

    public override void Execute() => Import();

    public void Import()
    {
        importFiles = Directory.GetFiles(importFrom, searchPattern, fileSearchOption);
        importFiles = UnityPathUtility.EnforceUnitySeparators(importFiles);

        foreach (var importFile in importFiles)
        {
            var fileName = Path.GetFileNameWithoutExtension(importFile);
            var folderName = fileName;

            ImportUtility.EnsureAssetFolderExists(importTo, folderName);
            var importDest = $"{importTo}/{folderName}";

            var container = ImportUtility.Create<LiveCameraStageExSobj>(importDest, fileName);
            container.FileName = Path.GetFileName(importFile);

            using (var file = File.OpenRead(importFile))
            {
                using (var reader = new BinaryReader(file))
                {
                    var cam = new LiveCameraStage();
                    cam.Deserialize(reader);

                    // Copy pan data over to new assets
                    var pans = new CameraPanSobj[cam.Pans.Length];
                    for (int i = 0; i < pans.Length; i++)
                    {
                        var incFileName = $"{fileName}.{i+1}";
                        ImportUtility.ImportProgBar<CameraPanSobj>(i, pans.Length, incFileName);

                        pans[i] = ImportUtility.Create<CameraPanSobj>(importDest, incFileName);
                        AssetDatabase.Refresh();
                        AssetDatabase.SaveAssets();
                        pans[i].value = cam.Pans[i];
                    }
                    // Link new assets to container
                    container.SetCameraPans(pans);
                }
            }
        }
        EditorUtility.ClearProgressBar();
    }
}
