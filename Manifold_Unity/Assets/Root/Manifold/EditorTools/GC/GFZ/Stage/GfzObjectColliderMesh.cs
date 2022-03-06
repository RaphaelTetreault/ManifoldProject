using GameCube.GFZ.Stage;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    /// <summary>
    /// 
    /// </summary>
    public class GfzObjectColliderMesh : MonoBehaviour
    {
        [SerializeField] private MeshFilter colliderMesh;

        public MeshFilter ColliderMesh
        {
            get => colliderMesh;
            set => colliderMesh = value;
        }

    }
}
