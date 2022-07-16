using GameCube.GFZ;
using GameCube.GFZ.GMA;
using GameCube.GFZ.LZ;
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

using Manifold.EditorTools.GC.GFZ.Stage.Track;


namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public static class SceneExportUtility
    {

        [MenuItem(GfzMenuItems.Stage.ExportActiveScene + " _F8", priority = GfzMenuItems.Stage.ExportActiveScenePriority)]
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
            var sceneParams = GetGfzSceneParameters();

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
            GameCube.AmusementVision.GxGame compressFormat;
            switch (format)
            {
                case SerializeFormat.AX:
                    compressFormat = GameCube.AmusementVision.GxGame.FZeroAX;
                    break;

                case SerializeFormat.GX:
                    compressFormat = GameCube.AmusementVision.GxGame.FZeroGX;
                    break;

                default:
                    throw new ArgumentException($"Invalid format '{format}' specified for serialization.");
            }

            // If objects have been mirrored, mirror again before export
            var mirroredObjects = GameObject.FindObjectsOfType<GfzMirroredObject>();
            foreach (var mirroredObject in mirroredObjects)
                mirroredObject.MirrorTransform();

            //// TEST
            //// Load the old stage to use it's data I don't know how to generate yet
            ////var oldScene = ColiCourseIO.LoadScene(settings.StageDir + scene.FileName);

            // TEST

            // Build a new scene!
            var scene = new Scene()
            {
                // Serialization settings
                Format = format,
                SerializeVerbose = verbose,
                // Exported filename 'COLI_COURSE##'
                CourseIndex = checked((byte)sceneParams.courseIndex),
                FileName = sceneParams.GetGfzInternalName(),
                // Data about the creator
                Author = sceneParams.author,
                Venue = sceneParams.venue,
                CourseName = sceneParams.courseName,
            };


            // Get scene-wide parameters from SceneParameters
            {
                // Construct range from 2 parameters
                scene.UnkRange0x00 = new ViewRange(sceneParams.rangeNear, sceneParams.rangeFar);
                // Use functions to get fog parameters
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
                var gfzDynamicSceneObjects = GameObject.FindObjectsOfType<GfzSceneObjectDynamic>(false).Reverse().ToArray();
                var gfzStaticSceneObjects = GameObject.FindObjectsOfType<GfzSceneObjectStatic>(false).Reverse().ToArray();
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
                foreach (var sceneObject in scene.sceneObjects)
                {
                    scene.SceneObjectLODs.AddRange(sceneObject.LODs);
                }

                // CString names
                // TODO: share references
                var sceneObjectNames = new List<ShiftJisCString>();
                sceneObjectNames.Add("");
                foreach (var thing in scene.SceneObjectLODs)
                {
                    scene.SceneObjectNames.Add(thing.Name);
                }
                // alphabetize, store
                scene.SceneObjectNames = scene.SceneObjectNames.OrderBy(x => x.Value).ToList();
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
                scene.staticColliderMeshManager.StaticSceneObjects = scene.staticSceneObjects is null ? new SceneObjectStatic[0] : scene.staticSceneObjects;
            }

            // TRACK
            {
                var track = GameObject.FindObjectOfType<GfzTrack>();
                track.InitTrackData();

                scene.RootTrackSegments = track.RootSegments;
                scene.AllTrackSegments = track.AllSegments;

                // Nodes (checkpoints-segment bound together)
                scene.trackNodes = track.TrackNodes;
                // Checkpoint matrix
                scene.trackCheckpointGrid = track.TrackCheckpointMatrix;
                scene.CheckpointGridXZ = track.TrackCheckpointMatrixBoundsXZ;

                // Track metadata
                scene.trackLength = track.TrackLength;
                scene.trackMinHeight = track.TrackMinHeight;

                // AI data
                // 2022/01/25: currently save out only the terminating element.
                scene.embeddedPropertyAreas = track.EmbeddedPropertyAreas;

                scene.CircuitType = track.CircuitType;
            }
            // Inject TRACK models
            {
                var trackModelsGma = CreateTrackModelsGma("Track Segment");
                var sceneObjects = CreateSceneObjectsFromGma(trackModelsGma);

                // Add SceneObject (template)< it's LODs, and name
                foreach (var sceneObject in sceneObjects)
                {
                    scene.SceneObjectLODs.AddRange(sceneObject.LODs);
                    scene.SceneObjectNames.Add(sceneObject.Name);
                }
                scene.sceneObjects = sceneObjects.Concat(scene.sceneObjects).ToArray();

                // add static / dynamic
                var dynamicSceneObjects = GetAsSceneObjectDynamic(sceneObjects);
                scene.dynamicSceneObjects = dynamicSceneObjects.Concat(scene.dynamicSceneObjects).ToArray();

                // save gma
                var gmaFileName = outputPath + $"st{scene.CourseIndex:00}.gma";
                using (var writer = new EndianBinaryWriter(File.Create(gmaFileName), Gma.endianness))
                    writer.Write(trackModelsGma);
                LzUtility.CompressAvLzToDisk(gmaFileName, compressFormat, true);
                Debug.Log($"Created models archive '{gmaFileName}'.");
            }

            // TEMP until data is stored properly in GFZ unity components
            MakeTrueNulls(scene);

            // Save out file and LZ'ed file
            var outputFile = outputPath + scene.FileName;
            using (var writer = new EndianBinaryWriter(File.Create(outputFile), Scene.endianness))
            {
                writer.Write(scene);
                writer.Flush();
            }
            LzUtility.CompressAvLzToDisk(outputFile, compressFormat, true);
            OSUtility.OpenDirectory(outputPath);
            Debug.Log($"Created course '{outputFile}'.");

            // Undo mirroring
            foreach (var mirroredObject in mirroredObjects)
                mirroredObject.MirrorTransform();

            // LOG
            using (var writer = new StreamWriter(File.Create(outputFile + ".txt")))
            {
                var builder = new System.Text.StringBuilder();
                scene.PrintMultiLine(builder);
                writer.Write(builder.ToString());
            }
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

        public static GfzSceneParameters GetGfzSceneParameters()
        {
            // Get parameters - ensure there is only 1 in scene!
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

            return sceneParams;
        }

        public static SceneObject[] CreateSceneObjectsFromGma(Gma gma)
        {
            var sceneObjects = new List<SceneObject>();

            foreach (var model in gma.Models)
            {
                // Only 1 LOD for now
                var lod = new SceneObjectLOD()
                {
                    Name = model.Name,
                    LodDistance = 0f,
                };

                // Wrap it up
                var sceneObject = new SceneObject()
                {
                    LodRenderFlags = 0,
                    LODs = new SceneObjectLOD[] { lod },
                    ColliderMesh = null,
                };

                // Add to list
                sceneObjects.Add(sceneObject);
            }

            return sceneObjects.ToArray();
        }

        // TODO: move to scene object dynamic
        public static SceneObjectDynamic[] GetAsSceneObjectDynamic(SceneObject[] sceneObjects)
        {
            var dynamics = new SceneObjectDynamic[sceneObjects.Length];
            for (int i = 0; i < dynamics.Length; i++)
            {
                dynamics[i] = new SceneObjectDynamic()
                {
                    Unk0x00 = 0xF,
                    Unk0x04 = unchecked((int)0xFFFFFFFF),
                    SceneObject = sceneObjects[i],
                    TransformMatrix3x4 = new(),
                };
            }
            return dynamics;
        }

        public static Gma CreateTrackModelsGma(string modelName)
        {
            // TODO: get GfzTrack, use it to get children
            var track = GameObject.FindObjectOfType<GfzTrack>(false);
            track.FindChildSegments();

            int debugIndex = 0;
            var models = new List<Model>();
            foreach (var rootTrackSegmentNode in track.AllRoots)
            {
                var shapeNodes = rootTrackSegmentNode.GetShapeNodes();
                foreach (var shape in shapeNodes)
                {
                    var gcmf = shape.CreateGcmf();
                    models.Add(new Model($"{modelName} {debugIndex++}", gcmf));
                }
            }

            // Create single GMA for model, comprised on many GCMFs (display lists and materials)
            var gma = new Gma();
            gma.Models = models.ToArray();

            return gma;
        }
    }
}
