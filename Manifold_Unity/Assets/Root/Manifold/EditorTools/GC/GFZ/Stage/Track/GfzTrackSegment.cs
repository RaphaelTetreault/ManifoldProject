using GameCube.GFZ.Stage;
using Manifold.IO;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    [ExecuteInEditMode]
    public sealed class GfzTrackSegment : MonoBehaviour
    {
        [field: Header("Track Segment Order")]
        // TODO: in the future, make readonly and assign on export based on order (from GfzTrack script?)
        [field: SerializeField] public GfzTrackSegment PreviousSegment { get; internal set; }
        [field: SerializeField] public GfzTrackSegment NextSegment { get; internal set; }

        [field: Header("Checkpoints")]
        [field: SerializeField] public float MetersPerCheckpoint { get; private set; } = 100f;

        [field: Header("Track Curves")]
        [field: SerializeField] public SegmentPathGenerator SegmentPathGenerator { get; private set; }
        [field: SerializeField] public AnimationCurveTRS AnimationCurveTRS { get; private set; } = new();
        [field: SerializeField] public HierarchichalAnimationCurveTRS TrsHierarchy { get; private set; } = new();



        public float GetSegmentLength()
        {
            // 2022/01/31: current work assumes min and max of 0 and 1
            var maxTime = AnimationCurveTRS.GetMaxTime();
            Assert.IsTrue(maxTime == 1);
            // TODO: get min time, assert

            var distance = AnimationCurveTRS.GetDistanceBetweenRepeated(0, 1);
            if (distance <= 0f)
            {
                var msg = "Distance is 0 which is invalid. TRS animation curves must define path.";
                throw new System.ArgumentException(msg);
            }
            return distance;
        }
        /// <summary>
        /// Sum of lengths from all previous segments.
        /// </summary>
        /// <returns></returns>
        public float GetDistanceOffset()
        {
            var track = FindObjectOfType<GfzTrack>();
            Assert.IsTrue(track != null, $"track is null.");
            Assert.IsTrue(track.StartSegmentShape != null, $"track.StartSegment is null.");
            Assert.IsTrue(track.StartSegmentShape.Segment != null, $"track.StartSegment.Segment is null.");

            // If we are the start segment, offset is 0
            var startSegment = track.StartSegmentShape.Segment;
            if (this == startSegment)
                return 0f;

            var distanceOffset = 0f;
            var previousSegment = PreviousSegment;
            while (previousSegment is not null)
            {
                distanceOffset += previousSegment.GetSegmentLength();

                // If we strumble onto the first segment, stop getting lengths
                // (we are at the start, don't get subsequent previous node)
                if (previousSegment == startSegment)
                    break;

                previousSegment = previousSegment.PreviousSegment;

                // If somehow previous segments wrap to this segment, we done goofed
                Assert.IsTrue(previousSegment != this, $"You done goofed. 'track.StartSegment' is probably not set.");
            }
            return distanceOffset;
        }


        public EmbeddedTrackPropertyArea[] GetEmbededPropertyAreas()
        {
            // Get all properties on self and children.
            var embededProperties = GetComponentsInChildren<GfzTrackEmbeddedProperty>();

            // Iterate over collection
            var count = embededProperties.Length;
            var embededPropertyAreas = new EmbeddedTrackPropertyArea[count + 1];
            for (int i = 0; i < count; i++)
            {
                embededPropertyAreas[i] = embededProperties[i].GetEmbededProperty();
            }
            embededPropertyAreas[count] = EmbeddedTrackPropertyArea.Terminator();
            return embededPropertyAreas;
        }

        public TrackSegment GetTrackSegment()
        {
            var trackSegment = new TrackSegment();

            trackSegment.LocalPosition = transform.localPosition;
            trackSegment.LocalRotation = transform.localRotation.eulerAngles;
            trackSegment.LocalScale = transform.localScale;

            // Get animation data
            trackSegment.AnimationCurveTRS = AnimationCurveTRS.ToTrackSegment();
            // Move rotation.z to child node, otherwise matrix is messed up with x, y, and z rotations
            trackSegment.Children = new TrackSegment[] { new() };
            trackSegment.Children[0].AnimationCurveTRS.RotationZ = trackSegment.AnimationCurveTRS.RotationZ;
            // Make root rotation.z empty (null makes errors)
            trackSegment.AnimationCurveTRS.RotationZ = new();

            // TODO: currently hardcoded
            // Well, could be good actually. Other tracks can simply override this
            trackSegment.SegmentType = TrackSegmentType.IsMatrix;
            trackSegment.Children[0].SegmentType = TrackSegmentType.IsTrack;

            // :clap:
            return trackSegment;
        }

        public bool IsContinuousToNext()
        {
            if (PreviousSegment is null)
                throw new System.NullReferenceException($"{nameof(PreviousSegment)} node is not set!");

            var selfEndMaxTime = AnimationCurveTRS.GetMaxTime();
            var selfEnd = AnimationCurveTRS.Position.Evaluate(selfEndMaxTime);
            var nextStart = PreviousSegment.AnimationCurveTRS.Position.Evaluate(0);
            bool isContinuousToNext = Vector3.Distance(selfEnd, nextStart) < 0.01f; // 1cm
            return isContinuousToNext;
        }

        public bool IsContinuousFromPrevious()
        {
            if (NextSegment is null)
                throw new System.NullReferenceException($"{nameof(NextSegment)} node is not set!");

            var prevEndMaxTime = PreviousSegment.AnimationCurveTRS.GetMaxTime();
            var prevEnd = PreviousSegment.AnimationCurveTRS.Position.Evaluate(prevEndMaxTime);
            var selfStart = AnimationCurveTRS.Position.Evaluate(0);
            bool isContinuousFromPrev = Vector3.Distance(selfStart, prevEnd) < 0.01f; // 1cm
            return isContinuousFromPrev;
        }


        private void Awake()
        {
            if (SegmentPathGenerator is not null)
                return;

            SegmentPathGenerator.OnEdited += GenerateAnimationCurves;
        }

        private void OnDestroy()
        {
            if (SegmentPathGenerator is not null)
                return;

            SegmentPathGenerator.OnEdited -= GenerateAnimationCurves;
        }

        private void OnValidate()
        {
            if (SegmentPathGenerator is null)
            {
                SegmentPathGenerator = GetComponent<SegmentPathGenerator>();
            }
        }

        /// <summary>
        /// Creates new animation TRS from segment generator
        /// </summary>
        public void GenerateAnimationCurves()
        {
            AnimationCurveTRS = SegmentPathGenerator.GenerateAnimationCurveTRS();
        }

    }
}
