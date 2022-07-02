using GameCube.GFZ.Stage;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public abstract class GfzSegmentShape : MonoBehaviour
    {
        [field: Header("Segment Base")]
        [field: SerializeField] public GfzTrackSegment Segment { get; private set; }

        public AnimationCurveTRS AnimationCurveTRS => Segment.AnimationCurveTRS;
        


        /// <summary>
        /// Generates the entire mesh for this segment.
        /// </summary>
        /// <returns></returns>
        public abstract Mesh[] GenerateMeshes();

        /// <summary>
        /// Generates the entire TrackSegment tree for this segment.
        /// </summary>
        /// <returns></returns>
        public abstract TrackSegment GenerateTrackSegment();

        /// <summary>
        /// Acquires GfzTrackEmbededProperty from this GameObject instance.
        /// </summary>
        /// <returns></returns>
        public virtual GfzTrackEmbeddedProperty GetEmbeddedProperties()
        {
            var embeddedProperties = GetComponents<GfzTrackEmbeddedProperty>();
            IO.Assert.IsTrue(embeddedProperties.Length == 1);
            return embeddedProperties[0];
        }


        protected virtual void OnValidate()
        {
            if (Segment == null)
            {
                Segment = GetComponent<GfzTrackSegment>();
            }
        }

    }
}
