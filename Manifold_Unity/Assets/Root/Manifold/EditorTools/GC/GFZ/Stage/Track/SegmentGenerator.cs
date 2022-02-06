using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCube.GFZ.CourseCollision;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public abstract class SegmentGenerator : MonoBehaviour
    {
        public abstract AnimationCurveTRS GetAnimationCurveTRS();
    }
}
