using GameCube.GFZ;
using GameCube.GFZ.FMI;
using Manifold.IO.GFZ.FMI;
using System.IO;
using UnityEngine;

namespace Manifold.IO.GFZ.Camera
{
    [CreateAssetMenu(menuName = MenuConst.GfzFMI + "FMI Analyzer")]
    public class FmiAnalyzer : ExecutableScriptableObject,
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
        protected FmiSobj[] analysisSobjs;

        #endregion

        public override string ExecuteText => "Analyze Live Camera Stage";

        public void Analyze()
        {
            analysisSobjs = AssetDatabaseUtility.GetSobjByOption(analysisSobjs, analysisOption, searchFolders);

            var filePath = AnalyzerUtility.GetAnalysisFilePathTSV(outputPath, "FMI Animation");
            WriteFmiAnimation(filePath);

            var filePath2 = AnalyzerUtility.GetAnalysisFilePathTSV(outputPath, "FMI Particle");
            WriteFmiParticle(filePath2);

            OSUtility.OpenDirectory(openFolderAfterAnalysis, filePath);
        }

        public override void Execute() => Analyze();

        public void WriteFmiAnimation(string filePath)
        {
            using (var writer = AnalyzerUtility.OpenWriter(filePath))
            {
                WriteFmiHeader(writer);
                writer.PushCol("Index");
                writer.WriteColNicify(nameof(ExhaustAnimation.position));
                writer.WriteColNicify(nameof(ExhaustAnimation.unk_0x0C));
                writer.WriteColNicify(nameof(ExhaustAnimation.animType));
                writer.PushRow();

                foreach (var sobj in analysisSobjs)
                {
                    var fmi = sobj.Value;
                    var idx = 0;
                    foreach (var anim in fmi.animations)
                    {
                        WriteFmiHeader(writer, fmi);
                        writer.PushCol(idx++);
                        writer.PushCol(anim.position);
                        writer.PushCol(anim.unk_0x0C);
                        writer.PushCol(anim.animType);
                        writer.PushRow();
                    }
                }
            }
        }

        public void WriteFmiParticle(string filePath)
        {
            using (var writer = AnalyzerUtility.OpenWriter(filePath))
            {
                WriteFmiHeader(writer);
                writer.PushCol("Index");
                writer.WriteColNicify(nameof(ExhaustParticle.position));
                writer.WriteColNicify(nameof(ExhaustParticle.unk_0x0C));
                writer.WriteColNicify(nameof(ExhaustParticle.unk_0x10));
                writer.WriteColNicify(nameof(ExhaustParticle.scaleMin));
                writer.WriteColNicify(nameof(ExhaustParticle.scaleMax));
                writer.WriteColNicify(nameof(ExhaustParticle.colorMin) + " r");
                writer.WriteColNicify(nameof(ExhaustParticle.colorMin) + " g");
                writer.WriteColNicify(nameof(ExhaustParticle.colorMin) + " b");
                writer.WriteColNicify(nameof(ExhaustParticle.colorMin) + " a");
                writer.WriteColNicify(nameof(ExhaustParticle.colorMax) + " r");
                writer.WriteColNicify(nameof(ExhaustParticle.colorMax) + " g");
                writer.WriteColNicify(nameof(ExhaustParticle.colorMax) + " b");
                writer.WriteColNicify(nameof(ExhaustParticle.colorMax) + " a");
                writer.PushRow();

                foreach (var sobj in analysisSobjs)
                {
                    var fmi = sobj.Value;
                    var idx = 0;
                    foreach (var particle in fmi.particles)
                    {
                        WriteFmiHeader(writer, fmi);
                        writer.PushCol(idx++);
                        writer.PushCol(particle.position);
                        writer.PushCol(particle.unk_0x0C);
                        writer.PushCol(particle.unk_0x10);
                        writer.PushCol(particle.scaleMin);
                        writer.PushCol(particle.scaleMax);
                        writer.PushCol(particle.colorMin.r);
                        writer.PushCol(particle.colorMin.g);
                        writer.PushCol(particle.colorMin.b);
                        writer.PushCol(particle.colorMin.a);
                        writer.PushCol(particle.colorMax.r);
                        writer.PushCol(particle.colorMax.g);
                        writer.PushCol(particle.colorMax.b);
                        writer.PushCol(particle.colorMax.a);
                        writer.PushRow();
                    }
                }
            }
        }

        public void WriteFmiHeader(StreamWriter writer)
        {
            writer.PushCol("File Name");
            writer.WriteColNicify(nameof(Fmi.unk_0x00));
            writer.WriteColNicify(nameof(Fmi.unk_0x01));
            writer.WriteColNicify(nameof(Fmi.animationCount));
            writer.WriteColNicify(nameof(Fmi.exhaustCount));
            writer.WriteColNicify(nameof(Fmi.unk_0x04));
            writer.WriteColNicify(nameof(Fmi.unk_0x05));
            writer.WriteColNicify(nameof(Fmi.unk_0x06));
            writer.WriteColNicify(nameof(Fmi.unk_0x07));
            writer.WriteColNicify(nameof(Fmi.unk_0x08));
        }

        public void WriteFmiHeader(StreamWriter writer, Fmi fmi)
        {
            writer.PushCol(fmi.FileName);
            writer.PushCol(fmi.unk_0x00);
            writer.PushCol(fmi.unk_0x01);
            writer.PushCol(fmi.animationCount);
            writer.PushCol(fmi.exhaustCount);
            writer.PushCol(fmi.unk_0x04);
            writer.PushCol(fmi.unk_0x05);
            writer.PushCol(fmi.unk_0x06);
            writer.PushCol(fmi.unk_0x07);
            writer.PushCol(fmi.unk_0x08);
        }

    }
}