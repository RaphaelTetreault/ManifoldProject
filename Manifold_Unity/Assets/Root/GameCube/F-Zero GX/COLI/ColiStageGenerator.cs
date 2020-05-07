using GameCube.FZeroGX.COLI_COURSE;
using StarkTools.IO;
using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;
using System.ComponentModel;

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
            EditorSceneManager.OpenScene(scenePath);

            foreach (var gobj in coli.scene.gameObjects)
            {
                var pfName = $"pf_{gobj.name}";
                var assets = AssetDatabase.FindAssets(pfName);
                Assert.IsTrue(assets.Length == 1);
                var path = AssetDatabase.GUIDToAssetPath(assets[0]);
                var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.GameObject>(path);

                //// Progress bar update
                var title = $"Generating Ssene {coli.name})";
                var info = $"{path}";
                var progress = count / (float)total;
                EditorUtility.DisplayProgressBar(title, info, progress);

                var instance = Instantiate(asset);
                gobj.transform.SetUnityTransform(instance.transform);
                count++;
            }
            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }
    }
}