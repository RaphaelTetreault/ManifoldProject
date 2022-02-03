using GameCube.GFZ.CourseCollision;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    /// <summary>
    /// 
    /// </summary>
    public class GfzUnknownCourseMetadataTrigger : MonoBehaviour,
        IGfzConvertable<MiscellaneousTrigger>
    {
        /// <summary>
        /// Big Blue Ordeal trigger type scale (when compared to default Unity cube).
        /// </summary>
        public const float scale = 27.5f;

        // METHODS
        public MiscellaneousTrigger ExportGfz()
        {
            // Convert unity transform to gfz transform
            var transform = TransformConverter.ToGfzTransformPRXS(this.transform);
            transform.Scale /= scale;

            var value = new MiscellaneousTrigger
            {
                transform = transform,
                metadataType = CourseMetadataType.BigBlueOrdeal,
            };

            return value;
        }

        public void ImportGfz(MiscellaneousTrigger value)
        {
            transform.CopyGfzTransformPRXS(value.transform);
            transform.localScale *= scale;
        }

    }
}
