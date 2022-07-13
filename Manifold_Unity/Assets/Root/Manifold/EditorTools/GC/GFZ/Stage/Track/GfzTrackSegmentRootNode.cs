using Manifold.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public abstract class GfzTrackSegmentRootNode : GfzTrackSegmentNode
    {
        [field: SerializeField] public GfzTrackSegmentRootNode Prev { get; set; }
        [field: SerializeField] public GfzTrackSegmentRootNode Next { get; set; }
        [field: SerializeField] public float SegmentLength { get; protected set; } = -1;


        public float GetSegmentLength()
        {
            var root = GetRoot() as GfzTrackSegmentRootNode;
            var segmentLength = root.SegmentLength;
            if (segmentLength <= 0f)
            {
                var msg = "Distance is 0 which is invalid. TRS animation curves must define path.";
                throw new System.ArgumentException(msg);
            }
            return segmentLength;
        }

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
    }
}
