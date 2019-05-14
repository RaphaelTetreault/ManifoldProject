using StarkTools.IO;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
//using System.Diagnostics;

namespace GameCube.FZeroGX.COLI_COURSE
{
    [CreateAssetMenu]
    public class ColiAnalyzer : AnalyzerSobj<ColiSceneSobj>
    {
        public override void Analyze()
        {
            if (loadAllOfType)
            {
                analysisSobjs = LoadAllOfType();
            }

            
            var time = FileTimestamp();
            //for (int i = 0; i < TopologyParameters.kFieldCount; i++)
            //{
            //    var filename = $"TopologyParameters{i+1}-{time}.tsv";
            //    var filePath = Path.Combine(destinationDirectory, filename);
            //    EditorUtility.DisplayProgressBar(ButtonText, filePath, (float)(i+1) / TopologyParameters.kFieldCount);
            //    AnalyzeTrackData(filename, i);
            //    Debug.Log(filePath);
            //}
            //EditorUtility.ClearProgressBar();

            var filename = $"TrackTransforms-{time}.tsv";
            AnalyzeTransforms(filename);
        }

        public void AnalyzeTrackData(string filename, int paramIndex)
        {
            using (var writer = OpenWriter(filename))
            {
                // Write header
                writer.PushCol("File Path");
                writer.PushCol("Track Node Index");
                writer.PushCol($"Param [{paramIndex+1}] Index");
                writer.PushCol("Address");
                writer.PushCol("unk_0x00");
                writer.PushCol("unk_0x04");
                writer.PushCol("unk_0x08");
                writer.PushCol("unk_0x0C");
                writer.PushCol("unk_0x10");
                writer.PushRow();

                // All files
                foreach (var sobj in analysisSobjs)
                {
                    // Individual file
                    var trackNodeCount = 1;
                    var trackNodeTotal = sobj.scene.trackNodes.Length;
                    foreach (var trackTransform in sobj.scene.trackTransforms)
                    {
                        // All Params[][]
                        var @params = trackTransform.topologyParameters.Params();
                        var paramCount = 1;
                        var paramTotal = @params[paramIndex].Length;
                        // Individual Params[]
                        foreach (var param in @params[paramIndex])
                        {
                            writer.PushCol(sobj.FilePath);
                            writer.PushCol($"[{trackNodeCount}/{trackNodeTotal}]");
                            writer.PushCol($"[{paramCount}/{paramTotal}]");
                            writer.PushCol("0x" + param.StartAddress.ToString("X8"));
                            writer.PushCol(param.unk_0x00);
                            writer.PushCol(param.unk_0x04);
                            writer.PushCol(param.unk_0x08);
                            writer.PushCol(param.unk_0x0C);
                            writer.PushCol(param.unk_0x10);
                            writer.PushRow();
                        }
                        trackNodeCount++;
                    }
                }
            }
        }

        public void AnalyzeTransforms(string filename)
        {
            using (var writer = OpenWriter(filename))
            {
                WriteTrackTransformHeader(writer);

                foreach (var sobj in analysisSobjs)
                {
                    var index = 0;
                    var total = sobj.scene.trackTransforms.Count;
                    foreach (var trackTransform in sobj.scene.trackTransforms)
                    {
                        WriteTrackTransformRecursive(writer, sobj, 0, index++, total, trackTransform);
                    }
                    index = 0;
                }
            }
        }

        public void WriteTrackTransformRecursive(StreamWriter writer, ColiSceneSobj sobj, int level, int index, int total, TrackTransform trackTransform)
        {
            WriteTrackTransform(writer, sobj, level, index, total, trackTransform);

            foreach (var child in trackTransform.children)
            {
                WriteTrackTransformRecursive(writer, sobj, level+1, index, total, child);
            }
        }

        public void WriteTrackTransform(StreamWriter writer, ColiSceneSobj sobj, int level, int index, int total, TrackTransform trackTransform)
        {
            writer.PushCol(sobj.FilePath);
            writer.PushCol($"[{index}/{total}]");
            writer.PushCol($"{level}");
            writer.PushCol("0x" + trackTransform.StartAddress.ToString("X8"));
            writer.PushCol(trackTransform.hierarchyDepth);
            writer.PushCol(trackTransform.zero_0x01);
            writer.PushCol(trackTransform.hasChildren);
            writer.PushCol(trackTransform.zero_0x03);
            writer.PushCol("0x" + trackTransform.topologyParamsAbsPtr.ToString("X8"));
            writer.PushCol("0x" + trackTransform.unk_0x08_absPtr.ToString("X8"));
            writer.PushCol(trackTransform.childCount);
            writer.PushCol("0x" + trackTransform.childrenAbsPtr.ToString("X8"));
            writer.PushCol(trackTransform.localScale);
            writer.PushCol(trackTransform.localRotation);
            writer.PushCol(trackTransform.localPosition);
            writer.PushCol(trackTransform.unk_0x38);
            writer.PushCol(trackTransform.unk_0x3C);
            writer.PushCol(trackTransform.unk_0x40);
            writer.PushCol(trackTransform.zero_0x44);
            writer.PushCol(trackTransform.zero_0x48);
            writer.PushCol(trackTransform.unk_0x4C);
            writer.PushRow();
        }

        public void WriteTrackTransformHeader(StreamWriter writer)
        {
            writer.PushCol("File Path");
            writer.PushCol("Root Index");
            writer.PushCol("Transform Depth");
            writer.PushCol("Address");
            writer.PushCol("Hierarchy Depth");
            writer.PushCol("zero_0x01");
            writer.PushCol("hasChildren");
            writer.PushCol("zero_0x03");
            writer.PushCol("topologyParamsAbsPtr");
            writer.PushCol("zero_0x08");
            writer.PushCol("Child Count");
            writer.PushCol("Children Abs Ptr");
            writer.PushCol("localScale");
            writer.PushCol("localRotation");
            writer.PushCol("localPosition");
            writer.PushCol("unk_0x38");
            writer.PushCol("unk_0x3C");
            writer.PushCol("unk_0x40");
            writer.PushCol("unk_0x44");
            writer.PushCol("unk_0x48");
            writer.PushCol("unk_0x4C");
            writer.PushRow();
        }
    }
}
