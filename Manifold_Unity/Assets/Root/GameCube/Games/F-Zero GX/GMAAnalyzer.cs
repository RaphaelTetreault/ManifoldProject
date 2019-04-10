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
    protected string materialAnimFile;
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

        var numProcesses = 5f;
        var processIndex = 0;
        var time = DateTime.Now.ToString("(yyyy-MM-dd)-(HH-mm-ss)");
        var gcfmAnalysisFile = Path.Combine(destinationDirectory, $"{gcmfFile}_{time}.tsv");
        var texAnalysisFile = Path.Combine(destinationDirectory, $"{textureFile}_{time}.tsv");
        var matAnalysisFile = Path.Combine(destinationDirectory, $"{materialFile}_{time}.tsv");
        var matAnimAnalysisFile = Path.Combine(destinationDirectory, $"{materialAnimFile}_{time}.tsv");
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

        // GCMF MAT
        EditorUtility.DisplayProgressBar(ButtonText, matAnimAnalysisFile, processIndex++ / numProcesses);
        WriteMatAnimAnalysis(matAnimAnalysisFile);

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
                if (gcmf == null || gcmf.ModelName == GCMF.kNullEntryName)
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
            writer.PushRow();
        }

        foreach (var sobj in analysisSobjs)
        {
            // Write contents
            var index = 0;
            var maxIndex = sobj.value.GcmfCount;
            foreach (var gcmf in sobj.Value.GCMF)
            {
                if (string.IsNullOrEmpty(gcmf.ModelName) ||
                    gcmf.ModelName == GCMF.kNullEntryName)
                    continue;

                writer.PushCol(sobj.FileName);
                writer.PushCol(gcmf.ModelName);
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
            writer.PushCol("Mipmap Settings");
            writer.PushCol("Wrap Flags");
            writer.PushCol("Tpl Texture Index");
            writer.PushCol("Unk_0x06");
            writer.PushCol("Anisotropic Level");
            writer.PushCol("Unk_0x0C");
            writer.PushCol("Is Swappable Texture?");
            writer.PushCol("Index");
            writer.PushCol("Unk_0x10");
            writer.PushRow();
        }

        foreach (var sobj in analysisSobjs)
        {
            // Write contents
            var modelIndex = 0;
            var modelIndexMax = sobj.value.GcmfCount;
            foreach (var gcmf in sobj.Value.GCMF)
            {
                var texIndex = 1; // debugger is not zero-indexed
                var texIndexMax = gcmf.TextureCount;
                foreach (var tex in gcmf.Texture)
                {
                    writer.PushCol(sobj.FileName);
                    writer.PushCol($"[{modelIndex}/{modelIndexMax}]");
                    writer.PushCol(gcmf.ModelName);
                    writer.PushCol($"[{texIndex}/{texIndexMax}]");
                    writer.PushCol("0x" + tex.StartAddress.ToString("X8"));
                    writer.PushCol(tex.Unk_0x00);
                    writer.PushCol(tex.MipmapSettings);
                    writer.PushCol(tex.Wrapflags);
                    writer.PushCol(tex.Tpltextureid);
                    writer.PushCol(tex.Unk_0x06);
                    writer.PushCol(tex.Anisotropiclevel);
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
            writer.PushCol("Color0");
            writer.PushCol("Color1");
            writer.PushCol("Color2");
            writer.PushCol("Unk_0x10");
            writer.PushCol("Unk_0x11");
            writer.PushCol("Unk_0x12");
            writer.PushCol("Vertex Render Flags");
            writer.PushCol("Unk_0x14");
            writer.PushCol("Unk_0x15");
            writer.PushCol("Tex 0 Index");
            writer.PushCol("Tex 1 Index");
            writer.PushCol("Tex 2 Index");
            writer.PushCol("Vertex Descriptor Flags");
            //writer.PushCol("Transform matrix specific indices");
            writer.PushCol("Mat display list size");
            writer.PushCol("Tl mat display list size");
            writer.PushCol("Bounding Sphere Origin");
            writer.PushCol("Unk_0x40");
            writer.PushRow();
        }

        foreach (var sobj in analysisSobjs)
        {
            // Write contents
            var index = 0;
            var maxIndex = sobj.value.GcmfCount;
            foreach (var gcmf in sobj.Value.GCMF)
            {
                // skip null models
                if (string.IsNullOrEmpty(gcmf.ModelName) ||
                    gcmf.ModelName == GCMF.kNullEntryName ||
                    string.IsNullOrEmpty(gcmf.Material.ModelName))
                    continue;

                writer.PushCol(sobj.FileName);
                writer.PushCol($"[{index}/{maxIndex}]");
                writer.PushCol(gcmf.ModelName);
                writer.PushCol("0x" + gcmf.Material.StartAddress.ToString("X8"));
                writer.PushCol(gcmf.Material.Unk_0x00);
                writer.PushCol(gcmf.Material.Color0);
                writer.PushCol(gcmf.Material.Color1);
                writer.PushCol(gcmf.Material.Color2);
                writer.PushCol(gcmf.Material.Unk_0x10);
                writer.PushCol(gcmf.Material.Unk_0x11);
                writer.PushCol(gcmf.Material.Unk_0x12);
                writer.PushCol(gcmf.Material.VertexRenderFlags);
                writer.PushCol(gcmf.Material.Unk_0x14);
                writer.PushCol(gcmf.Material.Unk_0x15);
                writer.PushCol(gcmf.Material.Tex0Index);
                writer.PushCol(gcmf.Material.Tex1Index);
                writer.PushCol(gcmf.Material.Tex2Index);
                writer.PushCol(gcmf.Material.VertexDescriptorFlags);
                //writer.PushCol(gcmf.Material.TransformMatrixSpecificIndices);
                writer.PushCol(gcmf.Material.MatDisplayListSize);
                writer.PushCol(gcmf.Material.TlMatDisplayListSize);
                writer.PushCol(gcmf.Material.BoudingSphereOrigin);
                writer.PushCol(gcmf.Material.Unk_0x40);
                writer.PushRow();

                index++;
            }
        }
        writer.Close();
    }

    public void WriteMatAnimAnalysis(string fileName)
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
            //writer.PushCol("< HEX");
            writer.PushCol("Unk_0x08");
            //writer.PushCol("< HEX");
            writer.PushCol("Unk_0x0C");
            //writer.PushCol("< HEX");
            writer.PushCol("Unk_0x10");
            writer.PushCol("Unk_0x11");
            writer.PushCol("Unk_0x12");
            writer.PushCol("Vertex Render Flags");
            writer.PushCol("Unk_0x14");
            writer.PushCol("Unk_0x15");
            writer.PushCol("Tex 0 Index");
            writer.PushCol("Tex 1 Index");
            writer.PushCol("Tex 2 Index");
            writer.PushCol("Vertex Descriptor Flags");
            //writer.PushCol("Transform matrix specific indices");
            writer.PushCol("Mat display list size");
            writer.PushCol("Tl mat display list size");
            writer.PushCol("Unk_0x30");
            writer.PushCol("Unk_0x3C");
            writer.PushCol("< HEX");
            writer.PushCol("< BIN");
            writer.PushCol("Unk_0x40");
            //writer.PushCol("< HEX");
            writer.PushCol("Unk_0x44");
            //writer.PushCol("< HEX");
            writer.PushCol("Unk_0x48");
            writer.PushCol("< HEX");
            writer.PushCol("< BIN");
            writer.PushCol("Unk_0x4C");
            writer.PushCol("< HEX");
            writer.PushCol("< BIN");
            writer.PushCol("Unk_0x50");
            //writer.PushCol("< HEX");
            writer.PushCol("Unk_0x54");
            //writer.PushCol("< HEX");
            writer.PushCol("Unk_0x58");
            //writer.PushCol("< HEX");
            writer.PushRow();
        }

        foreach (var sobj in analysisSobjs)
        {
            // Write contents
            var index = 0;
            var maxIndex = sobj.value.GcmfCount;
            foreach (var gcmf in sobj.Value.GCMF)
            {
                // skip null models
                if (string.IsNullOrEmpty(gcmf.ModelName) ||
                    gcmf.ModelName == GCMF.kNullEntryName ||
                    string.IsNullOrEmpty(gcmf.MaterialAnim.ModelName))
                    continue;

                writer.PushCol(sobj.FileName);
                writer.PushCol($"[{index}/{maxIndex}]");
                writer.PushCol(gcmf.ModelName);
                writer.PushCol("0x" + gcmf.MaterialAnim.StartAddress.ToString("X8"));
                writer.PushCol(gcmf.MaterialAnim.Unk_0x00);
                writer.PushCol("0x" + gcmf.MaterialAnim.Unk_0x04.ToString("X8"));
                //writer.PushCol(gcmf.MaterialAnim.Unk_0x08);
                writer.PushCol("0x" + gcmf.MaterialAnim.Unk_0x08.ToString("X8"));
                //writer.PushCol(gcmf.MaterialAnim.Unk_0x0C);
                writer.PushCol("0x" + gcmf.MaterialAnim.Unk_0x0C.ToString("X8"));
                writer.PushCol(gcmf.MaterialAnim.Unk_0x10);
                writer.PushCol(gcmf.MaterialAnim.Unk_0x11);
                writer.PushCol(gcmf.MaterialAnim.Unk_0x12);
                writer.PushCol(gcmf.MaterialAnim.VertexRenderFlags);
                writer.PushCol(gcmf.MaterialAnim.Unk_0x14);
                writer.PushCol(gcmf.MaterialAnim.Unk_0x15);
                writer.PushCol(gcmf.MaterialAnim.Tex0Index);
                writer.PushCol(gcmf.MaterialAnim.Tex1Index);
                writer.PushCol(gcmf.MaterialAnim.Tex2Index);
                writer.PushCol(gcmf.MaterialAnim.Vertexdescriptorflags);
                //writer.PushCol(gcmf.MaterialAnim.TransformMatrixSpecificIndices);
                writer.PushCol(gcmf.MaterialAnim.Matdisplaylistsize);
                writer.PushCol(gcmf.MaterialAnim.Tlmatdisplaylistsize);
                writer.PushCol(gcmf.MaterialAnim.Unk_0x30);
                writer.PushCol(gcmf.MaterialAnim.Unk_0x3C);
                writer.PushCol("0x" + gcmf.MaterialAnim.Unk_0x3C.ToString("X8"));
                writer.PushCol("0b_" + Convert.ToString(gcmf.MaterialAnim.Unk_0x3C, 2).PadLeft(32, '0'));
                //writer.PushCol(gcmf.MaterialAnim.Unk_0x40);
                writer.PushCol("0x" + gcmf.MaterialAnim.Unk_0x40.ToString("X8"));
                //writer.PushCol(gcmf.MaterialAnim.Unk_0x44);
                writer.PushCol("0x" + gcmf.MaterialAnim.Unk_0x44.ToString("X8"));
                writer.PushCol(gcmf.MaterialAnim.Unk_0x48);
                writer.PushCol("0x" + gcmf.MaterialAnim.Unk_0x48.ToString("X8"));
                writer.PushCol("0b_" + Convert.ToString(gcmf.MaterialAnim.Unk_0x48, 2).PadLeft(32, '0'));
                writer.PushCol(gcmf.MaterialAnim.Unk_0x4C);
                writer.PushCol("0x" + gcmf.MaterialAnim.Unk_0x4C.ToString("X8"));
                writer.PushCol("0b_" + Convert.ToString(gcmf.MaterialAnim.Unk_0x4C, 2).PadLeft(32, '0'));
                //writer.PushCol(gcmf.MaterialAnim.Unk_0x50);
                writer.PushCol("0x" + gcmf.MaterialAnim.Unk_0x50.ToString("X8"));
                //writer.PushCol(gcmf.MaterialAnim.Unk_0x54);
                writer.PushCol("0x" + gcmf.MaterialAnim.Unk_0x54.ToString("X8"));
                //writer.PushCol(gcmf.MaterialAnim.Unk_0x58);
                writer.PushCol("0x" + gcmf.MaterialAnim.Unk_0x58.ToString("X8"));
                writer.PushRow();

                index++;
            }
        }
        writer.Close();
    }
}
