using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    /// <summary>
    /// A script attached to objects with transforms that need to be inverted along
    /// the X axis when being exported to a GFZ scene.
    /// </summary>
    public sealed class GfzMirroredObject : MonoBehaviour
    {
        public void MirrorTransform()
        {
            var s = transform.localScale;
            transform.localScale = new Vector3(-s.x, s.y, s.z);
        }

    }
}
