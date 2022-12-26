using GameCube.GFZ.Stage;
using Manifold.EditorTools.Attributes;
using UnityEngine;
using static Manifold.EditorTools.GC.GFZ.Stage.GfzSceneObjectLODs;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public sealed class GfzSceneObjectDynamic : MonoBehaviour,
        IGfzConvertable<SceneObjectDynamic>
    {
        [Header("Dynamic Data")]
        [SerializeField] private ObjectRenderFlags0x00 unk_0x00;
        [SerializeField] private ObjectRenderFlags0x04 unk_0x04;
        [SerializeField] private GfzSceneObject sceneObject;
        [SerializeField] private GfzAnimationClip animationClip;
        [SerializeField] private GfzTextureScroll textureScroll;
        [SerializeField] private GfzSkeletalAnimator skeletalAnimator;

        public GfzSceneObject GfzSceneObject => sceneObject;
        public GfzAnimationClip AnimationClip => animationClip;
        public GfzTextureScroll TextureScroll => textureScroll;
        public GfzSkeletalAnimator SkeletalAnimator => skeletalAnimator;


        internal void SetSceneObject(GfzSceneObject sceneObject)
        {
            this.sceneObject = sceneObject;
        }

        public SceneObjectDynamic ExportGfz()
        {
            var dynamicSceneObject = new SceneObjectDynamic();

            // Data from this structure
            dynamicSceneObject.Unk0x00 = unk_0x00;
            dynamicSceneObject.Unk0x04 = unk_0x04;
            dynamicSceneObject.TransformTRXS = TransformConverter.ToGfzTransformTRXS(transform, Space.World);

            // Values from pointed classes
            // These functions should return null if necessary
            dynamicSceneObject.SceneObject = sceneObject.ExportGfz();
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

            // Transform Matrix 3x4 is handled above and does not need a component
        }

        public void OnDrawGizmosSelected()
        {
            if (sceneObject != null && sceneObject.ColliderMesh != null)
            {
                sceneObject.ColliderMesh.OnDrawGizmosSelected();
                sceneObject.ColliderMesh.DrawMesh(transform);
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
