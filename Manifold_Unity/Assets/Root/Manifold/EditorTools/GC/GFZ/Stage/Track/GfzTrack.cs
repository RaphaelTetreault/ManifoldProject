using GameCube.GFZ.Stage;
using Manifold;
using Manifold.IO;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    /// <summary>
    /// Entry point for collecting and generating track data
    /// </summary>
    public class GfzTrack : MonoBehaviour
    {
        [field: SerializeField] public GfzTrackShape StartSegmentShape { get; private set; }
        [field: SerializeField] public GfzTrackShape[] AllRootSegmentShapes { get; private set; }

        public TrackMinHeight TrackMinHeight { get; private set; }
        public TrackLength TrackLength { get; private set; }
        public TrackSegment[] RootSegments { get; private set; }
        public TrackSegment[] AllSegments { get; private set; }
        public Checkpoint[] Checkpoints { get; private set; }
        public TrackNode[] TrackNodes { get; private set; }
        public EmbeddedTrackPropertyArea[] EmbeddedPropertyAreas { get; private set; }
        public CheckpointGrid TrackCheckpointMatrix { get; private set; }
        public GridXZ TrackCheckpointMatrixBoundsXZ { get; private set; }
        public CircuitType CircuitType { get; private set; }


        public void InitTrackData()
        {
            if (this.AllRootSegmentShapes.Length == 0)
                throw new MissingReferenceException($"No references to any {typeof(GfzTrackShape).Name}! Make sure references existin in inspector.");

            foreach (var seg in this.AllRootSegmentShapes)
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
            var allCheckpointsList = new List<Checkpoint>();
            var trackNodes = new List<TrackNode>();

            // AI data
            var trackEmbeddedPropertyAreas = new List<EmbeddedTrackPropertyArea>();


            foreach (var rootSegmentScript in this.AllRootSegmentShapes)
            {
                // Init the GFZ data, add to list
                var currRootSegment = rootSegmentScript.GenerateTrackSegment();
                rootSegments.Add(currRootSegment);

                // Get segments in proper order for binary serialization
                var segmentChildren = currRootSegment.GetGraphSerializableOrder();
                allSegments.AddRange(segmentChildren);

                // Get all checkpoints for this segment
                var checkpoints = CheckpointUtility.CreateCheckpoints2(rootSegmentScript.Segment, true);
                //var checkpoints = rootSegmentScript.Segment.CreateCheckpoints(true);
                allCheckpointsList.AddRange(checkpoints); // checkpoints flat

                //
                var embededPropertyAreas = rootSegmentScript.Segment.GetEmbededPropertyAreas();
                trackEmbeddedPropertyAreas.AddRange(embededPropertyAreas);

                // Compute some metadata based on segments
                trackLength.Value += rootSegmentScript.Segment.GetSegmentLength();

                // Create TrackNodes
                foreach (var checkpoint in checkpoints)
                {
                    var trackNode = new TrackNode()
                    {
                        // Add checkpoints. TODO: support branching
                        Checkpoints = new Checkpoint[] { checkpoint },
                        Segment = currRootSegment,
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
            var allCheckpoints = allCheckpointsList.ToArray();
            trackMinHeight.SetMinHeight(allCheckpoints);

            //
            var checkpointMatrixBoundsXZ = CheckpointGrid.GetMatrixBoundsXZ(allCheckpoints);
            var trackCheckpointMatrix = new CheckpointGrid();
            trackCheckpointMatrix.GenerateIndexes(checkpointMatrixBoundsXZ, allCheckpoints);

            //
            TrackMinHeight = trackMinHeight;
            TrackLength = trackLength;
            RootSegments = rootSegments.ToArray();
            AllSegments = allSegments.ToArray();
            Checkpoints = allCheckpoints;
            TrackNodes = trackNodes.ToArray();
            EmbeddedPropertyAreas = trackEmbeddedPropertyAreas.ToArray();
            TrackCheckpointMatrix = trackCheckpointMatrix;
            TrackCheckpointMatrixBoundsXZ = checkpointMatrixBoundsXZ;
            // TEMP: because it caused me so much strife hunting a bug before...
            CircuitType = CircuitType.ClosedCircuit;
        }

        public void FindChildSegments()
        {
            StartSegmentShape = GetComponentInChildren<GfzTrackShape>(false);
            AllRootSegmentShapes = GetComponentsInChildren<GfzTrackShape>(false);
            Assert.IsTrue(StartSegmentShape is not null);
            Assert.IsTrue(StartSegmentShape == AllRootSegmentShapes[0]);
        }

    }
}
