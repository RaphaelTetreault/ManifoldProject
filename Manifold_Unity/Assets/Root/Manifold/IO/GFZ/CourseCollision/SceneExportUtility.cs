using GameCube.GFZ;
using GameCube.GFZ.CourseCollision;
using Manifold.IO;
using Manifold.IO.GFZ;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Unity.Mathematics;
using System.Linq;

namespace Manifold.IO.GFZ.CourseCollision
{
    public static class SceneExportUtility
    {

        public static void Temp()
        {
            //// 
            //coliScenes[sceneIndex] = new ColiScene();

            //// Breakout value
            //var scene = scenes[sceneIndex];
            //var coliScene = coliScenes[sceneIndex];

            //// Set serialization format
            //coliScene.Format = format;

            //// Get path to scene object
            //var scenePath = AssetDatabase.GetAssetPath(scene);
            //// Open scene in Unity...
            //EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            //// ... so we can start doing the usual Unity methods to find stuff
        }

        [MenuItem(Const.Menu.Manifold + "Scene Generation/Export (Active Scene)")]
        public static void ExportSceneActive()
        {
            var format = ColiScene.SerializeFormat.GX;
            ExportScene(format, true, true);
        }

        public static void ExportScene(ColiScene.SerializeFormat format, bool verbose, bool findInactive)
        {
            var settings = GfzProjectWindow.GetSettings();
            var outputPath = settings.SceneExportPath;
            var activeScene = EditorSceneManager.GetActiveScene();


            // Get scene parameters for general info
            var gfzSceneParameters = GameObject.FindObjectsOfType<GfzSceneParameters>();
            var assertMsg = $"More than one {nameof(GfzSceneParameters)} found in scene! There can only be one per scene.";
            Assert.IsTrue(gfzSceneParameters.Length == 1, assertMsg);
            var sceneParams = gfzSceneParameters[0];

            // This object contains the original scene deserialized. Use it in the meantime to get
            // data for which I can't/don't want to construct.
            //var gfzLegacyData = GameObject.FindObjectOfType<GfzLegacyData>();
            //var oldScene = gfzLegacyData.Scene;

            // 
            //var internalName = sceneParams.GetGfzInternalName();
            //var displayName = sceneParams.GetGfzDisplayName();

            // Before we do the work of exporting, see if the stage we are exporting (index/venue) align correctly.
            // If we export as Lightning but the index is, say, 1, index 1 belongs to Mute City. Warn of potential issues.
            bool isValidIndexForVenue = CourseUtility.GetVenue(sceneParams.courseIndex) == sceneParams.venue;
            if (!isValidIndexForVenue)
            {
                var msg =
                    $"The assigned venue '{sceneParams.venue}' and the stage index being used '{sceneParams.courseIndex}' " +
                    $"do not share the same venue. When loaded in-game, models will not load.";
                var title = "Export Scene: Venue/Index Mismatch";

                bool doExport = EditorUtility.DisplayDialog(title, msg, "Export Anyway");
                if (!doExport)
                {
                    return;
                }
            }

            // TODO: this should be in ColiScene
            LibGxFormat.AvGame compressFormat;
            switch (format)
            {
                case ColiScene.SerializeFormat.AX:
                    compressFormat = LibGxFormat.AvGame.FZeroAX;
                    break;

                case ColiScene.SerializeFormat.GX:
                    compressFormat = LibGxFormat.AvGame.FZeroGX;
                    break;

                case ColiScene.SerializeFormat.InvalidFormat:
                    throw new ArgumentException("No format specified for serialization!");

                default:
                    throw new NotImplementedException();
            }

            var mirroredObjects = GameObject.FindObjectsOfType<GfzMirroredObject>();
            foreach (var mirroredObject in mirroredObjects)
                mirroredObject.MirrorTransformZ();

            // TEST
            // Load the old stage to use it's data I don't know how to generate yet
            //var oldScene = ColiCourseIO.LoadScene(settings.StageDir + scene.FileName);

            // TEST
            // load in real, modify what you'd like
            var scene = ColiCourseIO.LoadScene(settings.StageDir + sceneParams.GetGfzInternalName());
            // Serialization settings
            scene.Format = format;
            scene.SerializeVerbose = verbose;
            // Exported filename 'COLI_COURSE##'
            //FileName = sceneParams.GetGfzInternalName();
            // Data about the creator
            scene.Author = sceneParams.author;
            scene.Venue = sceneParams.venue;
            scene.CourseName = sceneParams.courseName;

            //// Build a new scene!
            //var scene = new ColiScene()
            //{
            //    // Serialization settings
            //    Format = format,
            //    SerializeVerbose = verbose,
            //    // Exported filename 'COLI_COURSE##'
            //    FileName = sceneParams.GetGfzInternalName(),
            //    // Data about the creator
            //    Author = sceneParams.author,
            //    Venue = sceneParams.venue,
            //    CourseName = sceneParams.courseName,
            //};


            // Get scene-wide parameters from SceneParameters
            {
                // Construct range from 2 parameters
                scene.unkRange0x00 = new Range(sceneParams.rangeNear, sceneParams.rangeFar);
                // Use functions to get form parameters
                scene.fog = sceneParams.ToGfzFog();
                scene.fogCurves = sceneParams.ToGfzFogCurves();
            }

            // Triggers
            {
                var arcadeCheckpointTriggers = GameObject.FindObjectsOfType<GfzArcadeCheckpoint>(findInactive);
                scene.arcadeCheckpointTriggers = GetGfzValues(arcadeCheckpointTriggers);

                // This trigger type is a mess... Get all 3 representations, combine, assign.
                // Collect all trigger types. They all get converted to the same GFZ base type.
                var objectPaths = GameObject.FindObjectsOfType<GfzObjectPath>(findInactive);
                var storyCapsules = GameObject.FindObjectsOfType<GfzStoryCapsule>(findInactive);
                var unknownMetadataTriggers = GameObject.FindObjectsOfType<GfzUnknownCourseMetadataTrigger>(findInactive);
                // Make a list, add range for each type
                var courseMetadataTriggers = new List<CourseMetadataTrigger>();
                courseMetadataTriggers.AddRange(GetGfzValues(objectPaths));
                courseMetadataTriggers.AddRange(GetGfzValues(storyCapsules));
                courseMetadataTriggers.AddRange(GetGfzValues(unknownMetadataTriggers));
                // Convert list to array, assign to ColiScene
                scene.courseMetadataTriggers = courseMetadataTriggers.ToArray();

                // This trigger type is a mess... Get all 3 representations, combine, assign.
                var unknownTriggers = GameObject.FindObjectsOfType<GfzUnknownTrigger>(findInactive);
                scene.unknownTriggers = GetGfzValues(unknownTriggers);

                // 
                var visualEffectTriggers = GameObject.FindObjectsOfType<GfzVisualEffectTrigger>(findInactive);
                scene.visualEffectTriggers = GetGfzValues(visualEffectTriggers);

                // TODO:
                var storyObjectTrigger = GameObject.FindObjectsOfType<GfzStoryObjectTrigger>(findInactive);
                scene.storyObjectTriggers = GetGfzValues(storyObjectTrigger);
            }

            // Scene Objects
            {
                // SCENE OBJECTs
                var gfzDynamicSceneObjects = GameObject.FindObjectsOfType<GfzSceneObjectDynamic>(true).Reverse().ToArray();
                var gfzStaticSceneObjects = GameObject.FindObjectsOfType<GfzSceneObjectStatic>(true).Reverse().ToArray();
                var gfzSceneObjects = GameObject.FindObjectsOfType<GfzSceneObject>(true).Reverse().ToArray();
                var gfzSceneObjectLODs = GameObject.FindObjectsOfType<GfzSceneObjectLODs>(true).Reverse().ToArray();

                // Init shared references before copying values out.
                foreach (var gfzSceneObject in gfzSceneObjects)
                    gfzSceneObject.InitSharedReference();

                // STATIC / DYNAMIC
                scene.dynamicSceneObjects = GetGfzValues(gfzDynamicSceneObjects);
                scene.staticSceneObjects = GetGfzValues(gfzStaticSceneObjects);
                scene.sceneObjects = GetGfzValues(gfzSceneObjects);

                // LODs
                var sceneObjectLODs = new List<SceneObjectLOD>();
                foreach (var sceneObject in scene.sceneObjects)
                {
                    sceneObjectLODs.AddRange(sceneObject.lods);
                }
                scene.sceneObjectLODs = sceneObjectLODs.ToArray();

                // CString names
                // TODO: share references
                var sceneObjectNames = new List<CString>();
                sceneObjectNames.Add("");
                foreach (var thing in scene.sceneObjectLODs)
                {
                    //if (!sceneObjectNames.Contains(thing.name))
                    //{
                        sceneObjectNames.Add(thing.name);
                    //}
                }
                // alphabetize, store
                scene.sceneObjectNames = sceneObjectNames.OrderBy(x => x.value).ToArray();
            }

            // Static Collider Meshes
            {
                // TODO: generate from GFZ scene data

                //var unknownColliders = GameObject.FindObjectsOfType<GfzUnknownCollider>(findInactive);
                //scene.unknownColliders = GetGfzValues(unknownColliders
                scene.unknownColliders = new UnknownCollider[0];

                // Static Collider Matrix
                scene.staticColliderMeshes = new StaticColliderMeshes(format);
                // Bind to other references
                scene.staticColliderMeshes.unknownColliders = scene.unknownColliders;
                scene.staticColliderMeshes.staticSceneObjects = scene.staticSceneObjects;
                //scene.staticColliderMeshes.ComputeMatrixBoundsXZ();
                scene.staticColliderMeshes.meshBounds = new MatrixBoundsXZ();
                
                // Get data from scene
                //scene.staticColliderMeshes = oldScene.staticColliderMeshes;
                scene.staticColliderMeshes.SerializeFormat = format;
                // Point to existing references
                scene.staticColliderMeshes.unknownColliders = scene.unknownColliders;
                scene.staticColliderMeshes.staticSceneObjects = scene.staticSceneObjects;
            }

            //// Track Data
            //{
            //    // Actual track data
            //    scene.allTrackSegments = oldScene.allTrackSegments;
            //    scene.rootTrackSegments = oldScene.rootTrackSegments;

            //    // Checkpoint data
            //    scene.trackNodes = oldScene.trackNodes;
            //    scene.trackCheckpointBoundsXZ = oldScene.trackCheckpointBoundsXZ;
            //    scene.trackCheckpointMatrix = oldScene.trackCheckpointMatrix;

            //    // Track metadata
            //    scene.trackMinHeight = oldScene.trackMinHeight;
            //    scene.trackLength = oldScene.trackLength;

            //    //
            //    scene.surfaceAttributeAreas = oldScene.surfaceAttributeAreas;
            //}

            //
            MakeTrueNulls(scene);

            // Save out file and LZ'ed file
            var outputFile = outputPath + scene.FileName;
            using (var writer = new BinaryWriter(File.Create(outputFile)))
            {
                writer.WriteX(scene);
                writer.Flush();
            }
            GfzUtility.CompressAvLzToDisk(outputFile, compressFormat);
            OSUtility.OpenDirectory(outputPath);

            foreach (var mirroredObject in mirroredObjects)
                mirroredObject.MirrorTransformZ();
        }


        public static void MakeTrueNulls(ColiScene scene)
        {
            // TEMP
            // This is because I must handle Unity serializing nulls with empty instances
            foreach (var sceneObject in scene.sceneObjects)
            {
                var colliderGeo = sceneObject.colliderMesh;
                if (colliderGeo != null)
                {
                    if (colliderGeo.tris != null)
                        if (colliderGeo.tris.Length == 0)
                            colliderGeo.tris = null;

                    if (colliderGeo.quads != null)
                        if (colliderGeo.quads.Length == 0)
                            colliderGeo.quads = null;
                }
            }

            foreach (var dynamicSceneObject in scene.dynamicSceneObjects)
            {
                if (dynamicSceneObject.animationClip == null)
                    continue;

                foreach (var animationClipCurve in dynamicSceneObject.animationClip.curves)
                {
                    if (animationClipCurve.animationCurve == null)
                        continue;

                    if (animationClipCurve.animationCurve.Length == 0)
                    {
                        animationClipCurve.animationCurve = null;
                    }
                }
            }
        }


        // Helper function that converts arrays of 
        public static T[] GetGfzValues<T>(IGfzConvertable<T>[] unity)
        {
            var gfz = new T[unity.Length];
            for (int i = 0; i < unity.Length; i++)
                gfz[i] = unity[i].ExportGfz();

            return gfz;
        }


    }
}
