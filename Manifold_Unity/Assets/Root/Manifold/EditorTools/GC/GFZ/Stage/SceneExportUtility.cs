using GameCube.GFZ;
using GameCube.GFZ.Stage;
using Manifold.IO;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Unity.Mathematics;
using System.Linq;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public static class SceneExportUtility
    {

        [MenuItem(Const.Menu.Manifold + "Scene Generation/Export (Active Scene)")]
        public static void ExportSceneActive()
        {
            var format = SerializeFormat.GX;
            ExportScene(format, true, true);
        }

        public static void ExportScene(SerializeFormat format, bool verbose, bool findInactive)
        {
            var settings = GfzProjectWindow.GetSettings();
            var outputPath = settings.SceneExportPath;
            var activeScene = EditorSceneManager.GetActiveScene();


            // Get scene parameters for general info
            var gfzSceneParameters = GameObject.FindObjectsOfType<GfzSceneParameters>();
            if (gfzSceneParameters.Length == 0)
            {
                var errorMsg = $"No {nameof(GfzSceneParameters)} found in scene! There must be one per scene.";
                throw new ArgumentException(errorMsg);
            }
            else if (gfzSceneParameters.Length > 1)
            {
                var errorMsg = $"More than one {nameof(GfzSceneParameters)} found in scene! There can only be one per scene.";
                throw new ArgumentException(errorMsg);
            }
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
                case SerializeFormat.AX:
                    compressFormat = LibGxFormat.AvGame.FZeroAX;
                    break;

                case SerializeFormat.GX:
                    compressFormat = LibGxFormat.AvGame.FZeroGX;
                    break;

                case SerializeFormat.InvalidFormat:
                    throw new ArgumentException("No format specified for serialization!");

                default:
                    throw new NotImplementedException();
            }

            var mirroredObjects = GameObject.FindObjectsOfType<GfzMirroredObject>();
            foreach (var mirroredObject in mirroredObjects)
                mirroredObject.MirrorTransform();

            //// TEST
            //// Load the old stage to use it's data I don't know how to generate yet
            ////var oldScene = ColiCourseIO.LoadScene(settings.StageDir + scene.FileName);

            //// TEST
            //// load in real, modify what you'd like
            //var scene = ColiCourseIO.LoadScene(settings.StageDir + sceneParams.GetGfzInternalName());
            //// Serialization settings
            //scene.Format = format;
            //scene.SerializeVerbose = verbose;
            //// Exported filename 'COLI_COURSE##'
            ////FileName = sceneParams.GetGfzInternalName();
            //// Data about the creator
            //scene.Author = sceneParams.author;
            //scene.Venue = sceneParams.venue;
            //scene.CourseName = sceneParams.courseName;

            // Build a new scene!
            var scene = new Scene()
            {
                // Serialization settings
                Format = format,
                SerializeVerbose = verbose,
                // Exported filename 'COLI_COURSE##'
                FileName = sceneParams.GetGfzInternalName(),
                // Data about the creator
                Author = sceneParams.author,
                Venue = sceneParams.venue,
                CourseName = sceneParams.courseName,
            };


            // Get scene-wide parameters from SceneParameters
            {
                // Construct range from 2 parameters
                scene.unkRange0x00 = new ViewRange(sceneParams.rangeNear, sceneParams.rangeFar);
                // Use functions to get form parameters
                scene.fog = sceneParams.ToGfzFog();
                scene.fogCurves = sceneParams.ToGfzFogCurves();
            }

            // Triggers
            {
                var arcadeCheckpointTriggers = GameObject.FindObjectsOfType<GfzTimeExtensionTrigger>(findInactive);
                scene.timeExtensionTriggers = GetGfzValues(arcadeCheckpointTriggers);

                // This trigger type is a mess... Get all 3 representations, combine, assign.
                // Collect all trigger types. They all get converted to the same GFZ base type.
                var objectPaths = GameObject.FindObjectsOfType<GfzObjectPath>(findInactive);
                var storyCapsules = GameObject.FindObjectsOfType<GfzStoryCapsule>(findInactive);
                var unknownMetadataTriggers = GameObject.FindObjectsOfType<GfzUnknownMiscellaneousTrigger>(findInactive);
                // Make a list, add range for each type
                var courseMetadataTriggers = new List<MiscellaneousTrigger>();
                courseMetadataTriggers.AddRange(GetGfzValues(objectPaths));
                courseMetadataTriggers.AddRange(GetGfzValues(storyCapsules));
                courseMetadataTriggers.AddRange(GetGfzValues(unknownMetadataTriggers));
                // Convert list to array, assign to ColiScene
                scene.miscellaneousTriggers = courseMetadataTriggers.ToArray();

                // This trigger type is a mess... Get all 3 representations, combine, assign.
                var unknownTriggers = GameObject.FindObjectsOfType<GfzCullOverrideTrigger>(findInactive);
                scene.cullOverrideTriggers = GetGfzValues(unknownTriggers);

                // 
                var visualEffectTriggers = GameObject.FindObjectsOfType<GfzVisualEffectTrigger>(findInactive);
                scene.visualEffectTriggers = GetGfzValues(visualEffectTriggers);

                // TODO:
                var storyObjectTrigger = GameObject.FindObjectsOfType<GfzStoryObjectTrigger>(findInactive);
                scene.storyObjectTriggers = GetGfzValues(storyObjectTrigger);
            }

            // Scene Objects
            {
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
                    sceneObjectLODs.AddRange(sceneObject.LODs);
                }
                scene.sceneObjectLODs = sceneObjectLODs.ToArray();

                // CString names
                // TODO: share references
                var sceneObjectNames = new List<ShiftJisCString>();
                sceneObjectNames.Add("");
                foreach (var thing in scene.sceneObjectLODs)
                {
                    //if (!sceneObjectNames.Contains(thing.name))
                    //{
                    sceneObjectNames.Add(thing.Name);
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
                scene.staticColliderMeshManager = new StaticColliderMeshManager(format);
                // Bind to other references
                scene.staticColliderMeshManager.UnknownColliders = scene.unknownColliders;
                scene.staticColliderMeshManager.StaticSceneObjects = scene.staticSceneObjects;
                //scene.staticColliderMeshes.ComputeMatrixBoundsXZ();
                scene.staticColliderMeshManager.MeshGridXZ = new GridXZ();

                // Get data from scene
                //scene.staticColliderMeshes = oldScene.staticColliderMeshes;
                scene.staticColliderMeshManager.SerializeFormat = format;
                // Point to existing references
                scene.staticColliderMeshManager.UnknownColliders = scene.unknownColliders;
                scene.staticColliderMeshManager.StaticSceneObjects = scene.staticSceneObjects;
            }

            // TRACK
            {
                var track = GameObject.FindObjectOfType<GfzTrack>();
                track.InitTrackData();

                scene.rootTrackSegments = track.RootSegments;
                scene.allTrackSegments = track.AllSegments;

                // Nodes (checkpoints-segment bound together)
                scene.trackNodes = track.TrackNodes;
                // Checkpoint matrix
                scene.trackCheckpointGrid = track.TrackCheckpointMatrix;
                scene.checkpointGridXZ = track.TrackCheckpointMatrixBoundsXZ;

                // Track metadata
                scene.trackLength = track.TrackLength;
                scene.trackMinHeight = track.TrackMinHeight;

                // AI data
                // 2022/01/25: currently save out only the terminating element.
                scene.embeddedPropertyAreas = track.EmbeddedPropertyAreas;

                scene.circuitType = track.CircuitType;
            }

            // TEMP until data is stored properly in GFZ unity components
            MakeTrueNulls(scene);

            // Save out file and LZ'ed file
            var outputFile = outputPath + scene.FileName;
            using (var writer = new BinaryWriter(File.Create(outputFile)))
            {
                writer.WriteX(scene);
                writer.Flush();
            }
            LzUtility.CompressAvLzToDisk(outputFile, compressFormat, true);
            OSUtility.OpenDirectory(outputPath);

            foreach (var mirroredObject in mirroredObjects)
                mirroredObject.MirrorTransform();
        }


        public static void MakeTrueNulls(Scene scene)
        {
            // TEMP
            // This is because I must handle Unity serializing nulls with empty instances
            foreach (var sceneObject in scene.sceneObjects)
            {
                var colliderGeo = sceneObject.ColliderMesh;
                if (colliderGeo != null)
                {
                    if (colliderGeo.Tris != null)
                        if (colliderGeo.Tris.Length == 0)
                            colliderGeo.Tris = null;

                    if (colliderGeo.Quads != null)
                        if (colliderGeo.Quads.Length == 0)
                            colliderGeo.Quads = null;
                }
            }

            foreach (var dynamicSceneObject in scene.dynamicSceneObjects)
            {
                if (dynamicSceneObject.AnimationClip == null)
                    continue;

                foreach (var animationClipCurve in dynamicSceneObject.AnimationClip.Curves)
                {
                    if (animationClipCurve.AnimationCurve == null)
                        continue;

                    if (animationClipCurve.AnimationCurve.Length == 0)
                    {
                        animationClipCurve.AnimationCurve = null;
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
