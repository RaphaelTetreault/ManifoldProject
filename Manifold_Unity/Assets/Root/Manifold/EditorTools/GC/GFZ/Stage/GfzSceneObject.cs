using GameCube.GFZ.Stage;
using System;
using UnityEditor;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public class GfzSceneObject : MonoBehaviour,
        IGfzConvertable<SceneObject>,
        IEquatable<GfzSceneObject>
    {
        [SerializeField] private LodRenderFlags lodRenderFlags;
        [SerializeField] private GfzSceneObjectLODs sceneObjectLODs;
        [SerializeField] private GfzColliderMesh colliderMesh;

        public GfzSceneObjectLODs GfzSceneObjectLODs => sceneObjectLODs;
        public GfzColliderMesh ColliderMesh => colliderMesh;


        // This stuff is so that each call to Export gives the same reference
        private SceneObject SceneObjectReference { get; set; }
        public void InitSharedReference()
        {
            SceneObjectReference = new SceneObject();

            SceneObjectReference.LodRenderFlags = lodRenderFlags;
            SceneObjectReference.LODs = sceneObjectLODs.ExportGfz();

            if (colliderMesh != null)
                SceneObjectReference.ColliderMesh = colliderMesh.ExportGfz();
        }
        public SceneObject ExportGfz()
        {
            // If null, you have not init the shared reference value
            return SceneObjectReference;
        }

        public void ImportGfz(SceneObject sceneObject)
        {
            lodRenderFlags = sceneObject.LodRenderFlags;

            sceneObjectLODs = this.gameObject.AddComponent<GfzSceneObjectLODs>();
            sceneObjectLODs.ImportGfz(sceneObject.LODs);

            if (sceneObject.ColliderMesh != null)
            {
                colliderMesh = this.gameObject.AddComponent<GfzColliderMesh>();
                colliderMesh.ImportGfz(sceneObject.ColliderMesh);
                // NOTE: let dynamics assign mesh... convoluted otherwise
            }
        }

        public bool Equals(GfzSceneObject other)
        {
            var hasFlags = other.lodRenderFlags == lodRenderFlags;
            var hasLODs = other.sceneObjectLODs.Equals(sceneObjectLODs);
            //var hasCollider = other.colliderMesh.Equals(colliderMesh);

            return hasFlags && hasLODs; //&& hasCollider;
        }

        public void TryAssignColliderMesh(Mesh mesh)
        {
            // If no collider mesh or already assigned, skip
            if (colliderMesh == null)
                return;

            colliderMesh.ColliderMesh= mesh;
        }

        private void Reset()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            if (colliderMesh == null)
                colliderMesh = GetComponent<GfzColliderMesh>();
        }
    }
}
