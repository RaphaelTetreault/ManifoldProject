using GameCube.GFZ;
using GameCube.GFZ.CourseCollision;
using Manifold.IO;
using Manifold.IO.GFZ;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Unity.Mathematics;

namespace Manifold.IO.GFZ.CourseCollision
{
    public static class SceneImportUtility
    {
        // todo: make title consistent across progress bars.


        public const string ExecuteText = "Import COLI as Unity Scene";

        [MenuItem(Const.Menu.Manifold + "Scene Generation/Import All Stages")]
        public static void ImportAll()
        {
            var settings = GfzProjectWindow.GetSettings();
            var inputPath = settings.StageDir;
            var outputPath = settings.UnityImportDir + "stage/";
            foreach (var scene in ColiCourseIO.LoadAllStages(inputPath, "Generating Unity Scene From GFZ Scene"))
            {
                Import(scene, outputPath);
            }
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        [MenuItem(Const.Menu.Manifold + "Scene Generation/Import Stage (Single)")]
        public static void ImportSingle()
        {
            var settings = GfzProjectWindow.GetSettings();
            var inputPath = settings.StageDir;
            var outputPath = settings.UnityImportDir + "stage/";
            var sceneIndex = settings.SceneOfInterestID;
            var filePath = $"{inputPath}COLI_COURSE{sceneIndex:00}";
            var scene = ColiCourseIO.LoadScene(filePath);
            Import(scene, outputPath);
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        [MenuItem(Const.Menu.Manifold + "Scene Generation/Import Stage (Single, Select File)")]
        public static void ImportSingleSelect()
        {
            var settings = GfzProjectWindow.GetSettings();
            var filePath = EditorUtility.OpenFilePanel("Select Stage File", settings.StageDir, "");
            if (string.IsNullOrEmpty(filePath))
                return;
            var outputPath = settings.UnityImportDir + "stage/";
            var scene = ColiCourseIO.LoadScene(filePath);
            Import(scene, outputPath);
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        public static void Import(ColiScene scene, string outputPath)
        {
            // Create new, empty scene
            var scenePath = $"Assets/{outputPath}{scene.FileName}.unity";
            var unityScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            EditorSceneManager.SaveScene(unityScene, scenePath);
            // Keep reference of new scene
            unityScene = EditorSceneManager.OpenScene(scenePath);

            // Course-related values, used to find models
            // Triple digit IDs do overflow the "00" format, that's okay.
            var stageID = scene.ID;
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

            // Everything with a coordinate goes in here
            var mirrorRoot = new GameObject("Scene Root (Mirror Z)").transform;

            // Create scene objects, static objects, and dynamic objects
            var rootTransforms = CreateAllSceneObjects(scene, searchFolders);
            foreach (var transform in rootTransforms)
                transform.SetParent(mirrorRoot);

            TestTransformHeirarchy(scene).SetParent(mirrorRoot);
            CreateTrackTransformHierarchy(scene).SetParent(mirrorRoot);
            CreateTrackIndexChains(scene).SetParent(mirrorRoot);
            IncludeStaticMeshColliders(scene, stageFolder).SetParent(mirrorRoot);
            CreateBoundsVisual(scene).SetParent(mirrorRoot);

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
                triggersRoot.SetParent(mirrorRoot);
            }

            // Mirror all objects
            mirrorRoot.transform.localScale = new Vector3(1f, 1f, -1f);
            // Now that everything is placed right, get rid of mirror root
            var mirrorRootChildren = mirrorRoot.GetChildren();
            foreach (var child in mirrorRootChildren)
            {
                child.gameObject.AddComponent<GfzMirroredObject>();
                child.SetParent(null);
            }
            GameObject.DestroyImmediate(mirrorRoot.gameObject);

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
            noMeshObject.AddComponent<NoMeshTag>();

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
        private static Transform CreateArcadeCheckpointTriggers(ColiScene scene)
        {
            var arcadeCheckpointTriggers = scene.arcadeCheckpointTriggers;
            int count = 0;
            int total = arcadeCheckpointTriggers.Length;

            // Don't bother if no triggers
            var hasTriggers = total > 0;
            if (!hasTriggers)
                return null;

            var typeName = nameof(ArcadeCheckpointTrigger);
            string title = $"Creating {typeName}s";
            string format = WidthFormat(arcadeCheckpointTriggers);

            var root = new GameObject($"{typeName} [{total}]").transform;

            foreach (var arcadeCheckpointTrigger in arcadeCheckpointTriggers)
            {
                count++;
                var name = $"[{count.ToString(format)}] {typeName}";
                ImportUtility.ProgressBar(count, total, name, title);

                // Represent trigger as cube
                var gobj = CreatePrimitive(PrimitiveType.Cube, name, root).gameObject;
                // Add script to trigger, se values
                var script = gobj.AddComponent<GfzArcadeCheckpoint>();
                script.ImportGfz(arcadeCheckpointTrigger);
            }

            return root;
        }

        private static Transform CreateCourseMetadataTriggers(ColiScene scene)
        {
            var courseMetadataTriggers = scene.courseMetadataTriggers;
            int count = 0;
            int total = courseMetadataTriggers.Length;

            // Don't bother if no triggers
            var hasTriggers = total > 0;
            if (!hasTriggers)
                return null;

            // Create general data for progress bar, naming
            var typeName = nameof(CourseMetadataTrigger);
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
                ImportUtility.ProgressBar(count, total, name, title);

                // Parse trigger type
                // There are 3 ways this data is used
                switch (courseMetadataTrigger.metadataType)
                {
                    // Path data
                    case CourseMetadataType.Lightning_Lightning:
                        {
                            var pathObject = CreateMetadataPathObj(courseMetadataTrigger);
                            pathObject.name = $"[{count.ToString(format)}] Lightning Path";
                            pathObject.transform.parent = root;
                            pathObject.gizmosColor = Color.yellow;
                        }
                        break;
                    case CourseMetadataType.OuterSpace_Meteor:
                        {
                            var pathObject = CreateMetadataPathObj(courseMetadataTrigger);
                            pathObject.name = $"[{count.ToString(format)}] Meteor Path";
                            pathObject.transform.parent = root;
                            pathObject.gizmosColor = new Color32(255, 127, 0, 255); // orange
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
                        int index = (int)courseMetadataTrigger.metadataType;
                        throw new NotImplementedException($"{nameof(CourseMetadataType)} index {index} '{courseMetadataTrigger.metadataType}'");
                }
            }

            return root;
        }
        private static GfzObjectPath CreateMetadataPathObj(CourseMetadataTrigger data)
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
            objectPath.from = from;
            objectPath.to = to;

            return objectPath;
        }

        private static Transform CreateMetadataBboObj(CourseMetadataTrigger data)
        {
            // Object named by caller
            var gobj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var bboTrigger = gobj.AddComponent<GfzUnknownCourseMetadataTrigger>();
            bboTrigger.ImportGfz(data);

            return gobj.transform;
        }

        private static Transform CreateMetadataCapsuleObj(CourseMetadataTrigger data)
        {
            var gobj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            var capsuleTrigger = gobj.gameObject.AddComponent<GfzStoryCapsule>();
            capsuleTrigger.ImportGfz(data);

            return gobj.transform;
        }

        private static Transform CreateStoryObjectTriggers(ColiScene scene)
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
                ImportUtility.ProgressBar(count, total, name, title);

                // Create primite to represent trigger
                var gobj = CreatePrimitive(PrimitiveType.Cube, name, root).gameObject;
                // Assign script and values
                var script = gobj.AddComponent<GfzStoryObjectTrigger>();
                script.ImportGfz(storyObjectTrigger);
            }

            return root;
        }

        private static Transform CreateUnknownSolsTriggers(ColiScene scene)
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
                ImportUtility.ProgressBar(count, total, name, title);

                //
                var gobj = CreatePrimitive(PrimitiveType.Cube, name, root).gameObject;
                //
                var script = gobj.AddComponent<GfzUnknownCollider>();
                script.ImportGfz(unknownSolsTrigger);
            }

            return root;
        }

        private static Transform CreateUnknownTriggers(ColiScene scene)
        {
            var unknownTriggers = scene.unknownTriggers;
            int count = 0;
            int total = unknownTriggers.Length;

            // Don't bother if no triggers
            var hasTriggers = total > 0;
            if (!hasTriggers)
                return null;

            var typeName = nameof(UnknownTrigger);
            string title = $"Creating {typeName}s";
            string format = WidthFormat(unknownTriggers);

            var root = new GameObject($"{typeName} [{total}]").transform;

            foreach (var unknownTrigger in unknownTriggers)
            {
                count++;
                var name = $"[{count.ToString(format)}/{total}] {typeName}";
                ImportUtility.ProgressBar(count, total, name, title);

                //
                var gobj = CreatePrimitive(PrimitiveType.Cube, name, root).gameObject;
                //
                var script = gobj.AddComponent<GfzUnknownTrigger>();
                script.ImportGfz(unknownTrigger);
            }

            return root;
        }

        private static Transform CreateVisualEffectTriggers(ColiScene scene)
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
                ImportUtility.ProgressBar(count, total, name, title);

                var gobj = CreatePrimitive(PrimitiveType.Cube, name, root).gameObject;
                var script = gobj.AddComponent<GfzVisualEffectTrigger>();
                script.ImportGfz(vfxTrigger);
            }

            return root;
        }

        #endregion


        #region Track Transform Hierarchies

        private static int elementIndex = 0;
        public static Transform CreateTrackTransformHierarchy(ColiScene scene)
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
            int total = scene.rootTrackSegments.Length;
            foreach (var trackTransform in scene.rootTrackSegments)
            {
                // Recursively create transforms
                count++;
                var name = $"[{count}/{total}] Control Point | {count}";
                CreateControlPointRecursive(scene, trackTransform, root, mesh, name, 0);
            }

            return root.transform;
        }

        public static void CreateTrackTransformSet(ColiScene scene)
        {
            // Get mesh for debugging
            var mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");

            // Create object to house data
            var controlPointsRoot = new GameObject();
            controlPointsRoot.name = "Control Points Set";

            // Loop over every top transform
            //int count = 0;
            int index = 0;
            int total = scene.rootTrackSegments.Length;
            foreach (var trackTransform in scene.rootTrackSegments)
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

        public static void CreateControlPointRecursive(ColiScene scene, TrackSegment trackTransform, GameObject parent, Mesh mesh, string name, int depth)
        {
            //
            var controlPoint = new GameObject();
            var tag = controlPoint.AddComponent<TagTrackTransform>();
            tag.Data = trackTransform;
            //
            controlPoint.name = $"{elementIndex++} {name}";
            controlPoint.transform.parent = parent.transform;
            // Set transform
            controlPoint.transform.localPosition = trackTransform.localPosition;
            controlPoint.transform.localRotation = Quaternion.Euler(trackTransform.localRotation);
            controlPoint.transform.localScale = trackTransform.localScale;

            //
            var display = controlPoint.AddComponent<DisplayTrackTransformSingle>();
            display.depth = depth;

            //
            var children = trackTransform.GetChildren(scene.allTrackSegments);
            int count = 1;
            int total = children.Length;
            foreach (var child in children)
            {
                var nameNext = $"{name}.{count++}";
                CreateControlPointRecursive(scene, child, controlPoint, mesh, nameNext, depth + 1);
            }
        }

        public static void CreateControlPointSequential(ColiScene scene, TrackSegment trackTransform, GameObject parent, Mesh mesh, string name, int depth, int index)
        {
            //Create new control point
            var controlPoint = new GameObject();
            //
            controlPoint.name = $"{name} {depth}.{index}";
            controlPoint.transform.parent = parent.transform;
            // Set transform
            controlPoint.transform.position = trackTransform.localPosition;
            controlPoint.transform.rotation = Quaternion.Euler(trackTransform.localRotation);
            controlPoint.transform.localScale = trackTransform.localScale;

            //
            var children = trackTransform.GetChildren(scene.allTrackSegments);
            index = 0;
            foreach (var next in children)
            {
                CreateControlPointSequential(scene, next, parent, mesh, name, depth + 1, ++index);
            }
        }


        #endregion

        // COLLIDER OBJECTS
        public static Transform IncludeStaticMeshColliders(ColiScene scene, string stageFolder)
        {
            var root = new GameObject();
            root.name = $"Static Mesh Colliders";

            // TODO: it would be wiser to tag the prefabs with some tag type so that
            // we need only pull in objects of that type. The string loading method
            // is bound to break at some point.
            for (int i = 0; i < scene.staticColliderMeshes.SurfaceCount; i++)
            {
                var property = (StaticColliderMeshProperty)i;
                var meshName = $"st{scene.ID:00}_{i:00}_{property}";
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

        public static Transform TestTransformHeirarchy(ColiScene scene)
        {
            var root = new GameObject();
            root.name = $"Track Curves (Test)";

            //
            var increment = 1f / 1000f * scene.rootTrackSegments.Length;
            int count = 0;
            foreach (var tt in scene.rootTrackSegments)
            {
                var subgroup = new GameObject();
                subgroup.name = $"Subgroup {++count}";
                subgroup.transform.parent = root.transform;

                var topology = tt.trackCurves;
                float3 timeScale = new float3(
                    GetCurveTime(topology.unityCurves[0]),
                    GetCurveTime(topology.unityCurves[1]),
                    GetCurveTime(topology.unityCurves[2]));
                float3 timeRotation = new float3(
                    GetCurveTime(topology.unityCurves[3]),
                    GetCurveTime(topology.unityCurves[4]),
                    GetCurveTime(topology.unityCurves[5]));
                float3 timePosition = new float3(
                    GetCurveTime(topology.unityCurves[6]),
                    GetCurveTime(topology.unityCurves[7]),
                    GetCurveTime(topology.unityCurves[8]));

                for (float t = 0f; t < 1f; t += increment)
                {
                    float3 scale = new float3(
                        topology.unityCurves[0].EvaluateDefault(t * timeScale.x, 1),
                        topology.unityCurves[1].EvaluateDefault(t * timeScale.y, 1),
                        topology.unityCurves[2].EvaluateDefault(t * timeScale.z, 1));
                    float3 rotation = new float3(
                        topology.unityCurves[3].EvaluateDefault(t * timeRotation.x, 0),
                        topology.unityCurves[4].EvaluateDefault(t * timeRotation.y, 0),
                        topology.unityCurves[5].EvaluateDefault(t * timeRotation.z, 0));
                    float3 position = new float3(
                        topology.unityCurves[6].EvaluateDefault(t * timePosition.x, 0),
                        topology.unityCurves[7].EvaluateDefault(t * timePosition.y, 0),
                        topology.unityCurves[8].EvaluateDefault(t * timePosition.z, 0));

                    var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.name = $"time {t:0.000}";
                    cube.transform.parent = subgroup.transform;

                    //cube.transform.position = position + tt.localPosition;
                    //cube.transform.rotation = Quaternion.Euler(rotation);// + tt.localRotation);
                    //cube.transform.localScale = scale.Multiply(tt.localScale);
                    cube.transform.position = position + tt.localPosition;
                    cube.transform.rotation = Quaternion.Euler(rotation + tt.localRotation);// math.mul(quaternion.EulerXYZ(rotation), quaternion.EulerXYZ(tt.localRotation));
                    cube.transform.localScale = scale * tt.localScale;
                }
            }

            return root.transform;
        }

        public static float GetCurveTime(UnityEngine.AnimationCurve curve)
        {
            if (curve.length == 0)
                return 0f;

            return curve.keys[curve.length - 1].time;
        }

        public static Matrix4x4 GetTransformMatrix(TrackSegment trackTransform, float increment)
        {
            var transformMatrix = new Matrix4x4();
            transformMatrix.SetTRS(
                trackTransform.localPosition,
                Quaternion.Euler(trackTransform.localRotation),
                trackTransform.localScale);

            // get child matrix
            //if (trackTransform.topologyMetadata == TrackTopologyMetadata.IsTransformParent)
            //{
            //    var childMatrix = GetTransformMatrix(trackTransform.children[0], increment);
            //    return transformMatrix * childMatrix;
            //}

            return transformMatrix;
        }

        public static Matrix4x4 GetAnimMatrix(TrackSegment trackTransform, float time)
        {
            var topology = trackTransform.trackCurves;
            float3 timeScale = new float3(
                GetCurveTime(topology.unityCurves[0]),
                GetCurveTime(topology.unityCurves[1]),
                GetCurveTime(topology.unityCurves[2]));
            float3 timeRotation = new float3(
                GetCurveTime(topology.unityCurves[3]),
                GetCurveTime(topology.unityCurves[4]),
                GetCurveTime(topology.unityCurves[5]));
            float3 timePosition = new float3(
                GetCurveTime(topology.unityCurves[6]),
                GetCurveTime(topology.unityCurves[7]),
                GetCurveTime(topology.unityCurves[8]));

            float3 scale = new float3(
                topology.unityCurves[0].EvaluateDefault(time * timeScale.x, 1),
                topology.unityCurves[1].EvaluateDefault(time * timeScale.y, 1),
                topology.unityCurves[2].EvaluateDefault(time * timeScale.z, 1));
            float3 rotation = new float3(
                topology.unityCurves[3].EvaluateDefault(time * timeRotation.x, 0),
                topology.unityCurves[4].EvaluateDefault(time * timeRotation.y, 0),
                topology.unityCurves[5].EvaluateDefault(time * timeRotation.z, 0));
            float3 position = new float3(
                topology.unityCurves[6].EvaluateDefault(time * timePosition.x, 0),
                topology.unityCurves[7].EvaluateDefault(time * timePosition.y, 0),
                topology.unityCurves[8].EvaluateDefault(time * timePosition.z, 0));

            var animationMatrix = new Matrix4x4();
            animationMatrix.SetTRS(position, Quaternion.Euler(rotation), scale);

            // get child matrix
            //if (trackTransform.topologyMetadata == TrackTopologyMetadata.IsTransformParent)
            //{
            //    var childMatrix = GetAnimMatrix(trackTransform.children[0], time);
            //
            //    return animationMatrix * childMatrix;
            //}

            return animationMatrix;
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

        public static Transform CreateBoundsVisual(ColiScene scene)
        {
            // Get all bounds
            var boundsTrack = scene.trackCheckpointBoundsXZ;
            var boundsColliders = scene.staticColliderMeshes.meshBounds;
            //
            float yHeight = scene.trackMinHeight;

            // Create objects
            var boundsRoot = new GameObject("Bounds").transform;
            var boundsTrackNodes = CreateBoundsObject(boundsTrack, yHeight, "Track Nodes");
            var boundsStaticColliders = CreateBoundsObject(boundsColliders, yHeight, $"Static Colliders");

            // Set object parents
            boundsTrackNodes.transform.SetParent(boundsRoot);
            boundsStaticColliders.transform.SetParent(boundsRoot);

            // return parent object
            return boundsRoot;
        }

        public static Transform CreateBoundsObject(MatrixBoundsXZ bounds, float yHeight, string name)
        {
            var displayName = $"{name} ({bounds.numSubdivisionsX}x{bounds.numSubdivisionsZ})";
            var boundsObject = CreatePrimitive(PrimitiveType.Cube, displayName);
            (var center, var scale) = bounds.GetCenterAndScale();
            boundsObject.position = new float3(center.x, yHeight, center.y);
            boundsObject.localScale = new float3(scale.x, 1f, scale.y);

            return boundsObject;
        }


        public static Transform CreateTrackIndexChains(ColiScene scene)
        {
            var root = new GameObject();
            root.name = $"Track Index Chains";

            int chainIndex = 0;
            var trackNodes = scene.trackNodes;
            foreach (var indexList in scene.trackCheckpointMatrix.indexLists)
            {
                var chain = new GameObject().transform;
                chain.name = $"Chain {chainIndex++}";
                chain.parent = root.transform;

                for (int i = 0; i < indexList.Length; i++)
                {
                    int index = indexList.Indexes[i];
                    var node = trackNodes[index];

                    for (int j = 0; j < node.checkpoints.Length; j++)
                    {
                        var position = node.checkpoints[j].start.position;
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


        public static Transform[] CreateAllSceneObjects(ColiScene scene, params string[] searchFolders)
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

        public static Transform CreateDynamicSceneObjects(ColiScene scene, Dictionary<SceneObject, GfzSceneObject> sceneObjectDict, params string[] searchFolders)
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
                var gfzSceneObject = sceneObjectDict[dynamicSceneObject.sceneObject];
                script.SetSceneObject(gfzSceneObject);
            }

            return dynamicsRoot;
        }

        public static Transform CreateStaticSceneObjects(ColiScene scene, Dictionary<SceneObject, GfzSceneObject> sceneObjectDict, params string[] searchFolders)
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
                var gfzSceneObject = sceneObjectDict[staticSceneObject.sceneObject];
                script.SetSceneObject(gfzSceneObject);
            }

            return root;
        }

        public static Transform CreateGlobalParams(ColiScene scene)
        {
            var sceneParamsObj = new GameObject("Scene Parameters");
            var sceneParams = sceneParamsObj.AddComponent<GfzSceneParameters>();
            sceneParams.venue = CourseUtility.GetVenue(scene.ID);
            // TODO: embed course name in file, use that if it exists.
            sceneParams.courseName = CourseUtility.GetCourseName(scene.ID);
            sceneParams.courseIndex = scene.ID;
            sceneParams.author = "Amusement Vision";
            // Other data
            sceneParams.staticColliderMeshesActive = scene.staticColliderMeshesActive;
            sceneParams.circuitType = scene.circuitType;

            // Copy fog parameters over
            var fog = scene.fog;
            sceneParams.exportCustomFog = true; // whatever we import, use that
            sceneParams.fogInterpolation = fog.interpolation;
            sceneParams.fogNear = fog.fogRange.near;
            sceneParams.fogFar = fog.fogRange.far;
            var color = fog.colorRGBA;
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