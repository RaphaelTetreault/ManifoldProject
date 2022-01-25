using GameCube.GFZ.CourseCollision;
using UnityEngine;

namespace Manifold.IO.GFZ.CourseCollision
{
    public sealed class GfzSceneObjectDynamic : MonoBehaviour,
        IGfzConvertable<SceneObjectDynamic>
    {
        [SerializeField] [Hex] private int unk_0x00;
        [SerializeField] [Hex] private int unk_0x04;
        [SerializeField] private GfzSceneObject sceneObject;
        [SerializeField] private GfzAnimationClip animationClip;
        [SerializeField] private GfzTextureScroll textureScroll;
        [SerializeField] private GfzSkeletalAnimator skeletalAnimator;

        public GfzSceneObject SceneObject => sceneObject;
        public GfzAnimationClip AnimationClip => animationClip;
        public GfzTextureScroll TextureScroll => textureScroll; 
        public GfzSkeletalAnimator SkeletalAnimator => skeletalAnimator;



        public SceneObjectDynamic ExportGfz()
        {
            var dynamicSceneObject = new SceneObjectDynamic();

            // Data from this structure
            dynamicSceneObject.unk0x00 = unk_0x00;
            dynamicSceneObject.unk0x04 = unk_0x04;
            dynamicSceneObject.transformPRXS = TransformConverter.ToGfzTransformPRXS(transform);

            // Values from pointed classes
            // These functions should return null if necessary
            dynamicSceneObject.sceneObject = sceneObject.ExportGfz(); // todo, unmangle references in generator
            dynamicSceneObject.animationClip = animationClip.ExportGfz();
            dynamicSceneObject.textureScroll = textureScroll.ExportGfz();
            dynamicSceneObject.skeletalAnimator = skeletalAnimator.ExportGfz();
            // This value only exists if we don't have an animation
            if (dynamicSceneObject.animationClip == null)
            {
                dynamicSceneObject.transformMatrix3x4 = TransformConverter.ToGfzTransformMatrix3x4(transform);
            }

            return dynamicSceneObject;
        }

        public void ImportGfz(SceneObjectDynamic dynamicSceneObject)
        {
            // SCENE OBJECT DYNAMIC
            {
                unk_0x00 = dynamicSceneObject.unk0x00;
                unk_0x04 = dynamicSceneObject.unk0x04;

                // TRANSFORM
                // Copy most reliable transform if available
                if (dynamicSceneObject.transformMatrix3x4 != null)
                {
                    transform.CopyGfzTransformMatrix3x4(dynamicSceneObject.transformMatrix3x4);
                }
                else
                {
                    transform.CopyGfzTransformPRXS(dynamicSceneObject.transformPRXS);
                }
            }

            // Add scripts and import data
            sceneObject = this.gameObject.AddComponent<GfzSceneObject>();
            sceneObject.ImportGfz(dynamicSceneObject.sceneObject);

            animationClip = this.gameObject.AddComponent<GfzAnimationClip>();
            animationClip.ImportGfz(dynamicSceneObject.animationClip);

            textureScroll = this.gameObject.AddComponent<GfzTextureScroll>();
            textureScroll.ImportGfz(dynamicSceneObject.textureScroll);

            skeletalAnimator = this.gameObject.AddComponent<GfzSkeletalAnimator>();
            skeletalAnimator.ImportGfz(dynamicSceneObject.skeletalAnimator);

            // Transform Matrix 3x4 is handled above and does not need a component
        }

    }
}
