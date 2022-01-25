using GameCube.GFZ.CourseCollision;
using UnityEngine;

namespace Manifold.IO.GFZ.CourseCollision
{
    internal class GfzSceneObject : MonoBehaviour,
        IGfzConvertable<SceneObject>
    {
        [SerializeField] private LodRenderFlags lodRenderFlags;
        //[SerializeField] private MeshFilter colliderMesh; // <-- Would be ideal sometime if this were how it's done
        [SerializeField] private bool exportColliderMesh;
        [SerializeField] private ColliderGeometry srcColliderMesh;

        [SerializeField] private GfzSceneObjectLODs sceneObjectLODs;


        public SceneObject ExportGfz()
        {
            var sceneObject = new SceneObject();

            sceneObject.lodRenderFlags = lodRenderFlags;
            sceneObject.lods = sceneObjectLODs.ExportGfz();

            if (exportColliderMesh)
            {
                sceneObject.colliderGeometry = srcColliderMesh;
            }

            return sceneObject;
        }

        public void ImportGfz(SceneObject sceneObject)
        {
            lodRenderFlags = sceneObject.lodRenderFlags;

            sceneObjectLODs = this.gameObject.AddComponent<GfzSceneObjectLODs>();
            sceneObjectLODs.ImportGfz(sceneObject.lods);

            bool hasColliderGeometry = sceneObject.colliderGeometry != null;
            if (hasColliderGeometry)
            {
                srcColliderMesh = sceneObject.colliderGeometry;
            }
            exportColliderMesh = hasColliderGeometry;

        }
    }
}
