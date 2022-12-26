using GameCube.GFZ.Stage;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Manifold.EditorTools.GC.GFZ.Stage.GfzSceneObjectLODs;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public sealed class GfzCompleteSceneObject : MonoBehaviour,
        //IGfzConvertable<SceneObjectDynamic>
        //IGfzConvertable<SceneObjectDynamic>,
        //IGfzConvertable<SceneObject>,
        //IGfzConvertable<SceneObjectLOD[]>
    {
        [Header("Dynamic Data")]
        [SerializeField] private ObjectRenderFlags0x00 unk_0x00;
        [SerializeField] private ObjectRenderFlags0x04 unk_0x04;
        [SerializeField] private GfzAnimationClip animationClip;
        [SerializeField] private GfzTextureScroll textureScroll;
        [SerializeField] private GfzSkeletalAnimator skeletalAnimator;
        [Header("SceneObject Data")]
        [SerializeField] private LodRenderFlags lodRenderFlags;
        [SerializeField] private GfzLOD[] levelsOfDetail;
        [SerializeField] private GfzColliderMesh colliderMesh;
        [Header("Metadata")]
        [SerializeField, ReadOnlyGUI] private string assetSource;

        public GfzAnimationClip AnimationClip => animationClip;
        public GfzTextureScroll TextureScroll => textureScroll;
        public GfzSkeletalAnimator SkeletalAnimator => skeletalAnimator;
        public GfzColliderMesh ColliderMesh => colliderMesh;

        public LodRenderFlags LodRenderFlags => lodRenderFlags;
        public GfzLOD[] LODs => levelsOfDetail;


        public static Dictionary<GfzCompleteSceneObject, SceneObject> SceneObjectsDict => new Dictionary<GfzCompleteSceneObject, SceneObject>();
        public static SceneObject[] GetSceneObjects => SceneObjectsDict.Values.ToArray();
        public static void InitSharedSceneObjects()
        {
            SceneObjectsDict.Clear();
        }
        public static SceneObject GetSharedSceneObjectReference(GfzCompleteSceneObject unityObject)
        {
            foreach (var kvp in SceneObjectsDict)
            {
                var key = kvp.Key;
                bool isEquivilent = unityObject.IsSceneObjectReferenceEquivilent(key);
                if (isEquivilent)
                    return kvp.Value;
            }

            // Not in list. Make one and return that.
            SceneObject sceneObject = unityObject.ExportGfz_SceneObject();
            SceneObjectsDict.Add(unityObject, sceneObject);
            return sceneObject;
        }

        public SceneObjectDynamic ExportGfz_SceneObjectDynamic()
        {
            var dynamicSceneObject = new SceneObjectDynamic();

            // Data from this structure
            dynamicSceneObject.Unk0x00 = unk_0x00;
            dynamicSceneObject.Unk0x04 = unk_0x04;
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
                sceneObject.ColliderMesh = colliderMesh.ExportGfz();

            return sceneObject;
        }

        public void ImportGfz(SceneObjectDynamic dynamicSceneObject)
        {
            // SCENE OBJECT DYNAMIC
            {
                unk_0x00 = dynamicSceneObject.Unk0x00;
                unk_0x04 = dynamicSceneObject.Unk0x04;

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


            //
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
                // NOTE: let dynamics assign mesh... convoluted otherwise
            }
        }

        public bool IsSceneObjectReferenceEquivilent(GfzCompleteSceneObject other)
        {
            bool sameFlags = this.LodRenderFlags == other.LodRenderFlags;
            bool sameLODs = HasSameLODs(other.levelsOfDetail);
            bool sameCollider = ColliderMesh.IsReferenceEquivilent(other.ColliderMesh);
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


        public void OnDrawGizmosSelected()
        {
            if (ColliderMesh != null)
            {
                ColliderMesh.OnDrawGizmosSelected();
                ColliderMesh.DrawMesh(transform);
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
        }

    }
}
