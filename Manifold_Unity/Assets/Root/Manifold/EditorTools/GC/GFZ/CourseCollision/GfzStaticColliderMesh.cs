using GameCube.GFZ.CourseCollision;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.CourseCollision
{
    /// <summary>
    /// 
    /// </summary>
    public class GfzStaticColliderMesh : MonoBehaviour
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
