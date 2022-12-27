using GameCube.GFZ.Stage;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Manifold.EditorTools.GC.GFZ.Stage.GfzSceneObjectLODs;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public sealed class GfzCompleteSceneObject : MonoBehaviour
    {
        [SerializeField] private ObjectRenderFlags0x00 objectRenderFlags_0x00;
        [SerializeField] private ObjectRenderFlags0x04 objectRenderFlags_0x04;
        [SerializeField] private LodRenderFlags lodRenderFlags;
        [SerializeField] private GfzLOD[] levelsOfDetail;

        [Header("Optional Components")]
        [SerializeField] private GfzAnimationClip animationClip;
        [SerializeField] private GfzTextureScroll textureScroll;
        [SerializeField] private GfzSkeletalAnimator skeletalAnimator;
        [SerializeField] private GfzColliderMesh colliderMesh;

        [Header("Debug")]
        [SerializeField] private bool previewLodRadius = false;


        public GfzAnimationClip AnimationClip => animationClip;
        public GfzTextureScroll TextureScroll => textureScroll;
        public GfzSkeletalAnimator SkeletalAnimator => skeletalAnimator;
        public GfzColliderMesh ColliderMesh => colliderMesh;
        public LodRenderFlags LodRenderFlags => lodRenderFlags;
        public GfzLOD[] LODs => levelsOfDetail;

        #region MANAGE EXPORT
        public static readonly Dictionary<GfzCompleteSceneObject, SceneObject> SceneObjectsDict = new Dictionary<GfzCompleteSceneObject, SceneObject>();
        public static readonly Dictionary<GfzColliderMesh, ColliderMesh> ColliderMeshesDict = new Dictionary<GfzColliderMesh, ColliderMesh>();
        public static SceneObject[] GetAllSharedSceneObjects => SceneObjectsDict.Values.ToArray();
        public static void InitSharedSceneObjects()
        {
            SceneObjectsDict.Clear();
            ColliderMeshesDict.Clear();
        }
        public static SceneObject GetSharedSceneObjectReference(GfzCompleteSceneObject completeSceneObject)
        {
            foreach (var sceneObjectKVP in SceneObjectsDict)
            {
                var key = sceneObjectKVP.Key;
                bool isEquivilent = completeSceneObject.IsSceneObjectReferenceEquivilent(key);
                if (isEquivilent)
                    return sceneObjectKVP.Value;
            }

            // Not in list. Make one and return that.
            SceneObject sceneObject = completeSceneObject.ExportGfz_SceneObject();
            SceneObjectsDict.Add(completeSceneObject, sceneObject);
            return sceneObject;
        }
        public static ColliderMesh GetSharedColliderMeshReference(GfzColliderMesh gfzColliderMesh)
        {
            foreach (var colliderMeshesKVP in ColliderMeshesDict)
            {
                var key = colliderMeshesKVP.Key;
                bool isEquivilent = gfzColliderMesh.IsReferenceEquivilent(key);
                if (isEquivilent)
                    return colliderMeshesKVP.Value;
            }

            // Not in list. Make one and return that.
            var colliderMesh = gfzColliderMesh.ExportGfz();
            ColliderMeshesDict.Add(gfzColliderMesh, colliderMesh);
            return colliderMesh;
        }
        #endregion

        public SceneObjectDynamic ExportGfz_SceneObjectDynamic()
        {
            var dynamicSceneObject = new SceneObjectDynamic();

            // Data from this structure
            dynamicSceneObject.ObjectRenderFlags0x00 = objectRenderFlags_0x00;
            dynamicSceneObject.ObjectRenderFlags0x04 = objectRenderFlags_0x04;
            dynamicSceneObject.TransformTRXS = TransformConverter.ToGfzTransformTRXS(transform, Space.World);

            // Values from pointed classes
            // These functions should return null if necessary
            dynamicSceneObject.SceneObject = GetSharedSceneObjectReference(this);
            if (dynamicSceneObject.SceneObject is null)
                throw new System.ArgumentException(name + " SceneObjectDynamic's SceneObject reference is null.");

            if (animationClip != null)
                dynamicSceneObject.AnimationClip = animationClip.ExportGfz();

            if (textureScroll != null)
                dynamicSceneObject.TextureScroll = textureScroll.ExportGfz();

            if (skeletalAnimator != null)
                dynamicSceneObject.SkeletalAnimator = skeletalAnimator.ExportGfz();

            // This value only exists if we don't have an animation
            if (dynamicSceneObject.AnimationClip == null)
                dynamicSceneObject.TransformMatrix3x4 = TransformConverter.ToGfzTransformMatrix3x4(transform, Space.World);

            return dynamicSceneObject;
        }
        public SceneObject ExportGfz_SceneObject()
        {
            // CREATE LODs in proprietary format
            var lods = new SceneObjectLOD[levelsOfDetail.Length];
            for (int i = 0; i < lods.Length; i++)
                lods[i] = new SceneObjectLOD()
                {
                    Name = levelsOfDetail[i].modelName,
                    LodDistance = levelsOfDetail[i].lodDistance,
                };

            // Create the scene object
            var sceneObject = new SceneObject()
            {
                LodRenderFlags = lodRenderFlags,
                LODs = lods,
            };

            if (colliderMesh != null)
            {
                var nativeColliderMesh = GetSharedColliderMeshReference(colliderMesh);
                sceneObject.ColliderMesh = nativeColliderMesh;
            }

            return sceneObject;
        }

        public void ImportGfz_DynamicSceneObject(SceneObjectDynamic dynamicSceneObject)
        {
            // SCENE OBJECT DYNAMIC
            {
                objectRenderFlags_0x00 = dynamicSceneObject.ObjectRenderFlags0x00;
                objectRenderFlags_0x04 = dynamicSceneObject.ObjectRenderFlags0x04;

                // TRANSFORM
                // Copy most reliable transform if available
                if (dynamicSceneObject.TransformMatrix3x4 != null)
                {
                    transform.CopyTransform(dynamicSceneObject.TransformMatrix3x4);
                }
                else
                {
                    transform.CopyTransform(dynamicSceneObject.TransformTRXS);
                }
            }

            // Add scripts and import data
            if (dynamicSceneObject.AnimationClip != null)
            {
                animationClip = this.gameObject.AddComponent<GfzAnimationClip>();
                animationClip.ImportGfz(dynamicSceneObject.AnimationClip);
            }

            if (dynamicSceneObject.TextureScroll != null)
            {
                textureScroll = this.gameObject.AddComponent<GfzTextureScroll>();
                textureScroll.ImportGfz(dynamicSceneObject.TextureScroll);
            }

            if (dynamicSceneObject.SkeletalAnimator != null)
            {
                skeletalAnimator = this.gameObject.AddComponent<GfzSkeletalAnimator>();
                skeletalAnimator.ImportGfz(dynamicSceneObject.SkeletalAnimator);
            }

            // Transform Matrix 3x4 is handled above and does not need a component'


            // Copy over 'SceneObject' data
            var sceneObject = dynamicSceneObject.SceneObject;
            var sceneObjectLODs = sceneObject.LODs;
            IO.Assert.IsTrue(sceneObjectLODs.Length > 0);

            // Assign LODs
            lodRenderFlags = sceneObject.LodRenderFlags;
            levelsOfDetail = new GfzLOD[sceneObjectLODs.Length];
            for (int i = 0; i < levelsOfDetail.Length; i++)
            {
                var sceneObjectLOD = sceneObjectLODs[i];
                levelsOfDetail[i] = new GfzLOD()
                {
                    modelName = sceneObjectLOD.Name,
                    lodDistance = sceneObjectLOD.LodDistance,
                };
            }

            // COLLIDER MESH
            if (sceneObject.ColliderMesh != null)
            {
                colliderMesh = this.gameObject.AddComponent<GfzColliderMesh>();
                colliderMesh.ImportGfz(sceneObject.ColliderMesh);
                // NOTE: mesh is assigned by importing script
            }
        }

        public bool IsSceneObjectReferenceEquivilent(GfzCompleteSceneObject other)
        {
            bool sameFlags = this.LodRenderFlags == other.LodRenderFlags;
            bool sameLODs = HasSameLODs(other.levelsOfDetail);
            bool sameCollider = HasSameColliderMesh(other.ColliderMesh);
            bool isSame = sameFlags && sameLODs && sameCollider;
            return isSame;
        }
        public bool HasSameLODs(GfzLOD[] otherLODs)
        {
            bool sameLODsLength = this.LODs.Length == otherLODs.Length;
            if (!sameLODsLength)
                return false;

            for (int i = 0; i < levelsOfDetail.Length; i++)
            {
                var thisLOD = levelsOfDetail[i];
                var otherLOD = otherLODs[i];

                bool sameName = thisLOD.modelName == otherLOD.modelName;
                bool sameDistance = thisLOD.lodDistance == otherLOD.lodDistance;
                bool isSame = sameName & sameDistance;
                if (!isSame)
                    return false;
            }
            // else is true
            return true;
        }
        public bool HasSameColliderMesh(GfzColliderMesh other)
        {
            // Weeds out when one or the other is null
            bool isNotSame = this.ColliderMesh == null ^ other == null;
            if (isNotSame)
                return false;

            // Case 1: both are null. Check for null, if so, both are null, means they are the same.
            if (this.ColliderMesh == null)
                return true;

            // Case 2: both are instances. Compare instances.
            bool isEquivilent = ColliderMesh.IsReferenceEquivilent(other);
            return isEquivilent;
        }


        public void OnDrawGizmosSelected()
        {
            if (ColliderMesh != null)
            {
                //ColliderMesh.OnDrawGizmosSelected();
                ColliderMesh.DrawMesh(transform);
            }

            if (previewLodRadius)
            {
                Gizmos.color = new Color32(255, 0, 0, 127);
                float radius = 0f;
                for (int i = 0; i < LODs.Length; i++)
                {
                    var lod = LODs[i];
                    radius += lod.lodDistance * 10;
                    Gizmos.DrawWireSphere(transform.position, radius);
                }
                Gizmos.DrawSphere(transform.position, radius);
            }
        }
        private void Reset()
        {
            OnValidate();
        }
        private void OnValidate()
        {
            if (animationClip == null)
                animationClip = GetComponent<GfzAnimationClip>();

            if (textureScroll == null)
                textureScroll = GetComponent<GfzTextureScroll>();

            if (skeletalAnimator == null)
                skeletalAnimator = GetComponent<GfzSkeletalAnimator>();

            if (colliderMesh == null)
                colliderMesh = GetComponent<GfzColliderMesh>();
        }

    }
}
