using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    /// <summary>
    /// Visualize the collider mesh. True data resides on SceneObject (template).
    /// </summary>
    public class GfzColliderMeshDebugger : MonoBehaviour
    {
        [field: SerializeField]
        public GfzColliderMesh ColliderMesh { get; set; }

        public void OnDrawGizmosSelected()
        {
            if (ColliderMesh == null)
                return;

            ColliderMesh.OnDrawGizmosSelected();
            ColliderMesh.DrawMesh(transform);
        }

        private void Reset()
        {
            OnValidate();
        }
        private void OnValidate()
        {
            if (ColliderMesh == null)
                ColliderMesh = GetComponent<GfzColliderMesh>();
        }
    }
}
