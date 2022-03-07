using GameCube.GFZ.Stage;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public class GfzTrack : MonoBehaviour
    {
        [SerializeField] private GfzSegmentShape startSegment;
        [SerializeField] private GfzSegmentShape[] rootSegments;



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

            foreach (var seg in this.rootSegments)
            {
                if (seg == null)
                {
                    throw new Exception($"Segment in {name} is null! Cannot export null segments.");
                }

                if (!seg.isActiveAndEnabled)
                {
                    throw new Exception($"Segment {seg.name} is disabled! Can only export active segments.");
                }
            }

            // Track metadata
            var trackMinHeight = new TrackMinHeight();
            var trackLength = new TrackLength();

            // Track Animation Data
            // Get all track segments as GFZ values
            var rootSegments = new List<TrackSegment>();
            // Get all of these segments in an order proper for serialization
            var allSegments = new List<TrackSegment>();

            // TrackNodes, Checkpoints
            var allCheckpoints = new List<Checkpoint>();
            var trackNodes = new List<TrackNode>();

            // AI data
            var trackEmbeddedPropertyAreas = new List<EmbeddedTrackPropertyArea>();


            foreach (var rootSegmentScript in this.rootSegments)
            {
                // Init the GFZ data, add to list
                var currRootSegment = rootSegmentScript.GenerateTrackSegment();
                rootSegments.Add(currRootSegment);

                // Get segments in proper order for binary serialization
                var segmentChildren = currRootSegment.GetGraphSerializableOrder();
                allSegments.AddRange(segmentChildren);

                // Get all checkpoints for this segment
                var checkpoints = rootSegmentScript.Segment.CreateCheckpoints(true);
                allCheckpoints.AddRange(checkpoints); // checkpoints flat

                //
                var embededPropertyAreas = rootSegmentScript.Segment.GetEmbededPropertyAreas();
                trackEmbeddedPropertyAreas.AddRange(embededPropertyAreas);

                // Compute some metadata based on segments
                trackLength.value += rootSegmentScript.Segment.GetSegmentLength();

                // Create TrackNodes
                foreach (var checkpoint in checkpoints)
                {
                    var trackNode = new TrackNode()
                    {
                        // Add checkpoints. TODO: support branching
                        checkpoints = new Checkpoint[] { checkpoint },
                        segment = currRootSegment,
                    };

                    // Add to master list
                    trackNodes.Add(trackNode);
                }
            }

            //// Set circuit type depending on if 
            //var lastSegmentIndex = this.rootSegments.Length - 1;
            //CircuitType = this.rootSegments[lastSegmentIndex].Segment.NextSegment != null
            //    ? CircuitType.OpenCircuit
            //    : CircuitType.ClosedCircuit;

            // TODO: name this something better
            var checkpointsArray = allCheckpoints.ToArray();
            trackMinHeight.SetMinHeight(checkpointsArray);

            //
            var checkpointMatrixBoundsXZ = TrackCheckpointGrid.GetMatrixBoundsXZ(checkpointsArray);
            var trackCheckpointMatrix = new TrackCheckpointGrid();
            trackCheckpointMatrix.GenerateIndexes(checkpointMatrixBoundsXZ, checkpointsArray);

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
            // TEMP: because it caused me so much strife hunting a bug before...
            CircuitType = CircuitType.ClosedCircuit;
        }

    }
}
