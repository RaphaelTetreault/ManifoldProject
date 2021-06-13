using GameCube.GFZ;
using GameCube.GFZ.CourseCollision;
using UnityEngine;

namespace Manifold.IO.GFZ.CourseCollision
{
    /// <summary>
    /// Component class for Object Paths. Uses cases are: Lightning's lightning,
    /// Outer Space's meteors.
    /// </summary>
    public class GfzObjectPath : MonoBehaviour
    {
        [Header("Gizmos")]
        public float gizmosRadius = 10f;
        public Color gizmosColor = Color.white;
        [Header("Path")]
        public UnityEngine.Transform from;
        public UnityEngine.Transform to;

        private void OnDrawGizmos()
        {
            Gizmos.color = gizmosColor;
            Gizmos.DrawLine(from.position, to.position);
            Gizmos.DrawSphere(from.position, gizmosRadius * .95f);
            Gizmos.DrawWireSphere(from.position, gizmosRadius);
            Gizmos.DrawSphere(to.position, gizmosRadius);
        }

        // METHODS
        public CourseMetadataTrigger ExportGfz(Venue venue)
        {
            // Path object should only exist on Lightning or Outer Space
            var isValidVenue = venue == Venue.Lightning || venue == Venue.OuterSpace;
            if (!isValidVenue)
                throw new System.FormatException($"Invalid venue {venue} for PathObject!");

            // Select which type based on venue
            var metadataType = venue == Venue.Lightning
                ? CourseMetadataType.Lightning_Lightning
                : CourseMetadataType.OuterSpace_Meteor;

            // Get transform values from "from" node
            var transform = TransformConverter.ToGfzTransform(from);
            // The scale field is used as the "to" position
            transform.Scale = to.position;


            var value = new CourseMetadataTrigger
            {
                transform = transform,
                metadataType = metadataType,
            };

            return value;
        }
    }
}
