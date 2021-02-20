using GameCube.GFZ.CourseCollision;
using UnityEngine;

namespace Manifold.IO.GFZ.CourseCollision
{
    [CreateAssetMenu(menuName = MenuConst.GfzCourseCollision + "COLI Scene")]
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
