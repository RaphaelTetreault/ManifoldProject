using Manifold.IO;
using Manifold.IO.GFZ;
using Manifold.EditorTools.GFZ.Stage;
using GameCube.GFZ.CourseCollision;
using System;
using System.IO;
using System.Linq;
using UnityEditor;

namespace Manifold.EditorTools.GFZ.UnityMenu
{
    public class TableLogStageMenu
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
            TableLog.Stage.Analyze(scenes, output);
            OSUtility.OpenDirectory(settings.AnalysisOutput);
        }


        [MenuItem(menu + "Coli Scene (Header)")]
        public static void MenuAnalyzeHeaders() => MenuForward(TableLog.Stage.AnalyzeHeaders, TableLog.Stage.tsvHeader);

        [MenuItem(menu + "General Data")]
        public static void MenuAnalyzeGeneralData() => MenuForward(TableLog.Stage.AnalyzeGeneralData, TableLog.Stage.tsvGeneralData);


        [MenuItem(menu + "Track Keyables All")]
        public static void MenuAnalyzeTrackKeyablesAll() => MenuForward(TableLog.Stage.AnalyzeTrackKeyablesAll, TableLog.Stage.tsvTrackKeyablesAll);

        [MenuItem(menu + "Track Transforms")]
        public static void MenuAnalyzeTrackSegments() => MenuForward(TableLog.Stage.AnalyzeTrackSegments, TableLog.Stage.tsvTrackSegment);

        [MenuItem(menu + "Surface Attribute Areas")]
        public static void MenuAnalyzeSurfaceAttributeAreas() => MenuForward(TableLog.Stage.AnalyzeSurfaceAttributeAreas, TableLog.Stage.tsvSurfaceAttributeArea);

        [MenuItem(menu + "Track Nodes")]
        public static void MenuAnalyzeTrackNodes() => MenuForward(TableLog.Stage.AnalyzeTrackNodes, TableLog.Stage.tsvTrackNode);


        [MenuItem(menu + "Scene Objects")]
        public static void MenuAnalyzeSceneObjects() => MenuForward(TableLog.Stage.AnalyzeSceneObjects, TableLog.Stage.tsvSceneObject);

        [MenuItem(menu + "Scene Object LODs")]
        public static void MenuAnalyzeSceneObjectLODs() => MenuForward(TableLog.Stage.AnalyzeSceneObjectLODs, TableLog.Stage.tsvSceneObjectLod);

        [MenuItem(menu + "Scene Object + Scene Object LODs")]
        public static void MenuAnalyzeSceneObjectsAndLODs() => MenuForward(TableLog.Stage.AnalyzeSceneObjectsAndLODs, TableLog.Stage.tsvSceneObjectsAndLod);


        [MenuItem(menu + "Scene Objects Dynamic")]
        public static void MenuAnalyzeSceneObjectDynamic() => MenuForward(TableLog.Stage.AnalyzeSceneObjectDynamic, TableLog.Stage.tsvSceneObjectDynamic);

        [MenuItem(menu + "Scene Objects Dynamic - Animation Clip")]
        public static void MenuAnalyzeAnimationClips() => MenuForward(TableLog.Stage.AnalyzeAnimationClips, TableLog.Stage.tsvAnimationClip);

        [MenuItem(menu + "Scene Objects Dynamic - Texture Metadata")]
        public static void MenuAnalyzeTextureMetadata() => MenuForward(TableLog.Stage.AnalyzeTextureMetadata, TableLog.Stage.tsvTextureMetadata);

        [MenuItem(menu + "Scene Objects Dynamic - Skeletal Animator")]
        public static void MenuAnalyzeSkeletalAnimator() => MenuForward(TableLog.Stage.AnalyzeSkeletalAnimator, TableLog.Stage.tsvSkeletalAnimator);

        [MenuItem(menu + "Scene Objects Dynamic - Collider Geometry (Tris)")]
        public static void MenuAnalyzeColliderGeometryTri() => MenuForward(TableLog.Stage.AnalyzeColliderGeometryTri, TableLog.Stage.tsvColliderGeometryTri);

        [MenuItem(menu + "Scene Objects Dynamic - Collider Geometry (Quads)")]
        public static void MenuAnalyzeColliderGeometryQuad() => MenuForward(TableLog.Stage.AnalyzeColliderGeometryQuad, TableLog.Stage.tsvColliderGeometryQuad);

        [MenuItem(menu + "Scene Object Dynamic - Transform")]
        public static void MenuAnalyzeSceneObjectTransforms() => MenuForward(TableLog.Stage.AnalyzeSceneObjectTransforms, TableLog.Stage.tsvTransform);


        [MenuItem(menu + "Trigger - Arcade Checkpoints")]
        public static void MenuAnalyzeArcadeCheckpointTriggers() => MenuForward(TableLog.Stage.AnalyzeArcadeCheckpointTriggers, TableLog.Stage.tsvArcadeCheckpointTrigger);

        [MenuItem(menu + "Trigger - Course Metadata")]
        public static void MenuAnalyzeCourseMetadataTriggers() => MenuForward(TableLog.Stage.AnalyzeCourseMetadataTriggers, TableLog.Stage.tsvCourseMetadataTrigger);

        [MenuItem(menu + "Trigger - Story Object")]
        public static void MenuAnalyzeStoryObjectTrigger() => MenuForward(TableLog.Stage.AnalyzeStoryObjectTrigger, TableLog.Stage.tsvStoryObjectTrigger);

        [MenuItem(menu + "Trigger - Unknown Trigger")]
        public static void MenuAnalyzeUnknownTrigger() => MenuForward(TableLog.Stage.AnalyzeUnknownTrigger, TableLog.Stage.tsvUnknownTrigger);

        [MenuItem(menu + "Trigger - Visual Effect Trigger")]
        public static void MenuAnalyzeVisualEffectTriggers() => MenuForward(TableLog.Stage.AnalyzeVisualEffectTriggers, TableLog.Stage.tsvVisualEffectTrigger);


        [MenuItem(menu + "Fog")]
        public static void MenuAnalyzeFog() => MenuForward(TableLog.Stage.AnalyzeFog, TableLog.Stage.tsvFog);

        [MenuItem(menu + "Fog Curves")]
        public static void MenuAnalyzeFogCurves() => MenuForward(TableLog.Stage.AnalyzeFogCurves, TableLog.Stage.tsvFogCurves);



        [MenuItem(menu + "Static Collider Meshses")]
        public static void MenuAnalyzeSceneStaticCollider() => MenuForward(TableLog.Stage.AnalyzeStaticColliderMeshes, TableLog.Stage.tsvStaticColliderMeshes);

        [MenuItem(menu + "Unknown Collider")]
        public static void MenuAnalyzeUnknownCollider() => MenuForward(TableLog.Stage.AnalyzeUnknownCollider, TableLog.Stage.tsvUnknownCollider);


    }
}
