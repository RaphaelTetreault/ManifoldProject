using GameCube.GFZ.CourseCollision;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.CourseCollision
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
