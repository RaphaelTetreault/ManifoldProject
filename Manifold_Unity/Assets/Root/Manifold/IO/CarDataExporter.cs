using GameCube.FZeroGX.CarData;
using Manifold.IO;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Manifold/Export/" + "NEW CarData Exporter")]
public class CarDataExporter : ExecutableScriptableObject,
    IExportable
{
    [Header("Export Settings")]
    [SerializeField]
    protected ExportUtility.ExportOptions exportOptions
        = ExportUtility.ExportOptions.ExportFiles;

    [SerializeField, BrowseFolderField, Tooltip("Used for ExportOptions.ExportAllOfTypeInFolder")]
    protected string exportFrom = string.Empty;

    [SerializeField, BrowseFolderField]
    protected string exportTo;

    [SerializeField]
    protected bool allowOverwritingFiles = true;

    [SerializeField]
    protected bool preserveFolderStructure = true;

    [SerializeField]
    protected bool exportCompressed = false;

    [Header("Preferences")]
    [SerializeField]
    protected bool openFolderAfterExport = true;

    [Header("Exports")]
    [SerializeField]
    protected CarDataSobj[] exportSobjs;



    public override string ExecuteText => "Export CarData";

    public override void Execute() => Export();

    public void Export()
    {
        switch (exportOptions)
        {
            case ExportUtility.ExportOptions.ExportFiles:
                break;

            case ExportUtility.ExportOptions.ExportAllOfType:
                exportSobjs = AssetDatabaseUtility.GetAllOfType<CarDataSobj>();
                break;
            case ExportUtility.ExportOptions.ExportAllOfTypeInFolder:
                exportSobjs = AssetDatabaseUtility.GetAllOfType<CarDataSobj>(exportFrom);
                break;

            default:
                throw new NotImplementedException();
        }
        var exportedFiles = ExportUtility.ExportFiles(exportSobjs, exportTo, "", allowOverwritingFiles, preserveFolderStructure);
        ExportUtility.PrintExportsToConsole(this, exportedFiles);
        if (openFolderAfterExport)
        {
            ExportUtility.OpenFileFolder(exportedFiles);
        }

        if (exportCompressed)
        {
            var compressedFiles = GFZX01Utility.CompressEachAsLZ(exportedFiles, allowOverwritingFiles);
            ExportUtility.PrintExportsToConsole(this, compressedFiles);
        }
    }

}
