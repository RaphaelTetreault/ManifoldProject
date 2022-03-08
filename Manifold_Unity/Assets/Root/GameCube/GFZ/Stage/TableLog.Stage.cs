using Manifold;
using Manifold.IO;
using System.Collections.Generic;
using System.IO;

namespace GameCube.GFZ.Stage
{
    public static class StageTableLogger
    {
        // Names of files generated
        public static readonly string tsvHeader = $"{nameof(Scene)}-Header.tsv";
        public static readonly string tsvGeneralData = $"General Data.tsv";
        public static readonly string tsvTrackKeyablesAll = $"Track Keyables All.tsv";
        public static readonly string tsvTrackSegment = $"{nameof(TrackSegment)}.tsv";
        public static readonly string tsvSurfaceAttributeArea = $"{nameof(EmbeddedTrackPropertyArea)}.tsv";
        public static readonly string tsvTrackNode = $"{nameof(TrackNode)}.tsv";
        public static readonly string tsvSceneObject = $"{nameof(SceneObject)}.tsv";
        public static readonly string tsvSceneObjectLod = $"{nameof(SceneObjectLOD)}.tsv";
        public static readonly string tsvSceneObjectsAndLod = $"{nameof(SceneObjectLOD)}.tsv";
        public static readonly string tsvSceneObjectDynamic = $"{nameof(SceneObjectDynamic)}.tsv";
        public static readonly string tsvAnimationClip = $"{nameof(SceneObjectDynamic)}-{nameof(AnimationClip)}.tsv";
        public static readonly string tsvTextureMetadata = $"{nameof(SceneObjectDynamic)}-{nameof(TextureScroll)}.tsv";
        public static readonly string tsvSkeletalAnimator = $"{nameof(SceneObjectDynamic)}-{nameof(SkeletalAnimator)}.tsv";
        public static readonly string tsvColliderGeometryTri = $"{nameof(SceneObjectDynamic)}-{nameof(ColliderMesh)}-Tris.tsv";
        public static readonly string tsvColliderGeometryQuad = $"{nameof(SceneObjectDynamic)}-{nameof(ColliderMesh)}-Quads.tsv";
        public static readonly string tsvTransform = $"{nameof(TransformTRXS)}.tsv";
        public static readonly string tsvTimeExtensionTrigger = $"{nameof(TimeExtensionTrigger)}.tsv";
        public static readonly string tsvMiscellaneousTrigger = $"{nameof(MiscellaneousTrigger)}.tsv";
        public static readonly string tsvStoryObjectTrigger = $"{nameof(StoryObjectTrigger)}.tsv";
        public static readonly string tsvUnknownTrigger = $"{nameof(CullOverrideTrigger)}.tsv";
        public static readonly string tsvVisualEffectTrigger = $"{nameof(VisualEffectTrigger)}.tsv";
        public static readonly string tsvFog = $"{nameof(Fog)}.tsv";
        public static readonly string tsvFogCurves = $"{nameof(FogCurves)}.tsv";
        public static readonly string tsvStaticColliderMeshes = $"{nameof(StaticColliderMeshManager)}.tsv";
        public static readonly string tsvUnknownCollider = $"{nameof(UnknownCollider)}.tsv";


        #region Track Data / Transforms

        public static void AnalyzeTrackKeyablesAll(Scene[] scenes, string filename)
        {
            using (var writer = AnalyzerUtility.OpenWriter(filename))
            {
                // Write header
                writer.WriteNextCol("FileName");
                writer.WriteNextCol("Game");

                writer.WriteNextCol(nameof(TrackSegment.SegmentType));
                writer.WriteNextCol(nameof(TrackSegment.EmbeddedPropertyType));
                writer.WriteNextCol(nameof(TrackSegment.PerimeterFlags));
                writer.WriteNextCol(nameof(TrackSegment.PipeCylinderFlags));
                writer.WriteNextCol(nameof(TrackSegment.Unk_0x38));
                writer.WriteNextCol(nameof(TrackSegment.Unk_0x3A));

                writer.WriteNextCol("TrackTransform Index");
                writer.WriteNextCol("Keyable /9");
                writer.WriteNextCol("Keyable Index");
                writer.WriteNextCol("Keyable Order");
                writer.WriteNextCol("Nested Depth");
                writer.WriteNextCol("Address");
                writer.WriteNextCol(nameof(KeyableAttribute.EaseMode));
                writer.WriteNextCol(nameof(KeyableAttribute.EaseMode));
                writer.WriteNextCol(nameof(KeyableAttribute.Time));
                writer.WriteNextCol(nameof(KeyableAttribute.Value));
                writer.WriteNextCol(nameof(KeyableAttribute.TangentIn));
                writer.WriteNextCol(nameof(KeyableAttribute.TangentOut));
                writer.WriteNextRow();

                // foreach File
                foreach (var scene in scenes)
                {
                    // foreach Transform
                    int trackIndex = 0;
                    foreach (var trackTransform in scene.rootTrackSegments)
                    {
                        for (int keyablesIndex = 0; keyablesIndex < AnimationCurveTRS.kCurveCount; keyablesIndex++)
                        {
                            WriteTrackKeyableAttributeRecursive(writer, scene, 0, keyablesIndex, ++trackIndex, trackTransform);
                        }
                    }
                }

                writer.Flush();
            }
        }

        public static void AnalyzeTrackKeyables(Scene[] scenes, string filename, int keyablesSet)
        {
            using (var writer = AnalyzerUtility.OpenWriter(filename))
            {
                // Write header
                writer.WriteNextCol("FileName");
                writer.WriteNextCol("Game");

                writer.WriteNextCol(nameof(TrackSegment.SegmentType));
                writer.WriteNextCol(nameof(TrackSegment.EmbeddedPropertyType));
                writer.WriteNextCol(nameof(TrackSegment.PerimeterFlags));
                writer.WriteNextCol(nameof(TrackSegment.PipeCylinderFlags));
                writer.WriteNextCol(nameof(TrackSegment.Unk_0x38));
                writer.WriteNextCol(nameof(TrackSegment.Unk_0x3A));

                writer.WriteNextCol("TrackTransform Index");
                writer.WriteNextCol("Keyable /9");
                writer.WriteNextCol("Keyable Index");
                writer.WriteNextCol("Keyable Order");
                writer.WriteNextCol("Nested Depth");
                writer.WriteNextCol("Address");
                writer.WriteNextCol(nameof(KeyableAttribute.EaseMode));
                writer.WriteNextCol(nameof(KeyableAttribute.EaseMode));
                writer.WriteNextCol(nameof(KeyableAttribute.Time));
                writer.WriteNextCol(nameof(KeyableAttribute.Value));
                writer.WriteNextCol(nameof(KeyableAttribute.TangentIn));
                writer.WriteNextCol(nameof(KeyableAttribute.TangentOut));
                writer.WriteNextRow();

                // foreach File
                foreach (var scene in scenes)
                {
                    // foreach Transform
                    int trackTransformIndex = 0;
                    foreach (var trackTransform in scene.rootTrackSegments)
                    {
                        WriteTrackKeyableAttributeRecursive(writer, scene, nestedDepth: 0, keyablesSet, trackTransformIndex++, trackTransform);
                    }
                }

                writer.Flush();
            }
        }
        public static void WriteTrackKeyableAttributeRecursive(StreamWriter writer, Scene scene, int nestedDepth, int animationCurveIndex, int trackTransformIndex, TrackSegment trackTransform)
        {
            var animationCurves = trackTransform.AnimationCurveTRS.AnimationCurves;
            var keyableIndex = 1; // 0-n, depends on number of keyables in array
            int keyableTotal = animationCurves[animationCurveIndex].Length;

            // Animation data of this curve
            foreach (var keyables in animationCurves[animationCurveIndex].KeyableAttributes)
            {
                WriteKeyableAttribute(writer, scene, nestedDepth + 1, keyableIndex++, keyableTotal, animationCurveIndex, trackTransformIndex, keyables, trackTransform);
            }

            // TODO: do you even care to reimplement this at this point?
            // Go to track transform children, write their anim data (calls this function)
            //Debug.LogWarning("You refactored this analysis out!");
            //foreach (var child in trackTransform.children)
            //    WriteTrackKeyableAttributeRecursive(writer, sobj, nestedDepth + 1, animationCurveIndex, trackTransformIndex, child);
        }
        public static void WriteKeyableAttribute(StreamWriter writer, Scene scene, int nestedDepth, int keyableIndex, int keyableTotal, int keyablesSet, int trackTransformIndex, KeyableAttribute param, TrackSegment tt)
        {
            string gameId = scene.IsFileGX ? "GX" : "AX";

            writer.WriteNextCol(scene.FileName);
            writer.WriteNextCol(gameId);

            writer.WriteNextCol(tt.SegmentType);
            writer.WriteNextCol(tt.EmbeddedPropertyType);
            writer.WriteNextCol(tt.PerimeterFlags);
            writer.WriteNextCol(tt.PipeCylinderFlags);
            writer.WriteNextCol(tt.Unk_0x38);
            writer.WriteNextCol(tt.Unk_0x3A);

            writer.WriteNextCol(trackTransformIndex);
            writer.WriteNextCol(keyablesSet);
            writer.WriteNextCol(keyableIndex);
            writer.WriteNextCol($"[{keyableIndex}/{keyableTotal}]");
            writer.WriteNextCol($"{nestedDepth}");
            writer.WriteNextCol(param.PrintStartAddress());
            writer.WriteNextCol(param.EaseMode);
            writer.WriteNextCol((int)param.EaseMode);
            writer.WriteNextCol(param.Time);
            writer.WriteNextCol(param.Value);
            writer.WriteNextCol(param.TangentIn);
            writer.WriteNextCol(param.TangentOut);
            writer.WriteNextRow();
        }


        // Kicks off recursive write
        private static int s_order;
        public static void AnalyzeTrackSegments(Scene[] scenes, string filename)
        {
            using (var writer = AnalyzerUtility.OpenWriter(filename))
            {
                writer.WriteNextCol("Filename");
                writer.WriteNextCol("Order");
                writer.WriteNextCol("Root Index");
                writer.WriteNextCol("Transform Depth");
                writer.WriteNextCol("Address");
                writer.WriteNextCol(nameof(TrackSegment.SegmentType));
                writer.WriteNextCol(nameof(TrackSegment.EmbeddedPropertyType));
                writer.WriteNextCol(nameof(TrackSegment.PerimeterFlags));
                writer.WriteNextCol(nameof(TrackSegment.PipeCylinderFlags));
                writer.WriteNextCol(nameof(TrackSegment.AnimationCurvesTrsPtr));
                writer.WriteNextCol(nameof(TrackSegment.TrackCornerPtr));
                writer.WriteNextCol(nameof(TrackSegment.ChildrenPtr));
                writer.WriteNextCol(nameof(TrackSegment.LocalScale));
                writer.WriteNextCol(nameof(TrackSegment.LocalRotation));
                writer.WriteNextCol(nameof(TrackSegment.LocalPosition));
                writer.WriteNextCol(nameof(TrackSegment.Unk_0x38));
                writer.WriteNextCol(nameof(TrackSegment.Unk_0x39));
                writer.WriteNextCol(nameof(TrackSegment.Unk_0x3A));
                writer.WriteNextCol(nameof(TrackSegment.Unk_0x3B));
                writer.WriteNextCol(nameof(TrackSegment.RailHeightRight));
                writer.WriteNextCol(nameof(TrackSegment.RailHeightLeft));
                //writer.WriteNextCol(nameof(TrackSegment.zero_0x44));
                //writer.WriteNextCol(nameof(TrackSegment.zero_0x48));
                writer.WriteNextCol(nameof(TrackSegment.BranchIndex));
                writer.WriteNextCol();
                writer.WriteNextColNicify(nameof(TrackCorner.width));
                writer.WriteNextColNicify(nameof(TrackCorner.perimeterOptions));
                //
                writer.WriteNextRow();

                // RESET static variable
                s_order = 0;

                foreach (var scene in scenes)
                {
                    var index = 0;
                    var total = scene.rootTrackSegments.Length;
                    foreach (var trackTransform in scene.rootTrackSegments)
                    {
                        WriteTrackSegmentRecursive(writer, scene, 0, ++index, total, trackTransform);
                    }
                }

                writer.Flush();
            }
        }
        // Writes self and children
        public static void WriteTrackSegmentRecursive(StreamWriter writer, Scene scene, int depth, int index, int total, TrackSegment trackSegment)
        {
            // Write Parent
            WriteTrackSegment(writer, scene, depth, index, total, trackSegment);

            // Write children
            if (trackSegment.Children == null)
                return;

            foreach (var child in trackSegment.Children)
            {
                WriteTrackSegmentRecursive(writer, scene, depth + 1, index, total, child);
            }
        }
        // The actual writing to file
        public static void WriteTrackSegment(StreamWriter writer, Scene scene, int depth, int index, int total, TrackSegment trackTransform)
        {
            writer.WriteNextCol(scene.FileName);
            writer.WriteNextCol($"{s_order++}");
            writer.WriteNextCol($"[{index}/{total}]");
            writer.WriteNextCol($"{depth}");
            writer.WriteNextCol(trackTransform.PrintStartAddress());
            writer.WriteNextCol(trackTransform.SegmentType);
            writer.WriteNextCol(trackTransform.EmbeddedPropertyType);
            writer.WriteNextCol(trackTransform.PerimeterFlags);
            writer.WriteNextCol(trackTransform.PipeCylinderFlags);
            writer.WriteNextCol(trackTransform.AnimationCurvesTrsPtr);
            writer.WriteNextCol(trackTransform.TrackCornerPtr);
            writer.WriteNextCol(trackTransform.ChildrenPtr);
            writer.WriteNextCol(trackTransform.LocalScale);
            writer.WriteNextCol(trackTransform.LocalRotation);
            writer.WriteNextCol(trackTransform.LocalPosition);
            writer.WriteNextCol(trackTransform.Unk_0x38);
            writer.WriteNextCol(trackTransform.Unk_0x39);
            writer.WriteNextCol(trackTransform.Unk_0x3A);
            writer.WriteNextCol(trackTransform.Unk_0x3B);
            writer.WriteNextCol(trackTransform.RailHeightRight);
            writer.WriteNextCol(trackTransform.RailHeightLeft);
            //writer.WriteNextCol(trackTransform.zero_0x44);
            //writer.WriteNextCol(trackTransform.zero_0x48);
            writer.WriteNextCol(trackTransform.BranchIndex);
            //
            if (trackTransform.TrackCornerPtr.IsNotNull)
            {
                writer.WriteNextCol();
                writer.WriteNextCol(trackTransform.TrackCorner.width);
                writer.WriteNextCol(trackTransform.TrackCorner.perimeterOptions);
            }
            //
            writer.WriteNextRow();
        }


        #endregion

        #region Scene Objects' Animation Clips

        public static void AnalyzeAnimationClips(Scene[] scenes, string filename)
        {
            using (var writer = AnalyzerUtility.OpenWriter(filename))
            {
                // Write header
                writer.WriteNextCol("File Path");
                writer.WriteNextCol("Game Object #");
                writer.WriteNextCol("Game Object");
                writer.WriteNextCol("Anim Addr");
                writer.WriteNextCol("Key Addr");
                writer.WriteNextCol("Anim Index [0-10]");
                writer.WriteNextCol("Unk_0x00");
                writer.WriteNextCol("Time");
                writer.WriteNextCol("Value");
                writer.WriteNextCol("Unk_0x0C");
                writer.WriteNextCol("Unk_0x10");
                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    int gameObjectIndex = 0;
                    foreach (var gameObject in scene.dynamicSceneObjects)
                    {
                        if (gameObject.animationClip == null)
                            continue;
                        if (gameObject.animationClip.Curves == null)
                            continue;

                        int animIndex = 0;
                        foreach (var animationClipCurve in gameObject.animationClip.Curves)
                        {
                            if (animationClipCurve.AnimationCurve == null)
                                continue;

                            foreach (var keyable in animationClipCurve.AnimationCurve.KeyableAttributes)
                            {
                                writer.WriteNextCol(scene.FileName);
                                writer.WriteNextCol(gameObjectIndex);
                                writer.WriteNextCol(gameObject.Name);
                                writer.WriteNextCol(animationClipCurve.PrintStartAddress());
                                writer.WriteNextCol(keyable.PrintStartAddress());
                                writer.WriteNextCol(animIndex);
                                writer.WriteNextCol(keyable.EaseMode);
                                writer.WriteNextCol(keyable.Time);
                                writer.WriteNextCol(keyable.Value);
                                writer.WriteNextCol(keyable.TangentIn);
                                writer.WriteNextCol(keyable.TangentOut);
                                writer.WriteNextRow();
                            }
                            animIndex++;
                        }
                        gameObjectIndex++;
                    }
                }
                writer.Flush();
            }
        }

        public static void AnalyzeGameObjectAnimationClipIndex(Scene[] scenes, string filename, int index)
        {
            using (var writer = AnalyzerUtility.OpenWriter(filename))
            {
                // Write header
                writer.WriteNextCol("File Path");
                writer.WriteNextCol("Game Object #");
                writer.WriteNextCol("Game Object");
                writer.WriteNextCol("Anim Addr");
                writer.WriteNextCol("Key Addr");
                writer.WriteNextColNicify(nameof(AnimationClipCurve.Unk_0x00));
                writer.WriteNextColNicify(nameof(AnimationClipCurve.Unk_0x04));
                writer.WriteNextColNicify(nameof(AnimationClipCurve.Unk_0x08));
                writer.WriteNextColNicify(nameof(AnimationClipCurve.Unk_0x0C));
                writer.WriteNextCol("AnimClip Metadata");
                writer.WriteNextCol("AnimClip Metadata");
                writer.WriteNextCol("AnimClip Metadata");
                writer.WriteNextCol("Anim Index [0-10]");
                writer.WriteNextColNicify(nameof(KeyableAttribute.EaseMode));
                writer.WriteNextColNicify(nameof(KeyableAttribute.Time));
                writer.WriteNextColNicify(nameof(KeyableAttribute.Value));
                writer.WriteNextColNicify(nameof(KeyableAttribute.TangentIn));
                writer.WriteNextColNicify(nameof(KeyableAttribute.TangentOut));
                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    int objIndex = 0;
                    foreach (var dynamicSceneObject in scene.dynamicSceneObjects)
                    {
                        if (dynamicSceneObject.animationClip == null)
                            continue;
                        //if (gameObject.animation.curve == null)
                        //    continue;

                        int animIndex = 0;
                        foreach (var animationClipCurve in dynamicSceneObject.animationClip.Curves)
                        {
                            // Failing for some reason on indexes 6+ :/
                            if (animationClipCurve.AnimationCurve == null)
                                continue;

                            foreach (var keyable in animationClipCurve.AnimationCurve.KeyableAttributes)
                            {
                                /// HACK, write each anim index as separate file
                                if (animIndex != index)
                                    continue;

                                writer.WriteNextCol(scene.FileName);
                                writer.WriteNextCol(objIndex);
                                writer.WriteNextCol(dynamicSceneObject.Name);
                                writer.WriteNextCol(animationClipCurve.PrintStartAddress());
                                writer.WriteNextCol(keyable.PrintStartAddress());
                                writer.WriteNextCol(animationClipCurve.Unk_0x00);
                                writer.WriteNextCol(animationClipCurve.Unk_0x04);
                                writer.WriteNextCol(animationClipCurve.Unk_0x08);
                                writer.WriteNextCol(animationClipCurve.Unk_0x0C);
                                writer.WriteNextCol(animIndex);
                                writer.WriteNextCol(keyable.EaseMode);
                                writer.WriteNextCol(keyable.Time);
                                writer.WriteNextCol(keyable.Value);
                                writer.WriteNextCol(keyable.TangentIn);
                                writer.WriteNextCol(keyable.TangentOut);
                                writer.WriteNextRow();
                            }
                            animIndex++;
                        }
                        objIndex++;
                    }
                }
                writer.Flush();
            }
        }

        #endregion

        #region Scene Objects

        public static void AnalyzeSceneObjectDynamic(Scene[] scenes, string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Game Object #");
                writer.WriteNextCol("Game Object");
                writer.WriteNextCol(nameof(SceneObjectDynamic.unk0x00));
                writer.WriteNextCol(nameof(SceneObjectDynamic.unk0x00));
                writer.WriteNextCol(nameof(SceneObjectDynamic.unk0x04));
                writer.WriteNextCol(nameof(SceneObjectDynamic.unk0x04));
                writer.WriteNextCol(nameof(SceneObjectDynamic.sceneObjectPtr));
                writer.WriteNextCol(nameof(SceneObjectDynamic.transformPRXS.Position));
                writer.WriteNextCol(nameof(SceneObjectDynamic.transformPRXS.RotationEuler));
                writer.WriteNextCol(nameof(SceneObjectDynamic.transformPRXS.Scale));
                writer.WriteNextCol(nameof(SceneObjectDynamic.zero_0x2C));
                writer.WriteNextCol(nameof(SceneObjectDynamic.animationClipPtr));
                writer.WriteNextCol(nameof(SceneObjectDynamic.textureScrollPtr));
                writer.WriteNextCol(nameof(SceneObjectDynamic.skeletalAnimatorPtr));
                writer.WriteNextCol(nameof(SceneObjectDynamic.transformMatrix3x4Ptr));
                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    int sceneObjectIndex = 0;
                    foreach (var sceneObject in scene.dynamicSceneObjects)
                    {
                        writer.WriteNextCol(scene.FileName);
                        writer.WriteNextCol(sceneObjectIndex);
                        writer.WriteNextCol(sceneObject.Name);
                        writer.WriteNextCol(sceneObject.unk0x00);
                        writer.WriteNextCol($"0x{sceneObject.unk0x00:x8}");
                        writer.WriteNextCol(sceneObject.unk0x04);
                        writer.WriteNextCol($"0x{sceneObject.unk0x04:x8}");
                        writer.WriteNextCol(sceneObject.sceneObjectPtr.PrintAddress);
                        writer.WriteNextCol(sceneObject.transformPRXS.Position);
                        writer.WriteNextCol(sceneObject.transformPRXS.RotationEuler);
                        writer.WriteNextCol(sceneObject.transformPRXS.Scale);
                        writer.WriteNextCol(sceneObject.zero_0x2C);
                        writer.WriteNextCol(sceneObject.animationClipPtr.PrintAddress);
                        writer.WriteNextCol(sceneObject.textureScrollPtr.PrintAddress);
                        writer.WriteNextCol(sceneObject.skeletalAnimatorPtr.PrintAddress);
                        writer.WriteNextCol(sceneObject.transformMatrix3x4Ptr.PrintAddress);
                        writer.WriteNextRow();

                        sceneObjectIndex++;
                    }
                }
                writer.Flush();
            }
        }

        public static void AnalyzeTextureMetadata(Scene[] scenes, string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Game Object #");
                writer.WriteNextCol("Game Object");
                writer.WriteNextCol("Unknown 1 Index");
                writer.WriteNextColNicify(nameof(TextureScrollField.x));
                writer.WriteNextColNicify(nameof(TextureScrollField.y));
                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    int gameObjectIndex = 0;
                    foreach (var sceneObject in scene.dynamicSceneObjects)
                    {
                        if (sceneObject.textureScroll == null)
                            continue;

                        int fieldArrayIndex = 0;
                        foreach (var field in sceneObject.textureScroll.fields)
                        {
                            if (field == null)
                                return;

                            writer.WriteNextCol(scene.FileName);
                            writer.WriteNextCol(gameObjectIndex);
                            writer.WriteNextCol(sceneObject.Name);
                            writer.WriteNextCol(fieldArrayIndex);
                            writer.WriteNextCol(field.x);
                            writer.WriteNextCol(field.y);
                            writer.WriteNextRow();
                            fieldArrayIndex++;
                        }
                        gameObjectIndex++;
                    }
                }
                writer.Flush();
            }
        }

        public static void AnalyzeSkeletalAnimator(Scene[] scenes, string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Game Object #");
                writer.WriteNextCol("Game Object");

                writer.WriteNextColNicify(nameof(SkeletalAnimator.zero_0x00));
                writer.WriteNextColNicify(nameof(SkeletalAnimator.zero_0x04));
                writer.WriteNextColNicify(nameof(SkeletalAnimator.one_0x08));
                writer.WriteNextColNicify(nameof(SkeletalAnimator.propertiesPtr));

                writer.WriteNextColNicify(nameof(SkeletalProperties.unk_0x00));
                writer.WriteNextColNicify(nameof(SkeletalProperties.unk_0x04));
                writer.WriteFlagNames<EnumFlags32>();
                writer.WriteNextColNicify(nameof(SkeletalProperties.unk_0x08));
                writer.WriteFlagNames<EnumFlags32>();
                writer.WriteNextColNicify(nameof(SkeletalProperties.zero_0x0C));
                writer.WriteNextColNicify(nameof(SkeletalProperties.zero_0x10));
                writer.WriteNextColNicify(nameof(SkeletalProperties.zero_0x14));
                writer.WriteNextColNicify(nameof(SkeletalProperties.zero_0x18));
                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    int gameObjectIndex = 0;
                    foreach (var dynamicSceneObject in scene.dynamicSceneObjects)
                    {
                        if (dynamicSceneObject.skeletalAnimator == null)
                            continue;
                        if (dynamicSceneObject.skeletalAnimator.propertiesPtr.IsNull)
                            continue;

                        writer.WriteNextCol(scene.FileName);
                        writer.WriteNextCol(gameObjectIndex);
                        writer.WriteNextCol(dynamicSceneObject.Name);

                        writer.WriteNextCol(dynamicSceneObject.skeletalAnimator.zero_0x00);
                        writer.WriteNextCol(dynamicSceneObject.skeletalAnimator.zero_0x04);
                        writer.WriteNextCol(dynamicSceneObject.skeletalAnimator.one_0x08);
                        writer.WriteNextCol(dynamicSceneObject.skeletalAnimator.propertiesPtr);

                        writer.WriteNextCol(dynamicSceneObject.skeletalAnimator.properties.unk_0x00);
                        writer.WriteNextCol(dynamicSceneObject.skeletalAnimator.properties.unk_0x04);
                        writer.WriteFlags(dynamicSceneObject.skeletalAnimator.properties.unk_0x04);
                        writer.WriteNextCol(dynamicSceneObject.skeletalAnimator.properties.unk_0x08);
                        writer.WriteFlags(dynamicSceneObject.skeletalAnimator.properties.unk_0x08);
                        writer.WriteNextCol(dynamicSceneObject.skeletalAnimator.properties.zero_0x0C);
                        writer.WriteNextCol(dynamicSceneObject.skeletalAnimator.properties.zero_0x10);
                        writer.WriteNextCol(dynamicSceneObject.skeletalAnimator.properties.zero_0x14);
                        writer.WriteNextCol(dynamicSceneObject.skeletalAnimator.properties.zero_0x18);
                        writer.WriteNextRow();

                        gameObjectIndex++;
                    }
                }
                writer.Flush();
            }
        }

        public static void AnalyzeColliderGeometryTri(Scene[] scenes, string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Game Object #");
                writer.WriteNextCol("Game Object");

                writer.WriteNextCol("Tri Index");
                writer.WriteNextCol("Addr");

                writer.WriteNextColNicify(nameof(ColliderTriangle.DotProduct));
                writer.WriteNextColNicify(nameof(ColliderTriangle.Normal) + ".x");
                writer.WriteNextColNicify(nameof(ColliderTriangle.Normal) + ".y");
                writer.WriteNextColNicify(nameof(ColliderTriangle.Normal) + ".z");
                writer.WriteNextColNicify(nameof(ColliderTriangle.Vertex0) + ".x");
                writer.WriteNextColNicify(nameof(ColliderTriangle.Vertex0) + ".y");
                writer.WriteNextColNicify(nameof(ColliderTriangle.Vertex0) + ".z");
                writer.WriteNextColNicify(nameof(ColliderTriangle.Vertex1) + ".x");
                writer.WriteNextColNicify(nameof(ColliderTriangle.Vertex1) + ".y");
                writer.WriteNextColNicify(nameof(ColliderTriangle.Vertex1) + ".z");
                writer.WriteNextColNicify(nameof(ColliderTriangle.Vertex2) + ".x");
                writer.WriteNextColNicify(nameof(ColliderTriangle.Vertex2) + ".y");
                writer.WriteNextColNicify(nameof(ColliderTriangle.Vertex2) + ".z");
                writer.WriteNextColNicify(nameof(ColliderTriangle.Precomputed0) + ".x");
                writer.WriteNextColNicify(nameof(ColliderTriangle.Precomputed0) + ".y");
                writer.WriteNextColNicify(nameof(ColliderTriangle.Precomputed0) + ".z");
                writer.WriteNextColNicify(nameof(ColliderTriangle.Precomputed1) + ".x");
                writer.WriteNextColNicify(nameof(ColliderTriangle.Precomputed1) + ".y");
                writer.WriteNextColNicify(nameof(ColliderTriangle.Precomputed1) + ".z");
                writer.WriteNextColNicify(nameof(ColliderTriangle.Precomputed2) + ".x");
                writer.WriteNextColNicify(nameof(ColliderTriangle.Precomputed2) + ".y");
                writer.WriteNextColNicify(nameof(ColliderTriangle.Precomputed2) + ".z");

                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    int gameObjectIndex = 0;
                    foreach (var dynamicSceneObject in scene.dynamicSceneObjects)
                    {
                        if (dynamicSceneObject.sceneObject.colliderMesh == null)
                            continue;
                        if (dynamicSceneObject.sceneObject.colliderMesh.Tris.Length == 0)
                            continue;

                        int triIndex = 0;
                        foreach (var tri in dynamicSceneObject.sceneObject.colliderMesh.Tris)
                        {
                            writer.WriteNextCol(scene.FileName);
                            writer.WriteNextCol(gameObjectIndex);
                            writer.WriteNextCol(dynamicSceneObject.Name);

                            writer.WriteNextCol(triIndex++);
                            writer.WriteStartAddress(tri);

                            writer.WriteNextCol(tri.DotProduct);
                            writer.WriteNextCol(tri.Normal.x);
                            writer.WriteNextCol(tri.Normal.y);
                            writer.WriteNextCol(tri.Normal.z);
                            writer.WriteNextCol(tri.Vertex0.x);
                            writer.WriteNextCol(tri.Vertex0.y);
                            writer.WriteNextCol(tri.Vertex0.z);
                            writer.WriteNextCol(tri.Vertex1.x);
                            writer.WriteNextCol(tri.Vertex1.y);
                            writer.WriteNextCol(tri.Vertex1.z);
                            writer.WriteNextCol(tri.Vertex2.x);
                            writer.WriteNextCol(tri.Vertex2.y);
                            writer.WriteNextCol(tri.Vertex2.z);
                            writer.WriteNextCol(tri.Precomputed0.x);
                            writer.WriteNextCol(tri.Precomputed0.y);
                            writer.WriteNextCol(tri.Precomputed0.z);
                            writer.WriteNextCol(tri.Precomputed1.x);
                            writer.WriteNextCol(tri.Precomputed1.y);
                            writer.WriteNextCol(tri.Precomputed1.z);
                            writer.WriteNextCol(tri.Precomputed2.x);
                            writer.WriteNextCol(tri.Precomputed2.y);
                            writer.WriteNextCol(tri.Precomputed2.z);

                            writer.WriteNextRow();
                        }
                        gameObjectIndex++;
                    }
                }
                writer.Flush();
            }
        }

        public static void AnalyzeColliderGeometryQuad(Scene[] scenes, string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Game Object #");
                writer.WriteNextCol("Game Object");

                writer.WriteNextCol("Quad Index");
                writer.WriteNextCol("Addr");

                writer.WriteNextColNicify(nameof(ColliderQuad.DotProduct));
                writer.WriteNextColNicify(nameof(ColliderQuad.Normal) + ".x");
                writer.WriteNextColNicify(nameof(ColliderQuad.Normal) + ".y");
                writer.WriteNextColNicify(nameof(ColliderQuad.Normal) + ".z");
                writer.WriteNextColNicify(nameof(ColliderQuad.Vertex0) + ".x");
                writer.WriteNextColNicify(nameof(ColliderQuad.Vertex0) + ".y");
                writer.WriteNextColNicify(nameof(ColliderQuad.Vertex0) + ".z");
                writer.WriteNextColNicify(nameof(ColliderQuad.Vertex1) + ".x");
                writer.WriteNextColNicify(nameof(ColliderQuad.Vertex1) + ".y");
                writer.WriteNextColNicify(nameof(ColliderQuad.Vertex1) + ".z");
                writer.WriteNextColNicify(nameof(ColliderQuad.Vertex2) + ".x");
                writer.WriteNextColNicify(nameof(ColliderQuad.Vertex2) + ".y");
                writer.WriteNextColNicify(nameof(ColliderQuad.Vertex2) + ".z");
                writer.WriteNextColNicify(nameof(ColliderQuad.Vertex3) + ".x");
                writer.WriteNextColNicify(nameof(ColliderQuad.Vertex3) + ".y");
                writer.WriteNextColNicify(nameof(ColliderQuad.Vertex3) + ".z");
                writer.WriteNextColNicify(nameof(ColliderQuad.Precomputed0) + ".x");
                writer.WriteNextColNicify(nameof(ColliderQuad.Precomputed0) + ".y");
                writer.WriteNextColNicify(nameof(ColliderQuad.Precomputed0) + ".z");
                writer.WriteNextColNicify(nameof(ColliderQuad.Precomputed1) + ".x");
                writer.WriteNextColNicify(nameof(ColliderQuad.Precomputed1) + ".y");
                writer.WriteNextColNicify(nameof(ColliderQuad.Precomputed1) + ".z");
                writer.WriteNextColNicify(nameof(ColliderQuad.Precomputed2) + ".x");
                writer.WriteNextColNicify(nameof(ColliderQuad.Precomputed2) + ".y");
                writer.WriteNextColNicify(nameof(ColliderQuad.Precomputed2) + ".z");
                writer.WriteNextColNicify(nameof(ColliderQuad.Precomputed3) + ".x");
                writer.WriteNextColNicify(nameof(ColliderQuad.Precomputed3) + ".y");
                writer.WriteNextColNicify(nameof(ColliderQuad.Precomputed3) + ".z");

                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    int gameObjectIndex = 0;
                    foreach (var dynamicSceneObject in scene.dynamicSceneObjects)
                    {
                        if (dynamicSceneObject.sceneObject.colliderMesh == null)
                            continue;
                        if (dynamicSceneObject.sceneObject.colliderMesh.Quads.Length == 0)
                            continue;

                        int quadIndex = 0;
                        foreach (var quad in dynamicSceneObject.sceneObject.colliderMesh.Quads)
                        {
                            writer.WriteNextCol(scene.FileName);
                            writer.WriteNextCol(gameObjectIndex);
                            writer.WriteNextCol(dynamicSceneObject.Name);

                            writer.WriteNextCol(quadIndex++);
                            writer.WriteStartAddress(quad);

                            writer.WriteNextCol(quad.DotProduct);
                            writer.WriteNextCol(quad.Normal.x);
                            writer.WriteNextCol(quad.Normal.y);
                            writer.WriteNextCol(quad.Normal.z);
                            writer.WriteNextCol(quad.Vertex0.x);
                            writer.WriteNextCol(quad.Vertex0.y);
                            writer.WriteNextCol(quad.Vertex0.z);
                            writer.WriteNextCol(quad.Vertex1.x);
                            writer.WriteNextCol(quad.Vertex1.y);
                            writer.WriteNextCol(quad.Vertex1.z);
                            writer.WriteNextCol(quad.Vertex2.x);
                            writer.WriteNextCol(quad.Vertex2.y);
                            writer.WriteNextCol(quad.Vertex2.z);
                            writer.WriteNextCol(quad.Vertex3.x);
                            writer.WriteNextCol(quad.Vertex3.y);
                            writer.WriteNextCol(quad.Vertex3.z);
                            writer.WriteNextCol(quad.Precomputed0.x);
                            writer.WriteNextCol(quad.Precomputed0.y);
                            writer.WriteNextCol(quad.Precomputed0.z);
                            writer.WriteNextCol(quad.Precomputed1.x);
                            writer.WriteNextCol(quad.Precomputed1.y);
                            writer.WriteNextCol(quad.Precomputed1.z);
                            writer.WriteNextCol(quad.Precomputed2.x);
                            writer.WriteNextCol(quad.Precomputed2.y);
                            writer.WriteNextCol(quad.Precomputed2.z);
                            writer.WriteNextCol(quad.Precomputed3.x);
                            writer.WriteNextCol(quad.Precomputed3.y);
                            writer.WriteNextCol(quad.Precomputed3.z);

                            writer.WriteNextRow();
                        }
                        gameObjectIndex++;
                    }
                }
                writer.Flush();
            }
        }

        #endregion


        public static void AnalyzeHeaders(Scene[] scenes, string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Index");
                writer.WriteNextCol("Venue");
                writer.WriteNextCol("Course");
                writer.WriteNextCol("AX/GX");
                //
                writer.WriteNextCol(nameof(Scene.unkRange0x00) + "." + nameof(ViewRange.near));
                writer.WriteNextCol(nameof(Scene.unkRange0x00) + "." + nameof(ViewRange.far));
                writer.WriteNextCol(nameof(Scene.trackNodesPtr));
                writer.WriteNextCol(nameof(Scene.trackNodesPtr));
                writer.WriteNextCol(nameof(Scene.embeddedTrackPropertyAreasPtr));
                writer.WriteNextCol(nameof(Scene.embeddedTrackPropertyAreasPtr));
                writer.WriteNextCol(nameof(Scene.staticColliderMeshManagerActive));
                writer.WriteNextCol(nameof(Scene.embeddedTrackPropertyAreasPtr));
                writer.WriteNextCol(nameof(Scene.zeroes0x20Ptr));
                writer.WriteNextCol(nameof(Scene.trackMinHeightPtr));
                writer.WriteNextCol(nameof(Scene.zeroes0x28));
                writer.WriteNextCol(nameof(Scene.dynamicSceneObjectCount));
                writer.WriteNextCol(nameof(Scene.unk_sceneObjectCount1));
                writer.WriteNextCol(nameof(Scene.unk_sceneObjectCount2));
                writer.WriteNextCol(nameof(Scene.dynamicSceneObjectsPtr));
                writer.WriteNextCol(nameof(Scene.unkBool32_0x58));
                writer.WriteNextCol(nameof(Scene.unknownCollidersPtr));
                writer.WriteNextCol(nameof(Scene.unknownCollidersPtr));
                writer.WriteNextCol(nameof(Scene.sceneObjectsPtr));
                writer.WriteNextCol(nameof(Scene.sceneObjectsPtr));
                writer.WriteNextCol(nameof(Scene.staticSceneObjectsPtr));
                writer.WriteNextCol(nameof(Scene.staticSceneObjectsPtr));
                writer.WriteNextCol(nameof(Scene.zero0x74));
                writer.WriteNextCol(nameof(Scene.zero0x78));
                writer.WriteNextCol(nameof(Scene.circuitType));
                writer.WriteNextCol(nameof(Scene.fogCurvesPtr));
                writer.WriteNextCol(nameof(Scene.fogPtr));
                writer.WriteNextCol(nameof(Scene.zero0x88));
                writer.WriteNextCol(nameof(Scene.zero0x8C));
                writer.WriteNextCol(nameof(Scene.trackLengthPtr));
                writer.WriteNextCol(nameof(Scene.unknownTriggersPtr)); // len
                writer.WriteNextCol(nameof(Scene.unknownTriggersPtr)); // adr
                writer.WriteNextCol(nameof(Scene.visualEffectTriggersPtr)); // len
                writer.WriteNextCol(nameof(Scene.visualEffectTriggersPtr)); // adr
                writer.WriteNextCol(nameof(Scene.miscellaneousTriggersPtr)); // len
                writer.WriteNextCol(nameof(Scene.miscellaneousTriggersPtr)); // adr
                writer.WriteNextCol(nameof(Scene.timeExtensionTriggersPtr)); // len
                writer.WriteNextCol(nameof(Scene.timeExtensionTriggersPtr)); // adr
                writer.WriteNextCol(nameof(Scene.storyObjectTriggersPtr)); // len
                writer.WriteNextCol(nameof(Scene.storyObjectTriggersPtr)); // adr
                writer.WriteNextCol(nameof(Scene.checkpointGridPtr));
                // Structure
                writer.WriteNextCol(nameof(Scene.checkpointGridXZ) + "." + nameof(Scene.checkpointGridXZ.Left));
                writer.WriteNextCol(nameof(Scene.checkpointGridXZ) + "." + nameof(Scene.checkpointGridXZ.Top));
                writer.WriteNextCol(nameof(Scene.checkpointGridXZ) + "." + nameof(Scene.checkpointGridXZ.SubdivisionWidth));
                writer.WriteNextCol(nameof(Scene.checkpointGridXZ) + "." + nameof(Scene.checkpointGridXZ.SubdivisionLength));
                writer.WriteNextCol(nameof(Scene.checkpointGridXZ) + "." + nameof(Scene.checkpointGridXZ.NumSubdivisionsX));
                writer.WriteNextCol(nameof(Scene.checkpointGridXZ) + "." + nameof(Scene.checkpointGridXZ.NumSubdivisionsZ));
                // 
                writer.WriteNextCol(nameof(Scene.zeroes0xD8));
                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    writer.WriteNextCol(scene.FileName);
                    writer.WriteNextCol(scene.ID);
                    writer.WriteNextCol(CourseUtility.GetVenueID(scene.ID).GetDescription());
                    writer.WriteNextCol(((CourseIndexAX)scene.ID).GetDescription());
                    writer.WriteNextCol(scene.IsFileGX ? "GX" : "AX");

                    writer.WriteNextCol(scene.unkRange0x00.near);
                    writer.WriteNextCol(scene.unkRange0x00.far);
                    writer.WriteNextCol(scene.trackNodesPtr.Length);
                    writer.WriteNextCol(scene.trackNodesPtr.PrintAddress);
                    writer.WriteNextCol(scene.embeddedTrackPropertyAreasPtr.Length);
                    writer.WriteNextCol(scene.embeddedTrackPropertyAreasPtr.PrintAddress);
                    writer.WriteNextCol(scene.staticColliderMeshManagerActive);
                    writer.WriteNextCol(scene.staticColliderMeshManagerPtr.PrintAddress);
                    writer.WriteNextCol(scene.zeroes0x20Ptr.PrintAddress);
                    writer.WriteNextCol(scene.trackMinHeightPtr.PrintAddress);
                    writer.WriteNextCol(0);// coliHeader.zero_0x28);
                    writer.WriteNextCol(scene.dynamicSceneObjectCount);
                    if (scene.IsFileGX)
                    {
                        writer.WriteNextCol(scene.unk_sceneObjectCount1);
                    }
                    else // is AX
                    {
                        writer.WriteNextCol();
                    }
                    writer.WriteNextCol(scene.unk_sceneObjectCount2);
                    writer.WriteNextCol(scene.dynamicSceneObjectsPtr.PrintAddress);
                    writer.WriteNextCol(scene.unkBool32_0x58);
                    writer.WriteNextCol(scene.unknownCollidersPtr.Length);
                    writer.WriteNextCol(scene.unknownCollidersPtr.PrintAddress);
                    writer.WriteNextCol(scene.sceneObjectsPtr.Length);
                    writer.WriteNextCol(scene.sceneObjectsPtr.PrintAddress);
                    writer.WriteNextCol(scene.staticSceneObjectsPtr.Length);
                    writer.WriteNextCol(scene.staticSceneObjectsPtr.PrintAddress);
                    writer.WriteNextCol(scene.zero0x74);
                    writer.WriteNextCol(scene.zero0x78);
                    writer.WriteNextCol(scene.circuitType);
                    writer.WriteNextCol(scene.fogCurvesPtr.PrintAddress);
                    writer.WriteNextCol(scene.fogPtr.PrintAddress);
                    writer.WriteNextCol(scene.zero0x88);
                    writer.WriteNextCol(scene.zero0x8C);
                    writer.WriteNextCol(scene.trackLengthPtr.PrintAddress);
                    writer.WriteNextCol(scene.unknownTriggersPtr.Length);
                    writer.WriteNextCol(scene.unknownTriggersPtr.PrintAddress);
                    writer.WriteNextCol(scene.visualEffectTriggersPtr.Length);
                    writer.WriteNextCol(scene.visualEffectTriggersPtr.PrintAddress);
                    writer.WriteNextCol(scene.miscellaneousTriggersPtr.Length);
                    writer.WriteNextCol(scene.miscellaneousTriggersPtr.PrintAddress);
                    writer.WriteNextCol(scene.timeExtensionTriggersPtr.Length);
                    writer.WriteNextCol(scene.timeExtensionTriggersPtr.PrintAddress);
                    writer.WriteNextCol(scene.storyObjectTriggersPtr.Length);
                    writer.WriteNextCol(scene.storyObjectTriggersPtr.PrintAddress);
                    writer.WriteNextCol(scene.checkpointGridPtr.PrintAddress);
                    // Structure
                    writer.WriteNextCol(scene.checkpointGridXZ.Left);
                    writer.WriteNextCol(scene.checkpointGridXZ.Top);
                    writer.WriteNextCol(scene.checkpointGridXZ.SubdivisionWidth);
                    writer.WriteNextCol(scene.checkpointGridXZ.SubdivisionLength);
                    writer.WriteNextCol(scene.checkpointGridXZ.NumSubdivisionsX);
                    writer.WriteNextCol(scene.checkpointGridXZ.NumSubdivisionsZ);
                    //
                    writer.WriteNextCol(0);// coliHeader.zero_0xD8);
                    writer.WriteNextCol(scene.trackMinHeight.Value);
                    writer.WriteNextRow();
                }
                writer.Flush();
            }
        }


        #region TRIGGERS

        public static void AnalyzeArcadeCheckpointTriggers(Scene[] scenes, string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Index");
                writer.WriteNextCol("Venue");
                writer.WriteNextCol("Course");
                writer.WriteNextCol("AX/GX");
                //
                writer.WriteNextCol(nameof(TimeExtensionTrigger.transform.Position));
                writer.WriteNextCol(nameof(TimeExtensionTrigger.transform.RotationEuler));
                writer.WriteNextCol(nameof(TimeExtensionTrigger.transform.Scale));
                writer.WriteNextCol(nameof(TimeExtensionTrigger.transform.UnknownOption));
                writer.WriteNextCol(nameof(TimeExtensionTrigger.option));
                //
                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    var venueID = CourseUtility.GetVenueID(scene.ID).GetDescription();
                    var courseID = ((CourseIndexAX)scene.ID).GetDescription();
                    var isAxGx = scene.IsFileGX ? "GX" : "AX";

                    foreach (var arcadeCheckpooint in scene.timeExtensionTriggers)
                    {
                        writer.WriteNextCol(scene.FileName);
                        writer.WriteNextCol(scene.ID);
                        writer.WriteNextCol(venueID);
                        writer.WriteNextCol(courseID);
                        writer.WriteNextCol(isAxGx);
                        //
                        writer.WriteNextCol(arcadeCheckpooint.transform.Position);
                        writer.WriteNextCol(arcadeCheckpooint.transform.RotationEuler);
                        writer.WriteNextCol(arcadeCheckpooint.transform.Scale);
                        writer.WriteNextCol(arcadeCheckpooint.transform.UnknownOption);
                        writer.WriteNextCol(arcadeCheckpooint.option);
                        //
                        writer.WriteNextRow();
                    }
                    writer.Flush();
                }
            }
        }

        public static void AnalyzeCourseMetadataTriggers(Scene[] scenes, string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Index");
                writer.WriteNextCol("Venue");
                writer.WriteNextCol("Course");
                writer.WriteNextCol("AX/GX");
                //
                writer.WriteNextCol(nameof(MiscellaneousTrigger.Position));
                writer.WriteNextCol(nameof(MiscellaneousTrigger.RotationEuler));
                writer.WriteNextCol(nameof(MiscellaneousTrigger.Scale) + " / PositionTo");
                writer.WriteNextCol(nameof(MiscellaneousTrigger.Transform.UnknownOption));
                writer.WriteNextCol(nameof(MiscellaneousTrigger.MetadataType));
                //
                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    var venueID = CourseUtility.GetVenueID(scene.ID).GetDescription();
                    var courseID = ((CourseIndexAX)scene.ID).GetDescription();
                    var isAxGx = scene.IsFileGX ? "GX" : "AX";

                    foreach (var cmt in scene.miscellaneousTriggers)
                    {
                        writer.WriteNextCol(scene.FileName);
                        writer.WriteNextCol(scene.ID);
                        writer.WriteNextCol(venueID);
                        writer.WriteNextCol(courseID);
                        writer.WriteNextCol(isAxGx);
                        //
                        writer.WriteNextCol(cmt.Position);
                        writer.WriteNextCol(cmt.RotationEuler);
                        writer.WriteNextCol(cmt.Scale);
                        writer.WriteNextCol(cmt.Transform.UnknownOption);
                        writer.WriteNextCol(cmt.MetadataType);
                        //
                        writer.WriteNextRow();
                    }
                    writer.Flush();
                }
            }
        }

        public static void AnalyzeStoryObjectTrigger(Scene[] scenes, string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Index");
                writer.WriteNextCol("Venue");
                writer.WriteNextCol("Course");
                writer.WriteNextCol("AX/GX");
                //
                writer.WriteNextCol(nameof(StoryObjectTrigger.zero_0x00));
                writer.WriteNextCol(nameof(StoryObjectTrigger.rockGroupOrderIndex));
                writer.WriteNextCol(nameof(StoryObjectTrigger.RockGroup));
                writer.WriteNextCol(nameof(StoryObjectTrigger.Difficulty));
                writer.WriteNextCol(nameof(StoryObjectTrigger.story2RockScale));
                writer.WriteNextCol(nameof(StoryObjectTrigger.storyObjectPathPtr));
                writer.WriteNextCol(nameof(StoryObjectTrigger.scale));
                writer.WriteNextCol(nameof(StoryObjectTrigger.rotation));
                writer.WriteNextCol(nameof(StoryObjectTrigger.position));
                //
                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    var venueID = CourseUtility.GetVenueID(scene.ID).GetDescription();
                    var courseID = ((CourseIndexAX)scene.ID).GetDescription();
                    var isAxGx = scene.IsFileGX ? "GX" : "AX";

                    foreach (var item in scene.storyObjectTriggers)
                    {
                        writer.WriteNextCol(scene.FileName);
                        writer.WriteNextCol(scene.ID);
                        writer.WriteNextCol(venueID);
                        writer.WriteNextCol(courseID);
                        writer.WriteNextCol(isAxGx);
                        //
                        writer.WriteNextCol(item.zero_0x00);
                        writer.WriteNextCol(item.rockGroupOrderIndex);
                        writer.WriteNextCol(item.RockGroup);
                        writer.WriteNextCol(item.Difficulty);
                        writer.WriteNextCol(item.story2RockScale);
                        writer.WriteNextCol(item.storyObjectPathPtr);
                        writer.WriteNextCol(item.scale);
                        writer.WriteNextCol(item.rotation);
                        writer.WriteNextCol(item.position);
                        //
                        writer.WriteNextRow();
                    }
                    writer.Flush();
                }
            }
        }

        public static void AnalyzeUnknownTrigger(Scene[] scenes, string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Index");
                writer.WriteNextCol("Venue");
                writer.WriteNextCol("Course");
                writer.WriteNextCol("AX/GX");
                //
                writer.WriteNextCol("Start");
                writer.WriteNextCol("End");
                //
                writer.WriteNextCol(nameof(CullOverrideTrigger.Unk_0x20));
                writer.WriteNextCol(nameof(CullOverrideTrigger.Unk_0x20));
                //
                writer.WriteNextCol("Order");
                writer.WriteNextCol("Index");
                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    var venueID = CourseUtility.GetVenueID(scene.ID).GetDescription();
                    var courseID = ((CourseIndexAX)scene.ID).GetDescription();
                    var isAxGx = scene.IsFileGX ? "GX" : "AX";

                    int count = 0;
                    int total = scene.unknownTriggers.Length;
                    foreach (var item in scene.unknownTriggers)
                    {
                        count++;

                        writer.WriteNextCol(scene.FileName);
                        writer.WriteNextCol(scene.ID);
                        writer.WriteNextCol(venueID);
                        writer.WriteNextCol(courseID);
                        writer.WriteNextCol(isAxGx);

                        writer.WriteNextCol(item.PrintStartAddress());
                        writer.WriteNextCol(item.PrintEndAddress());

                        writer.WriteNextCol(item.Unk_0x20);
                        writer.WriteNextCol($"0x{(int)item.Unk_0x20:X8}");

                        writer.WriteNextCol(count);
                        writer.WriteNextCol($"[{count}/{total}]");

                        writer.WriteNextRow();
                    }
                }
                writer.Flush();
            }
        }

        public static void AnalyzeVisualEffectTriggers(Scene[] scenes, string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Index");
                writer.WriteNextCol("Venue");
                writer.WriteNextCol("Course");
                writer.WriteNextCol("AX/GX");
                //
                writer.WriteNextCol(nameof(VisualEffectTrigger.transform.Position));
                writer.WriteNextCol(nameof(VisualEffectTrigger.transform.RotationEuler));
                writer.WriteNextCol(nameof(VisualEffectTrigger.transform.Scale));
                writer.WriteNextCol(nameof(VisualEffectTrigger.transform.UnknownOption));
                writer.WriteNextCol(nameof(VisualEffectTrigger.animation));
                writer.WriteNextCol(nameof(VisualEffectTrigger.visualEffect));
                //
                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    var venueID = CourseUtility.GetVenueID(scene.ID).GetDescription();
                    var courseID = ((CourseIndexAX)scene.ID).GetDescription();
                    var isAxGx = scene.IsFileGX ? "GX" : "AX";

                    foreach (var vfx in scene.visualEffectTriggers)
                    {
                        writer.WriteNextCol(scene.FileName);
                        writer.WriteNextCol(scene.ID);
                        writer.WriteNextCol(venueID);
                        writer.WriteNextCol(courseID);
                        writer.WriteNextCol(isAxGx);
                        //
                        writer.WriteNextCol(vfx.transform.Position);
                        writer.WriteNextCol(vfx.transform.RotationEuler);
                        writer.WriteNextCol(vfx.transform.Scale);
                        writer.WriteNextCol(vfx.transform.UnknownOption);
                        writer.WriteNextCol(vfx.animation);
                        writer.WriteNextCol(vfx.visualEffect);
                        //
                        writer.WriteNextRow();
                    }
                    writer.Flush();
                }
            }
        }

        #endregion

        #region FOG

        public static void AnalyzeFogCurves(Scene[] scenes, string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Index");
                writer.WriteNextCol("Venue");
                writer.WriteNextCol("Course");
                writer.WriteNextCol("AX/GX");
                //
                writer.WriteNextCol("Index");
                //
                writer.WriteNextCol(nameof(KeyableAttribute.EaseMode));
                writer.WriteNextCol(nameof(KeyableAttribute.Time));
                writer.WriteNextCol(nameof(KeyableAttribute.Value));
                writer.WriteNextCol(nameof(KeyableAttribute.TangentIn));
                writer.WriteNextCol(nameof(KeyableAttribute.TangentOut));
                //
                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    if (scene.fogCurves == null)
                        continue;

                    var venueID = CourseUtility.GetVenueID(scene.ID).GetDescription();
                    var courseID = ((CourseIndexAX)scene.ID).GetDescription();
                    var isAxGx = scene.IsFileGX ? "GX" : "AX";

                    var totalD1 = scene.fogCurves.animationCurves.Length;
                    var countD1 = 0;
                    foreach (var animationCurve in scene.fogCurves.animationCurves)
                    {
                        countD1++;
                        foreach (var keyableAttribute in animationCurve.KeyableAttributes)
                        {
                            writer.WriteNextCol(scene.FileName);
                            writer.WriteNextCol(scene.ID);
                            writer.WriteNextCol(venueID);
                            writer.WriteNextCol(courseID);
                            writer.WriteNextCol(isAxGx);
                            //
                            writer.WriteNextCol($"[{countD1}/{totalD1}]");
                            //
                            writer.WriteNextCol(keyableAttribute.EaseMode);
                            writer.WriteNextCol(keyableAttribute.Time);
                            writer.WriteNextCol(keyableAttribute.Value);
                            writer.WriteNextCol(keyableAttribute.TangentIn);
                            writer.WriteNextCol(keyableAttribute.TangentOut);
                            //
                            writer.WriteNextRow();
                        }
                    }
                    writer.Flush();
                }
            }
        }

        public static void AnalyzeFog(Scene[] scenes, string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Index");
                writer.WriteNextCol("Venue");
                writer.WriteNextCol("Course");
                writer.WriteNextCol("AX/GX");
                //
                writer.WriteNextCol(nameof(Fog.Interpolation));
                writer.WriteNextCol(nameof(Fog.FogRange) + "." + nameof(ViewRange.near));
                writer.WriteNextCol(nameof(Fog.FogRange) + "." + nameof(ViewRange.far));
                writer.WriteNextCol(nameof(Fog.ColorRGB) + ".R");
                writer.WriteNextCol(nameof(Fog.ColorRGB) + ".G");
                writer.WriteNextCol(nameof(Fog.ColorRGB) + ".B");
                //writer.WriteNextCol(nameof(Fog.zero0x18) + ".x");
                //writer.WriteNextCol(nameof(Fog.zero0x18) + ".y");
                //writer.WriteNextCol(nameof(Fog.zero0x18) + ".z");
                //
                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    var venueID = CourseUtility.GetVenueID(scene.ID).GetDescription();
                    var courseID = ((CourseIndexAX)scene.ID).GetDescription();
                    var isAxGx = scene.IsFileGX ? "GX" : "AX";

                    writer.WriteNextCol(scene.FileName);
                    writer.WriteNextCol(scene.ID);
                    writer.WriteNextCol(venueID);
                    writer.WriteNextCol(courseID);
                    writer.WriteNextCol(isAxGx);
                    //
                    writer.WriteNextCol(scene.fog.Interpolation);
                    writer.WriteNextCol(scene.fog.FogRange.near);
                    writer.WriteNextCol(scene.fog.FogRange.far);
                    writer.WriteNextCol(scene.fog.ColorRGB.x);
                    writer.WriteNextCol(scene.fog.ColorRGB.y);
                    writer.WriteNextCol(scene.fog.ColorRGB.z);
                    //writer.WriteNextCol(scene.fog.zero0x18.x);
                    //writer.WriteNextCol(scene.fog.zero0x18.y);
                    //writer.WriteNextCol(scene.fog.zero0x18.z);
                    //
                    writer.WriteNextRow();
                }
                writer.Flush();
            }
        }

        #endregion


        public static void AnalyzeSceneObjectTransforms(Scene[] scenes, string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Game Object #");
                writer.WriteNextCol("Game Object");
                writer.WriteNextCol($"matrix.x");
                writer.WriteNextCol($"matrix.y");
                writer.WriteNextCol($"matrix.z");
                writer.WriteNextCol($"euler.x");
                writer.WriteNextCol($"euler.y");
                writer.WriteNextCol($"euler.z");
                writer.WriteNextCol("Decomposed phi");
                writer.WriteNextCol("Decomposed theta");
                writer.WriteNextCol("Decomposed psi");
                writer.WriteNextCol(nameof(UnknownTransformOption));
                writer.WriteNextCol(nameof(ObjectActiveOverride));
                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    int sceneObjectIndex = 0;
                    foreach (var sceneObject in scene.dynamicSceneObjects)
                    {
                        // Skip objects that don;'t have both matrix and decomposed rotation
                        // These are not helpful for comparision
                        if (!sceneObject.transformMatrix3x4Ptr.IsNotNull)
                            continue;

                        writer.WriteNextCol(scene.FileName);
                        writer.WriteNextCol(sceneObjectIndex);
                        writer.WriteNextCol(sceneObject.Name);

                        // Rotation values from clean, uncompressed matrix
                        var matrix = sceneObject.transformMatrix3x4.Rotation;
                        writer.WriteNextCol(matrix.x);
                        writer.WriteNextCol(matrix.y);
                        writer.WriteNextCol(matrix.z);

                        // Rotation values as reconstructed
                        var euler = sceneObject.transformPRXS.CompressedRotation.Eulers;
                        writer.WriteNextCol(euler.x);
                        writer.WriteNextCol(euler.y);
                        writer.WriteNextCol(euler.z);

                        // Decomposed rotation values, raw, requires processing to be used
                        var decomposed = sceneObject.transformPRXS.CompressedRotation;
                        writer.WriteNextCol(decomposed.X);
                        writer.WriteNextCol(decomposed.Y);
                        writer.WriteNextCol(decomposed.Z);
                        // The other parameters that go with the structure
                        writer.WriteNextCol(sceneObject.transformPRXS.UnknownOption);
                        writer.WriteNextCol(sceneObject.transformPRXS.ObjectActiveOverride);

                        writer.WriteNextRow();
                        sceneObjectIndex++;
                    }
                }
                writer.Flush();
            }
        }

        public static void AnalyzeTrackNodes(Scene[] scenes, string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Track Node");
                writer.WriteNextCol("Track Point");
                writer.WriteNextColNicify(nameof(Checkpoint.CurveTimeStart));
                writer.WriteNextColNicify(nameof(Checkpoint.CurveTimeEnd));
                writer.WriteNextColNicify(nameof(Checkpoint.PlaneStart.dotProduct));
                writer.WriteNextColNicify(nameof(Checkpoint.PlaneStart.normal) + ".x");
                writer.WriteNextColNicify(nameof(Checkpoint.PlaneStart.normal) + ".y");
                writer.WriteNextColNicify(nameof(Checkpoint.PlaneStart.normal) + ".z");
                writer.WriteNextColNicify(nameof(Checkpoint.PlaneStart.origin) + ".x");
                writer.WriteNextColNicify(nameof(Checkpoint.PlaneStart.origin) + ".y");
                writer.WriteNextColNicify(nameof(Checkpoint.PlaneStart.origin) + ".z");
                writer.WriteNextColNicify(nameof(Checkpoint.PlaneEnd.dotProduct));
                writer.WriteNextColNicify(nameof(Checkpoint.PlaneEnd.normal) + ".x");
                writer.WriteNextColNicify(nameof(Checkpoint.PlaneEnd.normal) + ".y");
                writer.WriteNextColNicify(nameof(Checkpoint.PlaneEnd.normal) + ".z");
                writer.WriteNextColNicify(nameof(Checkpoint.PlaneEnd.origin) + ".x");
                writer.WriteNextColNicify(nameof(Checkpoint.PlaneEnd.origin) + ".y");
                writer.WriteNextColNicify(nameof(Checkpoint.PlaneEnd.origin) + ".z");
                writer.WriteNextColNicify(nameof(Checkpoint.StartDistance));
                writer.WriteNextColNicify(nameof(Checkpoint.EndDistance));
                writer.WriteNextColNicify(nameof(Checkpoint.TrackWidth));
                writer.WriteNextColNicify(nameof(Checkpoint.ConnectToTrackIn));
                writer.WriteNextColNicify(nameof(Checkpoint.ConnectToTrackOut));
                //writer.WriteNextColNicify(nameof(Checkpoint.zero_0x4E));
                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    int nodeLength = scene.trackNodes.Length;
                    int nodeIndex = 0;
                    foreach (var trackNode in scene.trackNodes)
                    {
                        int pointLength = trackNode.checkpoints.Length;
                        int pointIndex = 0;
                        foreach (var trackPoint in trackNode.checkpoints)
                        {
                            writer.WriteNextCol($"COLI_COURSE{scene.ID:d2}");
                            writer.WriteNextCol($"[{nodeIndex}/{nodeLength}]");
                            writer.WriteNextCol($"[{pointIndex}/{pointLength}]");

                            writer.WriteNextCol(trackPoint.CurveTimeStart);
                            writer.WriteNextCol(trackPoint.CurveTimeEnd);
                            writer.WriteNextCol(trackPoint.PlaneStart.dotProduct);
                            writer.WriteNextCol(trackPoint.PlaneStart.normal.x);
                            writer.WriteNextCol(trackPoint.PlaneStart.normal.y);
                            writer.WriteNextCol(trackPoint.PlaneStart.normal.z);
                            writer.WriteNextCol(trackPoint.PlaneStart.origin.x);
                            writer.WriteNextCol(trackPoint.PlaneStart.origin.y);
                            writer.WriteNextCol(trackPoint.PlaneStart.origin.z);
                            writer.WriteNextCol(trackPoint.PlaneEnd.dotProduct);
                            writer.WriteNextCol(trackPoint.PlaneEnd.normal.x);
                            writer.WriteNextCol(trackPoint.PlaneEnd.normal.y);
                            writer.WriteNextCol(trackPoint.PlaneEnd.normal.z);
                            writer.WriteNextCol(trackPoint.PlaneEnd.origin.x);
                            writer.WriteNextCol(trackPoint.PlaneEnd.origin.y);
                            writer.WriteNextCol(trackPoint.PlaneEnd.origin.z);
                            writer.WriteNextCol(trackPoint.StartDistance);
                            writer.WriteNextCol(trackPoint.EndDistance);
                            writer.WriteNextCol(trackPoint.TrackWidth);
                            writer.WriteNextCol(trackPoint.ConnectToTrackIn);
                            writer.WriteNextCol(trackPoint.ConnectToTrackOut);
                            //writer.WriteNextCol(trackPoint.zero_0x4E);
                            writer.WriteNextRow();

                            pointIndex++;
                        }
                        nodeIndex++;
                    }
                }
                writer.Flush();
            }
        }

        public static void AnalyzeStaticColliderMeshes(Scene[] scenes, string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Index");
                writer.WriteNextColNicify(nameof(StaticColliderMeshManager.staticColliderTrisPtr));
                writer.WriteNextColNicify(nameof(StaticColliderMeshManager.triMeshGridPtrs));
                writer.WriteNextColNicify(nameof(GridXZ.Left));
                writer.WriteNextColNicify(nameof(GridXZ.Top));
                writer.WriteNextColNicify(nameof(GridXZ.SubdivisionWidth));
                writer.WriteNextColNicify(nameof(GridXZ.SubdivisionLength));
                writer.WriteNextColNicify(nameof(GridXZ.NumSubdivisionsX));
                writer.WriteNextColNicify(nameof(GridXZ.NumSubdivisionsZ));
                writer.WriteNextColNicify(nameof(StaticColliderMeshManager.staticColliderQuadsPtr));
                writer.WriteNextColNicify(nameof(StaticColliderMeshManager.quadMeshGridPtrs));
                writer.WriteNextColNicify(nameof(StaticColliderMeshManager.boundingSpherePtr));
                writer.WriteNextColNicify(nameof(StaticColliderMeshManager.staticSceneObjectsPtr));
                writer.WriteNextColNicify(nameof(StaticColliderMeshManager.unknownCollidersPtr));
                writer.WriteNextColNicify(nameof(StaticColliderMeshManager.unk_float));
                writer.WriteNextCol();
                writer.WriteNextColNicify(nameof(BoundingSphere.origin) + ".x");
                writer.WriteNextColNicify(nameof(BoundingSphere.origin) + ".x");
                writer.WriteNextColNicify(nameof(BoundingSphere.origin) + ".x");
                writer.WriteNextColNicify(nameof(BoundingSphere.radius));
                writer.WriteNextRow();


                int index = 0;

                foreach (var scene in scenes)
                {
                    var staticColliderMeshes = scene.staticColliderMeshManager;

                    writer.WriteNextCol($"COLI_COURSE{scene.ID:d2}");
                    writer.WriteNextCol(index++);
                    writer.WriteNextCol(staticColliderMeshes.staticColliderTrisPtr.PrintAddress);
                    writer.WriteNextCol(staticColliderMeshes.triMeshGridPtrs.Length);
                    writer.WriteNextCol(staticColliderMeshes.meshGridXZ.Left);
                    writer.WriteNextCol(staticColliderMeshes.meshGridXZ.Top);
                    writer.WriteNextCol(staticColliderMeshes.meshGridXZ.SubdivisionWidth);
                    writer.WriteNextCol(staticColliderMeshes.meshGridXZ.SubdivisionLength);
                    writer.WriteNextCol(staticColliderMeshes.meshGridXZ.NumSubdivisionsX);
                    writer.WriteNextCol(staticColliderMeshes.meshGridXZ.NumSubdivisionsZ);
                    writer.WriteNextCol(staticColliderMeshes.staticColliderQuadsPtr.PrintAddress);
                    writer.WriteNextCol(staticColliderMeshes.quadMeshGridPtrs.Length);
                    writer.WriteNextCol(staticColliderMeshes.boundingSpherePtr.PrintAddress);
                    writer.WriteNextCol(staticColliderMeshes.staticSceneObjectsPtr.PrintAddress);
                    writer.WriteNextCol(staticColliderMeshes.unknownCollidersPtr.PrintAddress);
                    writer.WriteNextCol(staticColliderMeshes.unk_float);
                    writer.WriteNextCol();
                    writer.WriteNextCol(staticColliderMeshes.BoundingSphere.origin.x);
                    writer.WriteNextCol(staticColliderMeshes.BoundingSphere.origin.y);
                    writer.WriteNextCol(staticColliderMeshes.BoundingSphere.origin.z);
                    writer.WriteNextCol(staticColliderMeshes.BoundingSphere.radius);
                    writer.WriteNextRow();
                }
                writer.Flush();
            }
        }

        public static void AnalyzeSceneObjectLODs(Scene[] scenes, string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Index");
                writer.WriteNextCol("Venue");
                writer.WriteNextCol("Course");
                writer.WriteNextCol("AX/GX");
                //
                writer.WriteNextCol("name");
                writer.WriteNextCol("Object Type");
                writer.WriteNextCol(nameof(SceneObjectLOD.zero_0x00));
                writer.WriteNextCol(nameof(SceneObjectLOD.lodNamePtr));
                writer.WriteNextCol(nameof(SceneObjectLOD.zero_0x08));
                writer.WriteNextCol(nameof(SceneObjectLOD.lodDistance));
                //
                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    var venueID = CourseUtility.GetVenueID(scene.ID).GetDescription();
                    var courseID = ((CourseIndexAX)scene.ID).GetDescription();
                    var isAxGx = scene.IsFileGX ? "GX" : "AX";

                    // Get all the scene object references
                    var objectsList = new List<SceneObjectLOD>();
                    foreach (var templateSceneObject in scene.sceneObjects)
                    {
                        var sceneObjects = templateSceneObject.lods;
                        foreach (var sceneObject in sceneObjects)
                            objectsList.Add(sceneObject);
                    }
                    //foreach (var staticSceneObject in scene.staticSceneObjects)
                    //{
                    //    var sceneObjects = staticSceneObject.templateSceneObject.sceneObjects;
                    //    foreach (var sceneObject in sceneObjects)
                    //        objectsList.Add((sceneObject, "Instance"));
                    //}

                    // iterate
                    foreach (var sceneObjectReference in objectsList)
                    {
                        writer.WriteNextCol(scene.FileName);
                        writer.WriteNextCol(scene.ID);
                        writer.WriteNextCol(venueID);
                        writer.WriteNextCol(courseID);
                        writer.WriteNextCol(isAxGx);
                        //
                        writer.WriteNextCol(sceneObjectReference.name);
                        writer.WriteNextCol(sceneObjectReference.zero_0x00);
                        writer.WriteNextCol(sceneObjectReference.lodNamePtr);
                        writer.WriteNextCol(sceneObjectReference.zero_0x08);
                        writer.WriteNextCol(sceneObjectReference.lodDistance);
                        //
                        writer.WriteNextRow();
                    }
                }
                writer.Flush();
            }
        }

        public static void AnalyzeSceneObjects(Scene[] scenes, string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Index");
                writer.WriteNextCol("Venue");
                writer.WriteNextCol("Course");
                writer.WriteNextCol("AX/GX");
                //
                writer.WriteNextCol("name");
                writer.WriteNextCol("Object Type");
                writer.WriteNextCol(nameof(SceneObject.lodRenderFlags));
                writer.WriteNextCol(nameof(SceneObject.lodsPtr));
                writer.WriteNextCol(nameof(SceneObject.colliderGeometryPtr));
                //
                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    var venueID = CourseUtility.GetVenueID(scene.ID).GetDescription();
                    var courseID = ((CourseIndexAX)scene.ID).GetDescription();
                    var isAxGx = scene.IsFileGX ? "GX" : "AX";

                    // Get all the scene object references
                    var sceneObjectsList = new List<(SceneObject sir, string category)>();
                    foreach (var sceneInstance in scene.sceneObjects)
                    {
                        sceneObjectsList.Add((sceneInstance, "Instance"));
                    }
                    foreach (var sceneOriginObject in scene.staticSceneObjects)
                    {
                        var sceneInstance = sceneOriginObject.sceneObject;
                        sceneObjectsList.Add((sceneInstance, "Origin"));
                    }

                    // iterate
                    foreach (var sceneObject in sceneObjectsList)
                    {
                        writer.WriteNextCol(scene.FileName);
                        writer.WriteNextCol(scene.ID);
                        writer.WriteNextCol(venueID);
                        writer.WriteNextCol(courseID);
                        writer.WriteNextCol(isAxGx);
                        //
                        writer.WriteNextCol(sceneObject.sir.PrimaryLOD.name);
                        writer.WriteNextCol(sceneObject.category);
                        writer.WriteNextCol(sceneObject.sir.lodRenderFlags);
                        writer.WriteNextCol(sceneObject.sir.lodsPtr);
                        writer.WriteNextCol(sceneObject.sir.colliderGeometryPtr);
                        //
                        writer.WriteNextRow();
                    }
                }
                writer.Flush();
            }
        }

        public static void AnalyzeSceneObjectsAndLODs(Scene[] scenes, string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Index");
                writer.WriteNextCol("Venue");
                writer.WriteNextCol("Course");
                writer.WriteNextCol("AX/GX");
                //
                writer.WriteNextCol("name");
                writer.WriteNextCol(nameof(SceneObject.lodRenderFlags));
                writer.WriteNextCol(nameof(SceneObject.lodsPtr) + " Len");
                writer.WriteNextCol(nameof(SceneObject.lodsPtr) + " Adr");
                writer.WriteNextCol(nameof(SceneObject.colliderGeometryPtr));
                writer.WriteNextCol(nameof(SceneObjectLOD) + " IDX");
                writer.WriteNextCol(nameof(SceneObjectLOD.zero_0x00));
                writer.WriteNextCol(nameof(SceneObjectLOD.lodNamePtr));
                writer.WriteNextCol(nameof(SceneObjectLOD.zero_0x08));
                writer.WriteNextCol(nameof(SceneObjectLOD.lodDistance));
                writer.WriteNextCol(nameof(SceneObjectLOD.name));
                //
                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    var venueID = CourseUtility.GetVenueID(scene.ID).GetDescription();
                    var courseID = ((CourseIndexAX)scene.ID).GetDescription();
                    var isAxGx = scene.IsFileGX ? "GX" : "AX";

                    foreach (var template in scene.sceneObjects)
                    {
                        var index = 0;
                        var length = template.lods.Length;
                        foreach (var sceneObject in template.lods)
                        {
                            writer.WriteNextCol(scene.FileName);
                            writer.WriteNextCol(scene.ID);
                            writer.WriteNextCol(venueID);
                            writer.WriteNextCol(courseID);
                            writer.WriteNextCol(isAxGx);
                            //
                            writer.WriteNextCol(template.Name);
                            writer.WriteNextCol(template.lodRenderFlags);
                            writer.WriteNextCol(template.lodsPtr.Length);
                            writer.WriteNextCol(template.lodsPtr.PrintAddress);
                            writer.WriteNextCol(template.colliderGeometryPtr);
                            writer.WriteNextCol($"[{++index}/{length}]");
                            writer.WriteNextCol(sceneObject.zero_0x00);
                            writer.WriteNextCol(sceneObject.lodNamePtr);
                            writer.WriteNextCol(sceneObject.zero_0x08);
                            writer.WriteNextCol(sceneObject.lodDistance);
                            writer.WriteNextCol(sceneObject.name);
                            writer.WriteNextRow();
                        }
                    }
                }
                writer.Flush();
            }
        }


        public static void AnalyzeGeneralData(Scene[] scenes, string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Index");
                writer.WriteNextCol("Venue");
                writer.WriteNextCol("Course");
                writer.WriteNextCol("AX/GX");
                //
                writer.WriteNextCol(nameof(ViewRange) + "." + nameof(ViewRange.near));
                writer.WriteNextCol(nameof(ViewRange) + "." + nameof(ViewRange.far));
                writer.WriteNextCol(nameof(Scene.trackMinHeight));
                writer.WriteNextCol(nameof(Scene.trackLength));
                //
                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    var venueID = CourseUtility.GetVenueID(scene.ID).GetDescription();
                    var courseID = ((CourseIndexAX)scene.ID).GetDescription();
                    var isAxGx = scene.IsFileGX ? "GX" : "AX";

                    writer.WriteNextCol(scene.FileName);
                    writer.WriteNextCol(scene.ID);
                    writer.WriteNextCol(venueID);
                    writer.WriteNextCol(courseID);
                    writer.WriteNextCol(isAxGx);
                    //
                    writer.WriteNextCol(scene.unkRange0x00.near);
                    writer.WriteNextCol(scene.unkRange0x00.far);
                    writer.WriteNextCol(scene.trackMinHeight.Value);
                    writer.WriteNextCol(scene.trackLength.Value);
                    writer.WriteNextRow();
                }
                writer.Flush();
            }
        }

        public static void AnalyzeSurfaceAttributeAreas(Scene[] scenes, string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Index");
                writer.WriteNextCol("Venue");
                writer.WriteNextCol("Course");
                writer.WriteNextCol("AX/GX");
                //
                writer.WriteNextCol(nameof(EmbeddedTrackPropertyArea.LengthFrom));
                writer.WriteNextCol(nameof(EmbeddedTrackPropertyArea.LengthTo));
                writer.WriteNextCol(nameof(EmbeddedTrackPropertyArea.WidthLeft));
                writer.WriteNextCol(nameof(EmbeddedTrackPropertyArea.WidthRight));
                writer.WriteNextCol(nameof(EmbeddedTrackPropertyArea.PropertyType));
                writer.WriteNextCol(nameof(EmbeddedTrackPropertyArea.TrackBranchID));
                //writer.WriteNextCol(nameof(EmbeddedTrackPropertyArea.zero_0x12));
                //
                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    var venueID = CourseUtility.GetVenueID(scene.ID).GetDescription();
                    var courseID = ((CourseIndexAX)scene.ID).GetDescription();
                    var isAxGx = scene.IsFileGX ? "GX" : "AX";

                    foreach (var surfaceAttributeArea in scene.embeddedPropertyAreas)
                    {
                        writer.WriteNextCol(scene.FileName);
                        writer.WriteNextCol(scene.ID);
                        writer.WriteNextCol(venueID);
                        writer.WriteNextCol(courseID);
                        writer.WriteNextCol(isAxGx);
                        //
                        writer.WriteNextCol(surfaceAttributeArea.LengthFrom);
                        writer.WriteNextCol(surfaceAttributeArea.LengthTo);
                        writer.WriteNextCol(surfaceAttributeArea.WidthLeft);
                        writer.WriteNextCol(surfaceAttributeArea.WidthRight);
                        writer.WriteNextCol(surfaceAttributeArea.PropertyType);
                        writer.WriteNextCol(surfaceAttributeArea.TrackBranchID);
                        //writer.WriteNextCol(surfaceAttributeArea.zero_0x12);
                        //
                        writer.WriteNextRow();
                    }
                }
                writer.Flush();
            }
        }


        public static void AnalyzeUnknownCollider(Scene[] scenes, string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Index");
                writer.WriteNextCol("Venue");
                writer.WriteNextCol("Course");
                writer.WriteNextCol("AX/GX");
                //
                writer.WriteNextCol(nameof(UnknownCollider.templateSceneObjectPtr));
                writer.WriteNextCol(nameof(UnknownCollider.transform.Position));
                writer.WriteNextCol(nameof(UnknownCollider.transform.RotationEuler));
                writer.WriteNextCol(nameof(UnknownCollider.transform.Scale));
                writer.WriteNextCol(nameof(UnknownCollider.transform.UnknownOption));
                //
                writer.WriteNextRow();

                foreach (var scene in scenes)
                {
                    var venueID = CourseUtility.GetVenueID(scene.ID).GetDescription();
                    var courseID = ((CourseIndexAX)scene.ID).GetDescription();
                    var isAxGx = scene.IsFileGX ? "GX" : "AX";

                    foreach (var unkSols in scene.unknownColliders)
                    {
                        writer.WriteNextCol(scene.FileName);
                        writer.WriteNextCol(scene.ID);
                        writer.WriteNextCol(venueID);
                        writer.WriteNextCol(courseID);
                        writer.WriteNextCol(isAxGx);
                        //
                        writer.WriteNextCol(unkSols.templateSceneObjectPtr);
                        writer.WriteNextCol(unkSols.transform.Position);
                        writer.WriteNextCol(unkSols.transform.RotationEuler);
                        writer.WriteNextCol(unkSols.transform.Scale);
                        writer.WriteNextCol(unkSols.transform.UnknownOption);
                        //
                        writer.WriteNextRow();
                    }
                    writer.Flush();
                }
            }
        }

    }
}
