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
            Lightning,
            OuterSpace,
        }

        [field: Header("Gizmos")]
        [field: SerializeField] public float GizmosRadius { get; set; } = 10f;
        [field: SerializeField] public Color GizmosColor { get; set; } = Color.white;
        
        [field: Header("Path")]
        [field: SerializeField] public Transform From { get; set; }
        [field: SerializeField] public Transform To { get; set; }
        
        [field:Header("Venue")]
        [field: SerializeField] public PathObjectVenue OobjectVenue { get; private set; }


        private void OnDrawGizmos()
        {
            Gizmos.color = GizmosColor;
            Gizmos.DrawLine(From.position, To.position);
            Gizmos.DrawSphere(From.position, GizmosRadius * .95f);
            Gizmos.DrawWireSphere(From.position, GizmosRadius);
            Gizmos.DrawSphere(To.position, GizmosRadius);
        }

        // METHODS
        public MiscellaneousTrigger ExportGfz()
        {
            var metadataType = OobjectVenue == PathObjectVenue.Lightning
                ? CourseMetadataType.Lightning_Lightning
                : CourseMetadataType.OuterSpace_Meteor;

            // Get transform values from "from" node
            var transform = TransformConverter.ToGfzTransformTRXS(From, Space.World);
            // The scale field is used as the "to" position
            transform.Scale = To.position;

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
