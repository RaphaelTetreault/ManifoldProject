using GameCube.GFZ;
using GameCube.GFZ.CourseCollision;
using UnityEngine;

namespace Manifold.IO.GFZ.CourseCollision
{
    public class SceneGlobalParameters : MonoBehaviour
    {
        public Venue venue;

        public float trackLength;
        public float trackMinHeight;
        public BoundsXZ trackBounds;
        public Fog fog;
    }
}
