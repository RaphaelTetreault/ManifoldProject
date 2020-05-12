using Manifold.IO;
using System.IO;
using UnityEditor;
using UnityEngine;
using GameCube.FZeroGX;
using GameCube.FZeroGX.COLI_COURSE;

[CreateAssetMenu(menuName = "Manifold/Analysis/" + "NEW COLI Analyzer")]
public class ColiAnalyzer : ExecutableScriptableObject,
        IAnalyzable
{
    #region MEMBERS

    [Header("Output")]
    [SerializeField, BrowseFolderField]
    protected string outputPath;
    [SerializeField, BrowseFolderField("Assets/"), Tooltip("Used with IOOption.allFromSourceFolder")]
    protected string[] searchFolders;

    //[Header("Output File Names")]
    //[SerializeField]
    //protected string fileName = "Topology Parameters";

    [Header("Preferences")]
    [SerializeField]
    protected bool openFolderAfterAnalysis = true;

    [Header("Analysis Options")]
    [SerializeField]
    protected IOOption analysisOption = IOOption.allFromAssetDatabase;
    [SerializeField]
    protected ColiSceneSobj[] analysisSobjs;

    #endregion

    #region PROPERTIES

    public override string ExecuteText => "Analyze COLI";

    #endregion

    public void Analyze()
    {
        analysisSobjs = IOUtility.GetSobjByOption(analysisSobjs, analysisOption, searchFolders);
        var time = AnalyzerUtility.FileTimestamp();

        // TOPOLOGY PARAMETERS
        {
            var count = TopologyParameters.kFieldCount;
            for (int i = 0; i < count; i++)
            {
                var filePath = $"{time} TopologyParameters {i + 1}.tsv";
                filePath = Path.Combine(outputPath, filePath);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, (float)(i + 1) / TopologyParameters.kFieldCount);
                AnalyzeTrackData(filePath, i);
            }
        }

        // TRACK TRANSFORMS
        {
            var filePath = $"{time} TrackTransforms.tsv";
            filePath = Path.Combine(outputPath, filePath);
            EditorUtility.DisplayProgressBar(ExecuteText, filePath, .5f);
            AnalyzeTransforms(filePath);
        }

        // ANIMATIONS
        {
            var filePath = $"{time} Animations.tsv";
            filePath = Path.Combine(outputPath, filePath);
            EditorUtility.DisplayProgressBar(ExecuteText, filePath, .5f);
            AnalyzeGameObjectAnimations(filePath);
        }

        // ANIMATIONS INDIVIDUALIZED
        {
            var count = GameCube.FZeroGX.COLI_COURSE.Animation.kSizeCurvesPtrs;
            for (int i = 0; i < count; i++)
            {
                var filePath = $"{time} Animations {i}.tsv";
                filePath = Path.Combine(outputPath, filePath);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, (float)(i + 1) / count);
                AnalyzeGameObjectAnimationsIndex(filePath, i);
            }
        }

        // OPEN FOLDER after analysis
        if (openFolderAfterAnalysis)
        {
            IOUtility.OpenFileFolder(outputPath + "/");
        }

        EditorUtility.ClearProgressBar();
    }

    public override void Execute()
        => Analyze();

    #region Track Data / Transforms

    public void AnalyzeTrackData(string filename, int paramIndex)
    {
        using (var writer = AnalyzerUtility.OpenWriter(filename))
        {
            // Write header
            writer.PushCol("File Path");
            writer.PushCol("Track Node Index");
            writer.PushCol($"Param [{paramIndex + 1}] Index");
            writer.PushCol("Address");
            writer.PushCol("unk_0x00");
            writer.PushCol("unk_0x04");
            writer.PushCol("unk_0x08");
            writer.PushCol("unk_0x0C");
            writer.PushCol("unk_0x10");
            writer.PushRow();

            // foreach File
            foreach (var sobj in analysisSobjs)
            {
                // foreach Transform
                foreach (var trackTransform in sobj.scene.trackTransforms)
                {
                    WriteTrackDataRecursive(writer, sobj, 0, paramIndex, trackTransform);
                }
            }

            writer.Flush();
        }
    }

    public void WriteTrackDataRecursive(StreamWriter writer, ColiSceneSobj sobj, int level, int i, TrackTransform trackTransform)
    {
        if (trackTransform.topologyParameters != null)
        {
            var @params = trackTransform.topologyParameters.Params();
            var printIndex = 1;
            var printTotal = @params[i].Length;

            // foreach Topology
            foreach (var param in @params[i])
                WriteTrackData(writer, sobj, level, printIndex++, printTotal, param);
        }

        foreach (var child in trackTransform.children)
        {
            WriteTrackDataRecursive(writer, sobj, level + 1, i, child);
        }
    }

    public void WriteTrackData(StreamWriter writer, ColiSceneSobj sobj, int level, int index, int total, TopologyParam param)
    {
        writer.PushCol(sobj.FilePath);
        writer.PushCol($"[{index}/{total}]");
        writer.PushCol($"{level}");
        writer.PushCol("0x" + param.StartAddress.ToString("X8"));
        writer.PushCol(param.unk_0x00);
        writer.PushCol(param.unk_0x04);
        writer.PushCol(param.unk_0x08);
        writer.PushCol(param.unk_0x0C);
        writer.PushCol(param.unk_0x10);
        writer.PushRow();
    }


    public void AnalyzeTransforms(string filename)
    {
        using (var writer = AnalyzerUtility.OpenWriter(filename))
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

            foreach (var sobj in analysisSobjs)
            {
                var index = 0;
                var total = sobj.scene.trackTransforms.Count;
                foreach (var trackTransform in sobj.scene.trackTransforms)
                {
                    WriteTrackTransformRecursive(writer, sobj, 0, index++, total, trackTransform);
                }
            }

            writer.Flush();
        }
    }

    public void WriteTrackTransformRecursive(StreamWriter writer, ColiSceneSobj sobj, int level, int index, int total, TrackTransform trackTransform)
    {
        WriteTrackTransform(writer, sobj, level, index, total, trackTransform);

        foreach (var child in trackTransform.children)
        {
            WriteTrackTransformRecursive(writer, sobj, level + 1, index, total, child);
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

    #endregion

    #region GameObject Animations

    public void AnalyzeGameObjectAnimations(string filename)
    {
        using (var writer = AnalyzerUtility.OpenWriter(filename))
        {
            // Write header
            writer.PushCol("File Path");
            writer.PushCol("Game Object #");
            writer.PushCol("Game Object");
            writer.PushCol("Anim Addr");
            writer.PushCol("Key Addr");
            writer.PushCol("Anim Index [0-10]");
            writer.PushCol("Unk_0x00");
            writer.PushCol("Time");
            writer.PushCol("Value");
            writer.PushCol("Unk_0x0C");
            writer.PushCol("Unk_0x10");
            writer.PushRow();

            foreach (var file in analysisSobjs)
            {
                int gameObjectIndex = 0;
                foreach (var gameObject in file.scene.gameObjects)
                {
                    if (gameObject.animation == null)
                        continue;

                    int animIndex = 0;
                    foreach (var animationCurve in gameObject.animation.animCurves)
                    {
                        foreach (var keyable in animationCurve.keyableAttributes)
                        {
                            writer.PushCol(file.fileName);
                            writer.PushCol(gameObjectIndex);
                            writer.PushCol(gameObject.name);
                            writer.PushCol($"0x{animationCurve.StartAddress:X8}");
                            writer.PushCol($"0x{keyable.StartAddress:X8}");
                            writer.PushCol(animIndex);
                            writer.PushCol(keyable.unk_0x00);
                            writer.PushCol(keyable.time);
                            writer.PushCol(keyable.value);
                            writer.PushCol(keyable.unk_0x0C);
                            writer.PushCol(keyable.unk_0x10);
                            writer.PushRow();
                        }
                        animIndex++;
                    }
                    gameObjectIndex++;
                }
            }
            writer.Flush();
        }
    }

    public void AnalyzeGameObjectAnimationsIndex(string filename, int index)
    {
        using (var writer = AnalyzerUtility.OpenWriter(filename))
        {
            // Write header
            writer.PushCol("File Path");
            writer.PushCol("Game Object #");
            writer.PushCol("Game Object");
            writer.PushCol("Anim Addr");
            writer.PushCol("Key Addr");
            writer.PushCol("Anim Index [0-10]");
            writer.PushCol("Unk_0x00");
            writer.PushCol("Time");
            writer.PushCol("Value");
            writer.PushCol("Unk_0x0C");
            writer.PushCol("Unk_0x10");
            writer.PushRow();

            foreach (var file in analysisSobjs)
            {
                int gameObjectIndex = 0;
                foreach (var gameObject in file.scene.gameObjects)
                {
                    int animIndex = 0;
                    foreach (var animationCurve in gameObject.animation.animCurves)
                    {
                        foreach (var keyable in animationCurve.keyableAttributes)
                        {
                            /// HACK, write each anim index as separate file
                            if (animIndex != index)
                                continue;

                            writer.PushCol(file.fileName);
                            writer.PushCol(gameObjectIndex);
                            writer.PushCol(gameObject.name);
                            writer.PushCol($"0x{animationCurve.StartAddress:X8}");
                            writer.PushCol($"0x{keyable.StartAddress:X8}");
                            writer.PushCol(animIndex);
                            writer.PushCol(keyable.unk_0x00);
                            writer.PushCol(keyable.time);
                            writer.PushCol(keyable.value);
                            writer.PushCol(keyable.unk_0x0C);
                            writer.PushCol(keyable.unk_0x10);
                            writer.PushRow();
                        }
                        animIndex++;
                    }
                    gameObjectIndex++;
                }
            }
            writer.Flush();
        }
    }

    #endregion
}
