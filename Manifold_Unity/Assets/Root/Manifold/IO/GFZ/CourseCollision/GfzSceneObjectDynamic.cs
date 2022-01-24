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

        public SceneObjectDynamic ExportGfz()
        {
            var value = new SceneObjectDynamic();

            // Data from this structure
            value.unk0x00 = unk_0x00;
            value.unk0x04 = unk_0x04;
            value.transformPRXS = TransformConverter.ToGfzTransformPRXS(transform);

            // Values from pointed classes
            // These functions should return null if necessary
            value.sceneObject = sceneObject.ExportGfz(); // todo, unmangle references in generator
            value.animationClip = animationClip.ExportGfz();
            value.textureScroll = textureScroll.ExportGfz();
            value.skeletalAnimator = skeletalAnimator.ExportGfz();
            // This value only exists if we don't have an animation
            if (animationClip == null)
            {
                value.transformMatrix3x4 = TransformConverter.ToGfzTransformMatrix3x4(transform);
            }

            return value;
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
