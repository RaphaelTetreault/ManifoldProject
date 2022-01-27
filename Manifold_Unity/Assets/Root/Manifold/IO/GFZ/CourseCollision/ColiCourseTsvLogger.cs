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
        private const string menu = Const.Menu.Manifold + "Analysis/";

        // Names of files generated
        private static readonly string tsvHeader = $"{nameof(ColiScene)}-Header.tsv";
        private static readonly string tsvGeneralData = $"General Data.tsv";
        private static readonly string tsvTrackKeyablesAll = $"Track Keyables All.tsv";
        private static readonly string tsvTrackSegment = $"{nameof(TrackSegment)}.tsv";
        private static readonly string tsvSurfaceAttributeArea = $"{nameof(SurfaceAttributeArea)}.tsv";
        private static readonly string tsvTrackNode = $"{nameof(TrackNode)}.tsv";
        private static readonly string tsvSceneObject = $"{nameof(SceneObject)}.tsv";
        private static readonly string tsvSceneObjectLod = $"{nameof(SceneObjectLOD)}.tsv";
        private static readonly string tsvSceneObjectsAndLod = $"{nameof(SceneObjectLOD)}.tsv";
        private static readonly string tsvSceneObjectDynamic = $"{nameof(SceneObjectDynamic)}.tsv";
        private static readonly string tsvAnimationClip = $"{nameof(SceneObjectDynamic)}-{nameof(AnimationClip)}.tsv";
        private static readonly string tsvTextureMetadata = $"{nameof(SceneObjectDynamic)}-{nameof(TextureScroll)}.tsv";
        private static readonly string tsvSkeletalAnimator = $"{nameof(SceneObjectDynamic)}-{nameof(SkeletalAnimator)}.tsv";
        private static readonly string tsvColliderGeometryTri = $"{nameof(SceneObjectDynamic)}-{nameof(ColliderMesh)}-Tris.tsv";
        private static readonly string tsvColliderGeometryQuad = $"{nameof(SceneObjectDynamic)}-{nameof(ColliderMesh)}-Quads.tsv";
        private static readonly string tsvTransform = $"{nameof(TransformPRXS)}.tsv";
        private static readonly string tsvArcadeCheckpointTrigger = $"{nameof(TimeExtensionTrigger)}.tsv";
        private static readonly string tsvCourseMetadataTrigger = $"{nameof(CourseMetadataTrigger)}.tsv";
        private static readonly string tsvStoryObjectTrigger = $"{nameof(StoryObjectTrigger)}.tsv";
        private static readonly string tsvUnknownTrigger = $"{nameof(UnknownTrigger)}.tsv";
        private static readonly string tsvVisualEffectTrigger = $"{nameof(VisualEffectTrigger)}.tsv";
        private static readonly string tsvFog = $"{nameof(Fog)}.tsv";
        private static readonly string tsvFogCurves = $"{nameof(FogCurves)}.tsv";
        private static readonly string tsvStaticColliderMeshes = $"{nameof(StaticColliderMeshes)}.tsv";
        private static readonly string tsvUnknownCollider = $"{nameof(UnknownCollider)}.tsv";


        #region Menu Items

        [MenuItem(menu + "Analyze all ColiScene Data #F5")]
        public static void AnalyzeAllPartsOfColiCourse()
        {
            var settings = GfzProjectWindow.GetSettings();
            var sceneIterator = ColiCourseIO.LoadAllStages(settings.StageDir, "Loading Stages...");
            var scenes = sceneIterator.ToArray();
            var output = settings.AnalysisOutput;
            Directory.CreateDirectory(output);
            Analyze(scenes, output);
            OSUtility.OpenDirectory(settings.AnalysisOutput);
        }


        [MenuItem(menu + "Coli Scene (Header)")]
        public static void MenuAnalyzeHeaders() => MenuForward(AnalyzeHeaders, tsvHeader);

        [MenuItem(menu + "General Data")]
        public static void MenuAnalyzeGeneralData() => MenuForward(AnalyzeGeneralData, tsvGeneralData);


        [MenuItem(menu + "Track Keyables All")]
        public static void MenuAnalyzeTrackKeyablesAll() => MenuForward(AnalyzeTrackKeyablesAll, tsvTrackKeyablesAll);

        [MenuItem(menu + "Track Transforms")]
        public static void MenuAnalyzeTrackSegments() => MenuForward(AnalyzeTrackSegments, tsvTrackSegment);

        [MenuItem(menu + "Surface Attribute Areas")]
        public static void MenuAnalyzeSurfaceAttributeAreas() => MenuForward(AnalyzeSurfaceAttributeAreas, tsvSurfaceAttributeArea);

        [MenuItem(menu + "Track Nodes")]
        public static void MenuAnalyzeTrackNodes() => MenuForward(AnalyzeTrackNodes, tsvTrackNode);


        [MenuItem(menu + "Scene Objects")]
        public static void MenuAnalyzeSceneObjects() => MenuForward(AnalyzeSceneObjects, tsvSceneObject);

        [MenuItem(menu + "Scene Object LODs")]
        public static void MenuAnalyzeSceneObjectLODs() => MenuForward(AnalyzeSceneObjectLODs, tsvSceneObjectLod);

        [MenuItem(menu + "Scene Object + Scene Object LODs")]
        public static void MenuAnalyzeSceneObjectsAndLODs() => MenuForward(AnalyzeSceneObjectsAndLODs, tsvSceneObjectsAndLod);


        [MenuItem(menu + "Scene Objects Dynamic")]
        public static void MenuAnalyzeSceneObjectDynamic() => MenuForward(AnalyzeSceneObjectDynamic, tsvSceneObjectDynamic);

        [MenuItem(menu + "Scene Objects Dynamic - Animation Clip")]
        public static void MenuAnalyzeAnimationClips() => MenuForward(AnalyzeAnimationClips, tsvAnimationClip);

        [MenuItem(menu + "Scene Objects Dynamic - Texture Metadata")]
        public static void MenuAnalyzeTextureMetadata() => MenuForward(AnalyzeTextureMetadata, tsvTextureMetadata);

        [MenuItem(menu + "Scene Objects Dynamic - Skeletal Animator")]
        public static void MenuAnalyzeSkeletalAnimator() => MenuForward(AnalyzeSkeletalAnimator, tsvSkeletalAnimator);

        [MenuItem(menu + "Scene Objects Dynamic - Collider Geometry (Tris)")]
        public static void MenuAnalyzeColliderGeometryTri() => MenuForward(AnalyzeColliderGeometryTri, tsvColliderGeometryTri);

        [MenuItem(menu + "Scene Objects Dynamic - Collider Geometry (Quads)")]
        public static void MenuAnalyzeColliderGeometryQuad() => MenuForward(AnalyzeColliderGeometryQuad, tsvColliderGeometryQuad);

        [MenuItem(menu + "Scene Object Dynamic - Transform")]
        public static void MenuAnalyzeSceneObjectTransforms() => MenuForward(AnalyzeSceneObjectTransforms, tsvTransform);


        [MenuItem(menu + "Trigger - Arcade Checkpoints")]
        public static void MenuAnalyzeArcadeCheckpointTriggers() => MenuForward(AnalyzeArcadeCheckpointTriggers, tsvArcadeCheckpointTrigger);

        [MenuItem(menu + "Trigger - Course Metadata")]
        public static void MenuAnalyzeCourseMetadataTriggers() => MenuForward(AnalyzeCourseMetadataTriggers, tsvCourseMetadataTrigger);

        [MenuItem(menu + "Trigger - Story Object")]
        public static void MenuAnalyzeStoryObjectTrigger() => MenuForward(AnalyzeStoryObjectTrigger, tsvStoryObjectTrigger);

        [MenuItem(menu + "Trigger - Unknown Trigger")]
        public static void MenuAnalyzeUnknownTrigger() => MenuForward(AnalyzeUnknownTrigger, tsvUnknownTrigger);

        [MenuItem(menu + "Trigger - Visual Effect Trigger")]
        public static void MenuAnalyzeVisualEffectTriggers() => MenuForward(AnalyzeVisualEffectTriggers, tsvVisualEffectTrigger);


        [MenuItem(menu + "Fog")]
        public static void MenuAnalyzeFog() => MenuForward(AnalyzeFog, tsvFog);

        [MenuItem(menu + "Fog Curves")]
        public static void MenuAnalyzeFogCurves() => MenuForward(AnalyzeFogCurves, tsvFogCurves);



        [MenuItem(menu + "Static Collider Meshses")]
        public static void MenuAnalyzeSceneStaticCollider() => MenuForward(AnalyzeStaticColliderMeshes, tsvStaticColliderMeshes);

        [MenuItem(menu + "Unknown Collider")]
        public static void MenuAnalyzeUnknownCollider() => MenuForward(AnalyzeUnknownCollider, tsvUnknownCollider);

        #endregion


        /// <summary>
        /// Forwards most analysis functions 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="fileName"></param>
        public static void MenuForward(Action<ColiScene[], string> action, string fileName)
        {
            var settings = GfzProjectWindow.GetSettings();
            var sceneIterator = ColiCourseIO.LoadAllStages(settings.StageDir, "Loading Stages...");
            var scenes = sceneIterator.ToArray();
            var outputPath = settings.AnalysisOutput + fileName;
            action.Invoke(scenes, outputPath);
            OSUtility.OpenDirectory(settings.AnalysisOutput);
        }

        public static void Analyze(ColiScene[] scenes, string outputPath)
        {
            var progress = 1f;
            var title = "Analysing All ColiScene Files...";
            var time = AnalyzerUtility.GetFileTimestamp();
            bool cancel;

            var destination = Path.Combine(outputPath, time);
            Directory.CreateDirectory(destination);

            // GENERAL DATA
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvGeneralData, progress);
            AnalyzeGeneralData(scenes, Path.Combine(destination, tsvGeneralData));
            if (cancel) goto END;

            // FOG
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvFog, progress);
            AnalyzeFog(scenes, Path.Combine(destination, tsvFog));
            if (cancel) goto END;

            // FOG CURVES
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvFogCurves, progress);
            AnalyzeFogCurves(scenes, Path.Combine(destination, tsvFogCurves));
            if (cancel) goto END;

            // COLI SCENE HEADER
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvHeader, progress);
            AnalyzeHeaders(scenes, Path.Combine(destination, tsvHeader));
            if (cancel) goto END;

            // ANIMATION CLIPS
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvAnimationClip, progress);
            AnalyzeAnimationClips(scenes, Path.Combine(destination, tsvAnimationClip));
            if (cancel) goto END;

            // ANIMATIONS INDIVIDUALIZED
            {
                var count = AnimationClip.kAnimationCurvesCount;
                for (int i = 0; i < count; i++)
                {
                    var fileName = $"{nameof(SceneObjectDynamic)}-{nameof(AnimationClip)}[{i}].tsv";
                    var filePath = Path.Combine(destination, fileName);
                    cancel = EditorUtility.DisplayCancelableProgressBar(title, fileName, (float)(i + 1) / count);
                    AnalyzeGameObjectAnimationClipIndex(scenes, filePath, i);
                    if (cancel) goto END;
                }
            }

            // TRANSFORMS
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvTransform, progress);
            AnalyzeSceneObjectTransforms(scenes, Path.Combine(destination, tsvTransform));
            if (cancel) goto END;

            // ARCADE CHECKPOINT TRIGGERS (AX)
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvArcadeCheckpointTrigger, progress);
            AnalyzeArcadeCheckpointTriggers(scenes, Path.Combine(destination, tsvArcadeCheckpointTrigger));
            if (cancel) goto END;

            // COURSE METADATA TRIGGERS
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvCourseMetadataTrigger, progress);
            AnalyzeCourseMetadataTriggers(scenes, Path.Combine(destination, tsvCourseMetadataTrigger));
            if (cancel) goto END;

            // STORY OBJECT TRIGGERS
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvStoryObjectTrigger, progress);
            AnalyzeStoryObjectTrigger(scenes, Path.Combine(destination, tsvStoryObjectTrigger));
            if (cancel) goto END;

            // UNKNOWN TRIGGERS
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvUnknownTrigger, progress);
            AnalyzeUnknownTrigger(scenes, Path.Combine(destination, tsvUnknownTrigger));
            if (cancel) goto END;

            // VISUAL EFFECT TRIGGERS
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvVisualEffectTrigger, progress);
            AnalyzeVisualEffectTriggers(scenes, Path.Combine(destination, tsvVisualEffectTrigger));
            if (cancel) goto END;


            // UNKNOWN COLLIDERS
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvUnknownCollider, progress);
            AnalyzeUnknownCollider(scenes, Path.Combine(destination, tsvUnknownCollider));
            if (cancel) goto END;


            // SCENE OBJECT DYNAMIC
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvSceneObjectDynamic, progress);
            AnalyzeSceneObjectDynamic(scenes, Path.Combine(destination, tsvSceneObjectDynamic));
            if (cancel) goto END;

            // TEXTURE METADATA
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvTextureMetadata, progress);
            AnalyzeTextureMetadata(scenes, Path.Combine(destination, tsvTextureMetadata));
            if (cancel) goto END;

            // SKELETAL ANIMATOR
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvSkeletalAnimator, progress);
            AnalyzeSkeletalAnimator(scenes, Path.Combine(destination, tsvSkeletalAnimator));
            if (cancel) goto END;

            // COLLIDER GEOMETRY (TRI)
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvColliderGeometryTri, progress);
            AnalyzeColliderGeometryTri(scenes, Path.Combine(destination, tsvColliderGeometryTri));
            if (cancel) goto END;

            // COLLIDER GEOMETRY (QUAD)
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvColliderGeometryQuad, progress);
            AnalyzeColliderGeometryQuad(scenes, Path.Combine(destination, tsvColliderGeometryQuad));
            if (cancel) goto END;

            // SCENE OBJECT LOD
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvSceneObjectLod, progress);
            AnalyzeSceneObjectLODs(scenes, Path.Combine(destination, tsvSceneObjectLod));
            if (cancel) goto END;

            // SCENE OBJECT
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvSceneObject, progress);
            AnalyzeSceneObjects(scenes, Path.Combine(destination, tsvSceneObject));
            if (cancel) goto END;

            // SCENE OBJECTS + SCENE OBJECT LODS
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvSceneObjectsAndLod, progress);
            AnalyzeSceneObjectsAndLODs(scenes, Path.Combine(destination, tsvSceneObjectsAndLod));
            if (cancel) goto END;

            // TRACK SEGMENT KEYABLES (INDEXED)
            {
                var count = TrackCurves.kCurveCount;
                for (int i = 0; i < count; i++)
                {
                    var fileName = $"{nameof(TrackCurves)}[{i+1}].tsv";
                    var filePath = Path.Combine(destination, fileName);
                    EditorUtility.DisplayProgressBar(title, filePath, (float)(i + 1) / TrackCurves.kCurveCount);
                    AnalyzeTrackKeyables(scenes, filePath, i);
                }
            }

            // TRACK SEGMENT KEYABLES (ALL)
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvTrackKeyablesAll, progress);
            AnalyzeTrackKeyablesAll(scenes, Path.Combine(destination, tsvTrackKeyablesAll));
            if (cancel) goto END;

            // TRACK SEGMENTS
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvTrackSegment, progress);
            AnalyzeTrackSegments(scenes, Path.Combine(destination, tsvTrackSegment));
            if (cancel) goto END;

            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvTrackNode, progress);
            AnalyzeTrackNodes(scenes, Path.Combine(destination, tsvTrackNode));
            if (cancel) goto END;

            // STATIC COLLIDER MESHES
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvStaticColliderMeshes, progress);
            AnalyzeStaticColliderMeshes(scenes, Path.Combine(destination, tsvStaticColliderMeshes));
            if (cancel) goto END;

            // SURFACE ATTRIBUTE AREAS
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvSurfaceAttributeArea, progress);
            AnalyzeSurfaceAttributeAreas(scenes, Path.Combine(destination, tsvSurfaceAttributeArea));
            if (cancel) goto END;


            END:
            OSUtility.OpenDirectory(Path.Combine(outputPath, time));
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

                writer.WriteNextCol(nameof(TrackSegment.segmentType));
                writer.WriteNextCol(nameof(TrackSegment.embeddedPropertyType));
                writer.WriteNextCol(nameof(TrackSegment.perimeterFlags));
                writer.WriteNextCol(nameof(TrackSegment.pipeCylinderFlags));
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

                writer.WriteNextCol(nameof(TrackSegment.segmentType));
                writer.WriteNextCol(nameof(TrackSegment.embeddedPropertyType));
                writer.WriteNextCol(nameof(TrackSegment.perimeterFlags));
                writer.WriteNextCol(nameof(TrackSegment.pipeCylinderFlags));
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

            writer.WriteNextCol(tt.segmentType);
            writer.WriteNextCol(tt.embeddedPropertyType);
            writer.WriteNextCol(tt.perimeterFlags);
            writer.WriteNextCol(tt.pipeCylinderFlags);
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
        public static void AnalyzeTrackSegments(ColiScene[] scenes, string filename)
        {
            using (var writer = AnalyzerUtility.OpenWriter(filename))
            {
                writer.WriteNextCol("Filename");
                writer.WriteNextCol("Order");
                writer.WriteNextCol("Root Index");
                writer.WriteNextCol("Transform Depth");
                writer.WriteNextCol("Address");
                writer.WriteNextCol(nameof(TrackSegment.segmentType));
                writer.WriteNextCol(nameof(TrackSegment.embeddedPropertyType));
                writer.WriteNextCol(nameof(TrackSegment.perimeterFlags));
                writer.WriteNextCol(nameof(TrackSegment.pipeCylinderFlags));
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
                        WriteTrackSegmentRecursive(writer, scene, 0, ++index, total, trackTransform);
                    }
                }

                writer.Flush();
            }
        }
        // Writes self and children
        public static void WriteTrackSegmentRecursive(StreamWriter writer, ColiScene scene, int depth, int index, int total, TrackSegment trackTransform)
        {
            // Write Parent
            WriteTrackSegment(writer, scene, depth, index, total, trackTransform);

            // Write children
            //foreach (var child in trackTransform.children)
            //{
            //    WriteTrackTransformRecursive(writer, sobj, depth + 1, index, total, child);
            //}
        }
        // The actual writing to file
        public static void WriteTrackSegment(StreamWriter writer, ColiScene scene, int depth, int index, int total, TrackSegment trackTransform)
        {
            writer.WriteNextCol(scene.FileName);
            writer.WriteNextCol($"{s_order++}");
            writer.WriteNextCol($"[{index}/{total}]");
            writer.WriteNextCol($"{depth}");
            writer.WriteNextCol(trackTransform.StartAddressHex());
            writer.WriteNextCol(trackTransform.segmentType);
            writer.WriteNextCol(trackTransform.embeddedPropertyType);
            writer.WriteNextCol(trackTransform.perimeterFlags);
            writer.WriteNextCol(trackTransform.pipeCylinderFlags);
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

        #region Scene Objects' Animation Clips

        public static void AnalyzeAnimationClips(ColiScene[] scenes, string filename)
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
                                writer.WriteNextCol(gameObject.Name);
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
                                writer.WriteNextCol(dynamicSceneObject.Name);
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

        #region Scene Objects

        public static void AnalyzeSceneObjectDynamic(ColiScene[] scenes, string fileName)
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
                writer.WriteNextCol(nameof(SceneObjectDynamic.sceneObjectPtr));
                writer.WriteNextCol(nameof(SceneObjectDynamic.transformPRXS.Position));
                writer.WriteNextCol(nameof(SceneObjectDynamic.transformPRXS.RotationEuler));
                writer.WriteNextCol(nameof(SceneObjectDynamic.transformPRXS.Scale));
                writer.WriteNextCol(nameof(SceneObjectDynamic.zero_0x2C));
                writer.WriteNextCol(nameof(SceneObjectDynamic.animationClipPtr));
                writer.WriteNextCol(nameof(SceneObjectDynamic.textureScrollPtr));
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
                        writer.WriteNextCol(sceneObject.Name);
                        writer.WriteNextCol(sceneObject.unk0x00);
                        writer.WriteNextCol($"0x{sceneObject.unk0x00:x8}");
                        writer.WriteNextCol(sceneObject.unk0x04);
                        writer.WriteNextCol($"0x{sceneObject.unk0x04:x8}");
                        writer.WriteNextCol(sceneObject.sceneObjectPtr.HexAddress);
                        writer.WriteNextCol(sceneObject.transformPRXS.Position);
                        writer.WriteNextCol(sceneObject.transformPRXS.RotationEuler);
                        writer.WriteNextCol(sceneObject.transformPRXS.Scale);
                        writer.WriteNextCol(sceneObject.zero_0x2C);
                        writer.WriteNextCol(sceneObject.animationClipPtr.HexAddress);
                        writer.WriteNextCol(sceneObject.textureScrollPtr.HexAddress);
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
                writer.WriteNextColNicify(nameof(TextureScrollField.x));
                writer.WriteNextColNicify(nameof(TextureScrollField.y));
                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    int gameObjectIndex = 0;
                    foreach (var sceneObject in scene.dynamicSceneObjects)
                    {
                        if (sceneObject.textureScroll == null)
                            continue;

                        int fieldArrayIndex = 0;
                        foreach (var field in sceneObject.textureScroll.fields)
                        {
                            if (field == null)
                                return;

                            writer.WriteNextCol(scene.FileName);
                            writer.WriteNextCol(gameObjectIndex);
                            writer.WriteNextCol(sceneObject.Name);
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

        public static void AnalyzeSkeletalAnimator(ColiScene[] scenes, string fileName)
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
                        writer.WriteNextCol(dynamicSceneObject.Name);

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

        public static void AnalyzeColliderGeometryTri(ColiScene[] scenes, string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Game Object #");
                writer.WriteNextCol("Game Object");

                writer.WriteNextCol("Tri Index");
                writer.WriteNextCol("Addr");

                writer.WriteNextColNicify(nameof(ColliderTriangle.dotProduct));
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
                        if (dynamicSceneObject.sceneObject.colliderMesh == null)
                            continue;
                        if (dynamicSceneObject.sceneObject.colliderMesh.triCount == 0)
                            continue;

                        int triIndex = 0;
                        foreach (var tri in dynamicSceneObject.sceneObject.colliderMesh.tris)
                        {
                            writer.WriteNextCol(scene.FileName);
                            writer.WriteNextCol(gameObjectIndex);
                            writer.WriteNextCol(dynamicSceneObject.Name);

                            writer.WriteNextCol(triIndex++);
                            writer.WriteStartAddress(tri);

                            writer.WriteNextCol(tri.dotProduct);
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

        public static void AnalyzeColliderGeometryQuad(ColiScene[] scenes, string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Game Object #");
                writer.WriteNextCol("Game Object");

                writer.WriteNextCol("Quad Index");
                writer.WriteNextCol("Addr");

                writer.WriteNextColNicify(nameof(ColliderQuad.dotProduct));
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
                        if (dynamicSceneObject.sceneObject.colliderMesh == null)
                            continue;
                        if (dynamicSceneObject.sceneObject.colliderMesh.quadCount == 0)
                            continue;

                        int quadIndex = 0;
                        foreach (var quad in dynamicSceneObject.sceneObject.colliderMesh.quads)
                        {
                            writer.WriteNextCol(scene.FileName);
                            writer.WriteNextCol(gameObjectIndex);
                            writer.WriteNextCol(dynamicSceneObject.Name);

                            writer.WriteNextCol(quadIndex++);
                            writer.WriteStartAddress(quad);

                            writer.WriteNextCol(quad.dotProduct);
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
                writer.WriteNextCol(nameof(ColiScene.sceneObjectsPtr));
                writer.WriteNextCol(nameof(ColiScene.sceneObjectsPtr));
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
                writer.WriteNextCol(nameof(ColiScene.trackCheckpointBoundsXZ) + "." + nameof(ColiScene.trackCheckpointBoundsXZ.left));
                writer.WriteNextCol(nameof(ColiScene.trackCheckpointBoundsXZ) + "." + nameof(ColiScene.trackCheckpointBoundsXZ.top));
                writer.WriteNextCol(nameof(ColiScene.trackCheckpointBoundsXZ) + "." + nameof(ColiScene.trackCheckpointBoundsXZ.subdivisionWidth));
                writer.WriteNextCol(nameof(ColiScene.trackCheckpointBoundsXZ) + "." + nameof(ColiScene.trackCheckpointBoundsXZ.subdivisionLength));
                writer.WriteNextCol(nameof(ColiScene.trackCheckpointBoundsXZ) + "." + nameof(ColiScene.trackCheckpointBoundsXZ.numSubdivisionsX));
                writer.WriteNextCol(nameof(ColiScene.trackCheckpointBoundsXZ) + "." + nameof(ColiScene.trackCheckpointBoundsXZ.numSubdivisionsZ));
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
                    writer.WriteNextCol(scene.sceneObjectsPtr.Length);
                    writer.WriteNextCol(scene.sceneObjectsPtr.HexAddress);
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
                    writer.WriteNextCol(scene.trackCheckpointBoundsXZ.left);
                    writer.WriteNextCol(scene.trackCheckpointBoundsXZ.top);
                    writer.WriteNextCol(scene.trackCheckpointBoundsXZ.subdivisionWidth);
                    writer.WriteNextCol(scene.trackCheckpointBoundsXZ.subdivisionLength);
                    writer.WriteNextCol(scene.trackCheckpointBoundsXZ.numSubdivisionsX);
                    writer.WriteNextCol(scene.trackCheckpointBoundsXZ.numSubdivisionsZ);
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
                writer.WriteNextCol(nameof(TimeExtensionTrigger.transform.Position));
                writer.WriteNextCol(nameof(TimeExtensionTrigger.transform.RotationEuler));
                writer.WriteNextCol(nameof(TimeExtensionTrigger.transform.Scale));
                writer.WriteNextCol(nameof(TimeExtensionTrigger.transform.UnknownOption));
                writer.WriteNextCol(nameof(TimeExtensionTrigger.option));
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
                        writer.WriteNextCol(arcadeCheckpooint.option);
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
                writer.WriteNextCol(nameof(Fog.colorRGBA) + ".R");
                writer.WriteNextCol(nameof(Fog.colorRGBA) + ".G");
                writer.WriteNextCol(nameof(Fog.colorRGBA) + ".B");
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
                    writer.WriteNextCol(scene.fog.colorRGBA.x);
                    writer.WriteNextCol(scene.fog.colorRGBA.y);
                    writer.WriteNextCol(scene.fog.colorRGBA.z);
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
                        writer.WriteNextCol(sceneObject.Name);

                        // Rotation values from clean, uncompressed matrix
                        var matrix = sceneObject.transformMatrix3x4.rotationEuler;
                        writer.WriteNextCol(matrix.x);
                        writer.WriteNextCol(matrix.y);
                        writer.WriteNextCol(matrix.z);

                        // Rotation values as reconstructed
                        var euler = sceneObject.transformPRXS.DecomposedRotation.EulerAngles;
                        writer.WriteNextCol(euler.x);
                        writer.WriteNextCol(euler.y);
                        writer.WriteNextCol(euler.z);

                        // Decomposed rotation values, raw, requires processing to be used
                        var decomposed = sceneObject.transformPRXS.DecomposedRotation;
                        writer.WriteNextCol(decomposed.phi);
                        writer.WriteNextCol(decomposed.theta);
                        writer.WriteNextCol(decomposed.psi);
                        // The other parameters that go with the structure
                        writer.WriteNextCol(sceneObject.transformPRXS.UnknownOption);
                        writer.WriteNextCol(sceneObject.transformPRXS.ObjectActiveOverride);

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
                writer.WriteNextColNicify(nameof(Checkpoint.curveTimeStart));
                writer.WriteNextColNicify(nameof(Checkpoint.curveTimeEnd));
                writer.WriteNextColNicify(nameof(Checkpoint.planeStart.dotProduct));
                writer.WriteNextColNicify(nameof(Checkpoint.planeStart.normal) + ".x");
                writer.WriteNextColNicify(nameof(Checkpoint.planeStart.normal) + ".y");
                writer.WriteNextColNicify(nameof(Checkpoint.planeStart.normal) + ".z");
                writer.WriteNextColNicify(nameof(Checkpoint.planeStart.origin) + ".x");
                writer.WriteNextColNicify(nameof(Checkpoint.planeStart.origin) + ".y");
                writer.WriteNextColNicify(nameof(Checkpoint.planeStart.origin) + ".z");
                writer.WriteNextColNicify(nameof(Checkpoint.planeEnd.dotProduct));
                writer.WriteNextColNicify(nameof(Checkpoint.planeEnd.normal) + ".x");
                writer.WriteNextColNicify(nameof(Checkpoint.planeEnd.normal) + ".y");
                writer.WriteNextColNicify(nameof(Checkpoint.planeEnd.normal) + ".z");
                writer.WriteNextColNicify(nameof(Checkpoint.planeEnd.origin) + ".x");
                writer.WriteNextColNicify(nameof(Checkpoint.planeEnd.origin) + ".y");
                writer.WriteNextColNicify(nameof(Checkpoint.planeEnd.origin) + ".z");
                writer.WriteNextColNicify(nameof(Checkpoint.endDistance));
                writer.WriteNextColNicify(nameof(Checkpoint.startDistance));
                writer.WriteNextColNicify(nameof(Checkpoint.trackWidth));
                writer.WriteNextColNicify(nameof(Checkpoint.connectToTrackIn));
                writer.WriteNextColNicify(nameof(Checkpoint.connectToTrackOut));
                writer.WriteNextColNicify(nameof(Checkpoint.zero_0x4E));
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
                            writer.WriteNextCol(trackPoint.planeStart.dotProduct);
                            writer.WriteNextCol(trackPoint.planeStart.normal.x);
                            writer.WriteNextCol(trackPoint.planeStart.normal.y);
                            writer.WriteNextCol(trackPoint.planeStart.normal.z);
                            writer.WriteNextCol(trackPoint.planeStart.origin.x);
                            writer.WriteNextCol(trackPoint.planeStart.origin.y);
                            writer.WriteNextCol(trackPoint.planeStart.origin.z);
                            writer.WriteNextCol(trackPoint.planeEnd.dotProduct);
                            writer.WriteNextCol(trackPoint.planeEnd.normal.x);
                            writer.WriteNextCol(trackPoint.planeEnd.normal.y);
                            writer.WriteNextCol(trackPoint.planeEnd.normal.z);
                            writer.WriteNextCol(trackPoint.planeEnd.origin.x);
                            writer.WriteNextCol(trackPoint.planeEnd.origin.y);
                            writer.WriteNextCol(trackPoint.planeEnd.origin.z);
                            writer.WriteNextCol(trackPoint.endDistance);
                            writer.WriteNextCol(trackPoint.startDistance);
                            writer.WriteNextCol(trackPoint.trackWidth);
                            writer.WriteNextCol(trackPoint.connectToTrackIn);
                            writer.WriteNextCol(trackPoint.connectToTrackOut);
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

        public static void AnalyzeStaticColliderMeshes(ColiScene[] scenes, string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Index");
                writer.WriteNextColNicify(nameof(StaticColliderMeshes.staticColliderTrisPtr));
                writer.WriteNextColNicify(nameof(StaticColliderMeshes.triMeshMatrixPtrs));
                writer.WriteNextColNicify(nameof(MatrixBoundsXZ.left));
                writer.WriteNextColNicify(nameof(MatrixBoundsXZ.top));
                writer.WriteNextColNicify(nameof(MatrixBoundsXZ.subdivisionWidth));
                writer.WriteNextColNicify(nameof(MatrixBoundsXZ.subdivisionLength));
                writer.WriteNextColNicify(nameof(MatrixBoundsXZ.numSubdivisionsX));
                writer.WriteNextColNicify(nameof(MatrixBoundsXZ.numSubdivisionsZ));
                writer.WriteNextColNicify(nameof(StaticColliderMeshes.staticColliderQuadsPtr));
                writer.WriteNextColNicify(nameof(StaticColliderMeshes.quadMeshMatrixPtrs));
                writer.WriteNextColNicify(nameof(StaticColliderMeshes.unkDataPtr));
                writer.WriteNextColNicify(nameof(StaticColliderMeshes.staticSceneObjectsPtr));
                writer.WriteNextColNicify(nameof(StaticColliderMeshes.unknownCollidersPtr));
                writer.WriteNextColNicify(nameof(StaticColliderMeshes.unk_float));
                writer.WriteNextCol();
                writer.WriteNextColNicify(nameof(UnknownStaticColliderMapData.unk_0x00));
                writer.WriteNextColNicify(nameof(UnknownStaticColliderMapData.unk_0x04));
                writer.WriteNextColNicify(nameof(UnknownStaticColliderMapData.unk_0x08));
                writer.WriteNextColNicify(nameof(UnknownStaticColliderMapData.unk_0x0C));
                writer.WriteNextRow();


                int index = 0;

                foreach (var scene in scenes)
                {
                    var staticColliderMeshes = scene.staticColliderMeshes;

                    writer.WriteNextCol($"COLI_COURSE{scene.ID:d2}");
                    writer.WriteNextCol(index++);
                    writer.WriteNextCol(staticColliderMeshes.staticColliderTrisPtr.HexAddress);
                    writer.WriteNextCol(staticColliderMeshes.triMeshMatrixPtrs.Length);
                    writer.WriteNextCol(staticColliderMeshes.meshBounds.left);
                    writer.WriteNextCol(staticColliderMeshes.meshBounds.top);
                    writer.WriteNextCol(staticColliderMeshes.meshBounds.subdivisionWidth);
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

        public static void AnalyzeSceneObjectLODs(ColiScene[] scenes, string fileName)
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
                writer.WriteNextCol(nameof(SceneObjectLOD.zero_0x00));
                writer.WriteNextCol(nameof(SceneObjectLOD.lodNamePtr));
                writer.WriteNextCol(nameof(SceneObjectLOD.zero_0x08));
                writer.WriteNextCol(nameof(SceneObjectLOD.lodDistance));
                //
                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    var venueID = CourseUtility.GetVenueID(scene.ID).GetDescription();
                    var courseID = ((CourseIndexAX)scene.ID).GetDescription();
                    var isAxGx = scene.IsFileGX ? "GX" : "AX";

                    // Get all the scene object references
                    var objectsList = new List<SceneObjectLOD>();
                    foreach (var templateSceneObject in scene.sceneObjects)
                    {
                        var sceneObjects = templateSceneObject.lods;
                        foreach (var sceneObject in sceneObjects)
                            objectsList.Add(sceneObject);
                    }
                    //foreach (var staticSceneObject in scene.staticSceneObjects)
                    //{
                    //    var sceneObjects = staticSceneObject.templateSceneObject.sceneObjects;
                    //    foreach (var sceneObject in sceneObjects)
                    //        objectsList.Add((sceneObject, "Instance"));
                    //}

                    // iterate
                    foreach (var sceneObjectReference in objectsList)
                    {
                        writer.WriteNextCol(scene.FileName);
                        writer.WriteNextCol(scene.ID);
                        writer.WriteNextCol(venueID);
                        writer.WriteNextCol(courseID);
                        writer.WriteNextCol(isAxGx);
                        //
                        writer.WriteNextCol(sceneObjectReference.name);
                        writer.WriteNextCol(sceneObjectReference.zero_0x00);
                        writer.WriteNextCol(sceneObjectReference.lodNamePtr);
                        writer.WriteNextCol(sceneObjectReference.zero_0x08);
                        writer.WriteNextCol(sceneObjectReference.lodDistance);
                        //
                        writer.WriteNextRow();
                    }
                }
                writer.Flush();
            }
        }

        public static void AnalyzeSceneObjects(ColiScene[] scenes, string fileName)
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
                writer.WriteNextCol(nameof(SceneObject.lodRenderFlags));
                writer.WriteNextCol(nameof(SceneObject.lodsPtr));
                writer.WriteNextCol(nameof(SceneObject.colliderGeometryPtr));
                //
                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    var venueID = CourseUtility.GetVenueID(scene.ID).GetDescription();
                    var courseID = ((CourseIndexAX)scene.ID).GetDescription();
                    var isAxGx = scene.IsFileGX ? "GX" : "AX";

                    // Get all the scene object references
                    var sceneObjectsList = new List<(SceneObject sir, string category)>();
                    foreach (var sceneInstance in scene.sceneObjects)
                    {
                        sceneObjectsList.Add((sceneInstance, "Instance"));
                    }
                    foreach (var sceneOriginObject in scene.staticSceneObjects)
                    {
                        var sceneInstance = sceneOriginObject.sceneObject;
                        sceneObjectsList.Add((sceneInstance, "Origin"));
                    }

                    // iterate
                    foreach (var sceneObject in sceneObjectsList)
                    {
                        writer.WriteNextCol(scene.FileName);
                        writer.WriteNextCol(scene.ID);
                        writer.WriteNextCol(venueID);
                        writer.WriteNextCol(courseID);
                        writer.WriteNextCol(isAxGx);
                        //
                        writer.WriteNextCol(sceneObject.sir.PrimarySceneObject.name);
                        writer.WriteNextCol(sceneObject.category);
                        writer.WriteNextCol(sceneObject.sir.lodRenderFlags);
                        writer.WriteNextCol(sceneObject.sir.lodsPtr);
                        writer.WriteNextCol(sceneObject.sir.colliderGeometryPtr);
                        //
                        writer.WriteNextRow();
                    }
                }
                writer.Flush();
            }
        }

        public static void AnalyzeSceneObjectsAndLODs(ColiScene[] scenes, string fileName)
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
                writer.WriteNextCol(nameof(SceneObject.lodRenderFlags));
                writer.WriteNextCol(nameof(SceneObject.lodsPtr) + " Len");
                writer.WriteNextCol(nameof(SceneObject.lodsPtr) + " Adr");
                writer.WriteNextCol(nameof(SceneObject.colliderGeometryPtr));
                writer.WriteNextCol(nameof(SceneObjectLOD) + " IDX");
                writer.WriteNextCol(nameof(SceneObjectLOD.zero_0x00));
                writer.WriteNextCol(nameof(SceneObjectLOD.lodNamePtr));
                writer.WriteNextCol(nameof(SceneObjectLOD.zero_0x08));
                writer.WriteNextCol(nameof(SceneObjectLOD.lodDistance));
                writer.WriteNextCol(nameof(SceneObjectLOD.name));
                //
                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    var venueID = CourseUtility.GetVenueID(scene.ID).GetDescription();
                    var courseID = ((CourseIndexAX)scene.ID).GetDescription();
                    var isAxGx = scene.IsFileGX ? "GX" : "AX";

                    foreach (var template in scene.sceneObjects)
                    {
                        var index = 0;
                        var length = template.lods.Length;
                        foreach (var sceneObject in template.lods)
                        {
                            writer.WriteNextCol(scene.FileName);
                            writer.WriteNextCol(scene.ID);
                            writer.WriteNextCol(venueID);
                            writer.WriteNextCol(courseID);
                            writer.WriteNextCol(isAxGx);
                            //
                            writer.WriteNextCol(template.Name);
                            writer.WriteNextCol(template.lodRenderFlags);
                            writer.WriteNextCol(template.lodsPtr.Length);
                            writer.WriteNextCol(template.lodsPtr.HexAddress);
                            writer.WriteNextCol(template.colliderGeometryPtr);
                            writer.WriteNextCol($"[{++index}/{length}]");
                            writer.WriteNextCol(sceneObject.zero_0x00);
                            writer.WriteNextCol(sceneObject.lodNamePtr);
                            writer.WriteNextCol(sceneObject.zero_0x08);
                            writer.WriteNextCol(sceneObject.lodDistance);
                            writer.WriteNextCol(sceneObject.name);
                            writer.WriteNextRow();
                        }
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


        public static void AnalyzeUnknownCollider(ColiScene[] scenes, string fileName)
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
                writer.WriteNextCol(nameof(UnknownCollider.templateSceneObjectPtr));
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
                        writer.WriteNextCol(unkSols.templateSceneObjectPtr);
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

    }
}
