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
            headers = true,
            unknownStruct6 = true,
            storyObject = true,
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

                filePath = $"{time} COLI {nameof(SceneObject)}.tsv";
                filePath = Path.Combine(outputPath, filePath);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, .5f);
                AnalyzeGameObjects(filePath);

                filePath = $"{time} COLI {nameof(SceneObject)} Unk1.tsv";
                filePath = Path.Combine(outputPath, filePath);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, .5f);
                AnalyzeGameObjectsUnk1(filePath);

                filePath = $"{time} COLI {nameof(SceneObject)} Skeletal.tsv";
                filePath = Path.Combine(outputPath, filePath);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, .5f);
                AnalyzeGameObjectsUnk2(filePath);

                filePath = $"{time} COLI {nameof(SceneObject)} Collision Tris.tsv";
                filePath = Path.Combine(outputPath, filePath);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, .5f);
                AnalyzeGameObjectsCollisionTri(filePath);

                filePath = $"{time} COLI {nameof(SceneObject)} Collision Quads.tsv";
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

            if (unknownStruct6)
            {
                string fileName = $"{time} COLI {nameof(ColiUnknownStruct6)}.tsv";
                string filePath = Path.Combine(outputPath, fileName);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, .5f);
                AnalyzeUnknownStruct6(filePath);
            }

            if (storyObject)
            {
                string fileName = $"{time} COLI {nameof(VenueMetadataObject)}.tsv";
                string filePath = Path.Combine(outputPath, fileName);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, .5f);
                AnalyzeVenueMetadataObjects(filePath);
            }

            if (venueMetadataObject)
            {
                string fileName = $"{time} COLI {nameof(StoryObject)}.tsv";
                string filePath = Path.Combine(outputPath, fileName);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, .5f);
                AnalyzeStoryObject(filePath);
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

        public void WriteTrackData(StreamWriter writer, ColiSceneSobj sobj, int level, int index, int total, KeyableAttribute param)
        {
            writer.PushCol(sobj.FilePath);
            writer.PushCol($"[{index}/{total}]");
            writer.PushCol($"{level}");
            writer.PushCol(param.StartAddressHex());
            writer.PushCol(param.easeMode);
            writer.PushCol(param.time);
            writer.PushCol(param.value);
            writer.PushCol(param.zTangentIn);
            writer.PushCol(param.zTangentOut);
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
            writer.PushCol(trackTransform.StartAddressHex());
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
                    foreach (var gameObject in file.Value.sceneObjects)
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
                                writer.PushCol(animationCurve.StartAddressHex());
                                writer.PushCol(keyable.StartAddressHex());
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

                                writer.PushCol(file.FileName);
                                writer.PushCol(gameObjectIndex);
                                writer.PushCol(gameObject.name);
                                writer.PushCol(animationCurve.StartAddressHex());
                                writer.PushCol(keyable.StartAddressHex());
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
                    foreach (var gameObject in file.Value.sceneObjects)
                    {
                        writer.PushCol(file.FileName);
                        writer.PushCol(gameObjectIndex);
                        writer.PushCol(gameObject.name);
                        writer.PushCol((int)gameObject.unk_0x00);
                        writer.WriteFlags(gameObject.unk_0x00);
                        writer.PushCol((int)gameObject.unk_0x04);
                        writer.WriteFlags(gameObject.unk_0x04);
                        writer.PushCol(gameObject.collisionBindingPtr.HexAddress);
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
                        writer.PushCol(gameObject.animationPtr.HexAddress);
                        writer.PushCol(gameObject.unkPtr_0x34.HexAddress);
                        writer.PushCol(gameObject.skeletalAnimatorPtr.HexAddress);
                        writer.PushCol(gameObject.transformPtr.HexAddress);
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
                    foreach (var gameObject in file.Value.sceneObjects)
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
                writer.PushRow();

                foreach (var file in coliSobjs)
                {
                    int gameObjectIndex = 0;
                    foreach (var sceneObject in file.Value.sceneObjects)
                    {
                        if (!sceneObject.skeletalAnimator.propertiesPtr.IsNotNullPointer)
                        {
                            continue;
                        }

                        writer.PushCol(file.FileName);
                        writer.PushCol(gameObjectIndex);
                        writer.PushCol(sceneObject.name);

                        writer.PushCol(sceneObject.skeletalAnimator.zero_0x00);
                        writer.PushCol(sceneObject.skeletalAnimator.zero_0x04);
                        writer.PushCol(sceneObject.skeletalAnimator.one_0x08);
                        writer.PushCol(sceneObject.skeletalAnimator.propertiesPtr);

                        writer.PushCol(sceneObject.skeletalAnimator.properties.unk_0x00);
                        writer.PushCol(sceneObject.skeletalAnimator.properties.unk_0x04);
                        writer.WriteFlags(sceneObject.skeletalAnimator.properties.unk_0x04);
                        writer.PushCol(sceneObject.skeletalAnimator.properties.unk_0x08);
                        writer.WriteFlags(sceneObject.skeletalAnimator.properties.unk_0x08);
                        writer.PushCol(sceneObject.skeletalAnimator.properties.zero_0x0C);
                        writer.PushCol(sceneObject.skeletalAnimator.properties.zero_0x10);
                        writer.PushCol(sceneObject.skeletalAnimator.properties.zero_0x14);
                        writer.PushCol(sceneObject.skeletalAnimator.properties.zero_0x18);
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

                writer.WriteColNicify(nameof(ColliderTriangle.unk_0x00));
                writer.WriteColNicify(nameof(ColliderTriangle.normal));
                writer.WriteColNicify(nameof(ColliderTriangle.vertex0));
                writer.WriteColNicify(nameof(ColliderTriangle.vertex1));
                writer.WriteColNicify(nameof(ColliderTriangle.vertex2));
                writer.WriteColNicify(nameof(ColliderTriangle.precomputed0));
                writer.WriteColNicify(nameof(ColliderTriangle.precomputed1));
                writer.WriteColNicify(nameof(ColliderTriangle.precomputed2));

                writer.PushRow();

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

                writer.PushRow();

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
                writer.PushCol(nameof(Header.surfaceAttributeAreasPtr));
                writer.PushCol(nameof(Header.surfaceAttributeAreasPtr.address));
                writer.PushCol(nameof(Header.boostPadsActive));
                writer.PushCol(nameof(Header.surfaceAttributesMeshTablePtr));
                writer.PushCol(nameof(Header.unkPtr_0x20));
                writer.PushCol(nameof(Header.unkPtr_0x24));
                writer.PushCol(nameof(Header.zero_0x28));
                writer.PushCol(nameof(Header.sceneObjectCount));
                writer.PushCol(nameof(Header.unk_sceneObjectCount1));
                writer.PushCol(nameof(Header.unk_sceneObjectCount2));
                writer.PushCol(nameof(Header.sceneObjectsPtr));
                writer.PushCol(nameof(Header.unkBool32_0x58));
                writer.PushCol(nameof(Header.unkArrayPtr_0x5C));
                writer.PushCol(nameof(Header.unkArrayPtr_0x5C.address));
                writer.PushCol(nameof(Header.collisionObjectReferences));
                writer.PushCol(nameof(Header.collisionObjectReferences.address));
                writer.PushCol(nameof(Header.unk_collisionObjectReferences));
                writer.PushCol(nameof(Header.unk_collisionObjectReferences.address));
                writer.PushCol(nameof(Header.unused_0x74_0x78));
                writer.PushCol(nameof(Header.unused_0x74_0x78.address));
                writer.PushCol(nameof(Header.circuitType));
                writer.PushCol(nameof(Header.unkPtr_0x80));
                writer.PushCol(nameof(Header.unkPtr_0x84));
                writer.PushCol(nameof(Header.unused_0x88_0x8C));
                writer.PushCol(nameof(Header.unused_0x88_0x8C.address));
                writer.PushCol(nameof(Header.trackLengthPtr));
                writer.PushCol(nameof(Header.unkArrayPtr_0x94));
                writer.PushCol(nameof(Header.unkArrayPtr_0x94.address));
                writer.PushCol(nameof(Header.unkArrayPtr_0x9C));
                writer.PushCol(nameof(Header.unkArrayPtr_0x9C.address));
                writer.PushCol(nameof(Header.pathObjects));
                writer.PushCol(nameof(Header.pathObjects.address));
                writer.PushCol(nameof(Header.arcadeCheckpoint));
                writer.PushCol(nameof(Header.arcadeCheckpoint.address));
                writer.PushCol(nameof(Header.storyObjects));
                writer.PushCol(nameof(Header.storyObjects.address));
                writer.PushCol(nameof(Header.trackIndexTable));
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
                    writer.PushCol(coli.ID);
                    writer.PushCol(CourseUtility.GetVenueID(coli.ID).GetDescription());
                    writer.PushCol(((CourseIDEx)coli.ID).GetDescription());
                    writer.PushCol(coliHeader.IsFileGX ? "GX" : "AX");

                    writer.PushCol(coliHeader.unk_0x00);
                    writer.PushCol(coliHeader.unk_0x04);
                    writer.PushCol(coliHeader.trackNodesPtr.length);
                    writer.PushCol(coliHeader.trackNodesPtr.HexAddress);
                    writer.PushCol(coliHeader.surfaceAttributeAreasPtr.length);
                    writer.PushCol(coliHeader.surfaceAttributeAreasPtr.HexAddress);
                    writer.PushCol(coliHeader.boostPadsActive);
                    writer.PushCol(coliHeader.surfaceAttributesMeshTablePtr.HexAddress);
                    writer.PushCol(coliHeader.unkPtr_0x20.HexAddress);
                    writer.PushCol(coliHeader.unkPtr_0x24.HexAddress);
                    writer.PushCol(0);// coliHeader.zero_0x28);
                    writer.PushCol(coliHeader.sceneObjectCount);
                    if (coliHeader.IsFileGX)
                    {
                        writer.PushCol(coliHeader.unk_sceneObjectCount1);
                    }
                    else // is AX
                    {
                        writer.PushCol();
                    }
                    writer.PushCol(coliHeader.unk_sceneObjectCount2);
                    writer.PushCol(coliHeader.sceneObjectsPtr.HexAddress);
                    writer.PushCol(coliHeader.unkBool32_0x58);
                    writer.PushCol(coliHeader.unkArrayPtr_0x5C.length);
                    writer.PushCol(coliHeader.unkArrayPtr_0x5C.HexAddress);
                    writer.PushCol(coliHeader.collisionObjectReferences.length);
                    writer.PushCol(coliHeader.collisionObjectReferences.HexAddress);
                    writer.PushCol(coliHeader.unk_collisionObjectReferences.length);
                    writer.PushCol(coliHeader.unk_collisionObjectReferences.HexAddress);
                    writer.PushCol(coliHeader.unused_0x74_0x78.length);
                    writer.PushCol(coliHeader.unused_0x74_0x78.HexAddress);
                    writer.PushCol(coliHeader.circuitType);
                    writer.PushCol(coliHeader.unkPtr_0x80.HexAddress);
                    writer.PushCol(coliHeader.unkPtr_0x84.HexAddress);
                    writer.PushCol(coliHeader.unused_0x88_0x8C.length);
                    writer.PushCol(coliHeader.unused_0x88_0x8C.HexAddress);
                    writer.PushCol(coliHeader.trackLengthPtr.HexAddress);
                    writer.PushCol(coliHeader.unkArrayPtr_0x94.length);
                    writer.PushCol(coliHeader.unkArrayPtr_0x94.HexAddress);
                    writer.PushCol(coliHeader.unkArrayPtr_0x9C.length);
                    writer.PushCol(coliHeader.unkArrayPtr_0x9C.HexAddress);
                    writer.PushCol(coliHeader.pathObjects.length);
                    writer.PushCol(coliHeader.pathObjects.HexAddress);
                    writer.PushCol(coliHeader.arcadeCheckpoint.length);
                    writer.PushCol(coliHeader.arcadeCheckpoint.HexAddress);
                    writer.PushCol(coliHeader.storyObjects.length);
                    writer.PushCol(coliHeader.storyObjects.HexAddress);
                    writer.PushCol(coliHeader.trackIndexTable.HexAddress);
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

        public void AnalyzeUnknownStruct6(string fileName)
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
                writer.PushCol("Tag");
                writer.PushCol("Start");
                writer.PushCol("End");
                //
                //writer.PushCol(nameof(ColiUnknownStruct6.unk_0x00));
                //writer.PushCol(nameof(ColiUnknownStruct6.unk_0x04));
                //writer.PushCol(nameof(ColiUnknownStruct6.unk_0x08));
                writer.PushCol(nameof(ColiUnknownStruct6.position));
                writer.PushCol(nameof(ColiUnknownStruct6.rotation));
                //writer.PushCol(nameof(ColiUnknownStruct6.unk_0x0C));
                //writer.PushCol(nameof(ColiUnknownStruct6.unk_0x0E));
                //writer.PushCol(nameof(ColiUnknownStruct6.unk_0x10));
                writer.PushCol(nameof(ColiUnknownStruct6.zero_0x12));
                //writer.PushCol(nameof(ColiUnknownStruct6.unk_0x14));
                //writer.PushCol(nameof(ColiUnknownStruct6.unk_0x18));
                //writer.PushCol(nameof(ColiUnknownStruct6.unk_0x1C));
                writer.PushCol(nameof(ColiUnknownStruct6.scaleOrRotation));
                writer.PushCol(nameof(ColiUnknownStruct6.unk_0x20));
                //
                writer.PushRow();

                foreach (var file in coliSobjs)
                {
                    var coli = file.Value;
                    WriteUnknownStruct6Row(writer, coli, "Arcade Checkpoints", coli.arcadeCheckpoints);
                    //WriteUnknownStruct6Row(writer, coli, "Path Objects", coli.pathObjects);
                    WriteUnknownStruct6Row(writer, coli, "0x94", coli.unknownStruct6_0x94);
                    WriteUnknownStruct6Row(writer, coli, "0x9C", coli.unknownStruct6_0x9C);
                }
                writer.Flush();
            }
        }

        private void WriteUnknownStruct6Row(StreamWriter writer, ColiScene scene, string tag, ColiUnknownStruct6[] struct6Array)
        {
            var venueID = CourseUtility.GetVenueID(scene.ID).GetDescription();
            var courseID = ((CourseIDEx)scene.ID).GetDescription();
            var isAxGx = scene.header.IsFileGX ? "GX" : "AX";

            foreach (var item in struct6Array)
            {
                writer.PushCol(scene.FileName);
                writer.PushCol(scene.ID);
                writer.PushCol(venueID);
                writer.PushCol(courseID);
                writer.PushCol(isAxGx);

                writer.PushCol(tag);

                writer.PushCol(item.StartAddressHex());
                writer.PushCol(item.EndAddressHex());

                writer.PushCol(item.position);
                writer.PushCol(item.rotation);
                //writer.PushCol(item.unk_0x00);
                //writer.PushCol(item.unk_0x04);
                //writer.PushCol(item.unk_0x08);
                //writer.PushCol($"0x{item.unk_0x0C:X4}");
                //writer.PushCol($"0x{item.unk_0x0E:X4}");
                //writer.PushCol($"0x{item.unk_0x10:X4}");
                //writer.PushCol($"0x{item.zero_0x12:X4}");
                writer.PushCol(item.scaleOrRotation);
                //writer.PushCol(item.unk_0x14);
                //writer.PushCol(item.unk_0x18);
                //writer.PushCol(item.unk_0x1C);
                writer.PushCol(item.unk_0x20);
                writer.PushRow();
            }
        }

        public void AnalyzeVenueMetadataObjects(string fileName)
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
                writer.PushCol(nameof(VenueMetadataObject.position));
                writer.PushCol(nameof(VenueMetadataObject.unk_0x0C));
                writer.PushCol(nameof(VenueMetadataObject.unk_0x0E));
                writer.PushCol(nameof(VenueMetadataObject.unk_0x10));
                writer.PushCol(nameof(VenueMetadataObject.unk_0x12));
                writer.PushCol(nameof(VenueMetadataObject.positionOrScale));
                writer.PushCol(nameof(VenueMetadataObject.venue));
                //
                writer.PushRow();

                foreach (var file in coliSobjs)
                {
                    var scene = file.Value;
                    var venueID = CourseUtility.GetVenueID(scene.ID).GetDescription();
                    var courseID = ((CourseIDEx)scene.ID).GetDescription();
                    var isAxGx = scene.header.IsFileGX ? "GX" : "AX";

                    foreach (var item in scene.venueMetadataObjects)
                    {
                        writer.PushCol(scene.FileName);
                        writer.PushCol(scene.ID);
                        writer.PushCol(venueID);
                        writer.PushCol(courseID);
                        writer.PushCol(isAxGx);
                        //
                        writer.PushCol(item.position);
                        writer.PushCol(item.unk_0x0C);
                        writer.PushCol(item.unk_0x0E);
                        writer.PushCol(item.unk_0x10);
                        writer.PushCol(item.unk_0x12);
                        writer.PushCol(item.positionOrScale);
                        writer.PushCol(item.venue);
                        //
                        writer.PushRow();
                    }
                    writer.Flush();
                }
            }
        }

        public void AnalyzeStoryObject(string fileName)
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
                writer.PushCol(nameof(StoryObject.zero_0x00));
                writer.PushCol(nameof(StoryObject.rockGroupOrderIndex));
                writer.PushCol(nameof(StoryObject.RockGroup));
                writer.PushCol(nameof(StoryObject.Difficulty));
                writer.PushCol(nameof(StoryObject.story2RockScale));
                writer.PushCol(nameof(StoryObject.animationPathPtr));
                writer.PushCol(nameof(StoryObject.scale));
                writer.PushCol(nameof(StoryObject.rotation));
                writer.PushCol(nameof(StoryObject.position));
                //
                writer.PushRow();

                foreach (var file in coliSobjs)
                {
                    var scene = file.Value;
                    var venueID = CourseUtility.GetVenueID(scene.ID).GetDescription();
                    var courseID = ((CourseIDEx)scene.ID).GetDescription();
                    var isAxGx = scene.header.IsFileGX ? "GX" : "AX";

                    foreach (var item in scene.storyObjects)
                    {
                        writer.PushCol(scene.FileName);
                        writer.PushCol(scene.ID);
                        writer.PushCol(venueID);
                        writer.PushCol(courseID);
                        writer.PushCol(isAxGx);
                        //
                        writer.PushCol(item.zero_0x00);
                        writer.PushCol(item.rockGroupOrderIndex);
                        writer.PushCol(item.RockGroup);
                        writer.PushCol(item.Difficulty);
                        writer.PushCol(item.story2RockScale);
                        writer.PushCol(item.animationPathPtr);
                        writer.PushCol(item.scale);
                        writer.PushCol(item.rotation);
                        writer.PushCol(item.position);
                        //
                        writer.PushRow();
                    }
                    writer.Flush();
                }
            }
        }
    }
}