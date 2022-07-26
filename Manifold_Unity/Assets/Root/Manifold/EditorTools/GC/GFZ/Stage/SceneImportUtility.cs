using GameCube.GFZ;
using GameCube.GFZ.Stage;
using Manifold.IO;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Unity.Mathematics;

using Manifold.EditorTools.GC.GFZ.Stage.Track;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public static class SceneImportUtility
    {
        // todo: make title consistent across progress bars.
        public static string ExecuteText => "Import COLI as Unity Scene";

        //[MenuItem(GfzMenuItems.Stage.Menu + "Midiman", priority = GfzMenuItems.Stage.ImportSingleSelectPriority + 1)]
        public static void ExportMidiman()
        {
            var settings = GfzProjectWindow.GetSettings();
            var inputPath = settings.SourceStageDirectory;
            var outputPath = settings.LogOutput;
            foreach (var scene in ColiCourseIO.LoadAllStages(inputPath, "???"))
            {
                var path = outputPath + scene.FileName + ".tsv";
                using (var writer = new StreamWriter(File.Create(path)))
                {
                    foreach (var model in scene.dynamicSceneObjects)
                    {
                        var name = model.Name;
                        var hasMatrix = model.TransformMatrix3x4 is not null;
                        var pos = hasMatrix ? model.TransformMatrix3x4.Position : model.TransformTRXS.Position;
                        var rot = hasMatrix ? model.TransformMatrix3x4.RotationEuler : model.TransformTRXS.RotationEuler;
                        var scl = hasMatrix ? model.TransformMatrix3x4.Scale : model.TransformTRXS.Scale;

                        writer.WriteNextCol(name);
                        writer.WriteNextCol(pos.x);
                        writer.WriteNextCol(pos.y);
                        writer.WriteNextCol(pos.z);
                        writer.WriteNextCol(rot.x);
                        writer.WriteNextCol(rot.y);
                        writer.WriteNextCol(rot.z);
                        writer.WriteNextCol(scl.x);
                        writer.WriteNextCol(scl.y);
                        writer.WriteNextCol(scl.z);
                        writer.WriteNextRow();
                    }
                }
            }
            EditorUtility.ClearProgressBar();
        }

        //[MenuItem(GfzMenuItems.Stage.Menu + "Midiman2", priority = GfzMenuItems.Stage.ImportSingleSelectPriority + 2)]
        public static void ExportMidiman2()
        {
            var settings = GfzProjectWindow.GetSettings();
            var inputPath = settings.SourceStageDirectory;
            var outputPath = settings.LogOutput;
            foreach (var scene in ColiCourseIO.LoadAllStages(inputPath, "???"))
            {
                var path = outputPath + scene.FileName + ".tsv";
                using (var writer = new StreamWriter(File.Create(path)))
                {
                    int trackNodeIndex = 0;
                    foreach (var trackNode in scene.trackNodes)
                    {
                        int checkpointIndex = 0;
                        foreach (var checkpoint in trackNode.Checkpoints)
                        {
                            var name = $"Checkpoint {trackNodeIndex}.{checkpointIndex}";
                            var pos0 = checkpoint.PlaneStart.origin;
                            var rot0 = Quaternion.LookRotation(checkpoint.PlaneStart.normal, Vector3.up).eulerAngles;
                            var pos1 = checkpoint.PlaneEnd.origin;
                            var rot1 = Quaternion.LookRotation(checkpoint.PlaneEnd.normal, Vector3.up).eulerAngles;

                            writer.WriteNextCol(name);
                            writer.WriteNextCol(pos0.x);
                            writer.WriteNextCol(pos0.y);
                            writer.WriteNextCol(pos0.z);
                            writer.WriteNextCol(rot0.x);
                            writer.WriteNextCol(rot0.y);
                            writer.WriteNextCol(rot0.z);
                            writer.WriteNextCol(pos1.x);
                            writer.WriteNextCol(pos1.y);
                            writer.WriteNextCol(pos1.z);
                            writer.WriteNextCol(rot1.x);
                            writer.WriteNextCol(rot1.y);
                            writer.WriteNextCol(rot1.z);
                            writer.WriteNextRow();
                            checkpointIndex++;
                        }
                        trackNodeIndex++;
                    }
                }
            }
            EditorUtility.ClearProgressBar();
        }


        [MenuItem(GfzMenuItems.Stage.ImportAll, priority = GfzMenuItems.Stage.ImportAllPriority)]
        public static void ImportAll()
        {
            var settings = GfzProjectWindow.GetSettings();
            var inputPath = settings.SourceStageDirectory;
            var outputPath = DirectoryUtility.GetTopDirectory(settings.SourceDirectory) + "/stage/";
            foreach (var scene in ColiCourseIO.LoadAllStages(inputPath, "Generating Unity Scene From GFZ Scene"))
            {
                Import(scene, outputPath);
            }
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        [MenuItem(GfzMenuItems.Stage.ImportSingle, priority = GfzMenuItems.Stage.ImportSinglePriority)]
        public static void ImportSingle()
        {
            var settings = GfzProjectWindow.GetSettings();
            var inputPath = settings.SourceStageDirectory;
            var outputPath = DirectoryUtility.GetTopDirectory(settings.SourceDirectory) + "/stage/";
            var sceneIndex = settings.SingleSceneIndex;
            var filePath = $"{inputPath}COLI_COURSE{sceneIndex:00}";
            var scene = ColiCourseIO.LoadScene(filePath);
            Import(scene, outputPath);
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        [MenuItem(GfzMenuItems.Stage.ImportSingleSelect, priority = GfzMenuItems.Stage.ImportSingleSelectPriority)]
        public static void ImportSingleSelect()
        {
            var settings = GfzProjectWindow.GetSettings();
            var filePath = EditorUtility.OpenFilePanel("Select Stage File", settings.SourceStageDirectory, "");
            if (string.IsNullOrEmpty(filePath))
                return;
            var outputPath = DirectoryUtility.GetTopDirectory(settings.SourceDirectory) + "/stage/";
            var scene = ColiCourseIO.LoadScene(filePath);
            Import(scene, outputPath);
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        public static void Import(Scene scene, string outputPath)
        {
            // Create new, empty scene
            var scenePath = $"Assets/{outputPath}{scene.FileName}.unity";
            var unityScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            AssetDatabaseUtility.CreateDirectoryForAsset(scenePath);
            EditorSceneManager.SaveScene(unityScene, scenePath);
            // Keep reference of new scene
            unityScene = EditorSceneManager.OpenScene(scenePath);

            // Course-related values, used to find models
            // Triple digit IDs do overflow the "00" format, that's okay.
            var stageID = scene.CourseIndex;
            var stageNumber = stageID.ToString("00");
            var venueID = CourseUtility.GetVenueID(stageID).ToString().ToLower();

            // TEMP: Get folder at root of import path. Find more elegant solution.
            var importFromRoot = outputPath.Split('/')[0];

            // Models are loaded from 3 folders.
            var initFolder = $"Assets/{importFromRoot}/init";
            var stageFolder = $"Assets/{importFromRoot}/stage/st{stageNumber}";
            var venueFolder = $"Assets/{importFromRoot}/bg/bg_{venueID}";
            var searchFolders = new string[] { initFolder, stageFolder, venueFolder };

            // Adds object with general info about the course.
            CreateGlobalParams(scene);

            // Create scene objects, static objects, and dynamic objects
            var rootTransforms = CreateAllSceneObjects(scene, searchFolders);
            CreateTrackTransformHierarchy(scene);
            CreateTrackIndexChains(scene);
            IncludeStaticMeshColliders(scene, stageFolder);
            CreateGridXZVisual(scene);
            CreateStaticMeshColliderManagerSphereBounds(scene);

            // TRIGGERS
            {
                // Create triggers, lump them all under 1 transform.
                var triggersRoot = new GameObject("Triggers").transform;
                var children = new List<Transform>();
                children.Add(CreateArcadeCheckpointTriggers(scene));
                children.Add(CreateCourseMetadataTriggers(scene));
                children.Add(CreateStoryObjectTriggers(scene));
                children.Add(CreateUnknownSolsTriggers(scene));
                children.Add(CreateUnknownTriggers(scene));
                children.Add(CreateVisualEffectTriggers(scene));
                foreach (var child in children)
                {
                    if (child != null)
                    {
                        child.parent = triggersRoot;
                        // Turn off root object for each trigger type.
                        child.gameObject.SetActive(false);
                    }
                }
            }

            // Hack AF, could use some cleaning
            var allRootSegments = GameObject.FindObjectsOfType<TagTrackSegment>();
            var rootSegments = allRootSegments.Where(x => x.depth == 0).Reverse().ToArray();
            var testTransforms = TestTransformHeirarchy(rootSegments);

            // Finally, save the scene file
            EditorSceneManager.SaveScene(unityScene, scenePath, false);
        }


        /// <summary>
        /// Used to create a dummy object
        /// </summary>
        /// <param name="displayName">The name of the object created</param>
        /// <returns></returns>
        private static GameObject CreateNoMeshObject()
        {
            // No models for this object. Make empty object.
            var noMeshObject = new GameObject();

            // Tag object with metadata
            noMeshObject.AddComponent<TagNoMesh>();

            // TEMP? Disable for visual clarity
            //noMeshObject.SetActive(false);

            return noMeshObject;
        }

        private static GameObject CreateInstanceFromDatabase(string assetPath)
        {
            // Load asset from database
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

            // Create instance of the prefab
            var instance = GameObject.Instantiate(asset);
            return instance;
        }


        #region CREATE TRIGGERS
        private static Transform CreateArcadeCheckpointTriggers(Scene scene)
        {
            var arcadeCheckpointTriggers = scene.timeExtensionTriggers;
            int count = 0;
            int total = arcadeCheckpointTriggers.Length;

            // Don't bother if no triggers
            var hasTriggers = total > 0;
            if (!hasTriggers)
                return null;

            var typeName = nameof(TimeExtensionTrigger);
            string title = $"Creating {typeName}s";
            string format = WidthFormat(arcadeCheckpointTriggers);

            var root = new GameObject($"{typeName} [{total}]").transform;

            foreach (var arcadeCheckpointTrigger in arcadeCheckpointTriggers)
            {
                count++;
                var name = $"[{count.ToString(format)}] {typeName}";
                ProgressBar.ShowIndexed(count, total, name, title);

                // Represent trigger as cube
                var gobj = CreatePrimitive(PrimitiveType.Cube, name, root).gameObject;
                // Add script to trigger, se values
                var script = gobj.AddComponent<GfzTimeExtensionTrigger>();
                script.ImportGfz(arcadeCheckpointTrigger);
            }

            return root;
        }

        private static Transform CreateCourseMetadataTriggers(Scene scene)
        {
            var courseMetadataTriggers = scene.miscellaneousTriggers;
            int count = 0;
            int total = courseMetadataTriggers.Length;

            // Don't bother if no triggers
            var hasTriggers = total > 0;
            if (!hasTriggers)
                return null;

            // Create general data for progress bar, naming
            var typeName = nameof(MiscellaneousTrigger);
            string title = $"Creating {typeName}s";
            string format = WidthFormat(courseMetadataTriggers);

            // Root object for all triggers
            var root = new GameObject($"{typeName} [{total}]").transform;

            // iterate over each trigger
            foreach (var courseMetadataTrigger in courseMetadataTriggers)
            {
                //
                count++;
                var name = $"[{count.ToString(format)}] {typeName}";
                ProgressBar.ShowIndexed(count, total, name, title);

                // Parse trigger type
                // There are 3 ways this data is used
                switch (courseMetadataTrigger.MetadataType)
                {
                    // Path data
                    case CourseMetadataType.Lightning_Lightning:
                        {
                            var pathObject = CreateMetadataPathObj(courseMetadataTrigger);
                            pathObject.name = $"[{count.ToString(format)}] Lightning Path";
                            pathObject.transform.parent = root;
                            pathObject.GizmosColor = Color.yellow;
                        }
                        break;
                    case CourseMetadataType.OuterSpace_Meteor:
                        {
                            var pathObject = CreateMetadataPathObj(courseMetadataTrigger);
                            pathObject.name = $"[{count.ToString(format)}] Meteor Path";
                            pathObject.transform.parent = root;
                            pathObject.GizmosColor = new Color32(255, 127, 0, 255); // orange
                        }
                        break;

                    // Obscure trigger data
                    case CourseMetadataType.BigBlueOrdeal:
                        {
                            var bboObject = CreateMetadataBboObj(courseMetadataTrigger);
                            bboObject.name = $"[{count.ToString(format)}] Big Blue Ordeal Unk Trigger";
                            bboObject.parent = root;
                        }
                        break;

                    // Story capsule data
                    case CourseMetadataType.Story1_CapsuleAX:
                        {
                            var storyCapsuleObject = CreateMetadataCapsuleObj(courseMetadataTrigger);
                            storyCapsuleObject.name = $"[{count.ToString(format)}] AX Story 1 Capsule";
                            storyCapsuleObject.parent = root;
                        }
                        break;
                    case CourseMetadataType.Story5_Capsule:
                        {
                            var storyCapsuleObject = CreateMetadataCapsuleObj(courseMetadataTrigger);
                            storyCapsuleObject.name = $"[{count.ToString(format)}] Story 5 Capsule";
                            storyCapsuleObject.parent = root;
                        }
                        break;

                    default:
                        int index = (int)courseMetadataTrigger.MetadataType;
                        throw new NotImplementedException($"{nameof(CourseMetadataType)} index {index} '{courseMetadataTrigger.MetadataType}'");
                }
            }

            return root;
        }
        private static GfzObjectPath CreateMetadataPathObj(MiscellaneousTrigger data)
        {
            var pathObject = new GameObject();
            var root = pathObject.transform;

            var from = new GameObject("from").transform;
            from.parent = root;
            from.position = data.PositionFrom;
            from.rotation = data.Rotation;

            var to = new GameObject("to").transform;
            to.parent = root;
            to.position = data.PositionTo;
            to.rotation = data.Rotation;

            var objectPath = pathObject.AddComponent<GfzObjectPath>();
            objectPath.From = from;
            objectPath.To = to;

            return objectPath;
        }

        private static Transform CreateMetadataBboObj(MiscellaneousTrigger data)
        {
            // Object named by caller
            var gobj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var bboTrigger = gobj.AddComponent<GfzUnknownMiscellaneousTrigger>();
            bboTrigger.ImportGfz(data);

            return gobj.transform;
        }

        private static Transform CreateMetadataCapsuleObj(MiscellaneousTrigger data)
        {
            var gobj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            var capsuleTrigger = gobj.gameObject.AddComponent<GfzStoryCapsule>();
            capsuleTrigger.ImportGfz(data);

            return gobj.transform;
        }

        private static Transform CreateStoryObjectTriggers(Scene scene)
        {
            // TODO: add animation paths.

            var storyObjectTriggers = scene.storyObjectTriggers;
            int count = 0;
            int total = storyObjectTriggers.Length;

            // Don't bother if no triggers
            var hasTriggers = total > 0;
            if (!hasTriggers)
                return null;

            var typeName = nameof(StoryObjectTrigger);
            string title = $"Creating {typeName}s";
            string format = WidthFormat(storyObjectTriggers);

            var root = new GameObject($"{typeName} [{total}]").transform;

            foreach (var storyObjectTrigger in storyObjectTriggers)
            {
                count++;
                // TODO: add data as component, not name?
                var name = $"[{count.ToString(format)}] {typeName}";
                ProgressBar.ShowIndexed(count, total, name, title);

                // Create primite to represent trigger
                var gobj = CreatePrimitive(PrimitiveType.Cube, name, root).gameObject;
                // Assign script and values
                var script = gobj.AddComponent<GfzStoryObjectTrigger>();
                script.ImportGfz(storyObjectTrigger);
            }

            return root;
        }

        private static Transform CreateUnknownSolsTriggers(Scene scene)
        {
            var unknownSolsTriggers = scene.unknownColliders;
            int count = 0;
            int total = unknownSolsTriggers.Length;

            // Don't bother if no triggers
            var hasTriggers = total > 0;
            if (!hasTriggers)
                return null;

            var typeName = nameof(UnknownCollider);
            string title = $"Creating {typeName}s";
            string format = WidthFormat(unknownSolsTriggers);

            var root = new GameObject($"{typeName} [{total}]").transform;

            foreach (var unknownSolsTrigger in unknownSolsTriggers)
            {
                count++;
                // TODO: add data as component, not name?
                var name = $"[{count.ToString(format)}] {typeName}";
                ProgressBar.ShowIndexed(count, total, name, title);

                //
                var gobj = CreatePrimitive(PrimitiveType.Cube, name, root).gameObject;
                //
                var script = gobj.AddComponent<GfzUnknownCollider>();
                script.ImportGfz(unknownSolsTrigger);
            }

            return root;
        }

        private static Transform CreateUnknownTriggers(Scene scene)
        {
            var unknownTriggers = scene.cullOverrideTriggers;
            int count = 0;
            int total = unknownTriggers.Length;

            // Don't bother if no triggers
            var hasTriggers = total > 0;
            if (!hasTriggers)
                return null;

            var typeName = nameof(CullOverrideTrigger);
            string title = $"Creating {typeName}s";
            string format = WidthFormat(unknownTriggers);

            var root = new GameObject($"{typeName} [{total}]").transform;

            foreach (var unknownTrigger in unknownTriggers)
            {
                count++;
                var name = $"[{count.ToString(format)}/{total}] {typeName}";
                ProgressBar.ShowIndexed(count, total, name, title);

                //
                var gobj = CreatePrimitive(PrimitiveType.Cube, name, root).gameObject;
                //
                var script = gobj.AddComponent<GfzCullOverrideTrigger>();
                script.ImportGfz(unknownTrigger);
            }

            return root;
        }

        private static Transform CreateVisualEffectTriggers(Scene scene)
        {
            var vfxTriggers = scene.visualEffectTriggers;
            int count = 0;
            int total = vfxTriggers.Length;

            // Don't bother if no triggers
            var hasTriggers = total > 0;
            if (!hasTriggers)
                return null;

            var typeName = nameof(VisualEffectTrigger);
            string title = $"Creating {typeName}s";
            string format = WidthFormat(vfxTriggers);

            var root = new GameObject($"{typeName} [{total}]").transform;

            foreach (var vfxTrigger in vfxTriggers)
            {
                count++;
                var name = $"[{count.ToString(format)}] {typeName}";
                ProgressBar.ShowIndexed(count, total, name, title);

                var gobj = CreatePrimitive(PrimitiveType.Cube, name, root).gameObject;
                var script = gobj.AddComponent<GfzVisualEffectTrigger>();
                script.ImportGfz(vfxTrigger);
            }

            return root;
        }

        #endregion


        #region Track Transform Hierarchies

        private static int elementIndex = 0;
        public static Transform CreateTrackTransformHierarchy(Scene scene)
        {
            elementIndex = 0;

            // Get mesh for debugging
            var mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");

            // Create object to house data
            var root = new GameObject();
            root.name = "Control Points Hierarchy";

            //
            elementIndex = 0;

            // Loop over every top transform
            int count = 0;
            int total = scene.RootTrackSegments.Length;
            foreach (var trackTransform in scene.RootTrackSegments)
            {
                // Recursively create transforms
                count++;
                var name = $"[{count}/{total}] Control Point | {count}";
                CreateControlPointRecursive(scene, trackTransform, root, mesh, name, 0);
            }

            return root.transform;
        }

        public static void CreateTrackTransformSet(Scene scene)
        {
            // Get mesh for debugging
            var mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");

            // Create object to house data
            var controlPointsRoot = new GameObject();
            controlPointsRoot.name = "Control Points Set";

            // Loop over every top transform
            //int count = 0;
            int index = 0;
            int total = scene.RootTrackSegments.Length;
            foreach (var trackTransform in scene.RootTrackSegments)
            {
                //
                var name = $"[{++index}/{total}] Control Point";
                var controlPointSet = new GameObject();
                controlPointSet.name = name;
                controlPointSet.transform.parent = controlPointsRoot.transform;
                CreateControlPointSequential(scene, trackTransform, controlPointSet, mesh, name, 0, 0);

                // Recursively create transforms
                //count++;
                //var name = $"[{count}/{total}] Control Point | {count}";
                //CreateControlPointRecursive(trackTransform, controlPointsParent, mesh, name, 0);
            }
        }

        public static TagTrackSegment CreateControlPointRecursive(Scene scene, TrackSegment trackSegment, GameObject parent, Mesh mesh, string name, int depth)
        {
            //
            var controlPoint = new GameObject();
            var tag = controlPoint.AddComponent<TagTrackSegment>();
            //
            controlPoint.name = $"{elementIndex++} {name}";
            controlPoint.transform.parent = parent.transform;
            // Set transform
            controlPoint.transform.localPosition = trackSegment.FallbackPosition;
            controlPoint.transform.localRotation = Quaternion.Euler(trackSegment.FallbackRotation);
            controlPoint.transform.localScale = trackSegment.FallbackScale;

            //
            tag.SetCurves(trackSegment);
            tag.segmentType = trackSegment.SegmentType;
            tag.embeddedPropertyType = trackSegment.EmbeddedPropertyType;
            tag.perimeterFlags = trackSegment.PerimeterFlags;
            tag.pipeCylinderFlags = trackSegment.PipeCylinderFlags;
            tag.fallbackScale = trackSegment.FallbackScale;
            tag.fallbackRotation = trackSegment.FallbackRotation;
            tag.fallbackPosition = trackSegment.FallbackPosition;
            tag.root_unk_0x38 = trackSegment.Root_unk_0x38;
            tag.root_unk_0x3A = trackSegment.Root_unk_0x3A;
            tag.railHeightRight = trackSegment.RailHeightRight;
            tag.railHeightLeft = trackSegment.RailHeightLeft;
            tag.branchIndex = trackSegment.BranchIndex; // 0, 1, 2, 3

            tag.depth = trackSegment.Depth;

            tag.hasCorner = trackSegment.TrackCornerPtr.IsNotNull;
            if (tag.hasCorner)
            {
                var q = trackSegment.TrackCorner.Transform.Rotation.value;
                tag.cornerPosition = trackSegment.TrackCorner.Transform.Position;
                tag.cornerRotation = new Quaternion(q.x, q.y, q.z, q.w).eulerAngles;
                tag.cornerScale = trackSegment.TrackCorner.Transform.Scale;
                tag.width = trackSegment.TrackCorner.Width;
                tag.perimeterFlags = trackSegment.TrackCorner.PerimeterOptions;
            }
            //
            var children = trackSegment.Children;
            int count = 1;
            int total = children.Length;
            foreach (var child in children)
            {
                var nameNext = $"{name}.{count++}";
                var tagChild = CreateControlPointRecursive(scene, child, controlPoint, mesh, nameNext, depth + 1);
                tag.children.Add(tagChild);
            }

            return tag;
        }

        public static void CreateControlPointSequential(Scene scene, TrackSegment trackTransform, GameObject parent, Mesh mesh, string name, int depth, int index)
        {
            //Create new control point
            var controlPoint = new GameObject();
            //
            controlPoint.name = $"{name} {depth}.{index}";
            controlPoint.transform.parent = parent.transform;
            // Set transform
            controlPoint.transform.position = trackTransform.FallbackPosition;
            controlPoint.transform.rotation = Quaternion.Euler(trackTransform.FallbackRotation);
            controlPoint.transform.localScale = trackTransform.FallbackScale;

            //
            var children = trackTransform.Children;
            index = 0;
            foreach (var child in children)
            {
                CreateControlPointSequential(scene, child, parent, mesh, name, depth + 1, ++index);
            }
        }


        #endregion

        // COLLIDER OBJECTS
        public static Transform IncludeStaticMeshColliders(Scene scene, string stageFolder)
        {
            var root = new GameObject();
            root.name = $"Static Mesh Colliders";

            // TODO: it would be wiser to tag the prefabs with some tag type so that
            // we need only pull in objects of that type. The string loading method
            // is bound to break at some point.
            for (int i = 0; i < scene.staticColliderMeshManager.SurfaceCount; i++)
            {
                var property = (StaticColliderMeshProperty)i;
                var meshName = $"st{scene.CourseIndex:00}_{i:00}_{property}";
                var assetPath = $"{stageFolder}/pf_{meshName}.prefab";
                var asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                if (asset == null)
                {
                    Debug.Log($"Could not find asset named '{assetPath}'.");
                    continue;
                }

                var instance = GameObject.Instantiate(asset, root.transform);
                instance.name = meshName;

                var script = instance.AddComponent<GfzStaticColliderMesh>();
                script.Property = property;
                script.ColliderMesh = instance.GetComponent<MeshFilter>();
            }

            return root.transform;
        }

        public static Transform TestTransformHeirarchy(TagTrackSegment[] rootSegments)
        {
            var root = new GameObject();
            root.name = $"Track Curves (Test)";

            //
            var increment = 1f / 1000f * rootSegments.Length;
            int count = 0;
            foreach (var trackSegment in rootSegments)
            {
                var subgroup = new GameObject();
                subgroup.name = $"Subgroup {++count}";
                subgroup.transform.parent = root.transform;

                for (float t = 0f; t < 1f; t += increment)
                {
                    var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.name = $"time {t:0.000}";
                    cube.transform.parent = subgroup.transform;

                    var mtx = GetMatrixRecursive(trackSegment, t);
                    cube.transform.localPosition = mtx.Position();
                    cube.transform.localRotation = mtx.Rotation();
                    var scale = mtx.Scale();
                    // Override scale because some places are wtf
                    cube.transform.localScale = new Vector3(scale.x, 1f, 1f);
                }
            }

            return root.transform;
        }

        public static Matrix4x4 GetMatrixRecursive(TagTrackSegment trackSegment, float time)
        {
            var selfAnimationMtx = GetAnimationMatrix(trackSegment, time);
            //var staticMtx = GetStaticMatrix(trackSegment);
            //var mtx = animationMtx * staticMtx;

            if (trackSegment.children != null && trackSegment.children.Count > 0)
            {
                // Iterate over children until a valid node is found.
                // Caveat: can't do branching paths like this since it only uses first found
                for (int i = 0; i < trackSegment.children.Count; i++)
                {
                    var child = trackSegment.children[i];

                    bool isInvalidEmbed =
                        child.segmentType == TrackSegmentType.IsEmbed && (
                        child.embeddedPropertyType == TrackEmbeddedPropertyType.IsDamage ||
                        child.embeddedPropertyType == TrackEmbeddedPropertyType.IsDirt ||
                        child.embeddedPropertyType == TrackEmbeddedPropertyType.IsSlip ||
                        child.embeddedPropertyType == TrackEmbeddedPropertyType.IsRecover);
                    if (isInvalidEmbed)
                        continue;

                    bool isModulated =
                        child.segmentType == TrackSegmentType.IsEmbed &&
                        child.embeddedPropertyType == TrackEmbeddedPropertyType.IsModulated;
                    bool isOpenPipeOrCapsule =
                        child.segmentType == TrackSegmentType.IsEmbed &&
                        child.embeddedPropertyType == TrackEmbeddedPropertyType.IsOpenPipeOrCylinder;

                    Matrix4x4 childAnimationMatrix;

                    //if (isModulated)
                    //{
                    //    // Strip out position data
                    //    childAnimationMatrix = GetAnimationMatrix(child, time);
                    //    var amr = childAnimationMatrix.Rotation();
                    //    var ams = childAnimationMatrix.Scale();
                    //    childAnimationMatrix = Matrix4x4.TRS(float3.zero, amr, ams);
                    //}
                    //else if (isOpenPipeOrCapsule)
                    //{
                    //    // Strip out scale data
                    //    childAnimationMatrix = GetAnimationMatrix(child, time);
                    //    var amp = childAnimationMatrix.Position();
                    //    var amr = childAnimationMatrix.Rotation();
                    //    childAnimationMatrix = Matrix4x4.TRS(amp, amr, new float3(1, 1, 1));
                    //}
                    //else
                    //{
                        childAnimationMatrix = GetMatrixRecursive(child, time);
                    //}

                    return selfAnimationMtx * childAnimationMatrix;
                }
            }

            return selfAnimationMtx;
        }

        //public static Matrix4x4 GetStaticMatrix(TagTrackSegment trackSegment)
        //{
        //    var staticMtx = Matrix4x4.TRS(
        //        trackSegment.localPosition,
        //        Quaternion.Euler(trackSegment.localRotation),
        //        trackSegment.localScale);

        //    return staticMtx;
        //}

        public static Matrix4x4 GetAnimationMatrix(TagTrackSegment trackSegment, float timeNormalized)
        {
            // Get max times
            var curves = trackSegment.curves;
            float3 tp = new float3(
                GetCurveMaxTime(curves.Position.x),
                GetCurveMaxTime(curves.Position.y),
                GetCurveMaxTime(curves.Position.z));
            float3 tr = new float3(
                GetCurveMaxTime(curves.Rotation.x),
                GetCurveMaxTime(curves.Rotation.y),
                GetCurveMaxTime(curves.Rotation.z));
            float3 ts = new float3(
                GetCurveMaxTime(curves.Scale.x),
                GetCurveMaxTime(curves.Scale.y),
                GetCurveMaxTime(curves.Scale.z));
            var t = timeNormalized;

            float3 position = new float3(
                curves.Position.x.EvaluateDefault(t * tp.x, trackSegment.fallbackPosition.x),
                curves.Position.y.EvaluateDefault(t * tp.y, trackSegment.fallbackPosition.y),
                -curves.Position.z.EvaluateDefault(t * tp.z, trackSegment.fallbackPosition.z));
            float3 rotation = new float3(
                -curves.Rotation.x.EvaluateDefault(t * tr.x, trackSegment.fallbackRotation.x),
                -curves.Rotation.y.EvaluateDefault(t * tr.y, trackSegment.fallbackRotation.y),
                curves.Rotation.z.EvaluateDefault(t * tr.z, trackSegment.fallbackRotation.z));
            float3 scale = new float3(
                curves.Scale.x.EvaluateDefault(t * ts.x, trackSegment.fallbackScale.x),
                curves.Scale.y.EvaluateDefault(t * ts.y, trackSegment.fallbackScale.y),
                curves.Scale.z.EvaluateDefault(t * ts.z, trackSegment.fallbackScale.z));

            var rx = Quaternion.Euler(rotation.x, 0, 0);
            var ry = Quaternion.Euler(0, rotation.y, 0);
            var rz = Quaternion.Euler(0, 0, rotation.z);
            //var qr = rx * ry * rz;
            var qr = rz * ry * rx;
            //var qr = rz * rx * ry;

            //var animationMtx = Matrix4x4.TRS(position, Quaternion.Euler(rotation), scale);
            var animationMtx = Matrix4x4.TRS(position, qr, scale);
            return animationMtx;
        }

        public static float GetCurveMaxTime(UnityEngine.AnimationCurve curve)
        {
            if (curve.length == 0)
                return 0f;

            return curve.keys[curve.length - 1].time;
        }

        public static Transform CreatePrimitive(PrimitiveType primitiveType, string name = null, Transform parent = null)
        {
            var gobj = GameObject.CreatePrimitive(primitiveType);
            gobj.transform.parent = parent;

            if (!string.IsNullOrEmpty(name))
                gobj.name = name;

            var collider = gobj.GetComponent<Collider>();
            GameObject.DestroyImmediate(collider);

            return gobj.transform;
        }

        public static Transform CreateGridXZVisual(Scene scene)
        {
            // Get all bounds
            var boundsTrack = scene.CheckpointGridXZ;
            var boundsColliders = scene.staticColliderMeshManager.MeshGridXZ;
            //
            float yHeight = scene.trackMinHeight;

            // Create objects
            var boundsRoot = new GameObject("Bounds").transform;
            var boundsTrackNodes = CreateGridBoundsXZ(boundsTrack, yHeight, "Track Nodes");
            var boundsStaticColliders = CreateGridBoundsXZ(boundsColliders, yHeight, $"Static Colliders");

            // Set object parents
            boundsTrackNodes.transform.SetParent(boundsRoot);
            boundsStaticColliders.transform.SetParent(boundsRoot);

            // return parent object
            return boundsRoot;
        }

        public static Transform CreateGridBoundsXZ(GridXZ grid, float yHeight, string name)
        {
            var displayName = $"{name} ({grid.NumSubdivisionsX}x{grid.NumSubdivisionsZ})";

            var gridObject = CreatePrimitive(PrimitiveType.Cube, displayName);
            (var center, var scale) = grid.GetCenterAndScale();
            gridObject.position = new float3(center.x, yHeight, center.y);
            gridObject.localScale = new float3(scale.x, 1f, scale.y);

            //
            int largest = math.max(grid.NumSubdivisionsX, grid.NumSubdivisionsZ);
            int format = largest.ToString().Length;

            //
            for (int z = 0; z < grid.NumSubdivisionsX; z++)
            {
                var centerZ = grid.Top + (grid.SubdivisionLength * (z + 0.5f));

                for (int x = 0; x < grid.NumSubdivisionsZ; x++)
                {
                    var centerX = grid.Left + (grid.SubdivisionWidth * (x + 0.5f));

                    //
                    var textX = x.ToString().PadLeft(format);
                    var textZ = z.ToString().PadLeft(format);
                    var cellName = $"{name} [{textX},{textZ}]";
                    var cell = CreatePrimitive(PrimitiveType.Cube, cellName);
                    cell.position = new float3(centerX, yHeight, centerZ);
                    cell.localScale = new float3(grid.SubdivisionWidth, 1f, grid.SubdivisionLength);
                    cell.parent = gridObject;
                }
            }

            return gridObject;
        }


        public static Transform CreateStaticMeshColliderManagerSphereBounds(Scene scene)
        {
            var root = new GameObject($"{nameof(GameCube.GFZ.BoundingSphere)}s");
            var boundingSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            boundingSphere.transform.localPosition = scene.staticColliderMeshManager.BoundingSphere.origin;
            boundingSphere.transform.localScale = scene.staticColliderMeshManager.BoundingSphere.radius * 2f * Vector3.one;
            boundingSphere.SetActive(false);
            boundingSphere.name = $"{nameof(StaticColliderMeshManager)}.{nameof(GameCube.GFZ.BoundingSphere)}";
            boundingSphere.transform.parent = root.transform;
            return root.transform;
        }

        public static Transform CreateTrackIndexChains(Scene scene)
        {
            var root = new GameObject();
            root.name = $"Track Index Chains";

            int chainIndex = 0;
            var trackNodes = scene.trackNodes;
            foreach (var indexList in scene.trackCheckpointGrid.IndexLists)
            {
                var chain = new GameObject().transform;
                chain.name = $"Chain {chainIndex++}";
                chain.parent = root.transform;

                for (int i = 0; i < indexList.Length; i++)
                {
                    int index = indexList.Indexes[i];
                    var node = trackNodes[index];

                    for (int j = 0; j < node.Checkpoints.Length; j++)
                    {
                        var position = node.Checkpoints[j].PlaneStart.origin;
                        var instance = CreatePrimitive(PrimitiveType.Sphere, $"{index}.{j}", chain);
                        instance.transform.position = position;
                        instance.transform.localScale = Vector3.one * 5f;
                    }
                }
            }
            return root.transform;
        }

        public static string GetAssetPath(string prefabName, params string[] searchFolders)
        {
            // Store all possible matches for the object name in questions using the folder constraints
            var assetGuids = AssetDatabase.FindAssets(prefabName, searchFolders);
            // This variable will store the path to the object we want.
            // It may end up staying empty in certain cases.
            var assetPath = string.Empty;

            // Begin a triage of the asset database
            if (assetGuids.Length == 1)
            {
                // We only found 1 model. Great!
                assetPath = AssetDatabase.GUIDToAssetPath(assetGuids[0]);
            }
            else // 0 or >1 matches
            {
                // If we have no 0 matches, the foreach will not run.
                // If we have more than one, the foreach will run BUT may not get a result.
                foreach (var assetGuid in assetGuids)
                {
                    // Get data about a specific object
                    var tempAssetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
                    var tempAssetName = System.IO.Path.GetFileNameWithoutExtension(tempAssetPath);
                    // See if it has the exact name
                    if (tempAssetName.Equals(prefabName))
                    {
                        // If it does, load that one, break iteration
                        assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
                        break;
                    }
                }

                // If the "break" is never hit, then there are 2 possibilities:
                // (1) Name is simple (ei: TEST) and flags a bunch of models, but no models exists for it.
                // (2) Model has empty string name, flagged a bunch of stuff, but has no exact match.
            }

            // Return our match. MAY BE EMPTY STRING.
            return assetPath;
        }

        public static string WidthFormat(Array array)
        {
            int displayDigitsLength = array.Length.ToString().Length;
            var digitsFormat = new string('0', displayDigitsLength);
            return digitsFormat;
        }


        public static Transform[] CreateAllSceneObjects(Scene scene, params string[] searchFolders)
        {
            var sceneObjectRoot = new GameObject($"{nameof(SceneObject)}s");
            var sceneObjectDict = new Dictionary<SceneObject, GfzSceneObject>();
            int index = 0;
            foreach (var sceneObject in scene.sceneObjects)
            {
                var gobj = new GameObject($"[{++index}] {sceneObject.Name}");
                gobj.transform.SetParent(sceneObjectRoot.transform);
                var gfzSceneObject = gobj.AddComponent<GfzSceneObject>();
                gfzSceneObject.ImportGfz(sceneObject);
                sceneObjectDict.Add(sceneObject, gfzSceneObject);
            }

            var rootStatics = CreateStaticSceneObjects(scene, sceneObjectDict, searchFolders);
            var rootDynamics = CreateDynamicSceneObjects(scene, sceneObjectDict, searchFolders);

            //
            return new Transform[] { sceneObjectRoot.transform, rootStatics, rootDynamics };
        }

        public static Transform CreateDynamicSceneObjects(Scene scene, Dictionary<SceneObject, GfzSceneObject> sceneObjectDict, params string[] searchFolders)
        {
            // Get some metadata from the number of scene objects
            var dynamicSceneObjects = scene.dynamicSceneObjects;

            // Create a string format from the highest index number used
            var digitsFormat = WidthFormat(dynamicSceneObjects);
            int count = 0;
            int total = dynamicSceneObjects.Length;
            // Create root for all scene objects
            var dynamicsRoot = new GameObject($"{nameof(SceneObjectDynamic)}s").transform;

            // Find all scene objects, add them to scene
            foreach (var dynamicSceneObject in dynamicSceneObjects)
            {
                // Generate the names of the objects we want
                var objectName = dynamicSceneObject.Name;
                var prefabName = $"pf_{objectName}";
                var displayName = $"[{count.ToString(digitsFormat)}] {objectName}";

                // Progress bar update
                var title = $"Generating Scene ({scene.FileName})";
                var info = $"[{count.ToString(digitsFormat)}/{total}] {objectName}";
                var progress = (float)count / total;
                var cancel = EditorUtility.DisplayCancelableProgressBar(title, info, progress);
                count++;

                // Find the asset path from database
                var assetPath = GetAssetPath(prefabName, searchFolders);
                // Create asset
                GameObject assetInstance = string.IsNullOrEmpty(assetPath)
                    ? CreateNoMeshObject()
                    : CreateInstanceFromDatabase(assetPath);
                // Assign name and set parent
                assetInstance.name = displayName;
                assetInstance.transform.parent = dynamicsRoot;

                //// Tack data of object onto Unity GameObject for inspection
                var script = assetInstance.AddComponent<GfzSceneObjectDynamic>();
                script.ImportGfz(dynamicSceneObject);

                // Bind reference
                var gfzSceneObject = sceneObjectDict[dynamicSceneObject.SceneObject];
                script.SetSceneObject(gfzSceneObject);
            }

            return dynamicsRoot;
        }

        public static Transform CreateStaticSceneObjects(Scene scene, Dictionary<SceneObject, GfzSceneObject> sceneObjectDict, params string[] searchFolders)
        {
            // Get some metadata from the number of scene objects
            var staticSceneObjects = scene.staticSceneObjects;

            // Create a string format from the highest index number used
            var digitsFormat = WidthFormat(staticSceneObjects);
            int count = 0;
            int total = staticSceneObjects.Length;

            // Create root for all scene objects
            var root = new GameObject($"{nameof(SceneObjectStatic)}s").transform;

            // Find all origin objects, add them to scene
            foreach (var staticSceneObject in staticSceneObjects)
            {
                // Generate the names of the objects we want
                var objectName = staticSceneObject.Name;
                var prefabName = $"pf_{objectName}";
                var displayName = $"[{count.ToString(digitsFormat)}] {objectName}";

                // Progress bar update
                var title = $"Generating Scene ({scene.FileName})";
                var info = $"[{count.ToString(digitsFormat)}/{total}] {objectName}";
                var progress = (float)count / total;
                EditorUtility.DisplayProgressBar(title, info, progress);
                count++;

                // Find the asset path from database
                var assetPath = GetAssetPath(prefabName, searchFolders);
                // Create asset
                GameObject assetInstance = string.IsNullOrEmpty(assetPath)
                    ? CreateNoMeshObject()
                    : CreateInstanceFromDatabase(assetPath);
                // Assign name and set parent
                assetInstance.name = displayName;
                assetInstance.transform.parent = root;

                var script = assetInstance.AddComponent<GfzSceneObjectStatic>();
                // Bind reference
                var gfzSceneObject = sceneObjectDict[staticSceneObject.SceneObject];
                script.SetSceneObject(gfzSceneObject);
            }

            return root;
        }

        public static Transform CreateGlobalParams(Scene scene)
        {
            var sceneParamsObj = new GameObject("Scene Parameters");
            var sceneParams = sceneParamsObj.AddComponent<GfzSceneParameters>();
            sceneParams.venue = CourseUtility.GetVenue(scene.CourseIndex);
            // TODO: embed course name in file, use that if it exists.
            sceneParams.courseName = CourseUtility.GetCourseName(scene.CourseIndex);
            sceneParams.courseIndex = scene.CourseIndex;
            sceneParams.author = "Amusement Vision";
            // Other data
            sceneParams.staticColliderMeshesActive = scene.StaticColliderMeshManagerActive;
            sceneParams.circuitType = scene.CircuitType;

            // Copy fog parameters over
            var fog = scene.fog;
            sceneParams.exportCustomFog = true; // whatever we import, use that
            sceneParams.fogInterpolation = fog.Interpolation;
            sceneParams.fogNear = fog.FogRange.near;
            sceneParams.fogFar = fog.FogRange.far;
            var color = fog.ColorRGB;
            sceneParams.color = new Color(color.x, color.y, color.z);

            if (scene.fogCurves != null)
            {
                var fogCurves = scene.fogCurves;
                // Convert from GFZ anim curves to Unity anim curves
                sceneParams.fogCurveNear = fogCurves.FogCurveNear.ToUnity();
                sceneParams.fogCurveFar = fogCurves.FogCurveFar.ToUnity();
                sceneParams.fogCurveR = fogCurves.FogCurveR.ToUnity();
                sceneParams.fogCurveG = fogCurves.FogCurveG.ToUnity();
                sceneParams.fogCurveB = fogCurves.FogCurveB.ToUnity();
            }

            return sceneParamsObj.transform;
        }
    }


}