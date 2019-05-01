using StarkTools.IO;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using GameCube.FZeroGX.GMA;

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
    //[SerializeField]
    //protected string materialAnimFile;
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
        //var matAnimAnalysisFile = Path.Combine(destinationDirectory, $"{materialAnimFile}_{time}.tsv");
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
        //EditorUtility.DisplayProgressBar(ButtonText, matAnimAnalysisFile, processIndex++ / numProcesses);
        //WriteMatAnimAnalysis(matAnimAnalysisFile);

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
                if (gcmf == null || gcmf.ModelName == GcmfProperties.kNullEntryName)
                    continue;

                foreach (var tex in gcmf.Textures)
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
                    gcmf.ModelName == GcmfProperties.kNullEntryName)
                    continue;

                // Get reference to type less
                var gcmfProp = gcmf.GcmfProperties;

                writer.PushCol(sobj.FileName);
                writer.PushCol(gcmf.ModelName);
                writer.PushCol($"GCMF[{index}/{maxIndex}]");
                writer.PushCol("0x" + gcmf.StartAddress.ToString("X8"));
                writer.PushCol(gcmfProp.Attributes);
                writer.PushCol(gcmfProp.Origin);
                writer.PushCol(gcmfProp.Radius);
                writer.PushCol(gcmfProp.TextureCount);
                writer.PushCol(gcmfProp.MaterialCount);
                writer.PushCol(gcmfProp.TranslucidMaterialCount);
                writer.PushCol(gcmfProp.TransformMatrixCount);
                writer.PushCol(gcmfProp.Zero_0x1F);
                writer.PushCol(gcmfProp.GcmfSize);
                writer.PushCol(gcmfProp.Zero_0x24);
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
                var texIndexMax = gcmf.Textures.Length;
                foreach (var tex in gcmf.Textures)
                {
                    writer.PushCol(sobj.FileName);
                    writer.PushCol($"GCMF[{modelIndex}/{modelIndexMax}]");
                    writer.PushCol(gcmf.ModelName);
                    writer.PushCol($"Tex[{texIndex}/{texIndexMax}]");
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
            writer.PushCol("Model Name");
            writer.PushCol("Address");
            writer.PushCol("Model Index");
            writer.PushCol("Mat Index");
            writer.PushCol("Tex Index");
            writer.PushCol("Zero 0x00");
            writer.PushCol("Unk_0x02");
            writer.PushCol("Unk_0x03");
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
            writer.PushCol("Zero 0x3C");
            writer.PushCol("Unk_0x40");
            writer.PushRow();
        }

        foreach (var sobj in analysisSobjs)
        {
            // Write contents
            var gcmfIndex = 1;
            var gcmfIndexMax = sobj.value.GcmfCount;
            foreach (var gcmf in sobj.Value.GCMF)
            {
                // skip null models
                if (string.IsNullOrEmpty(gcmf.ModelName) || gcmf.ModelName == GcmfProperties.kNullEntryName)
                    continue;

                var matIndex = 1;
                var texIndex = 1;
                var matIndexMax = gcmf.Submeshes.Length;
                var texIndexMax = 0;
                foreach (var gcmfRenderData in gcmf.Submeshes)
                    texIndexMax += gcmfRenderData.Material.TexturesUsedCount;

                foreach (var gcmfRenderData in gcmf.Submeshes)
                {
                    var material = gcmfRenderData.Material;

                    writer.PushCol(sobj.FileName);
                    writer.PushCol(gcmf.ModelName);
                    writer.PushCol("0x" + material.StartAddress.ToString("X8"));
                    writer.PushCol($"GCMF[{gcmfIndex}/{gcmfIndexMax}]");
                    writer.PushCol($"MAT[{matIndex}/{matIndexMax}]");
                    writer.PushCol($"TEX[{texIndex}/{texIndexMax}]");
                    writer.PushCol(material.Zero_0x00);
                    writer.PushCol(material.Unk_0x02);
                    writer.PushCol(material.Unk_0x03);
                    writer.PushCol(material.Color0);
                    writer.PushCol(material.Color1);
                    writer.PushCol(material.Color2);
                    writer.PushCol(material.Unk_0x10);
                    writer.PushCol(material.Unk_0x11);
                    writer.PushCol(material.TexturesUsedCount);
                    writer.PushCol(material.VertexRenderFlags);
                    writer.PushCol(material.Unk_0x14);
                    writer.PushCol(material.Unk_0x15);
                    writer.PushCol(material.Tex0Index);
                    writer.PushCol(material.Tex1Index);
                    writer.PushCol(material.Tex2Index);
                    writer.PushCol(material.VertexDescriptorFlags);
                    //writer.PushCol(gcmf.Material.TransformMatrixSpecificIndices);
                    writer.PushCol(material.MatDisplayListSize);
                    writer.PushCol(material.TlMatDisplayListSize);
                    writer.PushCol(material.BoudingSphereOrigin);
                    writer.PushCol(material.Unk_0x3C);
                    writer.PushCol(material.Unk_0x40);
                    writer.PushRow();

                    matIndex++;
                    texIndex += material.TexturesUsedCount;
                }
                gcmfIndex++;
            }
        }
        writer.Close();
    }

}
