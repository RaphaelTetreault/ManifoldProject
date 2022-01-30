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
        public Checkpoint[] Checkpoints { get; private set; }
        public TrackNode[] TrackNodes { get; private set; }
        public EmbeddedTrackPropertyArea[] EmbeddedPropertyAreas { get; private set; }
        public TrackCheckpointGrid TrackCheckpointMatrix { get; private set; }
        public GridXZ TrackCheckpointMatrixBoundsXZ { get; private set; }
        public CircuitType CircuitType { get; private set; }


        public void InitTrackData()
        {
            if (this.rootSegments.Length == 0)
                throw new MissingReferenceException($"No references to any {typeof(GfzTrackSegment).Name}! Make sure references existin in inspector.");

            // Track metadata
            var trackMinHeight = new TrackMinHeight();
            var trackLength = new TrackLength();
            // Get all track segments as GFZ values
            var rootSegments = new List<TrackSegment>();
            // Get all of these segments in an order proper for serialization
            var allSegments = new List<TrackSegment>();
            //
            var segmentCheckpoints = new List<Checkpoint[]>();
            var checkpoints = new List<Checkpoint>();
            var trackNodes = new List<TrackNode>();
            //
            var trackEmbeddedPropertyAreas = new List<EmbeddedTrackPropertyArea>();

            //
            var rootSegmentScripts = this.rootSegments;
            foreach (var rootSegmentScript in rootSegmentScripts)
            {
                // Init the GFZ data, add to list
                var rootSegment = rootSegmentScript.GenerateTrackSegment();
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
                        checkpoints = new Checkpoint[] { checkpoint },
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
            var checkpointMatrixBoundsXZ = TrackCheckpointGrid.GetMatrixBoundsXZ(checkpointsArray);
            var trackCheckpointMatrix = new TrackCheckpointGrid();
            trackCheckpointMatrix.GenerateIndexes(checkpointMatrixBoundsXZ, checkpointsArray);

            // TODO: actually store the damn values!
            //throw new NotImplementedException();

            // Set circuit type depending on if 
            var lastSegmentIndex = rootSegmentScripts.Length - 1;
            CircuitType = rootSegmentScripts[lastSegmentIndex].NextSegment != null
                ? CircuitType.ClosedCircuit
                : CircuitType.OpenCircuit;

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
