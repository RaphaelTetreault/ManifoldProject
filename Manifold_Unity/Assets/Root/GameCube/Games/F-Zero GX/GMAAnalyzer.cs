using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


[CreateAssetMenu]
public class GMAAnalyzer : NewTextAnalyzerSobj<GMASobj>
{
    [Header("Export Settings")]
    [SerializeField]
    protected string gcmfFile;
    [SerializeField]
    protected string textureFile;
    [SerializeField]
    protected string materialFile;

    public override string ProcessMessage => string.Empty;

    public override string TypeName => typeof(GMASobj).Name;

    public override void CreateAnalysis(GMASobj value)
    {
        var gcfmAnalysisFile = Path.Combine(destinationDirectory, $"{gcmfFile}.tsv");
        var texAnalysisFile = Path.Combine(destinationDirectory, $"{textureFile}.tsv");
        var matAnalysisFile = Path.Combine(destinationDirectory, $"{materialFile}.tsv");

        CreateAnalysis(value, gcfmAnalysisFile, WriteGcmfAnalysis);
        CreateAnalysis(value, texAnalysisFile, WriteTexAnalysis);
        CreateAnalysis(value, matAnalysisFile, WriteMatAnalysis);
    }

    public void WriteGcmfAnalysis(GMASobj sobj, StreamWriter writer)
    {
        writer.GoToEnd();

        // Write header
        if (writer.BaseStream.Length <= 0)
        {
            writer.PushCol("File Name");
            writer.PushCol("GCMF File Name");
            writer.PushCol("Unk0x04");
            writer.PushCol("unk0x04 bits");
            writer.PushCol("Origin");
            writer.PushCol("Radius");
            writer.PushCol("Texturecount");
            writer.PushCol("Materialcount");
            writer.PushCol("Translucidmaterialcount");
            writer.PushCol("Transformmatrixcount");
            writer.PushCol("Const0X00");
            writer.PushCol("Indicesrelptr");
            writer.PushCol("Unk0X24");
            writer.PushCol("Transformmatrixdefaultindices");
            writer.PushCol("Unk0X30");
            writer.PushCol("Unk0X34");
            writer.PushCol("Unk0X38");
            writer.PushCol("Unk0X3C");
            writer.PushCol("Texture");
            writer.PushCol("TransformMatrix");
            writer.PushCol("Material");
            writer.PushRow();
        }

        // Write contents
        var isFirstEntry = true;
        foreach (var gcmf in sobj.Value.GCMF)
        {
            if (isFirstEntry)
            {
                writer.PushCol(sobj.FileName);
                isFirstEntry = false;
            }
            else writer.PushCol();

            if (gcmf == null)
            {
                writer.PushRow("null");
                continue;
            }

            writer.PushCol(gcmf.FileName);
            writer.PushCol(gcmf.Unk0x04);
            writer.PushCol(Convert.ToString(gcmf.Unk0x04, 2).PadLeft(32, '0'));
            writer.PushCol(gcmf.Origin);
            writer.PushCol(gcmf.Radius);
            writer.PushCol(gcmf.Texturecount);
            writer.PushCol(gcmf.Materialcount);
            writer.PushCol(gcmf.Translucidmaterialcount);
            writer.PushCol(gcmf.Transformmatrixcount);
            writer.PushCol(gcmf.Const0X00);
            writer.PushCol(gcmf.Indicesrelptr);
            writer.PushCol(gcmf.Unk0X24);
            writer.PushCol(gcmf.Transformmatrixdefaultindices == null ? "null" : "exists");
            writer.PushCol(gcmf.Unk0X30);
            writer.PushCol(gcmf.Unk0X34);
            writer.PushCol(gcmf.Unk0X38);
            writer.PushCol(gcmf.Unk0X3C);
            writer.PushCol(gcmf.Texture == null ? "null" : "exists");
            writer.PushCol(gcmf.TransformMatrix == null ? "null" : "exists");
            writer.PushCol(gcmf.Material == null ? "null" : "exists");
            writer.PushRow();
        }
    }

    public void WriteTexAnalysis(GMASobj sobj, StreamWriter writer)
    {
        writer.GoToEnd();

        if (writer.BaseStream.Length <= 0)
        {
            writer.PushCol("File Name");
            writer.PushCol("GCMF File Name");
            writer.PushCol("Unkflag");
            writer.PushCol("Uvflags");
            writer.PushCol("Wrapflags");
            writer.PushCol("Tpltextureid");
            writer.PushCol("Unk_0X06");
            writer.PushCol("Anisotropiclevel");
            writer.PushCol("Unk_0X08");
            writer.PushCol("Unk_Flags_0X0C");
            writer.PushCol("Unk_0X0D");
            writer.PushCol("Index");
            writer.PushCol("Unk_0X10");
            for (int i = 0; i < Texture.kFifoPaddingSize; i++)
                writer.PushCol($"FifoPadding {i}");
            writer.PushRow();
        }

        // Write contents
        var isFirstEntry = true;
        foreach (var gcmf in sobj.Value.GCMF)
        {
            if (gcmf == null)
                continue;

            var isFirstTex = true;
            foreach (var tex in gcmf.Texture)
            {
                if (isFirstEntry)
                {
                    writer.PushCol(sobj.FileName);
                    isFirstEntry = false;
                }
                else writer.PushCol();

                if (isFirstTex)
                {
                    writer.PushCol(gcmf.FileName);
                    isFirstTex = false;
                }
                else writer.PushCol();

                //writer.PushCol(tex.Unkflag);
                writer.PushCol(Convert.ToString(tex.Unkflag, 2).PadLeft(16, '0'));
                writer.PushCol(tex.Uvflags);
                writer.PushCol(tex.Wrapflags);
                writer.PushCol(tex.Tpltextureid);
                writer.PushCol(tex.Unk_0X06);
                writer.PushCol(tex.Anisotropiclevel);
                writer.PushCol(tex.Unk_0X08);
                writer.PushCol(tex.Unk_Flags_0X0C);
                writer.PushCol(tex.Unk_0X0D);
                writer.PushCol(tex.Index);
                writer.PushCol(tex.Unk_0X10);
                foreach (var @byte in tex.Fifopadding)
                    writer.PushCol(@byte);
                writer.PushRow();
            }
        }
    }

    public void WriteMatAnalysis(GMASobj sobj, StreamWriter writer)
    {
        writer.GoToEnd();

        if (writer.BaseStream.Length <= 0)
        {
            writer.PushCol("File Name");
            writer.PushCol("Unk_Flags_0X00");
            writer.PushCol("Unk_0X04");
            writer.PushCol("Unk_0X08");
            writer.PushCol("Unk_0X0C");
            writer.PushCol("Unk_0X10");
            writer.PushCol("Unk_Count");
            writer.PushCol("Unk_0X12");
            writer.PushCol("Vertexrenderflags");
            writer.PushCol("Unk_0X14");
            writer.PushCol("Tex0Index");
            writer.PushCol("Tex1Index");
            writer.PushCol("Tex2Index");
            writer.PushCol("Vertexdescriptorflags");
            writer.PushCol("Transformmatrixspecidicindices");
            writer.PushCol("Matdisplaylistsize");
            writer.PushCol("Tlmatdisplaylistsize");
            writer.PushCol("Uvwcoordinates");
            writer.PushCol("Unk_0X3C");
            writer.PushCol("Unk_0X40");
            writer.PushCol("Unk_0X44");
            for (int i = 0; i < Material.kFifoPaddingSize; i++)
                writer.PushCol($"FifoPadding {i}");
            writer.PushRow();
        }

        // Write contents
        var isFirstEntry = true;
        foreach (var gcmf in sobj.Value.GCMF)
        {
            if (isFirstEntry)
            {
                writer.PushCol(sobj.FileName);
                isFirstEntry = false;
            }
            else writer.PushCol();

            if (gcmf == null)
            {
                writer.PushRow("null");
                continue;
            }

            writer.PushCol(gcmf.Material.Unk_Flags_0X00);
            writer.PushCol(gcmf.Material.Unk_0X04);
            writer.PushCol(gcmf.Material.Unk_0X08);
            writer.PushCol(gcmf.Material.Unk_0X0C);
            writer.PushCol(gcmf.Material.Unk_0X10);
            writer.PushCol(gcmf.Material.Unk_Count);
            writer.PushCol(gcmf.Material.Unk_0X12);
            writer.PushCol(gcmf.Material.Vertexrenderflags);
            writer.PushCol(gcmf.Material.Unk_0X14);
            writer.PushCol(gcmf.Material.Tex0Index);
            writer.PushCol(gcmf.Material.Tex1Index);
            writer.PushCol(gcmf.Material.Tex2Index);
            writer.PushCol(gcmf.Material.Vertexdescriptorflags);
            writer.PushCol(gcmf.Material.Transformmatrixspecidicindices);
            writer.PushCol(gcmf.Material.Matdisplaylistsize);
            writer.PushCol(gcmf.Material.Tlmatdisplaylistsize);
            writer.PushCol(gcmf.Material.Uvwcoordinates);
            writer.PushCol(gcmf.Material.Unk_0X3C);
            writer.PushCol(gcmf.Material.Unk_0X40);
            writer.PushCol(gcmf.Material.Unk_0X44);
            foreach (var @byte in gcmf.Material.Fifopadding)
                writer.PushCol(@byte);
            writer.PushRow();
        }
    }
}
