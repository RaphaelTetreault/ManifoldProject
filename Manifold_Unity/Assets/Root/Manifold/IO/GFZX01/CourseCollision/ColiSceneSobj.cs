using GameCube.GFZX01.CourseCollision;
using UnityEngine;

namespace Manifold.IO.GFZX01.CourseCollision
{
    [CreateAssetMenu(menuName = MenuConst.GFZX01_CourseCollision + "COLI Scene")]
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
