using GameCube.GFZ;
using GameCube.GFZ.Stage;
using Manifold.Conversion;
using Manifold.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

using System.Diagnostics;


namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public static class ColiCourseIO
    {
        // CONSTANTS
        private const string TestLoad = "Load All Stages";
        private const string TestSave = "Save All Stages";
        private const string ActiveRoot = " (Active Root)";
        private const string DebugAllRegions = " (Debug All Regions)";

        /// <summary>
        /// Loads all stages at the designated <paramref name="path"/> in iterative pattern,
        /// only loads one stages, passes it, than yields until next iteration.
        /// </summary>
        /// <param name="path">Path to load all stages from.</param>
        /// <param name="processTitle">Title of the dialog window.</param>
        /// <returns>ColiScene one at a time during iteration.</returns>
        public static IEnumerable<Scene> LoadAllStages(string path, string processTitle)
        {
            // Get record of all available files
            var filePaths = new List<string>();

            // Iterate over directory to get files
            // Const 256 = max stages addressable (FF isn't addressable, too).
            for (int i = 0; i < 256; i++)
            {
                // Uncompressed file names
                string filePath = $"{path}/COLI_COURSE{i:d2}";

                // Record files names which exist
                if (File.Exists(filePath))
                    filePaths.Add(filePath);
            }

            int length = filePaths.Count;
            for (int i = 0; i < length; i++)
            {
                // Progress bar
                var fileName = Path.GetFileName(filePaths[i]);
                var info = $"[{i + 1}/{length}] {fileName}";
                var progress = (float)i / length;
                var cancel = EditorUtility.DisplayCancelableProgressBar(processTitle, info, progress);
                if (cancel)
                    break;

                // Read scene file
                var scene = LoadScene(filePaths[i]);

                // Return next scene
                yield return scene;
            }

            EditorUtility.ClearProgressBar();
        }


        public static Scene LoadScene(string filePath)
        {
            var reader = new BinaryReader(File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read));
            var scene = new Scene();
            var fileName = Path.GetFileName(filePath);
            scene.FileName = fileName;
            scene.Deserialize(reader);

            // TODO: assign metadata properly, like author if AV stage, etc.

            return scene;
        }


        #region Test Load All Stages

        public static void TestLoadAllStages(string title, string path)
        {
            // TODO: should use AddressLogBinaryReader. Complications with
            // the iterator and making it generic.

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            // This loads all stages but does nothing with them
            foreach (var _ in LoadAllStages(path, title))
            { }

            stopwatch.Stop();
            UnityEngine.Debug.Log(stopwatch.ElapsedMilliseconds / 1000.0);
        }

        [MenuItem(Const.Menu.tests + "Load Stage (Single)")]
        public static void TestLoadSingle()
        {
            var settings = GfzProjectWindow.GetSettings();
            var root = settings.RootFolder;
            var path = $"{root}/stage/";
            var file = EditorUtility.OpenFilePanel("Select Scene", path, "");

            if (string.IsNullOrEmpty(file))
                return;

            LoadScene(file);

            DebugConsole.Log("Test single complete!");
        }

        [MenuItem(Const.Menu.tests + TestLoad + ActiveRoot + " _F6")]
        public static void TestLoadAllStages()
        {
            var settings = GfzProjectWindow.GetSettings();
            var root = settings.RootFolder;
            var path = $"{root}/stage/";
            TestLoadAllStages(TestLoad, path);
        }

        [MenuItem(Const.Menu.tests + TestLoad + DebugAllRegions)]
        public static void TestLoadAllStagesDebugAllRegions()
        {
            var settings = GfzProjectWindow.GetSettings();
            var directories = settings.GetTestRootDirectories();
            foreach (var directory in directories)
            {
                var path = $"{directory}/stage/";
                TestLoadAllStages(TestLoad, path);
            }
        }

        [MenuItem("Manifold/Reset Prompt #F8")]
        public static void ClearProgBar()
        {
            EditorUtility.ClearProgressBar();
        }

        #endregion

        #region Test Save All Stages

        public static void TestSaveAllStagesDisk(string title, string path, string dest)
        {
            // Iterate over all stages, serialize them to RAM.
            foreach (var coliScene in LoadAllStages(path, title))
            {
                var outputPath = dest + "stage/";
                var outputFile = outputPath + coliScene.FileName;
                var outputLog = outputPath + coliScene.FileName + "-log.txt";
                using (var writer = new BinaryWriter(File.Create(outputFile)))
                {
                    // Serialize the scene data to file
                    coliScene.SerializeVerbose = true;
                    writer.WriteX(coliScene);
                    writer.Flush();
                }

                // Write a log of the output file
                var log = new TextLogger(outputLog);
                LogSceneData(log, coliScene);
                log.Flush();
                log.Close();

                // Compress and make an LZed copy
                LzUtility.CompressAvLzToDisk(outputFile, LibGxFormat.AvGame.FZeroGX, true);
            }
            OSUtility.OpenDirectory(dest);
        }

        [MenuItem(Const.Menu.tests + TestSave + " To Disk" + ActiveRoot + " _F7")]
        public static void Temp()
        {
            var settings = GfzProjectWindow.GetSettings();
            var inputPath = settings.StageDir;
            var outputPath = settings.FileOutput;
            var title = $"Writing {nameof(Scene)} to disk...";
            TestSaveAllStagesDisk(title, inputPath, outputPath);
        }

        public static void TestSaveAllStagesMemory(string title, string path)
        {
            // Iterate over all stages, serialize them to RAM.
            foreach (var coliScene in LoadAllStages(path, title))
            {
                var writer = new AddressLogBinaryWriter(new MemoryStream());
                writer.WriteX(coliScene);
            }
        }

        [MenuItem(Const.Menu.tests + TestSave + ActiveRoot)]
        public static void TestSaveAllStagesMemory()
        {
            var settings = GfzProjectWindow.GetSettings();
            var root = settings.RootFolder;
            var path = $"{root}/stage/";
            TestSaveAllStagesMemory(TestSave, path);
        }

        [MenuItem(Const.Menu.tests + TestSave + DebugAllRegions)]
        public static void TestSaveAllStagesMemoryDebugAllRegions()
        {
            var settings = GfzProjectWindow.GetSettings();
            var directories = settings.GetTestRootDirectories();
            foreach (var directory in directories)
            {
                var path = $"{directory}/stage/";
                TestSaveAllStagesMemory(TestLoad, path);
            }
        }

        #endregion


        #region Roundtrip

        [MenuItem("Manifold/Tests/Roundtrip Test (Active Dir) _F8")]
        public static void RoundtripScenes()
        {
            var settings = GfzProjectWindow.GetSettings();
            var path = settings.StageDir;
            var title = "Roundtrip Test";
            Scene sceneWrite;
            var logPath = settings.LogOutput;

            // This loads all stages but does nothing with them
            foreach (var coliScene in LoadAllStages(path, title))
            {
                // Assign scene to temp, get initial hash
                sceneWrite = coliScene;
                var logInit = new TextLogger($"{logPath}log-init-{coliScene.FileName}.txt");
                LogSceneData(logInit, coliScene);
                logInit.Close();

                for (int i = 0; i < 2; i++)
                {
                    var logWrite = new TextLogger($"{logPath}log-in-out-{sceneWrite.FileName}-[{i}]-write.txt");
                    var logRead = new TextLogger($"{logPath}log-in-out-{sceneWrite.FileName}-[{i}]-read.txt");

                    // Write scene to buffer
                    var buffer = new MemoryStream();
                    var writer = new BinaryWriter(buffer);
                    writer.WriteX(sceneWrite);
                    writer.Flush();
                    //
                    var fsw = File.Create(logWrite.Path + ".bin");
                    fsw.Write(buffer.ToArray(), 0, (int)buffer.Length);
                    fsw.Flush();
                    fsw.Close();
                    buffer.Seek(0, SeekOrigin.Begin);
                    //
                    LogSceneData(logWrite, sceneWrite);
                    logWrite.Close();
                    // Reset buffer address
                    buffer.Seek(0, SeekOrigin.Begin);
                    // Create new scene to read buffer into
                    var sceneRead = new Scene();
                    sceneRead.SerializeVerbose = true;
                    sceneRead.FileName = sceneWrite.FileName;
                    // Read buffer, think of it like File.Open
                    var reader = new BinaryReader(buffer);
                    sceneRead.Deserialize(reader);
                    buffer.Seek(0, SeekOrigin.Begin);
                    //
                    var fsr = File.Create(logRead.Path + ".bin");
                    fsr.Write(buffer.ToArray(), 0, (int)buffer.Length);
                    fsr.Flush();
                    fsr.Close();
                    buffer.Seek(0, SeekOrigin.Begin);
                    //
                    LogSceneData(logRead, sceneRead);
                    logRead.Close();
                    //
                    sceneWrite = sceneRead;
                }

                //break;
            }

            EditorUtility.ClearProgressBar();
        }

        #endregion

        // TODO: roundtrip with AddressLog*
        // TODO: log output for 4 types (but maybe not all 4)

        // TODO: make this a function
        //var sceneIndex = scene.ID;
        //var sceneHash = md5.ComputeHash(readerIn.BaseStream);
        //var sceneHashStr = HashUtility.ByteArrayToString(sceneHash);
        //var romHashStr = HashLibrary.ColiCourseMD5_GFZJ01[sceneIndex];
        //        if (romHashStr == sceneHashStr)
        //        {
        //            scene.Venue = CourseUtility.GetVenue(sceneIndex);
        //            scene.CourseName = CourseUtility.GetCourseName(sceneIndex);
        //            scene.Author = "Amusement Vision";
        //        }

        public static void LogRoundtrip(string filePath, bool serializeVerbose)
        {
            // TEMP from previous function
            var openFolderAfterExport = true;
            //var allowOverwritingFiles = true;
            var exportTo = "W:/Windows Directories/Desktop/test";
            /////////////////////////////////////////////////////

            // Construct time stamp string
            var dateTime = DateTime.Now;
            var timestamp = $"[{dateTime:yyyy-MM-dd}][{dateTime:HH-mm-ss}]";

            // still valid??
            //ColiCourseUtility.SerializeVerbose = serializeVerbose;


            // LOAD FRESH FILE IN
            var readerIn = new BinaryReader(File.OpenRead(filePath));
            var sceneIn = new Scene();
            sceneIn.FileName = Path.GetFileName(filePath);
            sceneIn.SerializeVerbose = serializeVerbose;
            sceneIn.Deserialize(readerIn);
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
            var sceneOut = new Scene();
            sceneOut.SerializeVerbose = serializeVerbose;
            sceneOut.FileName = sceneIn.FileName;
            sceneOut.Deserialize(readerOut);
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

        public static void LogColiScene(Scene coliScene, TextLogger log, int functionIdx)
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

                log.WriteLine($"{nameof(Checkpoint)}");
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
                log.WriteLine($"{nameof(TrackSegment)}.{nameof(TrackSegment.AnimationCurveTRS)}");
                string[] labelSRP = new string[] { "Sca", "Rot", "Pos" }; // scale, rotation, position
                string[] labelXYZ = new string[] { "x", "y", "z" };
                for (int segmentIndex = 0; segmentIndex < coliScene.allTrackSegments.Length; segmentIndex++)
                {
                    var trackSegment = coliScene.allTrackSegments[segmentIndex];
                    var segmentIndexFormat = segmentIndex.ArrayFormat(coliScene.allTrackSegments);
                    log.WriteLine($"[{segmentIndex}]\t");

                    for (int animIndex = 0; animIndex < trackSegment.AnimationCurveTRS.AnimationCurves.Length; animIndex++)
                    {
                        var animCurve = trackSegment.AnimationCurveTRS.AnimationCurves[animIndex];
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
                    var cornerTopology = coliScene.allTrackSegments[i].TrackCorner;
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
                    var animClip = coliScene.dynamicSceneObjects[i].AnimationClip;

                    if (animClip == null)
                        continue;

                    var iFormat = i.ArrayFormat(coliScene.dynamicSceneObjects);
                    log.WriteLine($"[{iFormat}]\t");

                    for (int j = 0; j < animClip.Curves.Length; j++)
                    {
                        var animCurvesPlus = animClip.Curves[j];
                        //log.Write(PrintIndex(i, coliScene.sceneObjects));
                        log.WriteLine(PrintData(functionIdx, animCurvesPlus));
                    }
                }
                log.WriteLine();

                // TODO: add (print???) indexes
                log.WriteLine($"{nameof(TextureScroll)}");
                for (int i = 0; i < coliScene.dynamicSceneObjects.Length; i++)
                {
                    var textureMetadata = coliScene.dynamicSceneObjects[i].TextureScroll;
                    if (textureMetadata == null)
                        continue;
                    log.Write(PrintIndex(i, coliScene.dynamicSceneObjects));
                    log.WriteLine(PrintData(functionIdx, textureMetadata));
                }
                log.WriteLine();

                log.WriteLine($"{nameof(SkeletalAnimator)}");
                for (int i = 0; i < coliScene.dynamicSceneObjects.Length; i++)
                {
                    var skeletalAnimator = coliScene.dynamicSceneObjects[i].SkeletalAnimator;
                    if (skeletalAnimator == null)
                        continue;
                    log.Write(PrintIndex(i, coliScene.dynamicSceneObjects));
                    log.WriteLine(PrintData(functionIdx, skeletalAnimator));
                }
                log.WriteLine();

                log.WriteLine($"{nameof(TransformMatrix3x4)}");
                for (int i = 0; i < coliScene.dynamicSceneObjects.Length; i++)
                {
                    var matrix = coliScene.dynamicSceneObjects[i].TransformMatrix3x4;
                    if (matrix == null)
                        continue;
                    log.Write(PrintIndex(i, coliScene.dynamicSceneObjects));
                    log.WriteLine(PrintData(functionIdx, matrix));
                }
                log.WriteLine();
            }

            // Scene Object Dynamic Reference + internal data
            {
                log.WriteLine($"{nameof(SceneObject)}");
                for (int i = 0; i < coliScene.sceneObjects.Length; i++)
                {
                    var sceneInstance = coliScene.sceneObjects[i];
                    log.Write(PrintIndex(i, coliScene.sceneObjects));
                    log.WriteLine(PrintData(functionIdx, sceneInstance));
                }
                log.WriteLine();

                log.WriteLine($"{nameof(ColliderMesh)}");
                for (int i = 0; i < coliScene.sceneObjects.Length; i++)
                {
                    var colliderGeometry = coliScene.sceneObjects[i].ColliderMesh;
                    if (colliderGeometry == null)
                        continue;
                    log.Write(PrintIndex(i, coliScene.sceneObjects));
                    log.WriteLine(PrintData(functionIdx, colliderGeometry));
                }
                log.WriteLine();

                log.WriteLine($"{nameof(SceneObject)}");
                for (int i = 0; i < coliScene.sceneObjectLODs.Length; i++)
                {
                    var sceneObjectReference = coliScene.sceneObjectLODs[i];
                    log.Write(PrintIndex(i, coliScene.sceneObjectLODs));
                    log.WriteLine(PrintData(functionIdx, sceneObjectReference));
                }
                log.WriteLine();

                log.WriteLine($"{nameof(ShiftJisCString)}");
                for (int i = 0; i < coliScene.sceneObjectNames.Length; i++)
                {
                    var objectName = coliScene.sceneObjectNames[i];
                    log.Write(PrintIndex(i, coliScene.sceneObjectNames));
                    log.WriteLine(PrintData(functionIdx, objectName));
                }
                log.WriteLine();
            }
        }
        public static void LogSceneData(TextLogger log, Scene coliScene)
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
            log.WriteLine($"{nameof(Bool32)}: {coliScene.staticColliderMeshManagerActive}");
            log.WriteLine($"{nameof(Bool32)}: {coliScene.unkBool32_0x58}");
            log.WriteLine($"{nameof(coliScene.unkRange0x00)}: {coliScene.unkRange0x00}");
            log.WriteLine(); //
            log.WriteLine(); // yes, 2 WriteLines

            log.WriteHeading("FOG", padding, h1Width);
            log.WriteAddress(coliScene.fog);
            log.WriteAddress(coliScene.fogCurves);
            log.WriteLine();

            log.WriteHeading("TRIGGERS", padding, h1Width);
            log.WriteAddress(coliScene.timeExtensionTriggers);
            log.WriteAddress(coliScene.miscellaneousTriggers);
            log.WriteAddress(coliScene.storyObjectTriggers);
            log.WriteAddress(coliScene.unknownTriggers);
            log.WriteAddress(coliScene.visualEffectTriggers);

            // Writes non-array track data
            log.WriteHeading("TRACK DATA", padding, h1Width);
            log.WriteAddress(coliScene.trackLength);
            log.WriteAddress(coliScene.trackMinHeight);
            log.WriteAddress(coliScene.checkpointGridXZ);
            log.WriteLine();
            // Writes track objects array
            log.WriteAddress(coliScene.embeddedPropertyAreas);
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
            log.WriteLine($"{nameof(TrackSegment)}.{nameof(TrackSegment.AnimationCurveTRS)}");
            string[] labelSRP = new string[] { "Sca", "Rot", "Pos" };
            string[] labelXYZ = new string[] { "x", "y", "z" };
            for (int segmentIndex = 0; segmentIndex < coliScene.allTrackSegments.Length; segmentIndex++)
            {
                var trackSegment = coliScene.allTrackSegments[segmentIndex];
                log.WriteLine($"{nameof(TrackSegment)}[{segmentIndex}]\t{trackSegment}");

                for (int animIndex = 0; animIndex < trackSegment.AnimationCurveTRS.AnimationCurves.Length; animIndex++)
                {
                    var animCurve = trackSegment.AnimationCurveTRS.AnimationCurves[animIndex];
                    var currLabelSRP = labelSRP[animIndex / 3];
                    var currLabelXYZ = labelXYZ[animIndex % 3];
                    log.WriteLine($"{currLabelSRP}.{currLabelXYZ} [{animIndex}] ");
                    log.WriteArrayToString(animCurve.KeyableAttributes);
                    //log.WriteLine(HashSerializables.Hash(md5, animCurve));
                    log.WriteLine();
                }
            }

            //
            for (int i = 0; i < coliScene.allTrackSegments.Length; i++)
            {
                var segment = coliScene.allTrackSegments[i];
                log.WriteLine($"{nameof(TrackSegment)} Transform Coords [{i}]");
                log.WriteLine($"\tPosition: {segment.LocalPosition}");
                log.WriteLine($"\tRotation: {segment.LocalRotation}");
                log.WriteLine($"\tScale...: {segment.LocalScale}");
            }
            log.WriteLine();
            //
            log.WriteAddress(coliScene.trackCheckpointGrid);
            log.WriteAddress(coliScene.trackNodes);
            {
                var checkpoints = new List<Checkpoint>();
                foreach (var trackNode in coliScene.trackNodes)
                    foreach (var checkpoint in trackNode.checkpoints)
                        checkpoints.Add(checkpoint);
                log.WriteAddress(checkpoints.ToArray());
            }
            log.WriteLine();

            log.WriteHeading("STATIC COLLISION", padding, h1Width);
            log.WriteAddress(coliScene.staticColliderMeshManager);
            log.WriteLine();
            log.WriteLine(nameof(UnknownCollider) + "[]");
            log.WriteAddress(coliScene.unknownColliders);
            log.WriteLine();
            log.WriteLine(nameof(GameCube.GFZ.BoundingSphere));
            log.WriteLine("unk float: " + coliScene.staticColliderMeshManager.Unk_float);
            log.WriteAddress(coliScene.staticColliderMeshManager.BoundingSphere);
            log.WriteLine(coliScene.staticColliderMeshManager.UnknownCollidersPtr);
            log.WriteLine(coliScene.staticColliderMeshManager.StaticSceneObjectsPtr);
            log.WriteLine();
            log.WriteLine("Mesh Bounds");
            log.WriteAddress(coliScene.staticColliderMeshManager.MeshGridXZ);
            log.WriteLine();
            log.WriteLine("TRIANGLES");
            log.WriteAddress(coliScene.staticColliderMeshManager.ColliderTris);
            log.WriteAddress(coliScene.staticColliderMeshManager.TriMeshGrids);
            // Write each index list
            log.WriteNullInArray = false;
            for (int i = 0; i < coliScene.staticColliderMeshManager.TriMeshGrids.Length; i++)
            {
                var triIndexList = coliScene.staticColliderMeshManager.TriMeshGrids[i];
                if (triIndexList != null)
                {
                    log.WriteLine($"COLLIDER TYPE [{i}]: {(StaticColliderMeshProperty)i}");
                    log.WriteAddress(triIndexList.IndexLists);
                }
            }
            log.WriteNullInArray = true;
            log.WriteLine("QUADS");
            log.WriteAddress(coliScene.staticColliderMeshManager.ColliderQuads);
            log.WriteAddress(coliScene.staticColliderMeshManager.QuadMeshGrids);
            // Write each index list
            log.WriteNullInArray = false;
            for (int i = 0; i < coliScene.staticColliderMeshManager.QuadMeshGrids.Length; i++)
            {
                var quadIndexList = coliScene.staticColliderMeshManager.QuadMeshGrids[i];
                if (quadIndexList != null)
                {
                    log.WriteLine($"COLLIDER TYPE [{i}]: {(StaticColliderMeshProperty)i}");
                    log.WriteAddress(quadIndexList.IndexLists);
                }
            }
            log.WriteNullInArray = true;
            log.WriteLine();

            log.WriteHeading("SCENE OBJECTS", padding, h1Width);
            log.WriteLine("Object Names");
            log.WriteAddress(coliScene.sceneObjectNames);
            log.WriteAddress(coliScene.sceneObjectLODs);
            log.WriteAddress(coliScene.sceneObjects);
            log.WriteAddress(coliScene.staticSceneObjects);
            log.WriteAddress(coliScene.dynamicSceneObjects);
            {
                log.WriteHeading("FULL OBJECT SUMMARY", padding, h1Width);
                int total = coliScene.dynamicSceneObjects.Length;
                for (int i = 0; i < total; i++)
                {
                    var sceneObject = coliScene.dynamicSceneObjects[i];

                    log.WriteLine($"[{i}/{total}] {sceneObject.Name}");
                    log.WriteAddress(sceneObject);
                    log.WriteAddress(sceneObject.SceneObject);
                    log.WriteAddress(sceneObject.SceneObject.LODs);
                    log.WriteAddress(sceneObject.SceneObject.PrimaryLOD.Name);
                    log.WriteAddress(sceneObject.TransformMatrix3x4);

                    log.WriteAddress(sceneObject.SkeletalAnimator);
                    if (sceneObject.SkeletalAnimator != null)
                        log.WriteAddress(sceneObject.SkeletalAnimator.Properties);

                    log.WriteAddress(sceneObject.AnimationClip);
                    if (sceneObject.AnimationClip != null)
                        log.WriteAddress(sceneObject.AnimationClip.Curves);
                    // TODO: other sub classes?

                    log.WriteAddress(sceneObject.TextureScroll);
                    if (sceneObject.TextureScroll != null)
                        log.WriteAddress(sceneObject.TextureScroll.Fields);

                    log.WriteLine();
                }
            }
            log.WriteLine();
            log.Flush();
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
            var sceneIn = new Scene();
            sceneIn.FileName = Path.GetFileName(filePath);
            sceneIn.SerializeVerbose = serializeVerbose;
            sceneIn.Deserialize(readerIn);
            // Check to see if file is from ROM. Add correct metadata.
            // TODO: remove hardcoded match and use dynamic JP/EN/PAL
            readerIn.SeekBegin();
            var sceneIndex = sceneIn.ID;
            var sceneHash = md5.ComputeHash(readerIn.BaseStream);
            var sceneHashStr = HashUtility.ByteArrayToString(sceneHash);
            var romHashStr = MD5FileHashes.ColiCourse_GFZJ01[sceneIndex];
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
            var sceneOut = new Scene();
            sceneOut.SerializeVerbose = serializeVerbose;
            sceneOut.FileName = sceneIn.FileName;
            sceneOut.Deserialize(readerOut);
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
                var sceneIn = new Scene();
                sceneIn.FileName = filename;
                sceneIn.SerializeVerbose = serializeVerbose;
                sceneIn.Deserialize(readerIn);

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
        public static void TestHash(TextLogger log, Scene coliScene)
        {
            var md5 = MD5.Create();
            foreach (var trackSegment in coliScene.allTrackSegments)
            {
                log.WriteLine(HashUtility.HashBinary(md5, trackSegment));
            }
        }





        public static void ExportUnityScenes(Scene.SerializeFormat format, bool serializeVerbose, params SceneAsset[] scenes)
        {
            // TEMP from previous function
            //var serializeVerbose = true;
            var openFolderAfterExport = true;
            var allowOverwritingFiles = true;
            var exportTo = "C:/test";
            /////////////////////////////////////////////////////

            //ColiCourseUtility.SerializeVerbose = serializeVerbose;

            // Construct ColiScene from Unity Scene "scenes"
            var coliScenes = new Scene[scenes.Length];

            // Iterate over each scene asset
            for (int sceneIndex = 0; sceneIndex < coliScenes.Length; sceneIndex++)
            {
                // DEBUG
                const bool canFindInactive = true;
                const int h1Width = 96;
                const int h2Width = 32;
                const string padding = "-";

                // Initialize new instance
                coliScenes[sceneIndex] = new Scene();

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
                    coliScene.unkRange0x00 = new ViewRange(sceneParams.rangeNear, sceneParams.rangeFar);
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
                    coliScene.staticColliderMeshManager = new StaticColliderMeshManager(format);
                }

                // Triggers
                {
                    var arcadeCheckpointTriggers = GameObject.FindObjectsOfType<GfzTimeExtensionTrigger>(canFindInactive);
                    coliScene.timeExtensionTriggers = GetGfzValues(arcadeCheckpointTriggers);

                    // This trigger type is a mess... Get all 3 representations, combine, assign.
                    // Collect all trigger types. They all get converted to the same GFZ base type.
                    var objectPaths = GameObject.FindObjectsOfType<GfzObjectPath>(canFindInactive);
                    var storyCapsules = GameObject.FindObjectsOfType<GfzStoryCapsule>(canFindInactive);
                    var unknownMetadataTriggers = GameObject.FindObjectsOfType<GfzUnknownMiscellaneousTrigger>(canFindInactive);
                    // Make a list, add range for each type
                    var courseMetadataTriggers = new List<MiscellaneousTrigger>();
                    courseMetadataTriggers.AddRange(GetGfzValues(objectPaths));
                    courseMetadataTriggers.AddRange(GetGfzValues(storyCapsules));
                    courseMetadataTriggers.AddRange(GetGfzValues(unknownMetadataTriggers));
                    // Convert list to array, assign to ColiScene
                    coliScene.miscellaneousTriggers = courseMetadataTriggers.ToArray();

                    // TODO:
                    // story object triggers

                    // This trigger type is a mess... Get all 3 representations, combine, assign.
                    var unknownTriggers = GameObject.FindObjectsOfType<GfzCullOverrideTrigger>(canFindInactive);
                    coliScene.unknownTriggers = GetGfzValues(unknownTriggers);

                    var unknownSolsTriggers = GameObject.FindObjectsOfType<GfzUnknownCollider>(canFindInactive);
                    coliScene.unknownColliders = GetGfzValues(unknownSolsTriggers);

                    var visualEffectTriggers = GameObject.FindObjectsOfType<GfzVisualEffectTrigger>(canFindInactive);
                    coliScene.visualEffectTriggers = GetGfzValues(visualEffectTriggers);


                    log.WriteLine("TRIGGERS");
                    log.WriteLineSummary(coliScene.timeExtensionTriggers);
                    // Log. TODO: more granularity in type.
                    log.WriteLineSummary(coliScene.miscellaneousTriggers);
                    log.WriteLineSummary(coliScene.unknownTriggers);
                    log.WriteLineSummary(coliScene.unknownColliders);
                    log.WriteLineSummary(coliScene.visualEffectTriggers);
                    log.WriteLine();
                }

                // Scene Objects / Instances / References / Names
                {
                    var sceneObjects = GameObject.FindObjectsOfType<GfzSceneObjectDynamic>(canFindInactive);
                    coliScene.dynamicSceneObjects = new SceneObjectDynamic[0];

                    // TODO: construct the actual objects...

                    log.WriteLine();
                    log.WriteLine("SCENE OBJECTS");
                    log.WriteLineSummary(coliScene.dynamicSceneObjects);
                    log.WriteLineSummary(coliScene.staticSceneObjects);
                    log.WriteLineSummary(coliScene.sceneObjects);
                    log.WriteLineSummary(coliScene.sceneObjectLODs);
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
                    coliScene.trackCheckpointGrid = new TrackCheckpointGrid();

                    // TODO: this is temp data
                    coliScene.trackLength = new TrackLength();
                    coliScene.trackMinHeight = new TrackMinHeight();
                }
                //Debug.LogWarning("TODO: define bounds");

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


    }

}
