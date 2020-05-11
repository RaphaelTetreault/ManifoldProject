using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using GameCube.FZeroGX.COLI_COURSE;

namespace Manifold.IO.GFZX01
{
    [CreateAssetMenu(menuName = "Manifold/Import/" + "NEW COLI Scene Importer")]
    public class ColiSceneImporter : ExecutableScriptableObject,
        IImportable
    {
        [Header("Import Settings")]
        [SerializeField, BrowseFolderField()]
        protected string importSource;
        [SerializeField, BrowseFolderField("Assets/")]
        protected string importDestination;
        [SerializeField]
        protected ImportOption importOption = ImportOption.selectedFiles;

        [Header("Import Files")]
        [SerializeField] protected ColiSceneSobj[] colis;

        public override string ExecuteText => "Import COLI as Scene";

        public override void Execute() => Import();

        public void Import()
        {
            // Get Sobjs based on import option
            switch (importOption)
            {
                case ImportOption.selectedFiles:
                    // Do nothing and use files set up in inspector
                    break;

                case ImportOption.allOfTypeInImportSource:
                    colis = ImportUtility.GetAllOfTypeFromAssetDatabase<ColiSceneSobj>(importSource);
                    break;

                case ImportOption.allOfType:
                    colis = ImportUtility.GetAllOfTypeFromAssetDatabase<ColiSceneSobj>();
                    break;

                default:
                    throw new NotImplementedException();
            }

            foreach (var coliCourse in colis)
            {
                var count = 0;
                var total = coliCourse.scene.gameObjects.Length;

                var sceneName = coliCourse.name;
                var scenePath = $"Assets/_Scene/{sceneName}.unity";
                var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
                EditorSceneManager.SaveScene(scene, scenePath);
                // Keep reference of new scene
                scene = EditorSceneManager.OpenScene(scenePath);

                foreach (var gobj in coliCourse.scene.gameObjects)
                {
                    var pfName = $"pf_{gobj.name}";
                    var assets = AssetDatabase.FindAssets(pfName);
                    var pfPrintName = pfName;
                    // Temp. triage
                    string path = string.Empty;
                    if (assets.Length == 1)
                    {
                        // We're good
                    }
                    else if (assets.Length == 0)
                    {
                        // Load empty
                        assets = AssetDatabase.FindAssets("pf_NotFound");
                        pfPrintName = $"MISSING_MODEL_{pfName}";
                        path = assets[0];
                    }
                    else
                    {
                        // Implement folder search
                        // Exception: Can return 2+ objects even if only 1 of each name.
                        // Ex: MUT_TUNNEL_B and MUT_TUNNEL_BAR both get flagged if input is "MUT_TUNNEL_B"
                        foreach (var assetStr in assets)
                        {
                            if (assetStr.Equals(pfName))
                            {
                                assets = AssetDatabase.FindAssets(assetStr);
                                break;
                            }
                        }

                    }
                    path = AssetDatabase.GUIDToAssetPath(assets[0]);
                    //string path = AssetDatabase.GUIDToAssetPath(assets[0]);
                    UnityEngine.GameObject asset = AssetDatabase.LoadAssetAtPath<UnityEngine.GameObject>(path);

                    //// Progress bar update
                    var title = $"Generating Scene {coliCourse.name})";
                    var info = $"{pfPrintName}";
                    var progress = count / (float)total;
                    EditorUtility.DisplayProgressBar(title, info, progress);

                    var instance = Instantiate(asset);
                    instance.name = pfPrintName;

                    // Set Unity Transform values
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