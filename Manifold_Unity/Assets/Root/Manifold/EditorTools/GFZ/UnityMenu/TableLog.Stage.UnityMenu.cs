using Manifold.IO;
using Manifold.IO.GFZ;
using GameCube.GFZ.CourseCollision;
using System;
using System.IO;
using System.Linq;
using UnityEditor;

namespace Manifold.EditorTools.GFZ.UnityMenu
{
    public class StageTableLoggerUnityMenu
    {
        private const string menu = Const.Menu.Manifold + "Analysis/";


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


        [MenuItem(menu + "Analyze all ColiScene Data #F5")]
        public static void AnalyzeAllPartsOfColiCourse()
        {
            var settings = GfzProjectWindow.GetSettings();
            var sceneIterator = ColiCourseIO.LoadAllStages(settings.StageDir, "Loading Stages...");
            var scenes = sceneIterator.ToArray();
            var output = settings.AnalysisOutput;
            Directory.CreateDirectory(output);
            StageTableLogger.Analyze(scenes, output);
            OSUtility.OpenDirectory(settings.AnalysisOutput);
        }


        [MenuItem(menu + "Coli Scene (Header)")]
        public static void MenuAnalyzeHeaders() => MenuForward(StageTableLogger.AnalyzeHeaders, StageTableLogger.tsvHeader);

        [MenuItem(menu + "General Data")]
        public static void MenuAnalyzeGeneralData() => MenuForward(StageTableLogger.AnalyzeGeneralData, StageTableLogger.tsvGeneralData);


        [MenuItem(menu + "Track Keyables All")]
        public static void MenuAnalyzeTrackKeyablesAll() => MenuForward(StageTableLogger.AnalyzeTrackKeyablesAll, StageTableLogger.tsvTrackKeyablesAll);

        [MenuItem(menu + "Track Transforms")]
        public static void MenuAnalyzeTrackSegments() => MenuForward(StageTableLogger.AnalyzeTrackSegments, StageTableLogger.tsvTrackSegment);

        [MenuItem(menu + "Surface Attribute Areas")]
        public static void MenuAnalyzeSurfaceAttributeAreas() => MenuForward(StageTableLogger.AnalyzeSurfaceAttributeAreas, StageTableLogger.tsvSurfaceAttributeArea);

        [MenuItem(menu + "Track Nodes")]
        public static void MenuAnalyzeTrackNodes() => MenuForward(StageTableLogger.AnalyzeTrackNodes, StageTableLogger.tsvTrackNode);


        [MenuItem(menu + "Scene Objects")]
        public static void MenuAnalyzeSceneObjects() => MenuForward(StageTableLogger.AnalyzeSceneObjects, StageTableLogger.tsvSceneObject);

        [MenuItem(menu + "Scene Object LODs")]
        public static void MenuAnalyzeSceneObjectLODs() => MenuForward(StageTableLogger.AnalyzeSceneObjectLODs, StageTableLogger.tsvSceneObjectLod);

        [MenuItem(menu + "Scene Object + Scene Object LODs")]
        public static void MenuAnalyzeSceneObjectsAndLODs() => MenuForward(StageTableLogger.AnalyzeSceneObjectsAndLODs, StageTableLogger.tsvSceneObjectsAndLod);


        [MenuItem(menu + "Scene Objects Dynamic")]
        public static void MenuAnalyzeSceneObjectDynamic() => MenuForward(StageTableLogger.AnalyzeSceneObjectDynamic, StageTableLogger.tsvSceneObjectDynamic);

        [MenuItem(menu + "Scene Objects Dynamic - Animation Clip")]
        public static void MenuAnalyzeAnimationClips() => MenuForward(StageTableLogger.AnalyzeAnimationClips, StageTableLogger.tsvAnimationClip);

        [MenuItem(menu + "Scene Objects Dynamic - Texture Metadata")]
        public static void MenuAnalyzeTextureMetadata() => MenuForward(StageTableLogger.AnalyzeTextureMetadata, StageTableLogger.tsvTextureMetadata);

        [MenuItem(menu + "Scene Objects Dynamic - Skeletal Animator")]
        public static void MenuAnalyzeSkeletalAnimator() => MenuForward(StageTableLogger.AnalyzeSkeletalAnimator, StageTableLogger.tsvSkeletalAnimator);

        [MenuItem(menu + "Scene Objects Dynamic - Collider Geometry (Tris)")]
        public static void MenuAnalyzeColliderGeometryTri() => MenuForward(StageTableLogger.AnalyzeColliderGeometryTri, StageTableLogger.tsvColliderGeometryTri);

        [MenuItem(menu + "Scene Objects Dynamic - Collider Geometry (Quads)")]
        public static void MenuAnalyzeColliderGeometryQuad() => MenuForward(StageTableLogger.AnalyzeColliderGeometryQuad, StageTableLogger.tsvColliderGeometryQuad);

        [MenuItem(menu + "Scene Object Dynamic - Transform")]
        public static void MenuAnalyzeSceneObjectTransforms() => MenuForward(StageTableLogger.AnalyzeSceneObjectTransforms, StageTableLogger.tsvTransform);


        [MenuItem(menu + "Trigger - Arcade Checkpoints")]
        public static void MenuAnalyzeArcadeCheckpointTriggers() => MenuForward(StageTableLogger.AnalyzeArcadeCheckpointTriggers, StageTableLogger.tsvArcadeCheckpointTrigger);

        [MenuItem(menu + "Trigger - Course Metadata")]
        public static void MenuAnalyzeCourseMetadataTriggers() => MenuForward(StageTableLogger.AnalyzeCourseMetadataTriggers, StageTableLogger.tsvCourseMetadataTrigger);

        [MenuItem(menu + "Trigger - Story Object")]
        public static void MenuAnalyzeStoryObjectTrigger() => MenuForward(StageTableLogger.AnalyzeStoryObjectTrigger, StageTableLogger.tsvStoryObjectTrigger);

        [MenuItem(menu + "Trigger - Unknown Trigger")]
        public static void MenuAnalyzeUnknownTrigger() => MenuForward(StageTableLogger.AnalyzeUnknownTrigger, StageTableLogger.tsvUnknownTrigger);

        [MenuItem(menu + "Trigger - Visual Effect Trigger")]
        public static void MenuAnalyzeVisualEffectTriggers() => MenuForward(StageTableLogger.AnalyzeVisualEffectTriggers, StageTableLogger.tsvVisualEffectTrigger);


        [MenuItem(menu + "Fog")]
        public static void MenuAnalyzeFog() => MenuForward(StageTableLogger.AnalyzeFog, StageTableLogger.tsvFog);

        [MenuItem(menu + "Fog Curves")]
        public static void MenuAnalyzeFogCurves() => MenuForward(StageTableLogger.AnalyzeFogCurves, StageTableLogger.tsvFogCurves);



        [MenuItem(menu + "Static Collider Meshses")]
        public static void MenuAnalyzeSceneStaticCollider() => MenuForward(StageTableLogger.AnalyzeStaticColliderMeshes, StageTableLogger.tsvStaticColliderMeshes);

        [MenuItem(menu + "Unknown Collider")]
        public static void MenuAnalyzeUnknownCollider() => MenuForward(StageTableLogger.AnalyzeUnknownCollider, StageTableLogger.tsvUnknownCollider);


    }
}
