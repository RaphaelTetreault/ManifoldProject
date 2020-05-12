using GameCube.FZeroGX.CarData;
using Manifold.IO;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Data;

[CreateAssetMenu(menuName = "Manifold/Export/" + "CarData Exporter")]
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
    protected bool preserveFolderStructure = true;
    
    [SerializeField]
    protected bool exportCompressed = false;

    [SerializeField]
    protected CarDataSobj[] exportSobjs;


    public override string ExecuteText => "Export CarData";

    public override void Execute() => Export();

    public void Export()
    {
        var exportSobjs = this.exportSobjs;
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
        var exportedFiles = ExportUtility.ExportFiles(exportSobjs, exportTo, "", preserveFolderStructure);

        if (exportCompressed)
        {
            var compressedFiles = GFZX01Utility.CompressEachAsLZ(exportedFiles);
        }
    }
}
