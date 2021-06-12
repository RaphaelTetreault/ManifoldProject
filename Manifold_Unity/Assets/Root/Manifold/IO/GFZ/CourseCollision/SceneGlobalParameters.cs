using GameCube.GFZ.CourseCollision;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.IO.GFZ.CourseCollision
{
    public class SceneGlobalParameters : MonoBehaviour
    {
        public float trackLength;
        public float trackMinHeight;
        public BoundsXZ trackBounds;
        public Fog fog;
    }
}
