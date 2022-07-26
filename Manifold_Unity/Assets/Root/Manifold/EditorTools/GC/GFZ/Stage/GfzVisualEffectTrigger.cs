using GameCube.GFZ.Stage;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public class GfzVisualEffectTrigger : MonoBehaviour,
        IGfzConvertable<VisualEffectTrigger>
    {
        /// <summary>
        /// Visual effect trigger scale (when compared to default Unity cube).
        /// </summary>
        public const float scale = 10f;

        [field: SerializeField] public new TriggerableAnimation Animation { get; private set; }
        [field: SerializeField] public TriggerableVisualEffect VisualEffect { get; private set; }


        public VisualEffectTrigger ExportGfz()
        {
            // Convert unity transform to gfz transform
            var transform = TransformConverter.ToGfzTransformTRXS(this.transform, Space.World);

            var value = new VisualEffectTrigger
            {
                Transform = transform,
                Animation = Animation,
                VisualEffect = VisualEffect,
            };

            return value;
        }

        public void ImportGfz(VisualEffectTrigger value)
        {
            transform.CopyGfzTransform(value.Transform, Space.Self);
            transform.localScale *= scale;
            Animation = value.Animation;
            VisualEffect = value.VisualEffect;
        }
    }
}
