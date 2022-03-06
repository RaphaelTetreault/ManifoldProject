using GameCube.GFZ.Stage;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    [CreateAssetMenu(menuName = Const.Menu.GfzCourseCollision + "COLI Scene")]
    public sealed class ColiSceneSobj : FileAssetWrapper<Scene>
    {
        [SerializeField]
        private string filePath;

        public string FilePath
        {
            get => filePath;
            set => filePath = value;
        }
    }

}
