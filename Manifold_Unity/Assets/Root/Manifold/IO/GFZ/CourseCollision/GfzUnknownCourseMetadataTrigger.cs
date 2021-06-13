using GameCube.GFZ.CourseCollision;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.IO.GFZ.CourseCollision
{
    /// <summary>
    /// 
    /// </summary>
    public class GfzUnknownCourseMetadataTrigger : MonoBehaviour,
        IGfzConvertable<CourseMetadataTrigger>
    {
        /// <summary>
        /// Big Blue Ordeal trigger type scale (when compared to default Unity cube).
        /// </summary>
        public const float scale = 27.5f;

        // METHODS
        public CourseMetadataTrigger ExportGfz()
        {
            // Convert unity transform to gfz transform
            var transform = TransformConverter.ToGfzTransform(this.transform);
            transform.Scale /= scale;

            var value = new CourseMetadataTrigger
            {
                transform = transform,
                metadataType = CourseMetadataType.BigBlueOrdeal,
            };

            return value;
        }

        public void ImportGfz(CourseMetadataTrigger value)
        {
            transform.CopyGfzTransform(value.transform);
            transform.localScale *= scale;
        }

    }
}
