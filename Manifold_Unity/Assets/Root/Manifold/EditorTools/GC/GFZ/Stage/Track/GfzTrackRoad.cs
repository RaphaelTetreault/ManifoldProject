using GameCube.GFZ.Stage;
using Manifold.EditorTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public class GfzTrackRoad : GfzSegmentShape,
        IRailSegment
    {
        [Header("Gizmos")]
        [SerializeField]
        private bool doGizmos = true;

        [Header("Road Properties")]
        [Min(0f)]
        [SerializeField]
        private float railHeightLeft = 5f;

        [Min(0f)]
        [SerializeField]
        private float railHeightRight = 5f;

        public float RailHeightLeft => railHeightLeft;
        public float RailHeightRight => railHeightRight;


        public override Mesh[] GenerateMeshes()
        {
            throw new System.NotImplementedException();
        }

        public override TrackSegment GenerateTrackSegment()
        {
            var trackSegment = Segment.GetTrackSegment();
            var lastNode = trackSegment.Children[0];

            // Override the rail properies
            IO.Assert.IsTrue(lastNode.SegmentType == TrackSegmentType.IsTrack);

            // Rail height
            lastNode.SetRails(railHeightLeft, railHeightRight);

            //
            return trackSegment;
        }

    }
}
