using UnityEngine;

namespace Manifold.IO.GFZ.CourseCollision
{
    /// <summary>
    /// Component class for Object Paths. Uses cases are: Lightning's lightning,
    /// Outer Space's meteors.
    /// </summary>
    public class GfzObjectPath : MonoBehaviour
    {
        [Header("Gizmos")]
        public float gizmosRadius = 10f;
        public Color gizmosColor = Color.white;
        [Header("Path")]
        public Transform from;
        public Transform to;

        private void OnDrawGizmos()
        {
            Gizmos.color = gizmosColor;
            Gizmos.DrawLine(from.position, to.position);
            Gizmos.DrawSphere(from.position, gizmosRadius * .95f);
            Gizmos.DrawWireSphere(from.position, gizmosRadius);
            Gizmos.DrawSphere(to.position, gizmosRadius);
        }
    }
}
