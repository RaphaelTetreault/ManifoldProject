using GameCube.GFZ.Stage;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    /// <summary>
    /// 
    /// </summary>
    public class GfzVisualEffectTrigger : MonoBehaviour,
        IGfzConvertable<VisualEffectTrigger>
    {
        /// <summary>
        /// Visual effect trigger scale (when compared to default Unity cube).
        /// </summary>
        public const float scale = 10f;

        // INSPECTOR FIELDS
        [SerializeField] private new TriggerableAnimation animation;
        [SerializeField] private TriggerableVisualEffect visualEffect;

        // PROPERTIES
        public TriggerableAnimation Animation
        {
            get => animation;
            set => animation = value;
        }
        public TriggerableVisualEffect VisualEffect
        {
            get => visualEffect;
            set => visualEffect = value;
        }

        // METHODS
        public VisualEffectTrigger ExportGfz()
        {
            // Convert unity transform to gfz transform
            var transform = TransformConverter.ToGfzTransformTRXS(this.transform, Space.World);

            var value = new VisualEffectTrigger
            {
                Transform = transform,
                Animation = animation,
                VisualEffect = visualEffect,
            };

            return value;
        }

        public void ImportGfz(VisualEffectTrigger value)
        {
            transform.CopyTransform(value.Transform);
            transform.localScale *= scale;
            animation = value.Animation;
            visualEffect = value.VisualEffect;
        }
    }
}
