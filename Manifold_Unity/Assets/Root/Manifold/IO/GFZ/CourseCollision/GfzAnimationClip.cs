using GameCube.GFZ.CourseCollision;
using UnityEngine;

namespace Manifold.IO.GFZ.CourseCollision
{
    public class GfzAnimationClip : MonoBehaviour,
        IGfzConvertable<GameCube.GFZ.CourseCollision.AnimationClip>
    {
        [SerializeField] private bool exportAnimationClip;
        [SerializeField] private GameCube.GFZ.CourseCollision.AnimationClip srcAnimationClip;


        public GameCube.GFZ.CourseCollision.AnimationClip ExportGfz()
        {
            if (exportAnimationClip)
            {
                return srcAnimationClip;
            }
            else
            {
                return null;
            }
        }

        public void ImportGfz(GameCube.GFZ.CourseCollision.AnimationClip value)
        {
            bool hasAnimationClip = value != null;
            if (hasAnimationClip)
            {
                srcAnimationClip = value;
            }
            exportAnimationClip = hasAnimationClip;
        }
    }
}
