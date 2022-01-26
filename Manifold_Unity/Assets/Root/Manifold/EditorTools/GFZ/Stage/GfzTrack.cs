using GameCube.GFZ.CourseCollision;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{

    public class GfzTrack : MonoBehaviour
    {
        [SerializeField] private GfzTrackSegment startSegment;
        [SerializeField] private GfzTrackSegment[] segments;


        public GfzTrackSegment StartSegment
        {
            get => startSegment;
            set => startSegment = value;
        }

        public GfzTrackSegment[] Segments
        {
            get => segments;
            set => segments = value;
        }


        public void InitTrackData()
        {
            // Get all segments
            //      Store root segments (used for TrackNode[])
            //      Store all segments (that satisfies ArrayPointer serialization)
            // Calc the length of all segments
            // FREE: TrackLength
            // Get all checkpoints
            // Using checkpoints, create TrackNode[], bind segment with checkpoints
            // Calc matrix bounds for checkpoints
            // Calc TrackMinHeight
            // foreach (segment) get embedded property => array

            throw new NotImplementedException();
        }

        public TrackSegment[] GetRootSegments()
        {
            throw new NotImplementedException();
        }

        public TrackSegment[] GetAllSegments()
        {
            throw new NotImplementedException();
        }

        public TrackNode[] GetTrackNodes()
        {
            throw new NotImplementedException();
        }

        public TrackCheckpointMatrix GetCheckpointMatrix()
        {
            throw new NotImplementedException();
        }

        public MatrixBoundsXZ GetCheckpointMatrixBoundsXZ()
        {
            throw new NotImplementedException();
        }

        public TrackMinHeight GetTrackMinHeight()
        {
            throw new NotImplementedException();
        }

        public TrackLength GetTrackLength()
        {
            throw new NotImplementedException();
        }

        public SurfaceAttributeArea[] GetEmbeddedPropertyAreas()
        {
            // TODO: actually collect the data! In the meantime, this will suffice.
            return SurfaceAttributeArea.DefaultArray();
        }
    }
}
