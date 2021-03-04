using GameCube.GFZ;
using GameCube.GFZ.CourseCollision;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Manifold.IO.GFZ.CourseCollision
{
    [CreateAssetMenu(menuName = MenuConst.GfzCourseCollision + "COLI Analyzer")]
    public class ColiAnalyzer : ExecutableScriptableObject,
        IAnalyzable
    {
        #region MEMBERS

        [Header("Output")]
        [SerializeField, BrowseFolderField]
        protected string outputPath;
        [SerializeField, BrowseFolderField("Assets/"), Tooltip("Used with IOOption.allFromSourceFolder")]
        protected string[] searchFolders;

        [SerializeField]
        protected bool
            topologyParameters = true,
            trackTransform = true,
            gameObjects = true,
            animations = true,
            headers = true;

        [Header("Preferences")]
        [SerializeField]
        protected bool openFolderAfterAnalysis = true;

        [Header("Analysis Options")]
        [SerializeField]
        protected IOOption analysisOption = IOOption.allFromAssetDatabase;
        [SerializeField]
        protected ColiSceneSobj[] coliSobjs;

        #endregion

        #region PROPERTIES

        public override string ExecuteText => "Analyze COLI";

        #endregion

        public void Analyze()
        {
            coliSobjs = AssetDatabaseUtility.GetSobjByOption(coliSobjs, analysisOption, searchFolders);
            var time = AnalyzerUtility.GetFileTimestamp();

            // TOPOLOGY PARAMETERS
            if (topologyParameters)
            {
                var count = TopologyParameters.kFieldCount;
                for (int i = 0; i < count; i++)
                {
                    var filePath = $"{time} COLI TopologyParameters {i + 1}.tsv";
                    filePath = Path.Combine(outputPath, filePath);
                    EditorUtility.DisplayProgressBar(ExecuteText, filePath, (float)(i + 1) / TopologyParameters.kFieldCount);
                    AnalyzeTrackData(filePath, i);
                }
            }

            // TRACK TRANSFORMS
            if (trackTransform)
            {
                var filePath = $"{time} COLI TrackTransforms.tsv";
                filePath = Path.Combine(outputPath, filePath);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, .5f);
                AnalyzeTransforms(filePath);
            }

            //GAME OBJECTS
            if (gameObjects)
            {
                string filePath = string.Empty;

                filePath = $"{time} COLI GameObjects.tsv";
                filePath = Path.Combine(outputPath, filePath);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, .5f);
                AnalyzeGameObjects(filePath);

                filePath = $"{time} COLI GameObjects Unk1.tsv";
                filePath = Path.Combine(outputPath, filePath);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, .5f);
                AnalyzeGameObjectsUnk1(filePath);

                filePath = $"{time} COLI GameObjects Skeletal.tsv";
                filePath = Path.Combine(outputPath, filePath);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, .5f);
                AnalyzeGameObjectsUnk2(filePath);

                filePath = $"{time} COLI GameObjects Collision Tris.tsv";
                filePath = Path.Combine(outputPath, filePath);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, .5f);
                AnalyzeGameObjectsCollisionTri(filePath);

                filePath = $"{time} COLI GameObjects Collision Quads.tsv";
                filePath = Path.Combine(outputPath, filePath);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, .5f);
                AnalyzeGameObjectsCollisionQuad(filePath);
            }

            //
            if (headers)
            {
                string filePath = string.Empty;

                filePath = $"{time} COLI Headers.tsv";
                filePath = Path.Combine(outputPath, filePath);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, .5f);
                AnalyzeHeaders(filePath);
            }

            //
            if (animations)
            {
                // ANIMATIONS
                {
                    var filePath = $"{time} COLI Animations.tsv";
                    filePath = Path.Combine(outputPath, filePath);
                    EditorUtility.DisplayProgressBar(ExecuteText, filePath, .5f);
                    AnalyzeGameObjectAnimations(filePath);
                }

                // ANIMATIONS INDIVIDUALIZED
                {
                    var count = GameCube.GFZ.CourseCollision.AnimationClip.kSizeCurvesPtrs;
                    for (int i = 0; i < count; i++)
                    {
                        var filePath = $"{time} COLI Animations {i}.tsv";
                        filePath = Path.Combine(outputPath, filePath);
                        EditorUtility.DisplayProgressBar(ExecuteText, filePath, (float)(i + 1) / count);
                        AnalyzeGameObjectAnimationsIndex(filePath, i);
                    }
                }
            }


            // OPEN FOLDER after analysis
            if (openFolderAfterAnalysis)
            {
                OSUtility.OpenDirectory(outputPath + "/");
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
                foreach (var sobj in coliSobjs)
                {
                    // foreach Transform
                    foreach (var trackTransform in sobj.Value.trackTransforms)
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

                foreach (var sobj in coliSobjs)
                {
                    var index = 0;
                    var total = sobj.Value.trackTransforms.Count;
                    foreach (var trackTransform in sobj.Value.trackTransforms)
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

                foreach (var file in coliSobjs)
                {
                    int gameObjectIndex = 0;
                    foreach (var gameObject in file.Value.gameObjects)
                    {
                        if (gameObject.animation == null)
                            continue;

                        int animIndex = 0;
                        foreach (var animationCurve in gameObject.animation.animCurves)
                        {
                            foreach (var keyable in animationCurve.keyableAttributes)
                            {
                                writer.PushCol(file.FileName);
                                writer.PushCol(gameObjectIndex);
                                writer.PushCol(gameObject.name);
                                writer.PushCol($"0x{animationCurve.StartAddress:X8}");
                                writer.PushCol($"0x{keyable.StartAddress:X8}");
                                writer.PushCol(animIndex);
                                writer.PushCol(keyable.easeMode);
                                writer.PushCol(keyable.time);
                                writer.PushCol(keyable.value);
                                writer.PushCol(keyable.zTangentIn);
                                writer.PushCol(keyable.zTangentOut);
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

                foreach (var file in coliSobjs)
                {
                    int gameObjectIndex = 0;
                    foreach (var gameObject in file.Value.gameObjects)
                    {
                        int animIndex = 0;
                        foreach (var animationCurve in gameObject.animation.animCurves)
                        {
                            foreach (var keyable in animationCurve.keyableAttributes)
                            {
                                /// HACK, write each anim index as separate file
                                if (animIndex != index)
                                    continue;

                                writer.PushCol(file.FileName);
                                writer.PushCol(gameObjectIndex);
                                writer.PushCol(gameObject.name);
                                writer.PushCol($"0x{animationCurve.StartAddress:X8}");
                                writer.PushCol($"0x{keyable.StartAddress:X8}");
                                writer.PushCol(animIndex);
                                writer.PushCol(keyable.easeMode);
                                writer.PushCol(keyable.time);
                                writer.PushCol(keyable.value);
                                writer.PushCol(keyable.zTangentIn);
                                writer.PushCol(keyable.zTangentOut);
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

        #region GameObjects

        public void AnalyzeGameObjects(string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.PushCol("File");
                writer.PushCol("Game Object #");
                writer.PushCol("Game Object");
                writer.PushCol("unk_0x00 enum");
                writer.WriteFlagNames<EnumFlags32>();
                writer.PushCol("unk_0x04");
                writer.WriteFlagNames<EnumFlags32>();
                writer.PushCol("collisionBindingAbsPtr");
                writer.PushCol("collisionPosition");
                writer.PushCol("unk_0x18 enum");
                writer.WriteFlagNames<EnumFlags16>();
                writer.PushCol("unk_0x1A enum");
                writer.WriteFlagNames<EnumFlags16>();
                writer.PushCol("unk_0x1C enum");
                writer.WriteFlagNames<EnumFlags16>();
                writer.PushCol("unk_0x1E enum");
                writer.WriteFlagNames<EnumFlags16>();
                writer.PushCol("unk_0x1C");
                writer.PushCol("collisionScale");
                writer.PushCol("zero_0x2C");
                writer.PushCol("animationAbsPtr");
                writer.PushCol("unkPtr_0x34");
                writer.PushCol("unkPtr_0x38");
                writer.PushCol("transformPtr");
                writer.PushRow();

                foreach (var file in coliSobjs)
                {
                    int gameObjectIndex = 0;
                    foreach (var gameObject in file.Value.gameObjects)
                    {
                        writer.PushCol(file.FileName);
                        writer.PushCol(gameObjectIndex);
                        writer.PushCol(gameObject.name);
                        writer.PushCol((int)gameObject.unk_0x00);
                        writer.WriteFlags(gameObject.unk_0x00);
                        writer.PushCol((int)gameObject.unk_0x04);
                        writer.WriteFlags(gameObject.unk_0x04);
                        writer.PushCol("0x" + gameObject.collisionBindingAbsPtr.ToString("X"));
                        writer.PushCol(gameObject.position);
                        writer.PushCol((int)gameObject.unk_0x18);
                        writer.WriteFlags(gameObject.unk_0x18);
                        writer.PushCol((int)gameObject.unk_0x1A);
                        writer.WriteFlags(gameObject.unk_0x1A);
                        writer.PushCol((int)gameObject.unk_0x1C);
                        writer.WriteFlags(gameObject.unk_0x1C);
                        writer.PushCol((int)gameObject.unk_0x1E);
                        writer.WriteFlags(gameObject.unk_0x1E);
                        writer.PushCol(gameObject.scale);
                        writer.PushCol(gameObject.zero_0x2C);
                        writer.PushCol("0x" + gameObject.animationAbsPtr.ToString("X"));
                        writer.PushCol("0x" + gameObject.unkAbsPtr_0x34.ToString("X"));
                        writer.PushCol("0x" + gameObject.skeletalAnimatorAbsPtr.ToString("X"));
                        writer.PushCol("0x" + gameObject.transformAbsPtr.ToString("X"));
                        writer.PushRow();

                        gameObjectIndex++;
                    }
                }
                writer.Flush();
            }
        }

        public void AnalyzeGameObjectsUnk1(string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.PushCol("File");
                writer.PushCol("Game Object #");
                writer.PushCol("Game Object");
                writer.PushCol("Unknown 1 Index");
                writer.WriteColNicify(nameof(ObjectTable_Unk1_Entry.unk_0x00));
                writer.WriteColNicify(nameof(ObjectTable_Unk1_Entry.unk_0x04));
                writer.PushRow();

                foreach (var file in coliSobjs)
                {
                    int gameObjectIndex = 0;
                    foreach (var gameObject in file.Value.gameObjects)
                    {
                        int unkIndex = 0;
                        foreach (var unk1 in gameObject.unk1.unk)
                        {
                            writer.PushCol(file.FileName);
                            writer.PushCol(gameObjectIndex);
                            writer.PushCol(gameObject.name);
                            writer.PushCol(unkIndex);
                            writer.PushCol(unk1.unk_0x00);
                            writer.PushCol(unk1.unk_0x04);
                            writer.PushRow();
                            unkIndex++;
                        }
                        gameObjectIndex++;
                    }
                }
                writer.Flush();
            }
        }

        public void AnalyzeGameObjectsUnk2(string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.PushCol("File");
                writer.PushCol("Game Object #");
                writer.PushCol("Game Object");

                writer.WriteColNicify(nameof(SkeletalAnimator.zero_0x00));
                writer.WriteColNicify(nameof(SkeletalAnimator.zero_0x04));
                writer.WriteColNicify(nameof(SkeletalAnimator.one_0x08));
                writer.WriteColNicify(nameof(SkeletalAnimator.unkRelPtr));

                writer.WriteColNicify(nameof(SkeletalProperties.unk_0x00));
                writer.WriteColNicify(nameof(SkeletalProperties.unk_0x04));
                writer.WriteFlagNames<EnumFlags32>();
                writer.WriteColNicify(nameof(SkeletalProperties.unk_0x08));
                writer.WriteFlagNames<EnumFlags32>();
                writer.WriteColNicify(nameof(SkeletalProperties.zero_0x0C));
                writer.WriteColNicify(nameof(SkeletalProperties.zero_0x10));
                writer.WriteColNicify(nameof(SkeletalProperties.zero_0x14));
                writer.WriteColNicify(nameof(SkeletalProperties.zero_0x18));
                writer.PushRow();

                foreach (var file in coliSobjs)
                {
                    int gameObjectIndex = 0;
                    foreach (var gameObject in file.Value.gameObjects)
                    {
                        if (gameObject.skeletalAnimator.unkRelPtr == 0)
                        {
                            continue;
                        }

                        writer.PushCol(file.FileName);
                        writer.PushCol(gameObjectIndex);
                        writer.PushCol(gameObject.name);

                        writer.PushCol(gameObject.skeletalAnimator.zero_0x00);
                        writer.PushCol(gameObject.skeletalAnimator.zero_0x04);
                        writer.PushCol(gameObject.skeletalAnimator.one_0x08);
                        writer.PushCol(gameObject.skeletalAnimator.unkRelPtr);

                        writer.PushCol(gameObject.skeletalAnimator.properties.unk_0x00);
                        writer.PushCol(gameObject.skeletalAnimator.properties.unk_0x04);
                        writer.WriteFlags(gameObject.skeletalAnimator.properties.unk_0x04);
                        writer.PushCol(gameObject.skeletalAnimator.properties.unk_0x08);
                        writer.WriteFlags(gameObject.skeletalAnimator.properties.unk_0x08);
                        writer.PushCol(gameObject.skeletalAnimator.properties.zero_0x0C);
                        writer.PushCol(gameObject.skeletalAnimator.properties.zero_0x10);
                        writer.PushCol(gameObject.skeletalAnimator.properties.zero_0x14);
                        writer.PushCol(gameObject.skeletalAnimator.properties.zero_0x18);
                        writer.PushRow();

                        gameObjectIndex++;
                    }
                }
                writer.Flush();
            }
        }

        public void AnalyzeGameObjectsCollisionTri(string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.PushCol("File");
                writer.PushCol("Game Object #");
                writer.PushCol("Game Object");

                writer.PushCol("Tri Index");
                writer.PushCol("Addr");

                writer.WriteColNicify(nameof(CollisionTri.unk_0x00));
                writer.WriteColNicify(nameof(CollisionTri.normal));
                writer.WriteColNicify(nameof(CollisionTri.vertex0));
                writer.WriteColNicify(nameof(CollisionTri.vertex1));
                writer.WriteColNicify(nameof(CollisionTri.vertex2));
                writer.WriteColNicify(nameof(CollisionTri.precomputed0));
                writer.WriteColNicify(nameof(CollisionTri.precomputed1));
                writer.WriteColNicify(nameof(CollisionTri.precomputed2));

                writer.PushRow();

                foreach (var file in coliSobjs)
                {
                    int gameObjectIndex = 0;
                    foreach (var gameObject in file.Value.gameObjects)
                    {
                        if (gameObject.collisionBinding.collision.triCount == 0)
                        {
                            continue;
                        }

                        int triIndex = 0;
                        foreach (var tri in gameObject.collisionBinding.collision.tris)
                        {
                            writer.PushCol(file.FileName);
                            writer.PushCol(gameObjectIndex);
                            writer.PushCol(gameObject.name);

                            writer.PushCol(triIndex++);
                            writer.WriteStartAddress(tri);

                            writer.PushCol(tri.unk_0x00);
                            writer.PushCol(tri.normal);
                            writer.PushCol(tri.vertex0);
                            writer.PushCol(tri.vertex1);
                            writer.PushCol(tri.vertex2);
                            writer.PushCol(tri.precomputed0);
                            writer.PushCol(tri.precomputed1);
                            writer.PushCol(tri.precomputed2);

                            writer.PushRow();
                        }
                        gameObjectIndex++;
                    }
                }
                writer.Flush();
            }
        }

        public void AnalyzeGameObjectsCollisionQuad(string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.PushCol("File");
                writer.PushCol("Game Object #");
                writer.PushCol("Game Object");

                writer.PushCol("Quad Index");
                writer.PushCol("Addr");

                writer.WriteColNicify(nameof(CollisionQuad.unk_0x00));
                writer.WriteColNicify(nameof(CollisionQuad.normal));
                writer.WriteColNicify(nameof(CollisionQuad.vertex0));
                writer.WriteColNicify(nameof(CollisionQuad.vertex1));
                writer.WriteColNicify(nameof(CollisionQuad.vertex2));
                writer.WriteColNicify(nameof(CollisionQuad.vertex3));
                writer.WriteColNicify(nameof(CollisionQuad.precomputed0));
                writer.WriteColNicify(nameof(CollisionQuad.precomputed1));
                writer.WriteColNicify(nameof(CollisionQuad.precomputed2));
                writer.WriteColNicify(nameof(CollisionQuad.precomputed3));

                writer.PushRow();

                foreach (var file in coliSobjs)
                {
                    int gameObjectIndex = 0;
                    foreach (var gameObject in file.Value.gameObjects)
                    {
                        if (gameObject.collisionBinding.collision.quadCount == 0)
                        {
                            continue;
                        }

                        int quadIndex = 0;
                        foreach (var quad in gameObject.collisionBinding.collision.quads)
                        {
                            writer.PushCol(file.FileName);
                            writer.PushCol(gameObjectIndex);
                            writer.PushCol(gameObject.name);

                            writer.PushCol(quadIndex++);
                            writer.WriteStartAddress(quad);

                            writer.PushCol(quad.unk_0x00);
                            writer.PushCol(quad.normal);
                            writer.PushCol(quad.vertex0);
                            writer.PushCol(quad.vertex1);
                            writer.PushCol(quad.vertex2);
                            writer.PushCol(quad.vertex3);
                            writer.PushCol(quad.precomputed0);
                            writer.PushCol(quad.precomputed1);
                            writer.PushCol(quad.precomputed2);
                            writer.PushCol(quad.precomputed3);

                            writer.PushRow();
                        }
                        gameObjectIndex++;
                    }
                }
                writer.Flush();
            }
        }

        #endregion


        public void AnalyzeHeaders(string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.PushCol("File");
                writer.PushCol("Index");
                writer.PushCol("Stage");
                writer.PushCol("Venue");
                writer.PushCol("AX/GX");
                //
                writer.PushCol(nameof(Header.unk_0x00));
                writer.PushCol(nameof(Header.unk_0x04));
                writer.PushCol(nameof(Header.trackNodesPtr));
                writer.PushCol(nameof(Header.trackNodesPtr.address));
                writer.PushCol(nameof(Header.collisionEffectsAreasPtr));
                writer.PushCol(nameof(Header.collisionEffectsAreasPtr.address));
                writer.PushCol(nameof(Header.boostPadEnable));
                writer.PushCol(nameof(Header.collisionMeshTablePtr));
                writer.PushCol(nameof(Header.unkPtr_0x20));
                writer.PushCol(nameof(Header.unkPtr_0x24));
                writer.PushCol(nameof(Header.zero_0x28));
                writer.PushCol(nameof(Header.gameObjectCount));
                writer.PushCol(nameof(Header.unk_gameObjectCount1));
                writer.PushCol(nameof(Header.unk_gameObjectCount2));
                writer.PushCol(nameof(Header.gameObjectPtr));
                writer.PushCol(nameof(Header.unkBool32_0x58));
                writer.PushCol(nameof(Header.unkArrayPtr_0x5C));
                writer.PushCol(nameof(Header.unkArrayPtr_0x5C.address));
                writer.PushCol(nameof(Header.collisionObjectsMesh));
                writer.PushCol(nameof(Header.collisionObjectsMesh.address));
                writer.PushCol(nameof(Header.unkArrayPtr_0x6C));
                writer.PushCol(nameof(Header.unkArrayPtr_0x6C.address));
                writer.PushCol(nameof(Header.unused_0x74_0x78));
                writer.PushCol(nameof(Header.unused_0x74_0x78.address));
                writer.PushCol(nameof(Header.circuitType));
                writer.PushCol(nameof(Header.unkPtr_0x80));
                writer.PushCol(nameof(Header.unkPtr_0x84));
                writer.PushCol(nameof(Header.unused_0x88_0x8C));
                writer.PushCol(nameof(Header.unused_0x88_0x8C.address));
                writer.PushCol(nameof(Header.trackInfo));
                writer.PushCol(nameof(Header.unkArrayPtr_0x94));
                writer.PushCol(nameof(Header.unkArrayPtr_0x94.address));
                writer.PushCol(nameof(Header.unkArrayPtr_0x9C));
                writer.PushCol(nameof(Header.unkArrayPtr_0x9C.address));
                writer.PushCol(nameof(Header.pathObjects));
                writer.PushCol(nameof(Header.pathObjects.address));
                writer.PushCol(nameof(Header.arcadeCheckpoint));
                writer.PushCol(nameof(Header.arcadeCheckpoint.address));
                writer.PushCol(nameof(Header.storyModeSpecialObjects));
                writer.PushCol(nameof(Header.storyModeSpecialObjects.address));
                writer.PushCol(nameof(Header.trackNodeTable));
                // Structure
                writer.PushCol(nameof(Header.unknownStructure1_0xC0) + "." + nameof(Header.unknownStructure1_0xC0.unk_0x00));
                writer.PushCol(nameof(Header.unknownStructure1_0xC0) + "." + nameof(Header.unknownStructure1_0xC0.unk_0x04));
                writer.PushCol(nameof(Header.unknownStructure1_0xC0) + "." + nameof(Header.unknownStructure1_0xC0.unk_0x08));
                writer.PushCol(nameof(Header.unknownStructure1_0xC0) + "." + nameof(Header.unknownStructure1_0xC0.unk_0x0C));
                writer.PushCol(nameof(Header.unknownStructure1_0xC0) + "." + nameof(Header.unknownStructure1_0xC0.unk_0x10));
                writer.PushCol(nameof(Header.unknownStructure1_0xC0) + "." + nameof(Header.unknownStructure1_0xC0.unk_0x14));
                // 
                writer.PushCol(nameof(Header.zero_0xD8));
                writer.PushRow();

                foreach (var file in coliSobjs)
                {
                    var coli = file.Value;
                    var coliHeader = file.Value.header;

                    writer.PushCol(coli.FileName);
                    writer.PushCol(coli.id);
                    writer.PushCol(CourseUtility.GetVenueID(coli.id).GetDescription());
                    writer.PushCol(((CourseIDEx)coli.id).GetDescription());
                    writer.PushCol(coliHeader.IsFileGX ? "GX" : "AX");

                    writer.PushCol(coliHeader.unk_0x00);
                    writer.PushCol(coliHeader.unk_0x04);
                    writer.PushCol(coliHeader.trackNodesPtr.length);
                    writer.PushCol(coliHeader.trackNodesPtr.HexAddress);
                    writer.PushCol(coliHeader.collisionEffectsAreasPtr.length);
                    writer.PushCol(coliHeader.collisionEffectsAreasPtr.HexAddress);
                    writer.PushCol(coliHeader.boostPadEnable);
                    writer.PushCol(coliHeader.collisionMeshTablePtr.HexAddress);
                    writer.PushCol(coliHeader.unkPtr_0x20.HexAddress);
                    writer.PushCol(coliHeader.unkPtr_0x24.HexAddress);
                    writer.PushCol(0);// coliHeader.zero_0x28);
                    writer.PushCol(coliHeader.gameObjectCount);
                    if (coliHeader.IsFileGX)
                    {
                        writer.PushCol(coliHeader.unk_gameObjectCount1);
                    }
                    else // is AX
                    {
                        writer.PushCol();
                    }
                    writer.PushCol(coliHeader.unk_gameObjectCount2);
                    writer.PushCol(coliHeader.gameObjectPtr.HexAddress);
                    writer.PushCol(coliHeader.unkBool32_0x58);
                    writer.PushCol(coliHeader.unkArrayPtr_0x5C.length);
                    writer.PushCol(coliHeader.unkArrayPtr_0x5C.HexAddress);
                    writer.PushCol(coliHeader.collisionObjectsMesh.length);
                    writer.PushCol(coliHeader.collisionObjectsMesh.HexAddress);
                    writer.PushCol(coliHeader.unkArrayPtr_0x6C.length);
                    writer.PushCol(coliHeader.unkArrayPtr_0x6C.HexAddress);
                    writer.PushCol(coliHeader.unused_0x74_0x78.length);
                    writer.PushCol(coliHeader.unused_0x74_0x78.HexAddress);
                    writer.PushCol(coliHeader.circuitType);
                    writer.PushCol(coliHeader.unkPtr_0x80.HexAddress);
                    writer.PushCol(coliHeader.unkPtr_0x84.HexAddress);
                    writer.PushCol(coliHeader.unused_0x88_0x8C.length);
                    writer.PushCol(coliHeader.unused_0x88_0x8C.HexAddress);
                    writer.PushCol(coliHeader.trackInfo.HexAddress);
                    writer.PushCol(coliHeader.unkArrayPtr_0x94.length);
                    writer.PushCol(coliHeader.unkArrayPtr_0x94.HexAddress);
                    writer.PushCol(coliHeader.unkArrayPtr_0x9C.length);
                    writer.PushCol(coliHeader.unkArrayPtr_0x9C.HexAddress);
                    writer.PushCol(coliHeader.pathObjects.length);
                    writer.PushCol(coliHeader.pathObjects.HexAddress);
                    writer.PushCol(coliHeader.arcadeCheckpoint.length);
                    writer.PushCol(coliHeader.arcadeCheckpoint.HexAddress);
                    writer.PushCol(coliHeader.storyModeSpecialObjects.length);
                    writer.PushCol(coliHeader.storyModeSpecialObjects.HexAddress);
                    writer.PushCol(coliHeader.trackNodeTable.HexAddress);
                    // Structure
                    writer.PushCol(coliHeader.unknownStructure1_0xC0.unk_0x00);
                    writer.PushCol(coliHeader.unknownStructure1_0xC0.unk_0x04);
                    writer.PushCol(coliHeader.unknownStructure1_0xC0.unk_0x08);
                    writer.PushCol(coliHeader.unknownStructure1_0xC0.unk_0x0C);
                    writer.PushCol(coliHeader.unknownStructure1_0xC0.unk_0x10);
                    writer.PushCol(coliHeader.unknownStructure1_0xC0.unk_0x14);
                    //
                    writer.PushCol(0);// coliHeader.zero_0xD8);
                    writer.PushRow();
                }
                writer.Flush();
            }
        }
    }
}