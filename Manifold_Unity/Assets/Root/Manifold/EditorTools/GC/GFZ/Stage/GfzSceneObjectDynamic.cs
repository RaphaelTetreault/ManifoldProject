using GameCube.GFZ.Stage;
using Manifold.EditorTools.Attributes;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public sealed class GfzSceneObjectDynamic : MonoBehaviour,
        IGfzConvertable<SceneObjectDynamic>
    {
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
            dynamicSceneObject.TransformTRXS = TransformConverter.ToGfzTransformTRXS(transform);

            // Values from pointed classes
            // These functions should return null if necessary
            dynamicSceneObject.SceneObject = sceneObject.ExportGfz(); // todo, unmangle references in generator

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
                    transform.CopyGfzTransformMatrix3x4(dynamicSceneObject.TransformMatrix3x4);
                }
                else
                {
                    transform.CopyGfzTransformTRXS(dynamicSceneObject.TransformTRXS);
                }
            }

            // Add scripts and import data
            //sceneObject = this.gameObject.AddComponent<GfzSceneObject>();
            //sceneObject.ImportGfz(dynamicSceneObject.sceneObject);

            if (dynamicSceneObject.AnimationClip != null)
            {
                animationClip = this.gameObject.AddComponent<GfzAnimationClip>();
                animationClip.ImportGfz(dynamicSceneObject.AnimationClip);
            }

            if (dynamicSceneObject.TextureScroll != null)
            {
                //Debug.Log($"{nameof(TextureScroll)}");
                //Debug.Log($"{name} has {nameof(TextureScroll)}");
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

    }
}
