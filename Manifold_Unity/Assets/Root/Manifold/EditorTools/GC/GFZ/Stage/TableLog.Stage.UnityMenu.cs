using Manifold.IO;
using GameCube.GFZ.Stage;
using static GameCube.GFZ.Stage.StageTableLogger;
using System;
using System.IO;
using System.Linq;
using UnityEditor;


namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public class StageTableLoggerUnityMenu
    {
        private const string menu = Const.Menu.Manifold + "Analysis/";


        /// <summary>
        /// Forwards most analysis functions 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="fileName"></param>
        public static void MenuForward(Action<Scene[], string> action, string fileName)
        {
            var settings = GfzProjectWindow.GetSettings();
            var sceneIterator = ColiCourseIO.LoadAllStages(settings.StageDir, "Loading Stages...");
            var scenes = sceneIterator.ToArray();
            var outputPath = settings.AnalysisOutput + fileName;
            action.Invoke(scenes, outputPath);
            OSUtility.OpenDirectory(settings.AnalysisOutput);
        }

        public static void Analyze(Scene[] scenes, string outputPath)
        {
            var progress = 1f;
            var title = "Analysing All ColiScene Files...";
            var time = DateTime.Now.ToString();
            bool cancel;

            var destination = Path.Combine(outputPath, time);
            Directory.CreateDirectory(destination);

            // GENERAL DATA
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvGeneralData, progress);
            AnalyzeGeneralData(scenes, Path.Combine(destination, tsvGeneralData));
            if (cancel) return;

            // FOG
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvFog, progress);
            AnalyzeFog(scenes, Path.Combine(destination, tsvFog));
            if (cancel) return;

            // FOG CURVES
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvFogCurves, progress);
            AnalyzeFogCurves(scenes, Path.Combine(destination, tsvFogCurves));
            if (cancel) return;

            // COLI SCENE HEADER
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvHeader, progress);
            AnalyzeHeaders(scenes, Path.Combine(destination, tsvHeader));
            if (cancel) return;

            // ANIMATION CLIPS
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvAnimationClip, progress);
            AnalyzeAnimationClips(scenes, Path.Combine(destination, tsvAnimationClip));
            if (cancel) return;

            // ANIMATIONS INDIVIDUALIZED
            {
                var count = AnimationClip.kAnimationCurvesCount;
                for (int i = 0; i < count; i++)
                {
                    var fileName = $"{nameof(SceneObjectDynamic)}-{nameof(AnimationClip)}[{i}].tsv";
                    var filePath = Path.Combine(destination, fileName);
                    cancel = EditorUtility.DisplayCancelableProgressBar(title, fileName, (float)(i + 1) / count);
                    AnalyzeGameObjectAnimationClipIndex(scenes, filePath, i);
                    if (cancel) return;
                }
            }

            // TRANSFORMS
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvTransform, progress);
            AnalyzeSceneObjectTransforms(scenes, Path.Combine(destination, tsvTransform));
            if (cancel) return;

            // ARCADE CHECKPOINT TRIGGERS (AX)
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvTimeExtensionTrigger, progress);
            AnalyzeArcadeCheckpointTriggers(scenes, Path.Combine(destination, tsvTimeExtensionTrigger));
            if (cancel) return;

            // COURSE METADATA TRIGGERS
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvMiscellaneousTrigger, progress);
            AnalyzeCourseMetadataTriggers(scenes, Path.Combine(destination, tsvMiscellaneousTrigger));
            if (cancel) return;

            // STORY OBJECT TRIGGERS
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvStoryObjectTrigger, progress);
            AnalyzeStoryObjectTrigger(scenes, Path.Combine(destination, tsvStoryObjectTrigger));
            if (cancel) return;

            // UNKNOWN TRIGGERS
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvUnknownTrigger, progress);
            AnalyzeUnknownTrigger(scenes, Path.Combine(destination, tsvUnknownTrigger));
            if (cancel) return;

            // VISUAL EFFECT TRIGGERS
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvVisualEffectTrigger, progress);
            AnalyzeVisualEffectTriggers(scenes, Path.Combine(destination, tsvVisualEffectTrigger));
            if (cancel) return;


            // UNKNOWN COLLIDERS
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvUnknownCollider, progress);
            AnalyzeUnknownCollider(scenes, Path.Combine(destination, tsvUnknownCollider));
            if (cancel) return;


            // SCENE OBJECT DYNAMIC
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvSceneObjectDynamic, progress);
            AnalyzeSceneObjectDynamic(scenes, Path.Combine(destination, tsvSceneObjectDynamic));
            if (cancel) return;

            // TEXTURE METADATA
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvTextureMetadata, progress);
            AnalyzeTextureMetadata(scenes, Path.Combine(destination, tsvTextureMetadata));
            if (cancel) return;

            // SKELETAL ANIMATOR
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvSkeletalAnimator, progress);
            AnalyzeSkeletalAnimator(scenes, Path.Combine(destination, tsvSkeletalAnimator));
            if (cancel) return;

            // COLLIDER GEOMETRY (TRI)
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvColliderGeometryTri, progress);
            AnalyzeColliderGeometryTri(scenes, Path.Combine(destination, tsvColliderGeometryTri));
            if (cancel) return;

            // COLLIDER GEOMETRY (QUAD)
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvColliderGeometryQuad, progress);
            AnalyzeColliderGeometryQuad(scenes, Path.Combine(destination, tsvColliderGeometryQuad));
            if (cancel) return;

            // SCENE OBJECT LOD
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvSceneObjectLod, progress);
            AnalyzeSceneObjectLODs(scenes, Path.Combine(destination, tsvSceneObjectLod));
            if (cancel) return;

            // SCENE OBJECT
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvSceneObject, progress);
            AnalyzeSceneObjects(scenes, Path.Combine(destination, tsvSceneObject));
            if (cancel) return;

            // SCENE OBJECTS + SCENE OBJECT LODS
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvSceneObjectsAndLod, progress);
            AnalyzeSceneObjectsAndLODs(scenes, Path.Combine(destination, tsvSceneObjectsAndLod));
            if (cancel) return;

            // TRACK SEGMENT KEYABLES (INDEXED)
            {
                var count = GameCube.GFZ.Stage.AnimationCurveTRS.kCurveCount;
                for (int i = 0; i < count; i++)
                {
                    var fileName = $"{nameof(GameCube.GFZ.Stage.AnimationCurveTRS)}[{i + 1}].tsv";
                    var filePath = Path.Combine(destination, fileName);
                    EditorUtility.DisplayProgressBar(title, filePath, (float)(i + 1) / GameCube.GFZ.Stage.AnimationCurveTRS.kCurveCount);
                    AnalyzeTrackKeyables(scenes, filePath, i);
                }
            }

            // TRACK SEGMENT KEYABLES (ALL)
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvTrackKeyablesAll, progress);
            AnalyzeTrackKeyablesAll(scenes, Path.Combine(destination, tsvTrackKeyablesAll));
            if (cancel) return;

            // TRACK SEGMENTS
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvTrackSegment, progress);
            AnalyzeTrackSegments(scenes, Path.Combine(destination, tsvTrackSegment));
            if (cancel) return;

            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvTrackNode, progress);
            AnalyzeTrackNodes(scenes, Path.Combine(destination, tsvTrackNode));
            if (cancel) return;

            // STATIC COLLIDER MESHES
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvStaticColliderMeshes, progress);
            AnalyzeStaticColliderMeshes(scenes, Path.Combine(destination, tsvStaticColliderMeshes));
            if (cancel) return;

            // SURFACE ATTRIBUTE AREAS
            cancel = EditorUtility.DisplayCancelableProgressBar(title, tsvSurfaceAttributeArea, progress);
            AnalyzeSurfaceAttributeAreas(scenes, Path.Combine(destination, tsvSurfaceAttributeArea));
            if (cancel) return;
        }


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
            EditorUtility.ClearProgressBar();
        }


        [MenuItem(menu + "Coli Scene (Header)")]
        public static void MenuAnalyzeHeaders() => MenuForward(AnalyzeHeaders, tsvHeader);

        [MenuItem(menu + "General Data")]
        public static void MenuAnalyzeGeneralData() => MenuForward(AnalyzeGeneralData, tsvGeneralData);


        [MenuItem(menu + "Track Keyables All")]
        public static void MenuAnalyzeTrackKeyablesAll() => MenuForward(AnalyzeTrackKeyablesAll, tsvTrackKeyablesAll);

        [MenuItem(menu + "Track Segments")]
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
        public static void MenuAnalyzeArcadeCheckpointTriggers() => MenuForward(AnalyzeArcadeCheckpointTriggers, tsvTimeExtensionTrigger);

        [MenuItem(menu + "Trigger - Course Metadata")]
        public static void MenuAnalyzeCourseMetadataTriggers() => MenuForward(AnalyzeCourseMetadataTriggers, tsvMiscellaneousTrigger);

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


    }
}
