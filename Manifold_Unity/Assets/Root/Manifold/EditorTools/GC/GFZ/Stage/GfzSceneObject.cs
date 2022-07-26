using GameCube.GFZ.Stage;
using System;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public class GfzSceneObject : MonoBehaviour,
        IGfzConvertable<SceneObject>,
        IEquatable<GfzSceneObject>
    {
        [field: SerializeField] public LodRenderFlags LodRenderFlags { get; private set; }
        [field: SerializeField] public GfzSceneObjectLODs SceneObjectLODs { get; private set; }
        [field: SerializeField] public GfzColliderMesh ColliderMesh { get; private set; }

        // This stuff is so that each call to Export gives the same reference
        private SceneObject SceneObjectReference { get; set; }

        public void InitSharableReference()
        {
            SceneObjectReference = new SceneObject();

            SceneObjectReference.LodRenderFlags = LodRenderFlags;
            SceneObjectReference.LODs = SceneObjectLODs.ExportGfz();

            if (ColliderMesh != null)
                SceneObjectReference.ColliderMesh = ColliderMesh.ExportGfz();
        }
        public SceneObject ExportGfz()
        {
            // If null, you have not init the shared reference value
            if (SceneObjectReference == null)
                throw new Exception($"{nameof(SceneObject)} value not initialized!");

            return SceneObjectReference;
        }

        public void ImportGfz(SceneObject sceneObject)
        {
            LodRenderFlags = sceneObject.LodRenderFlags;

            SceneObjectLODs = this.gameObject.AddComponent<GfzSceneObjectLODs>();
            SceneObjectLODs.ImportGfz(sceneObject.LODs);

            if (sceneObject.ColliderMesh != null)
            {
                ColliderMesh = this.gameObject.AddComponent<GfzColliderMesh>();
                ColliderMesh.ImportGfz(sceneObject.ColliderMesh);
            }
        }

        public bool Equals(GfzSceneObject other)
        {
            bool hasFlags = other.LodRenderFlags == LodRenderFlags;
            bool hasLODs = other.SceneObjectLODs.Equals(SceneObjectLODs);
            bool isEqual = hasFlags && hasLODs;
            return isEqual;
        }
    }
}
