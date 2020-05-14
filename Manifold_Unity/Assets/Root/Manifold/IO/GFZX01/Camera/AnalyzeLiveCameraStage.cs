using GameCube.GFZX01.Camera;
using UnityEngine;

namespace Manifold.IO.GFZX01.Camera
{
    [CreateAssetMenu(menuName = MenuConst.GFZX01_Camera + "livecam_stage Analyzer")]
    public class AnalyzeLiveCameraStage : ExecutableScriptableObject,
        IAnalyzable
    {
        #region MEMBERS

        [Header("Output")]
        [SerializeField, BrowseFolderField]
        protected string outputPath;
        [SerializeField, BrowseFolderField("Assets/"), Tooltip("Used with IOOption.allFromSourceFolder")]
        protected string[] searchFolders;

        [Header("Preferences")]
        [SerializeField]
        protected bool openFolderAfterAnalysis = true;

        [Header("Analysis Options")]
        [SerializeField]
        protected IOOption analysisOption = IOOption.allFromAssetDatabase;
        [SerializeField]
        protected LiveCameraStageSobj[] analysisSobjs;

        #endregion

        public override string ExecuteText => "Analyze Live Camera Stage";

        public void Analyze()
        {
            analysisSobjs = IOUtility.GetSobjByOption(analysisSobjs, analysisOption, searchFolders);

            var filePath = AnalyzerUtility.GetAnalysisFilePathTSV(outputPath, "livecam_stage stacked");
            WriteLivecamStageStacked(filePath);

            var filePath2 = AnalyzerUtility.GetAnalysisFilePathTSV(outputPath, "livecam_stage single line");
            WriteLivecamStageSingleLine(filePath2);

            if (openFolderAfterAnalysis)
            {
                IOUtility.OpenFileFolder(filePath);
            }
        }

        public override void Execute() => Analyze();

        public void WriteLivecamStageStacked(string filePath)
        {
            using (var writer = AnalyzerUtility.OpenWriter(filePath))
            {
                writer.PushCol("File Name");
                writer.PushCol("Stage Index");
                writer.PushCol("Venue");
                writer.PushCol("Stage");
                writer.PushCol("Pan Index");
                writer.PushCol("Frame Count");
                writer.PushCol("Lerp Speed");
                writer.PushCol("unk_0x08");
                writer.PushCol("Section");
                writer.PushCol("camera pos");
                writer.PushCol("lookat pos");
                writer.PushCol("fov");
                writer.PushCol("unk_flags_0x1C");
                writer.WriteFlagNames<EnumFlags32>();
                writer.PushCol("unk_flags_0x20");
                writer.WriteFlagNames<EnumFlags32>();
                writer.PushRow();

                foreach (var sobj in analysisSobjs)
                {
                    var panIndex = 0;
                    foreach (var pan in sobj.value.Pans)
                    {

                        var stageIndexText = System.Text.RegularExpressions.Regex.Match(sobj.FileName, @"\d+").Value;
                        var stageIndex = int.Parse(stageIndexText);
                        var stageInfo = StageUtility.GetStageInfo(stageIndex);

                        var panArray = new CameraPanPoint[] { pan.from, pan.to };
                        for (int i = 0; i < panArray.Length; i++)
                        {
                            writer.PushCol(sobj.FileName);
                            writer.PushCol(stageIndex);
                            writer.PushCol(stageInfo.Item1.GetDescription());
                            writer.PushCol(stageInfo.Item3.GetDescription());
                            writer.PushCol(panIndex);
                            writer.PushCol(pan.frameCount);
                            writer.PushCol(pan.lerpSpeed);
                            writer.PushCol(pan.unk_0x08);

                            if (i == 0)
                            {
                                writer.PushCol("FROM");
                            }
                            else
                            {
                                writer.PushCol("TO");
                            }

                            writer.PushCol(panArray[i].cameraPosition);
                            writer.PushCol(panArray[i].lookatPosition);
                            writer.PushCol(panArray[i].fov);
                            writer.PushCol((int)panArray[i].unk_flags_0x1C);
                            writer.WriteFlags(panArray[i].unk_flags_0x1C);
                            writer.PushCol((int)panArray[i].unk_flags_0x20);
                            writer.WriteFlags(panArray[i].unk_flags_0x20);
                            writer.PushRow();
                        }
                        panIndex++;
                    }
                }
            }
        }

        public void WriteLivecamStageSingleLine(string filePath)
        {
            using (var writer = AnalyzerUtility.OpenWriter(filePath))
            {
                writer.PushCol("File Name");
                writer.PushCol("Stage Index");
                writer.PushCol("Venue");
                writer.PushCol("Stage");
                writer.PushCol("Pan Index");
                writer.PushCol("Frame Count");
                writer.PushCol("Lerp Speed");
                writer.PushCol("unk_0x08");
                writer.PushCol("From: camera pos");
                writer.PushCol("From: lookat pos");
                writer.PushCol("From: fov");
                writer.PushCol("From: unk_flags_0x1C");
                writer.WriteFlagNames<EnumFlags32>();
                writer.PushCol("From: unk_flags_0x20");
                writer.WriteFlagNames<EnumFlags32>();
                writer.PushCol("To: camera pos");
                writer.PushCol("To: lookat pos");
                writer.PushCol("To: fov");
                writer.PushCol("To: unk_flags_0x1C");
                writer.WriteFlagNames<EnumFlags32>();
                writer.PushCol("To: unk_flags_0x20");
                writer.WriteFlagNames<EnumFlags32>();
                writer.PushRow();

                foreach (var sobj in analysisSobjs)
                {
                    var panIndex = 0;
                    foreach (var pan in sobj.value.Pans)
                    {
                        var stageIndexText = System.Text.RegularExpressions.Regex.Match(sobj.FileName, @"\d+").Value;
                        var stageIndex = int.Parse(stageIndexText);
                        var stageInfo = StageUtility.GetStageInfo(stageIndex);

                        writer.PushCol(sobj.FileName);
                        writer.PushCol(stageIndex);
                        writer.PushCol(stageInfo.Item1.GetDescription());
                        writer.PushCol(stageInfo.Item3.GetDescription());
                        writer.PushCol(panIndex);
                        writer.PushCol(pan.frameCount);
                        writer.PushCol(pan.lerpSpeed);
                        writer.PushCol(pan.unk_0x08);
                        // FROM
                        writer.PushCol(pan.from.cameraPosition);
                        writer.PushCol(pan.from.lookatPosition);
                        writer.PushCol(pan.from.fov);
                        writer.PushCol((int)pan.from.unk_flags_0x1C);
                        writer.WriteFlags(pan.from.unk_flags_0x1C);
                        writer.PushCol((int)pan.from.unk_flags_0x20);
                        writer.WriteFlags(pan.from.unk_flags_0x20);
                        // TO
                        writer.PushCol(pan.to.cameraPosition);
                        writer.PushCol(pan.to.lookatPosition);
                        writer.PushCol(pan.to.fov);
                        writer.PushCol((int)pan.to.unk_flags_0x1C);
                        writer.WriteFlags(pan.to.unk_flags_0x1C);
                        writer.PushCol((int)pan.to.unk_flags_0x20);
                        writer.WriteFlags(pan.to.unk_flags_0x20);
                        writer.PushRow();
                        panIndex++;
                    }
                }
            }
        }
    }
}
