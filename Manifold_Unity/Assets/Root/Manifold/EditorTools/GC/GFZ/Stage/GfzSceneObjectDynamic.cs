using GameCube.GFZ.Stage;
using Manifold.EditorTools.Attributes;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public sealed class GfzSceneObjectDynamic : MonoBehaviour,
        IGfzConvertable<SceneObjectDynamic>
    {
        [field: SerializeField, Hex] public int Unk_0x00 { get; private set; }
        [field: SerializeField, Hex] public int Unk_0x04 { get; private set; }
        [field: SerializeField] public GfzSceneObject SceneObject { get; private set; }
        [field: SerializeField] public GfzAnimationClip AnimationClip { get; private set; }
        [field: SerializeField] public GfzTextureScroll TextureScroll { get; private set; }
        [field: SerializeField] public GfzSkeletalAnimator SkeletalAnimator { get; private set; }

        internal void SetSceneObject(GfzSceneObject sceneObject)
        {
            SceneObject = sceneObject;
        }

        public SceneObjectDynamic ExportGfz()
        {
            var dynamicSceneObject = new SceneObjectDynamic();

            // Data from this structure
            dynamicSceneObject.Unk0x00 = Unk_0x00;
            dynamicSceneObject.Unk0x04 = Unk_0x04;
            dynamicSceneObject.TransformTRXS = TransformConverter.ToGfzTransformTRXS(transform, Space.World);

            // Values from pointed classes
            // These functions should return null if necessary
            dynamicSceneObject.SceneObject = SceneObject.ExportGfz(); // todo, unmangle references in generator

            if (AnimationClip != null)
                dynamicSceneObject.AnimationClip = AnimationClip.ExportGfz();

            if (TextureScroll != null)
                dynamicSceneObject.TextureScroll = TextureScroll.ExportGfz();

            if (SkeletalAnimator != null)
                dynamicSceneObject.SkeletalAnimator = SkeletalAnimator.ExportGfz();

            // This value only exists if we don't have an animation
            if (dynamicSceneObject.AnimationClip == null)
                dynamicSceneObject.TransformMatrix3x4 = TransformConverter.ToGfzTransformMatrix3x4(transform, Space.World);

            return dynamicSceneObject;
        }

        public void ImportGfz(SceneObjectDynamic dynamicSceneObject)
        {
            // SCENE OBJECT DYNAMIC
            {
                Unk_0x00 = dynamicSceneObject.Unk0x00;
                Unk_0x04 = dynamicSceneObject.Unk0x04;

                // TRANSFORM
                // Copy most reliable transform if available
                if (dynamicSceneObject.TransformMatrix3x4 != null)
                {
                    transform.CopyGfzTransform(dynamicSceneObject.TransformMatrix3x4, Space.Self);
                }
                else
                {
                    transform.CopyGfzTransform(dynamicSceneObject.TransformTRXS, Space.Self);
                }
            }

            if (dynamicSceneObject.AnimationClip != null)
            {
                AnimationClip = this.gameObject.AddComponent<GfzAnimationClip>();
                AnimationClip.ImportGfz(dynamicSceneObject.AnimationClip);
            }

            if (dynamicSceneObject.TextureScroll != null)
            {
                TextureScroll = this.gameObject.AddComponent<GfzTextureScroll>();
                TextureScroll.ImportGfz(dynamicSceneObject.TextureScroll);
            }

            if (dynamicSceneObject.SkeletalAnimator != null)
            {
                SkeletalAnimator = this.gameObject.AddComponent<GfzSkeletalAnimator>();
                SkeletalAnimator.ImportGfz(dynamicSceneObject.SkeletalAnimator);
            }

            // Transform Matrix 3x4 is handled above and does not need a component
        }

    }
}
