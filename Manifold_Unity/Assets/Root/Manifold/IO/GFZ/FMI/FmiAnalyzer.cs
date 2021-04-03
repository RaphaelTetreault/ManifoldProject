using GameCube.GFZ;
using GameCube.GFZ.FMI;
using Manifold.IO.GFZ.FMI;
using System.IO;
using UnityEngine;

namespace Manifold.IO.GFZ.Camera
{
    [CreateAssetMenu(menuName = Const.Menu.GfzFMI + "FMI Analyzer")]
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
                writer.WriteNextCol("Index");
                writer.WriteColNicify(nameof(ExhaustAnimation.position));
                writer.WriteColNicify(nameof(ExhaustAnimation.unk_0x0C));
                writer.WriteColNicify(nameof(ExhaustAnimation.animType));
                writer.WriteNextRow();

                foreach (var sobj in analysisSobjs)
                {
                    var fmi = sobj.Value;
                    var idx = 0;
                    foreach (var anim in fmi.animations)
                    {
                        WriteFmiHeader(writer, fmi);
                        writer.WriteNextCol(idx++);
                        writer.WriteNextCol(anim.position);
                        writer.WriteNextCol(anim.unk_0x0C);
                        writer.WriteNextCol(anim.animType);
                        writer.WriteNextRow();
                    }
                }
            }
        }

        public void WriteFmiParticle(string filePath)
        {
            using (var writer = AnalyzerUtility.OpenWriter(filePath))
            {
                WriteFmiHeader(writer);
                writer.WriteNextCol("Index");
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
                writer.WriteNextRow();

                foreach (var sobj in analysisSobjs)
                {
                    var fmi = sobj.Value;
                    var idx = 0;
                    foreach (var particle in fmi.particles)
                    {
                        WriteFmiHeader(writer, fmi);
                        writer.WriteNextCol(idx++);
                        writer.WriteNextCol(particle.position);
                        writer.WriteNextCol(particle.unk_0x0C);
                        writer.WriteNextCol(particle.unk_0x10);
                        writer.WriteNextCol(particle.scaleMin);
                        writer.WriteNextCol(particle.scaleMax);
                        writer.WriteNextCol(particle.colorMin.r);
                        writer.WriteNextCol(particle.colorMin.g);
                        writer.WriteNextCol(particle.colorMin.b);
                        writer.WriteNextCol(particle.colorMin.a);
                        writer.WriteNextCol(particle.colorMax.r);
                        writer.WriteNextCol(particle.colorMax.g);
                        writer.WriteNextCol(particle.colorMax.b);
                        writer.WriteNextCol(particle.colorMax.a);
                        writer.WriteNextRow();
                    }
                }
            }
        }

        public void WriteFmiHeader(StreamWriter writer)
        {
            writer.WriteNextCol("File Name");
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
            writer.WriteNextCol(fmi.FileName);
            writer.WriteNextCol(fmi.unk_0x00);
            writer.WriteNextCol(fmi.unk_0x01);
            writer.WriteNextCol(fmi.animationCount);
            writer.WriteNextCol(fmi.exhaustCount);
            writer.WriteNextCol(fmi.unk_0x04);
            writer.WriteNextCol(fmi.unk_0x05);
            writer.WriteNextCol(fmi.unk_0x06);
            writer.WriteNextCol(fmi.unk_0x07);
            writer.WriteNextCol(fmi.unk_0x08);
        }

    }
}