using GameCube.GFZ.Stage;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    /// <summary>
    /// 
    /// </summary>
    public class GfzStaticColliderMeshVisualizer : MonoBehaviour
    {
        [SerializeField] private StaticColliderMeshProperty property;
        [SerializeField] private MeshFilter colliderMesh;

        public StaticColliderMeshProperty Property
        {
            get => property;
            set => property = value;
        }

        public MeshFilter ColliderMesh
        {
            get => colliderMesh;
            set => colliderMesh = value;
        }

    }
}
