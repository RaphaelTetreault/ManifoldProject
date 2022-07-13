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
    public sealed class GfzTrack : MonoBehaviour
    {
        [field: SerializeField] public GfzTrackSegmentNode FirstRoot { get; private set; }
        [field: SerializeField] public GfzTrackSegmentNode[] AllRoots { get; private set; }

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
            FindChildSegments(false);

            if (this.AllRoots.Length == 0)
                throw new MissingReferenceException($"No references to any {typeof(GfzTrackShape).Name}! Make sure references existin in inspector.");

            foreach (var seg in this.AllRoots)
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


            // Get all ROOT track segments
            foreach (var rootSegmentScript in AllRoots)
            {
                // Ask each root component to create one or more track segments
                var currRootSegment = rootSegmentScript.CreateTrackSegments();
                rootSegments.AddRange(currRootSegment);
            }

            foreach (var rootSegment in rootSegments)
            {
                // Get segments in proper order for binary serialization
                var trackSegmentHierarchy = rootSegment.GetGraphSerializableOrder();
                allSegments.AddRange(trackSegmentHierarchy);

                ////////////////////////////////////////////////////////////////
                // Get all checkpoints for this segment
                var checkpoints = CheckpointUtility.CreateCheckpoints(rootSegment, true);
                allCheckpointsList.AddRange(checkpoints); // checkpoints flat
                // Create TrackNodes
                foreach (var checkpoint in checkpoints)
                {
                    var trackNode = new TrackNode()
                    {
                        // Add checkpoints. TODO: support branching
                        Checkpoints = new Checkpoint[] { checkpoint },
                        RootSegment = rootSegment,
                    };

                    // Add to master list
                    trackNodes.Add(trackNode);
                }
                ///////////////////////////////////////////////////////////////////
                
                // TODO: interface
                var embededPropertyAreas = rootSegment.GetEmbededPropertyAreas();
                trackEmbeddedPropertyAreas.AddRange(embededPropertyAreas);

                // Compute some metadata based on segments
                trackLength.Value += rootSegment.GetSegmentLength();
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

        public void FindChildSegments(bool includeInactive = false)
        {
            FirstRoot = GetComponentInChildren<GfzTrackSegmentNode>(includeInactive);
            AllRoots = GetAllRootSegments(includeInactive);
            Assert.IsTrue(FirstRoot is not null);
            Assert.IsTrue(FirstRoot == AllRoots[0]);
        }

        public GfzTrackSegmentNode[] GetAllRootSegments(bool includeInactive = false)
        {
            var rootSegments = new List<GfzTrackSegmentNode>();
            foreach (var child in transform.GetChildren())
            {
                if (!includeInactive)
                {
                    var isActive = child.gameObject.activeSelf;
                    if (!isActive)
                        continue;
                }

                var rootSegment = child.GetComponent<GfzTrackSegmentNode>();
                var exists = rootSegment != null;
                if (exists)
                    rootSegments.Add(rootSegment);
            }
            return rootSegments.ToArray();
        }

    }
}
