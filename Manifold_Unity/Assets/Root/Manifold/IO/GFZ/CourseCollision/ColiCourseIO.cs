using GameCube.GFZ;
using GameCube.GFZ.CourseCollision;
using Manifold.IO.GFZ.CourseCollision;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

using Manifold.Conversion;
using System.Security.Cryptography;

namespace Manifold.IO.GFZ
{
    public static class ColiCourseIO
    {
        [MenuItem("Manifold/IO/Test IO 1")]
        public static void TestIO1()
        {
            LogRoundtrip("C:/GFZJ01/stage/COLI_COURSE01", true);
        }

        [MenuItem("Manifold/IO/Export Roundtrip Summaries (4 formats) %F1")]
        public static void TestHash()
        {
            LogSceneDataRoundtripFormat4("C:/GFZJ01/stage/COLI_COURSE01", true);
            //for (int i = 0; i <= 50; i++)
            //{
            //    try
            //    {
            //        LogSceneDataRoundtripFormat4($"C:/GFZJ01/stage/COLI_COURSE{i:d2}", true);
            //    }
            //    catch { }
            //}
        }

        //[MenuItem("Manifold/IO/Roundtrip No Log %F2")]
        //public static void ExportRoundtripNoLog()
        //{
        //    RoundtripNoLog("C:/GFZJ01/stage/COLI_COURSE01", true);
        //}


        [MenuItem("Manifold/IO/Roundtrip Error Check %F3")]
        public static void RoundtripErrorCheck()
        {
            SceneRoundtripErrorTest("C:/GFZJ01/stage/COLI_COURSE01", true);
        }

        [MenuItem("Manifold/IO/Roundtrip Error Check All GFZJ %F4")]
        public static void RoundtripErrorCheck2()
        {
            for (int i = 0; i <= 50; i++)
            {
                SceneRoundtripErrorTest($"C:/GFZJ01/stage/COLI_COURSE{i:d2}", true);
            }
            EditorUtility.DisplayDialog("Roundtrip Error Check All Gfze", "All tests passed!", "ok");
        }

        [MenuItem("Manifold/IO/Test Load Stage 01 GFZJ")]
        public static void TestLoadStage01()
        {            
                var filePath = $"C:/GFZJ01/stage/COLI_COURSE01";
                var fileName = Path.GetFileName(filePath);
                // LOAD FRESH FILE IN
                var reader = new BinaryReader(File.OpenRead(filePath));
                var scene = new ColiScene();
                scene.FileName = fileName;
                reader.ReadX(ref scene, false);
        }

        [MenuItem("Manifold/IO/Test Load All Stages GFZJ")]
        public static void TestLoadAllStages()
        {
            for (int i = 0; i <= 50; i++)
            {
                // Get file name (must exist)
                var filePath = $"C:/GFZJ01/stage/COLI_COURSE{i:d2}";
                if (!File.Exists(filePath))
                    continue;
                var fileName = Path.GetFileName(filePath);

                EditorUtility.DisplayProgressBar("Test Load All Stages", fileName, i/50f);

                // LOAD FRESH FILE IN
                var reader = new BinaryReader(File.OpenRead(filePath));
                var scene = new ColiScene();
                scene.FileName = fileName;
                reader.ReadX(ref scene, false);
            }
            EditorUtility.ClearProgressBar();
        }

        [MenuItem("Manifold/IO/SAVE All Stages GFZJ")]
        public static void TestSaveAllStages()
        {
            for (int i = 0; i <= 50; i++)
            {
                // Get file name (must exist)
                var filePath = $"C:/GFZJ01/stage/COLI_COURSE{i:d2}";
                if (!File.Exists(filePath))
                    continue;
                var fileName = Path.GetFileName(filePath);

                EditorUtility.DisplayProgressBar("Save All Stages", fileName, i / 50f);

                // Load scene files
                var reader = new AddressLogBinaryReader(File.OpenRead(filePath));
                var scene = new ColiScene();
                scene.FileName = fileName;
                reader.ReadX(ref scene, false);
                reader.LogRangesRead($"C:/test/output/log-ranges-read-{fileName}.txt");
                // Save scene files
                var savePath = $"C:/test/output/{fileName}";
                var writer = new AddressLogBinaryWriter(File.Create(savePath));
                scene.SerializeVerbose = true;
                scene.Serialize(writer);
                // Close reader/writer
                writer.Close();
                reader.Close();
                // Compress output
                GfzUtility.CompressAv(savePath, LibGxFormat.AvGame.FZeroGX);
            }
            EditorUtility.ClearProgressBar();
        }

        [MenuItem("Manifold/IO/GFZE/Log All")]
        public static void RoundtripErrorCheckAllGfze()
        {
            for (int i = 0; i <= 50; i++)
            {
                try
                {
                    LogSceneDataRoundtripFormat4($"C:/GFZE01/stage/COLI_COURSE{i:d2}", true);
                }
                catch
                {
                    //Debug.Log($"Skipping index {i:d2}");
                }
            }
        }

        public static void LogRoundtrip(string filePath, bool serializeVerbose)
        {
            // TEMP from previous function
            var openFolderAfterExport = true;
            var allowOverwritingFiles = true;
            var exportTo = "W:/Windows Directories/Desktop/test";
            /////////////////////////////////////////////////////

            // Construct time stamp string
            var dateTime = DateTime.Now;
            var timestamp = $"[{dateTime:yyyy-MM-dd}][{dateTime:HH-mm-ss}]";

            // still valid??
            ColiCourseUtility.SerializeVerbose = serializeVerbose;


            // LOAD FRESH FILE IN
            var readerIn = new BinaryReader(File.OpenRead(filePath));
            var sceneIn = new ColiScene();
            sceneIn.FileName = Path.GetFileName(filePath);
            sceneIn.SerializeVerbose = serializeVerbose;
            readerIn.ReadX(ref sceneIn, false);
            // Log true file
            {
                var logPath = Path.Combine(exportTo, $"{timestamp} - COLI_COURSE{sceneIn.ID:d2} IN.txt");
                var log = new TextLogger(logPath);
                LogSceneData(log, sceneIn);
                log.Close();
            }

            // WRITE INTERMEDIARY
            // Write scene out to memory stream...
            var writer = new BinaryWriter(new MemoryStream());
            writer.WriteX(sceneIn);
            writer.Flush();
            writer.BaseStream.Seek(0, SeekOrigin.Begin);

            // LOAD INTERMEDIARY
            // Load memory stream back in
            var readerOut = new BinaryReader(writer.BaseStream);
            var sceneOut = new ColiScene();
            sceneOut.SerializeVerbose = serializeVerbose;
            sceneOut.FileName = sceneIn.FileName;
            readerOut.ReadX(ref sceneOut, false);
            // Log intermediary
            {
                var logPath = Path.Combine(exportTo, $"{timestamp} - COLI_COURSE{sceneIn.ID:d2} OUT.txt");
                var log = new TextLogger(logPath);
                LogSceneData(log, sceneIn);
                log.Close();
            }

            // open folder location
            OSUtility.OpenDirectory(openFolderAfterExport, exportTo);
            // Set static variable
            // Export file...
            //var x = ExportUtility.ExportSerializable(sceneIn, exportTo, "", allowOverwritingFiles);
            //OSUtility.OpenDirectory(openFolderAfterExport, x);
        }



        // 2021/12/21: load stage in, save stage out (todo: is hardcoded)
        [MenuItem("Manifold/IO/Save Out Stages")]
        public static void InOutStage()
        {
            // TEMP from previous function
            var serializeVerbose = true;
            var openFolderAfterExport = true;
            var allowOverwritingFiles = true;
            var exportTo = "C:/test";
            /////////////////////////////////////////////////////
            // Construct time stamp string
            var dateTime = DateTime.Now;
            var timestamp = $"[{dateTime:yyyy-MM-dd}][{dateTime:HH-mm-ss}]";
            var md5 = MD5.Create();

            for (int i = 0; i < 50; i++)
            {
                // Get file name (must exists)
                var filePath = $"C:/GFZE01/stage/COLI_COURSE{i:d2}";
                if (!File.Exists(filePath))
                    continue;

                // LOAD FRESH FILE IN
                var readerIn = new BinaryReader(File.OpenRead(filePath));
                // Set scene instance, deserialize data
                var scene = new ColiScene();
                scene.FileName = Path.GetFileName(filePath);
                scene.SerializeVerbose = serializeVerbose;
                readerIn.ReadX(ref scene, false);
                // Check to see if file is from ROM. Add correct metadata.
                // TODO: remove hardcoded match and use dynamic JP/EN/PAL
                readerIn.SeekBegin();
                var sceneIndex = scene.ID;
                var sceneHash = md5.ComputeHash(readerIn.BaseStream);
                var sceneHashStr = HashUtility.ByteArrayToString(sceneHash);
                var romHashStr = HashLibrary.ColiCourseMD5_GFZJ01[sceneIndex];
                if (romHashStr == sceneHashStr)
                {
                    scene.Venue = CourseUtility.GetVenue(sceneIndex);
                    scene.CourseName = CourseUtility.GetCourseName(sceneIndex);
                    scene.Author = "Amusement Vision";
                }

                var sceneSavePath = ExportUtility.ExportSerializable(scene, exportTo, "", allowOverwritingFiles);
                GfzUtility.CompressAv(sceneSavePath, LibGxFormat.AvGame.FZeroGX, true, out _);
                OSUtility.OpenDirectory(sceneSavePath);
            }
        }


        public static void ExportUnityScenes(ColiScene.SerializeFormat format, bool serializeVerbose, params SceneAsset[] scenes)
        {
            // TEMP from previous function
            //var serializeVerbose = true;
            var openFolderAfterExport = true;
            var allowOverwritingFiles = true;
            var exportTo = "C:/test";
            /////////////////////////////////////////////////////

            //ColiCourseUtility.SerializeVerbose = serializeVerbose;

            // Construct ColiScene from Unity Scene "scenes"
            var coliScenes = new ColiScene[scenes.Length];

            // Iterate over each scene asset
            for (int sceneIndex = 0; sceneIndex < coliScenes.Length; sceneIndex++)
            {
                // DEBUG
                const bool canFindInactive = true;
                const int h1Width = 96;
                const int h2Width = 32;
                const string padding = "-";

                // Initialize new instance
                coliScenes[sceneIndex] = new ColiScene();

                // Breakout values
                var scene = scenes[sceneIndex];
                var coliScene = coliScenes[sceneIndex];

                // Set serialization format
                coliScene.Format = format;
                coliScene.SerializeVerbose = serializeVerbose;

                // Get path to scene object
                var scenePath = AssetDatabase.GetAssetPath(scene);
                // Open scene in Unity...
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                // ... so we can start doing the usual Unity methods to find stuff

                // Get scene parameters for general info
                var sceneParameters = GameObject.FindObjectsOfType<GfzSceneParameters>();
                Assert.IsTrue(sceneParameters.Length == 1);
                var sceneParams = sceneParameters[0];

                // Start logging!
                var internalName = sceneParams.GetGfzInternalName();
                var displayName = sceneParams.GetGfzDisplayName();
                var dateTime = DateTime.Now;
                var timestamp = $"[{dateTime:yyyy-MM-dd}][{dateTime:HH-mm-ss}]";
                var logPath = Path.Combine(exportTo, $"{timestamp} {internalName} - {displayName}.txt");
                var log = new TextLogger(logPath);

                // Write file and course title
                log.WriteHeading("FILE INFORMATION", padding, h2Width);
                log.WriteLine($"File: {internalName}");
                log.WriteLine($"Name: {displayName}");
                log.WriteLine();

                // Validate venue and id
                {
                    bool isValidIndexForVenue = CourseUtility.GetVenue(sceneParams.courseIndex) == sceneParams.venue;
                    byte r = isValidIndexForVenue ? (byte)0 : (byte)255;
                    log.WriteLineColor($"{nameof(isValidIndexForVenue)} = {isValidIndexForVenue}", r, 0, 0);
                }

                // Get scene-wide parameters from SceneParameters
                {
                    // Set filename to what F-Zero GX/AX would use
                    coliScene.FileName = internalName;
                    coliScene.CourseName = sceneParams.courseName;
                    coliScene.Venue = sceneParams.venue;
                    coliScene.Author = sceneParams.author;
                    // Construct range from 2 parameters
                    coliScene.unkRange0x00 = new Range(sceneParams.rangeNear, sceneParams.rangeFar);
                    // Use functions to get form parameters
                    coliScene.fog = sceneParams.ToGfzFog();
                    coliScene.fogCurves = sceneParams.ToGfzFogCurves();

                    var fogMsg = sceneParams.exportCustomFog
                        ? $"{nameof(Fog)}: using custom fog."
                        : $"{nameof(Fog)}: using default fog for {sceneParams.venue}.";
                    log.WriteLine(fogMsg);

                    var fogCurvesMsg = sceneParams.exportCustomFog
                        ? $"{nameof(FogCurves)}: using custom fog curves."
                        : $"{nameof(FogCurves)}: using default fog curves for {sceneParams.venue}.";
                    log.WriteLine(fogCurvesMsg);
                    log.WriteLine();
                }

                // Static Collider Meshes
                {
                    coliScene.staticColliderMeshes = new StaticColliderMeshes(format);
                }

                // Triggers
                {
                    var arcadeCheckpointTriggers = GameObject.FindObjectsOfType<GfzArcadeCheckpoint>(canFindInactive);
                    coliScene.arcadeCheckpointTriggers = GetGfzValues(arcadeCheckpointTriggers);

                    // This trigger type is a mess... Get all 3 representations, combine, assign.
                    // Collect all trigger types. They all get converted to the same GFZ base type.
                    var objectPaths = GameObject.FindObjectsOfType<GfzObjectPath>(canFindInactive);
                    var storyCapsules = GameObject.FindObjectsOfType<GfzStoryCapsule>(canFindInactive);
                    var unknownMetadataTriggers = GameObject.FindObjectsOfType<GfzUnknownCourseMetadataTrigger>(canFindInactive);
                    // Make a list, add range for each type
                    var courseMetadataTriggers = new List<CourseMetadataTrigger>();
                    courseMetadataTriggers.AddRange(GetGfzValues(objectPaths));
                    courseMetadataTriggers.AddRange(GetGfzValues(storyCapsules));
                    courseMetadataTriggers.AddRange(GetGfzValues(unknownMetadataTriggers));
                    // Convert list to array, assign to ColiScene
                    coliScene.courseMetadataTriggers = courseMetadataTriggers.ToArray();

                    // TODO:
                    // story object triggers

                    // This trigger type is a mess... Get all 3 representations, combine, assign.
                    var unknownTriggers = GameObject.FindObjectsOfType<GfzUnknownTrigger>(canFindInactive);
                    coliScene.unknownTriggers = GetGfzValues(unknownTriggers);

                    var unknownSolsTriggers = GameObject.FindObjectsOfType<GfzUnknownSolsTrigger>(canFindInactive);
                    coliScene.unknownSolsTriggers = GetGfzValues(unknownSolsTriggers);

                    var visualEffectTriggers = GameObject.FindObjectsOfType<GfzVisualEffectTrigger>(canFindInactive);
                    coliScene.visualEffectTriggers = GetGfzValues(visualEffectTriggers);


                    log.WriteLine("TRIGGERS");
                    log.WriteLineSummary(coliScene.arcadeCheckpointTriggers);
                    // Log. TODO: more granularity in type.
                    log.WriteLineSummary(coliScene.courseMetadataTriggers);
                    log.WriteLineSummary(coliScene.unknownTriggers);
                    log.WriteLineSummary(coliScene.unknownSolsTriggers);
                    log.WriteLineSummary(coliScene.visualEffectTriggers);
                    log.WriteLine();
                }

                // Scene Objects / Instances / References / Names
                {
                    var sceneObjects = GameObject.FindObjectsOfType<GfzSceneObject>(canFindInactive);
                    coliScene.dynamicSceneObjects = new SceneObjectDynamic[0];

                    // TODO: construct the actual objects...

                    log.WriteLine();
                    log.WriteLine("SCENE OBJECTS");
                    log.WriteLineSummary(coliScene.dynamicSceneObjects);
                    log.WriteLineSummary(coliScene.staticSceneObjects);
                    log.WriteLineSummary(coliScene.templateSceneObjects);
                    log.WriteLineSummary(coliScene.sceneObjects);
                    log.WriteLineSummary(coliScene.sceneObjectNames);
                    log.WriteLine();
                }

                // Track Data
                {
                    var controlPoints = GameObject.FindObjectsOfType<GfzControlPoint>(canFindInactive);

                    //coliScene.

                    // Parse all CPs, get root nodes only?
                    // Serialization could then run from root through chicldren, respecting array pointer seriialization.

                    //
                    coliScene.trackCheckpointMatrix = new TrackCheckpointMatrix();

                    // TODO: this is temp data
                    coliScene.trackLength = new TrackLength();
                    coliScene.trackMinHeight = new TrackMinHeight();
                }
                Debug.LogWarning("TODO: define bounds");

                // Export the file
                var exported = ExportUtility.ExportSerializable(coliScene, exportTo, "", allowOverwritingFiles);
                log.WriteHeading("EXPORT", padding, h1Width);
                log.WriteLine($"Exported file to path \"{exported}\"");
                log.WriteLine();
                LogSceneData(log, coliScene);
                log.WriteLine();
                log.WriteHeading("END OF FILE", padding, h1Width);
                log.Close();

                OSUtility.OpenDirectory(openFolderAfterExport, exported);
                OSUtility.OpenDirectory(openFolderAfterExport, logPath);
            }

            //
            // NOTE: no "preserve structure"
            // TODO: make these export functions less long.
            var exportedFiles = ExportUtility.ExportSerializable(coliScenes, exportTo, "", allowOverwritingFiles);
            OSUtility.OpenDirectory(openFolderAfterExport, exportedFiles);
        }


        /// <summary>
        /// Helper function to get GFZ data from Unity scene wrapper types.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="unity"></param>
        /// <returns></returns>
        public static T[] GetGfzValues<T>(IGfzConvertable<T>[] unity)
        {
            var gfz = new T[unity.Length];
            for (int i = 0; i < unity.Length; i++)
                gfz[i] = unity[i].ExportGfz();

            return gfz;
        }

        public static void TestHash(TextLogger log, ColiScene coliScene)
        {
            var md5 = MD5.Create();
            foreach (var trackSegment in coliScene.allTrackSegments)
            {
                log.WriteLine(HashUtility.HashBinary(md5, trackSegment));
            }
        }
        public static void LogSceneData(TextLogger log, ColiScene coliScene)
        {
            var md5 = MD5.Create();

            const int h1Width = 96; // heading 1 character width
            const string padding = "-"; // heading padding character

            // Write out all types and their address in file
            log.WriteHeading("SERIALIZATION SUMMARY", padding, h1Width);
            log.WriteLine();

            log.WriteHeading("GENERAL", padding, h1Width);
            log.WriteLine($"Venue: {coliScene.Venue}");
            log.WriteLine($"Course: {coliScene.VenueName} [{coliScene.CourseName}]");
            log.WriteLine($"Author: {coliScene.Author}");
            log.WriteLine($"{nameof(CircuitType)}: {coliScene.circuitType}");
            log.WriteLine($"{nameof(Bool32)}: {coliScene.staticColliderMeshesActive}");
            log.WriteLine($"{nameof(coliScene.unkRange0x00)}: {coliScene.unkRange0x00}");
            log.WriteAddress(coliScene.fog);
            log.WriteAddress(coliScene.fogCurves);
            log.WriteLine(); //
            log.WriteLine(); // yes, 2 WriteLines

            log.WriteHeading("TRIGGERS", padding, h1Width);
            log.WriteAddress(coliScene.arcadeCheckpointTriggers);
            log.WriteAddress(coliScene.courseMetadataTriggers);
            log.WriteAddress(coliScene.storyObjectTriggers);
            log.WriteAddress(coliScene.unknownSolsTriggers);
            log.WriteAddress(coliScene.unknownTriggers);
            log.WriteAddress(coliScene.visualEffectTriggers);

            // Writes non-array track data
            log.WriteHeading("TRACK DATA", padding, h1Width);
            log.WriteAddress(coliScene.trackLength);
            log.WriteAddress(coliScene.trackMinHeight);
            log.WriteAddress(coliScene.courseBoundsXZ);
            log.WriteLine();
            // Writes track objects array
            log.WriteAddress(coliScene.surfaceAttributeAreas);
            // Writes track segments (root then all)
            log.WriteLine("ROOT SEGMENTS");
            log.WriteAddress(coliScene.rootTrackSegments);
            log.WriteLine("ALL SEGMENTS");
            log.WriteAddress(coliScene.allTrackSegments);
            log.WriteLine();
            TestHash(log, coliScene);
            log.WriteLine();

            // This block writes out the contents of each TrackSegments AnimationCurves
            log.WriteLine("TRACK SEGMENT ANIMATION CURVES");
            log.WriteLine($"{nameof(TrackSegment)}.{nameof(TrackSegment.trackAnimationCurves)}");
            string[] labelSRP = new string[] { "Sca", "Rot", "Pos" };
            string[] labelXYZ = new string[] { "x", "y", "z" };
            for (int segmentIndex = 0; segmentIndex < coliScene.allTrackSegments.Length; segmentIndex++)
            {
                var trackSegment = coliScene.allTrackSegments[segmentIndex];
                log.WriteLine($"{nameof(TrackSegment)}[{segmentIndex}]\t{trackSegment}");

                for (int animIndex = 0; animIndex < trackSegment.trackAnimationCurves.animationCurves.Length; animIndex++)
                {
                    var animCurve = trackSegment.trackAnimationCurves.animationCurves[animIndex];
                    var currLabelSRP = labelSRP[animIndex / 3];
                    var currLabelXYZ = labelXYZ[animIndex % 3];
                    log.WriteLine($"{currLabelSRP}.{currLabelXYZ} [{animIndex}] ");
                    log.WriteArrayToString(animCurve.keyableAttributes);
                    //log.WriteLine(HashSerializables.Hash(md5, animCurve));
                    log.WriteLine();
                }
            }

            //
            for (int i = 0; i < coliScene.allTrackSegments.Length; i++)
            {
                var segment = coliScene.allTrackSegments[i];
                log.WriteLine($"{nameof(TrackSegment)} Transform Coords [{i}]");
                log.WriteLine($"\tPosition: {segment.localPosition}");
                log.WriteLine($"\tRotation: {segment.localRotation}");
                log.WriteLine($"\tScale...: {segment.localScale}");
            }
            log.WriteLine();
            //
            log.WriteAddress(coliScene.trackCheckpointMatrix);
            log.WriteAddress(coliScene.trackNodes);
            {
                var checkpoints = new List<TrackCheckpoint>();
                foreach (var trackNode in coliScene.trackNodes)
                    foreach (var checkpoint in trackNode.checkpoints)
                        checkpoints.Add(checkpoint);
                log.WriteAddress(checkpoints.ToArray());
            }


            // checkpoints
            // segments
            log.WriteLine();
            log.WriteHeading("STATIC COLLISION", padding, h1Width);
            log.WriteAddress(coliScene.staticColliderMeshes);
            log.WriteLine();
            log.WriteLine("Mesh Bounds");
            log.WriteAddress(coliScene.staticColliderMeshes.meshBounds);
            log.WriteAddress(coliScene.staticColliderMeshes.unusedMeshBounds);
            log.WriteLine();
            log.WriteLine("TRIANGLES");
            log.WriteAddress(coliScene.staticColliderMeshes.colliderTriangles);
            log.WriteAddress(coliScene.staticColliderMeshes.triMeshIndexMatrices);
            // Write each index list
            log.WriteNullInArray = false;
            for (int i = 0; i < coliScene.staticColliderMeshes.triMeshIndexMatrices.Length; i++)
            {
                var triIndexList = coliScene.staticColliderMeshes.triMeshIndexMatrices[i];
                if (triIndexList != null)
                {
                    log.WriteLine($"COLLIDER TYPE [{i}]: {(StaticColliderMeshProperty)i}");
                    log.WriteAddress(triIndexList.indexLists);
                }
            }
            log.WriteNullInArray = true;
            log.WriteLine("QUADS");
            log.WriteAddress(coliScene.staticColliderMeshes.colliderQuads);
            log.WriteAddress(coliScene.staticColliderMeshes.quadMeshIndexMatrices);
            // Write each index list
            log.WriteNullInArray = false;
            for (int i = 0; i < coliScene.staticColliderMeshes.quadMeshIndexMatrices.Length; i++)
            {
                var quadIndexList = coliScene.staticColliderMeshes.quadMeshIndexMatrices[i];
                if (quadIndexList != null)
                {
                    log.WriteLine($"COLLIDER TYPE [{i}]: {(StaticColliderMeshProperty)i}");
                    log.WriteAddress(quadIndexList.indexLists);
                }
            }
            log.WriteNullInArray = true;
            log.WriteLine();

            log.WriteHeading("SCENE OBJECTS", padding, h1Width);
            log.WriteLine("Object Names");
            log.WriteAddress(coliScene.sceneObjectNames);
            log.WriteAddress(coliScene.sceneObjects);
            log.WriteAddress(coliScene.templateSceneObjects);
            log.WriteAddress(coliScene.staticSceneObjects);
            log.WriteAddress(coliScene.dynamicSceneObjects);
            {
                log.WriteHeading("FULL OBJECT SUMMARY", padding, h1Width);
                int total = coliScene.dynamicSceneObjects.Length;
                for (int i = 0; i < total; i++)
                {
                    var sceneObject = coliScene.dynamicSceneObjects[i];

                    log.WriteLine($"[{i}/{total}] {sceneObject.nameCopy}");
                    log.WriteAddress(sceneObject);
                    log.WriteAddress(sceneObject.templateSceneObject);
                    log.WriteAddress(sceneObject.templateSceneObject.sceneObject);
                    log.WriteAddress(sceneObject.templateSceneObject.sceneObject.name);
                    log.WriteAddress(sceneObject.transformMatrix3x4);

                    log.WriteAddress(sceneObject.skeletalAnimator);
                    if (sceneObject.skeletalAnimator != null)
                        log.WriteAddress(sceneObject.skeletalAnimator.properties);

                    log.WriteAddress(sceneObject.animation);
                    if (sceneObject.animation != null)
                        log.WriteAddress(sceneObject.animation.curve);
                    // TODO: other sub classes?

                    log.WriteAddress(sceneObject.textureMetadata);
                    if (sceneObject.textureMetadata != null)
                        log.WriteAddress(sceneObject.textureMetadata.fields);

                    log.WriteLine();
                }
            }
            log.WriteLine();
        }

        /// <summary>
        /// Creates 4 files per scene, the one loaded, and another reserialized from load. 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="serializeVerbose"></param>
        public static void LogSceneDataRoundtripFormat4(string filePath, bool serializeVerbose)
        {
            var md5 = MD5.Create();
            string[] labels = new string[] { "Value", "Address", "Stream", "Hash" };

            // TEMP from previous function
            var openFolderAfterExport = true;
            var exportTo = "C:/test";
            /////////////////////////////////////////////////////

            // Construct time stamp string
            var dateTime = DateTime.Now;
            var timestamp = $"[{dateTime:yyyy-MM-dd}][{dateTime:HH-mm-ss}]";

            // LOAD FRESH FILE IN
            var readerIn = new BinaryReader(File.OpenRead(filePath));
            // Set scene instance, deserialize data
            var sceneIn = new ColiScene();
            sceneIn.FileName = Path.GetFileName(filePath);
            sceneIn.SerializeVerbose = serializeVerbose;
            readerIn.ReadX(ref sceneIn, false);
            // Check to see if file is from ROM. Add correct metadata.
            // TODO: remove hardcoded match and use dynamic JP/EN/PAL
            readerIn.SeekBegin();
            var sceneIndex = sceneIn.ID;
            var sceneHash = md5.ComputeHash(readerIn.BaseStream);
            var sceneHashStr = HashUtility.ByteArrayToString(sceneHash);
            var romHashStr = HashLibrary.ColiCourseMD5_GFZJ01[sceneIndex];
            if (romHashStr == sceneHashStr)
            {
                sceneIn.Venue = CourseUtility.GetVenue(sceneIndex);
                sceneIn.CourseName = CourseUtility.GetCourseName(sceneIndex);
                sceneIn.Author = "Amusement Vision";
            }

            // Log true file
            {
                //WriteTrackDataHashReport(log, sceneIn);
                for (int i = 0; i < NumFunctions; i++)
                {
                    var logPath = Path.Combine(exportTo, $"{timestamp} - {i + 1} {labels[i]} COLI_COURSE{sceneIn.ID:d2} IN.txt");
                    var log = new TextLogger(logPath);
                    LogColiScene(sceneIn, log, i);
                    log.Close();
                }
            }

            // WRITE INTERMEDIARY
            // Write scene out to memory stream...
            var writer = new BinaryWriter(new MemoryStream());
            writer.WriteX(sceneIn);
            writer.Flush();
            writer.BaseStream.Seek(0, SeekOrigin.Begin);

            // LOAD INTERMEDIARY
            // Load memory stream back in
            var readerOut = new BinaryReader(writer.BaseStream);
            var sceneOut = new ColiScene();
            sceneOut.SerializeVerbose = serializeVerbose;
            sceneOut.FileName = sceneIn.FileName;
            readerOut.ReadX(ref sceneOut, false);
            // Log intermediary
            {
                //WriteTrackDataHashReport(log, sceneIn);
                for (int i = 0; i < NumFunctions; i++)
                {
                    var logPath = Path.Combine(exportTo, $"{timestamp} - {i + 1} {labels[i]} COLI_COURSE{sceneIn.ID:d2} OUT.txt");
                    var log = new TextLogger(logPath);
                    LogColiScene(sceneIn, log, i);
                    log.Close();
                }
            }

            // open folder location
            OSUtility.OpenDirectory(openFolderAfterExport, $"{exportTo}/");
            // Set static variable
            // Export file...
            var x = ExportUtility.ExportSerializable(sceneIn, exportTo, "", true);
            OSUtility.OpenDirectory(openFolderAfterExport, x);
        }

        //public static void RoundtripNoLog(string filePath, bool serializeVerbose)
        //{
        //    var md5 = MD5.Create();
        //    // TEMP from previous function
        //    var openFolderAfterExport = true;
        //    var exportTo = "W:/Windows Directories/Desktop/test";
        //    /////////////////////////////////////////////////////

        //    // Construct time stamp string
        //    var dateTime = DateTime.Now;
        //    var timestamp = $"[{dateTime:yyyy-MM-dd}][{dateTime:HH-mm-ss}]";

        //    // LOAD FRESH FILE IN
        //    var readerIn = new BinaryReader(File.OpenRead(filePath));
        //    // Set scene instance, deserialize data
        //    var sceneIn = new ColiScene();
        //    sceneIn.FileName = Path.GetFileName(filePath);
        //    sceneIn.SerializeVerbose = serializeVerbose;
        //    readerIn.ReadX(ref sceneIn, false);
        //    // Check to see if file is from ROM. Add correct metadata.
        //    // TODO: remove hardcoded match and use dynamic JP/EN/PAL
        //    readerIn.SeekBegin();
        //    var sceneIndex = sceneIn.ID;
        //    var sceneHash = md5.ComputeHash(readerIn.BaseStream);
        //    var sceneHashStr = HashUtility.ByteArrayToString(sceneHash);
        //    var romHashStr = HashLibrary.ColiCourseMD5_GFZJ01[sceneIndex];
        //    if (romHashStr == sceneHashStr)
        //    {
        //        sceneIn.Venue = CourseUtility.GetVenue(sceneIndex);
        //        sceneIn.CourseName = CourseUtility.GetCourseName(sceneIndex);
        //        sceneIn.Author = "Amusement Vision";
        //    }

        //    // WRITE INTERMEDIARY
        //    // Write scene out to memory stream...
        //    var writer = new BinaryWriter(new MemoryStream());
        //    writer.WriteX(sceneIn);
        //    writer.Flush();
        //    writer.BaseStream.Seek(0, SeekOrigin.Begin);

        //    // LOAD INTERMEDIARY
        //    // Load memory stream back in
        //    var readerOut = new BinaryReader(writer.BaseStream);
        //    var sceneOut = new ColiScene();
        //    sceneOut.SerializeVerbose = serializeVerbose;
        //    sceneOut.FileName = sceneIn.FileName;
        //    readerOut.ReadX(ref sceneOut, false);

        //    // open folder location
        //    OSUtility.OpenDirectory(openFolderAfterExport, $"{exportTo}/");
        //    // Set static variable
        //    // Export file...
        //    var x = ExportUtility.ExportSerializable(sceneIn, exportTo, "", true);
        //    OSUtility.OpenDirectory(openFolderAfterExport, x);
        //}

        /// <summary>
        /// Function goal: round trip asset multiple times. If misalignment somewhere, should cause errors.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="serializeVerbose"></param>
        public static void SceneRoundtripErrorTest(string filePath, bool serializeVerbose)
        {
            if (!File.Exists(filePath))
                return;

            // Stream is changed after each iteration
            Stream stream = File.OpenRead(filePath);
            string filename = Path.GetFileName(filePath);
            const int iterations = 3;

            for (int i = 0; i < iterations; i++)
            {
                int index = i + 1;
                EditorUtility.DisplayProgressBar("Roundtrip Error Test", $"{filename}, Test ({index}/{iterations})", index / (float)iterations);

                // LOAD FRESH FILE IN
                var readerIn = new BinaryReader(stream);
                var sceneIn = new ColiScene();
                sceneIn.FileName = filename;
                sceneIn.SerializeVerbose = serializeVerbose;
                readerIn.ReadX(ref sceneIn, false);

                // WRITE INTERMEDIARY
                // Write scene out to memory stream...
                var writer = new BinaryWriter(new MemoryStream());
                writer.WriteX(sceneIn);
                writer.Flush();
                writer.BaseStream.Seek(0, SeekOrigin.Begin);

                // On next iteration, use the newly written stream as input
                stream = writer.BaseStream;
            }

            EditorUtility.ClearProgressBar();
        }



        // HACKISH PRINTING OF DATA IN VARUIOUS FORMATS
        public const int NumFunctions = 4;
        public static string PrintData<T>(int index, T data)
            where T : IBinaryAddressable, IBinarySerializable
        {
            if (data == null)
                return "null";

            switch (index)
            {
                case 0: return PrintValue(data);
                case 1: return PrintAddress(data);
                case 2: return PrintStream(data);
                case 3: return PrintHashMD5(data);
                default: throw new NotImplementedException();
            }
        }

        // Methods for printing out data.
        public static string PrintHashMD5<T>(T binarySerializable)
            where T : IBinarySerializable, IBinaryAddressable
        {
            return $"MD5 Hash: {HashUtility.HashBinary(MD5.Create(), binarySerializable)}";
        }
        public static string PrintAddress<T>(T binarySerializable)
            where T : IBinarySerializable, IBinaryAddressable
        {
            return binarySerializable.PrintAddressRange();
        }
        public static string PrintValue<T>(T binarySerializable)
            where T : IBinarySerializable, IBinaryAddressable
        {
            return binarySerializable.ToString();
        }
        public static string PrintStream<T>(T binarySerializable)
            where T : IBinarySerializable, IBinaryAddressable
        {
            return $"Stream: {HashUtility.SerializableToStreamString(binarySerializable)}";
        }
        public static string PrintIndex(int i, Array array)
        {
            var iFormat = i.ArrayFormat(array);
            return $"[{iFormat}]\t";
        }
        //


        public static void LogColiScene(ColiScene coliScene, TextLogger log, int functionIdx)
        {
            // Write some metadata
            var venueName = CourseUtility.GetVenueName(coliScene.ID);
            log.WriteLine($"Stage: {venueName} [{coliScene.CourseName}]");
            log.WriteLine($"Stage ID: {coliScene.ID}");
            log.WriteLine($"Stage Author: {coliScene.Author}");
            log.WriteLine($"Format: {coliScene.Format}");
            log.WriteLine($"File Size: {coliScene.FileSize:n0} bytes");
            log.WriteLine();

            // TRACK DATA
            {
                log.WriteLine($"{nameof(TrackNode)}");
                for (int i = 0; i < coliScene.trackNodes.Length; i++)
                {
                    var trackNode = coliScene.trackNodes[i];
                    log.Write(PrintIndex(i, coliScene.trackNodes));
                    log.WriteLine(PrintData(functionIdx, trackNode));
                }
                log.WriteLine();

                log.WriteLine($"{nameof(TrackCheckpoint)}");
                for (int i = 0; i < coliScene.trackNodes.Length; i++)
                {
                    var trackNode = coliScene.trackNodes[i];
                    var iFormat = i.ArrayFormat(coliScene.trackNodes);

                    for (int j = 0; j < trackNode.checkpoints.Length; j++)
                    {
                        var checkpoint = trackNode.checkpoints[j];
                        var jFormat = j.ArrayFormat(trackNode.checkpoints);
                        // Write index, string to log
                        log.Write($"[{iFormat},{jFormat}]\t");
                        log.WriteLine(PrintData(functionIdx, checkpoint));
                    }
                }
                log.WriteLine();

                //
                log.WriteLine($"{nameof(TrackSegment)}");
                for (int i = 0; i < coliScene.allTrackSegments.Length; i++)
                {
                    var trackSegment = coliScene.allTrackSegments[i];
                    log.Write(PrintIndex(i, coliScene.trackNodes));
                    log.WriteLine(PrintData(functionIdx, trackSegment));
                }
                log.WriteLine();

                //
                log.WriteLine($"{nameof(TrackSegment)}.{nameof(TrackSegment.trackAnimationCurves)}");
                string[] labelSRP = new string[] { "Sca", "Rot", "Pos" }; // scale, rotation, position
                string[] labelXYZ = new string[] { "x", "y", "z" };
                for (int segmentIndex = 0; segmentIndex < coliScene.allTrackSegments.Length; segmentIndex++)
                {
                    var trackSegment = coliScene.allTrackSegments[segmentIndex];
                    var segmentIndexFormat = segmentIndex.ArrayFormat(coliScene.allTrackSegments);
                    log.WriteLine($"[{segmentIndex}]\t");

                    for (int animIndex = 0; animIndex < trackSegment.trackAnimationCurves.animationCurves.Length; animIndex++)
                    {
                        var animCurve = trackSegment.trackAnimationCurves.animationCurves[animIndex];
                        // NOTE: delete. At most 4, so no 2 digit indexes
                        //var animCurveFormat = segmentIndex.ArrayFormat(coliScene.trackNodes);
                        var currLabelSRP = labelSRP[animIndex / 3];
                        var currLabelXYZ = labelXYZ[animIndex % 3];
                        log.Write($"[{segmentIndexFormat},{animIndex}]\t");
                        log.Write($"{currLabelSRP}.{currLabelXYZ}\t");
                        log.WriteLine(PrintData(functionIdx, animCurve));
                    }
                }
                log.WriteLine();

                //
                log.WriteLine($"{nameof(TrackSegment)}.{nameof(TrackCorner)}");
                for (int i = 0; i < coliScene.allTrackSegments.Length; i++)
                {
                    var cornerTopology = coliScene.allTrackSegments[i].trackCorner;
                    var iFormat = i.ArrayFormat(coliScene.trackNodes);
                    log.Write($"[{iFormat}]\t");
                    if (cornerTopology != null)
                        log.WriteLine(PrintData(functionIdx, cornerTopology));
                    else
                        log.WriteLine("null");
                }
                log.WriteLine();
            }

            // Scene Object Dynamic + internal data
            {
                log.WriteLine($"{nameof(SceneObjectDynamic)}");
                for (int i = 0; i < coliScene.dynamicSceneObjects.Length; i++)
                {
                    var sceneObject = coliScene.dynamicSceneObjects[i];
                    log.Write(PrintIndex(i, coliScene.dynamicSceneObjects));
                    log.WriteLine(PrintData(functionIdx, sceneObject));
                }
                log.WriteLine();

                // TODO: add indexes
                log.WriteLine($"{nameof(AnimationClipCurve)}");
                for (int i = 0; i < coliScene.dynamicSceneObjects.Length; i++)
                {
                    var animClip = coliScene.dynamicSceneObjects[i].animation;

                    if (animClip == null)
                        continue;

                    var iFormat = i.ArrayFormat(coliScene.dynamicSceneObjects);
                    log.WriteLine($"[{iFormat}]\t");

                    for (int j = 0; j < animClip.curve.Length; j++)
                    {
                        var animCurvesPlus = animClip.curve[j];
                        //log.Write(PrintIndex(i, coliScene.sceneObjects));
                        log.WriteLine(PrintData(functionIdx, animCurvesPlus));
                    }
                }
                log.WriteLine();

                // TODO: add (print???) indexes
                log.WriteLine($"{nameof(TextureMetadata)}");
                for (int i = 0; i < coliScene.dynamicSceneObjects.Length; i++)
                {
                    var textureMetadata = coliScene.dynamicSceneObjects[i].textureMetadata;
                    if (textureMetadata == null)
                        continue;
                    log.Write(PrintIndex(i, coliScene.dynamicSceneObjects));
                    log.WriteLine(PrintData(functionIdx, textureMetadata));
                }
                log.WriteLine();

                log.WriteLine($"{nameof(SkeletalAnimator)}");
                for (int i = 0; i < coliScene.dynamicSceneObjects.Length; i++)
                {
                    var skeletalAnimator = coliScene.dynamicSceneObjects[i].skeletalAnimator;
                    if (skeletalAnimator == null)
                        continue;
                    log.Write(PrintIndex(i, coliScene.dynamicSceneObjects));
                    log.WriteLine(PrintData(functionIdx, skeletalAnimator));
                }
                log.WriteLine();

                log.WriteLine($"{nameof(TransformMatrix3x4)}");
                for (int i = 0; i < coliScene.dynamicSceneObjects.Length; i++)
                {
                    var matrix = coliScene.dynamicSceneObjects[i].transformMatrix3x4;
                    if (matrix == null)
                        continue;
                    log.Write(PrintIndex(i, coliScene.dynamicSceneObjects));
                    log.WriteLine(PrintData(functionIdx, matrix));
                }
                log.WriteLine();
            }

            // Scene Object Dynamic Reference + internal data
            {
                log.WriteLine($"{nameof(SceneObjectTemplate)}");
                for (int i = 0; i < coliScene.templateSceneObjects.Length; i++)
                {
                    var sceneInstance = coliScene.templateSceneObjects[i];
                    log.Write(PrintIndex(i, coliScene.templateSceneObjects));
                    log.WriteLine(PrintData(functionIdx, sceneInstance));
                }
                log.WriteLine();

                log.WriteLine($"{nameof(ColliderGeometry)}");
                for (int i = 0; i < coliScene.templateSceneObjects.Length; i++)
                {
                    var colliderGeometry = coliScene.templateSceneObjects[i].colliderGeometry;
                    if (colliderGeometry == null)
                        continue;
                    log.Write(PrintIndex(i, coliScene.templateSceneObjects));
                    log.WriteLine(PrintData(functionIdx, colliderGeometry));
                }
                log.WriteLine();

                log.WriteLine($"{nameof(SceneObjectTemplate)}");
                for (int i = 0; i < coliScene.sceneObjects.Length; i++)
                {
                    var sceneObjectReference = coliScene.sceneObjects[i];
                    log.Write(PrintIndex(i, coliScene.sceneObjects));
                    log.WriteLine(PrintData(functionIdx, sceneObjectReference));
                }
                log.WriteLine();

                log.WriteLine($"{nameof(CString)}");
                for (int i = 0; i < coliScene.sceneObjectNames.Length; i++)
                {
                    var objectName = coliScene.sceneObjectNames[i];
                    log.Write(PrintIndex(i, coliScene.sceneObjectNames));
                    log.WriteLine(PrintData(functionIdx, objectName));
                }
                log.WriteLine();
            }
        }
    }

}
