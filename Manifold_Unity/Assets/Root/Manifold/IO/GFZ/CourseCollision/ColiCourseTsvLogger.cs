using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCube.GFZ;
using GameCube.GFZ.CourseCollision;
using Manifold.IO;
using UnityEditor;
using System.IO;

namespace Manifold.IO.GFZ.CourseCollision
{
    public static class ColiCourseTsvLogger
    {
        private static float RandomTime => 1f;
        private static string ExecuteText => "TEMP";


        private const string menu = Const.Menu.Manifold + "Analysis/";


        [MenuItem(menu + "Analyze all ColiCourse" + " (Active Dir) #F5")]
        public static void AnalyzeAllPartsOfColiCourse()
        {
            var settings = GfzProjectWindow.GetSettings();
            var sceneIterator = ColiCourseIO.LoadAllStages(settings.StageDir, "Loading Stages...");
            var scenes = sceneIterator.ToArray();
            var output = settings.AnalysisOutput + "/" + AnalyzerUtility.GetFileTimestamp() + "/";
            Directory.CreateDirectory(output);
            Analyze(scenes, output);
            OSUtility.OpenDirectory(settings.AnalysisOutput);
        }

        //// Trying to make this stuff automatic?
        //public static void Forwarder(Action<ColiScene[], string> action)
        //{
        //    var settings = GfzProjectWindow.GetSettings();
        //    var sceneIterator = ColiCourseIO.LoadAllStages(settings.StageDir, "Loading Stages...");
        //    var scenes = sceneIterator.ToArray();
        //    var output = settings.AnalysisOutput + "/" + AnalyzerUtility.GetFileTimestamp() + "/"; // TODO: make folder with timestamp instead of files
        //    Directory.CreateDirectory(output);
        //    action.Invoke(scenes, output);
        //    OSUtility.OpenDirectory(settings.AnalysisOutput);
        //}



        public static void Analyze(ColiScene[] scenes, string outputPath)
        {
            var time = AnalyzerUtility.GetFileTimestamp();

            {
                var fileName = $"{time} COLI General Data.tsv";
                var filePath = Path.Combine(outputPath, fileName);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, RandomTime);
                AnalyzeGeneralData(scenes, filePath);
            }

            {
                // FOG parameter
                {
                    var fileName = $"{time} COLI {nameof(Fog)}.tsv";
                    var filePath = Path.Combine(outputPath, fileName);
                    EditorUtility.DisplayProgressBar(ExecuteText, filePath, RandomTime);
                    AnalyzeFog(scenes, filePath);
                }

                // FOG CURVE which is based off of FOG parameter
                {
                    var fileName = $"{time} COLI {nameof(FogCurves)}.tsv";
                    var filePath = Path.Combine(outputPath, fileName);
                    EditorUtility.DisplayProgressBar(ExecuteText, filePath, RandomTime);
                    AnalyzeFogCurves(scenes, filePath);
                }
            }

            {
                string filePath = string.Empty;

                filePath = $"{time} COLI {nameof(ColiScene)} header.tsv";
                filePath = Path.Combine(outputPath, filePath);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, RandomTime);
                AnalyzeHeaders(scenes, filePath);
            }

            {
                // ANIMATIONS
                {
                    var fileName = $"{time} COLI {nameof(AnimationClip)}.tsv";
                    var filePath = Path.Combine(outputPath, fileName);
                    EditorUtility.DisplayProgressBar(ExecuteText, filePath, RandomTime);
                    AnalyzeGameObjectAnimationClips(scenes, filePath);
                }

                // ANIMATIONS INDIVIDUALIZED
                {
                    var count = AnimationClip.kSizeCurvesPtrs;
                    for (int i = 0; i < count; i++)
                    {
                        var fileName = $"{time} COLI {nameof(AnimationClip)} {i}.tsv";
                        var filePath = Path.Combine(outputPath, fileName);
                        EditorUtility.DisplayProgressBar(ExecuteText, filePath, (float)(i + 1) / count);
                        AnalyzeGameObjectAnimationClipIndex(scenes, filePath, i);
                    }
                }
            }

            {
                string fileName = $"{time} COLI Compare {nameof(Transform)}.tsv";
                string filePath = Path.Combine(outputPath, fileName);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, RandomTime);
                AnalyzeSceneObjectTransforms(scenes, filePath);
            }

            {
                // AX ARCADE CHECKPOINTS
                {
                    string fileName = $"{time} COLI {nameof(ArcadeCheckpointTrigger)}.tsv";
                    string filePath = Path.Combine(outputPath, fileName);
                    EditorUtility.DisplayProgressBar(ExecuteText, filePath, RandomTime);
                    AnalyzeArcadeCheckpointTriggers(scenes, filePath);
                }

                // COURSE METADATA TRIGGERS
                {
                    string fileName = $"{time} COLI {nameof(CourseMetadataTrigger)}.tsv";
                    string filePath = Path.Combine(outputPath, fileName);
                    EditorUtility.DisplayProgressBar(ExecuteText, filePath, RandomTime);
                    AnalyzeCourseMetadataTriggers(scenes, filePath);
                }

                // STORY OBJECT TRIGGERS
                {
                    string fileName = $"{time} COLI {nameof(StoryObjectTrigger)}.tsv";
                    string filePath = Path.Combine(outputPath, fileName);
                    EditorUtility.DisplayProgressBar(ExecuteText, filePath, RandomTime);
                    AnalyzeStoryObjectTrigger(scenes, filePath);
                }

                // UNKNOWN TRIGGERS
                {
                    string fileName = $"{time} COLI {nameof(UnknownTrigger)}.tsv";
                    string filePath = Path.Combine(outputPath, fileName);
                    EditorUtility.DisplayProgressBar(ExecuteText, filePath, RandomTime);
                    AnalyzeUnknownTrigger(scenes, filePath);
                }

                // UNKNOWN SOLS TRIGGERS
                {
                    string fileName = $"{time} COLI {nameof(UnknownCollider)}.tsv";
                    string filePath = Path.Combine(outputPath, fileName);
                    EditorUtility.DisplayProgressBar(ExecuteText, filePath, RandomTime);
                    AnalyzeUnknownSolsTriggers(scenes, filePath);
                }

                // VFX TRIGGERS
                {
                    string fileName = $"{time} COLI {nameof(VisualEffectTrigger)}.tsv";
                    string filePath = Path.Combine(outputPath, fileName);
                    EditorUtility.DisplayProgressBar(ExecuteText, filePath, RandomTime);
                    AnalyzeVisualEffectTriggers(scenes, filePath);
                }
            }

            //GAME OBJECTS
            {
                {
                    var fileName = $"{time} COLI {nameof(SceneObjectDynamic)}.tsv";
                    var filePath = Path.Combine(outputPath, fileName);
                    EditorUtility.DisplayProgressBar(ExecuteText, filePath, .1f);
                    AnalyzeSceneObjects(scenes, filePath);
                }

                {
                    var fileName = $"{time} COLI {nameof(SceneObjectDynamic)} {nameof(TextureMetadata)}.tsv";
                    var filePath = Path.Combine(outputPath, fileName);
                    EditorUtility.DisplayProgressBar(ExecuteText, filePath, .3f);
                    AnalyzeTextureMetadata(scenes, filePath);
                }

                {
                    var fileName = $"{time} COLI {nameof(SceneObjectDynamic)} {nameof(SkeletalAnimator)}.tsv";
                    var filePath = Path.Combine(outputPath, fileName);
                    EditorUtility.DisplayProgressBar(ExecuteText, filePath, .5f);
                    AnalyzeSceneObjectsSkeletalAnimator(scenes, filePath);
                }

                {
                    var fileName = $"{time} COLI {nameof(SceneObjectDynamic)} {nameof(ColliderTriangle)}.tsv";
                    var filePath = Path.Combine(outputPath, fileName);
                    EditorUtility.DisplayProgressBar(ExecuteText, filePath, .7f);
                    AnalyzeSceneObjectsCollisionTri(scenes, filePath);
                }

                {
                    var fileName = $"{time} COLI {nameof(SceneObjectDynamic)} {nameof(ColliderQuad)}.tsv";
                    var filePath = Path.Combine(outputPath, fileName);
                    EditorUtility.DisplayProgressBar(ExecuteText, filePath, .9f);
                    AnalyzeSceneObjectsCollisionQuad(scenes, filePath);
                }
            }

            {
                {
                    var fileName = $"{time} COLI {nameof(SceneObject)}.tsv";
                    var filePath = Path.Combine(outputPath, fileName);
                    EditorUtility.DisplayProgressBar(ExecuteText, filePath, .33f);
                    AnalyzeSceneObjectReferences(scenes, filePath);
                }

                {
                    var fileName = $"{time} COLI {nameof(SceneObjectTemplate)}.tsv";
                    var filePath = Path.Combine(outputPath, fileName);
                    EditorUtility.DisplayProgressBar(ExecuteText, filePath, .66f);
                    AnalyzeSceneInstanceReferences(scenes, filePath);
                }

                {
                    var fileName = $"{time} COLI {nameof(SceneObjectTemplate)} and {nameof(SceneObject)}.tsv";
                    var filePath = Path.Combine(outputPath, fileName);
                    EditorUtility.DisplayProgressBar(ExecuteText, filePath, 1f);
                    AnalyzeSceneInstanceAndObjectReferences(scenes, filePath);
                }
            }

            // TOPOLOGY PARAMETERS
            {
                var count = TrackCurves.kCurveCount;
                for (int i = 0; i < count; i++)
                {
                    var fileName = $"{time} COLI {nameof(TrackCurves)} {i + 1}.tsv";
                    var filePath = Path.Combine(outputPath, fileName);
                    EditorUtility.DisplayProgressBar(ExecuteText, filePath, (float)(i + 1) / TrackCurves.kCurveCount);
                    AnalyzeTrackKeyables(scenes, filePath, i);
                }

                {
                    var fileName = $"{time} COLI {nameof(TrackCurves)} ALL.tsv";
                    var filePath = Path.Combine(outputPath, fileName);
                    EditorUtility.DisplayProgressBar(ExecuteText, filePath, RandomTime);
                    AnalyzeTrackKeyablesAll(scenes, filePath);
                }
            }

            // TRACK TRANSFORMS
            {
                var filePath = $"{time} COLI {nameof(TrackSegment)}.tsv";
                filePath = Path.Combine(outputPath, filePath);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, RandomTime);
                AnalyzeTrackTransforms(scenes, filePath);
            }

            //
            {
                var fileName = $"{time} COLI {nameof(StaticColliderMap)}.tsv";
                var filePath = Path.Combine(outputPath, fileName);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, RandomTime);
                AnalyzeSceneStaticCollider(scenes, filePath);
            }

            {
                var fileName = $"{time} COLI {nameof(SurfaceAttributeArea)}.tsv";
                var filePath = Path.Combine(outputPath, fileName);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, RandomTime);
                AnalyzeSurfaceAttributeAreas(scenes, filePath);
            }

            // OPEN FOLDER after analysis
            if (true)
            {
                OSUtility.OpenDirectory(outputPath + "/");
            }
            EditorUtility.ClearProgressBar();
        }


        #region Track Data / Transforms

        public static void AnalyzeTrackKeyablesAll(ColiScene[] scenes, string filename)
        {
            using (var writer = AnalyzerUtility.OpenWriter(filename))
            {
                // Write header
                writer.WriteNextCol("FileName");
                writer.WriteNextCol("Game");

                writer.WriteNextCol(nameof(TrackSegment.topologyMetadata));
                writer.WriteNextCol(nameof(TrackSegment.trackProperty));
                writer.WriteNextCol(nameof(TrackSegment.perimeterOptions));
                writer.WriteNextCol(nameof(TrackSegment.pipeCylinderOptions));
                writer.WriteNextCol(nameof(TrackSegment.unk_0x38));
                writer.WriteNextCol(nameof(TrackSegment.unk_0x3A));

                writer.WriteNextCol("TrackTransform Index");
                writer.WriteNextCol("Keyable /9");
                writer.WriteNextCol("Keyable Index");
                writer.WriteNextCol("Keyable Order");
                writer.WriteNextCol("Nested Depth");
                writer.WriteNextCol("Address");
                writer.WriteNextCol(nameof(KeyableAttribute.easeMode));
                writer.WriteNextCol(nameof(KeyableAttribute.easeMode));
                writer.WriteNextCol(nameof(KeyableAttribute.time));
                writer.WriteNextCol(nameof(KeyableAttribute.value));
                writer.WriteNextCol(nameof(KeyableAttribute.zTangentIn));
                writer.WriteNextCol(nameof(KeyableAttribute.zTangentOut));
                writer.WriteNextRow();

                // foreach File
                foreach (var scene in scenes)
                {
                    // foreach Transform
                    int trackIndex = 0;
                    foreach (var trackTransform in scene.rootTrackSegments)
                    {
                        for (int keyablesIndex = 0; keyablesIndex < TrackCurves.kCurveCount; keyablesIndex++)
                        {
                            WriteTrackKeyableAttributeRecursive(writer, scene, 0, keyablesIndex, ++trackIndex, trackTransform);
                        }
                    }
                }

                writer.Flush();
            }
        }

        public static void AnalyzeTrackKeyables(ColiScene[] scenes, string filename, int keyablesSet)
        {
            using (var writer = AnalyzerUtility.OpenWriter(filename))
            {
                // Write header
                writer.WriteNextCol("FileName");
                writer.WriteNextCol("Game");

                writer.WriteNextCol(nameof(TrackSegment.topologyMetadata));
                writer.WriteNextCol(nameof(TrackSegment.trackProperty));
                writer.WriteNextCol(nameof(TrackSegment.perimeterOptions));
                writer.WriteNextCol(nameof(TrackSegment.pipeCylinderOptions));
                writer.WriteNextCol(nameof(TrackSegment.unk_0x38));
                writer.WriteNextCol(nameof(TrackSegment.unk_0x3A));

                writer.WriteNextCol("TrackTransform Index");
                writer.WriteNextCol("Keyable /9");
                writer.WriteNextCol("Keyable Index");
                writer.WriteNextCol("Keyable Order");
                writer.WriteNextCol("Nested Depth");
                writer.WriteNextCol("Address");
                writer.WriteNextCol(nameof(KeyableAttribute.easeMode));
                writer.WriteNextCol(nameof(KeyableAttribute.easeMode));
                writer.WriteNextCol(nameof(KeyableAttribute.time));
                writer.WriteNextCol(nameof(KeyableAttribute.value));
                writer.WriteNextCol(nameof(KeyableAttribute.zTangentIn));
                writer.WriteNextCol(nameof(KeyableAttribute.zTangentOut));
                writer.WriteNextRow();

                // foreach File
                foreach (var scene in scenes)
                {
                    // foreach Transform
                    int trackTransformIndex = 0;
                    foreach (var trackTransform in scene.rootTrackSegments)
                    {
                        WriteTrackKeyableAttributeRecursive(writer, scene, nestedDepth: 0, keyablesSet, trackTransformIndex++, trackTransform);
                    }
                }

                writer.Flush();
            }
        }
        public static void WriteTrackKeyableAttributeRecursive(StreamWriter writer, ColiScene scene, int nestedDepth, int animationCurveIndex, int trackTransformIndex, TrackSegment trackTransform)
        {
            var animationCurves = trackTransform.trackCurves.animationCurves;
            var keyableIndex = 1; // 0-n, depends on number of keyables in array
            int keyableTotal = animationCurves[animationCurveIndex].Length;

            // Animation data of this curve
            foreach (var keyables in animationCurves[animationCurveIndex].keyableAttributes)
            {
                WriteKeyableAttribute(writer, scene, nestedDepth + 1, keyableIndex++, keyableTotal, animationCurveIndex, trackTransformIndex, keyables, trackTransform);
            }

            // TODO: do you even care to reimplement this at this point?
            // Go to track transform children, write their anim data (calls this function)
            //Debug.LogWarning("You refactored this analysis out!");
            //foreach (var child in trackTransform.children)
            //    WriteTrackKeyableAttributeRecursive(writer, sobj, nestedDepth + 1, animationCurveIndex, trackTransformIndex, child);
        }

        public static void WriteKeyableAttribute(StreamWriter writer, ColiScene scene, int nestedDepth, int keyableIndex, int keyableTotal, int keyablesSet, int trackTransformIndex, KeyableAttribute param, TrackSegment tt)
        {
            string gameId = scene.IsFileGX ? "GX" : "AX";

            writer.WriteNextCol(scene.FileName);
            writer.WriteNextCol(gameId);

            writer.WriteNextCol(tt.topologyMetadata);
            writer.WriteNextCol(tt.trackProperty);
            writer.WriteNextCol(tt.perimeterOptions);
            writer.WriteNextCol(tt.pipeCylinderOptions);
            writer.WriteNextCol(tt.unk_0x38);
            writer.WriteNextCol(tt.unk_0x3A);

            writer.WriteNextCol(trackTransformIndex);
            writer.WriteNextCol(keyablesSet);
            writer.WriteNextCol(keyableIndex);
            writer.WriteNextCol($"[{keyableIndex}/{keyableTotal}]");
            writer.WriteNextCol($"{nestedDepth}");
            writer.WriteNextCol(param.StartAddressHex());
            writer.WriteNextCol(param.easeMode);
            writer.WriteNextCol((int)param.easeMode);
            writer.WriteNextCol(param.time);
            writer.WriteNextCol(param.value);
            writer.WriteNextCol(param.zTangentIn);
            writer.WriteNextCol(param.zTangentOut);
            writer.WriteNextRow();
        }


        // Kicks off recursive write
        private static int s_order;
        public static void AnalyzeTrackTransforms(ColiScene[] scenes, string filename)
        {
            using (var writer = AnalyzerUtility.OpenWriter(filename))
            {
                writer.WriteNextCol("Filename");
                writer.WriteNextCol("Order");
                writer.WriteNextCol("Root Index");
                writer.WriteNextCol("Transform Depth");
                writer.WriteNextCol("Address");
                writer.WriteNextCol(nameof(TrackSegment.topologyMetadata));
                writer.WriteNextCol(nameof(TrackSegment.trackProperty));
                writer.WriteNextCol(nameof(TrackSegment.perimeterOptions));
                writer.WriteNextCol(nameof(TrackSegment.pipeCylinderOptions));
                writer.WriteNextCol(nameof(TrackSegment.trackCurvesPtr));
                writer.WriteNextCol(nameof(TrackSegment.trackCornerPtr));
                writer.WriteNextCol(nameof(TrackSegment.childrenPtrs));
                writer.WriteNextCol(nameof(TrackSegment.localScale));
                writer.WriteNextCol(nameof(TrackSegment.localRotation));
                writer.WriteNextCol(nameof(TrackSegment.localPosition));
                writer.WriteNextCol(nameof(TrackSegment.unk_0x38));
                writer.WriteNextCol(nameof(TrackSegment.unk_0x39));
                writer.WriteNextCol(nameof(TrackSegment.unk_0x3A));
                writer.WriteNextCol(nameof(TrackSegment.unk_0x3B));
                writer.WriteNextCol(nameof(TrackSegment.railHeightRight));
                writer.WriteNextCol(nameof(TrackSegment.railHeightLeft));
                writer.WriteNextCol(nameof(TrackSegment.zero_0x44));
                writer.WriteNextCol(nameof(TrackSegment.zero_0x48));
                writer.WriteNextCol(nameof(TrackSegment.unk_0x4C));
                writer.WriteNextCol();
                writer.WriteNextColNicify(nameof(TrackCorner.width));
                writer.WriteNextColNicify(nameof(TrackCorner.perimeterOptions));
                //
                writer.WriteNextRow();

                // RESET static variable
                s_order = 0;

                foreach (var scene in scenes)
                {
                    var index = 0;
                    var total = scene.rootTrackSegments.Length;
                    foreach (var trackTransform in scene.rootTrackSegments)
                    {
                        WriteTrackTransformRecursive(writer, scene, 0, ++index, total, trackTransform);
                    }
                }

                writer.Flush();
            }
        }
        // Writes self and children
        public static void WriteTrackTransformRecursive(StreamWriter writer, ColiScene scene, int depth, int index, int total, TrackSegment trackTransform)
        {
            // Write Parent
            WriteTrackTransform(writer, scene, depth, index, total, trackTransform);

            // Write children
            //foreach (var child in trackTransform.children)
            //{
            //    WriteTrackTransformRecursive(writer, sobj, depth + 1, index, total, child);
            //}
        }
        // The actual writing to file
        public static void WriteTrackTransform(StreamWriter writer, ColiScene scene, int depth, int index, int total, TrackSegment trackTransform)
        {
            writer.WriteNextCol(scene.FileName);
            writer.WriteNextCol($"{s_order++}");
            writer.WriteNextCol($"[{index}/{total}]");
            writer.WriteNextCol($"{depth}");
            writer.WriteNextCol(trackTransform.StartAddressHex());
            writer.WriteNextCol(trackTransform.topologyMetadata);
            writer.WriteNextCol(trackTransform.trackProperty);
            writer.WriteNextCol(trackTransform.perimeterOptions);
            writer.WriteNextCol(trackTransform.pipeCylinderOptions);
            writer.WriteNextCol(trackTransform.trackCurvesPtr);
            writer.WriteNextCol(trackTransform.trackCornerPtr);
            writer.WriteNextCol(trackTransform.childrenPtrs);
            writer.WriteNextCol(trackTransform.localScale);
            writer.WriteNextCol(trackTransform.localRotation);
            writer.WriteNextCol(trackTransform.localPosition);
            writer.WriteNextCol(trackTransform.unk_0x38);
            writer.WriteNextCol(trackTransform.unk_0x39);
            writer.WriteNextCol(trackTransform.unk_0x3A);
            writer.WriteNextCol(trackTransform.unk_0x3B);
            writer.WriteNextCol(trackTransform.railHeightRight);
            writer.WriteNextCol(trackTransform.railHeightLeft);
            writer.WriteNextCol(trackTransform.zero_0x44);
            writer.WriteNextCol(trackTransform.zero_0x48);
            writer.WriteNextCol(trackTransform.unk_0x4C);
            //
            if (trackTransform.trackCornerPtr.IsNotNullPointer)
            {
                writer.WriteNextCol();
                writer.WriteNextCol(trackTransform.trackCorner.width);
                writer.WriteNextCol(trackTransform.trackCorner.perimeterOptions);
            }
            //
            writer.WriteNextRow();
        }


        #endregion

        #region GameObject Animations

        public static void AnalyzeGameObjectAnimationClips(ColiScene[] scenes, string filename)
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

                foreach (var scene in scenes)
                {
                    int gameObjectIndex = 0;
                    foreach (var gameObject in scene.dynamicSceneObjects)
                    {
                        if (gameObject.animationClip == null)
                            continue;
                        if (gameObject.animationClip.curves == null)
                            continue;

                        int animIndex = 0;
                        foreach (var animationClipCurve in gameObject.animationClip.curves)
                        {
                            if (animationClipCurve.animationCurve == null)
                                continue;

                            foreach (var keyable in animationClipCurve.animationCurve.keyableAttributes)
                            {
                                writer.WriteNextCol(scene.FileName);
                                writer.WriteNextCol(gameObjectIndex);
                                writer.WriteNextCol(gameObject.nameCopy);
                                writer.WriteNextCol(animationClipCurve.StartAddressHex());
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

        public static void AnalyzeGameObjectAnimationClipIndex(ColiScene[] scenes, string filename, int index)
        {
            using (var writer = AnalyzerUtility.OpenWriter(filename))
            {
                // Write header
                writer.WriteNextCol("File Path");
                writer.WriteNextCol("Game Object #");
                writer.WriteNextCol("Game Object");
                writer.WriteNextCol("Anim Addr");
                writer.WriteNextCol("Key Addr");
                writer.WriteNextColNicify(nameof(AnimationClipCurve.unk_0x00));
                writer.WriteNextColNicify(nameof(AnimationClipCurve.unk_0x04));
                writer.WriteNextColNicify(nameof(AnimationClipCurve.unk_0x08));
                writer.WriteNextColNicify(nameof(AnimationClipCurve.unk_0x0C));
                writer.WriteNextCol("AnimClip Metadata");
                writer.WriteNextCol("AnimClip Metadata");
                writer.WriteNextCol("AnimClip Metadata");
                writer.WriteNextCol("Anim Index [0-10]");
                writer.WriteNextColNicify(nameof(KeyableAttribute.easeMode));
                writer.WriteNextColNicify(nameof(KeyableAttribute.time));
                writer.WriteNextColNicify(nameof(KeyableAttribute.value));
                writer.WriteNextColNicify(nameof(KeyableAttribute.zTangentIn));
                writer.WriteNextColNicify(nameof(KeyableAttribute.zTangentOut));
                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    int objIndex = 0;
                    foreach (var dynamicSceneObject in scene.dynamicSceneObjects)
                    {
                        if (dynamicSceneObject.animationClip == null)
                            continue;
                        //if (gameObject.animation.curve == null)
                        //    continue;

                        int animIndex = 0;
                        foreach (var animationClipCurve in dynamicSceneObject.animationClip.curves)
                        {
                            // Failing for some reason on indexes 6+ :/
                            if (animationClipCurve.animationCurve == null)
                                continue;

                            foreach (var keyable in animationClipCurve.animationCurve.keyableAttributes)
                            {
                                /// HACK, write each anim index as separate file
                                if (animIndex != index)
                                    continue;

                                writer.WriteNextCol(scene.FileName);
                                writer.WriteNextCol(objIndex);
                                writer.WriteNextCol(dynamicSceneObject.nameCopy);
                                writer.WriteNextCol(animationClipCurve.StartAddressHex());
                                writer.WriteNextCol(keyable.StartAddressHex());
                                writer.WriteNextCol(animationClipCurve.unk_0x00);
                                writer.WriteNextCol(animationClipCurve.unk_0x04);
                                writer.WriteNextCol(animationClipCurve.unk_0x08);
                                writer.WriteNextCol(animationClipCurve.unk_0x0C);
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
                        objIndex++;
                    }
                }
                writer.Flush();
            }
        }

        #endregion

        #region GameObjects

        public static void AnalyzeSceneObjects(ColiScene[] scenes, string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Game Object #");
                writer.WriteNextCol("Game Object");
                writer.WriteNextCol(nameof(SceneObjectDynamic.unk0x00));
                writer.WriteNextCol(nameof(SceneObjectDynamic.unk0x00));
                writer.WriteNextCol(nameof(SceneObjectDynamic.unk0x04));
                writer.WriteNextCol(nameof(SceneObjectDynamic.unk0x04));
                writer.WriteNextCol(nameof(SceneObjectDynamic.templateSceneObjectPtr));
                writer.WriteNextCol(nameof(SceneObjectDynamic.transform.Position));
                writer.WriteNextCol(nameof(SceneObjectDynamic.transform.RotationEuler));
                writer.WriteNextCol(nameof(SceneObjectDynamic.transform.Scale));
                writer.WriteNextCol(nameof(SceneObjectDynamic.zero_0x2C));
                writer.WriteNextCol(nameof(SceneObjectDynamic.animationClipPtr));
                writer.WriteNextCol(nameof(SceneObjectDynamic.textureMetadataPtr));
                writer.WriteNextCol(nameof(SceneObjectDynamic.skeletalAnimatorPtr));
                writer.WriteNextCol(nameof(SceneObjectDynamic.transformMatrix3x4Ptr));
                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    int sceneObjectIndex = 0;
                    foreach (var sceneObject in scene.dynamicSceneObjects)
                    {
                        writer.WriteNextCol(scene.FileName);
                        writer.WriteNextCol(sceneObjectIndex);
                        writer.WriteNextCol(sceneObject.nameCopy);
                        writer.WriteNextCol(sceneObject.unk0x00);
                        writer.WriteNextCol($"0x{sceneObject.unk0x00:x8}");
                        writer.WriteNextCol(sceneObject.unk0x04);
                        writer.WriteNextCol($"0x{sceneObject.unk0x04:x8}");
                        writer.WriteNextCol(sceneObject.templateSceneObjectPtr.HexAddress);
                        writer.WriteNextCol(sceneObject.transform.Position);
                        writer.WriteNextCol(sceneObject.transform.RotationEuler);
                        writer.WriteNextCol(sceneObject.transform.Scale);
                        writer.WriteNextCol(sceneObject.zero_0x2C);
                        writer.WriteNextCol(sceneObject.animationClipPtr.HexAddress);
                        writer.WriteNextCol(sceneObject.textureMetadataPtr.HexAddress);
                        writer.WriteNextCol(sceneObject.skeletalAnimatorPtr.HexAddress);
                        writer.WriteNextCol(sceneObject.transformMatrix3x4Ptr.HexAddress);
                        writer.WriteNextRow();

                        sceneObjectIndex++;
                    }
                }
                writer.Flush();
            }
        }

        public static void AnalyzeTextureMetadata(ColiScene[] scenes, string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Game Object #");
                writer.WriteNextCol("Game Object");
                writer.WriteNextCol("Unknown 1 Index");
                writer.WriteNextColNicify(nameof(TextureMetadataField.x));
                writer.WriteNextColNicify(nameof(TextureMetadataField.y));
                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    int gameObjectIndex = 0;
                    foreach (var sceneObject in scene.dynamicSceneObjects)
                    {
                        if (sceneObject.textureMetadata == null)
                            continue;

                        int fieldArrayIndex = 0;
                        foreach (var field in sceneObject.textureMetadata.fields)
                        {
                            if (field == null)
                                return;

                            writer.WriteNextCol(scene.FileName);
                            writer.WriteNextCol(gameObjectIndex);
                            writer.WriteNextCol(sceneObject.nameCopy);
                            writer.WriteNextCol(fieldArrayIndex);
                            writer.WriteNextCol(field.x);
                            writer.WriteNextCol(field.y);
                            writer.WriteNextRow();
                            fieldArrayIndex++;
                        }
                        gameObjectIndex++;
                    }
                }
                writer.Flush();
            }
        }

        public static void AnalyzeSceneObjectsSkeletalAnimator(ColiScene[] scenes, string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Game Object #");
                writer.WriteNextCol("Game Object");

                writer.WriteNextColNicify(nameof(SkeletalAnimator.zero_0x00));
                writer.WriteNextColNicify(nameof(SkeletalAnimator.zero_0x04));
                writer.WriteNextColNicify(nameof(SkeletalAnimator.one_0x08));
                writer.WriteNextColNicify(nameof(SkeletalAnimator.propertiesPtr));

                writer.WriteNextColNicify(nameof(SkeletalProperties.unk_0x00));
                writer.WriteNextColNicify(nameof(SkeletalProperties.unk_0x04));
                writer.WriteFlagNames<EnumFlags32>();
                writer.WriteNextColNicify(nameof(SkeletalProperties.unk_0x08));
                writer.WriteFlagNames<EnumFlags32>();
                writer.WriteNextColNicify(nameof(SkeletalProperties.zero_0x0C));
                writer.WriteNextColNicify(nameof(SkeletalProperties.zero_0x10));
                writer.WriteNextColNicify(nameof(SkeletalProperties.zero_0x14));
                writer.WriteNextColNicify(nameof(SkeletalProperties.zero_0x18));
                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    int gameObjectIndex = 0;
                    foreach (var dynamicSceneObject in scene.dynamicSceneObjects)
                    {
                        if (dynamicSceneObject.skeletalAnimator == null)
                            continue;
                        if (dynamicSceneObject.skeletalAnimator.propertiesPtr.IsNull)
                            continue;

                        writer.WriteNextCol(scene.FileName);
                        writer.WriteNextCol(gameObjectIndex);
                        writer.WriteNextCol(dynamicSceneObject.nameCopy);

                        writer.WriteNextCol(dynamicSceneObject.skeletalAnimator.zero_0x00);
                        writer.WriteNextCol(dynamicSceneObject.skeletalAnimator.zero_0x04);
                        writer.WriteNextCol(dynamicSceneObject.skeletalAnimator.one_0x08);
                        writer.WriteNextCol(dynamicSceneObject.skeletalAnimator.propertiesPtr);

                        writer.WriteNextCol(dynamicSceneObject.skeletalAnimator.properties.unk_0x00);
                        writer.WriteNextCol(dynamicSceneObject.skeletalAnimator.properties.unk_0x04);
                        writer.WriteFlags(dynamicSceneObject.skeletalAnimator.properties.unk_0x04);
                        writer.WriteNextCol(dynamicSceneObject.skeletalAnimator.properties.unk_0x08);
                        writer.WriteFlags(dynamicSceneObject.skeletalAnimator.properties.unk_0x08);
                        writer.WriteNextCol(dynamicSceneObject.skeletalAnimator.properties.zero_0x0C);
                        writer.WriteNextCol(dynamicSceneObject.skeletalAnimator.properties.zero_0x10);
                        writer.WriteNextCol(dynamicSceneObject.skeletalAnimator.properties.zero_0x14);
                        writer.WriteNextCol(dynamicSceneObject.skeletalAnimator.properties.zero_0x18);
                        writer.WriteNextRow();

                        gameObjectIndex++;
                    }
                }
                writer.Flush();
            }
        }

        public static void AnalyzeSceneObjectsCollisionTri(ColiScene[] scenes, string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Game Object #");
                writer.WriteNextCol("Game Object");

                writer.WriteNextCol("Tri Index");
                writer.WriteNextCol("Addr");

                writer.WriteNextColNicify(nameof(ColliderTriangle.unk_0x00));
                writer.WriteNextColNicify(nameof(ColliderTriangle.normal) + ".x");
                writer.WriteNextColNicify(nameof(ColliderTriangle.normal) + ".y");
                writer.WriteNextColNicify(nameof(ColliderTriangle.normal) + ".z");
                writer.WriteNextColNicify(nameof(ColliderTriangle.vertex0) + ".x");
                writer.WriteNextColNicify(nameof(ColliderTriangle.vertex0) + ".y");
                writer.WriteNextColNicify(nameof(ColliderTriangle.vertex0) + ".z");
                writer.WriteNextColNicify(nameof(ColliderTriangle.vertex1) + ".x");
                writer.WriteNextColNicify(nameof(ColliderTriangle.vertex1) + ".y");
                writer.WriteNextColNicify(nameof(ColliderTriangle.vertex1) + ".z");
                writer.WriteNextColNicify(nameof(ColliderTriangle.vertex2) + ".x");
                writer.WriteNextColNicify(nameof(ColliderTriangle.vertex2) + ".y");
                writer.WriteNextColNicify(nameof(ColliderTriangle.vertex2) + ".z");
                writer.WriteNextColNicify(nameof(ColliderTriangle.precomputed0) + ".x");
                writer.WriteNextColNicify(nameof(ColliderTriangle.precomputed0) + ".y");
                writer.WriteNextColNicify(nameof(ColliderTriangle.precomputed0) + ".z");
                writer.WriteNextColNicify(nameof(ColliderTriangle.precomputed1) + ".x");
                writer.WriteNextColNicify(nameof(ColliderTriangle.precomputed1) + ".y");
                writer.WriteNextColNicify(nameof(ColliderTriangle.precomputed1) + ".z");
                writer.WriteNextColNicify(nameof(ColliderTriangle.precomputed2) + ".x");
                writer.WriteNextColNicify(nameof(ColliderTriangle.precomputed2) + ".y");
                writer.WriteNextColNicify(nameof(ColliderTriangle.precomputed2) + ".z");

                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    int gameObjectIndex = 0;
                    foreach (var dynamicSceneObject in scene.dynamicSceneObjects)
                    {
                        if (dynamicSceneObject.templateSceneObject.colliderGeometry == null)
                            continue;
                        if (dynamicSceneObject.templateSceneObject.colliderGeometry.triCount == 0)
                            continue;

                        int triIndex = 0;
                        foreach (var tri in dynamicSceneObject.templateSceneObject.colliderGeometry.tris)
                        {
                            writer.WriteNextCol(scene.FileName);
                            writer.WriteNextCol(gameObjectIndex);
                            writer.WriteNextCol(dynamicSceneObject.nameCopy);

                            writer.WriteNextCol(triIndex++);
                            writer.WriteStartAddress(tri);

                            writer.WriteNextCol(tri.unk_0x00);
                            writer.WriteNextCol(tri.normal.x);
                            writer.WriteNextCol(tri.normal.y);
                            writer.WriteNextCol(tri.normal.z);
                            writer.WriteNextCol(tri.vertex0.x);
                            writer.WriteNextCol(tri.vertex0.y);
                            writer.WriteNextCol(tri.vertex0.z);
                            writer.WriteNextCol(tri.vertex1.x);
                            writer.WriteNextCol(tri.vertex1.y);
                            writer.WriteNextCol(tri.vertex1.z);
                            writer.WriteNextCol(tri.vertex2.x);
                            writer.WriteNextCol(tri.vertex2.y);
                            writer.WriteNextCol(tri.vertex2.z);
                            writer.WriteNextCol(tri.precomputed0.x);
                            writer.WriteNextCol(tri.precomputed0.y);
                            writer.WriteNextCol(tri.precomputed0.z);
                            writer.WriteNextCol(tri.precomputed1.x);
                            writer.WriteNextCol(tri.precomputed1.y);
                            writer.WriteNextCol(tri.precomputed1.z);
                            writer.WriteNextCol(tri.precomputed2.x);
                            writer.WriteNextCol(tri.precomputed2.y);
                            writer.WriteNextCol(tri.precomputed2.z);

                            writer.WriteNextRow();
                        }
                        gameObjectIndex++;
                    }
                }
                writer.Flush();
            }
        }

        public static void AnalyzeSceneObjectsCollisionQuad(ColiScene[] scenes, string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Game Object #");
                writer.WriteNextCol("Game Object");

                writer.WriteNextCol("Quad Index");
                writer.WriteNextCol("Addr");

                writer.WriteNextColNicify(nameof(ColliderQuad.unk_0x00));
                writer.WriteNextColNicify(nameof(ColliderQuad.normal) + ".x");
                writer.WriteNextColNicify(nameof(ColliderQuad.normal) + ".y");
                writer.WriteNextColNicify(nameof(ColliderQuad.normal) + ".z");
                writer.WriteNextColNicify(nameof(ColliderQuad.vertex0) + ".x");
                writer.WriteNextColNicify(nameof(ColliderQuad.vertex0) + ".y");
                writer.WriteNextColNicify(nameof(ColliderQuad.vertex0) + ".z");
                writer.WriteNextColNicify(nameof(ColliderQuad.vertex1) + ".x");
                writer.WriteNextColNicify(nameof(ColliderQuad.vertex1) + ".y");
                writer.WriteNextColNicify(nameof(ColliderQuad.vertex1) + ".z");
                writer.WriteNextColNicify(nameof(ColliderQuad.vertex2) + ".x");
                writer.WriteNextColNicify(nameof(ColliderQuad.vertex2) + ".y");
                writer.WriteNextColNicify(nameof(ColliderQuad.vertex2) + ".z");
                writer.WriteNextColNicify(nameof(ColliderQuad.vertex3) + ".x");
                writer.WriteNextColNicify(nameof(ColliderQuad.vertex3) + ".y");
                writer.WriteNextColNicify(nameof(ColliderQuad.vertex3) + ".z");
                writer.WriteNextColNicify(nameof(ColliderQuad.precomputed0) + ".x");
                writer.WriteNextColNicify(nameof(ColliderQuad.precomputed0) + ".y");
                writer.WriteNextColNicify(nameof(ColliderQuad.precomputed0) + ".z");
                writer.WriteNextColNicify(nameof(ColliderQuad.precomputed1) + ".x");
                writer.WriteNextColNicify(nameof(ColliderQuad.precomputed1) + ".y");
                writer.WriteNextColNicify(nameof(ColliderQuad.precomputed1) + ".z");
                writer.WriteNextColNicify(nameof(ColliderQuad.precomputed2) + ".x");
                writer.WriteNextColNicify(nameof(ColliderQuad.precomputed2) + ".y");
                writer.WriteNextColNicify(nameof(ColliderQuad.precomputed2) + ".z");
                writer.WriteNextColNicify(nameof(ColliderQuad.precomputed3) + ".x");
                writer.WriteNextColNicify(nameof(ColliderQuad.precomputed3) + ".y");
                writer.WriteNextColNicify(nameof(ColliderQuad.precomputed3) + ".z");

                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    int gameObjectIndex = 0;
                    foreach (var dynamicSceneObject in scene.dynamicSceneObjects)
                    {
                        if (dynamicSceneObject.templateSceneObject.colliderGeometry == null)
                            continue;
                        if (dynamicSceneObject.templateSceneObject.colliderGeometry.quadCount == 0)
                            continue;

                        int quadIndex = 0;
                        foreach (var quad in dynamicSceneObject.templateSceneObject.colliderGeometry.quads)
                        {
                            writer.WriteNextCol(scene.FileName);
                            writer.WriteNextCol(gameObjectIndex);
                            writer.WriteNextCol(dynamicSceneObject.nameCopy);

                            writer.WriteNextCol(quadIndex++);
                            writer.WriteStartAddress(quad);

                            writer.WriteNextCol(quad.unk_0x00);
                            writer.WriteNextCol(quad.normal.x);
                            writer.WriteNextCol(quad.normal.y);
                            writer.WriteNextCol(quad.normal.z);
                            writer.WriteNextCol(quad.vertex0.x);
                            writer.WriteNextCol(quad.vertex0.y);
                            writer.WriteNextCol(quad.vertex0.z);
                            writer.WriteNextCol(quad.vertex1.x);
                            writer.WriteNextCol(quad.vertex1.y);
                            writer.WriteNextCol(quad.vertex1.z);
                            writer.WriteNextCol(quad.vertex2.x);
                            writer.WriteNextCol(quad.vertex2.y);
                            writer.WriteNextCol(quad.vertex2.z);
                            writer.WriteNextCol(quad.vertex3.x);
                            writer.WriteNextCol(quad.vertex3.y);
                            writer.WriteNextCol(quad.vertex3.z);
                            writer.WriteNextCol(quad.precomputed0.x);
                            writer.WriteNextCol(quad.precomputed0.y);
                            writer.WriteNextCol(quad.precomputed0.z);
                            writer.WriteNextCol(quad.precomputed1.x);
                            writer.WriteNextCol(quad.precomputed1.y);
                            writer.WriteNextCol(quad.precomputed1.z);
                            writer.WriteNextCol(quad.precomputed2.x);
                            writer.WriteNextCol(quad.precomputed2.y);
                            writer.WriteNextCol(quad.precomputed2.z);
                            writer.WriteNextCol(quad.precomputed3.x);
                            writer.WriteNextCol(quad.precomputed3.y);
                            writer.WriteNextCol(quad.precomputed3.z);

                            writer.WriteNextRow();
                        }
                        gameObjectIndex++;
                    }
                }
                writer.Flush();
            }
        }

        #endregion


        public static void AnalyzeHeaders(ColiScene[] scenes, string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Index");
                writer.WriteNextCol("Venue");
                writer.WriteNextCol("Course");
                writer.WriteNextCol("AX/GX");
                //
                writer.WriteNextCol(nameof(ColiScene.unkRange0x00) + "." + nameof(Range.near));
                writer.WriteNextCol(nameof(ColiScene.unkRange0x00) + "." + nameof(Range.far));
                writer.WriteNextCol(nameof(ColiScene.trackNodesPtr));
                writer.WriteNextCol(nameof(ColiScene.trackNodesPtr));
                writer.WriteNextCol(nameof(ColiScene.surfaceAttributeAreasPtr));
                writer.WriteNextCol(nameof(ColiScene.surfaceAttributeAreasPtr));
                writer.WriteNextCol(nameof(ColiScene.staticColliderMeshesActive));
                writer.WriteNextCol(nameof(ColiScene.surfaceAttributeAreasPtr));
                writer.WriteNextCol(nameof(ColiScene.zeroes0x20Ptr));
                writer.WriteNextCol(nameof(ColiScene.trackMinHeightPtr));
                writer.WriteNextCol(nameof(ColiScene.zeroes0x28));
                writer.WriteNextCol(nameof(ColiScene.dynamicSceneObjectCount));
                writer.WriteNextCol(nameof(ColiScene.unk_sceneObjectCount1));
                writer.WriteNextCol(nameof(ColiScene.unk_sceneObjectCount2));
                writer.WriteNextCol(nameof(ColiScene.dynamicSceneObjectsPtr));
                writer.WriteNextCol(nameof(ColiScene.unkBool32_0x58));
                writer.WriteNextCol(nameof(ColiScene.unknownCollidersPtr));
                writer.WriteNextCol(nameof(ColiScene.unknownCollidersPtr));
                writer.WriteNextCol(nameof(ColiScene.templateSceneObjectsPtr));
                writer.WriteNextCol(nameof(ColiScene.templateSceneObjectsPtr));
                writer.WriteNextCol(nameof(ColiScene.staticSceneObjectsPtr));
                writer.WriteNextCol(nameof(ColiScene.staticSceneObjectsPtr));
                writer.WriteNextCol(nameof(ColiScene.zero0x74));
                writer.WriteNextCol(nameof(ColiScene.zero0x78));
                writer.WriteNextCol(nameof(ColiScene.circuitType));
                writer.WriteNextCol(nameof(ColiScene.fogCurvesPtr));
                writer.WriteNextCol(nameof(ColiScene.fogPtr));
                writer.WriteNextCol(nameof(ColiScene.zero0x88));
                writer.WriteNextCol(nameof(ColiScene.zero0x8C));
                writer.WriteNextCol(nameof(ColiScene.trackLengthPtr));
                writer.WriteNextCol(nameof(ColiScene.unknownTriggersPtr)); // len
                writer.WriteNextCol(nameof(ColiScene.unknownTriggersPtr)); // adr
                writer.WriteNextCol(nameof(ColiScene.visualEffectTriggersPtr)); // len
                writer.WriteNextCol(nameof(ColiScene.visualEffectTriggersPtr)); // adr
                writer.WriteNextCol(nameof(ColiScene.courseMetadataTriggersPtr)); // len
                writer.WriteNextCol(nameof(ColiScene.courseMetadataTriggersPtr)); // adr
                writer.WriteNextCol(nameof(ColiScene.arcadeCheckpointTriggersPtr)); // len
                writer.WriteNextCol(nameof(ColiScene.arcadeCheckpointTriggersPtr)); // adr
                writer.WriteNextCol(nameof(ColiScene.storyObjectTriggersPtr)); // len
                writer.WriteNextCol(nameof(ColiScene.storyObjectTriggersPtr)); // adr
                writer.WriteNextCol(nameof(ColiScene.trackCheckpointMatrixPtr));
                // Structure
                writer.WriteNextCol(nameof(ColiScene.trackNodeBoundsXZ) + "." + nameof(ColiScene.trackNodeBoundsXZ.left));
                writer.WriteNextCol(nameof(ColiScene.trackNodeBoundsXZ) + "." + nameof(ColiScene.trackNodeBoundsXZ.top));
                writer.WriteNextCol(nameof(ColiScene.trackNodeBoundsXZ) + "." + nameof(ColiScene.trackNodeBoundsXZ.subdivionWidth));
                writer.WriteNextCol(nameof(ColiScene.trackNodeBoundsXZ) + "." + nameof(ColiScene.trackNodeBoundsXZ.subdivisionLength));
                writer.WriteNextCol(nameof(ColiScene.trackNodeBoundsXZ) + "." + nameof(ColiScene.trackNodeBoundsXZ.numSubdivisionsX));
                writer.WriteNextCol(nameof(ColiScene.trackNodeBoundsXZ) + "." + nameof(ColiScene.trackNodeBoundsXZ.numSubdivisionsZ));
                // 
                writer.WriteNextCol(nameof(ColiScene.zeroes0xD8));
                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    writer.WriteNextCol(scene.FileName);
                    writer.WriteNextCol(scene.ID);
                    writer.WriteNextCol(CourseUtility.GetVenueID(scene.ID).GetDescription());
                    writer.WriteNextCol(((CourseIndexAX)scene.ID).GetDescription());
                    writer.WriteNextCol(scene.IsFileGX ? "GX" : "AX");

                    writer.WriteNextCol(scene.unkRange0x00.near);
                    writer.WriteNextCol(scene.unkRange0x00.far);
                    writer.WriteNextCol(scene.trackNodesPtr.Length);
                    writer.WriteNextCol(scene.trackNodesPtr.HexAddress);
                    writer.WriteNextCol(scene.surfaceAttributeAreasPtr.Length);
                    writer.WriteNextCol(scene.surfaceAttributeAreasPtr.HexAddress);
                    writer.WriteNextCol(scene.staticColliderMeshesActive);
                    writer.WriteNextCol(scene.staticColliderMeshesPtr.HexAddress);
                    writer.WriteNextCol(scene.zeroes0x20Ptr.HexAddress);
                    writer.WriteNextCol(scene.trackMinHeightPtr.HexAddress);
                    writer.WriteNextCol(0);// coliHeader.zero_0x28);
                    writer.WriteNextCol(scene.dynamicSceneObjectCount);
                    if (scene.IsFileGX)
                    {
                        writer.WriteNextCol(scene.unk_sceneObjectCount1);
                    }
                    else // is AX
                    {
                        writer.WriteNextCol();
                    }
                    writer.WriteNextCol(scene.unk_sceneObjectCount2);
                    writer.WriteNextCol(scene.dynamicSceneObjectsPtr.HexAddress);
                    writer.WriteNextCol(scene.unkBool32_0x58);
                    writer.WriteNextCol(scene.unknownCollidersPtr.Length);
                    writer.WriteNextCol(scene.unknownCollidersPtr.HexAddress);
                    writer.WriteNextCol(scene.templateSceneObjectsPtr.Length);
                    writer.WriteNextCol(scene.templateSceneObjectsPtr.HexAddress);
                    writer.WriteNextCol(scene.staticSceneObjectsPtr.Length);
                    writer.WriteNextCol(scene.staticSceneObjectsPtr.HexAddress);
                    writer.WriteNextCol(scene.zero0x74);
                    writer.WriteNextCol(scene.zero0x78);
                    writer.WriteNextCol(scene.circuitType);
                    writer.WriteNextCol(scene.fogCurvesPtr.HexAddress);
                    writer.WriteNextCol(scene.fogPtr.HexAddress);
                    writer.WriteNextCol(scene.zero0x88);
                    writer.WriteNextCol(scene.zero0x8C);
                    writer.WriteNextCol(scene.trackLengthPtr.HexAddress);
                    writer.WriteNextCol(scene.unknownTriggersPtr.Length);
                    writer.WriteNextCol(scene.unknownTriggersPtr.HexAddress);
                    writer.WriteNextCol(scene.visualEffectTriggersPtr.Length);
                    writer.WriteNextCol(scene.visualEffectTriggersPtr.HexAddress);
                    writer.WriteNextCol(scene.courseMetadataTriggersPtr.Length);
                    writer.WriteNextCol(scene.courseMetadataTriggersPtr.HexAddress);
                    writer.WriteNextCol(scene.arcadeCheckpointTriggersPtr.Length);
                    writer.WriteNextCol(scene.arcadeCheckpointTriggersPtr.HexAddress);
                    writer.WriteNextCol(scene.storyObjectTriggersPtr.Length);
                    writer.WriteNextCol(scene.storyObjectTriggersPtr.HexAddress);
                    writer.WriteNextCol(scene.trackCheckpointMatrixPtr.HexAddress);
                    // Structure
                    writer.WriteNextCol(scene.trackNodeBoundsXZ.left);
                    writer.WriteNextCol(scene.trackNodeBoundsXZ.top);
                    writer.WriteNextCol(scene.trackNodeBoundsXZ.subdivionWidth);
                    writer.WriteNextCol(scene.trackNodeBoundsXZ.subdivisionLength);
                    writer.WriteNextCol(scene.trackNodeBoundsXZ.numSubdivisionsX);
                    writer.WriteNextCol(scene.trackNodeBoundsXZ.numSubdivisionsZ);
                    //
                    writer.WriteNextCol(0);// coliHeader.zero_0xD8);
                    writer.WriteNextCol(scene.trackMinHeight.value);
                    writer.WriteNextRow();
                }
                writer.Flush();
            }
        }


        #region TRIGGERS

        public static void AnalyzeArcadeCheckpointTriggers(ColiScene[] scenes, string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Index");
                writer.WriteNextCol("Venue");
                writer.WriteNextCol("Course");
                writer.WriteNextCol("AX/GX");
                //
                writer.WriteNextCol(nameof(ArcadeCheckpointTrigger.transform.Position));
                writer.WriteNextCol(nameof(ArcadeCheckpointTrigger.transform.RotationEuler));
                writer.WriteNextCol(nameof(ArcadeCheckpointTrigger.transform.Scale));
                writer.WriteNextCol(nameof(ArcadeCheckpointTrigger.transform.UnknownOption));
                writer.WriteNextCol(nameof(ArcadeCheckpointTrigger.type));
                //
                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    var venueID = CourseUtility.GetVenueID(scene.ID).GetDescription();
                    var courseID = ((CourseIndexAX)scene.ID).GetDescription();
                    var isAxGx = scene.IsFileGX ? "GX" : "AX";

                    foreach (var arcadeCheckpooint in scene.arcadeCheckpointTriggers)
                    {
                        writer.WriteNextCol(scene.FileName);
                        writer.WriteNextCol(scene.ID);
                        writer.WriteNextCol(venueID);
                        writer.WriteNextCol(courseID);
                        writer.WriteNextCol(isAxGx);
                        //
                        writer.WriteNextCol(arcadeCheckpooint.transform.Position);
                        writer.WriteNextCol(arcadeCheckpooint.transform.RotationEuler);
                        writer.WriteNextCol(arcadeCheckpooint.transform.Scale);
                        writer.WriteNextCol(arcadeCheckpooint.transform.UnknownOption);
                        writer.WriteNextCol(arcadeCheckpooint.type);
                        //
                        writer.WriteNextRow();
                    }
                    writer.Flush();
                }
            }
        }

        public static void AnalyzeCourseMetadataTriggers(ColiScene[] scenes, string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Index");
                writer.WriteNextCol("Venue");
                writer.WriteNextCol("Course");
                writer.WriteNextCol("AX/GX");
                //
                writer.WriteNextCol(nameof(CourseMetadataTrigger.Position));
                writer.WriteNextCol(nameof(CourseMetadataTrigger.RotationEuler));
                writer.WriteNextCol(nameof(CourseMetadataTrigger.Scale) + " / PositionTo");
                writer.WriteNextCol(nameof(CourseMetadataTrigger.transform.UnknownOption));
                writer.WriteNextCol(nameof(CourseMetadataTrigger.metadataType));
                //
                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    var venueID = CourseUtility.GetVenueID(scene.ID).GetDescription();
                    var courseID = ((CourseIndexAX)scene.ID).GetDescription();
                    var isAxGx = scene.IsFileGX ? "GX" : "AX";

                    foreach (var cmt in scene.courseMetadataTriggers)
                    {
                        writer.WriteNextCol(scene.FileName);
                        writer.WriteNextCol(scene.ID);
                        writer.WriteNextCol(venueID);
                        writer.WriteNextCol(courseID);
                        writer.WriteNextCol(isAxGx);
                        //
                        writer.WriteNextCol(cmt.Position);
                        writer.WriteNextCol(cmt.RotationEuler);
                        writer.WriteNextCol(cmt.Scale);
                        writer.WriteNextCol(cmt.transform.UnknownOption);
                        writer.WriteNextCol(cmt.metadataType);
                        //
                        writer.WriteNextRow();
                    }
                    writer.Flush();
                }
            }
        }

        public static void AnalyzeStoryObjectTrigger(ColiScene[] scenes, string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Index");
                writer.WriteNextCol("Venue");
                writer.WriteNextCol("Course");
                writer.WriteNextCol("AX/GX");
                //
                writer.WriteNextCol(nameof(StoryObjectTrigger.zero_0x00));
                writer.WriteNextCol(nameof(StoryObjectTrigger.rockGroupOrderIndex));
                writer.WriteNextCol(nameof(StoryObjectTrigger.RockGroup));
                writer.WriteNextCol(nameof(StoryObjectTrigger.Difficulty));
                writer.WriteNextCol(nameof(StoryObjectTrigger.story2RockScale));
                writer.WriteNextCol(nameof(StoryObjectTrigger.storyObjectPathPtr));
                writer.WriteNextCol(nameof(StoryObjectTrigger.scale));
                writer.WriteNextCol(nameof(StoryObjectTrigger.rotation));
                writer.WriteNextCol(nameof(StoryObjectTrigger.position));
                //
                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    var venueID = CourseUtility.GetVenueID(scene.ID).GetDescription();
                    var courseID = ((CourseIndexAX)scene.ID).GetDescription();
                    var isAxGx = scene.IsFileGX ? "GX" : "AX";

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
                        writer.WriteNextCol(item.storyObjectPathPtr);
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

        public static void AnalyzeUnknownTrigger(ColiScene[] scenes, string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Index");
                writer.WriteNextCol("Venue");
                writer.WriteNextCol("Course");
                writer.WriteNextCol("AX/GX");
                //
                writer.WriteNextCol("Start");
                writer.WriteNextCol("End");
                //
                writer.WriteNextCol(nameof(UnknownTrigger.unk_0x20));
                writer.WriteNextCol(nameof(UnknownTrigger.unk_0x20));
                //
                writer.WriteNextCol("Order");
                writer.WriteNextCol("Index");
                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    var venueID = CourseUtility.GetVenueID(scene.ID).GetDescription();
                    var courseID = ((CourseIndexAX)scene.ID).GetDescription();
                    var isAxGx = scene.IsFileGX ? "GX" : "AX";

                    int count = 0;
                    int total = scene.unknownTriggers.Length;
                    foreach (var item in scene.unknownTriggers)
                    {
                        count++;

                        writer.WriteNextCol(scene.FileName);
                        writer.WriteNextCol(scene.ID);
                        writer.WriteNextCol(venueID);
                        writer.WriteNextCol(courseID);
                        writer.WriteNextCol(isAxGx);

                        writer.WriteNextCol(item.StartAddressHex());
                        writer.WriteNextCol(item.EndAddressHex());

                        writer.WriteNextCol(item.unk_0x20);
                        writer.WriteNextCol($"0x{(int)item.unk_0x20:X8}");

                        writer.WriteNextCol(count);
                        writer.WriteNextCol($"[{count}/{total}]");

                        writer.WriteNextRow();
                    }
                }
                writer.Flush();
            }
        }

        public static void AnalyzeUnknownSolsTriggers(ColiScene[] scenes, string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Index");
                writer.WriteNextCol("Venue");
                writer.WriteNextCol("Course");
                writer.WriteNextCol("AX/GX");
                //
                writer.WriteNextCol(nameof(UnknownCollider.unk_0x00));
                writer.WriteNextCol(nameof(UnknownCollider.transform.Position));
                writer.WriteNextCol(nameof(UnknownCollider.transform.RotationEuler));
                writer.WriteNextCol(nameof(UnknownCollider.transform.Scale));
                writer.WriteNextCol(nameof(UnknownCollider.transform.UnknownOption));
                //
                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    var venueID = CourseUtility.GetVenueID(scene.ID).GetDescription();
                    var courseID = ((CourseIndexAX)scene.ID).GetDescription();
                    var isAxGx = scene.IsFileGX ? "GX" : "AX";

                    foreach (var unkSols in scene.unknownColliders)
                    {
                        writer.WriteNextCol(scene.FileName);
                        writer.WriteNextCol(scene.ID);
                        writer.WriteNextCol(venueID);
                        writer.WriteNextCol(courseID);
                        writer.WriteNextCol(isAxGx);
                        //
                        writer.WriteNextCol(unkSols.unk_0x00);
                        writer.WriteNextCol(unkSols.transform.Position);
                        writer.WriteNextCol(unkSols.transform.RotationEuler);
                        writer.WriteNextCol(unkSols.transform.Scale);
                        writer.WriteNextCol(unkSols.transform.UnknownOption);
                        //
                        writer.WriteNextRow();
                    }
                    writer.Flush();
                }
            }
        }

        public static void AnalyzeVisualEffectTriggers(ColiScene[] scenes, string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Index");
                writer.WriteNextCol("Venue");
                writer.WriteNextCol("Course");
                writer.WriteNextCol("AX/GX");
                //
                writer.WriteNextCol(nameof(VisualEffectTrigger.transform.Position));
                writer.WriteNextCol(nameof(VisualEffectTrigger.transform.RotationEuler));
                writer.WriteNextCol(nameof(VisualEffectTrigger.transform.Scale));
                writer.WriteNextCol(nameof(VisualEffectTrigger.transform.UnknownOption));
                writer.WriteNextCol(nameof(VisualEffectTrigger.animation));
                writer.WriteNextCol(nameof(VisualEffectTrigger.visualEffect));
                //
                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    var venueID = CourseUtility.GetVenueID(scene.ID).GetDescription();
                    var courseID = ((CourseIndexAX)scene.ID).GetDescription();
                    var isAxGx = scene.IsFileGX ? "GX" : "AX";

                    foreach (var vfx in scene.visualEffectTriggers)
                    {
                        writer.WriteNextCol(scene.FileName);
                        writer.WriteNextCol(scene.ID);
                        writer.WriteNextCol(venueID);
                        writer.WriteNextCol(courseID);
                        writer.WriteNextCol(isAxGx);
                        //
                        writer.WriteNextCol(vfx.transform.Position);
                        writer.WriteNextCol(vfx.transform.RotationEuler);
                        writer.WriteNextCol(vfx.transform.Scale);
                        writer.WriteNextCol(vfx.transform.UnknownOption);
                        writer.WriteNextCol(vfx.animation);
                        writer.WriteNextCol(vfx.visualEffect);
                        //
                        writer.WriteNextRow();
                    }
                    writer.Flush();
                }
            }
        }


        #endregion

        #region FOG
        public static void AnalyzeFogCurves(ColiScene[] scenes, string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Index");
                writer.WriteNextCol("Venue");
                writer.WriteNextCol("Course");
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

                foreach (var scene in scenes)
                {
                    if (scene.fogCurves == null)
                        continue;

                    var venueID = CourseUtility.GetVenueID(scene.ID).GetDescription();
                    var courseID = ((CourseIndexAX)scene.ID).GetDescription();
                    var isAxGx = scene.IsFileGX ? "GX" : "AX";

                    var totalD1 = scene.fogCurves.animationCurves.Length;
                    var countD1 = 0;
                    foreach (var animationCurve in scene.fogCurves.animationCurves)
                    {
                        countD1++;
                        foreach (var keyableAttribute in animationCurve.keyableAttributes)
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

        public static void AnalyzeFog(ColiScene[] scenes, string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Index");
                writer.WriteNextCol("Venue");
                writer.WriteNextCol("Course");
                writer.WriteNextCol("AX/GX");
                //
                writer.WriteNextCol(nameof(Fog.interpolation));
                writer.WriteNextCol(nameof(Fog.fogRange) + "." + nameof(Range.near));
                writer.WriteNextCol(nameof(Fog.fogRange) + "." + nameof(Range.far));
                writer.WriteNextCol(nameof(Fog.colorRGB) + ".R");
                writer.WriteNextCol(nameof(Fog.colorRGB) + ".G");
                writer.WriteNextCol(nameof(Fog.colorRGB) + ".B");
                writer.WriteNextCol(nameof(Fog.zero0x18) + ".x");
                writer.WriteNextCol(nameof(Fog.zero0x18) + ".y");
                writer.WriteNextCol(nameof(Fog.zero0x18) + ".z");
                //
                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    var venueID = CourseUtility.GetVenueID(scene.ID).GetDescription();
                    var courseID = ((CourseIndexAX)scene.ID).GetDescription();
                    var isAxGx = scene.IsFileGX ? "GX" : "AX";

                    writer.WriteNextCol(scene.FileName);
                    writer.WriteNextCol(scene.ID);
                    writer.WriteNextCol(venueID);
                    writer.WriteNextCol(courseID);
                    writer.WriteNextCol(isAxGx);
                    //
                    writer.WriteNextCol(scene.fog.interpolation);
                    writer.WriteNextCol(scene.fog.fogRange.near);
                    writer.WriteNextCol(scene.fog.fogRange.far);
                    writer.WriteNextCol(scene.fog.colorRGB.x);
                    writer.WriteNextCol(scene.fog.colorRGB.y);
                    writer.WriteNextCol(scene.fog.colorRGB.z);
                    writer.WriteNextCol(scene.fog.zero0x18.x);
                    writer.WriteNextCol(scene.fog.zero0x18.y);
                    writer.WriteNextCol(scene.fog.zero0x18.z);
                    //
                    writer.WriteNextRow();
                }
                writer.Flush();
            }
        }

        #endregion

        public static void AnalyzeSceneObjectTransforms(ColiScene[] scenes, string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Game Object #");
                writer.WriteNextCol("Game Object");
                writer.WriteNextCol($"matrix.x");
                writer.WriteNextCol($"matrix.y");
                writer.WriteNextCol($"matrix.z");
                writer.WriteNextCol($"euler.x");
                writer.WriteNextCol($"euler.y");
                writer.WriteNextCol($"euler.z");
                writer.WriteNextCol("Decomposed phi");
                writer.WriteNextCol("Decomposed theta");
                writer.WriteNextCol("Decomposed psi");
                writer.WriteNextCol(nameof(UnknownTransformOption));
                writer.WriteNextCol(nameof(ObjectActiveOverride));
                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    int sceneObjectIndex = 0;
                    foreach (var sceneObject in scene.dynamicSceneObjects)
                    {
                        // Skip objects that don;'t have both matrix and decomposed rotation
                        // These are not helpful for comparision
                        if (!sceneObject.transformMatrix3x4Ptr.IsNotNullPointer)
                            continue;

                        writer.WriteNextCol(scene.FileName);
                        writer.WriteNextCol(sceneObjectIndex);
                        writer.WriteNextCol(sceneObject.nameCopy);

                        // Rotation values from clean, uncompressed matrix
                        var matrix = sceneObject.transformMatrix3x4.rotationEuler;
                        writer.WriteNextCol(matrix.x);
                        writer.WriteNextCol(matrix.y);
                        writer.WriteNextCol(matrix.z);

                        // Rotation values as reconstructed
                        var euler = sceneObject.transform.DecomposedRotation.EulerAngles;
                        writer.WriteNextCol(euler.x);
                        writer.WriteNextCol(euler.y);
                        writer.WriteNextCol(euler.z);

                        // Decomposed rotation values, raw, requires processing to be used
                        var decomposed = sceneObject.transform.DecomposedRotation;
                        writer.WriteNextCol(decomposed.phi);
                        writer.WriteNextCol(decomposed.theta);
                        writer.WriteNextCol(decomposed.psi);
                        // The other parameters that go with the structure
                        writer.WriteNextCol(sceneObject.transform.UnknownOption);
                        writer.WriteNextCol(sceneObject.transform.ObjectActiveOverride);

                        writer.WriteNextRow();
                        sceneObjectIndex++;
                    }
                }
                writer.Flush();
            }
        }

        public static void AnalyzeTrackNodes(ColiScene[] scenes, string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Track Node");
                writer.WriteNextCol("Track Point");
                writer.WriteNextColNicify(nameof(TrackCheckpoint.curveTimeStart));
                writer.WriteNextColNicify(nameof(TrackCheckpoint.curveTimeEnd));
                writer.WriteNextColNicify(nameof(TrackCheckpoint.trackDistanceStart));
                writer.WriteNextColNicify(nameof(TrackCheckpoint.tangentStart));
                writer.WriteNextColNicify(nameof(TrackCheckpoint.positionStart));
                writer.WriteNextColNicify(nameof(TrackCheckpoint.trackDistanceEnd));
                writer.WriteNextColNicify(nameof(TrackCheckpoint.tangentEnd));
                writer.WriteNextColNicify(nameof(TrackCheckpoint.positionEnd));
                writer.WriteNextColNicify(nameof(TrackCheckpoint.transformDistanceEnd));
                writer.WriteNextColNicify(nameof(TrackCheckpoint.transformDistanceStart));
                writer.WriteNextColNicify(nameof(TrackCheckpoint.trackWidth));
                writer.WriteNextColNicify(nameof(TrackCheckpoint.isTrackContinuousStart));
                writer.WriteNextColNicify(nameof(TrackCheckpoint.isTrackContinuousEnd));
                writer.WriteNextColNicify(nameof(TrackCheckpoint.zero_0x4E));
                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    int nodeLength = scene.trackNodes.Length;
                    int nodeIndex = 0;
                    foreach (var trackNode in scene.trackNodes)
                    {
                        int pointLength = trackNode.checkpoints.Length;
                        int pointIndex = 0;
                        foreach (var trackPoint in trackNode.checkpoints)
                        {
                            writer.WriteNextCol($"COLI_COURSE{scene.ID:d2}");
                            writer.WriteNextCol($"[{nodeIndex}/{nodeLength}]");
                            writer.WriteNextCol($"[{pointIndex}/{pointLength}]");

                            writer.WriteNextCol(trackPoint.curveTimeStart);
                            writer.WriteNextCol(trackPoint.curveTimeEnd);
                            writer.WriteNextCol(trackPoint.trackDistanceStart);
                            writer.WriteNextCol(trackPoint.tangentStart);
                            writer.WriteNextCol(trackPoint.positionStart);
                            writer.WriteNextCol(trackPoint.trackDistanceEnd);
                            writer.WriteNextCol(trackPoint.tangentEnd);
                            writer.WriteNextCol(trackPoint.positionEnd);
                            writer.WriteNextCol(trackPoint.transformDistanceEnd);
                            writer.WriteNextCol(trackPoint.transformDistanceStart);
                            writer.WriteNextCol(trackPoint.trackWidth);
                            writer.WriteNextCol(trackPoint.isTrackContinuousStart);
                            writer.WriteNextCol(trackPoint.isTrackContinuousEnd);
                            writer.WriteNextCol(trackPoint.zero_0x4E);
                            writer.WriteNextRow();

                            pointIndex++;
                        }
                        nodeIndex++;
                    }
                }
                writer.Flush();
            }
        }

        public static void AnalyzeSceneStaticCollider(ColiScene[] scenes, string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Index");
                writer.WriteNextColNicify(nameof(StaticColliderMap.staticColliderTrisPtr));
                writer.WriteNextColNicify(nameof(StaticColliderMap.triMeshMatrixPtrs));
                writer.WriteNextColNicify(nameof(MatrixBoundsXZ.left));
                writer.WriteNextColNicify(nameof(MatrixBoundsXZ.top));
                writer.WriteNextColNicify(nameof(MatrixBoundsXZ.subdivionWidth));
                writer.WriteNextColNicify(nameof(MatrixBoundsXZ.subdivisionLength));
                writer.WriteNextColNicify(nameof(MatrixBoundsXZ.numSubdivisionsX));
                writer.WriteNextColNicify(nameof(MatrixBoundsXZ.numSubdivisionsZ));
                writer.WriteNextColNicify(nameof(StaticColliderMap.staticColliderQuadsPtr));
                writer.WriteNextColNicify(nameof(StaticColliderMap.quadMeshMatrixPtrs));
                writer.WriteNextColNicify(nameof(StaticColliderMap.unkDataPtr));
                writer.WriteNextColNicify(nameof(StaticColliderMap.staticSceneObjectsPtr));
                writer.WriteNextColNicify(nameof(StaticColliderMap.unknownCollidersPtr));
                writer.WriteNextColNicify(nameof(StaticColliderMap.unk_float));
                writer.WriteNextCol();
                writer.WriteNextColNicify(nameof(UnknownStaticColliderMapData.unk_0x00));
                writer.WriteNextColNicify(nameof(UnknownStaticColliderMapData.unk_0x04));
                writer.WriteNextColNicify(nameof(UnknownStaticColliderMapData.unk_0x08));
                writer.WriteNextColNicify(nameof(UnknownStaticColliderMapData.unk_0x0C));
                writer.WriteNextRow();


                int index = 0;

                foreach (var scene in scenes)
                {
                    var staticColliderMeshes = scene.staticColliderMap;

                    writer.WriteNextCol($"COLI_COURSE{scene.ID:d2}");
                    writer.WriteNextCol(index++);
                    writer.WriteNextCol(staticColliderMeshes.staticColliderTrisPtr.HexAddress);
                    writer.WriteNextCol(staticColliderMeshes.triMeshMatrixPtrs.Length);
                    writer.WriteNextCol(staticColliderMeshes.meshBounds.left);
                    writer.WriteNextCol(staticColliderMeshes.meshBounds.top);
                    writer.WriteNextCol(staticColliderMeshes.meshBounds.subdivionWidth);
                    writer.WriteNextCol(staticColliderMeshes.meshBounds.subdivisionLength);
                    writer.WriteNextCol(staticColliderMeshes.meshBounds.numSubdivisionsX);
                    writer.WriteNextCol(staticColliderMeshes.meshBounds.numSubdivisionsZ);
                    writer.WriteNextCol(staticColliderMeshes.staticColliderQuadsPtr.HexAddress);
                    writer.WriteNextCol(staticColliderMeshes.quadMeshMatrixPtrs.Length);
                    writer.WriteNextCol(staticColliderMeshes.unkDataPtr.HexAddress);
                    writer.WriteNextCol(staticColliderMeshes.staticSceneObjectsPtr.HexAddress);
                    writer.WriteNextCol(staticColliderMeshes.unknownCollidersPtr.HexAddress);
                    writer.WriteNextCol(staticColliderMeshes.unk_float);
                    writer.WriteNextCol();
                    writer.WriteNextCol(staticColliderMeshes.unkData.unk_0x00);
                    writer.WriteNextCol(staticColliderMeshes.unkData.unk_0x04);
                    writer.WriteNextCol(staticColliderMeshes.unkData.unk_0x08);
                    writer.WriteNextCol(staticColliderMeshes.unkData.unk_0x0C);
                    writer.WriteNextRow();
                }
                writer.Flush();
            }
        }

        public static void AnalyzeSceneObjectReferences(ColiScene[] scenes, string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Index");
                writer.WriteNextCol("Venue");
                writer.WriteNextCol("Course");
                writer.WriteNextCol("AX/GX");
                //
                writer.WriteNextCol("name");
                writer.WriteNextCol("Object Type");
                writer.WriteNextCol(nameof(SceneObject.zero_0x00));
                writer.WriteNextCol(nameof(SceneObject.namePtr));
                writer.WriteNextCol(nameof(SceneObject.zero_0x08));
                writer.WriteNextCol(nameof(SceneObject.unk_0x0C));
                //
                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    var venueID = CourseUtility.GetVenueID(scene.ID).GetDescription();
                    var courseID = ((CourseIndexAX)scene.ID).GetDescription();
                    var isAxGx = scene.IsFileGX ? "GX" : "AX";

                    // Get all the scene object references
                    var objectsList = new List<(SceneObject sor, string category)>();
                    foreach (var sceneObject in scene.templateSceneObjects)
                    {
                        var sceneObjectReference = sceneObject.sceneObject;
                        objectsList.Add((sceneObject.sceneObject, "Instance"));
                    }
                    foreach (var sceneOriginObject in scene.staticSceneObjects)
                    {
                        var sceneObjectReference = sceneOriginObject.templateSceneObject.sceneObject;
                        objectsList.Add((sceneObjectReference, "Origin"));
                    }

                    // iterate
                    foreach (var sceneObjectReference in objectsList)
                    {
                        writer.WriteNextCol(scene.FileName);
                        writer.WriteNextCol(scene.ID);
                        writer.WriteNextCol(venueID);
                        writer.WriteNextCol(courseID);
                        writer.WriteNextCol(isAxGx);
                        //
                        writer.WriteNextCol(sceneObjectReference.sor.name);
                        writer.WriteNextCol(sceneObjectReference.category);
                        writer.WriteNextCol(sceneObjectReference.sor.zero_0x00);
                        writer.WriteNextCol(sceneObjectReference.sor.namePtr);
                        writer.WriteNextCol(sceneObjectReference.sor.zero_0x08);
                        writer.WriteNextCol(sceneObjectReference.sor.unk_0x0C);
                        //
                        writer.WriteNextRow();
                    }
                }
                writer.Flush();
            }
        }

        public static void AnalyzeSceneInstanceReferences(ColiScene[] scenes, string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Index");
                writer.WriteNextCol("Venue");
                writer.WriteNextCol("Course");
                writer.WriteNextCol("AX/GX");
                //
                writer.WriteNextCol("name");
                writer.WriteNextCol("Object Type");
                writer.WriteNextCol(nameof(SceneObjectTemplate.unk_0x00));
                writer.WriteNextCol(nameof(SceneObjectTemplate.unk_0x04));
                writer.WriteNextCol(nameof(SceneObjectTemplate.sceneObjectPtr));
                writer.WriteNextCol(nameof(SceneObjectTemplate.colliderGeometryPtr));
                //
                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    var venueID = CourseUtility.GetVenueID(scene.ID).GetDescription();
                    var courseID = ((CourseIndexAX)scene.ID).GetDescription();
                    var isAxGx = scene.IsFileGX ? "GX" : "AX";

                    // Get all the scene object references
                    var objectsList = new List<(SceneObjectTemplate sir, string category)>();
                    foreach (var sceneInstance in scene.templateSceneObjects)
                    {
                        objectsList.Add((sceneInstance, "Instance"));
                    }
                    foreach (var sceneOriginObject in scene.staticSceneObjects)
                    {
                        var sceneInstance = sceneOriginObject.templateSceneObject;
                        objectsList.Add((sceneInstance, "Origin"));
                    }

                    // iterate
                    foreach (var sceneObjectReference in objectsList)
                    {
                        writer.WriteNextCol(scene.FileName);
                        writer.WriteNextCol(scene.ID);
                        writer.WriteNextCol(venueID);
                        writer.WriteNextCol(courseID);
                        writer.WriteNextCol(isAxGx);
                        //
                        writer.WriteNextCol(sceneObjectReference.sir.sceneObject.name);
                        writer.WriteNextCol(sceneObjectReference.category);
                        writer.WriteNextCol(sceneObjectReference.sir.unk_0x00);
                        writer.WriteNextCol(sceneObjectReference.sir.unk_0x04);
                        writer.WriteNextCol(sceneObjectReference.sir.sceneObjectPtr);
                        writer.WriteNextCol(sceneObjectReference.sir.colliderGeometryPtr);
                        //
                        writer.WriteNextRow();
                    }
                }
                writer.Flush();
            }
        }

        public static void AnalyzeSceneInstanceAndObjectReferences(ColiScene[] scenes, string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Index");
                writer.WriteNextCol("Venue");
                writer.WriteNextCol("Course");
                writer.WriteNextCol("AX/GX");
                //
                writer.WriteNextCol("name");
                writer.WriteNextCol(nameof(SceneObjectTemplate.unk_0x00));
                writer.WriteNextCol(nameof(SceneObjectTemplate.unk_0x04));
                writer.WriteNextCol(nameof(SceneObjectTemplate.sceneObjectPtr));
                writer.WriteNextCol(nameof(SceneObjectTemplate.colliderGeometryPtr));
                writer.WriteNextCol(nameof(SceneObject.zero_0x00));
                writer.WriteNextCol(nameof(SceneObject.namePtr));
                writer.WriteNextCol(nameof(SceneObject.zero_0x08));
                writer.WriteNextCol(nameof(SceneObject.unk_0x0C));
                //
                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    var venueID = CourseUtility.GetVenueID(scene.ID).GetDescription();
                    var courseID = ((CourseIndexAX)scene.ID).GetDescription();
                    var isAxGx = scene.IsFileGX ? "GX" : "AX";

                    foreach (var sceneInstance in scene.templateSceneObjects)
                    {
                        writer.WriteNextCol(scene.FileName);
                        writer.WriteNextCol(scene.ID);
                        writer.WriteNextCol(venueID);
                        writer.WriteNextCol(courseID);
                        writer.WriteNextCol(isAxGx);
                        //
                        var objectReference = sceneInstance.sceneObject;
                        writer.WriteNextCol(objectReference.name);
                        writer.WriteNextCol(sceneInstance.unk_0x00);
                        writer.WriteNextCol(sceneInstance.unk_0x04);
                        writer.WriteNextCol(sceneInstance.sceneObjectPtr);
                        writer.WriteNextCol(sceneInstance.colliderGeometryPtr);
                        writer.WriteNextCol(objectReference.zero_0x00);
                        writer.WriteNextCol(objectReference.namePtr);
                        writer.WriteNextCol(objectReference.zero_0x08);
                        writer.WriteNextCol(objectReference.unk_0x0C);
                        writer.WriteNextRow();
                    }
                }
                writer.Flush();
            }
        }


        public static void AnalyzeGeneralData(ColiScene[] scenes, string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Index");
                writer.WriteNextCol("Venue");
                writer.WriteNextCol("Course");
                writer.WriteNextCol("AX/GX");
                //
                writer.WriteNextCol(nameof(Range) + "." + nameof(Range.near));
                writer.WriteNextCol(nameof(Range) + "." + nameof(Range.far));
                writer.WriteNextCol(nameof(ColiScene.trackMinHeight));
                writer.WriteNextCol(nameof(ColiScene.trackLength));
                //
                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    var venueID = CourseUtility.GetVenueID(scene.ID).GetDescription();
                    var courseID = ((CourseIndexAX)scene.ID).GetDescription();
                    var isAxGx = scene.IsFileGX ? "GX" : "AX";

                    writer.WriteNextCol(scene.FileName);
                    writer.WriteNextCol(scene.ID);
                    writer.WriteNextCol(venueID);
                    writer.WriteNextCol(courseID);
                    writer.WriteNextCol(isAxGx);
                    //
                    writer.WriteNextCol(scene.unkRange0x00.near);
                    writer.WriteNextCol(scene.unkRange0x00.far);
                    writer.WriteNextCol(scene.trackMinHeight.value);
                    writer.WriteNextCol(scene.trackLength.value);
                    writer.WriteNextRow();
                }
                writer.Flush();
            }
        }

        public static void AnalyzeSurfaceAttributeAreas(ColiScene[] scenes, string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Index");
                writer.WriteNextCol("Venue");
                writer.WriteNextCol("Course");
                writer.WriteNextCol("AX/GX");
                //
                writer.WriteNextCol(nameof(SurfaceAttributeArea.lengthFrom));
                writer.WriteNextCol(nameof(SurfaceAttributeArea.lengthTo));
                writer.WriteNextCol(nameof(SurfaceAttributeArea.widthLeft));
                writer.WriteNextCol(nameof(SurfaceAttributeArea.widthRight));
                writer.WriteNextCol(nameof(SurfaceAttributeArea.surfaceAttribute));
                writer.WriteNextCol(nameof(SurfaceAttributeArea.trackBranchID));
                writer.WriteNextCol(nameof(SurfaceAttributeArea.zero_0x12));
                //
                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    var venueID = CourseUtility.GetVenueID(scene.ID).GetDescription();
                    var courseID = ((CourseIndexAX)scene.ID).GetDescription();
                    var isAxGx = scene.IsFileGX ? "GX" : "AX";

                    foreach (var surfaceAttributeArea in scene.surfaceAttributeAreas)
                    {
                        writer.WriteNextCol(scene.FileName);
                        writer.WriteNextCol(scene.ID);
                        writer.WriteNextCol(venueID);
                        writer.WriteNextCol(courseID);
                        writer.WriteNextCol(isAxGx);
                        //
                        writer.WriteNextCol(surfaceAttributeArea.lengthFrom);
                        writer.WriteNextCol(surfaceAttributeArea.lengthTo);
                        writer.WriteNextCol(surfaceAttributeArea.widthLeft);
                        writer.WriteNextCol(surfaceAttributeArea.widthRight);
                        writer.WriteNextCol(surfaceAttributeArea.surfaceAttribute);
                        writer.WriteNextCol(surfaceAttributeArea.trackBranchID);
                        writer.WriteNextCol(surfaceAttributeArea.zero_0x12);
                        //
                        writer.WriteNextRow();
                    }
                }
                writer.Flush();
            }
        }

    }
}
