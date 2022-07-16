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
            var scale = transform.localScale;
            transform.localScale = new Vector3(scale.x, scale.y, -scale.z);
        }

    }
}
