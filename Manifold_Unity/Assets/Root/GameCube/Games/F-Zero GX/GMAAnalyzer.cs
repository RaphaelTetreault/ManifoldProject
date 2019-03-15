using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[CreateAssetMenu]
public class GMAAnalyzer : TextAnalyzerSobj<GMA>
{
    [Header("Export Settings")]
    [SerializeField]
    [BrowseFolderField]
    protected string destinationDirectory;

    [SerializeField]
    protected string
        gcmfFile,
        textureFile,
        materialFile;

    public override string ProcessMessage => string.Empty;

    public override string TypeName => typeof(GMA).Name;

    public override void CreateAnalysis(GMA value)
    {
        var gcfmAnalysisFile = Path.Combine(destinationDirectory, $"{gcmfFile}.tsv");
        var texAnalysisFile = Path.Combine(destinationDirectory, $"{textureFile}.tsv");
        var matAnalysisFile = Path.Combine(destinationDirectory, $"{materialFile}.tsv");

        CreateAnalysis(value, gcfmAnalysisFile, WriteGcmfAnalysis);
        CreateAnalysis(value, texAnalysisFile, WriteTexAnalysis);
        CreateAnalysis(value, matAnalysisFile, WriteMatAnalysis);
    }

    public void WriteGcmfAnalysis(GMA value, StreamWriter writer)
    {
        try
        {
            writer.GoToEnd();


            if (writer.BaseStream.Length <= 0)
            {
                writer.PushCol("File Name");
                writer.PushCol("GCMF File Name");
                writer.PushCol("unk0x04");
                writer.PushCol("unk0x04 bits");
                writer.PushRow();
            }

            foreach (var gcmf in value.GCMF)
            {
                writer.PushCol(value.FileName);

                if (gcmf == null)
                {
                    writer.PushRow("null");
                    continue;
                }

                writer.PushCol(gcmf.FileName);
                writer.PushCol(gcmf.Unk0x04);
                writer.PushCol(Convert.ToString(gcmf.Unk0x04, 2).PadLeft(32, '0'));
                writer.PushRow();
            }
        }
        catch (Exception e)
        {
            writer.PushRow();
            writer.PushRow($"Error Type: {e.GetType().Name}, Error Msg: {e.Message}");
        }
    }

    public void WriteTexAnalysis(GMA value, StreamWriter writer)
    {
        writer.GoToEnd();

        if (writer.BaseStream.Length <= 0)
        {
            writer.PushCol("File Name");
            writer.PushRow();
        }

        writer.PushCol(value.FileName);
        writer.PushRow();
    }

    public void WriteMatAnalysis(GMA value, StreamWriter writer)
    {
        writer.GoToEnd();

        if (writer.BaseStream.Length <= 0)
        {
            writer.PushCol("File Name");
            writer.PushRow();
        }

        writer.PushCol(value.FileName);
        writer.PushRow();
    }
}
