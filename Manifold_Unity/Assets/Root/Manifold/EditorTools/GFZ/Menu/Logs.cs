using Manifold;
using Manifold.IO;
using Manifold.IO.GFZ;
using Manifold.EditorTools;
using GameCube.GFZ;
using GameCube.GFZ.CourseCollision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

namespace Manifold.EditorTools.GFZ.Menu
{
    public static partial class Logs
    {
        public static class Stage
        {
            public const string ActiveRoot = " (Active Root)";

            #region MENU ITEM



            #endregion



            /// <summary>
            /// Writes simple log which enumerates all data with ToString() call.
            /// </summary>
            [MenuItem(Const.Menu.logs + "Log All Stages" + ActiveRoot)]
            public static void LogAllStage()
            {
                var settings = GfzProjectWindow.GetSettings();

                foreach (var coliScene in ColiCourseIO.LoadAllStages(settings.StageDir, "Logging Stages..."))
                {
                    var outputFile = $"{settings.LogOutput}/log-{coliScene.FileName}.txt";
                    var log = new TextLogger(outputFile);
                    LogScene(log, coliScene);
                    log.Flush();
                    log.Close();
                }

                OSUtility.OpenDirectory(settings.LogOutput);
            }


            public static void LogAnimationClips(TextLogger log, ColiScene scene, string indent = "\t", int indentLevel = 0)
            {
                foreach (var dynamicSceneObject in scene.dynamicSceneObjects)
                {
                    var animationClip = dynamicSceneObject.animationClip;

                    // Skip if it does not exist
                    if (animationClip == null)
                        continue;

                    var printOut = animationClip.PrintMultiLine(indent, indentLevel);
                    log.Write(printOut);
                }
            }


            public static void LogScene(TextLogger log, ColiScene scene)
            {
                // TODO: write header information

                LogAnimationClips(log, scene);
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
                log.WriteLine($"{nameof(Bool32)}: {coliScene.unkBool32_0x58}");
                log.WriteLine($"{nameof(coliScene.unkRange0x00)}: {coliScene.unkRange0x00}");
                log.WriteLine(); //
                log.WriteLine(); // yes, 2 WriteLines

                log.WriteHeading("FOG", padding, h1Width);
                log.WriteAddress(coliScene.fog);
                log.WriteAddress(coliScene.fogCurves);
                log.WriteLine();

                log.WriteHeading("TRIGGERS", padding, h1Width);
                log.WriteAddress(coliScene.arcadeCheckpointTriggers);
                log.WriteAddress(coliScene.courseMetadataTriggers);
                log.WriteAddress(coliScene.storyObjectTriggers);
                log.WriteAddress(coliScene.unknownTriggers);
                log.WriteAddress(coliScene.visualEffectTriggers);

                // Writes non-array track data
                log.WriteHeading("TRACK DATA", padding, h1Width);
                log.WriteAddress(coliScene.trackLength);
                log.WriteAddress(coliScene.trackMinHeight);
                log.WriteAddress(coliScene.trackCheckpointBoundsXZ);
                log.WriteLine();
                // Writes track objects array
                log.WriteAddress(coliScene.surfaceAttributeAreas);
                // Writes track segments (root then all)
                log.WriteLine("ROOT SEGMENTS");
                log.WriteAddress(coliScene.rootTrackSegments);
                log.WriteLine("ALL SEGMENTS");
                log.WriteAddress(coliScene.allTrackSegments);
                log.WriteLine();
                ColiCourseIO.TestHash(log, coliScene);
                log.WriteLine();

                // This block writes out the contents of each TrackSegments AnimationCurves
                log.WriteLine("TRACK SEGMENT ANIMATION CURVES");
                log.WriteLine($"{nameof(TrackSegment)}.{nameof(TrackSegment.trackCurves)}");
                string[] labelSRP = new string[] { "Sca", "Rot", "Pos" };
                string[] labelXYZ = new string[] { "x", "y", "z" };
                for (int segmentIndex = 0; segmentIndex < coliScene.allTrackSegments.Length; segmentIndex++)
                {
                    var trackSegment = coliScene.allTrackSegments[segmentIndex];
                    log.WriteLine($"{nameof(TrackSegment)}[{segmentIndex}]\t{trackSegment}");

                    for (int animIndex = 0; animIndex < trackSegment.trackCurves.animationCurves.Length; animIndex++)
                    {
                        var animCurve = trackSegment.trackCurves.animationCurves[animIndex];
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
                    var checkpoints = new List<Checkpoint>();
                    foreach (var trackNode in coliScene.trackNodes)
                        foreach (var checkpoint in trackNode.checkpoints)
                            checkpoints.Add(checkpoint);
                    log.WriteAddress(checkpoints.ToArray());
                }
                log.WriteLine();

                log.WriteHeading("STATIC COLLISION", padding, h1Width);
                log.WriteAddress(coliScene.staticColliderMeshes);
                log.WriteLine();
                log.WriteLine(nameof(UnknownCollider) + "[]");
                log.WriteAddress(coliScene.unknownColliders);
                log.WriteLine();
                log.WriteLine(nameof(GameCube.GFZ.BoundingSphere));
                log.WriteLine("unk float: " + coliScene.staticColliderMeshes.unk_float);
                log.WriteAddress(coliScene.staticColliderMeshes.boundingSphere);
                log.WriteLine(coliScene.staticColliderMeshes.boundingSpherePtr);
                log.WriteLine(coliScene.staticColliderMeshes.staticSceneObjectsPtr);
                log.WriteLine();
                log.WriteLine("Mesh Bounds");
                log.WriteAddress(coliScene.staticColliderMeshes.meshBounds);
                log.WriteLine();
                log.WriteLine("TRIANGLES");
                log.WriteAddress(coliScene.staticColliderMeshes.colliderTris);
                log.WriteAddress(coliScene.staticColliderMeshes.triMeshMatrices);
                // Write each index list
                log.WriteNullInArray = false;
                for (int i = 0; i < coliScene.staticColliderMeshes.triMeshMatrices.Length; i++)
                {
                    var triIndexList = coliScene.staticColliderMeshes.triMeshMatrices[i];
                    if (triIndexList != null)
                    {
                        log.WriteLine($"COLLIDER TYPE [{i}]: {(StaticColliderMeshProperty)i}");
                        log.WriteAddress(triIndexList.indexLists);
                    }
                }
                log.WriteNullInArray = true;
                log.WriteLine("QUADS");
                log.WriteAddress(coliScene.staticColliderMeshes.colliderQuads);
                log.WriteAddress(coliScene.staticColliderMeshes.quadMeshMatrices);
                // Write each index list
                log.WriteNullInArray = false;
                for (int i = 0; i < coliScene.staticColliderMeshes.quadMeshMatrices.Length; i++)
                {
                    var quadIndexList = coliScene.staticColliderMeshes.quadMeshMatrices[i];
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
                        log.WriteAddress(sceneObject.sceneObject);
                        log.WriteAddress(sceneObject.sceneObject.lods);
                        log.WriteAddress(sceneObject.sceneObject.PrimarySceneObject.name);
                        log.WriteAddress(sceneObject.transformMatrix3x4);

                        log.WriteAddress(sceneObject.skeletalAnimator);
                        if (sceneObject.skeletalAnimator != null)
                            log.WriteAddress(sceneObject.skeletalAnimator.properties);

                        log.WriteAddress(sceneObject.animationClip);
                        if (sceneObject.animationClip != null)
                            log.WriteAddress(sceneObject.animationClip.curves);
                        // TODO: other sub classes?

                        log.WriteAddress(sceneObject.textureScroll);
                        if (sceneObject.textureScroll != null)
                            log.WriteAddress(sceneObject.textureScroll.fields);

                        log.WriteLine();
                    }
                }
                log.WriteLine();
                log.Flush();
            }

        }
    }
}
