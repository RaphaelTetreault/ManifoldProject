using GameCube.GFZX01.CourseCollision;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Manifold.IO.GFZX01.CourseCollision
{
    [CreateAssetMenu(menuName = MenuConst.GFZX01_CourseCollision + "COLI Scene Importer")]
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
        [SerializeField] protected ColiSceneSobj[] colis;

        #endregion

        public override string ExecuteText => "Import COLI as Unity Scene";

        public override void Execute() => Import();

        public void Import()
        {
            colis = IOUtility.GetSobjByOption(colis, importOption, importFrom);

            foreach (var coliCourse in colis)
            {
                var count = 0;
                var total = coliCourse.Value.gameObjects.Length;

                // Create new, empty scene
                var sceneName = coliCourse.name;
                var scenePath = $"Assets/{importTo}/{sceneName}.unity";
                var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
                EditorSceneManager.SaveScene(scene, scenePath);
                // Keep reference of new scene
                scene = EditorSceneManager.OpenScene(scenePath);

                // Create track vis, set parameter
                var empty = new UnityEngine.GameObject();
                empty.name = nameof(TempTrackVis);
                var trackVis = empty.AddComponent<TempTrackVis>();
                trackVis.Coli = coliCourse;
                //

                foreach (var gobj in coliCourse.Value.gameObjects)
                {
                    // HACK: skip empties. Should really just do "model not found"
                    if (string.IsNullOrEmpty(gobj.name))
                    {
                        continue;
                    }


                    var pfName = $"pf_{gobj.name}";

                    var stageNumber = coliCourse.Value.id.ToString("00"); // triple digits do overflow, that's okay.
                    var venueID = coliCourse.Value.courseVenueID.ToString().ToLower();

                    // Models are loaded from 3 folders.
                    // Stage, Venue, and/or Init
                    var searchFolders = new string[] {
                        $"Assets/{"GFZJ01"}/bg/bg_{venueID}",
                        $"Assets/{"GFZJ01"}/stage/st{stageNumber}",
                        $"Assets/{"GFZJ01"}/init" };

                    var assetGuids = AssetDatabase.FindAssets(pfName, searchFolders);
                    // Remove "pf_" prefix
                    var pfPrintName = pfName.Remove(0, 3);

                    string assetPath = string.Empty;
                    if (assetGuids.Length == 1)
                    {
                        // We're good
                        assetPath = AssetDatabase.GUIDToAssetPath(assetGuids[0]);
                    }
                    else if (assetGuids.Length == 0)
                    {
                        // Load empty
                        assetGuids = AssetDatabase.FindAssets("pf_NotFound");
                        assetPath = AssetDatabase.GUIDToAssetPath(assetGuids[0]);
                        pfPrintName = $"NoModel:{gobj.name}";
                    }
                    else
                    {
                        foreach (var assetGuid in assetGuids)
                        {
                            var tempAssetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
                            var tempAssetName = System.IO.Path.GetFileNameWithoutExtension(tempAssetPath);
                            if (tempAssetName.Equals(pfName))
                            {
                                assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
                                break;
                            }
                        }

                        // Case where name is simple (ei: TEST) and flags a bunch of models, but no models exists for it.
                        if (string.IsNullOrEmpty(assetPath))
                        {
                            Debug.Log($"ITERATION FAILED. STAGE:\"{coliCourse.FileName}\" NAME:\"{gobj.name}\" - PATH:\"{assetPath}\"");
                            // Load empty
                            assetGuids = AssetDatabase.FindAssets("pf_NotFound");
                            assetPath = AssetDatabase.GUIDToAssetPath(assetGuids[0]);
                            pfPrintName = $"MISSING_MODEL_{gobj.name}";
                        }
                    }

                    var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.GameObject>(assetPath);
                    //UnityEngine.Assertions.Assert.IsFalse(asset == null, $"{gobj.name} - {assetPath}");
                        
                    //// Progress bar update
                    var title = $"Generating Scene ({coliCourse.name})";
                    var info = $"{pfPrintName}";
                    var progress = (float)count / total;
                    EditorUtility.DisplayProgressBar(title, info, progress);


                    var instance = Instantiate(asset);
                    instance.name = pfPrintName;

                    // TODO: attach components...
                    //var hasAnimation = gobj.animation != null;

                    // Tack data of object onto Unity GameObject for inspection
                    var gameObjectDef = instance.AddComponent<GfzGameObject>();
                    gameObjectDef.self = gobj;

                    // Apply GFZ Transform values onto Unity Transform
                    gobj.transform.SetUnityTransform(instance.transform);

                    count++;
                } // foreach GameObject
                EditorSceneManager.SaveScene(scene, scenePath, false);
            } // foreach COLI_COURSE
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

    }
}