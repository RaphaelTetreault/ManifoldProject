using StarkTools.IO;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu]
public class GMAAnalyzer : AnalyzerSobj<GMASobj>
{
    const string ExistsStr = "X";
    const string NullStr = "null";

    [SerializeField]
    protected string gcmfFile;
    [SerializeField]
    protected string textureFile;
    [SerializeField]
    protected string materialFile;
    [SerializeField]
    protected string texUnkFlagsFile;
    [SerializeField]
    protected bool printAllNames;

    public override string ProcessMessage => string.Empty;

    public override string TypeName => string.Empty;

    public override void Analyze()
    {
        if (loadAllOfType)
            analysisSobjs = LoadAllOfType();

        var numProcesses = 4f;
        var processIndex = 0;
        var time = DateTime.Now.ToString("(yyyy-MM-dd)(HH-mm-ss-FFFFFF)");
        var gcfmAnalysisFile = Path.Combine(destinationDirectory, $"{gcmfFile}_{time}.tsv");
        var texAnalysisFile = Path.Combine(destinationDirectory, $"{textureFile}_{time}.tsv");
        var matAnalysisFile = Path.Combine(destinationDirectory, $"{materialFile}_{time}.tsv");
        var texFlagAnalysisFile = Path.Combine(destinationDirectory, $"{texUnkFlagsFile}_{time}.tsv");

        // EX
        EditorUtility.DisplayProgressBar(ButtonText, texFlagAnalysisFile, processIndex++ / numProcesses);
        WriteGcmfTexUnkFlagsAnalysis(texFlagAnalysisFile);

        // GCMF
        EditorUtility.DisplayProgressBar(ButtonText, gcfmAnalysisFile, processIndex++ / numProcesses);
        WriteGcmfAnalysis(gcfmAnalysisFile);

        // GCMF TEX
        EditorUtility.DisplayProgressBar(ButtonText, texAnalysisFile, processIndex++ / numProcesses);
        WriteTexAnalysis(texAnalysisFile);

        // GCMF MAT
        EditorUtility.DisplayProgressBar(ButtonText, matAnalysisFile, processIndex++ / numProcesses);
        WriteMatAnalysis(matAnalysisFile);

        // END
        EditorUtility.ClearProgressBar();
    }

    public StreamWriter OpenWriter(string fileName)
    {
        var writeFile = Path.Combine(destinationDirectory, fileName);
        var fileStream = File.Open(writeFile, write.mode, write.access, write.share);
        var writer = new StreamWriter(fileStream);
        return writer;
    }

    public void WriteGcmfTexUnkFlagsAnalysis(string fileName)
    {
        var size = 8;
        var unkFlags0x00 = new int[size];
        var unkFlags0x02 = new int[size];
        var unkFlags0x03 = new int[size];
        var unkFlags0x0C = new int[size];
        var unkFlags0x10 = new int[size];

        foreach (var sobj in analysisSobjs)
        {
            foreach (var gcmf in sobj.Value.GCMF)
            {
                if (gcmf == null)
                    continue;

                foreach (var tex in gcmf.Texture)
                {
                    for (int i = 0; i < size; i++)
                    {
                        var isFlagEnabled = ((int)tex.Unk_0x00 >> i) & 1;
                        unkFlags0x00[i] += isFlagEnabled;

                        isFlagEnabled = ((int)tex.MipmapSettings >> i) & 1;
                        unkFlags0x02[i] += isFlagEnabled;

                        isFlagEnabled = ((int)tex.Wrapflags >> i) & 1;
                        unkFlags0x03[i] += isFlagEnabled;

                        isFlagEnabled = ((int)tex.Unk_0x0C >> i) & 1;
                        unkFlags0x0C[i] += isFlagEnabled;

                        isFlagEnabled = ((int)tex.Unk_0x10 >> i) & 1;
                        unkFlags0x10[i] += isFlagEnabled;
                    }
                }
            }
        }

        var writer = OpenWriter(fileName);

        writer.PushCol();
        for (int i = 0; i < size; i++)
            writer.PushCol($"Flag Bit {i}");
        writer.PushRow();

        writer.PushCol("UnkFlags0x00");
        for (int i = 0; i < unkFlags0x00.Length; i++)
            writer.PushCol(unkFlags0x00[i]);
        writer.PushRow();

        writer.PushCol("UnkFlags0x02");
        for (int i = 0; i < unkFlags0x02.Length; i++)
            writer.PushCol(unkFlags0x02[i]);
        writer.PushRow();

        writer.PushCol("UnkFlags0x03");
        for (int i = 0; i < unkFlags0x03.Length; i++)
            writer.PushCol(unkFlags0x03[i]);
        writer.PushRow();

        writer.PushCol("UnkFlags0x0C");
        for (int i = 0; i < unkFlags0x0C.Length; i++)
            writer.PushCol(unkFlags0x0C[i]);
        writer.PushRow();

        writer.PushCol("UnkFlags0x10");
        for (int i = 0; i < unkFlags0x10.Length; i++)
            writer.PushCol(unkFlags0x10[i]);
        writer.PushRow();

        writer.Close();
    }

    public void WriteGcmfAnalysis(string fileName)
    {
        var writer = OpenWriter(fileName);
        // Write header
        if (writer.BaseStream.Length <= 0)
        {
            writer.PushCol("File Name");
            writer.PushCol("GCMF File Name");
            writer.PushCol("GCMF Index");
            writer.PushCol("Address");
            writer.PushCol("Attributes");
            writer.PushCol("Origin");
            writer.PushCol("Radius");
            writer.PushCol("Texturecount");
            writer.PushCol("Materialcount");
            writer.PushCol("Translucidmaterialcount");
            writer.PushCol("Transform Matrix Count");
            writer.PushCol("Zero_0x1F");
            writer.PushCol("Indices Rel Ptr");
            writer.PushCol("Zero_0x24");
            //writer.PushCol("Transform Matrix Default Indices");
            writer.PushCol("Texture");
            writer.PushCol("TransformMatrix");
            writer.PushCol("Material");
            writer.PushRow();
        }

        foreach (var sobj in analysisSobjs)
        {
            // Write contents
            var index = 0;
            var maxIndex = sobj.value.GcmfCount;
            var isFirstEntry = true;
            foreach (var gcmf in sobj.Value.GCMF)
            {
                if (isFirstEntry || printAllNames)
                {
                    writer.PushCol(sobj.FileName);
                    isFirstEntry = false;
                }
                else writer.PushCol();

                if (string.IsNullOrEmpty(gcmf.ModelName))
                {
                    writer.PushRow(NullStr);
                    continue;
                }
                else
                {
                    writer.PushCol(gcmf.ModelName);
                }

                writer.PushCol($"[{index}/{maxIndex}]");
                writer.PushCol("0x" + gcmf.StartAddress.ToString("X8"));
                writer.PushCol(gcmf.Attributes);
                writer.PushCol(gcmf.Origin);
                writer.PushCol(gcmf.Radius);
                writer.PushCol(gcmf.TextureCount);
                writer.PushCol(gcmf.MaterialCount);
                writer.PushCol(gcmf.TranslucidMaterialCount);
                writer.PushCol(gcmf.Transformmatrixcount);
                writer.PushCol(gcmf.Zero_0x1F);
                writer.PushCol(gcmf.Indicesrelptr);
                writer.PushCol(gcmf.Zero_0x24);
                //writer.PushCol(gcmf.Transformmatrixdefaultindices == null ? NullStr : ExistsStr);
                writer.PushCol(gcmf.Texture == null ? NullStr : ExistsStr);
                writer.PushCol(gcmf.TransformMatrix == null ? NullStr : ExistsStr);
                writer.PushCol(gcmf.Material == null ? NullStr : ExistsStr);
                writer.PushRow();

                index++;
            }
        }
        writer.Close();
    }

    public void WriteTexAnalysis(string fileName)
    {
        var writer = OpenWriter(fileName);

        if (writer.BaseStream.Length <= 0)
        {
            writer.PushCol("File Name");
            writer.PushCol("Model Index");
            writer.PushCol("Model Name");
            writer.PushCol("Texture Index");
            writer.PushCol("Address");
            writer.PushCol("Unk_0x00");
            writer.PushCol("Mipmap");
            writer.PushCol("Wrapflags");
            writer.PushCol("Tpltextureid");
            writer.PushCol("Unk_0x06");
            writer.PushCol("Anisotropiclevel");
            writer.PushCol("Unk_0x08");
            writer.PushCol("Unk_Flags_0X0C");
            writer.PushCol("Unk_0x0D");
            writer.PushCol("Index");
            writer.PushCol("Unk_0x10");
            writer.PushRow();
        }

        foreach (var sobj in analysisSobjs)
        {
            // Write contents
            var modelIndex = 0;
            var modelIndexMax = sobj.value.GcmfCount;
            var isFirstGCMF = true;
            foreach (var gcmf in sobj.Value.GCMF)
            {
                if (gcmf == null)
                {
                    writer.PushRow(NullStr);
                    continue;
                }

                var texIndex = 1; // debugger is not zero-indexed
                var texIndexMax = gcmf.TextureCount;
                var isFirstTex = true;
                foreach (var tex in gcmf.Texture)
                {
                    if (isFirstGCMF || printAllNames)
                    {
                        writer.PushCol(sobj.FileName);
                        isFirstGCMF = false;
                    }
                    else writer.PushCol();


                    if (isFirstTex || printAllNames)
                    {
                        writer.PushCol($"[{modelIndex}/{modelIndexMax}]");
                        writer.PushCol(gcmf.ModelName);
                        isFirstTex = false;
                    }
                    else
                    {
                        writer.PushCol();
                        writer.PushCol();
                    }

                    writer.PushCol($"[{texIndex}/{texIndexMax}]");
                    writer.PushCol("0x" + tex.StartAddress.ToString("X8"));
                    writer.PushCol(tex.Unk_0x00);
                    writer.PushCol(tex.MipmapSettings);
                    writer.PushCol(tex.Wrapflags);
                    writer.PushCol(tex.Tpltextureid);
                    writer.PushCol(tex.Unk_0x06);
                    writer.PushCol(tex.Anisotropiclevel);
                    writer.PushCol(tex.Zero_0x08);
                    writer.PushCol(tex.Unk_0x0C);
                    writer.PushCol(tex.IsSwappableTexture);
                    writer.PushCol(tex.Index);
                    writer.PushCol(tex.Unk_0x10);
                    writer.PushRow();

                    texIndex++;
                }
                modelIndex++;
            }
        }

        writer.Close();
    }

    public void WriteMatAnalysis(string fileName)
    {
        var writer = OpenWriter(fileName);

        if (writer.BaseStream.Length <= 0)
        {
            writer.PushCol("File Name");
            writer.PushCol("Model Index");
            writer.PushCol("Model Name");
            writer.PushCol("Address");
            writer.PushCol("Unk_0x00");
            writer.PushCol("Unk_0x04");
            writer.PushCol("Unk_0x08");
            writer.PushCol("Unk_0x0C");
            writer.PushCol("Unk_0x10");
            writer.PushCol("Unk_0x11");
            writer.PushCol("Unk_0x12");
            writer.PushCol("Vertex render flags");
            writer.PushCol("Unk_0x14");
            writer.PushCol("Unk_0x15");
            writer.PushCol("Tex0Index");
            writer.PushCol("Tex1Index");
            writer.PushCol("Tex2Index");
            writer.PushCol("Vertex descriptor flags");
            //writer.PushCol("Transform matrix specific indices");
            writer.PushCol("Mat display list size");
            writer.PushCol("Tl mat display list size");
            writer.PushCol("Unk_0x30");
            writer.PushCol("Unk_0x3C");
            writer.PushCol("Unk_0x40");
            writer.PushCol("Unk_0x44");
            writer.PushCol("Unk_0x48");
            writer.PushCol("Unk_0x4C");
            writer.PushCol("Unk_0x50");
            writer.PushCol("Unk_0x54");
            writer.PushCol("Unk_0x58");
            writer.PushRow();
        }

        foreach (var sobj in analysisSobjs)
        {
            // Write contents
            var index = 0;
            var maxIndex = sobj.value.GcmfCount;
            var isFirstEntry = true;
            foreach (var gcmf in sobj.Value.GCMF)
            {
                // skip null models
                if (string.IsNullOrEmpty(gcmf.ModelName))
                    continue;

                if (isFirstEntry || printAllNames)
                {
                    writer.PushCol(sobj.FileName);
                    isFirstEntry = false;
                }
                else writer.PushCol();

                writer.PushCol($"[{index}/{maxIndex}]");
                writer.PushCol(gcmf.ModelName);
                writer.PushCol("0x" + gcmf.Material.StartAddress.ToString("X8"));
                writer.PushCol(gcmf.Material.Unk_Flags_0x00);
                writer.PushCol(gcmf.Material.Unk_0x04);
                writer.PushCol(gcmf.Material.Unk_0x08);
                writer.PushCol(gcmf.Material.Unk_0x0C);
                writer.PushCol(gcmf.Material.Unk_0x10);
                writer.PushCol(gcmf.Material.Unk_0x11);
                writer.PushCol(gcmf.Material.Unk_0x12);
                writer.PushCol(gcmf.Material.Vertexrenderflags);
                writer.PushCol(gcmf.Material.Unk_0x14);
                writer.PushCol(gcmf.Material.Unk_0x15);
                writer.PushCol(gcmf.Material.Tex0Index);
                writer.PushCol(gcmf.Material.Tex1Index);
                writer.PushCol(gcmf.Material.Tex2Index);
                writer.PushCol(gcmf.Material.Vertexdescriptorflags);
                //writer.PushCol(gcmf.Material.TransformMatrixSpecificIndices);
                writer.PushCol(gcmf.Material.Matdisplaylistsize);
                writer.PushCol(gcmf.Material.Tlmatdisplaylistsize);
                writer.PushCol(gcmf.Material.Unk_0x30);
                writer.PushCol(gcmf.Material.Unk_0x3C);
                writer.PushCol(gcmf.Material.Unk_0x40);
                writer.PushCol(gcmf.Material.Unk_0x44);
                writer.PushCol(gcmf.Material.Unk_0x48);
                writer.PushCol(gcmf.Material.Unk_0x4C);
                writer.PushCol(gcmf.Material.Unk_0x50);
                writer.PushCol(gcmf.Material.Unk_0x54);
                writer.PushCol(gcmf.Material.Unk_0x58);
                writer.PushRow();

                index++;
            }
        }
        writer.Close();
    }


}
