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

        //[MenuItem("Manifold/IO/Test All")]
        //public static void TestAll()
        //{
        //    for (int i = 0; i < 51; i++)
        //        try
        //        {
        //            TestImportExportLog($"C:/GFZJ01/stage/COLI_COURSE{i:00}", true);
        //        }
        //        catch (Exception e)
        //        {
        //            Debug.LogError(e.Message);
        //        }
        //}

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
                WriteFullReport(log, sceneIn);
                log.Close();
            }

            // WRITE INTERMEDIARY
            // Write scene out to memory stream...
            var writer = new BinaryWriter(new MemoryStream());
            writer.WriteX(sceneIn);
            writer.Flush();
            writer.BaseStream.Seek(0, SeekOrigin.Begin);
            
            // LOAD INTERMEDIARY
            // Load saved file back in
            var readerOut = new BinaryReader(writer.BaseStream);
            var sceneOut = new ColiScene();
            sceneOut.SerializeVerbose = serializeVerbose;
            sceneOut.FileName = sceneIn.FileName;
            readerOut.ReadX(ref sceneOut, false);
            // Log intermediary
            {
                var logPath = Path.Combine(exportTo, $"{timestamp} - COLI_COURSE{sceneIn.ID:d2} OUT.txt");
                var log = new TextLogger(logPath);
                WriteFullReport(log, sceneIn);
                log.Close();
            }

            // open folder location
            OSUtility.OpenDirectory(openFolderAfterExport, exportTo);
            // Set static variable
            // Export file...
            //var x = ExportUtility.ExportSerializable(sceneIn, exportTo, "", allowOverwritingFiles);
            //OSUtility.OpenDirectory(openFolderAfterExport, x);
        }


        public static void ExportUnityScenes(ColiScene.SerializeFormat format, params SceneAsset[] scenes)
        {
            // TEMP from previous function
            var serializeVerbose = true;
            var openFolderAfterExport = true;
            var allowOverwritingFiles = true;
            var exportTo = "W:/Windows Directories/Desktop/test";
            /////////////////////////////////////////////////////

            ColiCourseUtility.SerializeVerbose = serializeVerbose;

            // Construct ColiScene from Unity Scene "scenes"
            var coliScenes = new ColiScene[scenes.Length];

            // TODO: move to static script?
            // This way you can have editor export instead of through this dang scriptable object!

            for (int sceneIndex = 0; sceneIndex < coliScenes.Length; sceneIndex++)
            {
                // DEBUG
                const bool canFindInactive = true;
                const int h1Width = 96;
                const int h2Width = 32;
                const string padding = "-";

                // 
                coliScenes[sceneIndex] = new ColiScene();

                // Breakout value
                var scene = scenes[sceneIndex];
                var coliScene = coliScenes[sceneIndex];

                // Set serialization format
                coliScene.Format = format;

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
                    coliScene.sceneObjects = new SceneObject[0];

                    // TODO: construct the actual objects...

                    log.WriteLine();
                    log.WriteLine("SCENE OBJECTS");
                    log.WriteLineSummary(coliScene.sceneObjects);
                    log.WriteLineSummary(coliScene.sceneOriginObjects);
                    log.WriteLineSummary(coliScene.sceneInstances);
                    log.WriteLineSummary(coliScene.sceneObjectReferences);
                    log.WriteLineSummary(coliScene.objectNames);
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
                WriteFullReport(log, coliScene);
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
                log.WriteLine(HashSerializables.Hash(md5, trackSegment));
            }
        }
        public static void WriteFullReport(TextLogger log, ColiScene coliScene)
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
            log.WriteLine($"{nameof(BoostPlatesActive)}: {coliScene.boostPlatesActive}");
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
            string[] labelSRP = new string[] { "Scale", "Rotation", "Position" };
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
                    //log.WriteArrayToString(animCurve.keyableAttributes);
                    log.WriteLine(HashSerializables.Hash(md5, animCurve));
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
            log.WriteAddress(coliScene.staticColliderMeshes.ununsedMeshBounds);
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
            log.WriteAddress(coliScene.objectNames);
            log.WriteAddress(coliScene.sceneObjectReferences);
            log.WriteAddress(coliScene.sceneInstances);
            log.WriteAddress(coliScene.sceneOriginObjects);
            log.WriteAddress(coliScene.sceneObjects);
            {
                log.WriteHeading("FULL OBJECT SUMMARY", padding, h1Width);
                int total = coliScene.sceneObjects.Length;
                for (int i = 0; i < total; i++)
                {
                    var sceneObject = coliScene.sceneObjects[i];

                    log.WriteLine($"[{i}/{total}] {sceneObject.nameCopy}");
                    log.WriteAddress(sceneObject);
                    log.WriteAddress(sceneObject.instanceReference);
                    log.WriteAddress(sceneObject.instanceReference.objectReference);
                    log.WriteAddress(sceneObject.instanceReference.objectReference.name);
                    log.WriteAddress(sceneObject.transformMatrix3x4);

                    log.WriteAddress(sceneObject.skeletalAnimator);
                    if (sceneObject.skeletalAnimator != null)
                        log.WriteAddress(sceneObject.skeletalAnimator.properties);

                    log.WriteAddress(sceneObject.animation);
                    if (sceneObject.animation != null)
                        log.WriteAddress(sceneObject.animation.animationCurvePluses);
                    // TODO: other sub classes?

                    log.WriteAddress(sceneObject.unk1);
                    if (sceneObject.unk1 != null)
                        log.WriteAddress(sceneObject.unk1.unk);

                    log.WriteLine();
                }
            }
            log.WriteLine();
        }


    }

}
