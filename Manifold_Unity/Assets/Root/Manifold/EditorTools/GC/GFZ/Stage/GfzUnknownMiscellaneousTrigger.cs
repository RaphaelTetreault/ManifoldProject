using GameCube.GFZ.Stage;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public class GfzUnknownMiscellaneousTrigger : MonoBehaviour,
        IGfzConvertable<MiscellaneousTrigger>
    {
        /// <summary>
        /// Big Blue Ordeal trigger type scale (when compared to default Unity unit cube).
        /// </summary>
        public const float scale = 27.5f;

        // METHODS
        public MiscellaneousTrigger ExportGfz()
        {
            // Convert unity transform to gfz transform
            var transform = TransformConverter.ToGfzTransformTRXS(this.transform, Space.World);
            transform.Scale /= scale;

            var value = new MiscellaneousTrigger
            {
                Transform = transform,
                MetadataType = CourseMetadataType.BigBlueOrdeal,
            };

            return value;
        }

        public void ImportGfz(MiscellaneousTrigger value)
        {
            transform.CopyGfzTransform(value.Transform, Space.Self);
            transform.localScale *= scale;
        }

    }
}
