using GameCube.GFZ.CourseCollision;
using Nintendo64.FZX;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Manifold.IO.FZX
{
    [CreateAssetMenu(menuName = Const.Menu.Fzx + "FZEP Track Importer")]
    public class FzxFzepImporter : ExecutableScriptableObject,
    IImportable
    {
        #region MEMBERS

        [Header("Import Settings")]
        //[SerializeField, BrowseFolderField()]
        //protected string importFrom;

        [SerializeField, BrowseFolderField("Assets/")]
        protected string importTo;

        //[SerializeField]
        //protected IOOption importOption = IOOption.selectedFiles;

        [Header("Import Files")]
        [SerializeField] protected TextAsset[] courses;

        #endregion

        public override string ExecuteText => $"Import FZEP Track as GFZ";

        public override void Execute() => Import();

        public void Import()
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
                    var scenePath = $"Assets/{importTo}/{sceneName}.unity";
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