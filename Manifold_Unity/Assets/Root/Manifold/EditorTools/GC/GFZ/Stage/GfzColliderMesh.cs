using GameCube.GFZ.Stage;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public class GfzColliderMesh : MonoBehaviour,
        IGfzConvertable<ColliderMesh>
    {
        //[SerializeField] private MeshFilter colliderMesh; // <-- Would be ideal sometime if this were how it's done
        [field: SerializeField] public bool ExportColliderMesh { get; private set; }
        [field: SerializeField] public ColliderMesh SrcColliderMesh { get; private set; }

        public ColliderMesh ExportGfz()
        {
            if (ExportColliderMesh)
            {
                return SrcColliderMesh;
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
                SrcColliderMesh = colliderMesh;
            }
            ExportColliderMesh = hasColliderMesh;
        }
    }

}
