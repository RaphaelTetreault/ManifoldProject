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
                    var pfPrintName = pfName;

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
                        pfPrintName = $"MISSING_MODEL_{gobj.name}";
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

                    //Debug.Log($"{pfName} - {assetPath}");
                    var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.GameObject>(assetPath);
                    UnityEngine.Assertions.Assert.IsFalse(asset == null, $"{gobj.name} - {assetPath}");
                        
                    #region dep
                    //// Temp. triage
                    //string assetPath = string.Empty;
                    //if (assetGuids.Length == 1)
                    //{
                    //    // We're good
                    //    assetPath = AssetDatabase.GUIDToAssetPath(assetGuids[0]);
                    //}
                    //else if (assetGuids.Length == 0)
                    //{
                    //    // Load empty
                    //    assetGuids = AssetDatabase.FindAssets("pf_NotFound");
                    //    assetPath = AssetDatabase.GUIDToAssetPath(assetGuids[0]);
                    //    pfPrintName = $"MISSING_MODEL_{pfName}";
                    //}
                    //else
                    //{
                    //    // Implement folder search
                    //    // Exception: Can return 2+ objects even if only 1 of each name.
                    //    // Ex: MUT_TUNNEL_B and MUT_TUNNEL_BAR both get flagged if input is "MUT_TUNNEL_B"

                    //    //var courseID = coliCourse.Value.courseID.ToString();
                    //    var id = coliCourse.Value.id.ToString("00");
                    //    var venueID = coliCourse.Value.courseVenueID.ToString().ToLower();
                    //    var possibles = new System.Collections.Generic.List<string>();

                    //    foreach (var assetGuid in assetGuids)
                    //    {
                    //        var assetName = AssetDatabase.GUIDToAssetPath(assetGuid);
                    //        var assetNameOnly = System.IO.Path.GetFileNameWithoutExtension(assetName);
                    //        if (assetNameOnly.Equals(pfName))
                    //        {
                    //            possibles.Add(assetName);
                    //        }
                    //    }

                    //    if (possibles.Count == 0)
                    //    {
                    //        // we just hit a pf_ object
                    //        assetGuids = AssetDatabase.FindAssets("pf_NotFound");
                    //        assetPath = AssetDatabase.GUIDToAssetPath(assetGuids[0]);
                    //        pfPrintName = $"MISSING_MODEL_{pfName}";
                    //    }
                    //    else if (possibles.Count == 1)
                    //    {
                    //        assetPath = possibles[0];
                    //    }
                    //    else
                    //    {
                    //        foreach (var a in possibles)
                    //        {
                    //            var directory = System.IO.Path.GetDirectoryName(a);
                    //            var isCorrectVenue = directory.Contains($"bg_{venueID}");
                    //            var isCorrectStage = directory.Contains($"st{id}");
                    //            if (isCorrectVenue || isCorrectStage)
                    //            {
                    //                assetPath = a;
                    //                break;
                    //            }
                    //        }
                    //    }

                    //    if (assetPath == string.Empty)
                    //    {
                    //        throw new System.Exception($"result \"{assetPath}\"");
                    //    }

                    //    UnityEngine.Assertions.Assert.IsTrue(assetPath != string.Empty);
                    //}
                    //var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.GameObject>(assetPath);
                    #endregion

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