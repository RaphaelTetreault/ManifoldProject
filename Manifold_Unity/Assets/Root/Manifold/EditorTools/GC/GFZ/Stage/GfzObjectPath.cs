using GameCube.GFZ;
using GameCube.GFZ.Stage;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    /// <summary>
    /// Component class for Object Paths. Uses cases are: Lightning's lightning,
    /// Outer Space's meteors.
    /// </summary>
    public class GfzObjectPath : MonoBehaviour,
        IGfzConvertable<MiscellaneousTrigger>
    {
        public enum PathObjectVenue
        {
            Lightning = 1,
            OuterSpace,
        }

        [Header("Gizmos")]
        public float gizmosRadius = 10f;
        public Color gizmosColor = Color.white;
        [Header("Path")]
        public Transform from;
        public Transform to;
        [Header("Venue")]
        public PathObjectVenue objectVenue;

        private void OnDrawGizmos()
        {
            Gizmos.color = gizmosColor;
            Gizmos.DrawLine(from.position, to.position);
            Gizmos.DrawSphere(from.position, gizmosRadius * .95f);
            Gizmos.DrawWireSphere(from.position, gizmosRadius);
            Gizmos.DrawSphere(to.position, gizmosRadius);
        }

        // METHODS
        public MiscellaneousTrigger ExportGfz()
        {
            //// Path object should only exist on Lightning or Outer Space
            //var isValidVenue = venue == Venue.Lightning || venue == Venue.OuterSpace;
            //if (!isValidVenue)
            //    throw new System.FormatException($"Invalid venue {venue} for PathObject!");

            //// Select which type based on venue
            //var metadataType = venue == Venue.Lightning
            //    ? CourseMetadataType.Lightning_Lightning
            //    : CourseMetadataType.OuterSpace_Meteor;

            var metadataType = objectVenue == PathObjectVenue.Lightning
                ? CourseMetadataType.Lightning_Lightning
                : CourseMetadataType.OuterSpace_Meteor;

            // Get transform values from "from" node
            var transform = TransformConverter.ToGfzTransformTRXS(from, Space.World);
            // The scale field is used as the "to" position
            transform.Scale = to.position;


            var value = new MiscellaneousTrigger
            {
                Transform = transform,
                MetadataType = metadataType,
            };

            return value;
        }

        public void ImportGfz(MiscellaneousTrigger value)
        {
            throw new System.NotImplementedException();
        }
    }
}
