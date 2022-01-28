using GameCube.GFZ.CourseCollision;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.CourseCollision
{
    /// <summary>
    /// 
    /// </summary>
    public class GfzArcadeCheckpoint : MonoBehaviour,
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
            var transform = TransformConverter.ToGfzTransformPRXS(this.transform);
            transform.Scale /= scale;

            var value = new TimeExtensionTrigger
            {
                transform = transform,
                option = type,
            };

            return value;
        }

        public void ImportGfz(TimeExtensionTrigger value)
        {
            transform.CopyGfzTransformPRXS(value.transform);
            transform.localScale *= scale;
            type = value.option;
        }
    }
}
