using GameCube.GFZ;
using GameCube.GFZ.CourseCollision;
using Manifold.IO;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Unity.Mathematics;

namespace Manifold.IO.GFZ.CourseCollision
{
    [CreateAssetMenu(menuName = Const.Menu.GfzCourseCollision + "COLI Scene Importer")]
    public class ColiSceneImporter : ExecutableScriptableObject,
        IImportable
    {
        #region MEMBERS

        [Header("Import Settings")]
        [SerializeField, BrowseFolderField()]
        protected string importFrom;

        [SerializeField, BrowseFolderField("Assets/")]
        protected string importTo;

        [SerializeField]
        protected IOOption importOption = IOOption.selectedFiles;

        [Header("Import Files")]
        [SerializeField] protected ColiSceneSobj[] courseScenes;

        #endregion

        public override string ExecuteText => "Import COLI as Unity Scene";

        public override void Execute() => Import();

        public void Import()
        {
            courseScenes = AssetDatabaseUtility.GetSobjByOption(courseScenes, importOption, importFrom);

            foreach (var scene in courseScenes)
            {
                // Create new, empty scene
                var sceneName = scene.name;
                var scenePath = $"Assets/{importTo}/{sceneName}.unity";
                var unityScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                EditorSceneManager.SaveScene(unityScene, scenePath);
                // Keep reference of new scene
                unityScene = EditorSceneManager.OpenScene(scenePath);

                // Course-related values, used to find models
                // Triple digit IDs do overflow the "00" format, that's okay.
                var stageID = scene.Value.ID;
                var stageNumber = stageID.ToString("00");
                var venueID = CourseUtility.GetVenueID(stageID).ToString().ToLower();

                // TEMP: Get folder at root of import path.
                // TODO: use parameter in sobj for base folder?
                var importFromRoot = importFrom.Split('/')[0];

                // Models are loaded from 3 folders.
                var initFolder = $"Assets/{importFromRoot}/init";
                var stageFolder = $"Assets/{importFromRoot}/stage/st{stageNumber}";
                var venueFolder = $"Assets/{importFromRoot}/bg/bg_{venueID}";
                var searchFolders = new string[] { initFolder, stageFolder, venueFolder };

                // SCENE OBJECTS
                CreateSceneObjects(scene, searchFolders);
                // ORIGIN OBJECTS
                CreateOriginObjects(scene, searchFolders);

                // MISC DATA
                // Create debug object for visualization at top of scene hierarchy
                CreateDisplayerDebugObject(scene);

                // TRIGGERS
                {
                    CreateArcadeCheckpoints(scene);
                    CreateUnknownTriggers(scene);
                }

                // Track data transforms
                CreateTrackTransformHierarchy(scene);
                // Checkpoints?
                CreateTrackIndexChains(scene);
                // Include other misc data
                IncludeStaticMeshColliders(scene, stageFolder);

                // Finally, save the scene file
                EditorSceneManager.SaveScene(unityScene, scenePath, false);
            } // foreach COLI_COURSE
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }


        /// <summary>
        /// Used to create a dummy object
        /// </summary>
        /// <param name="displayName">The name of the object created</param>
        /// <returns></returns>
        private GameObject CreateNoMeshObject()
        {
            // No models for this object. Make empty object.
            var noMeshObject = new GameObject();

            // Tag object with metadata
            noMeshObject.AddComponent<NoMeshTag>();

            // TEMP? Disable for visual clarity
            noMeshObject.SetActive(false);

            return noMeshObject;
        }

        private GameObject CreateInstanceFromDatabase(string assetPath)
        {
            // Load asset from database
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

            // Create instance of the prefab
            var instance = Instantiate(asset);
            return instance;
        }


        #region CREATE TRIGGERS
        private UnityEngine.Transform CreateArcadeCheckpoints(ColiScene scene)
        {
            var typeName = nameof(ArcadeCheckpointTrigger);
            string title = $"Creating {typeName}s";
            int count = 0;
            int total = scene.unknownTriggers.Length;
            string format = WidthFormat(scene.unknownTriggers);

            var root = new GameObject($"{typeName} [{total}]").transform;

            foreach (var trigger in scene.arcadeCheckpointTriggers)
            {
                count++;
                var name = $"{typeName} [{count.ToString(format)}]";
                ImportUtility.ProgressBar(count, total, name, title);

                var obj = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
                obj.transform.parent = root;

                // Assign transform values used for 
                obj.transform.position = trigger.transform.Position;
                obj.transform.rotation = trigger.transform.Rotation;
                obj.transform.localScale = trigger.transform.Scale * 10f; // TODO: in code when converted
            }

            return root;
        }

        private UnityEngine.Transform CreateCourseMetadataTriggers(ColiScene scene)
        {
            // Create general data for progress bar, naming
            var typeName = nameof(CourseMetadataTrigger);
            string title = $"Creating {typeName}s";
            int count = 0;
            int total = scene.unknownTriggers.Length;
            string format = WidthFormat(scene.unknownTriggers);

            // Root object for all triggers
            var root = new GameObject($"{typeName} [{total}]").transform;

            // iterate over each trigger
            foreach (var trigger in scene.courseMetadataTriggers)
            {
                //
                count++;
                var name = $"{typeName} [{count.ToString(format)}]";
                ImportUtility.ProgressBar(count, total, name, title);

                // Parse trigger type
                // There are 3 ways this data is used
                switch (trigger.courseMetadata)
                {
                    // Path data
                    case CourseMetadataType.Lightning_Lightning:
                        {
                            var pathObject = CreateMetadataPathObj(trigger);
                            pathObject.name = $"Lightning Path [{count}]";
                            pathObject.parent = root;
                        }
                        break;
                    case CourseMetadataType.OuterSpace_Meteor:
                        {
                            var pathObject = CreateMetadataPathObj(trigger);
                            pathObject.name = $"Meteor Path [{count}]";
                            pathObject.parent = root;
                        }
                        break;

                    // Obscure trigger data
                    case CourseMetadataType.BigBlueOrdeal:
                        {
                            var bboObject = CreateMetadataPathObj(trigger);
                            bboObject.name = $"Big Blue Ordeal Unk Trigger [{count}]";
                            bboObject.parent = root;
                        }
                        break;

                    // Story capsule data
                    case CourseMetadataType.Story1_CapsuleAX:
                        {
                            var storyCapsuleObject = CreateMetadataPathObj(trigger);
                            storyCapsuleObject.name = $"AX Story 1 Capsule [{count}]";
                            storyCapsuleObject.parent = root;
                        }
                        break;
                    case CourseMetadataType.Story5_Capsule:
                        {
                            var storyCapsuleObject = CreateMetadataPathObj(trigger);
                            storyCapsuleObject.name = $"Story 5 Capsule [{count}]";
                            storyCapsuleObject.parent = root;
                        }
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }

            return root;
        }
        private UnityEngine.Transform CreateMetadataPathObj(CourseMetadataTrigger data)
        {
            var rootPathObject = new GameObject().transform;

            var from = new GameObject("from").transform;
            from.parent = rootPathObject;
            from.position = data.PositionFrom;
            from.rotation = data.Rotation;

            var to = new GameObject("to").transform;
            to.parent = rootPathObject;
            to.position = data.PositionTo;
            to.rotation = data.Rotation;

            return rootPathObject;
        }
        private UnityEngine.Transform CreateMetadataBboObj(CourseMetadataTrigger data)
        {
            // Object named by caller
            var obj = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            //var obj = new GameObject("Big Blue Ordeal Trigger").transform;
            obj.transform.position = data.Position;
            obj.transform.rotation = data.Rotation;
            obj.transform.localScale = data.ScaleBigBlueOrdeal;

            return obj;
        }
        private UnityEngine.Transform CreateMetadataCapsuleObj(CourseMetadataTrigger data)
        {
            var capsuleObject = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
            capsuleObject.position = data.Position;
            capsuleObject.rotation = data.Rotation;
            capsuleObject.localScale = data.ScaleCapsule;

            return capsuleObject;
        }



        private void CreateUnknownTriggers(ColiScene scene)
        {
            var parentObject = new GameObject();
            parentObject.name = $"{nameof(UnknownTrigger)} Debug Objects";

            string title = $"Createing {nameof(UnknownTrigger)}";
            int count = 0;
            int total = scene.unknownTriggers.Length;
            string format = WidthFormat(scene.unknownTriggers);

            foreach (var trigger in scene.unknownTriggers)
            {
                count++;
                // TODO: add data as component, not name?
                var name = $"[{count.ToString(format)}/{total}] Trigger a:{(ushort)trigger.unk_0x20:X4} b:{(ushort)trigger.unk_0x22:X4}";
                ImportUtility.ProgressBar(count, total, name, title);

                var triggerObject = new GameObject();
                triggerObject.transform.parent = parentObject.transform;
                //var meshFilter = triggerObject.AddComponent<MeshFilter>();
                //var meshRenderer = triggerObject.AddComponent<MeshRenderer>();

                // Assign transform values used for 
                triggerObject.transform.position = trigger.transform.Position;
                triggerObject.transform.rotation = trigger.transform.Rotation;
                triggerObject.transform.localScale = trigger.transform.Scale * 10f;

                var displayer = triggerObject.AddComponent<TempDisplayUnknownTrigger1>();
                displayer.unk1 = trigger.unk_0x20;
                displayer.unk2 = trigger.unk_0x22;
            }
        }

        #endregion

        private void CreateDisplayerDebugObject(ColiSceneSobj scene)
        {
            // TEMP DATA
            // Create track vis, set parameter
            var empty = new GameObject();
            empty.name = "DEBUG - Display Data";
            // Add displayers and assign value to all
            var displayables = new List<IColiCourseDisplayable>
                {
                    empty.AddComponent<DisplayCourseMetadataTrigger>(),
                    empty.AddComponent<DisplayStoryObjects>(),
                    empty.AddComponent<DisplayTrackCheckpoint>(),
                    empty.AddComponent<DisplayTrackIndexes>(),
                    empty.AddComponent<DisplayTrigger>(),
                    empty.AddComponent<DisplayUnknownTrigger2>(),
                    empty.AddComponent<DisplayVisualEffectTrigger>(),
                    empty.AddComponent<TempLodView>(),
                };
            foreach (var displayable in displayables)
            {
                displayable.SceneSobj = scene;
            }
        }


        #region Track Transform Hierarchies

        private int elementIndex = 0;
        public void CreateTrackTransformHierarchy(ColiScene scene)
        {
            // Get mesh for debugging
            var mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");

            // Create object to house data
            var controlPointsParent = new GameObject();
            controlPointsParent.name = "Control Points Hierarchy";

            //
            elementIndex = 0;

            // Loop over every top transform
            int count = 0;
            int total = scene.rootTrackTransforms.Length;
            foreach (var trackTransform in scene.rootTrackTransforms)
            {
                // Recursively create transforms
                count++;
                var name = $"[{count}/{total}] Control Point | {count}";
                CreateControlPointRecursive(trackTransform, controlPointsParent, mesh, name, 0);
            }
        }

        public void CreateTrackTransformSet(ColiScene scene)
        {
            // Get mesh for debugging
            var mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");

            // Create object to house data
            var controlPointsRoot = new GameObject();
            controlPointsRoot.name = "Control Points Set";

            // Loop over every top transform
            //int count = 0;
            int index = 0;
            int total = scene.rootTrackTransforms.Length;
            foreach (var trackTransform in scene.rootTrackTransforms)
            {
                //
                var name = $"[{++index}/{total}] Control Point";
                var controlPointSet = new GameObject();
                controlPointSet.name = name;
                controlPointSet.transform.parent = controlPointsRoot.transform;
                CreateControlPointSequential(trackTransform, controlPointSet, mesh, name, 0, 0);

                // Recursively create transforms
                //count++;
                //var name = $"[{count}/{total}] Control Point | {count}";
                //CreateControlPointRecursive(trackTransform, controlPointsParent, mesh, name, 0);
            }
        }

        public void CreateControlPointRecursive(TrackTransform trackTransform, GameObject parent, Mesh mesh, string name, int depth)
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
            int count = 1;
            int total = trackTransform.children.Length;
            foreach (var child in trackTransform.children)
            {
                name = $"{name}.{count++}";
                CreateControlPointRecursive(child, controlPoint, mesh, name, depth + 1);
            }
        }

        public void CreateControlPointSequential(TrackTransform trackTransform, GameObject parent, Mesh mesh, string name, int depth, int index)
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
            index = 0;
            foreach (var next in trackTransform.children)
            {
                CreateControlPointSequential(next, parent, mesh, name, depth + 1, ++index);
            }
        }

        #endregion

        // COLLIDER OBJECTS
        public void IncludeStaticMeshColliders(ColiSceneSobj sceneSobj, string stageFolder)
        {
            var scene = sceneSobj.Value;
            var parent = new GameObject();
            parent.name = $"Static Mesh Colliders";

            // TODO: it would be wiser to tag the prefabs with some tag type so that
            // we need only pull in objects of that type. The string loading method
            // is bound to break at some point.
            for (int i = 0; i < StaticColliderMeshes.GetSurfacesCount(scene); i++)
            {
                var meshName = $"st{scene.ID:00}_{i:00}_{(StaticMeshColliderProperty)i}";
                var assetPath = $"{stageFolder}/pf_{meshName}.prefab";
                var asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                if (asset == null)
                {
                    Debug.Log($"Could not find asset named '{assetPath}'.");
                    continue;
                }

                var instance = Instantiate(asset, parent.transform);
                instance.name = meshName;
            }

        }

        public void TestTransformHeirarchy(ColiSceneSobj sceneSobj)
        {
            var scene = sceneSobj.Value;

            var parent = new GameObject();
            parent.name = $"Test Sample Path";

            //
            var increment = 1f / 1000f * scene.rootTrackTransforms.Length;
            int count = 0;
            foreach (var tt in scene.rootTrackTransforms)
            {
                var subgroup = new GameObject();
                subgroup.name = $"Subgroup {++count}";
                subgroup.transform.parent = parent.transform;

                var topology = tt.trackTopology;
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
        }

        public float GetCurveTime(UnityEngine.AnimationCurve curve)
        {
            if (curve.length == 0)
                return 0f;

            return curve.keys[curve.length - 1].time;
        }

        public void NewTestTransformHierarchy(ColiSceneSobj sceneSobj)
        {
            var scene = sceneSobj.Value;

            var parent = new GameObject();
            parent.name = $"Test Sample Path";

            //
            var increment = 1f / 1000f * scene.rootTrackTransforms.Length;
            int count = 0;
            foreach (var tt in scene.rootTrackTransforms)
            {
                var subgroup = new GameObject();
                subgroup.name = $"Subgroup {++count}";
                subgroup.transform.parent = parent.transform;

                var topology = tt.trackTopology;
                var transformMatrix = GetTransformMatrix(tt, increment);

                for (float t = 0f; t < 1f; t += increment)
                {
                    var animMatrix = GetAnimMatrix(tt, t);
                    var finalMatrix = transformMatrix * animMatrix;

                    var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.name = $"time {t:0.000}";
                    cube.transform.parent = subgroup.transform;

                    cube.transform.position = finalMatrix.Position();
                    cube.transform.rotation = finalMatrix.Rotation();
                    cube.transform.localScale = finalMatrix.Scale();
                }
            }
        }

        public Matrix4x4 GetTransformMatrix(TrackTransform trackTransform, float increment)
        {
            var transformMatrix = new Matrix4x4();
            transformMatrix.SetTRS(
                trackTransform.localPosition,
                Quaternion.Euler(trackTransform.localRotation),
                trackTransform.localScale);

            // get child matrix
            if (trackTransform.topologyMetadata == TrackTopologyMetadata.IsTransformParent)
            {
                var childMatrix = GetTransformMatrix(trackTransform.children[0], increment);
                return transformMatrix * childMatrix;
            }

            return transformMatrix;
        }

        public Matrix4x4 GetAnimMatrix(TrackTransform trackTransform, float time)
        {
            var topology = trackTransform.trackTopology;
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
            if (trackTransform.topologyMetadata == TrackTopologyMetadata.IsTransformParent)
            {
                var childMatrix = GetAnimMatrix(trackTransform.children[0], time);

                return animationMatrix * childMatrix;
            }

            return animationMatrix;
        }

        public void CreateTrackIndexChains(ColiSceneSobj sceneSobj)
        {
            var scene = sceneSobj.Value;

            var root = new GameObject();
            root.name = $"Track Index Chains";

            int chainIndex = 0;
            var trackNodes = sceneSobj.Value.trackNodes;
            foreach (var indexList in sceneSobj.Value.trackCheckpointMatrix.indexLists)
            {
                var chain = new GameObject();
                chain.name = $"Chain {chainIndex++}";
                chain.transform.parent = root.transform;

                for (int i = 0; i < indexList.Length; i++)
                {
                    int index = indexList.Indexes[i];
                    var node = trackNodes[index];

                    for (int j = 0; j < node.points.Length; j++)
                    {
                        var position = node.points[j].positionStart;
                        var instance = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        instance.transform.position = position;
                        instance.transform.localScale = Vector3.one * 5f;
                        instance.name = $"{index}.{j}";
                        instance.transform.parent = chain.transform;
                    }
                }
            }
        }

        public string GetAssetPath(string prefabName, params string[] searchFolders)
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
        public string WidthFormat(System.Array array)
        {
            int displayDigitsLength = array.Length.ToString().Length;
            var digitsFormat = new string('0', displayDigitsLength);
            return digitsFormat;
        }

        public void CreateSceneObjects(ColiSceneSobj scene, params string[] searchFolders)
        {

            // Get some metadata from the number of scene objects
            var sceneObjects = scene.Value.sceneObjects;
            // Create a string format from the highest index number used
            var digitsFormat = WidthFormat(sceneObjects);
            int count = 0;
            int total = sceneObjects.Length;
            // Create root for all scene objects
            var sceneObjectsRoot = new GameObject($"Scene Objects").transform;

            // Find all scene objects, add them to scene
            foreach (var sceneObject in sceneObjects)
            {
                // Generate the names of the objects we want
                var objectName = sceneObject.nameCopy;
                var prefabName = $"pf_{objectName}";
                var displayName = $"[{count.ToString(digitsFormat)}] {objectName}";

                // Progress bar update
                var title = $"Generating Scene ({scene.name})";
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
                assetInstance.transform.parent = sceneObjectsRoot;

                // Tack data of object onto Unity GameObject for inspection
                var sceneObjectData = assetInstance.AddComponent<TagSceneObject>();
                sceneObjectData.Data = sceneObject;

                // Set object transform
                if (sceneObject.transformPtr.IsNotNullPointer)
                {
                    // Perhaps best way when matrix exists? No compression rotation
                    assetInstance.transform.position = sceneObject.transformMatrix3x4.Position;
                    assetInstance.transform.rotation = sceneObject.transformMatrix3x4.Matrix.ToUnityMatrix4x4().rotation; // 'quaternion' is... lacking
                    assetInstance.transform.localScale = sceneObject.transformMatrix3x4.Scale;
                }
                else
                {
                    // Apply GFZ Transform values onto Unity Transform
                    assetInstance.transform.position = sceneObject.transform.Position;
                    assetInstance.transform.rotation = sceneObject.transform.DecomposedRotation.Rotation; // Still using UnityEngine, not Unity.Mathematics
                    assetInstance.transform.localScale = sceneObject.transform.Scale;
                }
            }
        }
        public void CreateOriginObjects(ColiSceneSobj scene, params string[] searchFolders)
        {
            // Get some metadata from the number of scene objects
            var originObjects = scene.Value.sceneOriginObjects;

            // Create a string format from the highest index number used
            var digitsFormat = WidthFormat(originObjects);
            int count = 0;
            int total = originObjects.Length;

            // Create root for all scene objects
            var sceneObjectsRoot = new GameObject($"Origin Objects").transform;

            // Find all origin objects, add them to scene
            foreach (var originObject in originObjects)
            {
                // Generate the names of the objects we want
                var objectName = originObject.NameCopy;
                var prefabName = $"pf_{objectName}";
                var displayName = $"[{count.ToString(digitsFormat)}] {objectName}";

                // Progress bar update
                var title = $"Generating Scene ({scene.name})";
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
                assetInstance.transform.parent = sceneObjectsRoot;
            }
        }
    }

    public static class AnimationCurveExtensions
    {
        public static float EvaluateDefault(this UnityEngine.AnimationCurve curve, float time, float @default)
        {
            if (time == 0f)
                return @default;

            return curve.Evaluate(time);
        }
    }
}