using GameCube.GFZ.CourseCollision;
using System;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.CourseCollision
{
    public class GfzColliderMesh : MonoBehaviour,
        IGfzConvertable<ColliderMesh>
        //IEquatable<GfzColliderMesh>
    {
        //[SerializeField] private MeshFilter colliderMesh; // <-- Would be ideal sometime if this were how it's done
        [SerializeField] private bool exportColliderMesh;
        [SerializeField] private ColliderMesh srcColliderMesh;

        public ColliderMesh ExportGfz()
        {
            if (exportColliderMesh)
            {
                return srcColliderMesh;
            }
            else
            {
                return null;
            }
        }

        public void ImportGfz(ColliderMesh colliderMesh)
        {
            bool hasColliderMesh = colliderMesh != null;
            if (hasColliderMesh)
            {
                srcColliderMesh = colliderMesh;
            }
            exportColliderMesh = hasColliderMesh;
        }

        //public bool Equals(GfzColliderMesh other)
        //{
        //    return other.srcColliderMesh == srcColliderMesh;
        //}
    }

}
