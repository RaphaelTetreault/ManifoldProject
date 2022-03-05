//using System.IO;
//using Manifold.IO;
//using UnityEditor;
//using UnityEngine;
//using UnityEngine.Serialization;

//namespace Manifold.EditorTools.GC.GFZ.GMA
//{
//    [CreateAssetMenu(menuName = Const.Menu.GfzGMA + "GMA Analyzer")]
//    public class GmaAnalyzer : ExecutableScriptableObject,
//        IAnalyzable
//    {
//        #region MEMBERS

//        [Header("Output")]
//        [FormerlySerializedAs("outputPath")]
//        [SerializeField, BrowseFolderField]
//        protected string outputTo;
//        [SerializeField, BrowseFolderField("Assets/"), Tooltip("Used with IOOption.allFromSourceFolder")]
//        protected string[] searchFolders;

//        [Header("Preferences")]
//        [SerializeField]
//        protected bool openFolderAfterAnalysis = true;

//        [Header("Analysis Options")]
//        [SerializeField]
//        protected IOOption analysisOption = IOOption.allFromAssetDatabase;
//        [SerializeField]
//        protected GmaSobj[] analysisSobjs;

//        #endregion

//        #region PROPERTIES

//        public override string ExecuteText => "Analyze GMA";

//        #endregion

//        #region METHODS

//        public void Analyze()
//        {
//            analysisSobjs = AssetDatabaseUtility.GetSobjByOption(analysisSobjs, analysisOption, searchFolders);

//            var numProcesses = 4f;
//            var processIndex = 0;
//            var time = AnalyzerUtility.GetFileTimestamp();

//            // EX
//            var texFlagAnalysisFile = Path.Combine(outputTo, $"{time} GMA Unknown Flags.tsv");
//            EditorUtility.DisplayProgressBar(ExecuteText, texFlagAnalysisFile, processIndex++ / numProcesses);
//            WriteGcmfTexUnkFlagsAnalysis(texFlagAnalysisFile);

//            // GCMF
//            var gcfmAnalysisFile = Path.Combine(outputTo, $"{time} GMA GCMF.tsv");
//            EditorUtility.DisplayProgressBar(ExecuteText, gcfmAnalysisFile, processIndex++ / numProcesses);
//            WriteGcmfAnalysis(gcfmAnalysisFile);

//            // GCMF TEX
//            var texAnalysisFile = Path.Combine(outputTo, $"{time} GMA Textures.tsv");
//            EditorUtility.DisplayProgressBar(ExecuteText, texAnalysisFile, processIndex++ / numProcesses);
//            WriteTexAnalysis(texAnalysisFile);

//            // GCMF MAT
//            var matAnalysisFile = Path.Combine(outputTo, $"{time} GMA Materials.tsv");
//            EditorUtility.DisplayProgressBar(ExecuteText, matAnalysisFile, processIndex++ / numProcesses);
//            WriteMatAnalysis(matAnalysisFile);

//            // END
//            EditorUtility.ClearProgressBar();

//            if (openFolderAfterAnalysis)
//            {
//                OSUtility.OpenDirectory(gcfmAnalysisFile);
//            }
//        }

//        public override void Execute()
//            => Analyze();

//        public void WriteGcmfAnalysis(string fileName)
//        {
//            var writer = AnalyzerUtility.OpenWriter(fileName);
//            // Write header
//            if (writer.BaseStream.Length <= 0)
//            {
//                writer.WriteNextCol("File Name");
//                writer.WriteNextCol("GCMF File Name");
//                writer.WriteNextCol("GCMF Index");
//                writer.WriteNextCol("Address");
//                writer.WriteNextCol("Attributes");
//                writer.WriteNextCol("Origin");
//                writer.WriteNextCol("Radius");
//                writer.WriteNextCol("Texturecount");
//                writer.WriteNextCol("Materialcount");
//                writer.WriteNextCol("Translucidmaterialcount");
//                writer.WriteNextCol("Transform Matrix Count");
//                writer.WriteNextCol("Zero_0x1F");
//                writer.WriteNextCol("Indices Rel Ptr");
//                writer.WriteNextCol("Zero_0x24");
//                writer.WriteNextRow();
//            }

//            foreach (var sobj in analysisSobjs)
//            {
//                var gma = sobj.Value;

//                // Write contents
//                var index = 0;
//                var maxIndex = gma.GcmfCount;
//                foreach (var gcmf in gma.GCMF)
//                {
//                    if (string.IsNullOrEmpty(gcmf.ModelName))
//                        continue;

//                    // Get reference to type less
//                    var gcmfProp = gcmf.GcmfProperties;

//                    writer.WriteNextCol(sobj.FileName);
//                    writer.WriteNextCol(gcmf.ModelName);
//                    writer.WriteNextCol($"GCMF[{index}/{maxIndex}]");
//                    writer.WriteNextCol(gcmf.StartAddressHex());
//                    writer.WriteNextCol(gcmfProp.Attributes);
//                    writer.WriteNextCol(gcmfProp.Origin);
//                    writer.WriteNextCol(gcmfProp.Radius);
//                    writer.WriteNextCol(gcmfProp.TextureCount);
//                    writer.WriteNextCol(gcmfProp.MaterialCount);
//                    writer.WriteNextCol(gcmfProp.TranslucidMaterialCount);
//                    writer.WriteNextCol(gcmfProp.TransformMatrixCount);
//                    writer.WriteNextCol(gcmfProp.Zero_0x1F);
//                    writer.WriteNextCol(gcmfProp.GcmfSize);
//                    writer.WriteNextCol(gcmfProp.Zero_0x24);
//                    writer.WriteNextRow();

//                    index++;
//                }
//            }
//            writer.Close();
//        }
        
//        public void WriteGcmfTexUnkFlagsAnalysis(string fileName)
//        {
//            var size = 8;
//            var unkFlags0x00 = new int[size];
//            var unkFlags0x02 = new int[size];
//            var unkFlags0x03 = new int[size];
//            var unkFlags0x0C = new int[size];
//            var unkFlags0x10 = new int[size];

//            foreach (var sobj in analysisSobjs)
//            {
//                foreach (var gcmf in sobj.Value.GCMF)
//                {
//                    if (gcmf == null || string.IsNullOrEmpty(gcmf.ModelName))
//                        continue;

//                    foreach (var tex in gcmf.Textures)
//                    {
//                        for (int i = 0; i < size; i++)
//                        {
//                            var isFlagEnabled = ((int)tex.Unk_0x00 >> i) & 1;
//                            unkFlags0x00[i] += isFlagEnabled;

//                            isFlagEnabled = ((int)tex.MipmapSettings >> i) & 1;
//                            unkFlags0x02[i] += isFlagEnabled;

//                            isFlagEnabled = ((int)tex.Wrapflags >> i) & 1;
//                            unkFlags0x03[i] += isFlagEnabled;

//                            isFlagEnabled = ((int)tex.Unk_0x0C >> i) & 1;
//                            unkFlags0x0C[i] += isFlagEnabled;

//                            isFlagEnabled = ((int)tex.Unk_0x10 >> i) & 1;
//                            unkFlags0x10[i] += isFlagEnabled;
//                        }
//                    }
//                }
//            }

//            var writer = AnalyzerUtility.OpenWriter(fileName);

//            writer.WriteNextCol();
//            for (int i = 0; i < size; i++)
//                writer.WriteNextCol($"Flag Bit {i}");
//            writer.WriteNextRow();

//            writer.WriteNextCol("UnkFlags0x00");
//            for (int i = 0; i < unkFlags0x00.Length; i++)
//                writer.WriteNextCol(unkFlags0x00[i]);
//            writer.WriteNextRow();

//            writer.WriteNextCol("UnkFlags0x02");
//            for (int i = 0; i < unkFlags0x02.Length; i++)
//                writer.WriteNextCol(unkFlags0x02[i]);
//            writer.WriteNextRow();

//            writer.WriteNextCol("UnkFlags0x03");
//            for (int i = 0; i < unkFlags0x03.Length; i++)
//                writer.WriteNextCol(unkFlags0x03[i]);
//            writer.WriteNextRow();

//            writer.WriteNextCol("UnkFlags0x0C");
//            for (int i = 0; i < unkFlags0x0C.Length; i++)
//                writer.WriteNextCol(unkFlags0x0C[i]);
//            writer.WriteNextRow();

//            writer.WriteNextCol("UnkFlags0x10");
//            for (int i = 0; i < unkFlags0x10.Length; i++)
//                writer.WriteNextCol(unkFlags0x10[i]);
//            writer.WriteNextRow();

//            writer.Close();
//        }

//        public void WriteMatAnalysis(string fileName)
//        {
//            var writer = AnalyzerUtility.OpenWriter(fileName);

//            if (writer.BaseStream.Length <= 0)
//            {
//                writer.WriteNextCol("File Name");
//                writer.WriteNextCol("Model Name");
//                writer.WriteNextCol("Address");
//                writer.WriteNextCol("Model Index");
//                writer.WriteNextCol("Mat Index");
//                writer.WriteNextCol("Tex Index");
//                writer.WriteNextCol("Zero 0x00");
//                writer.WriteNextCol("Unk_0x02");
//                writer.WriteNextCol("Unk_0x03");
//                writer.WriteNextCol("Color0");
//                writer.WriteNextCol("Color1");
//                writer.WriteNextCol("Color2");
//                writer.WriteNextCol("Unk_0x10");
//                writer.WriteNextCol("Unk_0x11");
//                writer.WriteNextCol("Unk_0x12");
//                writer.WriteNextCol("Vertex Render Flags");
//                writer.WriteNextCol("Unk_0x14");
//                writer.WriteNextCol("Unk_0x15");
//                writer.WriteNextCol("Tex 0 Index");
//                writer.WriteNextCol("Tex 1 Index");
//                writer.WriteNextCol("Tex 2 Index");
//                writer.WriteNextCol("Vertex Descriptor Flags");
//                writer.WriteNextCol("Transform Matrix Indexes");
//                writer.WriteNextCol("Mat display list size");
//                writer.WriteNextCol("Tl mat display list size");
//                writer.WriteNextCol("Bounding Sphere Origin");
//                writer.WriteNextCol("Zero 0x3C");
//                writer.WriteNextCol("Unk_0x40");
//                writer.WriteNextRow();
//            }

//            foreach (var sobj in analysisSobjs)
//            {
//                var gma = sobj.Value;

//                // Write contents
//                var gcmfIndex = 0; // 0 indexed in debugger
//                var gcmfIndexMax = gma.GcmfCount;
//                foreach (var gcmf in gma.GCMF)
//                {
//                    // skip null models
//                    if (string.IsNullOrEmpty(gcmf.ModelName))
//                        continue;

//                    var matIndex = 1;
//                    var texIndex = 1; // not zero indexed in debugger
//                    var matIndexMax = gcmf.Submeshes.Length;
//                    var texIndexMax = 0;
//                    foreach (var gcmfRenderData in gcmf.Submeshes)
//                        texIndexMax += gcmfRenderData.Material.TexturesUsedCount;

//                    foreach (var gcmfRenderData in gcmf.Submeshes)
//                    {
//                        var material = gcmfRenderData.Material;

//                        writer.WriteNextCol(sobj.FileName);
//                        writer.WriteNextCol(gcmf.ModelName);
//                        writer.WriteNextCol(material.StartAddressHex());
//                        writer.WriteNextCol($"GCMF[{gcmfIndex}/{gcmfIndexMax}]");
//                        writer.WriteNextCol($"MAT[{matIndex}/{matIndexMax}]");
//                        writer.WriteNextCol($"TEX[{texIndex}/{texIndexMax}]");
//                        writer.WriteNextCol(material.Zero_0x00);
//                        writer.WriteNextCol(material.Unk_0x02);
//                        writer.WriteNextCol(material.Unk_0x03);
//                        writer.WriteNextCol(material.Color0);
//                        writer.WriteNextCol(material.Color1);
//                        writer.WriteNextCol(material.Color2);
//                        writer.WriteNextCol(material.Unk_0x10);
//                        writer.WriteNextCol(material.Unk_0x11);
//                        writer.WriteNextCol(material.TexturesUsedCount);
//                        writer.WriteNextCol(material.VertexRenderFlags);
//                        writer.WriteNextCol(material.Unk_0x14);
//                        writer.WriteNextCol(material.Unk_0x15);
//                        writer.WriteNextCol(material.Tex0Index);
//                        writer.WriteNextCol(material.Tex1Index);
//                        writer.WriteNextCol(material.Tex2Index);
//                        writer.WriteNextCol(material.VertexDescriptorFlags);
//                        writer.WriteNextCol(material.MatrixIndexes);
//                        writer.WriteNextCol(material.MatDisplayListSize);
//                        writer.WriteNextCol(material.TlMatDisplayListSize);
//                        writer.WriteNextCol(material.BoudingSphereOrigin);
//                        writer.WriteNextCol(material.Unk_0x3C);
//                        writer.WriteNextCol(material.Unk_0x40);
//                        writer.WriteNextRow();

//                        matIndex++;
//                        texIndex += material.TexturesUsedCount;
//                    }
//                    gcmfIndex++;
//                }
//            }
//            writer.Close();
//        }

//        public void WriteTexAnalysis(string fileName)
//        {
//            var writer = AnalyzerUtility.OpenWriter(fileName);

//            if (writer.BaseStream.Length <= 0)
//            {
//                writer.WriteNextCol("File Name");
//                writer.WriteNextCol("Model Index");
//                writer.WriteNextCol("Model Name");
//                writer.WriteNextCol("Texture Index");
//                writer.WriteNextCol("Address");
//                writer.WriteNextCol("Unk_0x00");
//                writer.WriteNextCol("Mipmap Settings");
//                writer.WriteNextCol("Wrap Flags");
//                writer.WriteNextCol("Tpl Texture Index");
//                writer.WriteNextCol("Unk_0x06");
//                writer.WriteNextCol("Anisotropic Level");
//                writer.WriteNextCol("Unk_0x0C");
//                writer.WriteNextCol("Is Swappable Texture?");
//                writer.WriteNextCol("Index");
//                writer.WriteNextCol("Unk_0x10");
//                writer.WriteNextRow();
//            }

//            foreach (var sobj in analysisSobjs)
//            {
//                var gma = sobj.Value;
//                // Write contents
//                var modelIndex = 0;
//                var modelIndexMax = gma.GcmfCount;
//                foreach (var gcmf in gma.GCMF)
//                {
//                    var texIndex = 1; // debugger is not zero-indexed
//                    var texIndexMax = gcmf.Textures.Length;
//                    foreach (var tex in gcmf.Textures)
//                    {
//                        writer.WriteNextCol(sobj.FileName);
//                        writer.WriteNextCol($"GCMF[{modelIndex}/{modelIndexMax}]");
//                        writer.WriteNextCol(gcmf.ModelName);
//                        writer.WriteNextCol($"Tex[{texIndex}/{texIndexMax}]");
//                        writer.WriteNextCol(tex.StartAddressHex());
//                        writer.WriteNextCol(tex.Unk_0x00);
//                        writer.WriteNextCol(tex.MipmapSettings);
//                        writer.WriteNextCol(tex.Wrapflags);
//                        writer.WriteNextCol(tex.Tpltextureid);
//                        writer.WriteNextCol(tex.Unk_0x06);
//                        writer.WriteNextCol(tex.Anisotropiclevel);
//                        writer.WriteNextCol(tex.Unk_0x0C);
//                        writer.WriteNextCol(tex.IsSwappableTexture);
//                        writer.WriteNextCol(tex.Index);
//                        writer.WriteNextCol(tex.Unk_0x10);
//                        writer.WriteNextRow();

//                        texIndex++;
//                    }
//                    modelIndex++;
//                }
//            }

//            writer.Close();
//        }

//        #endregion
//    }
//}