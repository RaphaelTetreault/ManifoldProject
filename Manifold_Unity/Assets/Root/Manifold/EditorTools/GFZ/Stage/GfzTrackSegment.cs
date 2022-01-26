using GameCube.GFZ.CourseCollision;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    [RequireComponent(typeof(GfzTrackCheckpoints))]
    public abstract class GfzTrackSegment : MonoBehaviour
    {
        // Define delegates
        public delegate void OnEditCallback(GfzTrackSegment value);

        // Events
        public event OnEditCallback onEdit;

        // Fields
        [Header("Track Segment")]
        [SerializeField] protected GfzTrackSegment prev;
        [SerializeField] protected GfzTrackSegment next;
        [SerializeField, ReadOnly] protected GfzTrackCheckpoints checkpoints;
        [SerializeField, ReadOnly] protected GfzTrackEmbededProperty[] embededProperties;

        [Header("Track Curves")]
        [SerializeField] protected AnimationCurve3 position = new AnimationCurve3();
        [SerializeField] protected AnimationCurve3 rotation = new AnimationCurve3();
        [SerializeField] protected AnimationCurve3 scale = new AnimationCurve3();

        // Properties
        public GfzTrackSegment PreviousSegment
        {
            get => prev;
            set => prev = value;
        }

        public GfzTrackSegment NextSegment
        {
            get => next;
            set => next = value;
        }

        public GfzTrackCheckpoints TrackCheckpoints
        {
            get => checkpoints;
            set => checkpoints = value;
        }

        protected TrackSegment trackSegment;
        public TrackSegment TrackSegment => trackSegment;

        // Methods
        public virtual void OnValidate()
        {
            // Get reference if null
            if (checkpoints == null)
                checkpoints = GetComponent<GfzTrackCheckpoints>();

            onEdit?.Invoke(this);
        }

        public abstract void InitTrackSegment();

        public abstract float GetSegmentLength();

        public SurfaceAttributeArea[] GetEmbededPropertyAreas()
        {
            var count = embededProperties.Length;
            var embededPropertyAreas = new SurfaceAttributeArea[count];
            for (int i = 0; i < count; i++)
            {
                embededPropertyAreas[i] = embededProperties[i].GetEmbededProperty();
            }
            return embededPropertyAreas;
        }

    }
}
