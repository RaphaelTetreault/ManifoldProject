using Manifold.IO;
using Nintendo64.FZX;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Manifold.EditorTools.N64.FZX
{
    [CreateAssetMenu(menuName = Const.Menu.Fzx + "FZEP Track Importer")]
    public static class FzxFzepImporter
    {
        public static void Import(TextAsset[] courses)
        {
            foreach (var textAsset in courses)
            {
                var path = AssetDatabase.GetAssetPath(textAsset);
                path = Path.GetFullPath(path);

                using (var reader = new StreamReader(File.OpenRead(path)))
                {
                    Debug.Log(textAsset.name);
                    var fzep = new FzepCourse();
                    fzep.Deserialize(reader);

                    // Create new, empty scene
                    var sceneName = $"{textAsset.name} {fzep.name} {fzep.descriptionEn}";
                    var scenePath = $"Assets/FZEP-Temp/{sceneName}.unity";
                    var unityScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                    EditorSceneManager.SaveScene(unityScene, scenePath);
                    // Keep reference of new scene
                    unityScene = EditorSceneManager.OpenScene(scenePath);

                    var bezierHandler = new GameObject();
                    bezierHandler.name = "Bezier Handler";
                    var bezier = bezierHandler.AddComponent<FzxFzepBezier>();

                    foreach (var controlPoint in fzep.controlPoints)
                    {
                        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        cube.name = controlPoint.name;
                        cube.transform.position = controlPoint.Position;

                        bezier.nodes.Add(cube.transform);
                    }

                    EditorSceneManager.SaveScene(unityScene, scenePath);
                }
            }
        }

    }
}