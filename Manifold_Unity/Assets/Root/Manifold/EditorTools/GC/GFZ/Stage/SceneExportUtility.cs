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

using Manifold.EditorTools.GC.GFZ.GMA;
using Manifold.EditorTools.GC.GFZ.Stage.Track;


namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public static class SceneExportUtility
    {

        [MenuItem(GfzMenuItems.Stage.ExportActiveScene + " _F8", priority = GfzMenuItems.Stage.ExportActiveScenePriority)]
        public static void ExportSceneActive()
        {
            var format = SerializeFormat.GX;
            ExportScene(format, true);
        }

        public static void ExportScene(SerializeFormat format, bool verbose)
        {
            var settings = GfzProjectWindow.GetSettings();
            var outputPath = settings.SceneExportPath;
            var activeScene = EditorSceneManager.GetActiveScene();

            // Get scene parameters for general info
            var sceneParams = GetGfzSceneParameters();

            // Before we do the work of exporting, see if the stage we are exporting (index/venue) align correctly.
            // If we export as Lightning but the index is, say, 1, index 1 belongs to Mute City. Warn of potential issues.
            bool isValidIndexForVenue = CourseUtility.GetVenue(sceneParams.courseIndex) == sceneParams.venue;
            if (!isValidIndexForVenue)
            {
                var title = "Export Scene: Venue/Index Mismatch";
                var msg =
                    $"The assigned venue '{sceneParams.venue}' and the stage index being used '{sceneParams.courseIndex}' " +
                    $"do not share the same venue. When loaded in-game, models will not load.";

                bool doExport = EditorUtility.DisplayDialog(title, msg, "Export Anyway");
                if (!doExport)
                {
                    return;
                }
            }

            // TODO: this should be in ColiScene
            GameCube.AmusementVision.GxGame compressFormat;
            {
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
            }

            // If objects have been mirrored, mirror again before export
            var mirroredObjects = GameObject.FindObjectsOfType<GfzMirroredObject>();
            foreach (var mirroredObject in mirroredObjects)
                mirroredObject.MirrorTransform();

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
                var arcadeCheckpointTriggers = GameObject.FindObjectsOfType<GfzTimeExtensionTrigger>();
                scene.timeExtensionTriggers = GetGfzValues(arcadeCheckpointTriggers);

                // This trigger type is a mess... Get all 3 representations, combine, assign.
                // Collect all trigger types. They all get converted to the same GFZ base type.
                var objectPaths = GameObject.FindObjectsOfType<GfzObjectPath>();
                var storyCapsules = GameObject.FindObjectsOfType<GfzStoryCapsule>();
                var unknownMetadataTriggers = GameObject.FindObjectsOfType<GfzUnknownMiscellaneousTrigger>();
                // Make a list, add range for each type
                var courseMetadataTriggers = new List<MiscellaneousTrigger>();
                courseMetadataTriggers.AddRange(GetGfzValues(objectPaths));
                courseMetadataTriggers.AddRange(GetGfzValues(storyCapsules));
                courseMetadataTriggers.AddRange(GetGfzValues(unknownMetadataTriggers));
                // Convert list to array, assign to ColiScene
                scene.miscellaneousTriggers = courseMetadataTriggers.ToArray();

                // This trigger type is a mess... Get all 3 representations, combine, assign.
                var unknownTriggers = GameObject.FindObjectsOfType<GfzCullOverrideTrigger>();
                scene.cullOverrideTriggers = GetGfzValues(unknownTriggers);

                // 
                var visualEffectTriggers = GameObject.FindObjectsOfType<GfzVisualEffectTrigger>();
                scene.visualEffectTriggers = GetGfzValues(visualEffectTriggers);

                // TODO:
                var storyObjectTrigger = GameObject.FindObjectsOfType<GfzStoryObjectTrigger>();
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
                scene.unknownColliders = new UnknownCollider[0];

                // Static Collider Matrix
                scene.staticColliderMeshManager = new StaticColliderMeshManager(format);
                // Bind to other references
                scene.staticColliderMeshManager.UnknownColliders = scene.unknownColliders;
                scene.staticColliderMeshManager.StaticSceneObjects = scene.staticSceneObjects;

                // Get data from scene
                //scene.staticColliderMeshes = oldScene.staticColliderMeshes;
                scene.staticColliderMeshManager.SerializeFormat = format;
                // Point to existing references
                scene.staticColliderMeshManager.UnknownColliders = scene.unknownColliders;
                scene.staticColliderMeshManager.StaticSceneObjects = scene.staticSceneObjects is null ? new SceneObjectStatic[0] : scene.staticSceneObjects;

                // Build tri/quads for static collider mesh
                var staticColliders = GameObject.FindObjectsOfType<GfzStaticColliderMesh2>(false);
                scene.staticColliderMeshManager.ColliderTris = GetColliderTriangles(staticColliders);
                scene.staticColliderMeshManager.ComputeMeshGridXZ();
                scene.staticColliderMeshManager.TriMeshGrids[3] = GetIndexListsAll(scene.staticColliderMeshManager); // 3 == dash
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
                // 2022/01/25: currently save out only the terminating element.
                scene.embeddedPropertyAreas = track.EmbeddedPropertyAreas;

                scene.CircuitType = track.CircuitType;
            }
            // Inject TRACK models, + recover missing models
            {
                // This creates the GMA archive and gives us the scene objects and dynamic scene objects for them
                var gma = CreateTrackModelsGma(out SceneObject[] sceneObjects, out SceneObjectDynamic[] dynamicSceneObjects);

                // Add SceneObject's LODs and their name to archive
                foreach (var sceneObject in sceneObjects)
                {
                    scene.SceneObjectLODs.AddRange(sceneObject.LODs);
                    scene.SceneObjectNames.Add(sceneObject.Name); // wouldn't you need to put in all LOD names?
                }
                scene.sceneObjects = sceneObjects.Concat(scene.sceneObjects).ToArray();

                // add static / dynamic
                scene.dynamicSceneObjects = dynamicSceneObjects.Concat(scene.dynamicSceneObjects).ToArray();

                // save gma
                var gmaFileName = $"st{scene.CourseIndex:00}.gma";
                var gmaFilePath = outputPath + gmaFileName;

                // Recover models we might be overwriting
                var missingModels = RecoverMissingModelsFromStageGma(gmaFileName);
                gma.Models = gma.Models.Concat(missingModels).ToArray();

                using (var writer = new EndianBinaryWriter(File.Create(gmaFilePath), Gma.endianness))
                    writer.Write(gma);
                LzUtility.CompressAvLzToDisk(gmaFilePath, compressFormat, true);
                Debug.Log($"Created models archive '{gmaFilePath}'.");
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


        // Helper function that converts arrays of unity-edtor values into their gfz conterpart
        public static TGfzConvertable[] GetGfzValues<TGfzConvertable>(IGfzConvertable<TGfzConvertable>[] unity)
        {
            var gfz = new TGfzConvertable[unity.Length];
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

        public static SceneObject CreateSceneObject(string modelName)
        {
            // Only 1 LOD for now
            var lod = new SceneObjectLOD()
            {
                Name = modelName,
                LodDistance = 0f,
            };

            // Wrap it up
            var sceneObject = new SceneObject()
            {
                LodRenderFlags = 0,
                LODs = new SceneObjectLOD[] { lod },
                ColliderMesh = null,
            };

            return sceneObject;
        }
        public static SceneObjectDynamic CreateSceneObjectDynamic(SceneObject sceneObject, TransformMatrix3x4 transform)
        {
            var dynamicSceneObject = new SceneObjectDynamic()
            {
                Unk0x00 = ObjectRenderFlags0x00.renderObject,
                Unk0x04 = ObjectRenderFlags0x04._NULL,
                SceneObject = sceneObject,
                TransformMatrix3x4 = transform,
            };
            return dynamicSceneObject;
        }
        public static Gma CreateTrackModelsGma(out SceneObject[] sceneObjects, out SceneObjectDynamic[] dynamicSceneObjects)
        {
            // get GfzTrack, use it to get children
            var track = GameObject.FindObjectOfType<GfzTrack>(false);
            track.FindChildSegments();

            var models = new List<Model>();
            var _sceneObjects = new List<SceneObject>();
            var _dynamicSceneObjects = new List<SceneObjectDynamic>();

            int shapeIndex = 0;
            foreach (var rootTrackSegmentNode in track.AllRoots)
            {
                int subIndex = 0;
                var shapeNodes = rootTrackSegmentNode.GetShapeNodes();
                foreach (var shape in shapeNodes)
                {
                    var gcmf = shape.CreateGcmf();
                    var modelName = $"{shape.GetRoot().name}-{shape.name}-#{shapeIndex++}.{subIndex++}";
                    models.Add(new Model(modelName, gcmf));

                    var sceneObject = CreateSceneObject(modelName);
                    _sceneObjects.Add(sceneObject);

                    var transform = new TransformMatrix3x4();
                    var sceneObjectDynamic = CreateSceneObjectDynamic(sceneObject, transform);
                    _dynamicSceneObjects.Add(sceneObjectDynamic);
                }
            }

            // Create single GMA for model, comprised on many GCMFs (display lists and materials)
            var gma = new Gma();
            gma.Models = models.ToArray();

            sceneObjects = _sceneObjects.ToArray();
            dynamicSceneObjects = _dynamicSceneObjects.ToArray();

            return gma;
        }


        #region STATIC COLLIDER MESH MANAGER

        public static ColliderTriangle[] GetColliderTriangles(GfzStaticColliderMesh2[] staticColliders)
        {
            // for each script in scene
            //  get -> triangles, tri count (linear index order 0 to n), layer type
            // then
            //  build grid / cells
            // then
            //  recompute bounds of tri/quad
            //  add tri/quad to logical bounds
            //  fehking hell, also compute for large tri/quad if it crosses cells D:
            //   you could probably know if you need to do this based on cell vs tri/quad size

            int totalVertices = 0;
            var colliderTrianglesList = new List<ColliderTriangle[]>();

            // Get all triangles, count total
            foreach (var staticCollider in staticColliders)
            {
                var triangles = staticCollider.CreateColliderTriangles();
                colliderTrianglesList.Add(triangles);
                totalVertices += triangles.Length;
            };

            int baseOffset = 0;
            var allColliderTriangles = new ColliderTriangle[totalVertices];
            foreach (var collection in colliderTrianglesList)
            {
                collection.CopyTo(allColliderTriangles, baseOffset);
                baseOffset += collection.Length;
            }

            return allColliderTriangles;
        }

        public static StaticColliderMeshGrid GetIndexListsAll(StaticColliderMeshManager scmm)
        {
            var indexGrid = new StaticColliderMeshGrid();
            var indexLists = new IndexList[StaticColliderMeshGrid.kListCount];
            for (int i = 0; i < indexLists.Length; i++)
            {
                var indexList = new IndexList();
                indexList.Indexes = QuickIndexList(0, scmm.ColliderTris.Length);
                indexLists[i] = indexList;
            }
            indexGrid.IndexLists = indexLists;
            return indexGrid;
        }

        public static ushort[] QuickIndexList(int baseIndex, int count)
        {
            var indexes = new ushort[count];
            for (int i = 0; i < count; i++)
                indexes[i] = checked((ushort)(baseIndex + i));
            return indexes;
        }

        public static void AssignStaticColliderMeshManager(Scene scene)
        {
            // Build tri/quads for static collider mesh
            var staticColliders = GameObject.FindObjectsOfType<GfzStaticColliderMesh2>(false);
            scene.staticColliderMeshManager.ColliderTris = GetColliderTriangles(staticColliders);
            scene.staticColliderMeshManager.ComputeMeshGridXZ();
            scene.staticColliderMeshManager.TriMeshGrids[3] = GetIndexListsAll(scene.staticColliderMeshManager); // 3 == dash
        }

        #endregion


        public static Model[] RecoverMissingModelsFromStageGma(string missingFile)
        {
            string missingFileName = Path.GetFileNameWithoutExtension(missingFile);
            var tags = GameObject.FindObjectsOfType<GmaSourceTag>();
            var dictMissingModels = new Dictionary<string, GmaSourceTag>();

            foreach (var tag in tags)
            {
                bool isMissingModelReference = tag.FileName == missingFileName;
                if (!isMissingModelReference)
                    continue;

                if (!dictMissingModels.ContainsKey(tag.ModelName))
                    dictMissingModels.Add(tag.ModelName, tag);
            }

            var models = new Model[dictMissingModels.Count];
            var settings = GfzProjectWindow.GetSettings();
            string inputPath = settings.SourceStageDirectory;
            string filePath = inputPath + missingFile;

            using (var reader = new EndianBinaryReader(File.OpenRead(filePath), Gma.endianness))
            {
                int i = 0;
                foreach (var kvMissingModel in dictMissingModels)
                {
                    var missingModel = kvMissingModel.Value;
                    reader.JumpToAddress(missingModel.GcmfAddressRange.startAddress);

                    var model = new Model();
                    model.Name = missingModel.ModelName;
                    model.Gcmf = new Gcmf();
                    model.Gcmf.Deserialize(reader);
                    models[i++] = model;
                }
            }

            return models;
        }
    }
}
