using GameCube.GFZ;
using GameCube.GFZ.GMA;
using GameCube.GFZ.LZ;
using GameCube.GFZ.Stage;
using GameCube.GFZ.TPL;
using GameCube.GFZ.REL;
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
using Manifold.EditorTools.GC.GFZ.TPL;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public static class SceneExportUtility
    {
        private static T FindSingleInstanceOrError<T>() where T : UnityEngine.Object
        {
            var instances = GameObject.FindObjectsOfType<T>();
            int count = instances.Length;
            if (count == 0)
            {
                // User message
                string title = $"Missing Instance {typeof(T).Name}";
                string message = $"Expected 1 instance of script {typeof(T).Name} in scene but found {count} instances.";
                string ok = "ok";
                EditorUtility.DisplayDialog(title, message, ok);

                // Error
                throw new ArgumentException($"No instances of type {typeof(T).Name} found in scene!");
            }
            else if (count > 1)
            {
                // User message
                string title = $"Too Many Instances of {typeof(T).Name}";
                string message = $"Expected 1 instance of script {typeof(T).Name} in scene but found {count} instances.";
                string ok = "ok";
                EditorUtility.DisplayDialog(title, message, ok);

                // Error
                throw new ArgumentException($"More than one instance of type {typeof(T).Name} found in scene! There must only be 1!");
            }
            else
            {
                return instances[0];
            }
        }


        //[MenuItem(GfzMenuItems.Stage.TestPatchEnemyLine + " _F7", priority = GfzMenuItems.Stage.Priority.TestPatchEnemyLine)]
        public static void PatchEnemyLineFromSceneParameters()
        {
            var enemyLineInfo = GetEnemyLineInfo();
            var settings = GfzProjectWindow.GetSettings();
            string workingFile = Path.Combine(settings.WorkingFilesDirectory, enemyLineInfo.WorkingFile);
            string destination = Path.Combine(settings.WorkingFilesDirectory, enemyLineInfo.SourceFile);

            // Ensure file integrity
            switch (enemyLineInfo.GameCode)
            {
                case EnemyLineInformation.GameCode.GX_E:
                case EnemyLineInformation.GameCode.GX_J:
                case EnemyLineInformation.GameCode.GX_P:
                    {
                        // Decrypt file if it does not exist
                        bool fileExists = File.Exists(workingFile);
                        if (!fileExists)
                            DecryptEnemyLine();
                    }
                    break;

                case EnemyLineInformation.GameCode.AX:
                    {
                        bool fileExists = File.Exists(workingFile);
                        if (!fileExists)
                            throw new ArgumentException($"Could not find file {workingFile}");
                    }
                    break;

                default:
                    throw new NotImplementedException($"Unimplemented game code {enemyLineInfo.GameCode}");
            }


            // Patch things!
            var sceneParameters = FindSingleInstanceOrError<GfzSceneParameters>();
            using (var file = File.Open(workingFile, FileMode.Open))
            {
                // Patch file
                using (var writer = new EndianBinaryWriter(file, enemyLineInfo.Endianness))
                {
                    // do the things!
                    EnemyLineUtility.PatchCupSlot(writer, enemyLineInfo, sceneParameters.cup, sceneParameters.courseIndex);
                    EnemyLineUtility.PatchVenueIndex(writer, enemyLineInfo, sceneParameters.courseIndex, sceneParameters._venue);
                    EnemyLineUtility.PatchDifficultyRatingToSlot(writer, enemyLineInfo, sceneParameters.courseIndex, sceneParameters.difficulty);
                    // TODO: initialize course names, then call will be valid.
                    //EnemyLineUtility.PatchCustomCourseName(writer, enemyLineInfo, sceneParameters.courseIndex, sceneParameters.courseName);
                    // TODO: bgm

                    Debug.Log($"Success! Openned file {workingFile}");
                }

                // Compress patched file
                var game = enemyLineInfo.GameCode == EnemyLineInformation.GameCode.AX
                    ? GameCube.AmusementVision.GxGame.FZeroAX
                    : GameCube.AmusementVision.GxGame.FZeroGX;
                var compressed = LzUtility.CompressAvLz(workingFile, game);
                // Encrypt patched file
                var encrypted = EnemyLineUtility.Encrypt(compressed, enemyLineInfo);
                // Save to disk
                using (var writer = new BinaryWriter(File.Create(destination)))
                {
                    writer.Write(encrypted.ToArray());
                    Debug.Log($"Wrote file {destination}");
                }

            }
        }

        [MenuItem(GfzMenuItems.Stage.TestDecryptEnemyLine, priority = GfzMenuItems.Stage.Priority.TestDecryptEnemyLine)]
        public static void DecryptEnemyLine()
        {
            var enemyLineInfo = GetEnemyLineInfo();
            var settings = GfzProjectWindow.GetSettings();
            string encryptedFile = Path.Combine(settings.SourceDirectory, enemyLineInfo.SourceFile);

            switch (enemyLineInfo.GameCode)
            {
                case EnemyLineInformation.GameCode.GX_E:
                case EnemyLineInformation.GameCode.GX_J:
                case EnemyLineInformation.GameCode.GX_P:
                    {
                        // Decrypt file
                        FileStream line = File.Open(encryptedFile, FileMode.Open);
                        var decryptedLine = EnemyLineUtility.Decrypt(line, enemyLineInfo);
                        line.Close();

                        // Save decrypted file (with .lz extension)
                        string decryptedAndDecompressedFilePath = settings.WorkingFilesDirectory + "enemy_line/line__.rel";
                        string decryptedFilePath = decryptedAndDecompressedFilePath + ".lz";
                        using (var writer = new BinaryWriter(File.Create(decryptedFilePath, encryptedFile.Length)))
                        {
                            writer.Write(decryptedLine.ToArray());
                            Debug.Log($"Decrypted '{encryptedFile}' and wrote file '{decryptedFilePath}'");
                        }

                        // Decompress file
                        var decompressedFile = new MemoryStream();
                        using (var inputFile = File.Open(decryptedFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            GameCube.AmusementVision.LZ.Lz.Unpack(inputFile, decompressedFile);
                        }

                        // Save out decrypted AND decompressed file
                        using (var writer = new BinaryWriter(File.Create(decryptedAndDecompressedFilePath, (int)decompressedFile.Length)))
                        {
                            writer.Write(decompressedFile.ToArray());
                            Debug.Log($"Decompressed '{decryptedFilePath}' and wrote file '{decryptedAndDecompressedFilePath}'");
                            decompressedFile.Flush();
                        }
                    }
                    break;

                case EnemyLineInformation.GameCode.AX:
                    // Nothing to decrypt
                    break;

                default:
                    throw new NotImplementedException($"Unimplemented game code {enemyLineInfo.GameCode}");
            }
        }

        /// <summary>
        /// Automatically gets the correct EnemyLineInfo for the current project's 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static EnemyLineInformationLookup GetEnemyLineInfo()
        {
            // Search for GX line__.bin file
            var settings = GfzProjectWindow.GetSettings();
            string encryptedFile = settings.SourceDirectory + "enemy_line/line__.bin";

            bool fileExists = File.Exists(encryptedFile);
            if (fileExists)
            {
                // Check for GX line__.bin's hash
                var md5 = System.Security.Cryptography.MD5.Create();
                string fileHash = "";
                using (var reader = new BinaryReader(File.OpenRead(encryptedFile)))
                {
                    var bytes = reader.ReadBytes((int)reader.BaseStream.Length);
                    var hash = md5.ComputeHash(bytes);
                    foreach (var @byte in hash)
                        fileHash += @byte.ToString("x2");
                }

                switch (fileHash)
                {
                    case EnemyLineInformationLookupGfze01.kFileHashMD5: return EnemyLineInformation.GFZE01;
                    case EnemyLineInformationLookupGfzj01.kFileHashMD5: return EnemyLineInformation.GFZJ01;
                    case EnemyLineInformationLookupGfzp01.kFileHashMD5: return EnemyLineInformation.GFZP01;
                    default:
                        throw new ArgumentException($"File '{encryptedFile}' found but hash '{fileHash}' does not match.");
                }
            }
            else
            {
                // check for AX file
                // TODO: load main.dol, check hash?
            }

            throw new ArgumentException("Could not locate valid file for 'enemy_line' patching.");
        }




        [MenuItem(GfzMenuItems.Stage.ExportActiveScene + " _F8", priority = GfzMenuItems.Stage.Priority.ExportActiveScene)]
        public static void ExportSceneActive()
        {
            ExportScene(true);
        }

        public static void ExportScene(bool verbose)
        {
            var settings = GfzProjectWindow.GetSettings();
            var format = settings.SerializeFormat;
            var outputPath = settings.SceneExportPath;
            var activeScene = EditorSceneManager.GetActiveScene();

            //
            bool noWorkingFilesPathSpecified = string.IsNullOrEmpty(settings.WorkingFilesDirectory);
            if (noWorkingFilesPathSpecified)
                throw new ArgumentException($"Cannot export. No {nameof(settings.WorkingFilesDirectory)} specifided in settings.");

            // Get scene parameters for general info
            var sceneParams = GetGfzSceneParameters();

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
            var compressFormat = GetCompressFormat(scene.Format);
            var tplTextureContainer = new TplTextureContainer();
            var modelList = new List<Model>();
            var gfzSceneObjectsList = new List<GfzSceneObject>();
            var sceneObjectsList = new List<SceneObject>();
            var sceneObjectDynamicsList = new List<SceneObjectDynamic>();

            // Get scene-wide parameters from SceneParameters
            {
                // Construct range from 2 parameters
                scene.UnkRange0x00 = new ViewRange(sceneParams.rangeNear, sceneParams.rangeFar);
            }

            // FOG
            {
                var gfzFogs = GameObject.FindObjectsOfType<GfzFog>();
                bool hasAtLeastOneFogParameter = gfzFogs.Length > 0;
                if (hasAtLeastOneFogParameter)
                {
                    if (gfzFogs.Length > 1)
                    {
                        // TODO: SOME WARNING
                    }

                    // Select the first (hopefully only)
                    var gfzFog = gfzFogs[0];

                    // Use functions to get fog parameters
                    scene.fog = gfzFog.ToGfzFog();
                    scene.fogCurves = gfzFog.ToGfzFogCurves();
                }
                else
                {
                    scene.fog = new Fog();
                }
            }

            // Triggers
            {
                var timeExtensionTriggers = GameObject.FindObjectsOfType<GfzTimeExtensionTrigger>();
                scene.timeExtensionTriggers = GetGfzValues(timeExtensionTriggers);

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

                // Get dynamic scene objects
                var dynamicSceneObjects = GetGfzValues(gfzDynamicSceneObjects);
                sceneObjectDynamicsList.AddRange(dynamicSceneObjects);
                // Get their scene object reference
                foreach (var gfzDynamicSceneObject in gfzDynamicSceneObjects)
                {
                    var gfzSceneObject = gfzDynamicSceneObject.GfzSceneObject;
                    if (gfzSceneObject == null)
                    {
                        throw new Exception($"SceneObject reference is null! {gfzDynamicSceneObject.name}");
                    }

                    // Only add items once
                    bool containsSceneObject = gfzSceneObjectsList.Contains(gfzSceneObject);
                    if (!containsSceneObject)
                    {
                        var sceneObject = gfzSceneObject.ExportGfz();
                        sceneObjectsList.Add(sceneObject);
                        gfzSceneObjectsList.Add(gfzSceneObject);
                    }
                }

                // Collect and insert models wanted into the gma.
                var missingModels = RecoverMissingModelsFromStageGma(scene.CourseIndex, tplTextureContainer);
                modelList.AddRange(missingModels);
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
                scene.staticColliderMeshManager.ColliderTris = GetColliderTriangles(format, out ushort[][] layerIndexes);
                scene.staticColliderMeshManager.ComputeMeshGridXZ();
                for (int i = 0; i < layerIndexes.Length; i++)
                {
                    var indexes = layerIndexes[i];
                    scene.staticColliderMeshManager.TriMeshGrids[i] = GetIndexListsAll(indexes);
                }
                // TODO: function in scmm which assigns triangles to cells automatically
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

            // Add temp models... in the future, have more proper pipeline with generic base class
            {
                var debugMeshes = GameObject.FindObjectsOfType<GfzDebugMesh>();
                int index = 0;
                foreach (var debugMesh in debugMeshes)
                {
                    string name = $"temp#{index++}_{debugMesh.name}";

                    var sceneObject = CreateSceneObject(name);
                    var sceneObjectDynamic = CreateSceneObjectDynamic();
                    sceneObjectDynamic.SceneObject = sceneObject;
                    sceneObjectDynamic.TransformMatrix3x4 = new();

                    sceneObjectsList.Add(sceneObject);
                    sceneObjectDynamicsList.Add(sceneObjectDynamic);

                    var gcmf = debugMesh.CreateGcmf(tplTextureContainer);
                    var model = new Model()
                    {
                        Name = name,
                        Gcmf = gcmf,
                    };
                    modelList.Add(model);
                }
            }

            // Inject TRACK models, + recover missing models
            // This creates the GMA archive and gives us the scene objects and dynamic scene objects for them
            // It will also add which texture hashes need to be in TPL
            var trackModels = CreateTrackModels(tplTextureContainer, out SceneObject[] trackSceneObjects, out SceneObjectDynamic[] trackDynamicSceneObjects);
            modelList.AddRange(trackModels);
            sceneObjectsList.AddRange(trackSceneObjects);
            sceneObjectDynamicsList.AddRange(trackDynamicSceneObjects);

            // Make GMA
            var gma = new Gma();
            gma.Models = modelList.ToArray();

            // Finalize scene objects
            scene.sceneObjects = sceneObjectsList.ToArray();
            scene.dynamicSceneObjects = sceneObjectDynamicsList.ToArray();
            foreach (var sceneObject in scene.sceneObjects)
            {
                // LODs
                var lods = sceneObject.LODs;
                scene.SceneObjectLODs.AddRange(lods);

                // LOD names
                foreach (var lod in lods)
                {
                    bool containsName = scene.SceneObjectNames.Contains(lod.Name);
                    if (containsName)
                    {
                        int elementIndex = scene.SceneObjectNames.IndexOf(lod.Name);
                        var existingName = scene.SceneObjectNames.ElementAt(elementIndex);
                        lod.Name = existingName;
                    }
                    else
                    {
                        scene.SceneObjectNames.Add(lod.Name);
                    }
                }
            }
            scene.SceneObjectNames = scene.SceneObjectNames.OrderBy(x => x.Value).ToList();
            scene.staticSceneObjects = new SceneObjectStatic[0];

            // Create output
            var fileName = $"st{scene.CourseIndex:00}";
            var gmaFileName = $"{fileName}.gma";
            var gmaFilePath = outputPath + gmaFileName;
            var tplFileName = $"{fileName}.tpl";
            var tplFilePath = outputPath + tplFileName;

            // GMA
            using (var writer = new EndianBinaryWriter(File.Create(gmaFilePath), Gma.endianness))
                writer.Write(gma);
            LzUtility.CompressAvLzToDisk(gmaFilePath, compressFormat, true);
            Debug.Log($"Created models archive '{gmaFilePath}'.");

            // TPL
            // Write out a hella bodged TPL. :eyes:
            // TODO: finish CMPR serialization, write textures for real.
            using (var writer = new BinaryWriter(File.Create(tplFilePath)))
            {
                var textureHashes = tplTextureContainer.GetTextureHashes();
                var stream = WriteBodgeTplFromTextureHashes(textureHashes);
                writer.Write(stream.ToArray());
            }
            LzUtility.CompressAvLzToDisk(tplFilePath, compressFormat, true);

            // SCENE
            // TEMP until data is stored properly in GFZ unity components
            MakeTrueNulls(scene);
            // Save out file and LZ'ed file
            var sceneFilePath = outputPath + scene.FileName;
            using (var writer = new EndianBinaryWriter(File.Create(sceneFilePath), Scene.endianness))
            {
                writer.Write(scene);
                writer.Flush();
            }
            LzUtility.CompressAvLzToDisk(sceneFilePath, compressFormat, true);
            OSUtility.OpenDirectory(outputPath);
            Debug.Log($"Created course '{sceneFilePath}'.");

            // LOG
            using (var writer = new StreamWriter(File.Create(sceneFilePath + ".txt")))
            {
                var builder = new System.Text.StringBuilder();
                scene.PrintMultiLine(builder);
                writer.Write(builder.ToString());
            }

            // User info
            // TODO: new means of assessing gma/tpl for BG
            //MessageCheckFileSizes(scene.CourseIndex, gmaFilePath, tplFilePath, sceneFilePath);

            //
            PatchEnemyLineFromSceneParameters();
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
        public static SceneObjectDynamic CreateSceneObjectDynamic()
        {
            var dynamicSceneObject = new SceneObjectDynamic()
            {
                Unk0x00 =
                    ObjectRenderFlags0x00.renderObject |
                    ObjectRenderFlags0x00.unk_RenderObject1 |
                    ObjectRenderFlags0x00.unk_RenderObject2 |
                    ObjectRenderFlags0x00.unk_RenderObject3 |
                    ObjectRenderFlags0x00.ReceiveEfbShadow,

                Unk0x04 = ObjectRenderFlags0x04._NULL,
            };
            return dynamicSceneObject;
        }
        public static Model[] CreateTrackModels(TplTextureContainer tpl, out SceneObject[] sceneObjects, out SceneObjectDynamic[] dynamicSceneObjects)
        {
            // get GfzTrack, use it to get children
            var track = GameObject.FindObjectOfType<GfzTrack>(false);
            track.RefreshSegmentNodes();

            var models = new List<Model>();
            var _sceneObjects = new List<SceneObject>();
            var _dynamicSceneObjects = new List<SceneObjectDynamic>();

            int shapeIndex = 0;
            foreach (var rootTrackSegmentNode in track.AllRoots)
            {
                var shapeNodes = rootTrackSegmentNode.GetShapeNodes();
                int subIndex = 0;
                foreach (var shape in shapeNodes)
                {
                    var gcmf = shape.CreateGcmf(out GcmfTemplate[] gcmfTemplates, tpl);
                    var modelName = $"{shape.GetRoot().name}-{shape.name}-#{shapeIndex}.{subIndex++}";
                    models.Add(new Model(modelName, gcmf));

                    var sceneObject = CreateSceneObject(modelName);
                    _sceneObjects.Add(sceneObject);

                    var sceneObjectDynamic = CreateSceneObjectDynamic();
                    sceneObjectDynamic.SceneObject = sceneObject;
                    sceneObjectDynamic.TransformMatrix3x4 = new();
                    sceneObjectDynamic.TextureScroll = GcmfTemplate.CombineTextureScrolls(gcmfTemplates);
                    sceneObjectDynamic.AssignTextureScrollFlags();
                    _dynamicSceneObjects.Add(sceneObjectDynamic);
                }
                shapeIndex++;
            }

            // OUT parameters
            sceneObjects = _sceneObjects.ToArray();
            dynamicSceneObjects = _dynamicSceneObjects.ToArray();
            // Return models
            return models.ToArray();
        }


        #region STATIC COLLIDER MESH MANAGER

        public static ColliderTriangle[] GetColliderTriangles(SerializeFormat format, out ushort[][] layerIndexes)
        {
            // All scripts in scene which are tagged as collidable
            var staticColliders = GameObject.FindObjectsOfType<GfzStaticColliderMesh>(false);
            // List to hold all collider triangle (each mesh is separate array in list)
            var colliderTriangleArrays = new List<ColliderTriangle[]>();
            // List to hold all indexes used per collider type.
            var triangleTypeLayerIndexes = GetIndexLists(format);
            int numberOfLayers = triangleTypeLayerIndexes.Length;

            // Get all triangles, count total
            int totalTriangles = 0;
            foreach (var staticCollider in staticColliders)
            {
                //staticCollider.CreateColliderOptimized(out ColliderTriangle[] triangles, out ColliderQuad[] quads);
                var triangles = staticCollider.CreateColliderTriangles();
                colliderTriangleArrays.Add(triangles);

                // Check to see what flags are used to 
                for (int layerIndex = 0; layerIndex < numberOfLayers; layerIndex++)
                {
                    // See if this collider asks to be part of this layer
                    bool usesLayer = (((uint)staticCollider.ColliderType >> layerIndex) & 1) > 0;
                    if (!usesLayer)
                        continue;

                    // If so, add an index to the recorded triangle
                    for (int i = 0; i < triangles.Length; i++)
                    {
                        ushort index = checked((ushort)(totalTriangles + i));
                        triangleTypeLayerIndexes[layerIndex].Add(index);
                    }
                }

                // Increment base index for triangles
                totalTriangles += triangles.Length;
            };


            // Place all indexes for each layer into the output array
            layerIndexes = new ushort[numberOfLayers][];
            for (int i = 0; i < layerIndexes.Length; i++)
            {
                // Cap all index layers with 0xFFFF like game does
                if (triangleTypeLayerIndexes[i].Count > 0)
                    triangleTypeLayerIndexes[i].Add(0xFFFF);
                // make array
                layerIndexes[i] = triangleTypeLayerIndexes[i].ToArray();
            }

            // Concatenate all arrays into a single array
            int baseOffset = 0;
            var allColliderTriangles = new ColliderTriangle[totalTriangles];
            foreach (var collection in colliderTriangleArrays)
            {
                collection.CopyTo(allColliderTriangles, baseOffset);
                baseOffset += collection.Length;
            }

            return allColliderTriangles;
        }

        public static List<ushort>[] GetIndexLists(SerializeFormat format)
        {
            int numLists = format == SerializeFormat.GX
                ? StaticColliderMeshManager.kCountGxSurfaceTypes
                : StaticColliderMeshManager.kCountAxSurfaceTypes;

            var indexListsForEachType = new List<ushort>[numLists];
            for (int i = 0; i < numLists; i++)
                indexListsForEachType[i] = new List<ushort>();

            return indexListsForEachType;
        }

        public static StaticColliderMeshGrid GetIndexListsAll(ushort[] indexes)
        {
            var indexGrid = new StaticColliderMeshGrid();
            var indexLists = new IndexList[StaticColliderMeshGrid.kListCount];
            for (int i = 0; i < indexLists.Length; i++)
            {
                var indexList = new IndexList();
                indexList.Indexes = indexes;
                indexLists[i] = indexList;
            }
            indexGrid.IndexLists = indexLists;
            return indexGrid;
        }

        #endregion

        public static (string gma, string tpl) GetBgGmaTplPaths(GfzSceneParameters sceneParameters)
        {
            //var venueID = CourseUtility.GetVenueID(stageID).ToString().ToLower();
            VenueID venueID = VenueID.None;
            var bg = $"bg_{venueID}";

            var settings = GfzProjectWindow.GetSettings();
            string inputPath = settings.SourceDirectory;

            var bgGma = Directory.GetFiles(inputPath, $"{bg}.gma", SearchOption.AllDirectories);
            var bgTpl = Directory.GetFiles(inputPath, $"{bg}.tpl", SearchOption.AllDirectories);
            Assert.IsTrue(bgGma.Length == 1);
            Assert.IsTrue(bgTpl.Length == 1);

            return (bgGma[0], bgTpl[0]);
        }

        public static Model[] RecoverMissingModelsFromStageGma(int stageID, TplTextureContainer textureHashesToIndex)
        {
            var venueID = CourseUtility.GetVenueID(stageID).ToString().ToLower();
            var bg = $"bg_{venueID}";
            const string race = "race";

            //string missingFileName = Path.GetFileNameWithoutExtension(missingFile);
            var tags = GameObject.FindObjectsOfType<GmaSourceTag>();
            var dictMissingModels = new Dictionary<string, GmaSourceTag>();

            foreach (var tag in tags)
            {
                bool isMissingModelReference =
                    tag.FileName != bg &&
                    tag.FileName != race;
                if (!isMissingModelReference)
                    continue;

                if (!dictMissingModels.ContainsKey(tag.ModelName))
                    dictMissingModels.Add(tag.ModelName, tag);
            }

            var models = new Model[dictMissingModels.Count];
            var settings = GfzProjectWindow.GetSettings();
            string inputPath = settings.SourceDirectory;

            var missingModels = dictMissingModels.Values.ToArray();
            for (int i = 0; i < missingModels.Length; i++)
            {
                var missingModel = missingModels[i];
                var filePaths = Directory.GetFiles(inputPath, $"{missingModel.FileName}.gma", SearchOption.AllDirectories);
                Assert.IsTrue(filePaths.Length == 1);
                var filePath = filePaths[0];

                using (var reader = new EndianBinaryReader(File.OpenRead(filePath), Gma.endianness))
                {
                    reader.JumpToAddress(missingModel.GcmfAddressRange.startAddress);

                    var model = new Model();
                    model.Name = missingModel.ModelName;
                    model.Gcmf = new Gcmf();
                    model.Gcmf.Deserialize(reader);
                    models[i] = model;

                    // TODO: don't be making arrays of size 1
                    // Edit TPL texture indexes, add textures to output TPL
                    RecoverMissingModelTextureHashes(missingModel.FileName, new Model[] { model }, textureHashesToIndex);
                }
            }

            return models;
        }

        private static void RecoverMissingModelTextureHashes(string sourceFile, Model[] models, TplTextureContainer tplTemplate)
        {
            var settings = GfzProjectWindow.GetSettings();
            string inputPath = settings.AssetsWorkingDirectory;

            // The structure which translates old TPL indexes into texture hashes
            string fileToTextureHashPath = inputPath + "tpl/TPL-TextureDescription-to-Hash.asset";
            var fileToTextureHashAsset = AssetDatabase.LoadAssetAtPath<TplTextureToTextureHash>(fileToTextureHashPath);
            if (fileToTextureHashAsset == null)
            {
                MessageHasNoTplHashReferenceObjects();
                fileToTextureHashAsset = AssetDatabase.LoadAssetAtPath<TplTextureToTextureHash>(fileToTextureHashPath);
            }
            var fileToTextureHashDict = fileToTextureHashAsset.GetDictionary();
            TplTextureHashes textureHashes = fileToTextureHashDict[sourceFile];

            foreach (var model in models)
            {
                var tevLayers = model.Gcmf.TevLayers;
                for (int i = 0; i < tevLayers.Length; i++)
                {
                    var tevLayer = tevLayers[i];
                    var tplIndex = tevLayer.TplTextureIndex;
                    string textureHash = textureHashes[tplIndex];

                    ushort textureIndex = tplTemplate.ContainsHash(textureHash)
                        ? tplTemplate.GetTextureHashIndex(textureHash)
                        : tplTemplate.AddTextureHash(textureHash);

                    tevLayer.TplTextureIndex = textureIndex;
                }
            }
        }

        public static MemoryStream WriteBodgeTplFromTextureHashes(string[] textureHashes)
        {
            var settings = GfzProjectWindow.GetSettings();
            string assetsWorkingDir = settings.AssetsWorkingDirectory;

            // The structure which translates old TPL indexes into texture hashes
            string textureHashToTextureInfoPath = assetsWorkingDir + "tpl/TPL-TextureHash-to-TextureInfo.asset";
            var textureHashToTextureInfoAsset = AssetDatabase.LoadAssetAtPath<TextureHashToTextureInfo>(textureHashToTextureInfoPath);
            if (textureHashToTextureInfoAsset == null)
            {
                MessageHasNoTplHashReferenceObjects();
                textureHashToTextureInfoAsset = AssetDatabase.LoadAssetAtPath<TextureHashToTextureInfo>(textureHashToTextureInfoPath);
            }
            var textureHashToTextureInfoDict = textureHashToTextureInfoAsset.GetDictionary();

            // Make a TPL, just copy texture data around
            var descriptions = new List<TextureDescription>();

            foreach (var textureHash in textureHashes)
            {
                TextureInfo textureInfo = textureHashToTextureInfoDict[textureHash];
                TextureDescription textureDescription = textureInfo.AsTextureDescription();
                descriptions.Add(textureDescription);
            }

            var stream = new MemoryStream();
            using (var writer = new EndianBinaryWriter(stream, Tpl.endianness))
            {
                // Write count
                writer.Write(descriptions.Count);

                // write descriptions. Addrs are wrong.
                foreach (var desc in descriptions)
                    writer.Write(desc);

                var align = writer.BaseStream.GetLengthOfAlignment(32); // fifo
                for (byte i = 0; i < align; i++)
                    writer.Write(i);

                //
                var src = settings.SourceDirectory;
                for (int i = 0; i < textureHashes.Length; i++)
                {
                    var textureHash = textureHashes[i];
                    TextureInfo textureInfo = textureHashToTextureInfoDict[textureHash];

                    var pattern = $"{textureInfo.SourceFileName}.tpl";
                    var tplPaths = Directory.GetFiles(src, pattern, SearchOption.AllDirectories);
                    Assert.IsTrue(tplPaths.Length == 1, pattern);
                    var tplPath = tplPaths[0];
                    using (var reader = new EndianBinaryReader(File.OpenRead(tplPath), Tpl.endianness))
                    {
                        // copy over data
                        reader.JumpToAddress(textureInfo.AddressRange.startAddress);
                        var bytes = reader.ReadBytes(textureInfo.AddressRange.Size);
                        descriptions[i].TexturePtr = writer.BaseStream.Position;
                        writer.Write(bytes);
                    }
                }

                writer.JumpToAddress(4);
                // write descriptions. Addrs are wrong.
                foreach (var desc in descriptions)
                    writer.Write(desc);
            }

            return stream;
        }

        public static void MessageHasNoTplHashReferenceObjects()
        {
            const string title = "Missing TPL Hash Reference Object";
            string message =
                $"You do not have a {typeof(TextureHashToTextureInfo).Name} or {nameof(TplTextureToTextureHash)} " +
                $"scriptable object in your assets folder. Would you like to build it now?";

            bool doAction = EditorUtility.DisplayDialog(title, message, "Yes", "No");
            if (doAction)
            {
                TplMenuItems.BuildHashReferenceObject();
            }
            else
            {
                throw new NullReferenceException(title);
            }
        }

        public static void MessageCheckFileSizes(GfzSceneParameters sceneParameters, params string[] filePaths)
        {
            (string bgGmaFilePath, string bgTplFilePath) = GetBgGmaTplPaths(sceneParameters);

            var all = new List<string>();
            all.Add(bgGmaFilePath);
            all.Add(bgTplFilePath);
            all.AddRange(filePaths);

            MessageCheckFileSizes(all.ToArray());
        }

        public static void MessageCheckFileSizes(params string[] filePaths)
        {
            int totalBytes = 0;
            for (int i = 0; i < filePaths.Length; i++)
            {
                string filePath = filePaths[i];
                using (var reader = new BinaryReader(File.OpenRead(filePath)))
                {
                    int length = (int)reader.BaseStream.Length;
                    totalBytes += length;
                    Debug.Log($"{filePath}: {length:n0} bytes.");
                }
            }

            const int oneMegaByte = 1_000_000;
            const int memBudgetBytes = 6_400_000; // 6,402,016: stats from GFZJ, Mute City 
            const float memBudgetMB = (float)memBudgetBytes / (float)oneMegaByte;
            float sizeInMB = totalBytes / (float)oneMegaByte;
            float percentOfAllocation = totalBytes / (float)memBudgetBytes * 100f;
            Debug.Log($"Total bytes: {totalBytes:n0}");
            Debug.Log($"Approximate memory budget: {sizeInMB:n1} of {memBudgetMB:n1} MB ({percentOfAllocation:n0}%)");
            if (percentOfAllocation > 95)
                Debug.LogWarning($"Consider reducing track mesh polycount, cleaning up path keyframes, and/or hiding/deleting unused objects/triggers.");
            if (percentOfAllocation > 100)
                Debug.LogError($"Expect background-archive models to not load in or the game to crash.");
        }

        public static GameCube.AmusementVision.GxGame GetCompressFormat(SerializeFormat serializeFormat)
        {
            switch (serializeFormat)
            {
                case SerializeFormat.AX:
                    return GameCube.AmusementVision.GxGame.FZeroAX;

                case SerializeFormat.GX:
                    return GameCube.AmusementVision.GxGame.FZeroGX;

                default:
                    throw new ArgumentException($"Invalid format '{serializeFormat}' specified for serialization.");
            }
        }

    }
}
