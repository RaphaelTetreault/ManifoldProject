using GameCube.GFZ.CourseCollision;
using System;
using UnityEngine;

namespace Manifold.IO.GFZ.CourseCollision
{
    public class GfzSceneObject : MonoBehaviour,
        IGfzConvertable<SceneObject>,
        IEquatable<GfzSceneObject>
    {
        [SerializeField] private LodRenderFlags lodRenderFlags;
        [SerializeField] private GfzSceneObjectLODs sceneObjectLODs;
        [SerializeField] private GfzColliderMesh colliderMesh;

        public GfzSceneObjectLODs SceneObjectLODs => sceneObjectLODs;
        public GfzColliderMesh ColliderMesh => colliderMesh;

        public SceneObject ExportGfz()
        {
            var sceneObject = new SceneObject();

            sceneObject.lodRenderFlags = lodRenderFlags;
            sceneObject.lods = sceneObjectLODs.ExportGfz();
            if (colliderMesh != null)
                sceneObject.colliderMesh = colliderMesh.ExportGfz();

            return sceneObject;
        }

        public void ImportGfz(SceneObject sceneObject)
        {
            lodRenderFlags = sceneObject.lodRenderFlags;

            sceneObjectLODs = this.gameObject.AddComponent<GfzSceneObjectLODs>();
            sceneObjectLODs.ImportGfz(sceneObject.lods);

            colliderMesh = this.gameObject.AddComponent<GfzColliderMesh>();
            colliderMesh.ImportGfz(sceneObject.colliderMesh);
        }

        public bool Equals(GfzSceneObject other)
        {
            var hasFlags    = other.lodRenderFlags == lodRenderFlags;
            var hasLODs     = other.sceneObjectLODs.Equals(sceneObjectLODs);
            //var hasCollider = other.colliderMesh.Equals(colliderMesh);

            return hasFlags && hasLODs; //&& hasCollider;
        }
    }
}
