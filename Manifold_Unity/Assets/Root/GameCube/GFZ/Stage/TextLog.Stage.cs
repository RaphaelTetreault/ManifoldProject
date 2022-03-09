using Manifold.IO;
using System.Collections.Generic;
using System.Security.Cryptography;


namespace GameCube.GFZ.Stage
{
    public static class StageTextLogger
    {

        public static void LogAnimationClips(TextLogger log, Scene scene, int indentLevel = 0, string indent = "\t")
        {
            //var builder = new System.Text.StringBuilder();

            //foreach (var dynamicSceneObject in scene.dynamicSceneObjects)
            //{
            //    var animationClip = dynamicSceneObject.AnimationClip;

            //    // Skip if it does not exist
            //    if (animationClip == null)
            //        continue;

            //    animationClip.PrintMultiLine(builder, indentLevel, indent);
            //    log.Write(builder);
            //}
            throw new System.NotImplementedException();
        }

        public static void LogSceneMetadata(TextLogger log, Scene scene, int indentLevel = 0, string indent = "\t")
        {
            log.WriteLine($"Venue: {scene.Venue}");
            log.WriteLine($"Course: {scene.VenueName} [{scene.CourseName}]");
            log.WriteLine($"Author: {scene.Author}");
            log.WriteLine($"{nameof(CircuitType)}: {scene.CircuitType}");
            log.WriteLine($"{nameof(Bool32)}: {scene.UnkBool32_0x58}");
            log.WriteLine($"{nameof(scene.UnkRange0x00)}: {scene.UnkRange0x00}");
        }

        public static void LogScene(TextLogger log, Scene scene, int indentLevel = 0, string indent = "\t")
        {
            //// TODO: write header information
            //var builder = new System.Text.StringBuilder();

            //LogSceneMetadata(log, scene, indentLevel, indent);
            //log.WriteLine();

            //foreach (var dynamicSceneObject in scene.dynamicSceneObjects)
            //{
            //    var animationClip = dynamicSceneObject.AnimationClip;
            //    if (animationClip != null)
            //        log.Write(animationClip.PrintMultiLine(builder, indentLevel, indent));
            //}

            throw new System.NotImplementedException();
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
            log.WriteAddress(coliScene.SceneObjectNames);
            log.WriteAddress(coliScene.SceneObjectLODs);
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

    }
}
