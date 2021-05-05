using GameCube.GFZ;
using GameCube.GFZ.CourseCollision;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

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
                // Progress bar values
                var count = 0;
                var total = scene.Value.sceneObjects.Length;

                // Create new, empty scene
                var sceneName = scene.name;
                var scenePath = $"Assets/{importTo}/{sceneName}.unity";
                var unityScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                EditorSceneManager.SaveScene(unityScene, scenePath);
                // Keep reference of new scene
                unityScene = EditorSceneManager.OpenScene(scenePath);
                
                // DEBUGGING DATA
                {
                    // Create debug object for visualization at top of scene hierarchy
                    CreateDisplayerDebugObject(scene);

                    // Unknown volumes
                    CreateUnknownTrigger1Volumes(scene);

                    // Track data transforms
                    CreateTrackTransformHierarchy(scene);
                    CreateTrackTransformSet(scene);
                }

                // Course-related values, used to find models
                // Triple digits do overflow the "00" format, that's okay.
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

                // Get some metadata from the number of scene objects
                var sceneObjects = scene.Value.sceneObjects;
                // Create a string format from the highest index number used
                int displayDigitsLength = sceneObjects.Length.ToString().Length;
                var digitsFormat = new string('0', displayDigitsLength);
                
                // Find all scene objects, add them to scene
                foreach (var sceneObject in sceneObjects)
                {
                    // HACK: skip empties. Should really just do "model not found"
                    if (string.IsNullOrEmpty(sceneObject.name))
                    {
                        count++;
                        continue;
                    }

                    // Generate the names of the objects we want
                    var objectName = sceneObject.name;
                    var prefabName = $"pf_{objectName}";
                    var displayName = $"[{count.ToString(digitsFormat)}] {objectName}";

                    // Store all possible matches for the object name in questions using the folder constraints
                    var assetGuids = AssetDatabase.FindAssets(prefabName, searchFolders);
                    // This variable will store the path to the object we want
                    var assetPath = string.Empty;

                    // Begin a triage of the asset database
                    if (assetGuids.Length == 1)
                    {
                        // We only found 1 model. Great!
                        assetPath = AssetDatabase.GUIDToAssetPath(assetGuids[0]);
                    }
                    else if (assetGuids.Length == 0)
                    {
                        // No models for this object. Make empty object.
                        CreateNoMeshObject(displayName);
                        // Stop here, go to next object in iteration
                        count++;
                        continue;
                    }
                    else
                    {
                        // We have multiple object matches
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

                        // Case where name is simple (ei: TEST) and flags a bunch of models, but no models exists for it.
                        // Moreover, skips models with empty string names (which is possible)
                        if (string.IsNullOrEmpty(assetPath))
                        {
                            // No models for this object. Make empty object.
                            CreateNoMeshObject(displayName);
                            // Stop here, go to next object in iteration
                            count++;
                            continue;
                        }
                    }

                    // Load asset based on previous triage
                    var asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                    // Progress bar update
                    var title = $"Generating Scene ({scene.name})";
                    var info = $"{displayName}";
                    var progress = (float)count / total;
                    EditorUtility.DisplayProgressBar(title, info, progress);
                    count++;

                    // Create instance of the prefab
                    var instance = Instantiate(asset);
                    instance.name = displayName;

                    // Tack data of object onto Unity GameObject for inspection
                    var sceneObjectData = instance.AddComponent<TagSceneObject>();
                    sceneObjectData.Data = sceneObject;

                    //
                    if (sceneObject.transformPtr.IsNotNullPointer)
                    {
                        // Perhaps best way when matrix exists? No compression rotation
                        instance.transform.position = sceneObject.transformMatrix3x4.Position;
                        instance.transform.rotation = sceneObject.transformMatrix3x4.Rotation;
                        instance.transform.localScale = sceneObject.transformMatrix3x4.Scale;
                    }
                    else
                    {
                        // Apply GFZ Transform values onto Unity Transform
                        instance.transform.position = sceneObject.transform.Position;
                        instance.transform.rotation = sceneObject.transform.DecomposedRotation.Rotation;
                        instance.transform.localScale = sceneObject.transform.Scale;
                    }
                }

                // HACK: force-add models for AX test stages
                // In the future, this will break with custom stages with indexes > 50
                if (stageID > 50)
                {
                    CreateLegacySceneFromAX(scene, stageFolder);
                }

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
        private GameObject CreateNoMeshObject(string displayName)
        {
            // No models for this object. Make empty object.
            var noMeshObject = new GameObject();
            noMeshObject.name = displayName;
            // Tag object with metadata
            noMeshObject.AddComponent<NoMeshTag>();
            // TEMP? Disable for visual clarity
            noMeshObject.SetActive(false);

            return noMeshObject;
        }

        private void CreateUnknownTrigger1Volumes(ColiScene scene)
        {
            var parentObject = new GameObject();
            parentObject.name = $"{nameof(UnknownTrigger1)} Debug Objects";

            int triggerCount = 0;
            int triggerTotal = scene.unknownTrigger1s.Length;
            foreach (var trigger in scene.unknownTrigger1s)
            {
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

                triggerObject.name = $"Trigger[{triggerCount}/{triggerTotal}] a:{(ushort)trigger.unk_0x20:X4} b:{(ushort)trigger.unk_0x22:X4}";
                triggerCount++;
            }
        }

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

        private void CreateLegacySceneFromAX(ColiSceneSobj scene, string stageFolder)
        {
            // Get models for AX scene
            // One of each is used in scene, all relative to origin.
            var searchFolders = new string[] { stageFolder };
            var guids = AssetDatabase.FindAssets("t:prefab", searchFolders);

            // Progress bar variables
            var count = 0;
            var total = guids.Length;

            foreach (var assetGuid in guids)
            {
                // Progress bar
                var title = $"Generating Scene ({scene.name})";
                var info = $"HACK: adding AX models...";
                var progress = (float)count / total;
                EditorUtility.DisplayProgressBar(title, info, progress);
                count++;

                // Load models
                var hackPath = AssetDatabase.GUIDToAssetPath(assetGuid);
                var hackObject = AssetDatabase.LoadAssetAtPath<GameObject>(hackPath);
                var instance = Instantiate(hackObject);
                instance.name = hackObject.name;
            }
        }


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
            int total = scene.trackTransforms.Count;
            foreach (var trackTransform in scene.trackTransforms)
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
            int total = scene.trackTransforms.Count;
            foreach (var trackTransform in scene.trackTransforms)
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
    }
}