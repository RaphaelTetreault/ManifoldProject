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


        //public GfzTrackSegment StartSegment
        //{
        //    get => startSegment;
        //    set => startSegment = value;
        //}

        //public GfzTrackSegment[] RootSegments
        //{
        //    get => rootSegments;
        //    set => rootSegments = value;
        //}


        public TrackMinHeight TrackMinHeight { get; private set; }
        public TrackLength TrackLength { get; private set; }
        public TrackSegment[] RootSegments { get; private set; }
        public TrackSegment[] AllSegments { get; private set; }
        public TrackCheckpoint[] Checkpoints { get; private set; }
        public TrackNode[] TrackNodes { get; private set; }
        public SurfaceAttributeArea[] EmbeddedPropertyAreas { get; private set; }
        public TrackCheckpointMatrix TrackCheckpointMatrix { get; private set; }
        public MatrixBoundsXZ TrackCheckpointMatrixBoundsXZ { get; private set; }


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
            var segmentCheckpoints = new List<TrackCheckpoint[]>();
            var checkpoints = new List<TrackCheckpoint>();
            var trackNodes = new List<TrackNode>();
            //
            var trackEmbeddedPropertyAreas = new List<SurfaceAttributeArea>();

            //
            var rootSegmentScripts = this.rootSegments;
            foreach (var rootSegmentScript in rootSegmentScripts)
            {
                // Init the GFZ data, add to list
                rootSegmentScript.InitTrackSegment();
                var rootSegment = rootSegmentScript.TrackSegment;
                rootSegments.Add(rootSegment);

                // Get segments in proper order for binary serialization
                var segmentChildren = rootSegment.GetChildrenArrayPointerOrdered();
                allSegments.AddRange(segmentChildren);

                // Get all checkpoints for this segment
                var x = rootSegmentScript.GetCheckpoints();
                segmentCheckpoints.Add(x);
                checkpoints.AddRange(x);

                //
                var embededPropertyAreas = rootSegmentScript.GetEmbededPropertyAreas();
                trackEmbeddedPropertyAreas.AddRange(embededPropertyAreas);

                // Compute some metadata based on segments
                trackLength.value += rootSegmentScript.GetSegmentLength();

                // Create TrackNodes
                foreach (var checkpoint in checkpoints)
                {
                    var trackNode = new TrackNode()
                    {
                        // Add checkpoints. TODO: support branching
                        checkpoints = new TrackCheckpoint[] { checkpoint },
                        segment = rootSegment,
                    };

                    // Add to master list
                    trackNodes.Add(trackNode);
                }
            }

            // TODO: name this something better
            var checkpointsArray = checkpoints.ToArray();
            trackMinHeight.SetMinHeight(checkpointsArray);

            //
            var checkpointMatrixBoundsXZ = TrackCheckpointMatrix.GetMatrixBoundsXZ(checkpointsArray);
            var trackCheckpointMatrix = new TrackCheckpointMatrix();
            trackCheckpointMatrix.GenerateIndexes(checkpointMatrixBoundsXZ, checkpointsArray);

            // TODO: actually store the damn values!
            //throw new NotImplementedException();

            //
            TrackMinHeight = trackMinHeight;
            TrackLength = trackLength;
            RootSegments = rootSegments.ToArray();
            AllSegments = allSegments.ToArray();
            Checkpoints = checkpointsArray;
            TrackNodes = trackNodes.ToArray();
            EmbeddedPropertyAreas = trackEmbeddedPropertyAreas.ToArray();
            TrackCheckpointMatrix = trackCheckpointMatrix;
            TrackCheckpointMatrixBoundsXZ = checkpointMatrixBoundsXZ;
        }

    }
}
