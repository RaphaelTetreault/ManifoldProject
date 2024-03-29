using GameCube.AmusementVision;
using GameCube.GFZ;
using GameCube.GFZ.LZ;
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
    [Obsolete]
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
            var reader = new EndianBinaryReader(File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read), Scene.endianness);
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
            var root = settings.SourceDirectory;
            var path = $"{root}/stage/";
            var file = EditorUtility.OpenFilePanel("Select Scene", path, "");

            if (string.IsNullOrEmpty(file))
                return;

            LoadScene(file);

            DebugConsole.Log("Test single complete!");
        }

        [MenuItem(Const.Menu.tests + TestLoad + ActiveRoot)]
        public static void TestLoadAllStages()
        {
            var settings = GfzProjectWindow.GetSettings();
            var root = settings.SourceDirectory;
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

        [MenuItem("Manifold/Reset Prompt")]
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
                using (var writer = new EndianBinaryWriter(File.Create(outputFile), Scene.endianness))
                {
                    // Serialize the scene data to file
                    coliScene.SerializeVerbose = true;
                    writer.Write(coliScene);
                    writer.Flush();
                }

                // Write a log of the output file
                var log = new TextLogger(outputLog);
                LogSceneData(log, coliScene);
                log.Flush();
                log.Close();

                // Compress and make an LZed copy
                LzUtility.CompressAvLzToDisk(outputFile, GxGame.FZeroGX, true);
            }
            OSUtility.OpenDirectory(dest);
        }

        [MenuItem(Const.Menu.tests + TestSave + " To Disk" + ActiveRoot)]
        public static void Temp()
        {
            var settings = GfzProjectWindow.GetSettings();
            var inputPath = settings.SourceStageDirectory;
            var outputPath = settings.FileOutput;
            var title = $"Writing {nameof(Scene)} to disk...";
            TestSaveAllStagesDisk(title, inputPath, outputPath);
        }

        public static void TestSaveAllStagesMemory(string title, string path)
        {
            throw new NotImplementedException();
            //// Iterate over all stages, serialize them to RAM.
            //foreach (var coliScene in LoadAllStages(path, title))
            //{
            //    var writer = new AddressLogBinaryWriter(new MemoryStream());
            //    writer.Write(coliScene);
            //}
        }

        [MenuItem(Const.Menu.tests + TestSave + ActiveRoot)]
        public static void TestSaveAllStagesMemory()
        {
            var settings = GfzProjectWindow.GetSettings();
            var root = settings.SourceDirectory;
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

        [MenuItem("Manifold/Tests/Roundtrip Test (Active Dir)")]
        public static void RoundtripScenes()
        {
            var settings = GfzProjectWindow.GetSettings();
            var path = settings.SourceStageDirectory;
            var title = "Roundtrip Test";
            Scene sceneWrite;
            var logPath = settings.LogOutput;

            // This loads all stages but does nothing with them
            foreach (var coliScene in LoadAllStages(path, title))
            {
                // Assign scene to temp, get initial hash
                sceneWrite = coliScene;
                var logInit = new TextLogger($"{logPath}log-init-{coliScene.FileName}.txt");
                {
                    var builder = new System.Text.StringBuilder();
                    sceneWrite.PrintMultiLine(builder);
                    logInit.Write(builder.ToString());
                    logInit.Close();
                }

                for (int i = 0; i < 2; i++)
                {
                    var logWrite = new TextLogger($"{logPath}log-in-out-{sceneWrite.FileName}-[{i}]-write.txt");
                    var logRead = new TextLogger($"{logPath}log-in-out-{sceneWrite.FileName}-[{i}]-read.txt");

                    // Write scene to buffer
                    var buffer = new MemoryStream();
                    var writer = new EndianBinaryWriter(buffer, Scene.endianness);
                    writer.Write(sceneWrite);
                    writer.Flush();
                    //
                    var fsw = File.Create(logWrite.Path + ".bin");
                    fsw.Write(buffer.ToArray(), 0, (int)buffer.Length);
                    fsw.Flush();
                    fsw.Close();
                    buffer.Seek(0, SeekOrigin.Begin);
                    //
                    {
                        var builder = new System.Text.StringBuilder();
                        sceneWrite.PrintMultiLine(builder);
                        logWrite.Write(builder.ToString());
                        logWrite.Close();
                    }
                    // Reset buffer address
                    buffer.Seek(0, SeekOrigin.Begin);
                    // Create new scene to read buffer into
                    var sceneRead = new Scene();
                    sceneRead.SerializeVerbose = true;
                    sceneRead.FileName = sceneWrite.FileName;
                    // Read buffer, think of it like File.Open
                    var reader = new EndianBinaryReader(buffer, Scene.endianness);
                    sceneRead.Deserialize(reader);
                    buffer.Seek(0, SeekOrigin.Begin);
                    //
                    var fsr = File.Create(logRead.Path + ".bin");
                    fsr.Write(buffer.ToArray(), 0, (int)buffer.Length);
                    fsr.Flush();
                    fsr.Close();
                    buffer.Seek(0, SeekOrigin.Begin);
                    //
                    {
                        var builder = new System.Text.StringBuilder();
                        sceneWrite.PrintMultiLine(builder);
                        logRead.Write(builder.ToString());
                        logRead.Close();
                    }
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
            var readerIn = new EndianBinaryReader(File.OpenRead(filePath), Scene.endianness);
            var sceneIn = new Scene();
            sceneIn.FileName = Path.GetFileName(filePath);
            sceneIn.SerializeVerbose = serializeVerbose;
            sceneIn.Deserialize(readerIn);
            // Log true file
            {
                var logPath = Path.Combine(exportTo, $"{timestamp} - COLI_COURSE{sceneIn.CourseIndex:d2} IN.txt");
                var log = new TextLogger(logPath);
                LogSceneData(log, sceneIn);
                log.Close();
            }

            // WRITE INTERMEDIARY
            // Write scene out to memory stream...
            var writer = new EndianBinaryWriter(new MemoryStream(), Scene.endianness);
            writer.Write(sceneIn);
            writer.Flush();
            writer.BaseStream.Seek(0, SeekOrigin.Begin);

            // LOAD INTERMEDIARY
            // Load memory stream back in
            var readerOut = new EndianBinaryReader(writer.BaseStream, Scene.endianness);
            var sceneOut = new Scene();
            sceneOut.SerializeVerbose = serializeVerbose;
            sceneOut.FileName = sceneIn.FileName;
            sceneOut.Deserialize(readerOut);
            // Log intermediary
            {
                var logPath = Path.Combine(exportTo, $"{timestamp} - COLI_COURSE{sceneIn.CourseIndex:d2} OUT.txt");
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
            var venueName = CourseUtility.GetVenueName(coliScene.CourseIndex);
            log.WriteLine($"Stage: {venueName} [{coliScene.CourseName}]");
            log.WriteLine($"Stage ID: {coliScene.CourseIndex}");
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

                    for (int j = 0; j < trackNode.Checkpoints.Length; j++)
                    {
                        var checkpoint = trackNode.Checkpoints[j];
                        var jFormat = j.ArrayFormat(trackNode.Checkpoints);
                        // Write index, string to log
                        log.Write($"[{iFormat},{jFormat}]\t");
                        log.WriteLine(PrintData(functionIdx, checkpoint));
                    }
                }
                log.WriteLine();

                //
                log.WriteLine($"{nameof(TrackSegment)}");
                for (int i = 0; i < coliScene.AllTrackSegments.Length; i++)
                {
                    var trackSegment = coliScene.AllTrackSegments[i];
                    log.Write(PrintIndex(i, coliScene.trackNodes));
                    log.WriteLine(PrintData(functionIdx, trackSegment));
                }
                log.WriteLine();

                //
                log.WriteLine($"{nameof(TrackSegment)}.{nameof(TrackSegment.AnimationCurveTRS)}");
                string[] labelSRP = new string[] { "Sca", "Rot", "Pos" }; // scale, rotation, position
                string[] labelXYZ = new string[] { "x", "y", "z" };
                for (int segmentIndex = 0; segmentIndex < coliScene.AllTrackSegments.Length; segmentIndex++)
                {
                    var trackSegment = coliScene.AllTrackSegments[segmentIndex];
                    var segmentIndexFormat = segmentIndex.ArrayFormat(coliScene.AllTrackSegments);
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
                for (int i = 0; i < coliScene.AllTrackSegments.Length; i++)
                {
                    var cornerTopology = coliScene.AllTrackSegments[i].TrackCorner;
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

                //log.WriteLine($"{nameof(SceneObject)}");
                //for (int i = 0; i < coliScene.SceneObjectLODs.Length; i++)
                //{
                //    var sceneObjectReference = coliScene.SceneObjectLODs[i];
                //    log.Write(PrintIndex(i, coliScene.SceneObjectLODs));
                //    log.WriteLine(PrintData(functionIdx, sceneObjectReference));
                //}
                //log.WriteLine();

                //log.WriteLine($"{nameof(ShiftJisCString)}");
                //for (int i = 0; i < coliScene.SceneObjectNames.Length; i++)
                //{
                //    var objectName = coliScene.SceneObjectNames[i];
                //    log.Write(PrintIndex(i, coliScene.SceneObjectNames));
                //    log.WriteLine(PrintData(functionIdx, objectName));
                //}
                //log.WriteLine();
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
            log.WriteLine($"{nameof(CircuitType)}: {coliScene.CircuitType}");
            log.WriteLine($"{nameof(Bool32)}: {coliScene.StaticColliderMeshManagerActive}");
            log.WriteLine($"{nameof(Bool32)}: {coliScene.UnkBool32_0x58}");
            log.WriteLine($"{nameof(coliScene.UnkRange0x00)}: {coliScene.UnkRange0x00}");
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
            log.WriteAddress(coliScene.cullOverrideTriggers);
            log.WriteAddress(coliScene.visualEffectTriggers);

            // Writes non-array track data
            log.WriteHeading("TRACK DATA", padding, h1Width);
            log.WriteAddress(coliScene.trackLength);
            log.WriteAddress(coliScene.trackMinHeight);
            log.WriteAddress(coliScene.CheckpointGridXZ);
            log.WriteLine();
            // Writes track objects array
            log.WriteAddress(coliScene.embeddedPropertyAreas);
            // Writes track segments (root then all)
            log.WriteLine("ROOT SEGMENTS");
            log.WriteAddress(coliScene.RootTrackSegments);
            log.WriteLine("ALL SEGMENTS");
            log.WriteAddress(coliScene.AllTrackSegments);
            log.WriteLine();
            TestHash(log, coliScene);
            log.WriteLine();

            // This block writes out the contents of each TrackSegments AnimationCurves
            log.WriteLine("TRACK SEGMENT ANIMATION CURVES");
            log.WriteLine($"{nameof(TrackSegment)}.{nameof(TrackSegment.AnimationCurveTRS)}");
            string[] labelSRP = new string[] { "Sca", "Rot", "Pos" };
            string[] labelXYZ = new string[] { "x", "y", "z" };
            for (int segmentIndex = 0; segmentIndex < coliScene.AllTrackSegments.Length; segmentIndex++)
            {
                var trackSegment = coliScene.AllTrackSegments[segmentIndex];
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
            for (int i = 0; i < coliScene.AllTrackSegments.Length; i++)
            {
                var segment = coliScene.AllTrackSegments[i];
                log.WriteLine($"{nameof(TrackSegment)} Transform Coords [{i}]");
                log.WriteLine($"\tPosition: {segment.FallbackPosition}");
                log.WriteLine($"\tRotation: {segment.FallbackRotation}");
                log.WriteLine($"\tScale...: {segment.FallbackScale}");
            }
            log.WriteLine();
            //
            log.WriteAddress(coliScene.trackCheckpointGrid);
            log.WriteAddress(coliScene.trackNodes);
            {
                var checkpoints = new List<Checkpoint>();
                foreach (var trackNode in coliScene.trackNodes)
                    foreach (var checkpoint in trackNode.Checkpoints)
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
            //log.WriteAddress(coliScene.SceneObjectNames);
            //log.WriteAddress(coliScene.SceneObjectLODs);
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
            var readerIn = new EndianBinaryReader(File.OpenRead(filePath), Scene.endianness);
            // Set scene instance, deserialize data
            var sceneIn = new Scene();
            sceneIn.FileName = Path.GetFileName(filePath);
            sceneIn.SerializeVerbose = serializeVerbose;
            sceneIn.Deserialize(readerIn);
            // Check to see if file is from ROM. Add correct metadata.
            // TODO: remove hardcoded match and use dynamic JP/EN/PAL
            readerIn.SeekBegin();
            var sceneIndex = sceneIn.CourseIndex;
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
                    var logPath = Path.Combine(exportTo, $"{timestamp} - {i + 1} {labels[i]} COLI_COURSE{sceneIn.CourseIndex:d2} IN.txt");
                    var log = new TextLogger(logPath);
                    LogColiScene(sceneIn, log, i);
                    log.Close();
                }
            }

            // WRITE INTERMEDIARY
            // Write scene out to memory stream...
            var writer = new EndianBinaryWriter(new MemoryStream(), Scene.endianness);
            writer.Write(sceneIn);
            writer.Flush();
            writer.BaseStream.Seek(0, SeekOrigin.Begin);

            // LOAD INTERMEDIARY
            // Load memory stream back in
            var readerOut = new EndianBinaryReader(writer.BaseStream, Scene.endianness);
            var sceneOut = new Scene();
            sceneOut.SerializeVerbose = serializeVerbose;
            sceneOut.FileName = sceneIn.FileName;
            sceneOut.Deserialize(readerOut);
            // Log intermediary
            {
                //WriteTrackDataHashReport(log, sceneIn);
                for (int i = 0; i < NumFunctions; i++)
                {
                    var logPath = Path.Combine(exportTo, $"{timestamp} - {i + 1} {labels[i]} COLI_COURSE{sceneIn.CourseIndex:d2} OUT.txt");
                    var log = new TextLogger(logPath);
                    LogColiScene(sceneIn, log, i);
                    log.Close();
                }
            }

            // open folder location
            OSUtility.OpenDirectory(openFolderAfterExport, $"{exportTo}/");
            // Set static variable
            // Export file...
            //var x = ExportUtility.ExportSerializable(sceneIn, exportTo, "", true);
            //OSUtility.OpenDirectory(openFolderAfterExport, x);

            // Clean this class up / move to where appropriate with new codebase
            throw new NotImplementedException();
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
                var readerIn = new EndianBinaryReader(stream, Scene.endianness);
                var sceneIn = new Scene();
                sceneIn.FileName = filename;
                sceneIn.SerializeVerbose = serializeVerbose;
                sceneIn.Deserialize(readerIn);

                // WRITE INTERMEDIARY
                // Write scene out to memory stream...
                var writer = new EndianBinaryWriter(new MemoryStream(), Scene.endianness);
                writer.Write(sceneIn);
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
            return binarySerializable.AddressRange.ToString();
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
            foreach (var trackSegment in coliScene.AllTrackSegments)
            {
                log.WriteLine(HashUtility.HashBinary(md5, trackSegment));
            }
        }

    }

}
