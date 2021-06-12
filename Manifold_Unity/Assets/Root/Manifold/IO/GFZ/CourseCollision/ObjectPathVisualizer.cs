using UnityEngine;

namespace Manifold.IO.GFZ.CourseCollision
{
    public class ObjectPathVisualizer : MonoBehaviour
    {
        public float radius = 10f;
        public Color color = Color.white;
        public Transform from;
        public Transform to;
        private void OnDrawGizmos()
        {
            Gizmos.color = color;
            Gizmos.DrawLine(from.position, to.position);
            Gizmos.DrawSphere(from.position, radius * .95f);
            Gizmos.DrawWireSphere(from.position, radius);
            Gizmos.DrawSphere(to.position, radius);
        }
    }
}
