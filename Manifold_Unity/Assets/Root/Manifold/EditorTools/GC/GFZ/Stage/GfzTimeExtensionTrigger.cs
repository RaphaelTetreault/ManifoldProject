using GameCube.GFZ.Stage;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public class GfzTimeExtensionTrigger : MonoBehaviour,
        IGfzConvertable<TimeExtensionTrigger>
    {
        /// <summary>
        /// Arcade checkpoint trigger scale (when compared to default Unity cube).
        /// </summary>
        public const float scale = 10f;

        [field: SerializeField] public TimeExtensionOption Type { get; private set; }


        // METHODS
        public TimeExtensionTrigger ExportGfz()
        {
            // Convert unity transform to gfz transform
            var transform = TransformConverter.ToGfzTransformTRXS(this.transform, Space.World);
            transform.Scale /= scale;

            var value = new TimeExtensionTrigger
            {
                Transform = transform,
                Option = Type,
            };

            return value;
        }

        public void ImportGfz(TimeExtensionTrigger value)
        {
            transform.CopyGfzTransform(value.Transform, Space.Self);
            transform.localScale *= scale;
            Type = value.Option;
        }
    }
}
