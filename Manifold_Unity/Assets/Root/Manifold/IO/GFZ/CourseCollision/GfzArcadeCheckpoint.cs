using GameCube.GFZ.CourseCollision;
using UnityEngine;

namespace Manifold.IO.GFZ.CourseCollision
{
    /// <summary>
    /// 
    /// </summary>
    public class GfzArcadeCheckpoint : MonoBehaviour,
        IGfzConvertable<ArcadeCheckpointTrigger>
    {
        /// <summary>
        /// Arcade checkpoint trigger scale (when compared to default Unity cube).
        /// </summary>
        public const float scale = 10f;

        // INSPECTOR FIELDS
        [SerializeField] private ArcadeCheckpointType type;

        // PROPERTIES
        public ArcadeCheckpointType Type
        {
            get => type;
            set => type = value;
        }

        // METHODS
        public ArcadeCheckpointTrigger ExportGfz()
        {
            // Convert unity transform to gfz transform
            var transform = TransformConverter.ToGfzTransformPRXS(this.transform);
            transform.Scale /= scale;

            var value = new ArcadeCheckpointTrigger
            {
                transform = transform,
                type = type,
            };

            return value;
        }

        public void ImportGfz(ArcadeCheckpointTrigger value)
        {
            transform.CopyGfzTransformPRXS(value.transform);
            transform.localScale *= scale;
            type = value.type;
        }
    }
}
