using GameCube.GFZ;
using GameCube.GFZ.CourseCollision;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Manifold.IO.GFZ.CourseCollision
{
    [CreateAssetMenu(menuName = Const.Menu.GfzCourseCollision + "COLI Analyzer")]
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
            animations = true,
            coliUnk5 = true,
            headers = true,
            sceneObjects = true,
            storyObjectTriggers = true,
            topologyParameters = true,
            trackTransform = true,
            transformsComparison = true,
            unknownAnimationData = true,
            unknownTrigger1 = true,
            venueMetadataObject = true;

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

            //
            if (transformsComparison)
            {
                string fileName = $"{time} COLI Comapre Transforms.tsv";
                string filePath = Path.Combine(outputPath, fileName);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, .5f);
                AnalyzeSceneObjectTransforms(filePath);
            }

            if (coliUnk5)
            {
                string fileName = $"{time} COLI {nameof(UnknownStageData1)}.tsv";
                string filePath = Path.Combine(outputPath, fileName);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, .5f);
                AnalyzeUnknownStageData1(filePath);
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

            if (storyObjectTriggers)
            {
                string fileName = $"{time} COLI {nameof(StoryObjectTrigger)}.tsv";
                string filePath = Path.Combine(outputPath, fileName);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, .5f);
                AnalyzeStoryObjectTrigger(filePath);
            }

            //GAME OBJECTS
            if (sceneObjects)
            {
                string filePath = string.Empty;

                filePath = $"{time} COLI {nameof(SceneObject)}.tsv";
                filePath = Path.Combine(outputPath, filePath);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, .5f);
                AnalyzeGameObjects(filePath);

                filePath = $"{time} COLI {nameof(SceneObject)} {nameof(UnknownSceneObjectData)}.tsv";
                filePath = Path.Combine(outputPath, filePath);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, .5f);
                AnalyzeGameObjectsUnk1(filePath);

                filePath = $"{time} COLI {nameof(SceneObject)} {nameof(SkeletalAnimator)}.tsv";
                filePath = Path.Combine(outputPath, filePath);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, .5f);
                AnalyzeGameObjectsSkeletalAnimator(filePath);

                filePath = $"{time} COLI {nameof(SceneObject)}  {nameof(ColliderTriangle)}.tsv";
                filePath = Path.Combine(outputPath, filePath);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, .5f);
                AnalyzeGameObjectsCollisionTri(filePath);

                filePath = $"{time} COLI {nameof(SceneObject)} {nameof(ColliderQuad)}.tsv";
                filePath = Path.Combine(outputPath, filePath);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, .5f);
                AnalyzeGameObjectsCollisionQuad(filePath);
            }

            // TOPOLOGY PARAMETERS
            if (topologyParameters)
            {
                var count = TopologyParameters.kFieldCount;
                for (int i = 0; i < count; i++)
                {
                    var filePath = $"{time} COLI {nameof(TopologyParameters)} {i + 1}.tsv";
                    filePath = Path.Combine(outputPath, filePath);
                    EditorUtility.DisplayProgressBar(ExecuteText, filePath, (float)(i + 1) / TopologyParameters.kFieldCount);
                    AnalyzeTrackData(filePath, i);
                }
            }

            // TRACK TRANSFORMS
            if (trackTransform)
            {
                var filePath = $"{time} COLI {nameof(TrackTransform)}.tsv";
                filePath = Path.Combine(outputPath, filePath);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, .5f);
                AnalyzeTransforms(filePath);
            }

            if (unknownAnimationData)
            {
                string fileName = $"{time} COLI {nameof(UnknownStageData2)}.tsv";
                string filePath = Path.Combine(outputPath, fileName);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, .5f);
                AnalyzeUnknownAnimationData(filePath);
            }

            if (unknownTrigger1)
            {
                string fileName = $"{time} COLI {nameof(UnknownTrigger1)}.tsv";
                string filePath = Path.Combine(outputPath, fileName);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, .5f);
                AnalyzeUnknownTrigger1(filePath);
            }

            if (venueMetadataObject)
            {
                string fileName = $"{time} COLI {nameof(StoryObjectTrigger)}.tsv";
                string filePath = Path.Combine(outputPath, fileName);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, .5f);
                AnalyzeStoryObjectTrigger(filePath);
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
                writer.WriteNextCol("File Path");
                writer.WriteNextCol("Track Node Index");
                writer.WriteNextCol($"Param [{paramIndex + 1}] Index");
                writer.WriteNextCol("Address");
                writer.WriteNextCol("unk_0x00");
                writer.WriteNextCol("unk_0x04");
                writer.WriteNextCol("unk_0x08");
                writer.WriteNextCol("unk_0x0C");
                writer.WriteNextCol("unk_0x10");
                writer.WriteNextRow();

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

        public void WriteTrackData(StreamWriter writer, ColiSceneSobj sobj, int level, int index, int total, KeyableAttribute param)
        {
            writer.WriteNextCol(sobj.FilePath);
            writer.WriteNextCol($"[{index}/{total}]");
            writer.WriteNextCol($"{level}");
            writer.WriteNextCol(param.StartAddressHex());
            writer.WriteNextCol(param.easeMode);
            writer.WriteNextCol(param.time);
            writer.WriteNextCol(param.value);
            writer.WriteNextCol(param.zTangentIn);
            writer.WriteNextCol(param.zTangentOut);
            writer.WriteNextRow();
        }


        public void AnalyzeTransforms(string filename)
        {
            using (var writer = AnalyzerUtility.OpenWriter(filename))
            {
                writer.WriteNextCol("File Path");
                writer.WriteNextCol("Root Index");
                writer.WriteNextCol("Transform Depth");
                writer.WriteNextCol("Address");
                writer.WriteNextCol("Hierarchy Depth");
                writer.WriteNextCol("zero_0x01");
                writer.WriteNextCol("hasChildren");
                writer.WriteNextCol("zero_0x03");
                writer.WriteNextCol("topologyParamsAbsPtr");
                writer.WriteNextCol("zero_0x08");
                writer.WriteNextCol("Child Count");
                writer.WriteNextCol("Children Abs Ptr");
                writer.WriteNextCol("localScale");
                writer.WriteNextCol("localRotation");
                writer.WriteNextCol("localPosition");
                writer.WriteNextCol("unk_0x38");
                writer.WriteNextCol("unk_0x3C");
                writer.WriteNextCol("unk_0x40");
                writer.WriteNextCol("unk_0x44");
                writer.WriteNextCol("unk_0x48");
                writer.WriteNextCol("unk_0x4C");
                writer.WriteNextRow();

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
            writer.WriteNextCol(sobj.FilePath);
            writer.WriteNextCol($"[{index}/{total}]");
            writer.WriteNextCol($"{level}");
            writer.WriteNextCol(trackTransform.StartAddressHex());
            writer.WriteNextCol(trackTransform.hierarchyDepth);
            writer.WriteNextCol(trackTransform.zero_0x01);
            writer.WriteNextCol(trackTransform.hasChildren);
            writer.WriteNextCol(trackTransform.zero_0x03);
            writer.WriteNextCol("0x" + trackTransform.topologyParamsAbsPtr.ToString("X8"));
            writer.WriteNextCol("0x" + trackTransform.unk_0x08_absPtr.ToString("X8"));
            writer.WriteNextCol(trackTransform.childCount);
            writer.WriteNextCol("0x" + trackTransform.childrenAbsPtr.ToString("X8"));
            writer.WriteNextCol(trackTransform.localScale);
            writer.WriteNextCol(trackTransform.localRotation);
            writer.WriteNextCol(trackTransform.localPosition);
            writer.WriteNextCol(trackTransform.unk_0x38);
            writer.WriteNextCol(trackTransform.unk_0x3C);
            writer.WriteNextCol(trackTransform.unk_0x40);
            writer.WriteNextCol(trackTransform.zero_0x44);
            writer.WriteNextCol(trackTransform.zero_0x48);
            writer.WriteNextCol(trackTransform.unk_0x4C);
            writer.WriteNextRow();
        }

        #endregion

        #region GameObject Animations

        public void AnalyzeGameObjectAnimations(string filename)
        {
            using (var writer = AnalyzerUtility.OpenWriter(filename))
            {
                // Write header
                writer.WriteNextCol("File Path");
                writer.WriteNextCol("Game Object #");
                writer.WriteNextCol("Game Object");
                writer.WriteNextCol("Anim Addr");
                writer.WriteNextCol("Key Addr");
                writer.WriteNextCol("Anim Index [0-10]");
                writer.WriteNextCol("Unk_0x00");
                writer.WriteNextCol("Time");
                writer.WriteNextCol("Value");
                writer.WriteNextCol("Unk_0x0C");
                writer.WriteNextCol("Unk_0x10");
                writer.WriteNextRow();

                foreach (var file in coliSobjs)
                {
                    int gameObjectIndex = 0;
                    foreach (var gameObject in file.Value.sceneObjects)
                    {
                        if (gameObject.animation == null)
                            continue;

                        int animIndex = 0;
                        foreach (var animationCurve in gameObject.animation.animCurves)
                        {
                            foreach (var keyable in animationCurve.keyableAttributes)
                            {
                                writer.WriteNextCol(file.FileName);
                                writer.WriteNextCol(gameObjectIndex);
                                writer.WriteNextCol(gameObject.name);
                                writer.WriteNextCol(animationCurve.StartAddressHex());
                                writer.WriteNextCol(keyable.StartAddressHex());
                                writer.WriteNextCol(animIndex);
                                writer.WriteNextCol(keyable.easeMode);
                                writer.WriteNextCol(keyable.time);
                                writer.WriteNextCol(keyable.value);
                                writer.WriteNextCol(keyable.zTangentIn);
                                writer.WriteNextCol(keyable.zTangentOut);
                                writer.WriteNextRow();
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
                writer.WriteNextCol("File Path");
                writer.WriteNextCol("Game Object #");
                writer.WriteNextCol("Game Object");
                writer.WriteNextCol("Anim Addr");
                writer.WriteNextCol("Key Addr");
                writer.WriteNextCol("Anim Index [0-10]");
                writer.WriteNextCol("Unk_0x00");
                writer.WriteNextCol("Time");
                writer.WriteNextCol("Value");
                writer.WriteNextCol("Unk_0x0C");
                writer.WriteNextCol("Unk_0x10");
                writer.WriteNextRow();

                foreach (var file in coliSobjs)
                {
                    int gameObjectIndex = 0;
                    foreach (var gameObject in file.Value.sceneObjects)
                    {
                        int animIndex = 0;
                        foreach (var animationCurve in gameObject.animation.animCurves)
                        {
                            foreach (var keyable in animationCurve.keyableAttributes)
                            {
                                /// HACK, write each anim index as separate file
                                if (animIndex != index)
                                    continue;

                                writer.WriteNextCol(file.FileName);
                                writer.WriteNextCol(gameObjectIndex);
                                writer.WriteNextCol(gameObject.name);
                                writer.WriteNextCol(animationCurve.StartAddressHex());
                                writer.WriteNextCol(keyable.StartAddressHex());
                                writer.WriteNextCol(animIndex);
                                writer.WriteNextCol(keyable.easeMode);
                                writer.WriteNextCol(keyable.time);
                                writer.WriteNextCol(keyable.value);
                                writer.WriteNextCol(keyable.zTangentIn);
                                writer.WriteNextCol(keyable.zTangentOut);
                                writer.WriteNextRow();
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
                writer.WriteNextCol("File");
                writer.WriteNextCol("Game Object #");
                writer.WriteNextCol("Game Object");
                writer.WriteNextCol(nameof(SceneObject.unk_0x00));
                writer.WriteFlagNames<EnumFlags32>();
                writer.WriteNextCol(nameof(SceneObject.unk_0x04));
                writer.WriteFlagNames<EnumFlags32>();
                writer.WriteNextCol(nameof(SceneObject.collisionBindingPtr));
                writer.WriteNextCol(nameof(SceneObject.transform.Position));
                writer.WriteNextCol(nameof(SceneObject.transform.RotationEuler));
                writer.WriteNextCol(nameof(SceneObject.transform.Scale));
                writer.WriteNextCol(nameof(SceneObject.zero_0x2C));
                writer.WriteNextCol(nameof(SceneObject.animationPtr));
                writer.WriteNextCol(nameof(SceneObject.unkPtr_0x34));
                writer.WriteNextCol(nameof(SceneObject.skeletalAnimatorPtr));
                writer.WriteNextCol(nameof(SceneObject.transformPtr));
                writer.WriteNextRow();

                foreach (var file in coliSobjs)
                {
                    int sceneObjectIndex = 0;
                    foreach (var sceneObject in file.Value.sceneObjects)
                    {
                        writer.WriteNextCol(file.FileName);
                        writer.WriteNextCol(sceneObjectIndex);
                        writer.WriteNextCol(sceneObject.name);
                        writer.WriteNextCol((int)sceneObject.unk_0x00);
                        writer.WriteFlags(sceneObject.unk_0x00);
                        writer.WriteNextCol((int)sceneObject.unk_0x04);
                        writer.WriteFlags(sceneObject.unk_0x04);
                        writer.WriteNextCol(sceneObject.collisionBindingPtr.HexAddress);
                        writer.WriteNextCol(sceneObject.transform.Position);
                        writer.WriteNextCol(sceneObject.transform.RotationEuler);
                        writer.WriteNextCol(sceneObject.transform.Scale);
                        writer.WriteNextCol(sceneObject.zero_0x2C);
                        writer.WriteNextCol(sceneObject.animationPtr.HexAddress);
                        writer.WriteNextCol(sceneObject.unkPtr_0x34.HexAddress);
                        writer.WriteNextCol(sceneObject.skeletalAnimatorPtr.HexAddress);
                        writer.WriteNextCol(sceneObject.transformPtr.HexAddress);
                        writer.WriteNextRow();

                        sceneObjectIndex++;
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
                writer.WriteNextCol("File");
                writer.WriteNextCol("Game Object #");
                writer.WriteNextCol("Game Object");
                writer.WriteNextCol("Unknown 1 Index");
                writer.WriteColNicify(nameof(UnknownSceneObjectFloatPair.unk_0x00));
                writer.WriteColNicify(nameof(UnknownSceneObjectFloatPair.unk_0x04));
                writer.WriteNextRow();

                foreach (var file in coliSobjs)
                {
                    int gameObjectIndex = 0;
                    foreach (var sceneObject in file.Value.sceneObjects)
                    {
                        if (sceneObject.unk1 == null)
                            continue;

                        int unkIndex = 0;
                        foreach (var unk1 in sceneObject.unk1.unk)
                        {
                            writer.WriteNextCol(file.FileName);
                            writer.WriteNextCol(gameObjectIndex);
                            writer.WriteNextCol(sceneObject.name);
                            writer.WriteNextCol(unkIndex);
                            writer.WriteNextCol(unk1.unk_0x00);
                            writer.WriteNextCol(unk1.unk_0x04);
                            writer.WriteNextRow();
                            unkIndex++;
                        }
                        gameObjectIndex++;
                    }
                }
                writer.Flush();
            }
        }

        public void AnalyzeGameObjectsSkeletalAnimator(string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Game Object #");
                writer.WriteNextCol("Game Object");

                writer.WriteColNicify(nameof(SkeletalAnimator.zero_0x00));
                writer.WriteColNicify(nameof(SkeletalAnimator.zero_0x04));
                writer.WriteColNicify(nameof(SkeletalAnimator.one_0x08));
                writer.WriteColNicify(nameof(SkeletalAnimator.propertiesPtr));

                writer.WriteColNicify(nameof(SkeletalProperties.unk_0x00));
                writer.WriteColNicify(nameof(SkeletalProperties.unk_0x04));
                writer.WriteFlagNames<EnumFlags32>();
                writer.WriteColNicify(nameof(SkeletalProperties.unk_0x08));
                writer.WriteFlagNames<EnumFlags32>();
                writer.WriteColNicify(nameof(SkeletalProperties.zero_0x0C));
                writer.WriteColNicify(nameof(SkeletalProperties.zero_0x10));
                writer.WriteColNicify(nameof(SkeletalProperties.zero_0x14));
                writer.WriteColNicify(nameof(SkeletalProperties.zero_0x18));
                writer.WriteNextRow();

                foreach (var file in coliSobjs)
                {
                    int gameObjectIndex = 0;
                    foreach (var sceneObject in file.Value.sceneObjects)
                    {
                        if (!sceneObject.skeletalAnimator.propertiesPtr.IsNotNullPointer)
                        {
                            continue;
                        }

                        writer.WriteNextCol(file.FileName);
                        writer.WriteNextCol(gameObjectIndex);
                        writer.WriteNextCol(sceneObject.name);

                        writer.WriteNextCol(sceneObject.skeletalAnimator.zero_0x00);
                        writer.WriteNextCol(sceneObject.skeletalAnimator.zero_0x04);
                        writer.WriteNextCol(sceneObject.skeletalAnimator.one_0x08);
                        writer.WriteNextCol(sceneObject.skeletalAnimator.propertiesPtr);

                        writer.WriteNextCol(sceneObject.skeletalAnimator.properties.unk_0x00);
                        writer.WriteNextCol(sceneObject.skeletalAnimator.properties.unk_0x04);
                        writer.WriteFlags(sceneObject.skeletalAnimator.properties.unk_0x04);
                        writer.WriteNextCol(sceneObject.skeletalAnimator.properties.unk_0x08);
                        writer.WriteFlags(sceneObject.skeletalAnimator.properties.unk_0x08);
                        writer.WriteNextCol(sceneObject.skeletalAnimator.properties.zero_0x0C);
                        writer.WriteNextCol(sceneObject.skeletalAnimator.properties.zero_0x10);
                        writer.WriteNextCol(sceneObject.skeletalAnimator.properties.zero_0x14);
                        writer.WriteNextCol(sceneObject.skeletalAnimator.properties.zero_0x18);
                        writer.WriteNextRow();

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
                writer.WriteNextCol("File");
                writer.WriteNextCol("Game Object #");
                writer.WriteNextCol("Game Object");

                writer.WriteNextCol("Tri Index");
                writer.WriteNextCol("Addr");

                writer.WriteColNicify(nameof(ColliderTriangle.unk_0x00));
                writer.WriteColNicify(nameof(ColliderTriangle.normal));
                writer.WriteColNicify(nameof(ColliderTriangle.vertex0));
                writer.WriteColNicify(nameof(ColliderTriangle.vertex1));
                writer.WriteColNicify(nameof(ColliderTriangle.vertex2));
                writer.WriteColNicify(nameof(ColliderTriangle.precomputed0));
                writer.WriteColNicify(nameof(ColliderTriangle.precomputed1));
                writer.WriteColNicify(nameof(ColliderTriangle.precomputed2));

                writer.WriteNextRow();

                foreach (var file in coliSobjs)
                {
                    int gameObjectIndex = 0;
                    foreach (var gameObject in file.Value.sceneObjects)
                    {
                        if (gameObject.colliderBinding.colliderGeometry.triCount == 0)
                        {
                            continue;
                        }

                        int triIndex = 0;
                        foreach (var tri in gameObject.colliderBinding.colliderGeometry.tris)
                        {
                            writer.WriteNextCol(file.FileName);
                            writer.WriteNextCol(gameObjectIndex);
                            writer.WriteNextCol(gameObject.name);

                            writer.WriteNextCol(triIndex++);
                            writer.WriteStartAddress(tri);

                            writer.WriteNextCol(tri.unk_0x00);
                            writer.WriteNextCol(tri.normal);
                            writer.WriteNextCol(tri.vertex0);
                            writer.WriteNextCol(tri.vertex1);
                            writer.WriteNextCol(tri.vertex2);
                            writer.WriteNextCol(tri.precomputed0);
                            writer.WriteNextCol(tri.precomputed1);
                            writer.WriteNextCol(tri.precomputed2);

                            writer.WriteNextRow();
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
                writer.WriteNextCol("File");
                writer.WriteNextCol("Game Object #");
                writer.WriteNextCol("Game Object");

                writer.WriteNextCol("Quad Index");
                writer.WriteNextCol("Addr");

                writer.WriteColNicify(nameof(ColliderQuad.unk_0x00));
                writer.WriteColNicify(nameof(ColliderQuad.normal));
                writer.WriteColNicify(nameof(ColliderQuad.vertex0));
                writer.WriteColNicify(nameof(ColliderQuad.vertex1));
                writer.WriteColNicify(nameof(ColliderQuad.vertex2));
                writer.WriteColNicify(nameof(ColliderQuad.vertex3));
                writer.WriteColNicify(nameof(ColliderQuad.precomputed0));
                writer.WriteColNicify(nameof(ColliderQuad.precomputed1));
                writer.WriteColNicify(nameof(ColliderQuad.precomputed2));
                writer.WriteColNicify(nameof(ColliderQuad.precomputed3));

                writer.WriteNextRow();

                foreach (var file in coliSobjs)
                {
                    int gameObjectIndex = 0;
                    foreach (var gameObject in file.Value.sceneObjects)
                    {
                        if (gameObject.colliderBinding.colliderGeometry.quadCount == 0)
                        {
                            continue;
                        }

                        int quadIndex = 0;
                        foreach (var quad in gameObject.colliderBinding.colliderGeometry.quads)
                        {
                            writer.WriteNextCol(file.FileName);
                            writer.WriteNextCol(gameObjectIndex);
                            writer.WriteNextCol(gameObject.name);

                            writer.WriteNextCol(quadIndex++);
                            writer.WriteStartAddress(quad);

                            writer.WriteNextCol(quad.unk_0x00);
                            writer.WriteNextCol(quad.normal);
                            writer.WriteNextCol(quad.vertex0);
                            writer.WriteNextCol(quad.vertex1);
                            writer.WriteNextCol(quad.vertex2);
                            writer.WriteNextCol(quad.vertex3);
                            writer.WriteNextCol(quad.precomputed0);
                            writer.WriteNextCol(quad.precomputed1);
                            writer.WriteNextCol(quad.precomputed2);
                            writer.WriteNextCol(quad.precomputed3);

                            writer.WriteNextRow();
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
                writer.WriteNextCol("File");
                writer.WriteNextCol("Index");
                writer.WriteNextCol("Stage");
                writer.WriteNextCol("Venue");
                writer.WriteNextCol("AX/GX");
                //
                writer.WriteNextCol(nameof(Header.unk_0x00) + " " + nameof(Header));
                writer.WriteNextCol(nameof(Header.trackNodesPtr));
                writer.WriteNextCol(nameof(Header.trackNodesPtr.address));
                writer.WriteNextCol(nameof(Header.surfaceAttributeAreasPtr));
                writer.WriteNextCol(nameof(Header.surfaceAttributeAreasPtr.address));
                writer.WriteNextCol(nameof(Header.boostPadsActive));
                writer.WriteNextCol(nameof(Header.surfaceAttributeMeshTablePtr));
                writer.WriteNextCol(nameof(Header.unknownData_0x20_Ptr));
                writer.WriteNextCol(nameof(Header.unknownFloat_0x24_Ptr));
                writer.WriteNextCol(nameof(Header.zero_0x28));
                writer.WriteNextCol(nameof(Header.sceneObjectCount));
                writer.WriteNextCol(nameof(Header.unk_sceneObjectCount1));
                writer.WriteNextCol(nameof(Header.unk_sceneObjectCount2));
                writer.WriteNextCol(nameof(Header.sceneObjectsPtr));
                writer.WriteNextCol(nameof(Header.unkBool32_0x58));
                writer.WriteNextCol(nameof(Header.unknownTrigger2sPtr));
                writer.WriteNextCol(nameof(Header.unknownTrigger2sPtr.address));
                writer.WriteNextCol(nameof(Header.collisionObjectReferences));
                writer.WriteNextCol(nameof(Header.collisionObjectReferences.address));
                writer.WriteNextCol(nameof(Header.unk_collisionObjectReferences));
                writer.WriteNextCol(nameof(Header.unk_collisionObjectReferences.address));
                writer.WriteNextCol(nameof(Header.unused_0x74_0x78));
                writer.WriteNextCol(nameof(Header.unused_0x74_0x78.address));
                writer.WriteNextCol(nameof(Header.circuitType));
                writer.WriteNextCol(nameof(Header.unknownStageData2Ptr));
                writer.WriteNextCol(nameof(Header.unknownStageData1Ptr));
                writer.WriteNextCol(nameof(Header.unused_0x88_0x8C));
                writer.WriteNextCol(nameof(Header.unused_0x88_0x8C.address));
                writer.WriteNextCol(nameof(Header.trackLengthPtr));
                writer.WriteNextCol(nameof(Header.unknownTrigger1sPtr));
                writer.WriteNextCol(nameof(Header.unknownTrigger1sPtr.address));
                writer.WriteNextCol(nameof(Header.visualEffectTriggersPtr));
                writer.WriteNextCol(nameof(Header.visualEffectTriggersPtr.address));
                writer.WriteNextCol(nameof(Header.courseMetadataTriggersPtr));
                writer.WriteNextCol(nameof(Header.courseMetadataTriggersPtr.address));
                writer.WriteNextCol(nameof(Header.arcadeCheckpointTriggersPtr));
                writer.WriteNextCol(nameof(Header.arcadeCheckpointTriggersPtr.address));
                writer.WriteNextCol(nameof(Header.storyObjectTriggersPtr));
                writer.WriteNextCol(nameof(Header.storyObjectTriggersPtr.address));
                writer.WriteNextCol(nameof(Header.trackIndexTable));
                // Structure
                writer.WriteNextCol(nameof(Header.unknownStructure1_0xC0) + "." + nameof(Header.unknownStructure1_0xC0.unk_0x00));
                writer.WriteNextCol(nameof(Header.unknownStructure1_0xC0) + "." + nameof(Header.unknownStructure1_0xC0.unk_0x04));
                writer.WriteNextCol(nameof(Header.unknownStructure1_0xC0) + "." + nameof(Header.unknownStructure1_0xC0.unk_0x08));
                writer.WriteNextCol(nameof(Header.unknownStructure1_0xC0) + "." + nameof(Header.unknownStructure1_0xC0.unk_0x0C));
                writer.WriteNextCol(nameof(Header.unknownStructure1_0xC0) + "." + nameof(Header.unknownStructure1_0xC0.unk_0x10));
                writer.WriteNextCol(nameof(Header.unknownStructure1_0xC0) + "." + nameof(Header.unknownStructure1_0xC0.unk_0x14));
                // 
                writer.WriteNextCol(nameof(Header.zero_0xD8));
                writer.WriteNextRow();

                foreach (var file in coliSobjs)
                {
                    var coli = file.Value;
                    var coliHeader = file.Value.header;

                    writer.WriteNextCol(coli.FileName);
                    writer.WriteNextCol(coli.ID);
                    writer.WriteNextCol(CourseUtility.GetVenueID(coli.ID).GetDescription());
                    writer.WriteNextCol(((CourseIDEx)coli.ID).GetDescription());
                    writer.WriteNextCol(coliHeader.IsFileGX ? "GX" : "AX");

                    writer.WriteNextCol(coliHeader.unk_0x00.a);
                    writer.WriteNextCol(coliHeader.unk_0x00.b);
                    writer.WriteNextCol(coliHeader.trackNodesPtr.length);
                    writer.WriteNextCol(coliHeader.trackNodesPtr.HexAddress);
                    writer.WriteNextCol(coliHeader.surfaceAttributeAreasPtr.length);
                    writer.WriteNextCol(coliHeader.surfaceAttributeAreasPtr.HexAddress);
                    writer.WriteNextCol(coliHeader.boostPadsActive);
                    writer.WriteNextCol(coliHeader.surfaceAttributeMeshTablePtr.HexAddress);
                    writer.WriteNextCol(coliHeader.unknownData_0x20_Ptr.HexAddress);
                    writer.WriteNextCol(coliHeader.unknownFloat_0x24_Ptr.HexAddress);
                    writer.WriteNextCol(0);// coliHeader.zero_0x28);
                    writer.WriteNextCol(coliHeader.sceneObjectCount);
                    if (coliHeader.IsFileGX)
                    {
                        writer.WriteNextCol(coliHeader.unk_sceneObjectCount1);
                    }
                    else // is AX
                    {
                        writer.WriteNextCol();
                    }
                    writer.WriteNextCol(coliHeader.unk_sceneObjectCount2);
                    writer.WriteNextCol(coliHeader.sceneObjectsPtr.HexAddress);
                    writer.WriteNextCol(coliHeader.unkBool32_0x58);
                    writer.WriteNextCol(coliHeader.unknownTrigger2sPtr.length);
                    writer.WriteNextCol(coliHeader.unknownTrigger2sPtr.HexAddress);
                    writer.WriteNextCol(coliHeader.collisionObjectReferences.length);
                    writer.WriteNextCol(coliHeader.collisionObjectReferences.HexAddress);
                    writer.WriteNextCol(coliHeader.unk_collisionObjectReferences.length);
                    writer.WriteNextCol(coliHeader.unk_collisionObjectReferences.HexAddress);
                    writer.WriteNextCol(coliHeader.unused_0x74_0x78.length);
                    writer.WriteNextCol(coliHeader.unused_0x74_0x78.HexAddress);
                    writer.WriteNextCol(coliHeader.circuitType);
                    writer.WriteNextCol(coliHeader.unknownStageData2Ptr.HexAddress);
                    writer.WriteNextCol(coliHeader.unknownStageData1Ptr.HexAddress);
                    writer.WriteNextCol(coliHeader.unused_0x88_0x8C.length);
                    writer.WriteNextCol(coliHeader.unused_0x88_0x8C.HexAddress);
                    writer.WriteNextCol(coliHeader.trackLengthPtr.HexAddress);
                    writer.WriteNextCol(coliHeader.unknownTrigger1sPtr.length);
                    writer.WriteNextCol(coliHeader.unknownTrigger1sPtr.HexAddress);
                    writer.WriteNextCol(coliHeader.visualEffectTriggersPtr.length);
                    writer.WriteNextCol(coliHeader.visualEffectTriggersPtr.HexAddress);
                    writer.WriteNextCol(coliHeader.courseMetadataTriggersPtr.length);
                    writer.WriteNextCol(coliHeader.courseMetadataTriggersPtr.HexAddress);
                    writer.WriteNextCol(coliHeader.arcadeCheckpointTriggersPtr.length);
                    writer.WriteNextCol(coliHeader.arcadeCheckpointTriggersPtr.HexAddress);
                    writer.WriteNextCol(coliHeader.storyObjectTriggersPtr.length);
                    writer.WriteNextCol(coliHeader.storyObjectTriggersPtr.HexAddress);
                    writer.WriteNextCol(coliHeader.trackIndexTable.HexAddress);
                    // Structure
                    writer.WriteNextCol(coliHeader.unknownStructure1_0xC0.unk_0x00);
                    writer.WriteNextCol(coliHeader.unknownStructure1_0xC0.unk_0x04);
                    writer.WriteNextCol(coliHeader.unknownStructure1_0xC0.unk_0x08);
                    writer.WriteNextCol(coliHeader.unknownStructure1_0xC0.unk_0x0C);
                    writer.WriteNextCol(coliHeader.unknownStructure1_0xC0.unk_0x10);
                    writer.WriteNextCol(coliHeader.unknownStructure1_0xC0.unk_0x14);
                    //
                    writer.WriteNextCol(0);// coliHeader.zero_0xD8);
                    writer.WriteNextRow();
                }
                writer.Flush();
            }
        }

        public void AnalyzeUnknownTrigger1(string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Index");
                writer.WriteNextCol("Stage");
                writer.WriteNextCol("Venue");
                writer.WriteNextCol("AX/GX");
                //
                writer.WriteNextCol("Start");
                writer.WriteNextCol("End");
                //
                writer.WriteNextCol(nameof(UnknownTrigger1.transform.Position));
                writer.WriteNextCol(nameof(UnknownTrigger1.transform.RotationEuler));
                writer.WriteNextCol(nameof(UnknownTrigger1.transform.Scale));
                writer.WriteNextCol(nameof(UnknownTrigger1.unk_0x20));
                //
                writer.WriteNextCol("Index");
                writer.WriteNextRow();

                foreach (var file in coliSobjs)
                {
                    var scene = file.Value;
                    var venueID = CourseUtility.GetVenueID(scene.ID).GetDescription();
                    var courseID = ((CourseIDEx)scene.ID).GetDescription();
                    var isAxGx = scene.header.IsFileGX ? "GX" : "AX";

                    int count = 0;
                    int total = scene.unknownTrigger1s.Length;
                    foreach (var item in scene.unknownTrigger1s)
                    {
                        count++;

                        writer.WriteNextCol(scene.FileName);
                        writer.WriteNextCol(scene.ID);
                        writer.WriteNextCol(venueID);
                        writer.WriteNextCol(courseID);
                        writer.WriteNextCol(isAxGx);

                        writer.WriteNextCol(item.StartAddressHex());
                        writer.WriteNextCol(item.EndAddressHex());

                        writer.WriteNextCol(item.transform.Position);
                        writer.WriteNextCol(item.transform.RotationEuler);
                        writer.WriteNextCol(item.transform.Scale);
                        writer.WriteNextCol(item.unk_0x20);

                        writer.WriteNextCol($"[{count}/{total}]");

                        writer.WriteNextRow();
                    }
                }
                writer.Flush();
            }
        }

        //public void AnalyzeVenueMetadataObjects(string fileName)
        //{
        //    using (var writer = AnalyzerUtility.OpenWriter(fileName))
        //    {
        //        // Write header
        //        writer.PushCol("File");
        //        writer.PushCol("Index");
        //        writer.PushCol("Stage");
        //        writer.PushCol("Venue");
        //        writer.PushCol("AX/GX");
        //        //
        //        writer.PushCol(nameof(VenueMetadataObject.position));
        //        writer.PushCol(nameof(VenueMetadataObject.unk_0x0C));
        //        writer.PushCol(nameof(VenueMetadataObject.unk_0x0E));
        //        writer.PushCol(nameof(VenueMetadataObject.unk_0x10));
        //        writer.PushCol(nameof(VenueMetadataObject.unk_0x12));
        //        writer.PushCol(nameof(VenueMetadataObject.positionOrScale));
        //        writer.PushCol(nameof(VenueMetadataObject.venue));
        //        //
        //        writer.PushRow();

        //        foreach (var file in coliSobjs)
        //        {
        //            var scene = file.Value;
        //            var venueID = CourseUtility.GetVenueID(scene.ID).GetDescription();
        //            var courseID = ((CourseIDEx)scene.ID).GetDescription();
        //            var isAxGx = scene.header.IsFileGX ? "GX" : "AX";

        //            foreach (var item in scene.venueMetadataObjects)
        //            {
        //                writer.PushCol(scene.FileName);
        //                writer.PushCol(scene.ID);
        //                writer.PushCol(venueID);
        //                writer.PushCol(courseID);
        //                writer.PushCol(isAxGx);
        //                //
        //                writer.PushCol(item.position);
        //                writer.PushCol(item.unk_0x0C);
        //                writer.PushCol(item.unk_0x0E);
        //                writer.PushCol(item.unk_0x10);
        //                writer.PushCol(item.unk_0x12);
        //                writer.PushCol(item.positionOrScale);
        //                writer.PushCol(item.venue);
        //                //
        //                writer.PushRow();
        //            }
        //            writer.Flush();
        //        }
        //    }
        //}

        public void AnalyzeStoryObjectTrigger(string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Index");
                writer.WriteNextCol("Stage");
                writer.WriteNextCol("Venue");
                writer.WriteNextCol("AX/GX");
                //
                writer.WriteNextCol(nameof(StoryObjectTrigger.zero_0x00));
                writer.WriteNextCol(nameof(StoryObjectTrigger.rockGroupOrderIndex));
                writer.WriteNextCol(nameof(StoryObjectTrigger.RockGroup));
                writer.WriteNextCol(nameof(StoryObjectTrigger.Difficulty));
                writer.WriteNextCol(nameof(StoryObjectTrigger.story2RockScale));
                writer.WriteNextCol(nameof(StoryObjectTrigger.animationPathPtr));
                writer.WriteNextCol(nameof(StoryObjectTrigger.scale));
                writer.WriteNextCol(nameof(StoryObjectTrigger.rotation));
                writer.WriteNextCol(nameof(StoryObjectTrigger.position));
                //
                writer.WriteNextRow();

                foreach (var file in coliSobjs)
                {
                    var scene = file.Value;
                    var venueID = CourseUtility.GetVenueID(scene.ID).GetDescription();
                    var courseID = ((CourseIDEx)scene.ID).GetDescription();
                    var isAxGx = scene.header.IsFileGX ? "GX" : "AX";

                    foreach (var item in scene.storyObjectTriggers)
                    {
                        writer.WriteNextCol(scene.FileName);
                        writer.WriteNextCol(scene.ID);
                        writer.WriteNextCol(venueID);
                        writer.WriteNextCol(courseID);
                        writer.WriteNextCol(isAxGx);
                        //
                        writer.WriteNextCol(item.zero_0x00);
                        writer.WriteNextCol(item.rockGroupOrderIndex);
                        writer.WriteNextCol(item.RockGroup);
                        writer.WriteNextCol(item.Difficulty);
                        writer.WriteNextCol(item.story2RockScale);
                        writer.WriteNextCol(item.animationPathPtr);
                        writer.WriteNextCol(item.scale);
                        writer.WriteNextCol(item.rotation);
                        writer.WriteNextCol(item.position);
                        //
                        writer.WriteNextRow();
                    }
                    writer.Flush();
                }
            }
        }

        public void AnalyzeUnknownAnimationData(string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Index");
                writer.WriteNextCol("Stage");
                writer.WriteNextCol("Venue");
                writer.WriteNextCol("AX/GX");
                //
                writer.WriteNextCol("Index");
                //
                writer.WriteNextCol(nameof(KeyableAttribute.easeMode));
                writer.WriteNextCol(nameof(KeyableAttribute.time));
                writer.WriteNextCol(nameof(KeyableAttribute.value));
                writer.WriteNextCol(nameof(KeyableAttribute.zTangentIn));
                writer.WriteNextCol(nameof(KeyableAttribute.zTangentOut));
                //
                writer.WriteNextRow();

                foreach (var file in coliSobjs)
                {
                    var scene = file.Value;
                    var venueID = CourseUtility.GetVenueID(scene.ID).GetDescription();
                    var courseID = ((CourseIDEx)scene.ID).GetDescription();
                    var isAxGx = scene.header.IsFileGX ? "GX" : "AX";

                    var totalD1 = scene.unknownStageData2.unkAnimData.Length;
                    var countD1 = 0;
                    foreach (var animDataCollection in scene.unknownStageData2.unkAnimData)
                    {
                        countD1++;
                        foreach (var keyableAttribute in animDataCollection)
                        {
                            writer.WriteNextCol(scene.FileName);
                            writer.WriteNextCol(scene.ID);
                            writer.WriteNextCol(venueID);
                            writer.WriteNextCol(courseID);
                            writer.WriteNextCol(isAxGx);
                            //
                            writer.WriteNextCol($"[{countD1}/{totalD1}]");
                            //
                            writer.WriteNextCol(keyableAttribute.easeMode);
                            writer.WriteNextCol(keyableAttribute.time);
                            writer.WriteNextCol(keyableAttribute.value);
                            writer.WriteNextCol(keyableAttribute.zTangentIn);
                            writer.WriteNextCol(keyableAttribute.zTangentOut);
                            //
                            writer.WriteNextRow();
                        }
                    }
                    writer.Flush();
                }
            }
        }

        public void AnalyzeUnknownStageData1(string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Index");
                writer.WriteNextCol("Stage");
                writer.WriteNextCol("Venue");
                writer.WriteNextCol("AX/GX");
                //
                writer.WriteNextCol(nameof(UnknownStageData1.unk_0x00));
                writer.WriteNextCol(nameof(UnknownStageData1.unk_0x04) + " " + nameof(UnknownStageData1.unk_0x04.a));
                writer.WriteNextCol(nameof(UnknownStageData1.unk_0x04) + " " + nameof(UnknownStageData1.unk_0x04.b));
                writer.WriteNextCol(nameof(UnknownStageData1.unk_0x0C));
                writer.WriteNextCol(nameof(UnknownStageData1.unk_0x18));
                //
                writer.WriteNextRow();

                foreach (var file in coliSobjs)
                {
                    var scene = file.Value;
                    var venueID = CourseUtility.GetVenueID(scene.ID).GetDescription();
                    var courseID = ((CourseIDEx)scene.ID).GetDescription();
                    var isAxGx = scene.header.IsFileGX ? "GX" : "AX";

                    writer.WriteNextCol(scene.FileName);
                    writer.WriteNextCol(scene.ID);
                    writer.WriteNextCol(venueID);
                    writer.WriteNextCol(courseID);
                    writer.WriteNextCol(isAxGx);
                    //
                    writer.WriteNextCol(scene.unknownStageData1.unk_0x00);
                    writer.WriteNextCol(scene.unknownStageData1.unk_0x04.a);
                    writer.WriteNextCol(scene.unknownStageData1.unk_0x04.b);
                    writer.WriteNextCol(scene.unknownStageData1.unk_0x0C);
                    writer.WriteNextCol(scene.unknownStageData1.unk_0x18);
                    //
                    writer.WriteNextRow();
                }
                writer.Flush();
            }
        }


        public void AnalyzeSceneObjectTransforms(string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Game Object #");
                writer.WriteNextCol("Game Object");
                writer.WriteNextCol(nameof(SceneObject.transform.Position) + ".x");
                writer.WriteNextCol(nameof(SceneObject.transformMatrix3x4.Position) + ".x");
                writer.WriteNextCol(nameof(SceneObject.transform.Position) + ".y");
                writer.WriteNextCol(nameof(SceneObject.transformMatrix3x4.Position) + ".z");
                writer.WriteNextCol(nameof(SceneObject.transform.Position) + ".z");
                writer.WriteNextCol(nameof(SceneObject.transformMatrix3x4.Position) + ".z");
                writer.WriteNextCol($"matrix: {nameof(SceneObject.transformMatrix3x4.RotationEuler)}.x");
                writer.WriteNextCol($"matrix: {nameof(SceneObject.transformMatrix3x4.RotationEuler)}.y");
                writer.WriteNextCol($"matrix: {nameof(SceneObject.transformMatrix3x4.RotationEuler)}.z");
                writer.WriteNextCol($"scene : {nameof(SceneObject.transform.RotationEuler)}.x");
                writer.WriteNextCol($"scene : {nameof(SceneObject.transform.RotationEuler)}.y");
                writer.WriteNextCol($"scene : {nameof(SceneObject.transform.RotationEuler)}.z");
                writer.WriteNextCol(nameof(SceneObject.transform.UnkFlags));
                writer.WriteNextCol("Backing x");
                writer.WriteNextCol("Backing y");
                writer.WriteNextCol("Backing z");
                writer.WriteNextCol(nameof(SceneObject.transform.Scale) + ".x");
                writer.WriteNextCol(nameof(SceneObject.transformMatrix3x4.Scale) + ".x");
                writer.WriteNextCol(nameof(SceneObject.transform.Scale) + ".y");
                writer.WriteNextCol(nameof(SceneObject.transformMatrix3x4.Scale) + ".y");
                writer.WriteNextCol(nameof(SceneObject.transform.Scale) + ".z");
                writer.WriteNextCol(nameof(SceneObject.transformMatrix3x4.Scale) + ".z");

                writer.WriteNextRow();

                foreach (var file in coliSobjs)
                {
                    int sceneObjectIndex = 0;
                    foreach (var sceneObject in file.Value.sceneObjects)
                    {
                        writer.WriteNextCol(file.FileName);
                        writer.WriteNextCol(sceneObjectIndex);
                        writer.WriteNextCol(sceneObject.name);
                        // position
                        writer.WriteNextCol(sceneObject.transform.Position.x);
                        writer.WriteNextCol(sceneObject.transformMatrix3x4.Position.x);
                        writer.WriteNextCol(sceneObject.transform.Position.y);
                        writer.WriteNextCol(sceneObject.transformMatrix3x4.Position.y);
                        writer.WriteNextCol(sceneObject.transform.Position.z);
                        writer.WriteNextCol(sceneObject.transformMatrix3x4.Position.z);
                        // rotation
                        writer.WriteNextCol(sceneObject.transformMatrix3x4.RotationEuler.x);
                        writer.WriteNextCol(sceneObject.transformMatrix3x4.RotationEuler.y);
                        writer.WriteNextCol(sceneObject.transformMatrix3x4.RotationEuler.z);
                        //
                        writer.WriteNextCol(sceneObject.transform.RotationEuler.x);
                        writer.WriteNextCol(sceneObject.transform.RotationEuler.y);
                        writer.WriteNextCol(sceneObject.transform.RotationEuler.z);
                        writer.WriteNextCol($"0x{(int)sceneObject.transform.UnkFlags:X4}");
                        //
                        writer.WriteNextCol($"0x{(int)sceneObject.transform.RotX:X8}");
                        writer.WriteNextCol($"0x{(int)sceneObject.transform.RotY:X8}");
                        writer.WriteNextCol($"0x{(int)sceneObject.transform.RotZ:X8}");


                        // scale
                        writer.WriteNextCol(sceneObject.transform.Scale.x);
                        writer.WriteNextCol(sceneObject.transformMatrix3x4.Scale.x);
                        writer.WriteNextCol(sceneObject.transform.Scale.y);
                        writer.WriteNextCol(sceneObject.transformMatrix3x4.Scale.y);
                        writer.WriteNextCol(sceneObject.transform.Scale.z);
                        writer.WriteNextCol(sceneObject.transformMatrix3x4.Scale.z);

                        writer.WriteNextRow();

                        sceneObjectIndex++;
                    }
                }
                writer.Flush();
            }
        }

    }
}