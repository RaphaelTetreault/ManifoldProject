using GameCube.GFZ.CourseCollision;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{

    public class GfzTrack : MonoBehaviour
    {
        [SerializeField] private GfzTrackSegment startSegment;
        [SerializeField] private GfzTrackSegment[] rootSegments;


        public GfzTrackSegment StartSegment
        {
            get => startSegment;
            set => startSegment = value;
        }

        public GfzTrackSegment[] RootSegments
        {
            get => rootSegments;
            set => rootSegments = value;
        }


        public void InitTrackData()
        {
            // Track metadata
            var trackMinHeight = new TrackMinHeight();
            var trackLength = new TrackLength();
            // Get all track segments as GFZ values
            var rootSegments = new List<TrackSegment>();
            // Get all of these segments in an order proper for serialization
            var allSegments = new List<TrackSegment>();
            //
            var checkpoints = new List<TrackCheckpoint>();
            var trackNodes = new List<TrackNode>();
            //
            var trackEmbededPropertyAreas = new List<SurfaceAttributeArea>();

            //
            foreach (var rootSegment in this.rootSegments)
            {
                // Init the GFZ data, add to list
                rootSegment.InitTrackSegment();
                var rootTrackSegment = rootSegment.TrackSegment;
                rootSegments.Add(rootTrackSegment);

                // Get segments in proper order for binary serialization
                var segmentChildren = rootTrackSegment.GetChildrenArrayPointerOrdered();
                allSegments.AddRange(segmentChildren);

                // Get all checkpoints for this segment
                var segmentCheckpoints = rootSegment.GetCheckpoints();
                checkpoints.AddRange(segmentCheckpoints);

                //
                var embededPropertyAreas = rootSegment.GetEmbededPropertyAreas();
                trackEmbededPropertyAreas.AddRange(embededPropertyAreas);

                // Compute some metadata based on segments
                trackLength.value += rootSegment.GetSegmentLength();

                // Create TrackNodes
                foreach (var segmentCheckpoint in segmentCheckpoints)
                {
                    var trackNode = new TrackNode()
                    {
                        // Add checkpoints. TODO: support branching
                        checkpoints = new TrackCheckpoint[] { segmentCheckpoint },
                        segment = rootTrackSegment,
                    };

                    // Add to master list
                    trackNodes.Add(trackNode);
                }
            }

            // TODO: name this something better
            var temp = checkpoints.ToArray();

            trackMinHeight.SetMinHeight(temp);

            //
            var checkpointMatrixBoundsXZ = TrackCheckpointMatrix.GetMatrixBoundsXZ(temp);
            var trackCheckpointMatrix = new TrackCheckpointMatrix();
            trackCheckpointMatrix.GenerateIndexes(checkpointMatrixBoundsXZ, temp);

            // TODO: actually store the damn values!
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
