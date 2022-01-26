using GameCube.GFZ.CourseCollision;
using Manifold.IO;
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


        protected TrackSegment trackSegment;
        public TrackSegment TrackSegment => trackSegment;

        // Methods
        public virtual void OnValidate()
        {
            onEdit?.Invoke(this);
        }

        public abstract void InitTrackSegment();

        public abstract float GetSegmentLength();

        public SurfaceAttributeArea[] GetEmbededPropertyAreas()
        {
            // Get all properties on self and children.
            var embededProperties = GetComponentsInChildren<GfzTrackEmbededProperty>();

            // Iterate over collection
            var count = embededProperties.Length;
            var embededPropertyAreas = new SurfaceAttributeArea[count];
            for (int i = 0; i < count; i++)
            {
                embededPropertyAreas[i] = embededProperties[i].GetEmbededProperty();
            }

            return embededPropertyAreas;
        }

        public TrackCheckpoint[] GetCheckpoints()
        {
            // Collect all possible checkpoint scripts on object
            var checkpointScripts = GetComponentsInChildren<GfzTrackCheckpoints>();
            // Make sure there is only one
            Assert.IsTrue(checkpointScripts.Length == 0);
            var checkpointScript = checkpointScripts[0];

            // Get the gfz value for it, return
            var checkpoints = checkpointScript.GetCheckpoints();
            return checkpoints;
        }
    }
}
