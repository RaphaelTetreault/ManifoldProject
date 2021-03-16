using GameCube.GFZ;
using GameCube.GFZ.CourseCollision;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Manifold.IO.GFZ.CourseCollision
{
    [CreateAssetMenu(menuName = MenuConst.GfzCourseCollision + "COLI Scene Importer")]
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

            foreach (var course in courseScenes)
            {
                // Progress bar values
                var count = 0;
                var total = course.Value.sceneObjects.Length;

                // Create new, empty scene
                var sceneName = course.name;
                var scenePath = $"Assets/{importTo}/{sceneName}.unity";
                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                EditorSceneManager.SaveScene(scene, scenePath);
                // Keep reference of new scene
                scene = EditorSceneManager.OpenScene(scenePath);

                // TEMP DATA
                // Create track vis, set parameter
                var empty = new GameObject();
                empty.name = nameof(TempTrackVis);
                var trackVis = empty.AddComponent<TempTrackVis>();
                trackVis.SceneSobj = course;
                var pathVis = empty.AddComponent<DisplayVenueMetadataObjects>();
                pathVis.sceneSobj = course;
                var storyVis = empty.AddComponent<DisplayStoryObjects>();
                storyVis.SceneSobj = course;



                // Course-related values, used to find models
                // triple digits do "overflow", that's okay.
                var stageID = course.Value.ID;
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

                // Find all gameobjects, add them to scene
                foreach (var sceneObject in course.Value.sceneObjects)
                {
                    // HACK: skip empties. Should really just do "model not found"
                    if (string.IsNullOrEmpty(sceneObject.name))
                    {
                        continue;
                    }

                    var objectName = sceneObject.name;
                    var prefabName = $"pf_{objectName}";
                    var assetGuids = AssetDatabase.FindAssets(prefabName, searchFolders);

                    string assetPath = string.Empty;
                    if (assetGuids.Length == 1)
                    {
                        // We only found 1 model. Great!
                        assetPath = AssetDatabase.GUIDToAssetPath(assetGuids[0]);
                    }
                    else if (assetGuids.Length == 0)
                    {
                        // No models for this object. Make empty object.
                        var emptyGameObject = new GameObject();
                        emptyGameObject.name = $"NoModel:{objectName}";
                        // Stop here, go to next object in iteration
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
                        if (string.IsNullOrEmpty(assetPath))
                        {
                            // No models for this object. Make empty object.
                            var emptyGameObject = new GameObject();
                            emptyGameObject.name = $"NoModel:{objectName}";
                            // Stop here, go to next object in iteration
                            continue;
                        }
                    }

                    // Load asset based on previous triage
                    var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.GameObject>(assetPath);

                    // Progress bar update
                    var title = $"Generating Scene ({course.name})";
                    var info = $"{objectName}";
                    var progress = (float)count / total;
                    EditorUtility.DisplayProgressBar(title, info, progress);
                    count++;

                    // Create instance of the prefab
                    var instance = Instantiate(asset);
                    instance.name = objectName;

                    // TODO: attach components...
                    //var hasAnimation = gobj.animation != null;

                    // Tack data of object onto Unity GameObject for inspection
                    var sceneObjectData = instance.AddComponent<GfzSceneObject>();
                    sceneObjectData.self = sceneObject;

                    // Apply GFZ Transform values onto Unity Transform
                    sceneObject.transform.SetUnityTransform(instance.transform);
                }

                // HACK: force-add models for AX test stages
                if (stageID > 50)
                {
                    // Get models for AX scene
                    // One of each is used in scene, all relative to origin.
                    var hackSearchFolders = new string[] { stageFolder };
                    var hackGuids = AssetDatabase.FindAssets("t:prefab", hackSearchFolders);

                    // Progress bar variables
                    var hackCount = 0;
                    var hackTotal = hackGuids.Length;

                    foreach (var assetGuid in hackGuids)
                    {
                        // Progress bar
                        var title = $"Generating Scene ({course.name})";
                        var info = $"HACK: adding AX models...";
                        var progress = (float)hackCount / hackTotal;
                        EditorUtility.DisplayProgressBar(title, info, progress);
                        hackCount++;

                        // Load models
                        var hackPath = AssetDatabase.GUIDToAssetPath(assetGuid);
                        var hackObject = AssetDatabase.LoadAssetAtPath<GameObject>(hackPath);
                        var instance = Instantiate(hackObject);
                        instance.name = hackObject.name;
                    }
                }

                EditorSceneManager.SaveScene(scene, scenePath, false);
            } // foreach COLI_COURSE
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

    }
}