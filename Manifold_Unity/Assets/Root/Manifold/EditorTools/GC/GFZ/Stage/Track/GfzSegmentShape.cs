using GameCube.GFZ.Stage;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public abstract class GfzSegmentShape : MonoBehaviour
    {
        [Header("Segment Base")]
        [SerializeField]
        protected GfzTrackSegment segment;

        public GfzTrackSegment Segment
        {
            get => segment;
            set => segment = value;
        }

        public AnimationCurveTRS AnimationCurveTRS
        {
            get => segment.AnimationCurveTRS;
        }


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
        public virtual GfzTrackEmbededProperty GetEmbeddedProperties()
        {
            var embeddedProperties = GetComponents<GfzTrackEmbededProperty>();
            Assert.IsTrue(embeddedProperties.Length == 1);
            return embeddedProperties[0];
        }


        protected virtual void OnValidate()
        {
            if (segment == null)
            {
                segment = GetComponent<GfzTrackSegment>();
            }
        }

    }
}
