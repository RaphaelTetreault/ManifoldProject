using Manifold.IO;
using System.Collections.Generic;
using System.Security.Cryptography;


namespace GameCube.GFZ.Stage
{
    public static class StageTextLogger
    {

        public static void LogAnimationClips(TextLogger log, Scene scene, int indentLevel = 0, string indent = "\t")
        {
            foreach (var dynamicSceneObject in scene.dynamicSceneObjects)
            {
                var animationClip = dynamicSceneObject.animationClip;

                // Skip if it does not exist
                if (animationClip == null)
                    continue;

                var printOut = animationClip.PrintMultiLine(indentLevel, indent);
                log.Write(printOut);
            }
        }

        public static void LogSceneMetadata(TextLogger log, Scene scene, int indentLevel = 0, string indent = "\t")
        {
            log.WriteLine($"Venue: {scene.Venue}");
            log.WriteLine($"Course: {scene.VenueName} [{scene.CourseName}]");
            log.WriteLine($"Author: {scene.Author}");
            log.WriteLine($"{nameof(CircuitType)}: {scene.circuitType}");
            log.WriteLine($"{nameof(Bool32)}: {scene.unkBool32_0x58}");
            log.WriteLine($"{nameof(scene.unkRange0x00)}: {scene.unkRange0x00}");
        }

        public static void LogScene(TextLogger log, Scene scene, int indentLevel = 0, string indent = "\t")
        {
            // TODO: write header information

            LogSceneMetadata(log, scene, indentLevel, indent);
            log.WriteLine();

            foreach (var dynamicSceneObject in scene.dynamicSceneObjects)
            {
                var animationClip = dynamicSceneObject.animationClip;
                if (animationClip != null)
                    log.Write(animationClip.PrintMultiLine(indentLevel, indent));
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
            log.WriteLine("unk float: " + coliScene.staticColliderMeshManager.unk_float);
            log.WriteAddress(coliScene.staticColliderMeshManager.BoundingSphere);
            log.WriteLine(coliScene.staticColliderMeshManager.unknownCollidersPtr);
            log.WriteLine(coliScene.staticColliderMeshManager.staticSceneObjectsPtr);
            log.WriteLine();
            log.WriteLine("Mesh Bounds");
            log.WriteAddress(coliScene.staticColliderMeshManager.meshGridXZ);
            log.WriteLine();
            log.WriteLine("TRIANGLES");
            log.WriteAddress(coliScene.staticColliderMeshManager.colliderTris);
            log.WriteAddress(coliScene.staticColliderMeshManager.triMeshGrids);
            // Write each index list
            log.WriteNullInArray = false;
            for (int i = 0; i < coliScene.staticColliderMeshManager.triMeshGrids.Length; i++)
            {
                var triIndexList = coliScene.staticColliderMeshManager.triMeshGrids[i];
                if (triIndexList != null)
                {
                    log.WriteLine($"COLLIDER TYPE [{i}]: {(StaticColliderMeshProperty)i}");
                    log.WriteAddress(triIndexList.IndexLists);
                }
            }
            log.WriteNullInArray = true;
            log.WriteLine("QUADS");
            log.WriteAddress(coliScene.staticColliderMeshManager.colliderQuads);
            log.WriteAddress(coliScene.staticColliderMeshManager.quadMeshGrids);
            // Write each index list
            log.WriteNullInArray = false;
            for (int i = 0; i < coliScene.staticColliderMeshManager.quadMeshGrids.Length; i++)
            {
                var quadIndexList = coliScene.staticColliderMeshManager.quadMeshGrids[i];
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
                    log.WriteAddress(sceneObject.sceneObject);
                    log.WriteAddress(sceneObject.sceneObject.LODs);
                    log.WriteAddress(sceneObject.sceneObject.PrimaryLOD.name);
                    log.WriteAddress(sceneObject.transformMatrix3x4);

                    log.WriteAddress(sceneObject.skeletalAnimator);
                    if (sceneObject.skeletalAnimator != null)
                        log.WriteAddress(sceneObject.skeletalAnimator.properties);

                    log.WriteAddress(sceneObject.animationClip);
                    if (sceneObject.animationClip != null)
                        log.WriteAddress(sceneObject.animationClip.Curves);
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
