using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.CourseCollision
{
    public class GfzMirroredObject : MonoBehaviour
    {
        public void MirrorTransformZ()
        {
            var s = transform.localScale;
            transform.localScale = new Vector3(s.x, s.y, -s.z);
        }

    }
}
