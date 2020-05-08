using GameCube.FZeroGX.COLI_COURSE;
using StarkTools.IO;
using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Assertions;

[CreateAssetMenu(menuName = "Manifold/Generate/" + "Coli Stage Generator")]
public class ColiStageGenerator : ImportSobjs<ColiSceneSobj>
{
    public override string ProcessMessage
      => null;

    public override string HelpBoxMessage
        => null;

    public override string TypeName
        => null;

    protected override string DefaultQueryFormat
        => null;

    [SerializeField] ColiSceneSobj[] colis;

    public override void Import()
    {
        foreach (var coli in colis)
        {
            var count = 0;
            var total = coli.scene.gameObjects.Length;

            var sceneName = coli.name;
            var scenePath = $"Assets/_Scene/{sceneName}.unity";
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            EditorSceneManager.SaveScene(scene, scenePath);
            // Keep reference of new scene
            scene = EditorSceneManager.OpenScene(scenePath);

            foreach (var gobj in coli.scene.gameObjects)
            {
                var pfName = $"pf_{gobj.name}";
                var assets = AssetDatabase.FindAssets(pfName);
                var pfPrintName = pfName;
                // Temp. triage
                string path = string.Empty;
                if (assets.Length == 1)
                {
                    // We're good
                } else if (assets.Length == 0)
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
                var title = $"Generating Scene {coli.name})";
                var info = $"{pfPrintName}";
                var progress = count / (float)total;
                EditorUtility.DisplayProgressBar(title, info, progress);

                var instance = Instantiate(asset);
                instance.name = pfPrintName;

                // Set Unity Transform values
                gobj.transform.SetUnityTransform(instance.transform);

                count++;
            }
            EditorSceneManager.SaveScene(scene, scenePath, false);
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }
    }
}