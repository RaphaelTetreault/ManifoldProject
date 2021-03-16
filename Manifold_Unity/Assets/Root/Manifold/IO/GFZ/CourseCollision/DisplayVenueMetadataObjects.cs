using UnityEngine;

namespace Manifold.IO.GFZ.CourseCollision
{
    public class DisplayVenueMetadataObjects : MonoBehaviour
    {
        public ColiSceneSobj sceneSobj;
        public float radius = 15f;
        public Color color = Color.green;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            foreach (var item in sceneSobj.Value.venueMetadataObjects)
            {
                Gizmos.DrawSphere(item.position, radius);
                Gizmos.DrawWireSphere(item.positionOrScale, radius);
                Gizmos.DrawLine(item.position, item.positionOrScale);
            }
        }
    }
}
