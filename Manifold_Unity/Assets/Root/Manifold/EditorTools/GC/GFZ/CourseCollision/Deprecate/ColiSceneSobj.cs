using GameCube.GFZ.CourseCollision;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.CourseCollision
{
    [CreateAssetMenu(menuName = Const.Menu.GfzCourseCollision + "COLI Scene")]
    public sealed class ColiSceneSobj : FileAssetWrapper<ColiScene>
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
