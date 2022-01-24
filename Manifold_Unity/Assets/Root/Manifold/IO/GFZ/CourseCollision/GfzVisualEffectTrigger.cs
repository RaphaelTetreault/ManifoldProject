using GameCube.GFZ.CourseCollision;
using UnityEngine;

namespace Manifold.IO.GFZ.CourseCollision
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
            var transform = TransformConverter.ToGfzTransformPRXS(this.transform);

            var value = new VisualEffectTrigger
            {
                transform = transform,
                animation = animation,
                visualEffect = visualEffect,
            };

            return value;
        }

        public void ImportGfz(VisualEffectTrigger value)
        {
            transform.CopyGfzTransformPRXS(value.transform);
            transform.localScale *= scale;
            animation = value.animation;
            visualEffect = value.visualEffect;
        }
    }
}
