using GameCube.GFZ.Stage;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    /// <summary>
    /// 
    /// </summary>
    public class GfzTimeExtensionTrigger : MonoBehaviour,
        IGfzConvertable<TimeExtensionTrigger>
    {
        /// <summary>
        /// Arcade checkpoint trigger scale (when compared to default Unity cube).
        /// </summary>
        public const float scale = 10f;

        // INSPECTOR FIELDS
        [SerializeField] private TimeExtensionOption type;

        // PROPERTIES
        public TimeExtensionOption Type
        {
            get => type;
            set => type = value;
        }

        // METHODS
        public TimeExtensionTrigger ExportGfz()
        {
            // Convert unity transform to gfz transform
            var transform = TransformConverter.ToGfzTransformTRXS(this.transform);
            transform.Scale /= scale;

            var value = new TimeExtensionTrigger
            {
                Transform = transform,
                Option = type,
            };

            return value;
        }

        public void ImportGfz(TimeExtensionTrigger value)
        {
            transform.CopyGfzTransformTRXS(value.Transform);
            transform.localScale *= scale;
            type = value.Option;
        }
    }
}
