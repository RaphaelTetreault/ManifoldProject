﻿using Manifold.IO;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public abstract class GfzPathSegment : GfzSegmentNode
    {
        [field: SerializeField] public GfzPathSegment Prev { get; set; }
        [field: SerializeField] public GfzPathSegment Next { get; set; }
        [field: SerializeField] public Vector3 StartPosition { get; set; }
        [field: SerializeField] public Vector3 EndPosition { get; set; }


        /// <summary>
        /// The final TRS to use as GFZ track segment
        /// </summary>
        //protected abstract AnimationCurveTRS TrackSegmentAnimationCurveTRS { get; }


        public abstract float GetSegmentLength();

        /// <summary>
        /// Sum of lengths from all previous segments.
        /// </summary>
        /// <returns></returns>
        public float GetDistanceOffset()
        {
            var track = FindObjectOfType<GfzTrack>();
            Assert.IsTrue(track != null, $"track is null.");

            // Call functions to update references
            //track.FindChildSegments();
            //track.AssignContinuity();
            Assert.IsTrue(track.FirstRoot != null, $"track.StartSegment is null.");

            // If we are the start segment, offset is 0
            var startSegment = track.FirstRoot;
            if (this == startSegment)
                return 0f;

            var distanceOffset = 0f;
            var previousSegment = Prev;
            while (previousSegment is not null)
            {
                distanceOffset += previousSegment.GetSegmentLength();

                // If we strumble onto the first segment, stop getting lengths
                // (we are at the start, don't get subsequent previous node)
                if (previousSegment == startSegment)
                    break;

                previousSegment = previousSegment.Prev;

                // If somehow previous segments wrap to this segment, we done goofed
                Assert.IsTrue(previousSegment != this, $"You done goofed. 'track.StartSegment' is probably not set.");
            }
            return distanceOffset;
        }

        public sealed override GameCube.GFZ.Stage.TrackSegment CreateTrackSegment()
        {
            var trs = CopyAnimationCurveTRS(false);

            var trackSegmentRoot = new GameCube.GFZ.Stage.TrackSegment();
            var trackSegmentRZ = new GameCube.GFZ.Stage.TrackSegment();
            trackSegmentRoot.Children = new GameCube.GFZ.Stage.TrackSegment[] { trackSegmentRZ };
            trackSegmentRZ.Children = CreateChildTrackSegments();

            trackSegmentRoot.BranchIndex = trackSegmentRZ.BranchIndex = GetBranchIndex();

            {
                var trsXY = trs.CreateDeepCopy();
                trsXY.Rotation.z = new AnimationCurve();
                trackSegmentRoot.AnimationCurveTRS = trsXY.ToTrackSegment();
            }
            {
                var trsRZ = new AnimationCurveTRS();
                trsRZ.Rotation.z = trs.Rotation.z;
                trackSegmentRZ.AnimationCurveTRS = trsRZ.ToTrackSegment();
            }

            return trackSegmentRoot;
        }

        public void UpdateStartEndPoints()
        {
            var trs = CopyAnimationCurveTRS(false);
            StartPosition = trs.Position.Evaluate(0);
            EndPosition = trs.Position.Evaluate(trs.Position.GetMaxTime());
        }

        public override void InvokeUpdates()
        {
            base.InvokeUpdates();
            UpdateStartEndPoints();
        }

    }
}
